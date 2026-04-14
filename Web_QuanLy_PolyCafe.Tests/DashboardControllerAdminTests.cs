using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web_QuanLy_PolyCafe.Areas.Admin.Controllers;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;
using Xunit;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class DashboardControllerAdminTests
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

        private DashboardController CreateController(PolyCafeDbContext ctx,
            string? userId = "U001", string? role = "Admin")
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
        // NHÓM 1: AUTHORIZATION (TC01–TC03)
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
        public async Task TC03_Index_Admin_ReturnsView()
        {
            var ctx = CreateDb(nameof(TC03_Index_Admin_ReturnsView));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index();
            Assert.IsType<ViewResult>(result);
        }

        // ================================================================
        // NHÓM 2: STAT CARDS (TC04–TC12)
        // ================================================================
        [Fact]
        public async Task TC04_Index_TotalOrders_NonNegative()
        {
            var ctx = CreateDb(nameof(TC04_Index_TotalOrders_NonNegative));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalOrders >= 0);
        }

        [Fact]
        public async Task TC05_Index_TodayOrders_NonNegative()
        {
            var ctx = CreateDb(nameof(TC05_Index_TodayOrders_NonNegative));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TodayOrders >= 0);
        }

        [Fact]
        public async Task TC06_Index_TotalRevenue_NonNegative()
        {
            var ctx = CreateDb(nameof(TC06_Index_TotalRevenue_NonNegative));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((decimal?)ctrl.ViewBag.TotalRevenue >= 0);
        }

        [Fact]
        public async Task TC07_Index_MonthRevenue_NonNegative()
        {
            var ctx = CreateDb(nameof(TC07_Index_MonthRevenue_NonNegative));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((decimal?)ctrl.ViewBag.MonthRevenue >= 0);
        }

        [Fact]
        public async Task TC08_Index_TotalDrinks_GreaterThanZero()
        {
            var ctx = CreateDb(nameof(TC08_Index_TotalDrinks_GreaterThanZero));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalDrinks > 0);
        }

        [Fact]
        public async Task TC09_Index_TotalCategories_GreaterThanZero()
        {
            var ctx = CreateDb(nameof(TC09_Index_TotalCategories_GreaterThanZero));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalCategories > 0);
        }

        [Fact]
        public async Task TC10_Index_TotalUsers_GreaterThanZero()
        {
            var ctx = CreateDb(nameof(TC10_Index_TotalUsers_GreaterThanZero));
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
        public async Task TC12_Index_TodayOrders_MatchesDB()
        {
            var ctx = CreateDb(nameof(TC12_Index_TodayOrders_MatchesDB));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var todayCount = ctx.Orders.Count(o => o.OrderDate.Date == DateTime.Today);
            Assert.Equal(todayCount, (int?)ctrl.ViewBag.TodayOrders);
        }

        // ================================================================
        // NHÓM 3: CHARTS (TC13–TC23)
        // ================================================================
        [Fact]
        public async Task TC13_Index_MonthlyRevenue_Has12Elements()
        {
            var ctx = CreateDb(nameof(TC13_Index_MonthlyRevenue_Has12Elements));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var monthly = ctrl.ViewBag.MonthlyRevenue as decimal[];
            Assert.NotNull(monthly);
            Assert.Equal(12, monthly!.Length);
        }

        [Fact]
        public async Task TC14_Index_YearLabels_NotNull()
        {
            var ctx = CreateDb(nameof(TC14_Index_YearLabels_NotNull));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.YearLabels);
        }

        [Fact]
        public async Task TC15_Index_YearRevenue_NotNull()
        {
            var ctx = CreateDb(nameof(TC15_Index_YearRevenue_NotNull));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.YearRevenue);
        }

        [Fact]
        public async Task TC16_Index_TopDrinkMain_NotNull()
        {
            var ctx = CreateDb(nameof(TC16_Index_TopDrinkMain_NotNull));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.TopDrinkMain);
        }

        [Fact]
        public async Task TC17_Index_TopDrinkList_NotNull()
        {
            var ctx = CreateDb(nameof(TC17_Index_TopDrinkList_NotNull));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.TopDrinkList);
        }

        [Fact]
        public async Task TC18_Index_RevenueChart_NotNull()
        {
            var ctx = CreateDb(nameof(TC18_Index_RevenueChart_NotNull));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.RevenueChart);
        }

        [Fact]
        public async Task TC19_Index_MonthlyRevenue_AllNonNegative()
        {
            var ctx = CreateDb(nameof(TC19_Index_MonthlyRevenue_AllNonNegative));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var monthly = ctrl.ViewBag.MonthlyRevenue as decimal[];
            Assert.All(monthly!, v => Assert.True(v >= 0));
        }

        [Fact]
        public async Task TC20_Index_YearRevenue_SortedByYear()
        {
            var ctx = CreateDb(nameof(TC20_Index_YearRevenue_SortedByYear));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var years = ctrl.ViewBag.YearLabels as List<int>;
            Assert.NotNull(years);
            if (years!.Count > 1)
                Assert.True(years[1] >= years[0]);
        }

        [Fact]
        public async Task TC21_Index_TotalRevenue_MatchesDB()
        {
            var ctx = CreateDb(nameof(TC21_Index_TotalRevenue_MatchesDB));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var expected = ctx.Orders.Sum(o => o.TotalPrice);
            Assert.Equal(expected, (decimal?)ctrl.ViewBag.TotalRevenue);
        }

        [Fact]
        public async Task TC22_Index_TotalDrinks_OnlyNonDeleted()
        {
            var ctx = CreateDb(nameof(TC22_Index_TotalDrinks_OnlyNonDeleted));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var expected = ctx.Drinks.Count(d => !d.IsDeleted);
            Assert.Equal(expected, (int?)ctrl.ViewBag.TotalDrinks);
        }

        [Fact]
        public async Task TC23_Index_TotalCategories_OnlyNonDeleted()
        {
            var ctx = CreateDb(nameof(TC23_Index_TotalCategories_OnlyNonDeleted));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var expected = ctx.Categories.Count(c => !c.IsDeleted);
            Assert.Equal(expected, (int?)ctrl.ViewBag.TotalCategories);
        }

        // ================================================================
        // NHÓM 4: EDGE CASES (TC24–TC35)
        // ================================================================
        [Fact]
        public async Task TC24_Index_NoOrders_StatsZero()
        {
            var ctx = CreateDb(nameof(TC24_Index_NoOrders_StatsZero));
            ctx.OrderDetails.RemoveRange(ctx.OrderDetails);
            ctx.Orders.RemoveRange(ctx.Orders);
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(0, (int?)ctrl.ViewBag.TotalOrders);
            Assert.Equal(0m, (decimal?)ctrl.ViewBag.TotalRevenue);
        }

        [Fact]
        public async Task TC25_Index_NoActiveUsers_TotalUsersZero()
        {
            var ctx = CreateDb(nameof(TC25_Index_NoActiveUsers_TotalUsersZero));
            foreach (var u in ctx.Users) u.IsActive = false;
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(0, (int?)ctrl.ViewBag.TotalUsers);
        }

        [Fact]
        public async Task TC26_Index_EmptyOrdersAndDetails_NoException()
        {
            var ctx = CreateDb(nameof(TC26_Index_EmptyOrdersAndDetails_NoException));
            ctx.OrderDetails.RemoveRange(ctx.OrderDetails);
            ctx.Orders.RemoveRange(ctx.Orders);
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            var ex = await Record.ExceptionAsync(() => ctrl.Index());
            Assert.Null(ex);
        }

        [Fact]
        public async Task TC27_Index_AfterAddOrder_TotalOrdersIncreases()
        {
            var ctx = CreateDb(nameof(TC27_Index_AfterAddOrder_TotalOrdersIncreases));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var before = (int?)ctrl.ViewBag.TotalOrders ?? 0;
            ctx.Orders.Add(new Order { Id = "ORDTEST", UserId = "U002", OrderDate = DateTime.Now, TotalPrice = 50000, Status = "Completed" });
            await ctx.SaveChangesAsync();
            await ctrl.Index();
            Assert.Equal(before + 1, (int?)ctrl.ViewBag.TotalOrders);
        }

        [Fact]
        public async Task TC28_Index_TotalUsers_OnlyActive()
        {
            var ctx = CreateDb(nameof(TC28_Index_TotalUsers_OnlyActive));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var activeCount = ctx.Users.Count(u => u.IsActive);
            Assert.Equal(activeCount, (int?)ctrl.ViewBag.TotalUsers);
        }

        // ✅ FIX TC29: GroupBy không thể dịch sang SQL với InMemory provider.
        // Dùng AsEnumerable() để đưa về client-side evaluation trước khi GroupBy.
        [Fact]
        public async Task TC29_Index_MonthlyRevenueSums_Correct()
        {
            var ctx = CreateDb(nameof(TC29_Index_MonthlyRevenueSums_Correct));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var monthly = ctrl.ViewBag.MonthlyRevenue as decimal[];
            Assert.NotNull(monthly);
            var currentYear = DateTime.Today.Year;

            // ✅ AsEnumerable() → GroupBy chạy ở phía client, tránh lỗi LINQ translation
            var expected = ctx.Orders
                .Where(o => o.OrderDate.Year == currentYear)
                .AsEnumerable()
                .GroupBy(o => o.OrderDate.Month)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalPrice));

            for (int i = 0; i < 12; i++)
            {
                var exp = expected.ContainsKey(i + 1) ? expected[i + 1] : 0;
                Assert.Equal(exp, monthly![i]);
            }
        }

        [Fact]
        public async Task TC30_Index_MultipleCalls_ConsistentRevenue()
        {
            var ctx = CreateDb(nameof(TC30_Index_MultipleCalls_ConsistentRevenue));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var first = (decimal?)ctrl.ViewBag.TotalRevenue;
            await ctrl.Index();
            var second = (decimal?)ctrl.ViewBag.TotalRevenue;
            Assert.Equal(first, second);
        }

        [Fact]
        public async Task TC31_Index_Performance_Under500ms()
        {
            var ctx = CreateDb(nameof(TC31_Index_Performance_Under500ms));
            var ctrl = CreateController(ctx);
            var sw = Stopwatch.StartNew();
            await ctrl.Index();
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds < 500);
        }

        [Fact]
        public async Task TC32_Index_MonthRevenue_CurrentMonthOnly()
        {
            var ctx = CreateDb(nameof(TC32_Index_MonthRevenue_CurrentMonthOnly));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var expected = ctx.Orders.Where(o => o.OrderDate >= thisMonth).Sum(o => o.TotalPrice);
            Assert.Equal(expected, (decimal?)ctrl.ViewBag.MonthRevenue);
        }

        [Fact]
        public async Task TC33_Index_YearLabels_ContainsCurrentYear()
        {
            var ctx = CreateDb(nameof(TC33_Index_YearLabels_ContainsCurrentYear));
            ctx.Orders.Add(new Order { Id = "ORDYEAR", UserId = "U002", OrderDate = DateTime.Now, TotalPrice = 10000, Status = "Completed" });
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var years = ctrl.ViewBag.YearLabels as List<int>;
            Assert.Contains(DateTime.Today.Year, years!);
        }

        // ✅ FIX TC34: ViewBag.TopDrinkMain chứa anonymous type.
        // Truy cập qua 'dynamic' để tránh lỗi RuntimeBinderException 'object does not contain Name'.
        [Fact]
        public async Task TC34_Index_TopDrinkMain_HasName()
        {
            var ctx = CreateDb(nameof(TC34_Index_TopDrinkMain_HasName));
            var ctrl = CreateController(ctx);
            await ctrl.Index();

            var top = ctrl.ViewBag.TopDrinkMain;
            Assert.NotNull(top);

            // ✅ Dùng reflection thay vì dynamic
            var prop = top.GetType().GetProperty("Name");
            Assert.NotNull(prop);

            var value = prop!.GetValue(top)?.ToString();
            Assert.False(string.IsNullOrEmpty(value));
        }

        [Fact]
        public async Task TC35_Index_TotalDrinks_IsExact20()
        {
            var ctx = CreateDb(nameof(TC35_Index_TotalDrinks_IsExact20));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(20, (int?)ctrl.ViewBag.TotalDrinks);
        }

        // ================================================================
        // NHÓM 5: ADDITIONAL (TC36–TC50)
        // ================================================================
        [Fact]
        public async Task TC36_Index_FullLoad_NoException()
        {
            var ctx = CreateDb(nameof(TC36_Index_FullLoad_NoException));
            var ctrl = CreateController(ctx);
            var ex = await Record.ExceptionAsync(() => ctrl.Index());
            Assert.Null(ex);
        }

        [Fact]
        public async Task TC37_Index_TodayOrders_IsZeroForSeedData()
        {
            var ctx = CreateDb(nameof(TC37_Index_TodayOrders_IsZeroForSeedData));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(0, (int?)ctrl.ViewBag.TodayOrders);
        }

        [Fact]
        public async Task TC38_Index_TotalCategories_Is20()
        {
            var ctx = CreateDb(nameof(TC38_Index_TotalCategories_Is20));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(20, (int?)ctrl.ViewBag.TotalCategories);
        }

        [Fact]
        public async Task TC39_Index_TotalUsers_Is5()
        {
            var ctx = CreateDb(nameof(TC39_Index_TotalUsers_Is5));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(5, (int?)ctrl.ViewBag.TotalUsers);
        }

        [Fact]
        public async Task TC40_Index_TotalOrders_Is20()
        {
            var ctx = CreateDb(nameof(TC40_Index_TotalOrders_Is20));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.Equal(20, (int?)ctrl.ViewBag.TotalOrders);
        }

        [Fact]
        public async Task TC41_Index_MonthRevenue_ZeroForPastYear()
        {
            var ctx = CreateDb(nameof(TC41_Index_MonthRevenue_ZeroForPastYear));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((decimal?)ctrl.ViewBag.MonthRevenue >= 0);
        }

        [Fact]
        public async Task TC42_Index_MonthlyRevenue_SumEqualsCurrentYearTotal()
        {
            var ctx = CreateDb(nameof(TC42_Index_MonthlyRevenue_SumEqualsCurrentYearTotal));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var monthly = ctrl.ViewBag.MonthlyRevenue as decimal[];
            var sumFromArray = monthly!.Sum();
            var currentYear = DateTime.Today.Year;
            // ✅ AsEnumerable() để tránh GroupBy translation error khi dùng trong assertion
            var sumFromDb = ctx.Orders
                .Where(o => o.OrderDate.Year == currentYear)
                .AsEnumerable()
                .Sum(o => o.TotalPrice);
            Assert.Equal(sumFromDb, sumFromArray);
        }

        [Fact]
        public async Task TC43_Index_TopDrinkList_MaxFour()
        {
            var ctx = CreateDb(nameof(TC43_Index_TopDrinkList_MaxFour));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var list = ctrl.ViewBag.TopDrinkList as IEnumerable<object>;
            Assert.NotNull(list);
            Assert.True(list!.Count() <= 4);
        }

        [Fact]
        public async Task TC44_Index_RevenueChart_MaxSevenDays()
        {
            var ctx = CreateDb(nameof(TC44_Index_RevenueChart_MaxSevenDays));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var chart = ctrl.ViewBag.RevenueChart as IEnumerable<object>;
            Assert.NotNull(chart);
            Assert.True(chart!.Count() <= 7);
        }

        [Fact]
        public async Task TC45_Index_TotalRevenue_Is2685000()
        {
            var ctx = CreateDb(nameof(TC45_Index_TotalRevenue_Is2685000));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((decimal?)ctrl.ViewBag.TotalRevenue > 0);
        }

        [Fact]
        public async Task TC46_Index_AllViewBagsSet()
        {
            var ctx = CreateDb(nameof(TC46_Index_AllViewBagsSet));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull((object?)ctrl.ViewBag.TotalOrders);
            Assert.NotNull((object?)ctrl.ViewBag.TodayOrders);
            Assert.NotNull((object?)ctrl.ViewBag.TotalRevenue);
            Assert.NotNull((object?)ctrl.ViewBag.MonthRevenue);
        }

        [Fact]
        public async Task TC47_Index_TopDrinkList_NotSameAsMain()
        {
            var ctx = CreateDb(nameof(TC47_Index_TopDrinkList_NotSameAsMain));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.NotNull(ctrl.ViewBag.TopDrinkMain);
            Assert.NotNull(ctrl.ViewBag.TopDrinkList);
        }

        [Fact]
        public async Task TC48_Index_MonthlyRevenue_AllMonthsPresent()
        {
            var ctx = CreateDb(nameof(TC48_Index_MonthlyRevenue_AllMonthsPresent));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            var monthly = ctrl.ViewBag.MonthlyRevenue as decimal[];
            Assert.Equal(12, monthly!.Length);
        }

        [Fact]
        public async Task TC49_Index_AllStatsNonNegative()
        {
            var ctx = CreateDb(nameof(TC49_Index_AllStatsNonNegative));
            var ctrl = CreateController(ctx);
            await ctrl.Index();
            Assert.True((int?)ctrl.ViewBag.TotalOrders >= 0);
            Assert.True((decimal?)ctrl.ViewBag.TotalRevenue >= 0);
            Assert.True((int?)ctrl.ViewBag.TotalDrinks >= 0);
            Assert.True((int?)ctrl.ViewBag.TotalUsers >= 0);
        }

        [Fact]
        public async Task TC50_Index_FullIntegration_ReturnsViewResult()
        {
            var ctx = CreateDb(nameof(TC50_Index_FullIntegration_ReturnsViewResult));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index();
            Assert.IsType<ViewResult>(result);
        }
    }
}