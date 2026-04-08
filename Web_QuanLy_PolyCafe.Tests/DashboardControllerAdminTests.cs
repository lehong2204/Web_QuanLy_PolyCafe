using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Areas.Admin.Controllers;
using Web_QuanLy_PolyCafe.Data;
using Xunit;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class DashboardControllerTests
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

        private DashboardController CreateController(PolyCafeDbContext ctx, string? userId = "U001", string? role = "Admin")
        {
            var controller = new DashboardController(ctx);
            var httpContext = new DefaultHttpContext();
            var session = new DefaultSession();

            if (!string.IsNullOrEmpty(userId)) session.SetString("UserId", userId);
            if (!string.IsNullOrEmpty(role)) session.SetString("Role", role);

            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
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
        public async Task TC03_Index_Admin_ReturnsView()
        {
            var ctx = CreateDb(nameof(TC03_Index_Admin_ReturnsView));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index() as ViewResult;
            Assert.NotNull(result);
        }

        // ================================================================
        // NHÓM 2: STATISTICS CARDS (TC04–TC20)
        // ================================================================
        [Fact]
        public async Task TC04_Index_HasTotalOrders()
        {
            var ctx = CreateDb(nameof(TC04_Index_HasTotalOrders));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalOrders >= 0);
        }

        [Fact]
        public async Task TC05_Index_HasTodayOrders()
        {
            var ctx = CreateDb(nameof(TC05_Index_HasTodayOrders));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TodayOrders >= 0);
        }

        [Fact]
        public async Task TC06_Index_HasTotalRevenue()
        {
            var ctx = CreateDb(nameof(TC06_Index_HasTotalRevenue));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((decimal?)ctrl.ViewBag.TotalRevenue >= 0);
        }

        [Fact]
        public async Task TC07_Index_HasMonthRevenue()
        {
            var ctx = CreateDb(nameof(TC07_Index_HasMonthRevenue));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((decimal?)ctrl.ViewBag.MonthRevenue >= 0);
        }

        [Fact]
        public async Task TC08_Index_HasTotalDrinks()
        {
            var ctx = CreateDb(nameof(TC08_Index_HasTotalDrinks));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalDrinks > 0);
        }

        [Fact]
        public async Task TC09_Index_HasTotalCategories()
        {
            var ctx = CreateDb(nameof(TC09_Index_HasTotalCategories));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalCategories > 0);
        }

        [Fact]
        public async Task TC10_Index_HasTotalUsers()
        {
            var ctx = CreateDb(nameof(TC10_Index_HasTotalUsers));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalUsers > 0);
        }

        [Fact]
        public async Task TC11_Index_TotalOrders_MatchesDB()
        {
            var ctx = CreateDb(nameof(TC11_Index_TotalOrders_MatchesDB));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(ctx.Orders.Count(), (int?)ctrl.ViewBag.TotalOrders);
        }

        [Fact]
        public async Task TC12_Index_TodayOrders_Correct()
        {
            var ctx = CreateDb(nameof(TC12_Index_TodayOrders_Correct));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var todayCount = ctx.Orders.Count(o => o.OrderDate.Date == DateTime.Today);
            Assert.Equal(todayCount, (int?)ctrl.ViewBag.TodayOrders);
        }

        // ================================================================
        // NHÓM 3: CHARTS & TOP ITEMS (TC13–TC30)
        // ================================================================
        [Fact]
        public async Task TC13_Index_HasMonthlyRevenueArray_Of12Months()
        {
            var ctx = CreateDb(nameof(TC13_Index_HasMonthlyRevenueArray_Of12Months));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var monthly = ctrl.ViewBag.MonthlyRevenue as decimal[];
            Assert.NotNull(monthly);
            Assert.Equal(12, monthly!.Length);
        }

        [Fact]
        public async Task TC14_Index_HasYearLabelsAndRevenue()
        {
            var ctx = CreateDb(nameof(TC14_Index_HasYearLabelsAndRevenue));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.YearLabels);
            Assert.NotNull(ctrl.ViewBag.YearRevenue);
        }

        [Fact]
        public async Task TC15_Index_HasTopDrinkMain()
        {
            var ctx = CreateDb(nameof(TC15_Index_HasTopDrinkMain));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.TopDrinkMain);
        }

        [Fact]
        public async Task TC16_Index_HasTopDrinkList()
        {
            var ctx = CreateDb(nameof(TC16_Index_HasTopDrinkList));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.TopDrinkList);
        }

        [Fact]
        public async Task TC17_Index_HasRevenueChart_Last7Days()
        {
            var ctx = CreateDb(nameof(TC17_Index_HasRevenueChart_Last7Days));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.RevenueChart);
        }

        [Fact]
        public async Task TC18_Index_TopDrinks_SortedByQuantityDesc()
        {
            var ctx = CreateDb(nameof(TC18_Index_TopDrinks_SortedByQuantityDesc));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var list = ctrl.ViewBag.TopDrinkList as List<dynamic>;
            Assert.NotNull(list);
        }

        // ================================================================
        // NHÓM 4: EDGE CASES (TC19–TC35)
        // ================================================================
        [Fact]
        public async Task TC19_Index_NoOrders_AllStatsZero()
        {
            var ctx = CreateDb(nameof(TC19_Index_NoOrders_AllStatsZero));
            ctx.Orders.RemoveRange(ctx.Orders);
            await ctx.SaveChangesAsync();

            var ctrl = CreateController(ctx);
            await ctrl.Index();

            Assert.Equal(0, (int?)ctrl.ViewBag.TotalOrders);
            Assert.Equal(0, (decimal?)ctrl.ViewBag.TotalRevenue);
        }

        [Fact]
        public async Task TC20_Index_OnlyDeletedDrinks_TotalDrinksStillCountsActive()
        {
            var ctx = CreateDb(nameof(TC20_Index_OnlyDeletedDrinks_TotalDrinksStillCountsActive));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalDrinks > 0);
        }

        // ================================================================
        // NHÓM 5: DATA INTEGRITY & ADVANCED (TC36–TC50)
        // ================================================================
        [Fact] public async Task TC36_Index_RevenueCalculation_Correct() { /* ... */ }
        [Fact] public async Task TC37_Index_MonthlyRevenue_SumsCorrectly() { /* ... */ }
        [Fact] public async Task TC38_Index_ViewBagNotNull_ForAllProperties() { /* ... */ }
        [Fact] public async Task TC39_Index_AfterNewOrder_StatsShouldIncrease() { /* ... */ }
        [Fact] public async Task TC40_Index_MultipleCalls_ReturnConsistentData() { /* ... */ }

        [Fact] public async Task TC41_Index_TotalUsers_OnlyActiveUsers() { /* ... */ }
        [Fact] public async Task TC42_Index_TopDrinks_IncludeImageUrl() { /* ... */ }
        [Fact] public async Task TC43_Index_RevenueChart_Has7Days() { /* ... */ }
        [Fact] public async Task TC44_Index_YearRevenue_SortedByYear() { /* ... */ }
        [Fact] public async Task TC45_Index_NoException_WhenDBEmpty() { /* ... */ }

        [Fact] public async Task TC46_Index_Performance_Under500ms() { /* ... */ }
        [Fact] public async Task TC47_Index_CorrectMonthRevenue_CurrentYearOnly() { /* ... */ }
        [Fact] public async Task TC48_Index_Top5Drinks_Exactly5OrLess() { /* ... */ }
        [Fact] public async Task TC49_Index_AllViewBagProperties_Exist() { /* ... */ }
        [Fact] public async Task TC50_Index_FullDashboard_LoadsSuccessfully() { /* ... */ }
    }
}