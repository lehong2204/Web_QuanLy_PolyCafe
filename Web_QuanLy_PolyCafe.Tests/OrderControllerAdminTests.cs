using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web_QuanLy_PolyCafe.Areas.Admin.Controllers;
using Web_QuanLy_PolyCafe.Data;
using Xunit;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class OrderControllerAdminTests
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

        private OrderController CreateController(PolyCafeDbContext ctx, string? userId = "U001", string? role = "Admin")
        {
            var controller = new OrderController(ctx);
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

        // ============================
        // NHÓM 1: AUTHORIZATION (TC01–TC08)
        // ============================
        [Fact] public async Task TC01_Index_NotLoggedIn_RedirectsToLogin() { /* ... */ }
        [Fact] public async Task TC02_Index_Staff_RedirectsToPOS() { /* ... */ }
        [Fact] public async Task TC03_Index_Admin_ReturnsView() { /* ... */ }
        [Fact] public async Task TC04_Detail_NotLoggedIn_RedirectsToLogin() { /* ... */ }
        [Fact] public async Task TC05_Detail_Staff_RedirectsToPOS() { /* ... */ }
        [Fact] public async Task TC06_Detail_Admin_ReturnsView() { /* ... */ }
        [Fact] public async Task TC07_Detail_OrderNotFound_ReturnsNotFound() { /* ... */ }
        [Fact] public async Task TC08_Index_ViewBagContainsSearchAndDate() { /* ... */ }

        // ============================
        // NHÓM 2: INDEX FILTER (TC09–TC20)
        // ============================
        [Fact] public async Task TC09_Index_SearchByOrderId_Works() { /* ... */ }
        [Fact] public async Task TC10_Index_SearchByUserName_Works() { /* ... */ }
        [Fact] public async Task TC11_Index_FilterByDate_Works() { /* ... */ }
        [Fact] public async Task TC12_Index_ReturnsOrdersDescending() { /* ... */ }
        [Fact] public async Task TC13_Index_EmptyDatabase_ReturnsEmptyList() { /* ... */ }
        [Fact] public async Task TC14_Index_IncludesUserAndDetails() { /* ... */ }
        [Fact] public async Task TC15_Index_IncludesVoucherIfAny() { /* ... */ }
        [Fact] public async Task TC16_Index_MultipleOrders_ReturnsAll() { /* ... */ }
        [Fact] public async Task TC17_Index_SearchCaseInsensitive() { /* ... */ }
        [Fact] public async Task TC18_Index_FilterInvalidDate_Ignores() { /* ... */ }
        [Fact] public async Task TC19_Index_ViewModelIsListOfOrders() { /* ... */ }
        [Fact] public async Task TC20_Index_OrderDetailsIncluded() { /* ... */ }

        // ============================
        // NHÓM 3: DETAIL (TC21–TC30)
        // ============================
        [Fact] public async Task TC21_Detail_ValidOrder_ReturnsOrder() { /* ... */ }
        [Fact] public async Task TC22_Detail_IncludesUserVoucherDetails() { /* ... */ }
        [Fact] public async Task TC23_Detail_OrderHasVariants() { /* ... */ }
        [Fact] public async Task TC24_Detail_OrderWithoutVoucher_StillWorks() { /* ... */ }
        [Fact] public async Task TC25_Detail_OrderWithoutDetails_StillWorks() { /* ... */ }
        [Fact] public async Task TC26_Detail_OrderWithMultipleDetails_ReturnsAll() { /* ... */ }
        [Fact] public async Task TC27_Detail_OrderWithDeletedDrink_StillShows() { /* ... */ }
        [Fact] public async Task TC28_Detail_OrderWithInactiveUser_StillShows() { /* ... */ }
        [Fact] public async Task TC29_Detail_OrderWithNullVariant_StillWorks() { /* ... */ }
        [Fact] public async Task TC30_Detail_ViewModelIsOrder() { /* ... */ }

        // ============================
        // NHÓM 4: EDGE CASES (TC31–TC50)
        // ============================
        [Fact] public async Task TC31_Index_SearchEmpty_ReturnsAll() { /* ... */ }
        [Fact] public async Task TC32_Index_SearchWhitespace_ReturnsAll() { /* ... */ }
        [Fact] public async Task TC33_Index_SearchNonExisting_ReturnsEmpty() { /* ... */ }
        [Fact] public async Task TC34_Detail_IdEmpty_ReturnsNotFound() { /* ... */ }
        [Fact] public async Task TC35_Detail_IdWhitespace_ReturnsNotFound() { /* ... */ }
        [Fact] public async Task TC36_Index_Performance_Under500ms() { /* ... */ }
        [Fact] public async Task TC37_Index_MultipleCalls_ConsistentData() { /* ... */ }
        [Fact] public async Task TC38_Detail_MultipleCalls_ConsistentData() { /* ... */ }
        [Fact] public async Task TC39_Index_ReturnsOrdersWithCorrectUserNames() { /* ... */ }
        [Fact] public async Task TC40_Index_ReturnsOrdersWithCorrectVoucherCodes() { /* ... */ }
        [Fact] public async Task TC41_Detail_ReturnsCorrectTotalPrice() { /* ... */ }
        [Fact] public async Task TC42_Detail_ReturnsCorrectDiscountAmount() { /* ... */ }
        [Fact] public async Task TC43_Detail_ReturnsCorrectStatus() { /* ... */ }
        [Fact] public async Task TC44_Index_ReturnsCorrectOrderCount() { /* ... */ }
        [Fact] public async Task TC45_Index_ReturnsCorrectDrinkNames() { /* ... */ }
        [Fact] public async Task TC46_Index_ReturnsCorrectVariantNames() { /* ... */ }
        [Fact] public async Task TC47_Index_ReturnsCorrectCategoryNames() { /* ... */ }
        [Fact] public async Task TC48_Detail_ReturnsCorrectCreatedAt() { /* ... */ }
        [Fact] public async Task TC49_Detail_ReturnsCorrectUserEmail() { /* ... */ }
        [Fact] public async Task TC50_FullOrderCycle_Works() { /* ... */ }
    }
}
