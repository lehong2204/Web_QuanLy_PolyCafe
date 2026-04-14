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

        private OrderController CreateController(PolyCafeDbContext ctx,
            string? userId = "U001", string? role = "Admin")
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

        // ================================================================
        // NHÓM 1: AUTHORIZATION (TC01–TC08)
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
        public async Task TC03_Index_Admin_ReturnsView()
        {
            var ctx = CreateDb(nameof(TC03_Index_Admin_ReturnsView));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            Assert.NotNull(result);
            Assert.IsType<List<Order>>(result!.Model);
        }

        [Fact]
        public async Task TC04_Detail_NotLoggedIn_RedirectsToLogin()
        {
            var ctx = CreateDb(nameof(TC04_Detail_NotLoggedIn_RedirectsToLogin));
            var ctrl = CreateController(ctx, userId: null);
            var result = await ctrl.Detail("ORD001") as RedirectToActionResult;
            Assert.Equal("Login", result!.ActionName);
        }

        [Fact]
        public async Task TC05_Detail_Staff_RedirectsToPOS()
        {
            var ctx = CreateDb(nameof(TC05_Detail_Staff_RedirectsToPOS));
            var ctrl = CreateController(ctx, "U002", "Staff");
            var result = await ctrl.Detail("ORD001") as RedirectToActionResult;
            Assert.Equal("POS", result!.ActionName);
        }

        [Fact]
        public async Task TC06_Detail_Admin_ReturnsView()
        {
            var ctx = CreateDb(nameof(TC06_Detail_Admin_ReturnsView));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TC07_Detail_NotFound_ReturnsNotFound()
        {
            var ctx = CreateDb(nameof(TC07_Detail_NotFound_ReturnsNotFound));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD999");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task TC08_Index_ReturnsOrderDescByDate()
        {
            var ctx = CreateDb(nameof(TC08_Index_ReturnsOrderDescByDate));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            if (model.Count > 1)
                Assert.True(model[0].OrderDate >= model[1].OrderDate);
        }

        // ================================================================
        // NHÓM 2: INDEX WITH SEARCH & DATE (TC09–TC20)
        // ================================================================
        [Fact]
        public async Task TC09_Index_SearchById_FiltersProperly()
        {
            var ctx = CreateDb(nameof(TC09_Index_SearchById_FiltersProperly));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("ORD001", null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Contains(model, o => o.Id == "ORD001");
        }

        [Fact]
        public async Task TC10_Index_SearchByUserName_FiltersProperly()
        {
            var ctx = CreateDb(nameof(TC10_Index_SearchByUserName_FiltersProperly));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("Nguyễn Văn An", null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.All(model, o => Assert.NotNull(o.User));
        }

        [Fact]
        public async Task TC11_Index_FilterByDate_Works()
        {
            var ctx = CreateDb(nameof(TC11_Index_FilterByDate_Works));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "2025-01-10") as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.All(model, o => Assert.Equal(new DateTime(2025, 1, 10), o.OrderDate.Date));
        }

        [Fact]
        public async Task TC12_Index_EmptySearch_ReturnsAll()
        {
            var ctx = CreateDb(nameof(TC12_Index_EmptySearch_ReturnsAll));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("", null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Equal(20, model.Count);
        }

        [Fact]
        public async Task TC13_Index_NoMatch_ReturnsEmpty()
        {
            var ctx = CreateDb(nameof(TC13_Index_NoMatch_ReturnsEmpty));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("XYZNOTEXIST", null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task TC14_Index_Returns20SeedOrders()
        {
            var ctx = CreateDb(nameof(TC14_Index_Returns20SeedOrders));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Equal(20, model.Count);
        }

        [Fact]
        public async Task TC15_Index_IncludesUser()
        {
            var ctx = CreateDb(nameof(TC15_Index_IncludesUser));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.All(model, o => Assert.NotNull(o.User));
        }

        [Fact]
        public async Task TC16_Index_IncludesOrderDetails()
        {
            var ctx = CreateDb(nameof(TC16_Index_IncludesOrderDetails));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.All(model, o => Assert.NotNull(o.OrderDetails));
        }

        [Fact]
        public async Task TC17_Index_ViewBagSearch_Persisted()
        {
            var ctx = CreateDb(nameof(TC17_Index_ViewBagSearch_Persisted));
            var ctrl = CreateController(ctx);
            await ctrl.Index("testSearch", null);
            Assert.Equal("testSearch", ctrl.ViewBag.Search);
        }

        [Fact]
        public async Task TC18_Index_ViewBagDate_Persisted()
        {
            var ctx = CreateDb(nameof(TC18_Index_ViewBagDate_Persisted));
            var ctrl = CreateController(ctx);
            await ctrl.Index(null, "2025-01-15");
            Assert.Equal("2025-01-15", ctrl.ViewBag.Date);
        }

        [Fact]
        public async Task TC19_Index_InvalidDate_ReturnsAll()
        {
            var ctx = CreateDb(nameof(TC19_Index_InvalidDate_ReturnsAll));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "not-a-date") as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Equal(20, model.Count);
        }

        [Fact]
        public async Task TC20_Index_DateWithNoOrders_ReturnsEmpty()
        {
            var ctx = CreateDb(nameof(TC20_Index_DateWithNoOrders_ReturnsEmpty));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "2000-01-01") as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Empty(model);
        }

        // ================================================================
        // NHÓM 3: DETAIL (TC21–TC35)
        // ================================================================
        [Fact]
        public async Task TC21_Detail_ReturnsCorrectOrder()
        {
            var ctx = CreateDb(nameof(TC21_Detail_ReturnsCorrectOrder));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = Assert.IsType<Order>(result!.Model);
            Assert.Equal("ORD001", model.Id);
        }

        [Fact]
        public async Task TC22_Detail_IncludesUser()
        {
            var ctx = CreateDb(nameof(TC22_Detail_IncludesUser));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.NotNull(model!.User);
        }

        [Fact]
        public async Task TC23_Detail_IncludesVoucher_WhenPresent()
        {
            var ctx = CreateDb(nameof(TC23_Detail_IncludesVoucher_WhenPresent));
            var ctrl = CreateController(ctx);
            // ORD002 has VoucherId = "V001"
            var result = await ctrl.Detail("ORD002") as ViewResult;
            var model = result!.Model as Order;
            Assert.NotNull(model!.Voucher);
        }

        [Fact]
        public async Task TC24_Detail_IncludesOrderDetails()
        {
            var ctx = CreateDb(nameof(TC24_Detail_IncludesOrderDetails));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.NotNull(model!.OrderDetails);
            Assert.True(model.OrderDetails!.Any());
        }

        [Fact]
        public async Task TC25_Detail_OrderDetails_IncludeDrink()
        {
            var ctx = CreateDb(nameof(TC25_Detail_OrderDetails_IncludeDrink));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.All(model!.OrderDetails!, od => Assert.NotNull(od.Drink));
        }

        [Fact]
        public async Task TC26_Detail_VoucherNull_WhenNoVoucher()
        {
            var ctx = CreateDb(nameof(TC26_Detail_VoucherNull_WhenNoVoucher));
            var ctrl = CreateController(ctx);
            // ORD001 has no voucher
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.Null(model!.Voucher);
        }

        [Fact]
        public async Task TC27_Detail_CorrectTotalPrice()
        {
            var ctx = CreateDb(nameof(TC27_Detail_CorrectTotalPrice));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal(80000m, model!.TotalPrice);
        }

        [Fact]
        public async Task TC28_Detail_CorrectStatus()
        {
            var ctx = CreateDb(nameof(TC28_Detail_CorrectStatus));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal("Completed", model!.Status);
        }

        [Fact]
        public async Task TC29_Detail_CorrectUserId()
        {
            var ctx = CreateDb(nameof(TC29_Detail_CorrectUserId));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD002") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal("U003", model!.UserId);
        }

        [Fact]
        public async Task TC30_Detail_DiscountAmount_Correct()
        {
            var ctx = CreateDb(nameof(TC30_Detail_DiscountAmount_Correct));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD002") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal(9000m, model!.DiscountAmount);
        }

        [Fact]
        public async Task TC31_Detail_OrderDate_Correct()
        {
            var ctx = CreateDb(nameof(TC31_Detail_OrderDate_Correct));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal(new DateTime(2025, 1, 10, 8, 30, 0), model!.OrderDate);
        }

        [Fact]
        public async Task TC32_Detail_MultipleOrderDetails_ORD003()
        {
            var ctx = CreateDb(nameof(TC32_Detail_MultipleOrderDetails_ORD003));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD003") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal(2, model!.OrderDetails!.Count);
        }

        [Fact]
        public async Task TC33_Detail_OrderDetail_IncludesVariant_WhenPresent()
        {
            var ctx = CreateDb(nameof(TC33_Detail_OrderDetail_IncludesVariant_WhenPresent));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            var detailWithVariant = model!.OrderDetails!.FirstOrDefault(od => od.VariantId != null);
            if (detailWithVariant != null)
                Assert.NotNull(detailWithVariant.Variant);
        }

        [Fact]
        public async Task TC34_Index_SearchByPartialId_Works()
        {
            var ctx = CreateDb(nameof(TC34_Index_SearchByPartialId_Works));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("ORD", null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.True(model.Count > 0);
        }

        [Fact]
        public async Task TC35_Detail_ORD006_StatusPending()
        {
            var ctx = CreateDb(nameof(TC35_Detail_ORD006_StatusPending));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD006") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal("Pending", model!.Status);
        }

        // ================================================================
        // NHÓM 4: EDGE CASES (TC36–TC50)
        // ================================================================
        [Fact]
        public async Task TC36_Index_AfterAddOrder_CountIs21()
        {
            var ctx = CreateDb(nameof(TC36_Index_AfterAddOrder_CountIs21));
            ctx.Orders.Add(new Order { Id = "ORDNEW", UserId = "U002", OrderDate = DateTime.Now, TotalPrice = 50000, Status = "Completed" });
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Equal(21, model.Count);
        }

        [Fact]
        public async Task TC37_Index_OrdersWithNullVoucher_Handled()
        {
            var ctx = CreateDb(nameof(TC37_Index_OrdersWithNullVoucher_Handled));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.True(model.Any(o => o.VoucherId == null));
        }

        [Fact]
        public async Task TC38_Index_AllOrdersHaveUser()
        {
            var ctx = CreateDb(nameof(TC38_Index_AllOrdersHaveUser));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.All(model, o => Assert.NotNull(o.User));
        }

        [Fact]
        public async Task TC39_Index_SearchDate_2025_01_11_Returns1()
        {
            var ctx = CreateDb(nameof(TC39_Index_SearchDate_2025_01_11_Returns1));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "2025-01-11") as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Equal(1, model.Count);
        }

        [Fact]
        public async Task TC40_Detail_ORD008_StatusCancelled()
        {
            var ctx = CreateDb(nameof(TC40_Detail_ORD008_StatusCancelled));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD008") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal("Cancelled", model!.Status);
        }

        [Fact]
        public async Task TC41_Index_SearchByDate_NoResults_Returns0()
        {
            var ctx = CreateDb(nameof(TC41_Index_SearchByDate_NoResults_Returns0));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, "2099-01-01") as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task TC42_Detail_ORD009_HasDiscount40000()
        {
            var ctx = CreateDb(nameof(TC42_Detail_ORD009_HasDiscount40000));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD009") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal(40000m, model!.DiscountAmount);
        }

        [Fact]
        public async Task TC43_Index_MultipleCallsConsistent()
        {
            var ctx = CreateDb(nameof(TC43_Index_MultipleCallsConsistent));
            var ctrl = CreateController(ctx);
            var r1 = (await ctrl.Index(null, null) as ViewResult)!.Model as List<Order>;
            var r2 = (await ctrl.Index(null, null) as ViewResult)!.Model as List<Order>;
            Assert.Equal(r1!.Count, r2!.Count);
        }

        [Fact]
        public async Task TC44_Detail_ORD020_HasLargeDiscount()
        {
            var ctx = CreateDb(nameof(TC44_Detail_ORD020_HasLargeDiscount));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD020") as ViewResult;
            var model = result!.Model as Order;
            Assert.Equal(56250m, model!.DiscountAmount);
        }

        [Fact]
        public async Task TC45_Index_SearchId_ORD010_ReturnsOne()
        {
            var ctx = CreateDb(nameof(TC45_Index_SearchId_ORD010_ReturnsOne));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index("ORD010", null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            Assert.Contains(model, o => o.Id == "ORD010");
        }

        [Fact]
        public async Task TC46_Index_AllOrderDetails_HaveDrink()
        {
            var ctx = CreateDb(nameof(TC46_Index_AllOrderDetails_HaveDrink));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Index(null, null) as ViewResult;
            var model = Assert.IsType<List<Order>>(result!.Model);
            foreach (var order in model)
                if (order.OrderDetails != null)
                    Assert.All(order.OrderDetails, od => Assert.NotNull(od.Drink));
        }

        [Fact]
        public async Task TC47_Detail_IncludesDrinkName()
        {
            var ctx = CreateDb(nameof(TC47_Detail_IncludesDrinkName));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            var od = model!.OrderDetails!.First();
            Assert.NotNull(od.Drink?.Name);
        }

        [Fact]
        public async Task TC48_Detail_Quantity_IsCorrect()
        {
            var ctx = CreateDb(nameof(TC48_Detail_Quantity_IsCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            var od = model!.OrderDetails!.First(x => x.Id == "OD001");
            Assert.Equal(2, od.Quantity);
        }

        [Fact]
        public async Task TC49_Detail_UnitPrice_IsCorrect()
        {
            var ctx = CreateDb(nameof(TC49_Detail_UnitPrice_IsCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.Detail("ORD001") as ViewResult;
            var model = result!.Model as Order;
            var od = model!.OrderDetails!.First(x => x.Id == "OD001");
            Assert.Equal(40000m, od.UnitPrice);
        }

        [Fact]
        public async Task TC50_Index_And_Detail_FullIntegration_NoException()
        {
            var ctx = CreateDb(nameof(TC50_Index_And_Detail_FullIntegration_NoException));
            var ctrl = CreateController(ctx);
            var ex = await Record.ExceptionAsync(async () =>
            {
                await ctrl.Index(null, null);
                await ctrl.Detail("ORD001");
                await ctrl.Detail("ORD020");
            });
            Assert.Null(ex);
        }
    }
}