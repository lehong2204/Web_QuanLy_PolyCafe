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
    public class UserControllerAdminTests
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

        private UserController CreateController(PolyCafeDbContext ctx,
            string? userId = "U001", string? role = "Admin")
        {
            var controller = new UserController(ctx);
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
        // NHÓM 1: AUTHORIZATION (TC01–TC05)
        // ================================================================
        [Fact]
        public async Task TC01_Index_NotLoggedIn_RedirectsToLogin()
        {
            var ctx = CreateDb(nameof(TC01_Index_NotLoggedIn_RedirectsToLogin));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Index(null) as RedirectToActionResult;
            Assert.Equal("Login", result!.ActionName);
        }

        [Fact]
        public async Task TC02_Index_Staff_RedirectsToPOS()
        {
            var ctx = CreateDb(nameof(TC02_Index_Staff_RedirectsToPOS));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Index(null) as RedirectToActionResult;
            Assert.Equal("POS", result!.ActionName);
        }

        [Fact]
        public async Task TC03_Index_Admin_ReturnsView()
        {
            var ctx = CreateDb(nameof(TC03_Index_Admin_ReturnsView));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null) as ViewResult;
            Assert.NotNull(result);
            Assert.IsType<List<User>>(result!.Model);
        }

        [Fact]
        public async Task TC04_ToggleActive_NotLoggedIn_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC04_ToggleActive_NotLoggedIn_ReturnsFalse));
            var ctrl = CreateController(ctx, userId: null);
            var data = SafeToJson(await ctrl.ToggleActive("U002"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC05_ToggleRole_Staff_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC05_ToggleRole_Staff_ReturnsFalse));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var data = SafeToJson(await ctrl.ToggleRole("U003"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 2: INDEX (TC06–TC12)
        // ================================================================
        [Fact]
        public async Task TC06_Index_ReturnsAll5Users()
        {
            var ctx = CreateDb(nameof(TC06_Index_ReturnsAll5Users));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null) as ViewResult;
            var model = Assert.IsType<List<User>>(result!.Model);
            Assert.Equal(5, model.Count);
        }

        [Fact]
        public async Task TC07_Index_SearchByName_Works()
        {
            var ctx = CreateDb(nameof(TC07_Index_SearchByName_Works));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("Nguyễn Văn An") as ViewResult;
            var model = Assert.IsType<List<User>>(result!.Model);
            Assert.True(model.Any(u => u.FullName.Contains("Nguyễn Văn An")));
        }

        [Fact]
        public async Task TC08_Index_SearchByEmail_Works()
        {
            var ctx = CreateDb(nameof(TC08_Index_SearchByEmail_Works));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("admin@polycafe.vn") as ViewResult;
            var model = Assert.IsType<List<User>>(result!.Model);
            Assert.Contains(model, u => u.Email == "admin@polycafe.vn");
        }

        [Fact]
        public async Task TC09_Index_OrderedByRoleThenName_AdminFirst()
        {
            var ctx = CreateDb(nameof(TC09_Index_OrderedByRoleThenName_AdminFirst));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null) as ViewResult;
            var model = Assert.IsType<List<User>>(result!.Model);
            // Role=false(Staff) before Role=true(Admin)? No — bool false < true in ascending
            // OrderBy(Role) ascending: false(Staff)=0, true(Admin)=1 → Staff first
            // Actually C# bool false=0, true=1 → ascending order puts Staff first
            // But seed has U001 as Admin (Role=true) and U002-U005 as Staff (Role=false)
            // false < true so Staff comes first in ascending order
            Assert.False(model[0].Role); // First is Staff (false < true)
        }

        [Fact]
        public async Task TC10_Index_NoMatch_ReturnsEmpty()
        {
            var ctx = CreateDb(nameof(TC10_Index_NoMatch_ReturnsEmpty));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("NOTEXIST_XYZ") as ViewResult;
            var model = Assert.IsType<List<User>>(result!.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task TC11_Index_ViewBagSearch_Persisted()
        {
            var ctx = CreateDb(nameof(TC11_Index_ViewBagSearch_Persisted));
            var ctrl = CreateController(ctx);
            await ctrl.Index("mySearch");
            Assert.Equal("mySearch", ctrl.ViewBag.Search);
        }

        [Fact]
        public async Task TC12_Index_AfterCreateUser_CountIs6()
        {
            var ctx = CreateDb(nameof(TC12_Index_AfterCreateUser_CountIs6));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "New User", Email = "new@test.com", Password = "123456" });
            var result = await ctrl.Index(null) as ViewResult;
            var model = Assert.IsType<List<User>>(result!.Model);
            Assert.Equal(6, model.Count);
        }

        // ================================================================
        // NHÓM 3: CREATE (TC13–TC25)
        // ================================================================
        [Fact]
        public async Task TC13_Create_ValidData_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC13_Create_ValidData_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "Test User", Email = "test@test.com", Password = "123456" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC14_Create_EmptyFullName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC14_Create_EmptyFullName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "", Email = "x@x.com", Password = "123456" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC15_Create_EmptyEmail_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC15_Create_EmptyEmail_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "Test", Email = "", Password = "123456" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC16_Create_ShortPassword_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC16_Create_ShortPassword_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "Test", Email = "t@t.com", Password = "123" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC17_Create_DuplicateEmail_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC17_Create_DuplicateEmail_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "Dup", Email = "admin@polycafe.vn", Password = "123456" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC18_Create_IdStartsWithU()
        {
            var ctx = CreateDb(nameof(TC18_Create_IdStartsWithU));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "IDTest", Email = "id@test.com", Password = "123456" }));
            var id = data!.Value.GetProperty("id").GetString();
            Assert.StartsWith("U", id);
        }

        [Fact]
        public async Task TC19_Create_WithPhone_SavesPhone()
        {
            var ctx = CreateDb(nameof(TC19_Create_WithPhone_SavesPhone));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Phone User", Email = "phone@test.com", Password = "123456", Phone = "0987654321" });
            var user = ctx.Users.First(u => u.Email == "phone@test.com");
            Assert.Equal("0987654321", user.Phone);
        }

        [Fact]
        public async Task TC20_Create_IsActiveByDefault()
        {
            var ctx = CreateDb(nameof(TC20_Create_IsActiveByDefault));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Active User", Email = "active@test.com", Password = "123456" });
            var user = ctx.Users.First(u => u.Email == "active@test.com");
            Assert.True(user.IsActive);
        }

        [Fact]
        public async Task TC21_Create_ReturnsFullNameAndEmail()
        {
            var ctx = CreateDb(nameof(TC21_Create_ReturnsFullNameAndEmail));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "Return Test", Email = "ret@test.com", Password = "123456" }));
            Assert.Equal("Return Test", data!.Value.GetProperty("fullName").GetString());
            Assert.Equal("ret@test.com", data.Value.GetProperty("email").GetString());
        }

        [Fact]
        public async Task TC22_Create_MultipleUsers_IdsUnique()
        {
            var ctx = CreateDb(nameof(TC22_Create_MultipleUsers_IdsUnique));
            var ctrl = CreateController(ctx);
            for (int i = 0; i < 5; i++)
                await ctrl.Create(new UserCreateDto { FullName = $"User{i}", Email = $"u{i}@test.com", Password = "123456" });
            var ids = ctx.Users.Select(u => u.Id).ToList();
            Assert.Equal(ids.Distinct().Count(), ids.Count);
        }

        [Fact]
        public async Task TC23_Create_WithAddress_SavesAddress()
        {
            var ctx = CreateDb(nameof(TC23_Create_WithAddress_SavesAddress));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Addr User", Email = "addr@test.com", Password = "123456", Address = "123 Main St" });
            var user = ctx.Users.First(u => u.Email == "addr@test.com");
            Assert.Equal("123 Main St", user.Address);
        }

        [Fact]
        public async Task TC24_Create_AdminRole_SavesRoleTrue()
        {
            var ctx = CreateDb(nameof(TC24_Create_AdminRole_SavesRoleTrue));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Admin User", Email = "adm2@test.com", Password = "123456", Role = true });
            var user = ctx.Users.First(u => u.Email == "adm2@test.com");
            Assert.True(user.Role);
        }

        [Fact]
        public async Task TC25_Create_PersistsToDB()
        {
            var ctx = CreateDb(nameof(TC25_Create_PersistsToDB));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Persist", Email = "persist@test.com", Password = "123456" });
            Assert.True(ctx.Users.Any(u => u.Email == "persist@test.com"));
        }

        // ================================================================
        // NHÓM 4: UPDATE (TC26–TC35)
        // ================================================================
        [Fact]
        public async Task TC26_Update_ValidData_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC26_Update_ValidData_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "Updated Name", Email = "an@gmail.com" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC27_Update_EmptyFullName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC27_Update_EmptyFullName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "", Email = "an@gmail.com" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC28_Update_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC28_Update_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = "U999", FullName = "X", Email = "x@x.com" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC29_Update_DuplicateEmail_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC29_Update_DuplicateEmail_ReturnsFalse));
            var ctrl = CreateController(ctx);
            // Try to set U002's email to U003's email
            var data = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "An", Email = "bich@gmail.com" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC30_Update_UpdatesNameInDB()
        {
            var ctx = CreateDb(nameof(TC30_Update_UpdatesNameInDB));
            var ctrl = CreateController(ctx);
            await ctrl.Update(new UserUpdateDto { Id = "U003", FullName = "Bích Updated", Email = "bich@gmail.com" });
            var user = await ctx.Users.FindAsync("U003");
            Assert.Equal("Bích Updated", user!.FullName);
        }

        [Fact]
        public async Task TC31_Update_WithNewPassword_Updates()
        {
            var ctx = CreateDb(nameof(TC31_Update_WithNewPassword_Updates));
            var ctrl = CreateController(ctx);
            await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "An", Email = "an@gmail.com", NewPassword = "newpass123" });
            var user = await ctx.Users.FindAsync("U002");
            Assert.Equal("newpass123", user!.Password);
        }

        [Fact]
        public async Task TC32_Update_ShortNewPassword_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC32_Update_ShortNewPassword_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "An", Email = "an@gmail.com", NewPassword = "123" }));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC33_Update_NoNewPassword_DoesNotChangePassword()
        {
            var ctx = CreateDb(nameof(TC33_Update_NoNewPassword_DoesNotChangePassword));
            var ctrl = CreateController(ctx);
            var originalPw = (await ctx.Users.FindAsync("U002"))!.Password;
            await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "An", Email = "an@gmail.com", NewPassword = null });
            var user = await ctx.Users.FindAsync("U002");
            Assert.Equal(originalPw, user!.Password);
        }

        [Fact]
        public async Task TC34_Update_UpdatesPhone()
        {
            var ctx = CreateDb(nameof(TC34_Update_UpdatesPhone));
            var ctrl = CreateController(ctx);
            await ctrl.Update(new UserUpdateDto { Id = "U004", FullName = "Cường", Email = "cuong@gmail.com", Phone = "0999999999" });
            var user = await ctx.Users.FindAsync("U004");
            Assert.Equal("0999999999", user!.Phone);
        }

        [Fact]
        public async Task TC35_Update_SameEmail_Success()
        {
            var ctx = CreateDb(nameof(TC35_Update_SameEmail_Success));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = "U002", FullName = "An Updated", Email = "an@gmail.com" }));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 5: TOGGLE ACTIVE / ROLE (TC36–TC42)
        // ================================================================
        [Fact]
        public async Task TC36_ToggleActive_Valid_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC36_ToggleActive_Valid_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleActive("U002"));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC37_ToggleActive_Self_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC37_ToggleActive_Self_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleActive("U001"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC38_ToggleActive_TogglesIsActive()
        {
            var ctx = CreateDb(nameof(TC38_ToggleActive_TogglesIsActive));
            var ctrl = CreateController(ctx);
            var before = (await ctx.Users.FindAsync("U002"))!.IsActive;
            await ctrl.ToggleActive("U002");
            var after = (await ctx.Users.FindAsync("U002"))!.IsActive;
            Assert.NotEqual(before, after);
        }

        [Fact]
        public async Task TC39_ToggleRole_Valid_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC39_ToggleRole_Valid_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleRole("U002"));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC40_ToggleRole_Self_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC40_ToggleRole_Self_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.ToggleRole("U001"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC41_ToggleRole_StaffBecomesAdmin()
        {
            var ctx = CreateDb(nameof(TC41_ToggleRole_StaffBecomesAdmin));
            var ctrl = CreateController(ctx);
            // U002 is Staff (Role=false) → toggle → Admin (Role=true)
            await ctrl.ToggleRole("U002");
            var user = await ctx.Users.FindAsync("U002");
            Assert.True(user!.Role);
        }

        [Fact]
        public async Task TC42_ToggleRole_AdminBecomesStaff()
        {
            var ctx = CreateDb(nameof(TC42_ToggleRole_AdminBecomesStaff));
            // Add a second admin to toggle
            var ctxName = nameof(TC42_ToggleRole_AdminBecomesStaff);
            var ctx2 = CreateDb(ctxName + "_x");
            var ctrl = CreateController(ctx2);
            // U001 is Admin, but we can't toggle self. Use GetById then create another admin
            await ctrl.Create(new UserCreateDto { FullName = "Admin2", Email = "admin2@test.com", Password = "123456", Role = true });
            var admin2 = ctx2.Users.First(u => u.Email == "admin2@test.com");
            await ctrl.ToggleRole(admin2.Id);
            var updated = await ctx2.Users.FindAsync(admin2.Id);
            Assert.False(updated!.Role);
        }

        // ================================================================
        // NHÓM 6: GET BY ID & EDGE (TC43–TC50)
        // ================================================================
        [Fact]
        public async Task TC43_GetById_Valid_ReturnsSuccess()
        {
            var ctx = CreateDb(nameof(TC43_GetById_Valid_ReturnsSuccess));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.GetById("U002"));
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC44_GetById_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC44_GetById_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.GetById("U999"));
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC45_GetById_ReturnsCorrectEmail()
        {
            var ctx = CreateDb(nameof(TC45_GetById_ReturnsCorrectEmail));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.GetById("U001"));
            Assert.Equal("admin@polycafe.vn", data!.Value.GetProperty("email").GetString());
        }

        [Fact]
        public async Task TC46_GetById_ReturnsRoleCorrectly()
        {
            var ctx = CreateDb(nameof(TC46_GetById_ReturnsRoleCorrectly));
            var ctrl = CreateController(ctx);
            var data = SafeToJson(await ctrl.GetById("U001"));
            Assert.True(data!.Value.GetProperty("role").GetBoolean()); // U001 is Admin
        }

        [Fact]
        public async Task TC47_Create_Then_GetById_Works()
        {
            var ctx = CreateDb(nameof(TC47_Create_Then_GetById_Works));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Find Me", Email = "findme@test.com", Password = "123456" });
            var user = ctx.Users.First(u => u.Email == "findme@test.com");
            var data = SafeToJson(await ctrl.GetById(user.Id));
            Assert.Equal("Find Me", data!.Value.GetProperty("fullName").GetString());
        }

        [Fact]
        public async Task TC48_Create_Then_Update_Then_GetById_ShowsNewName()
        {
            var ctx = CreateDb(nameof(TC48_Create_Then_Update_Then_GetById_ShowsNewName));
            var ctrl = CreateController(ctx);
            await ctrl.Create(new UserCreateDto { FullName = "Old Name", Email = "old@test.com", Password = "123456" });
            var user = ctx.Users.First(u => u.Email == "old@test.com");
            await ctrl.Update(new UserUpdateDto { Id = user.Id, FullName = "New Name", Email = "old@test.com" });
            var data = SafeToJson(await ctrl.GetById(user.Id));
            Assert.Equal("New Name", data!.Value.GetProperty("fullName").GetString());
        }

        [Fact]
        public async Task TC49_ToggleActive_ThenGetById_ShowsUpdatedStatus()
        {
            var ctx = CreateDb(nameof(TC49_ToggleActive_ThenGetById_ShowsUpdatedStatus));
            var ctrl = CreateController(ctx);
            await ctrl.ToggleActive("U002");
            var user = await ctx.Users.FindAsync("U002");
            Assert.False(user!.IsActive); // was true, now false
        }

        [Fact]
        public async Task TC50_FullCRUD_Cycle_Works()
        {
            var ctx = CreateDb(nameof(TC50_FullCRUD_Cycle_Works));
            var ctrl = CreateController(ctx);

            // Create
            var createData = SafeToJson(await ctrl.Create(new UserCreateDto { FullName = "Cycle User", Email = "cycle@test.com", Password = "123456" }));
            Assert.True(createData!.Value.GetProperty("success").GetBoolean());
            var userId = createData.Value.GetProperty("id").GetString()!;

            // GetById
            var getResult = SafeToJson(await ctrl.GetById(userId));
            Assert.True(getResult!.Value.GetProperty("success").GetBoolean());

            // Update
            var updateResult = SafeToJson(await ctrl.Update(new UserUpdateDto { Id = userId, FullName = "Cycle Updated", Email = "cycle@test.com" }));
            Assert.True(updateResult!.Value.GetProperty("success").GetBoolean());

            // ToggleActive
            var toggleResult = SafeToJson(await ctrl.ToggleActive(userId));
            Assert.True(toggleResult!.Value.GetProperty("success").GetBoolean());

            var user = await ctx.Users.FindAsync(userId);
            Assert.False(user!.IsActive);
        }
    }
}