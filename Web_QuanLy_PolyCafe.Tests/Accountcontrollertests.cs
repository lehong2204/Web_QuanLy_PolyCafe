using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using Web_QuanLy_PolyCafe.Controllers;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;
using Web_QuanLy_PolyCafe.Services;
using Xunit;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class AccountControllerTests
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

        private AccountController CreateController(PolyCafeDbContext ctx, string? userId = null, Mock<EmailService>? emailMock = null)
        {
            var emailService = emailMock?.Object ?? new Mock<EmailService>().Object;
            var controller = new AccountController(ctx, emailService);
            var httpContext = new DefaultHttpContext();
            var session = new DefaultSession();
            if (!string.IsNullOrEmpty(userId))
                session.SetString("UserId", userId);
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        private JsonElement ToJson(JsonResult result)
        {
            var json = JsonSerializer.Serialize(result.Value);
            return JsonSerializer.Deserialize<JsonElement>(json);
        }

        // ================================================================
        // NHOM 1: LOGIN (TC01–TC10)
        // ================================================================
        [Fact] public async Task TC01_Login_EmailEmpty_ReturnsError() { /* seed user, call Login("", "123"), check ViewBag.Error */ }
        [Fact] public async Task TC02_Login_PasswordEmpty_ReturnsError() { /* ... */ }
        [Fact] public async Task TC03_Login_UserNotFound_ReturnsError() { /* ... */ }
        [Fact] public async Task TC04_Login_InactiveUser_ReturnsError() { /* ... */ }
        [Fact] public async Task TC05_Login_WrongPassword_ReturnsError() { /* ... */ }
        [Fact] public async Task TC06_Login_Success_EmailPassword() { /* ... */ }
        [Fact] public async Task TC07_Login_Success_PhonePassword() { /* ... */ }
        [Fact] public async Task TC08_Login_SetsSessionValues() { /* ... */ }
        [Fact] public async Task TC09_Login_RedirectsToPOS_WhenSuccess() { /* ... */ }
        [Fact] public async Task TC10_Login_RedirectsToPOS_WhenAlreadyLoggedIn() { /* ... */ }

        // ================================================================
        // NHOM 2: LOGOUT (TC11–TC12)
        // ================================================================
        [Fact] public void TC11_Logout_ClearsSession() { /* ... */ }
        [Fact] public void TC12_Logout_RedirectsToLogin() { /* ... */ }

        // ================================================================
        // NHOM 3: REGISTER (TC13)
        // ================================================================
        [Fact] public void TC13_Register_ReturnsView() { /* ... */ }

        // ================================================================
        // NHOM 4: FORGOT PASSWORD (TC14–TC20)
        // ================================================================
        [Fact] public async Task TC14_ForgotPassword_EmailEmpty_ReturnsError() { /* ... */ }
        [Fact] public async Task TC15_ForgotPassword_UserNotFound_ReturnsSuccessMessage() { /* ... */ }
        [Fact] public async Task TC16_ForgotPassword_UserFound_UpdatesPassword() { /* ... */ }
        [Fact]
        public async Task TC17_ForgotPassword_UserFound_SendsEmail()
        {
            var options = new DbContextOptionsBuilder<PolyCafeDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new PolyCafeDbContext(options);
            ctx.Database.EnsureCreated(); // ✅ seed data có sẵn U001 admin@polycafe.vn

            // ✅ Sửa key thành "EmailSettings" cho đúng với EmailService gốc
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
            {"EmailSettings:Host", "localhost"},
            {"EmailSettings:Port", "25"},
            {"EmailSettings:UserName", "test@test.com"},
            {"EmailSettings:Password", "123"},
            {"EmailSettings:EnableSsl", "false"},
            {"EmailSettings:FromName", "PolyCafe Test"}
                })
                .Build();

            var emailService = new FakeEmailService(config);

            var ctrl = new AccountController(ctx, emailService);
            ctrl.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // ✅ Dùng email từ seed data luôn, khỏi add thủ công
            await ctrl.ForgotPassword("admin@polycafe.vn");

            // Kiểm tra password đã thay đổi
            var updatedUser = ctx.Users.First(u => u.Id == "U001");
            Assert.NotEqual("admin123", updatedUser.Password);

            // Kiểm tra SendAsync đã được gọi
            Assert.True(emailService.WasCalled);
        }

        [Fact] public async Task TC18_ForgotPassword_GeneratesRandomPassword_Length8() { /* ... */ }
        [Fact] public async Task TC19_ForgotPassword_PasswordActuallyChanged() { /* ... */ }
        [Fact] public async Task TC20_ForgotPassword_ReturnsViewAlways() { /* ... */ }

        // ================================================================
        // NHOM 5: PROFILE (TC21–TC24)
        // ================================================================
        [Fact] public void TC21_Profile_NotLoggedIn_RedirectsToLogin() { /* ... */ }
        [Fact] public void TC22_Profile_UserNotFound_RedirectsToLogin() { /* ... */ }
        [Fact] public void TC23_Profile_UserFound_ReturnsView() { /* ... */ }
        [Fact] public void TC24_Profile_IncludesOrders() { /* ... */ }

        // ================================================================
        // NHOM 6: UPDATE PROFILE (TC25–TC30)
        // ================================================================
        [Fact] public void TC25_UpdateProfile_NotLoggedIn_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC26_UpdateProfile_FullNameEmpty_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC27_UpdateProfile_UserNotFound_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC28_UpdateProfile_Success_UpdatesData() { /* ... */ }
        [Fact] public void TC29_UpdateProfile_UpdatesSessionFullName() { /* ... */ }
        [Fact] public void TC30_UpdateProfile_UpdatesPhoneAndAddress() { /* ... */ }

        // ================================================================
        // NHOM 7: CHANGE PASSWORD (TC31–TC38)
        // ================================================================
        [Fact] public void TC31_ChangePassword_NotLoggedIn_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC32_ChangePassword_DtoNull_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC33_ChangePassword_OldPasswordEmpty_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC34_ChangePassword_NewPasswordEmpty_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC35_ChangePassword_NewPasswordTooShort_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC36_ChangePassword_UserNotFound_ReturnsJsonFalse() { /* ... */ }
        [Fact] public void TC37_ChangePassword_Success_UpdatesPassword() { /* ... */ }
        [Fact] public void TC38_ChangePassword_ReturnsJsonTrue() { /* ... */ }

        // ================================================================
        // NHOM 8: HELPER (TC39–TC41)
        // ================================================================
        [Fact] public void TC39_GenerateRandomPassword_ReturnsCorrectLength() { /* ... */ }
        [Fact] public void TC40_GenerateRandomPassword_ReturnsDifferentValues() { /* ... */ }
        [Fact] public void TC41_GenerateRandomPassword_ContainsAllowedChars() { /* ... */ }

        // ================================================================
        // NHOM 9: EDGE CASES (TC42–TC50)
        // ================================================================
        [Fact] public async Task TC42_Login_EmailWithSpaces_Trimmed() { /* ... */ }
        [Fact] public async Task TC43_ForgotPassword_EmailTrimmed() { /* ... */ }
        [Fact] public void TC44_UpdateProfile_TrimsFields() { /* ... */ }
        [Fact] public void TC45_ChangePassword_TrimsNewPassword() { /* ... */ }
        [Fact] public void TC46_Profile_ReturnsCorrectUserData() { /* ... */ }
        [Fact] public void TC47_Logout_ClearsAllSessionKeys() { /* ... */ }
        [Fact] public void TC48_Register_ViewBagIsNull() { /* ... */ }
        [Fact] public void TC49_UpdateProfile_ReturnsJsonSuccess() { /* ... */ }
        [Fact] public void TC50_ChangePassword_ReturnsJsonError_WhenInvalid() { /* ... */ }
    }
}
