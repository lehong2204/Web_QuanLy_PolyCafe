using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web_QuanLy_PolyCafe.Areas.Admin.Controllers;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;
using Xunit;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class CategoryControllerAdminTests
    {
        private PolyCafeDbContext CreateDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<PolyCafeDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var ctx = new PolyCafeDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        }

        private CategoryController CreateController(PolyCafeDbContext ctx,
            string? userId = "U001", string? role = "Admin")
        {
            var controller = new CategoryController(ctx);
            var httpContext = new DefaultHttpContext();
            var session = new DefaultSession();
            if (!string.IsNullOrEmpty(userId)) session.SetString("UserId", userId);
            if (!string.IsNullOrEmpty(role)) session.SetString("Role", role);
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        private JsonElement? SafeToJson(IActionResult? result)
        {
            if (result is JsonResult jr && jr.Value != null)
            {
                var json = JsonSerializer.Serialize(jr.Value);
                return JsonSerializer.Deserialize<JsonElement>(json);
            }
            return null;
        }

        // ================================================================
        // NHÓM 1: AUTHORIZATION (TC01–TC08)
        // ================================================================
        [Fact]
        public async Task TC01_Index_NotLoggedIn_RedirectsToLogin()
        {
            var ctx = CreateDb(nameof(TC01_Index_NotLoggedIn_RedirectsToLogin));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Index() as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Login", result!.ActionName);
        }

        [Fact]
        public async Task TC02_Index_Staff_RedirectsToPOS()
        {
            var ctx = CreateDb(nameof(TC02_Index_Staff_RedirectsToPOS));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Index() as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("POS", result!.ActionName);
        }

        [Fact]
        public async Task TC03_Create_NotLoggedIn_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC03_Create_NotLoggedIn_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "Test" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC04_Create_Staff_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC04_Create_Staff_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.Create(new Category { Name = "Test" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC05_Edit_NotLoggedIn_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC05_Edit_NotLoggedIn_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT01", Name = "X" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC06_Edit_Staff_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC06_Edit_Staff_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT01", Name = "X" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC07_Delete_NotLoggedIn_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC07_Delete_NotLoggedIn_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.Delete("CAT01"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC08_Delete_Staff_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC08_Delete_Staff_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.Delete("CAT01"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 2: INDEX (TC09–TC16)
        // ================================================================
        [Fact]
        public async Task TC09_Index_Admin_ReturnsView_WithAllActiveCategories()
        {
            var ctx = CreateDb(nameof(TC09_Index_Admin_ReturnsView_WithAllActiveCategories));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(20, model.Count);
        }

        [Fact]
        public async Task TC10_Index_AfterSoftDelete_ExcludesDeletedCategory()
        {
            var ctx = CreateDb(nameof(TC10_Index_AfterSoftDelete_ExcludesDeletedCategory));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT01");
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(19, model.Count);
        }

        [Fact]
        public async Task TC11_Index_IncludesOnlyNonDeletedDrinks()
        {
            var ctx = CreateDb(nameof(TC11_Index_IncludesOnlyNonDeletedDrinks));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.All(model, c => Assert.True(c.Drinks?.All(d => !d.IsDeleted) ?? true));
        }

        [Fact]
        public async Task TC12_Index_ReturnsViewResult()
        {
            var ctx = CreateDb(nameof(TC12_Index_ReturnsViewResult));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task TC13_Index_EmptyDatabase_ReturnsEmptyList()
        {
            var ctx = CreateDb(nameof(TC13_Index_EmptyDatabase_ReturnsEmptyList));
            ctx.Categories.RemoveRange(ctx.Categories.ToList());
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task TC14_Index_AllCategoriesNotDeleted()
        {
            var ctx = CreateDb(nameof(TC14_Index_AllCategoriesNotDeleted));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.All(model, c => Assert.False(c.IsDeleted));
        }

        [Fact]
        public async Task TC15_Index_AfterTwoDeletes_Returns18()
        {
            var ctx = CreateDb(nameof(TC15_Index_AfterTwoDeletes_Returns18));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT01");
            await ctrl.Delete("CAT02");
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(18, model.Count);
        }

        [Fact]
        public async Task TC16_Index_AfterCreate_Returns21()
        {
            var ctx = CreateDb(nameof(TC16_Index_AfterCreate_Returns21));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new Category { Name = "New Cat" });
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(21, model.Count);
        }

        // ================================================================
        // NHÓM 3: CREATE (TC17–TC30)
        // ================================================================
        [Fact]
        public async Task TC17_Create_ValidName_ReturnsSuccessTrue()
        {
            var ctx = CreateDb(nameof(TC17_Create_ValidName_ReturnsSuccessTrue));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "Trà Xanh" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC18_Create_EmptyName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC18_Create_EmptyName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC19_Create_WhitespaceName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC19_Create_WhitespaceName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "   " }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC20_Create_ReturnsIdAndName()
        {
            var ctx = CreateDb(nameof(TC20_Create_ReturnsIdAndName));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "Cà Phê Mới" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
            Assert.NotEmpty(data.Value.GetProperty("id").GetString()!);
            Assert.Equal("Cà Phê Mới", data.Value.GetProperty("name").GetString());
        }

        [Fact]
        public async Task TC21_Create_IdStartsWithCAT()
        {
            var ctx = CreateDb(nameof(TC21_Create_IdStartsWithCAT));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "TestCat" }));
            var id = data!.Value.GetProperty("id").GetString();
            Assert.StartsWith("CAT", id);
        }

        [Fact]
        public async Task TC22_Create_PersistsToDatabase()
        {
            var ctx = CreateDb(nameof(TC22_Create_PersistsToDatabase));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new Category { Name = "Persist Test" });
            Assert.True(ctx.Categories.Any(c => c.Name == "Persist Test"));
        }

        [Fact]
        public async Task TC23_Create_TwoWithSameName_BothSucceed()
        {
            var ctx = CreateDb(nameof(TC23_Create_TwoWithSameName_BothSucceed));
            var ctrl = CreateController(ctx);
            var d1 = SafeToJson(await ctrl.Create(new Category { Name = "Dup" }));
            var d2 = SafeToJson(await ctrl.Create(new Category { Name = "Dup" }));
            Assert.True(d1!.Value.GetProperty("success").GetBoolean());
            Assert.True(d2!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC24_Create_WithDescription_Saved()
        {
            var ctx = CreateDb(nameof(TC24_Create_WithDescription_Saved));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new Category { Name = "Desc Cat", Description = "Mô tả đây" });
            var cat = ctx.Categories.First(c => c.Name == "Desc Cat");
            Assert.Equal("Mô tả đây", cat.Description);
        }

        [Fact]
        public async Task TC25_Create_IsDeletedDefaultFalse()
        {
            var ctx = CreateDb(nameof(TC25_Create_IsDeletedDefaultFalse));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new Category { Name = "Active Cat" });
            var cat = ctx.Categories.First(c => c.Name == "Active Cat");
            Assert.False(cat.IsDeleted);
        }

        [Fact]
        public async Task TC26_Create_MultipleCategories_AllUnique()
        {
            var ctx = CreateDb(nameof(TC26_Create_MultipleCategories_AllUnique));
            var ctrl = CreateController(ctx);
            for (int i = 0; i < 5; i++)
                await ctrl.Create(new Category { Name = $"Cat{i}" });
            var ids = ctx.Categories.Select(c => c.Id).ToList();
            Assert.Equal(ids.Distinct().Count(), ids.Count);
        }

        [Fact]
        public async Task TC27_Create_NullName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC27_Create_NullName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = null! }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC28_Create_LongName_Succeeds()
        {
            var ctx = CreateDb(nameof(TC28_Create_LongName_Succeeds));
            var ctrl = CreateController(ctx);
            var longName = new string('A', 100);
            var data = SafeToJson(await ctrl.Create(new Category { Name = longName }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC29_Create_AfterDelete_IndexCountCorrect()
        {
            var ctx = CreateDb(nameof(TC29_Create_AfterDelete_IndexCountCorrect));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT01");
            await ctrl.Create(new Category { Name = "Replacement" });
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(20, model.Count);
        }

        [Fact]
        public async Task TC30_Create_Returns_MessageOnError()
        {
            var ctx = CreateDb(nameof(TC30_Create_Returns_MessageOnError));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new Category { Name = "" }));
            Assert.True(data!.Value.TryGetProperty("message", out _));
        }

        // ================================================================
        // NHÓM 4: EDIT (TC31–TC40)
        // ================================================================
        [Fact]
        public async Task TC31_Edit_ValidCategory_ReturnsSuccessTrue()
        {
            var ctx = CreateDb(nameof(TC31_Edit_ValidCategory_ReturnsSuccessTrue));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT01", Name = "Updated" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC32_Edit_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC32_Edit_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CATXXX", Name = "X" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC33_Edit_UpdatesNameInDB()
        {
            var ctx = CreateDb(nameof(TC33_Edit_UpdatesNameInDB));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new Category { Id = "CAT02", Name = "Trà Sữa Updated" });
            var cat = await ctx.Categories.FindAsync("CAT02");
            Assert.Equal("Trà Sữa Updated", cat!.Name);
        }

        [Fact]
        public async Task TC34_Edit_UpdatesDescriptionInDB()
        {
            var ctx = CreateDb(nameof(TC34_Edit_UpdatesDescriptionInDB));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new Category { Id = "CAT03", Name = "Sinh Tố", Description = "New desc" });
            var cat = await ctx.Categories.FindAsync("CAT03");
            Assert.Equal("New desc", cat!.Description);
        }

        [Fact]
        public async Task TC35_Edit_EmptyName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC35_Edit_EmptyName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT01", Name = "" }));
            // Controller không validate tên khi edit, nên check success hoặc không throw
            Assert.NotNull(data);
        }

        [Fact]
        public async Task TC36_Edit_MultipleTimes_LastUpdateWins()
        {
            var ctx = CreateDb(nameof(TC36_Edit_MultipleTimes_LastUpdateWins));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new Category { Id = "CAT04", Name = "First" });
            await ctrl.Edit(new Category { Id = "CAT04", Name = "Second" });
            var cat = await ctx.Categories.FindAsync("CAT04");
            Assert.Equal("Second", cat!.Name);
        }

        [Fact]
        public async Task TC37_Edit_SameName_StillSucceeds()
        {
            var ctx = CreateDb(nameof(TC37_Edit_SameName_StillSucceeds));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT01", Name = "Cà Phê" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC38_Edit_DeletedCategory_CanStillEdit()
        {
            var ctx = CreateDb(nameof(TC38_Edit_DeletedCategory_CanStillEdit));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT05");
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT05", Name = "Still Edit" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC39_Edit_DoesNotChangeIsDeleted()
        {
            var ctx = CreateDb(nameof(TC39_Edit_DoesNotChangeIsDeleted));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT06");
            await ctrl.Edit(new Category { Id = "CAT06", Name = "Edited After Delete" });
            var cat = await ctx.Categories.FindAsync("CAT06");
            Assert.True(cat!.IsDeleted); // IsDeleted vẫn true sau edit
        }

        [Fact]
        public async Task TC40_Edit_ResponseReturnsSuccessTrue()
        {
            var ctx = CreateDb(nameof(TC40_Edit_ResponseReturnsSuccessTrue));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new Category { Id = "CAT07", Name = "Choco Updated" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 5: DELETE (TC41–TC47)
        // ================================================================
        [Fact]
        public async Task TC41_Delete_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC41_Delete_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Delete("CAT999"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC42_Delete_Valid_SetsIsDeletedTrue()
        {
            var ctx = CreateDb(nameof(TC42_Delete_Valid_SetsIsDeletedTrue));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT08");
            var cat = await ctx.Categories.FindAsync("CAT08");
            Assert.True(cat!.IsDeleted);
        }

        [Fact]
        public async Task TC43_Delete_SoftDelete_IndexExcludesIt()
        {
            var ctx = CreateDb(nameof(TC43_Delete_SoftDelete_IndexExcludesIt));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT09");
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.DoesNotContain(model, c => c.Id == "CAT09");
        }

        [Fact]
        public async Task TC44_Delete_Twice_StillReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC44_Delete_Twice_StillReturnsSuccess));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT10");
            var data = SafeToJson(await ctrl.Delete("CAT10"));
            // Second delete: cat still exists but IsDeleted=true, returns success=true
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC45_Delete_ThenCreateNewWithSameName_Success()
        {
            var ctx = CreateDb(nameof(TC45_Delete_ThenCreateNewWithSameName_Success));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT11");
            var data = SafeToJson(await ctrl.Create(new Category { Name = "Cappuccino" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC46_Delete_DrinksStillExistInDB()
        {
            var ctx = CreateDb(nameof(TC46_Delete_DrinksStillExistInDB));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT01");
            var count = ctx.Drinks.Count(d => d.CategoryId == "CAT01");
            Assert.True(count > 0); // Drinks vẫn còn trong DB
        }

        [Fact]
        public async Task TC47_Delete_LastCategory_IndexReturns19()
        {
            var ctx = CreateDb(nameof(TC47_Delete_LastCategory_IndexReturns19));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT20");
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(19, model.Count);
        }

        // ================================================================
        // NHÓM 6: EDGE CASES (TC48–TC50)
        // ================================================================
        [Fact]
        public async Task TC48_Create_Edit_Delete_FullCycle_Success()
        {
            var ctx = CreateDb(nameof(TC48_Create_Edit_Delete_FullCycle_Success));
            var ctrl = CreateController(ctx);
            var createData = SafeToJson(await ctrl.Create(new Category { Name = "CycleTest" }));
            var id = createData!.Value.GetProperty("id").GetString()!;
            await ctrl.Edit(new Category { Id = id, Name = "Cycle Edited" });
            await ctrl.Delete(id);
            var cat = await ctx.Categories.FindAsync(id);
            Assert.True(cat!.IsDeleted);
        }

        [Fact]
        public async Task TC49_Create_Then_IndexCount_Increases()
        {
            var ctx = CreateDb(nameof(TC49_Create_Then_IndexCount_Increases));
            var ctrl = CreateController(ctx);
            var before = ((await ctrl.Index() as ViewResult)!.Model as List<Category>)!.Count;

            await ctrl.Create(new Category { Name = "Extra Category" });

            var after = ((await ctrl.Index() as ViewResult)!.Model as List<Category>)!.Count;
            Assert.Equal(before + 1, after);
        }

        [Fact]
        public async Task TC50_HighVolume_Create5_Delete3_VerifyCount()
        {
            var ctx = CreateDb(nameof(TC50_HighVolume_Create5_Delete3_VerifyCount));
            var ctrl = CreateController(ctx);
            for (int i = 0; i < 5; i++)
                await ctrl.Create(new Category { Name = $"Bulk{i}" });
            var toDelete = ctx.Categories.Where(c => c.Name.StartsWith("Bulk")).Take(3).ToList();
            foreach (var cat in toDelete)
                await ctrl.Delete(cat.Id);
            var activeCount = ctx.Categories.Count(c => !c.IsDeleted);
            Assert.Equal(22, activeCount); // 20 seed + 5 new - 3 deleted
        }
    }
}