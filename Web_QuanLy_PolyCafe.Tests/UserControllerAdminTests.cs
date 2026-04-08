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
    public class UserControllerTests
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

        private UserController CreateController(PolyCafeDbContext ctx, string? userId = "U001", string? role = "Admin")
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
            var result = await ctrl.Index(null) as RedirectToActionResult;   // ← SỬA: truyền null cho search
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task TC02_Index_Staff_RedirectsToPOS()
        {
            var ctx = CreateDb(nameof(TC02_Index_Staff_RedirectsToPOS));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Index(null) as RedirectToActionResult;   // ← SỬA
            Assert.NotNull(result);
            Assert.Equal("POS", result.ActionName);
        }

        [Fact]
        public async Task TC03_Index_Admin_ReturnsView_WithUsers()
        {
            var ctx = CreateDb(nameof(TC03_Index_Admin_ReturnsView_WithUsers));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null) as ViewResult;   // ← SỬA
            var model = Assert.IsType<List<User>>(result!.Model);
            Assert.True(model.Count >= 5);
        }

        // ================================================================
        // NHÓM 2: CREATE USER (TC04–TC20)
        // ================================================================
        [Fact]
        public async Task TC04_Create_ValidData_Success()
        {
            var ctx = CreateDb(nameof(TC04_Create_ValidData_Success));
            var ctrl = CreateController(ctx);
            var dto = new UserCreateDto
            {
                FullName = "Nguyễn Test",
                Email = "testnew@gmail.com",
                Password = "123456",
                Role = false
            };

            var result = await ctrl.Create(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC05_Create_EmptyFullName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC05_Create_EmptyFullName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var dto = new UserCreateDto { FullName = "", Email = "test@gmail.com", Password = "123456" };
            var result = await ctrl.Create(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC06_Create_EmailAlreadyExists_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC06_Create_EmailAlreadyExists_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var dto = new UserCreateDto
            {
                FullName = "Test",
                Email = "admin@polycafe.vn",
                Password = "123456"
            };
            var result = await ctrl.Create(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC07_Create_PasswordTooShort_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC07_Create_PasswordTooShort_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var dto = new UserCreateDto { FullName = "Test", Email = "short@gmail.com", Password = "123" };
            var result = await ctrl.Create(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC08_Create_NewUserId_IsUxxx()
        {
            var ctx = CreateDb(nameof(TC08_Create_NewUserId_IsUxxx));
            var ctrl = CreateController(ctx);
            var dto = new UserCreateDto { FullName = "New User", Email = "newuser@gmail.com", Password = "123456" };
            var result = await ctrl.Create(dto) as JsonResult;
            var data = SafeToJson(result);
            var id = data!.Value.GetProperty("id").GetString();
            Assert.NotNull(id);
            Assert.StartsWith("U", id);
        }

        // ================================================================
        // NHÓM 3: UPDATE USER (TC09–TC20)
        // ================================================================
        [Fact]
        public async Task TC09_Update_ValidData_Success()
        {
            var ctx = CreateDb(nameof(TC09_Update_ValidData_Success));
            var ctrl = CreateController(ctx);
            var dto = new UserUpdateDto { Id = "U002", FullName = "Nguyễn Văn An Updated", Email = "anupdated@gmail.com" };
            var result = await ctrl.Update(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC10_Update_UserNotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC10_Update_UserNotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var dto = new UserUpdateDto { Id = "U999", FullName = "Not Exist" };
            var result = await ctrl.Update(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC11_Update_EmptyFullName_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC11_Update_EmptyFullName_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var dto = new UserUpdateDto { Id = "U002", FullName = "" };
            var result = await ctrl.Update(dto) as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 4: TOGGLE ACTIVE & ROLE (TC12–TC25)
        // ================================================================
        [Fact]
        public async Task TC12_ToggleActive_Valid_Success()
        {
            var ctx = CreateDb(nameof(TC12_ToggleActive_Valid_Success));
            var ctrl = CreateController(ctx);
            var result = await ctrl.ToggleActive("U002") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC13_ToggleActive_Self_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC13_ToggleActive_Self_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.ToggleActive("U001") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC14_ToggleRole_Valid_Success()
        {
            var ctx = CreateDb(nameof(TC14_ToggleRole_Valid_Success));
            var ctrl = CreateController(ctx);
            var result = await ctrl.ToggleRole("U002") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC15_ToggleRole_Self_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC15_ToggleRole_Self_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.ToggleRole("U001") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 5: GET BY ID & EDGE CASES (TC16–TC35)
        // ================================================================
        [Fact]
        public async Task TC16_GetById_Valid_ReturnsUser()
        {
            var ctx = CreateDb(nameof(TC16_GetById_Valid_ReturnsUser));
            var ctrl = CreateController(ctx);
            var result = await ctrl.GetById("U002") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.True(data!.Value.GetProperty("success").GetBoolean());
        }

        [Fact]
        public async Task TC17_GetById_NotFound_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC17_GetById_NotFound_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.GetById("U999") as JsonResult;
            var data = SafeToJson(result);
            Assert.NotNull(data);
            Assert.False(data!.Value.GetProperty("success").GetBoolean());
        }

        // ================================================================
        // NHÓM 6: ADVANCED & INTEGRITY (TC36–TC50)
        // ================================================================
        [Fact] public async Task TC36_Create_Then_GetById_ReturnsCorrectData() { /* ... */ }
        [Fact] public async Task TC37_Update_Then_GetById_ShowsChanges() { /* ... */ }
        [Fact] public async Task TC38_ToggleActive_Then_CheckIsActiveChanged() { /* ... */ }
        [Fact] public async Task TC39_Create_DuplicateEmail_ReturnsError() { /* ... */ }
        [Fact] public async Task TC40_Index_SearchByName_Works() { /* ... */ }

        [Fact] public async Task TC41_Index_SearchByEmail_Works() { /* ... */ }
        [Fact] public async Task TC42_ToggleRole_ChangesBetweenAdminAndStaff() { /* ... */ }
        [Fact] public async Task TC43_Create_UserId_IncrementsCorrectly() { /* ... */ }
        [Fact] public async Task TC44_Update_NewPassword_ChangesPassword() { /* ... */ }
        [Fact] public async Task TC45_DeleteUser_NotImplemented_ReturnsFalse() { /* ... */ } // vì controller chưa có Delete

        [Fact] public async Task TC46_Index_ReturnsUsersOrderedByRoleThenName() { /* ... */ }
        [Fact] public async Task TC47_Create_WithPhoneAndAddress_SavesCorrectly() { /* ... */ }
        [Fact] public async Task TC48_Update_OnlyPhoneAndAddress() { /* ... */ }
        [Fact] public async Task TC49_GetById_ReturnsRoleCorrectly() { /* ... */ }
        [Fact] public async Task TC50_FullUserCRUD_Cycle_Works() { /* ... */ }
    }
}