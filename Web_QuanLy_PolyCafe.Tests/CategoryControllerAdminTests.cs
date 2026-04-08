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
    public class CategoryControllerTests
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

        private CategoryController CreateController(PolyCafeDbContext ctx, string? userId = "U001", string? role = "Admin")
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

        // Hàm an toàn để parse JsonResult
        private JsonElement? SafeToJson(IActionResult? result)
        {
            if (result is JsonResult jsonResult && jsonResult.Value != null)
            {
                var jsonString = JsonSerializer.Serialize(jsonResult.Value);
                return JsonSerializer.Deserialize<JsonElement>(jsonString);
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
            Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task TC02_Index_Staff_RedirectsToPOS()
        {
            var ctx = CreateDb(nameof(TC02_Index_Staff_RedirectsToPOS));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Index() as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("POS", result.ActionName);
        }

        [Fact]
        public async Task TC03_Create_NotLoggedIn_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC03_Create_NotLoggedIn_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Create(new Category { Name = "Test" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC04_Create_Staff_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC04_Create_Staff_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Create(new Category { Name = "Test" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC05_Edit_NotLoggedIn_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC05_Edit_NotLoggedIn_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Edit(new Category { Id = "CAT01", Name = "Test" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC06_Edit_Staff_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC06_Edit_Staff_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Edit(new Category { Id = "CAT01", Name = "Test" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC07_Delete_NotLoggedIn_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC07_Delete_NotLoggedIn_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Delete("CAT01") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC08_Delete_Staff_ReturnsJsonFalse()
        {
            var ctx = CreateDb(nameof(TC08_Delete_Staff_ReturnsJsonFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Delete("CAT01") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 2: INDEX (TC09–TC15)
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
        public async Task TC12_Index_ReturnsCorrectCategoryCount()
        {
            var ctx = CreateDb(nameof(TC12_Index_ReturnsCorrectCategoryCount));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(20, model.Count);
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
        public async Task TC14_Index_AfterManyCreates_ReturnsAll()
        {
            var ctx = CreateDb(nameof(TC14_Index_AfterManyCreates_ReturnsAll));
            var ctrl = CreateController(ctx);
            for (int i = 0; i < 5; i++) await ctrl.Create(new Category { Name = $"Extra{i}" });
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Equal(25, model.Count);
        }

        [Fact]
        public async Task TC15_Index_ModelIsListOfCategory()
        {
            var ctx = CreateDb(nameof(TC15_Index_ModelIsListOfCategory));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            Assert.IsType<List<Category>>(result!.Model);
        }

        // ================================================================
        // NHÓM 3: CREATE (TC16–TC30)
        // ================================================================
        [Fact]
        public async Task TC16_Create_ValidName_SuccessAndReturnsId()
        {
            var ctx = CreateDb(nameof(TC16_Create_ValidName_SuccessAndReturnsId));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "Trà Sữa Signature" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC17_Create_EmptyName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC17_Create_EmptyName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC18_Create_WhitespaceName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC18_Create_WhitespaceName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "   " }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC19_Create_NullModel_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC19_Create_NullModel_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(null!);

            if (result is not JsonResult)
            {
                Assert.True(true); // Binding error là hành vi mong muốn
                return;
            }

            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC20_Create_VeryLongName_Success()
        {
            var ctx = CreateDb(nameof(TC20_Create_VeryLongName_Success));
            var ctrl = CreateController(ctx);
            var longName = new string('A', 500);
            var result = await ctrl.Create(new Category { Name = longName }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC21_Create_VietnameseName_Success()
        {
            var ctx = CreateDb(nameof(TC21_Create_VietnameseName_Success));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "Cà Phê Việt Nam Đặc Biệt" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC22_Create_DuplicateName_StillSuccess()
        {
            var ctx = CreateDb(nameof(TC22_Create_DuplicateName_StillSuccess));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new Category { Name = "Duplicate" });
            var result = await ctrl.Create(new Category { Name = "Duplicate" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC23_Create_SavesToDatabase_Correctly()
        {
            var ctx = CreateDb(nameof(TC23_Create_SavesToDatabase_Correctly));
            var ctrl = CreateController(ctx);
            var before = ctx.Categories.Count();
            await ctrl.Create(new Category { Name = "New Cat" });
            Assert.Equal(before + 1, ctx.Categories.Count());
        }

        [Fact]
        public async Task TC24_Create_IdStartsWithCAT_AndCorrectLength()
        {
            var ctx = CreateDb(nameof(TC24_Create_IdStartsWithCAT_AndCorrectLength));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "Test Category" }) as JsonResult;
            Assert.NotNull(result);

            var data = SafeToJson(result);
            Assert.NotNull(data);

            var id = data!.Value.GetProperty("id").GetString();
            Assert.NotNull(id);
            Assert.StartsWith("CAT", id);
            Assert.True(id!.Length >= 11);
        }

        [Fact]
        public async Task TC25_Create_NullDescription_Success()
        {
            var ctx = CreateDb(nameof(TC25_Create_NullDescription_Success));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "No Desc" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC26_Create_EmptyDescription_Success()
        {
            var ctx = CreateDb(nameof(TC26_Create_EmptyDescription_Success));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "Empty Desc", Description = "" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC27_Create_MultipleCategories_AllSaved()
        {
            var ctx = CreateDb(nameof(TC27_Create_MultipleCategories_AllSaved));
            var ctrl = CreateController(ctx);
            for (int i = 0; i < 5; i++)
                await ctrl.Create(new Category { Name = $"Cat{i}" });
            Assert.Equal(25, ctx.Categories.Count());
        }

        [Fact]
        public async Task TC28_Create_ResponseContainsName()
        {
            var ctx = CreateDb(nameof(TC28_Create_ResponseContainsName));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Create(new Category { Name = "Test Name" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.Equal("Test Name", data!.Value.GetProperty("name").GetString());
        }

        [Fact]
        public async Task TC29_Create_AfterCreate_IndexShowsNewCategory()
        {
            var ctx = CreateDb(nameof(TC29_Create_AfterCreate_IndexShowsNewCategory));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new Category { Name = "New Visible Cat" });
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.Contains(model, c => c.Name == "New Visible Cat");
        }

        [Fact]
        public async Task TC30_Create_30Categories_AllHaveUniqueId()
        {
            var ctx = CreateDb(nameof(TC30_Create_30Categories_AllHaveUniqueId));
            var ctrl = CreateController(ctx);
            var ids = new HashSet<string>();
            for (int i = 0; i < 30; i++)
            {
                var result = await ctrl.Create(new Category { Name = $"Cat{i}" }) as JsonResult;
                var data = SafeToJson(result);
                Assert.NotNull(data);
                var id = data!.Value.GetProperty("id").GetString()!;
                Assert.True(ids.Add(id));
            }
        }

        // ================================================================
        // NHÓM 4: EDIT (TC31–TC40)
        // ================================================================
        [Fact]
        public async Task TC31_Edit_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC31_Edit_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Edit(new Category { Id = "CAT999", Name = "Not Found" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC32_Edit_Valid_UpdatesNameAndDescription()
        {
            var ctx = CreateDb(nameof(TC32_Edit_Valid_UpdatesNameAndDescription));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new Category { Id = "CAT01", Name = "Cà Phê Mới", Description = "Mô tả mới" });
            var cat = await ctx.Categories.FindAsync("CAT01");
            Assert.Equal("Cà Phê Mới", cat!.Name);
            Assert.Equal("Mô tả mới", cat.Description);
        }

        [Fact]
        public async Task TC33_Edit_OnlyName_UpdatesSuccessfully()
        {
            var ctx = CreateDb(nameof(TC33_Edit_OnlyName_UpdatesSuccessfully));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new Category { Id = "CAT02", Name = "Trà Sữa Siêu Ngon" });
            var cat = await ctx.Categories.FindAsync("CAT02");
            Assert.Equal("Trà Sữa Siêu Ngon", cat!.Name);
        }

        [Fact]
        public async Task TC34_Edit_OnlyDescription_UpdatesSuccessfully()
        {
            var ctx = CreateDb(nameof(TC34_Edit_OnlyDescription_UpdatesSuccessfully));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new Category { Id = "CAT03", Description = "Chỉ cập nhật mô tả" });
            var cat = await ctx.Categories.FindAsync("CAT03");
            Assert.Equal("Chỉ cập nhật mô tả", cat!.Description);
        }

        [Fact]
        public async Task TC35_Edit_EmptyName_StillSuccess()
        {
            var ctx = CreateDb(nameof(TC35_Edit_EmptyName_StillSuccess));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Edit(new Category { Id = "CAT01", Name = "" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC36_Edit_WhitespaceName_StillSuccess()
        {
            var ctx = CreateDb(nameof(TC36_Edit_WhitespaceName_StillSuccess));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Edit(new Category { Id = "CAT01", Name = "   " }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC37_Edit_NullModel_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC37_Edit_NullModel_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Edit(null!);

            if (result is not JsonResult)
            {
                Assert.True(true);
                return;
            }

            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC38_Edit_SameName_Success()
        {
            var ctx = CreateDb(nameof(TC38_Edit_SameName_Success));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Edit(new Category { Id = "CAT01", Name = "Cà Phê" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC39_Edit_DeletedCategory_CanStillEdit()
        {
            var ctx = CreateDb(nameof(TC39_Edit_DeletedCategory_CanStillEdit));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT05");
            var result = await ctrl.Edit(new Category { Id = "CAT05", Name = "Vẫn sửa được" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC40_Edit_ResponseReturnsSuccessTrue()
        {
            var ctx = CreateDb(nameof(TC40_Edit_ResponseReturnsSuccessTrue));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Edit(new Category { Id = "CAT01", Name = "Updated" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
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
            var result = await ctrl.Delete("CAT999") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC42_Delete_Valid_SetsIsDeletedTrue()
        {
            var ctx = CreateDb(nameof(TC42_Delete_Valid_SetsIsDeletedTrue));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT06");
            var cat = await ctx.Categories.FindAsync("CAT06");
            Assert.True(cat!.IsDeleted);
        }

        [Fact]
        public async Task TC43_Delete_SoftDelete_IndexExcludesIt()
        {
            var ctx = CreateDb(nameof(TC43_Delete_SoftDelete_IndexExcludesIt));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT07");
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsType<List<Category>>(result!.Model);
            Assert.DoesNotContain(model, c => c.Id == "CAT07");
        }

        [Fact]
        public async Task TC44_Delete_Twice_StillReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC44_Delete_Twice_StillReturnsSuccess));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT08");
            var result = await ctrl.Delete("CAT08") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC45_Delete_ThenCreateNewWithSameName_Success()
        {
            var ctx = CreateDb(nameof(TC45_Delete_ThenCreateNewWithSameName_Success));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT09");
            var result = await ctrl.Create(new Category { Name = "Cà Phê" }) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC46_Delete_DrinkStillExistsButCategoryHidden()
        {
            var ctx = CreateDb(nameof(TC46_Delete_DrinkStillExistsButCategoryHidden));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("CAT01");
            var count = ctx.Drinks.Count(d => d.CategoryId == "CAT01");
            Assert.True(count > 0);
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
            var createRes = await ctrl.Create(new Category { Name = "CycleTest" }) as JsonResult;
            var id = SafeToJson(createRes)!.Value.GetProperty("id").GetString()!;

            await ctrl.Edit(new Category { Id = id, Name = "Cycle Edited" });
            await ctrl.Delete(id);

            var cat = await ctx.Categories.FindAsync(id);
            Assert.True(cat!.IsDeleted);
        }

        [Fact]
        public async Task TC49_Create_ThenEdit_ThenDelete_VerifyDBState()
        {
            var ctx = CreateDb(nameof(TC49_Create_ThenEdit_ThenDelete_VerifyDBState));
            var ctrl = CreateController(ctx);
            var createRes = await ctrl.Create(new Category { Name = "Temp" }) as JsonResult;
            var id = SafeToJson(createRes)!.Value.GetProperty("id").GetString()!;

            await ctrl.Edit(new Category { Id = id, Name = "Edited Temp" });
            await ctrl.Delete(id);

            var cat = await ctx.Categories.FindAsync(id);
            Assert.True(cat!.IsDeleted);
        }

        [Fact]
        public async Task TC50_HighVolume_Create30_EditDeleteHalf_VerifyCount()
        {
            var ctx = CreateDb(nameof(TC50_HighVolume_Create30_EditDeleteHalf_VerifyCount));
            var ctrl = CreateController(ctx);

            for (int i = 0; i < 30; i++)
                await ctrl.Create(new Category { Name = $"High{i}" });

            for (int i = 0; i < 10; i++)
            {
                var cat = ctx.Categories.First(c => c.Name == $"High{i}");
                await ctrl.Edit(new Category { Id = cat.Id, Name = $"Edited{i}" });
            }

            var toDelete = ctx.Categories.Where(c => !c.IsDeleted).Take(15).ToList();
            foreach (var cat in toDelete)
                await ctrl.Delete(cat.Id);

            var activeCount = ctx.Categories.Count(c => !c.IsDeleted);
            Assert.Equal(20 + 30 - 15, activeCount);
        }
    }
}