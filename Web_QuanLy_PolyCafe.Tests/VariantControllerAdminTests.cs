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
    public class VariantControllerAdminTests
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

        private VariantController CreateController(PolyCafeDbContext ctx,
            string? userId = "U001", string? role = "Admin")
        {
            var controller = new VariantController(ctx);
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
        // NHÓM 1: AUTHORIZATION (TC01–TC10)
        // ================================================================
        [Fact]
        public async Task TC01_Index_NotLoggedIn_RedirectsToLogin()
        {
            var ctx = CreateDb(nameof(TC01_Index_NotLoggedIn_RedirectsToLogin));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Index(null, null) as RedirectToActionResult;
            Assert.Equal("Login", result!.ActionName);
        }

        [Fact]
        public async Task TC02_Index_Staff_RedirectsToPOS()
        {
            var ctx = CreateDb(nameof(TC02_Index_Staff_RedirectsToPOS));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Index(null, null) as RedirectToActionResult;
            Assert.Equal("POS", result!.ActionName);
        }

        [Fact]
        public async Task TC03_Add_NotLoggedIn_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC03_Add_NotLoggedIn_ReturnsFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = "Test" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC04_Add_Staff_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC04_Add_Staff_ReturnsFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = "Test" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC05_Edit_NotLoggedIn_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC05_Edit_NotLoggedIn_ReturnsFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DV001", VariantName = "X" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC06_Edit_Staff_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC06_Edit_Staff_ReturnsFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DV001", VariantName = "X" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC07_ToggleActive_NotLoggedIn_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC07_ToggleActive_NotLoggedIn_ReturnsFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV001", IsActive = false }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC08_Delete_NotLoggedIn_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC08_Delete_NotLoggedIn_ReturnsFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.Delete("DV001"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC09_Delete_Staff_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC09_Delete_Staff_ReturnsFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.Delete("DV001"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC10_ToggleActive_Staff_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC10_ToggleActive_Staff_ReturnsFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV001", IsActive = false }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 2: INDEX (TC11–TC18)
        // ================================================================
        [Fact]
        public async Task TC11_Index_Admin_ReturnsView()
        {
            var ctx = CreateDb(nameof(TC11_Index_Admin_ReturnsView));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            Assert.NotNull(result);
            Assert.IsType<List<DrinkVariant>>(result!.Model);
        }

        [Fact]
        public async Task TC12_Index_Returns20SeedVariants()
        {
            var ctx = CreateDb(nameof(TC12_Index_Returns20SeedVariants));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.Equal(20, model.Count);
        }

        [Fact]
        public async Task TC13_Index_SearchByVariantName_Filters()
        {
            var ctx = CreateDb(nameof(TC13_Index_SearchByVariantName_Filters));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("Size", null) as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.All(model, v => Assert.Contains("Size", v.VariantName));
        }

        [Fact]
        public async Task TC14_Index_FilterByDrinkId_Works()
        {
            var ctx = CreateDb(nameof(TC14_Index_FilterByDrinkId_Works));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "D001") as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.All(model, v => Assert.Equal("D001", v.DrinkId));
        }

        [Fact]
        public async Task TC15_Index_D001_HasThreeVariants()
        {
            var ctx = CreateDb(nameof(TC15_Index_D001_HasThreeVariants));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "D001") as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.Equal(3, model.Count); // DV001, DV002, DV003
        }

        [Fact]
        public async Task TC16_Index_ViewBagDrinks_NotNull()
        {
            var ctx = CreateDb(nameof(TC16_Index_ViewBagDrinks_NotNull));
            var ctrl = CreateController(ctx);
            await ctrl.Index(null, null);
            Assert.NotNull(ctrl.ViewBag.Drinks);
        }

        [Fact]
        public async Task TC17_Index_SearchPersisted_InViewBag()
        {
            var ctx = CreateDb(nameof(TC17_Index_SearchPersisted_InViewBag));
            var ctrl = CreateController(ctx);
            await ctrl.Index("SizeSearch", null);
            Assert.Equal("SizeSearch", ctrl.ViewBag.Search);
        }

        [Fact]
        public async Task TC18_Index_NonExistSearch_ReturnsEmpty()
        {
            var ctx = CreateDb(nameof(TC18_Index_NonExistSearch_ReturnsEmpty));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("NOTEXIST_XYZ", null) as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.Empty(model);
        }

        // ================================================================
        // NHÓM 3: ADD (TC19–TC28)
        // ================================================================
        [Fact]
        public async Task TC19_Add_ValidData_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC19_Add_ValidData_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = "Size XL", ExtraPrice = 15000 }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC20_Add_EmptyVariantName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC20_Add_EmptyVariantName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = "" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC21_Add_EmptyDrinkId_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC21_Add_EmptyDrinkId_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "", VariantName = "Test" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC22_Add_DrinkNotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC22_Add_DrinkNotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "DXXX", VariantName = "Test" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC23_Add_IdStartsWithVAR()
        {
            var ctx = CreateDb(nameof(TC23_Add_IdStartsWithVAR));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D002", VariantName = "New Variant" }));
            var id = data!.Value.GetProperty("id").GetString();
            Assert.StartsWith("VAR", id);
        }

        [Fact]
        public async Task TC24_Add_PersistsToDB()
        {
            var ctx = CreateDb(nameof(TC24_Add_PersistsToDB));
            var ctrl = CreateController(ctx);
            await ctrl.Add(new AddVariantRequest { DrinkId = "D003", VariantName = "Persist Variant", ExtraPrice = 8000 });
            Assert.True(ctx.DrinkVariants.Any(v => v.VariantName == "Persist Variant"));
        }

        [Fact]
        public async Task TC25_Add_IsActiveByDefault()
        {
            var ctx = CreateDb(nameof(TC25_Add_IsActiveByDefault));
            var ctrl = CreateController(ctx);
            await ctrl.Add(new AddVariantRequest { DrinkId = "D004", VariantName = "Active V" });
            var v = ctx.DrinkVariants.First(x => x.VariantName == "Active V");
            Assert.True(v.IsActive);
        }

        [Fact]
        public async Task TC26_Add_ReturnsDrinkName()
        {
            var ctx = CreateDb(nameof(TC26_Add_ReturnsDrinkName));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = "DrinkName Test" }));
            Assert.NotNull(data!.Value.GetProperty("drinkName").GetString());
        }

        [Fact]
        public async Task TC27_Add_ExtraPrice_SavedCorrectly()
        {
            var ctx = CreateDb(nameof(TC27_Add_ExtraPrice_SavedCorrectly));
            var ctrl = CreateController(ctx);
            await ctrl.Add(new AddVariantRequest { DrinkId = "D005", VariantName = "Extra Test", ExtraPrice = 12000 });
            var v = ctx.DrinkVariants.First(x => x.VariantName == "Extra Test");
            Assert.Equal(12000, v.ExtraPrice);
        }

        [Fact]
        public async Task TC28_Add_WhitespaceVariantName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC28_Add_WhitespaceVariantName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = "   " }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 4: EDIT (TC29–TC36)
        // ================================================================
        [Fact]
        public async Task TC29_Edit_Valid_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC29_Edit_Valid_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DV001", VariantName = "Size S Updated", ExtraPrice = 0 }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC30_Edit_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC30_Edit_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DVXXX", VariantName = "X" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC31_Edit_EmptyName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC31_Edit_EmptyName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DV001", VariantName = "" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC32_Edit_UpdatesNameInDB()
        {
            var ctx = CreateDb(nameof(TC32_Edit_UpdatesNameInDB));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new EditVariantRequest { Id = "DV002", VariantName = "Size M New", ExtraPrice = 5000 });
            var v = await ctx.DrinkVariants.FindAsync("DV002");
            Assert.Equal("Size M New", v!.VariantName);
        }

        [Fact]
        public async Task TC33_Edit_UpdatesExtraPrice()
        {
            var ctx = CreateDb(nameof(TC33_Edit_UpdatesExtraPrice));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new EditVariantRequest { Id = "DV003", VariantName = "Size L", ExtraPrice = 20000 });
            var v = await ctx.DrinkVariants.FindAsync("DV003");
            Assert.Equal(20000, v!.ExtraPrice);
        }

        [Fact]
        public async Task TC34_Edit_MultipleTimes_LastUpdateWins()
        {
            var ctx = CreateDb(nameof(TC34_Edit_MultipleTimes_LastUpdateWins));
            var ctrl = CreateController(ctx);
            await ctrl.Edit(new EditVariantRequest { Id = "DV004", VariantName = "First", ExtraPrice = 0 });
            await ctrl.Edit(new EditVariantRequest { Id = "DV004", VariantName = "Second", ExtraPrice = 5000 });
            var v = await ctx.DrinkVariants.FindAsync("DV004");
            Assert.Equal("Second", v!.VariantName);
        }

        [Fact]
        public async Task TC35_Edit_ZeroExtraPrice_Allowed()
        {
            var ctx = CreateDb(nameof(TC35_Edit_ZeroExtraPrice_Allowed));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DV005", VariantName = "No Extra", ExtraPrice = 0 }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC36_Edit_SameName_Success()
        {
            var ctx = CreateDb(nameof(TC36_Edit_SameName_Success));
            var ctrl = CreateController(ctx);
            var v = await ctx.DrinkVariants.FindAsync("DV006");
            var data = SafeToJson(await ctrl.Edit(new EditVariantRequest { Id = "DV006", VariantName = v!.VariantName, ExtraPrice = v.ExtraPrice }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 5: TOGGLE ACTIVE + DELETE (TC37–TC50)
        // ================================================================
        [Fact]
        public async Task TC37_ToggleActive_SetFalse_UpdatesDB()
        {
            var ctx = CreateDb(nameof(TC37_ToggleActive_SetFalse_UpdatesDB));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV007", IsActive = false }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
            var v = await ctx.DrinkVariants.FindAsync("DV007");
            Assert.False(v!.IsActive);
        }

        [Fact]
        public async Task TC38_ToggleActive_SetTrue_UpdatesDB()
        {
            var ctx = CreateDb(nameof(TC38_ToggleActive_SetTrue_UpdatesDB));
            var ctrl = CreateController(ctx);
            await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV008", IsActive = false });
            await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV008", IsActive = true });
            var v = await ctx.DrinkVariants.FindAsync("DV008");
            Assert.True(v!.IsActive);
        }

        [Fact]
        public async Task TC39_ToggleActive_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC39_ToggleActive_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DVXXX", IsActive = false }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC40_ToggleActive_ReturnsIsActive_InResponse()
        {
            var ctx = CreateDb(nameof(TC40_ToggleActive_ReturnsIsActive_InResponse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV009", IsActive = false }));
            Assert.False(data!.Value.GetProperty("isActive").GetBoolean());
        }

        [Fact]
        public async Task TC41_Delete_Valid_RemovesFromDB()
        {
            var ctx = CreateDb(nameof(TC41_Delete_Valid_RemovesFromDB));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Delete("DV001"));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
            Assert.Null(await ctx.DrinkVariants.FindAsync("DV001"));
        }

        [Fact]
        public async Task TC42_Delete_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC42_Delete_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Delete("DVXXX"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC43_Delete_ReducesVariantCount()
        {
            var ctx = CreateDb(nameof(TC43_Delete_ReducesVariantCount));
            var ctrl = CreateController(ctx);
            var before = ctx.DrinkVariants.Count();
            await ctrl.Delete("DV002");
            Assert.Equal(before - 1, ctx.DrinkVariants.Count());
        }

        [Fact]
        public async Task TC44_Add_ThenEdit_ThenDelete_FullCycle()
        {
            var ctx = CreateDb(nameof(TC44_Add_ThenEdit_ThenDelete_FullCycle));
            var ctrl = CreateController(ctx);

            var addData = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D006", VariantName = "Cycle V", ExtraPrice = 0 }));
            var id = addData!.Value.GetProperty("id").GetString()!;

            await ctrl.Edit(new EditVariantRequest { Id = id, VariantName = "Cycle Edited", ExtraPrice = 5000 });
            var v = await ctx.DrinkVariants.FindAsync(id);
            Assert.Equal("Cycle Edited", v!.VariantName);

            await ctrl.Delete(id);
            Assert.Null(await ctx.DrinkVariants.FindAsync(id));
        }

        [Fact]
        public async Task TC45_Add_Multiple_AllUnique()
        {
            var ctx = CreateDb(nameof(TC45_Add_Multiple_AllUnique));
            var ctrl = CreateController(ctx);
            for (int i = 0; i < 5; i++)
                await ctrl.Add(new AddVariantRequest { DrinkId = "D007", VariantName = $"Var{i}" });
            var ids = ctx.DrinkVariants.Select(v => v.Id).ToList();
            Assert.Equal(ids.Distinct().Count(), ids.Count);
        }

        [Fact]
        public async Task TC46_Index_AfterAdd_CountIs21()
        {
            var ctx = CreateDb(nameof(TC46_Index_AfterAdd_CountIs21));
            var ctrl = CreateController(ctx);
            await ctrl.Add(new AddVariantRequest { DrinkId = "D008", VariantName = "Extra Size" });
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.Equal(21, model.Count);
        }

        [Fact]
        public async Task TC47_Index_AfterDelete_CountIs19()
        {
            var ctx = CreateDb(nameof(TC47_Index_AfterDelete_CountIs19));
            var ctrl = CreateController(ctx);
            await ctrl.Delete("DV010");
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<DrinkVariant>>(result!.Model);
            Assert.Equal(19, model.Count);
        }

        [Fact]
        public async Task TC48_ToggleActive_ThenIndex_IsActiveUpdated()
        {
            var ctx = CreateDb(nameof(TC48_ToggleActive_ThenIndex_IsActiveUpdated));
            var ctrl = CreateController(ctx);
            await ctrl.ToggleActive(new ToggleActiveRequest { Id = "DV011", IsActive = false });
            var v = await ctx.DrinkVariants.FindAsync("DV011");
            Assert.False(v!.IsActive);
        }

        [Fact]
        public async Task TC49_Add_NullVariantName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC49_Add_NullVariantName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Add(new AddVariantRequest { DrinkId = "D001", VariantName = null! }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC50_FullIntegration_NoException()
        {
            var ctx = CreateDb(nameof(TC50_FullIntegration_NoException));
            var ctrl = CreateController(ctx);
            var ex = await Record.ExceptionAsync(async () =>
            {
                await ctrl.Index(null, null);
                await ctrl.Add(new AddVariantRequest { DrinkId = "D009", VariantName = "Integration Test" });
                var v = ctx.DrinkVariants.First(x => x.VariantName == "Integration Test");
                await ctrl.Edit(new EditVariantRequest { Id = v.Id, VariantName = "Integration Edited", ExtraPrice = 5000 });
                await ctrl.ToggleActive(new ToggleActiveRequest { Id = v.Id, IsActive = false });
                await ctrl.Delete(v.Id);
            });
            Assert.Null(ex);
        }
    }
}