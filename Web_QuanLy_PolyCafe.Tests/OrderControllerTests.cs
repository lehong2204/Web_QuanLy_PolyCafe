using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web_QuanLy_PolyCafe.Controllers;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;
using Xunit;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class OrderControllerTests
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

        private OrderController CreateController(PolyCafeDbContext ctx, string? userId = "U001")
        {
            var controller = new OrderController(ctx);
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
        // NHOM 1: CREATEORDER - TINH TIEN CO BAN (TC01-TC10)
        // ================================================================

        // TC01: Dat 1 mon, khong voucher, khong variant
        // D001 Ca Phe Sua Da 35.000d x 2 = 70.000d
        [Fact]
        public async Task TC01_CreateOrder_NoVoucherNoVariant_TotalCorrect()
        {
            var ctx = CreateDb(nameof(TC01_CreateOrder_NoVoucherNoVariant_TotalCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 2 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(70000m, data.GetProperty("total").GetDecimal());
        }

        // TC02: Dat 1 mon so luong = 1
        // D009 Tra Dao Cam Sa 50.000d x 1 = 50.000d
        [Fact]
        public async Task TC02_CreateOrder_SingleItem_TotalCorrect()
        {
            var ctx = CreateDb(nameof(TC02_CreateOrder_SingleItem_TotalCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D009", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(50000m, data.GetProperty("total").GetDecimal());
        }

        // TC03: Dat nhieu mon khac nhau
        // D001 35.000d x 1 + D009 50.000d x 2 = 135.000d
        [Fact]
        public async Task TC03_CreateOrder_MultipleItems_TotalCorrect()
        {
            var ctx = CreateDb(nameof(TC03_CreateOrder_MultipleItems_TotalCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto>
                {
                    new() { DrinkId = "D001", Quantity = 1 },
                    new() { DrinkId = "D009", Quantity = 2 }
                }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(135000m, data.GetProperty("total").GetDecimal());
        }

        // TC04: Dat mon so luong lon
        // D008 Nuoc Ep Dua Hau 40.000d x 10 = 400.000d
        [Fact]
        public async Task TC04_CreateOrder_LargeQuantity_TotalCorrect()
        {
            var ctx = CreateDb(nameof(TC04_CreateOrder_LargeQuantity_TotalCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D008", Quantity = 10 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(400000m, data.GetProperty("total").GetDecimal());
        }

        // TC05: Variant ExtraPrice = 0 (Size S) -> gia goc
        // D001 35.000d + DV001 ExtraPrice 0 = 35.000d x 1
        [Fact]
        public async Task TC05_CreateOrder_VariantZeroExtraPrice_TotalUnchanged()
        {
            var ctx = CreateDb(nameof(TC05_CreateOrder_VariantZeroExtraPrice_TotalUnchanged));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", VariantId = "DV001", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(35000m, data.GetProperty("total").GetDecimal());
        }

        // TC06: Variant Size M ExtraPrice = 5.000d
        // D001 35.000d + DV002 5.000d = 40.000d x 1
        [Fact]
        public async Task TC06_CreateOrder_VariantSizeM_ExtraPriceAdded()
        {
            var ctx = CreateDb(nameof(TC06_CreateOrder_VariantSizeM_ExtraPriceAdded));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", VariantId = "DV002", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(40000m, data.GetProperty("total").GetDecimal());
        }

        // TC07: Variant Size L ExtraPrice = 10.000d
        // D001 35.000d + DV003 10.000d = 45.000d x 1
        [Fact]
        public async Task TC07_CreateOrder_VariantSizeL_ExtraPriceAdded()
        {
            var ctx = CreateDb(nameof(TC07_CreateOrder_VariantSizeL_ExtraPriceAdded));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", VariantId = "DV003", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(45000m, data.GetProperty("total").GetDecimal());
        }

        // TC08: Variant them tran chau ExtraPrice = 8.000d
        // D003 45.000d + DV006 8.000d = 53.000d x 1
        [Fact]
        public async Task TC08_CreateOrder_VariantTopping_ExtraPriceAdded()
        {
            var ctx = CreateDb(nameof(TC08_CreateOrder_VariantTopping_ExtraPriceAdded));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", VariantId = "DV006", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(53000m, data.GetProperty("total").GetDecimal());
        }

        // TC09: Nhieu mon, co variant
        // D001+DV002 40.000d x 1 + D003+DV006 53.000d x 2 = 40.000 + 106.000 = 146.000d
        [Fact]
        public async Task TC09_CreateOrder_MultipleItemsWithVariant_TotalCorrect()
        {
            var ctx = CreateDb(nameof(TC09_CreateOrder_MultipleItemsWithVariant_TotalCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto>
                {
                    new() { DrinkId = "D001", VariantId = "DV002", Quantity = 1 },
                    new() { DrinkId = "D003", VariantId = "DV006", Quantity = 2 }
                }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(146000m, data.GetProperty("total").GetDecimal());
        }

        // TC10: Drink khong ton tai -> skip, tong = 0, van success
        [Fact]
        public async Task TC10_CreateOrder_DrinkNotFound_SkippedTotalZero()
        {
            var ctx = CreateDb(nameof(TC10_CreateOrder_DrinkNotFound_SkippedTotalZero));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D999", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(0m, data.GetProperty("total").GetDecimal());
        }

        // ================================================================
        // NHOM 2: CREATEORDER - VOUCHER (TC11-TC22)
        // ================================================================

        // TC11: Voucher giam % - WELCOME10 giam 10%
        // D003 45.000d x 1 = 45.000d -> giam 4.500d -> 40.500d
        [Fact]
        public async Task TC11_CreateOrder_VoucherPercent_DiscountCorrect()
        {
            var ctx = CreateDb(nameof(TC11_CreateOrder_VoucherPercent_DiscountCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", Quantity = 1 } },
                VoucherCode = "WELCOME10"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(40500m, data.GetProperty("total").GetDecimal());
        }

        // TC12: Voucher giam co dinh - GIAM20K giam 20.000d
        // D005 55.000d x 1 = 55.000d -> giam 20.000d -> 35.000d
        [Fact]
        public async Task TC12_CreateOrder_VoucherFixed_DiscountCorrect()
        {
            var ctx = CreateDb(nameof(TC12_CreateOrder_VoucherFixed_DiscountCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D005", Quantity = 1 } },
                VoucherCode = "GIAM20K"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(35000m, data.GetProperty("total").GetDecimal());
        }

        // TC13: Voucher giam nhieu hon tong tien -> Math.Max(0) = 0
        // D002 35.000d, COMBO50K giam 50.000d -> 0d
        [Fact]
        public async Task TC13_CreateOrder_VoucherExceedsTotal_ResultZero()
        {
            var ctx = CreateDb(nameof(TC13_CreateOrder_VoucherExceedsTotal_ResultZero));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D002", Quantity = 1 } },
                VoucherCode = "COMBO50K"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(0m, data.GetProperty("total").GetDecimal());
        }

        // TC14: Voucher IsActive = false (POLY15) -> khong ap dung
        // D006 50.000d x 1 = 50.000d
        [Fact]
        public async Task TC14_CreateOrder_VoucherInactive_NotApplied()
        {
            var ctx = CreateDb(nameof(TC14_CreateOrder_VoucherInactive_NotApplied));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D006", Quantity = 1 } },
                VoucherCode = "POLY15"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(50000m, data.GetProperty("total").GetDecimal());
        }

        // TC15: Voucher code sai -> khong ap dung, tinh gia goc
        // D007 45.000d x 1 = 45.000d
        [Fact]
        public async Task TC15_CreateOrder_VoucherInvalidCode_NotApplied()
        {
            var ctx = CreateDb(nameof(TC15_CreateOrder_VoucherInvalidCode_NotApplied));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D007", Quantity = 1 } },
                VoucherCode = "XXXXX"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(45000m, data.GetProperty("total").GetDecimal());
        }

        // TC16: Voucher code viet thuong -> van ap dung (ToUpper)
        // D003 45.000d x 1, welcome10 -> giam 10% -> 40.500d
        [Fact]
        public async Task TC16_CreateOrder_VoucherLowerCase_Applied()
        {
            var ctx = CreateDb(nameof(TC16_CreateOrder_VoucherLowerCase_Applied));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", Quantity = 1 } },
                VoucherCode = "welcome10"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(40500m, data.GetProperty("total").GetDecimal());
        }

        // TC17: Voucher UsedCount tang +1 sau khi su dung
        [Fact]
        public async Task TC17_CreateOrder_Voucher_UsedCountIncremented()
        {
            var ctx = CreateDb(nameof(TC17_CreateOrder_Voucher_UsedCountIncremented));
            var ctrl = CreateController(ctx);
            var before = ctx.Vouchers.First(v => v.Code == "WELCOME10").UsedCount;
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", Quantity = 1 } },
                VoucherCode = "WELCOME10"
            });
            var after = ctx.Vouchers.First(v => v.Code == "WELCOME10").UsedCount;
            Assert.Equal(before + 1, after);
        }

        // TC18: Khong dung voucher -> UsedCount khong thay doi
        [Fact]
        public async Task TC18_CreateOrder_NoVoucher_UsedCountUnchanged()
        {
            var ctx = CreateDb(nameof(TC18_CreateOrder_NoVoucher_UsedCountUnchanged));
            var ctrl = CreateController(ctx);
            var before = ctx.Vouchers.First(v => v.Code == "WELCOME10").UsedCount;
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            });
            var after = ctx.Vouchers.First(v => v.Code == "WELCOME10").UsedCount;
            Assert.Equal(before, after);
        }

        // TC19: Voucher % ap dung len nhieu mon
        // D001 35.000d x 1 + D005 55.000d x 1 = 90.000d -> WELCOME10 giam 10% = 9.000d -> 81.000d
        [Fact]
        public async Task TC19_CreateOrder_VoucherPercent_AppliedOnTotal()
        {
            var ctx = CreateDb(nameof(TC19_CreateOrder_VoucherPercent_AppliedOnTotal));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto>
                {
                    new() { DrinkId = "D001", Quantity = 1 },
                    new() { DrinkId = "D005", Quantity = 1 }
                },
                VoucherCode = "WELCOME10"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(81000m, data.GetProperty("total").GetDecimal());
        }

        // TC20: Voucher fixed ap dung len nhieu mon
        // D001 35.000d x 1 + D009 50.000d x 1 = 85.000d -> GIAM20K giam 20.000d -> 65.000d
        [Fact]
        public async Task TC20_CreateOrder_VoucherFixed_AppliedOnTotal()
        {
            var ctx = CreateDb(nameof(TC20_CreateOrder_VoucherFixed_AppliedOnTotal));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto>
                {
                    new() { DrinkId = "D001", Quantity = 1 },
                    new() { DrinkId = "D009", Quantity = 1 }
                },
                VoucherCode = "GIAM20K"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(65000m, data.GetProperty("total").GetDecimal());
        }

        // TC21: Voucher LOYAL5 giam 5%
        // D004 Tra Sua Matcha 50.000d x 1 -> giam 5% = 2.500d -> 47.500d
        [Fact]
        public async Task TC21_CreateOrder_VoucherLoyal5Percent_DiscountCorrect()
        {
            var ctx = CreateDb(nameof(TC21_CreateOrder_VoucherLoyal5Percent_DiscountCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D004", Quantity = 1 } },
                VoucherCode = "LOYAL5"
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("success").GetBoolean());
            Assert.Equal(47500m, data.GetProperty("total").GetDecimal());
        }

        // TC22: VoucherId duoc luu vao Order khi dung voucher hop le
        [Fact]
        public async Task TC22_CreateOrder_ValidVoucher_VoucherIdSavedToOrder()
        {
            var ctx = CreateDb(nameof(TC22_CreateOrder_ValidVoucher_VoucherIdSavedToOrder));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", Quantity = 1 } },
                VoucherCode = "WELCOME10"
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            Assert.Equal("V001", lastOrder.VoucherId);
        }

        // ================================================================
        // NHOM 3: CREATEORDER - TRANG THAI VA DB (TC23-TC33)
        // ================================================================

        // TC23: Don hang duoc luu vao Orders table
        [Fact]
        public async Task TC23_CreateOrder_OrderSavedToDatabase()
        {
            var ctx = CreateDb(nameof(TC23_CreateOrder_OrderSavedToDatabase));
            var ctrl = CreateController(ctx);
            var before = ctx.Orders.Count();
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D002", Quantity = 1 } }
            });
            Assert.Equal(before + 1, ctx.Orders.Count());
        }

        // TC24: OrderDetails duoc luu dung so luong dong
        [Fact]
        public async Task TC24_CreateOrder_OrderDetailsSaved_CorrectCount()
        {
            var ctx = CreateDb(nameof(TC24_CreateOrder_OrderDetailsSaved_CorrectCount));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto>
                {
                    new() { DrinkId = "D001", Quantity = 1 },
                    new() { DrinkId = "D003", Quantity = 1 },
                    new() { DrinkId = "D005", Quantity = 1 }
                }
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            var detailCount = ctx.OrderDetails.Count(od => od.OrderId == lastOrder.Id);
            Assert.Equal(3, detailCount);
        }

        // TC25: Order.Status = "Completed"
        [Fact]
        public async Task TC25_CreateOrder_StatusIsCompleted()
        {
            var ctx = CreateDb(nameof(TC25_CreateOrder_StatusIsCompleted));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            Assert.Equal("Completed", lastOrder.Status);
        }

        // TC26: Order.UserId = UserId trong session
        [Fact]
        public async Task TC26_CreateOrder_UserIdFromSession()
        {
            var ctx = CreateDb(nameof(TC26_CreateOrder_UserIdFromSession));
            var ctrl = CreateController(ctx, "U002");
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            Assert.Equal("U002", lastOrder.UserId);
        }

        // TC27: Order.DiscountAmount dung khi co voucher %
        // D003 45.000d, WELCOME10 10% -> DiscountAmount = 4.500d
        [Fact]
        public async Task TC27_CreateOrder_DiscountAmountSaved_PercentVoucher()
        {
            var ctx = CreateDb(nameof(TC27_CreateOrder_DiscountAmountSaved_PercentVoucher));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", Quantity = 1 } },
                VoucherCode = "WELCOME10"
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            Assert.Equal(4500m, lastOrder.DiscountAmount);
        }

        // TC28: Order.DiscountAmount = 0 khi khong dung voucher
        [Fact]
        public async Task TC28_CreateOrder_NoVoucher_DiscountAmountZero()
        {
            var ctx = CreateDb(nameof(TC28_CreateOrder_NoVoucher_DiscountAmountZero));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            Assert.Equal(0m, lastOrder.DiscountAmount);
        }

        // TC29: orderCount trong response = so don hang moi nhat trong DB
        [Fact]
        public async Task TC29_CreateOrder_ResponseOrderCount_Correct()
        {
            var ctx = CreateDb(nameof(TC29_CreateOrder_ResponseOrderCount_Correct));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D004", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.Equal(ctx.Orders.Count(), data.GetProperty("orderCount").GetInt32());
        }

        // TC30: orderId trong response khong null va bat dau bang "ORD"
        [Fact]
        public async Task TC30_CreateOrder_OrderId_StartWithORD()
        {
            var ctx = CreateDb(nameof(TC30_CreateOrder_OrderId_StartWithORD));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D005", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            var orderId = data.GetProperty("orderId").GetString();
            Assert.NotNull(orderId);
            Assert.StartsWith("ORD", orderId);
        }

        // TC31: Dat 2 don lien tiep -> DB co them 2 order
        [Fact]
        public async Task TC31_CreateOrder_TwoOrders_BothSaved()
        {
            var ctx = CreateDb(nameof(TC31_CreateOrder_TwoOrders_BothSaved));
            var ctrl = CreateController(ctx);
            var before = ctx.Orders.Count();
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            });
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D002", Quantity = 1 } }
            });
            Assert.Equal(before + 2, ctx.Orders.Count());
        }

        // TC32: OrderDetail.UnitPrice luu dung (gia drink + extra variant)
        // D001 35.000d + DV002 5.000d = 40.000d
        [Fact]
        public async Task TC32_CreateOrder_OrderDetail_UnitPriceCorrect()
        {
            var ctx = CreateDb(nameof(TC32_CreateOrder_OrderDetail_UnitPriceCorrect));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", VariantId = "DV002", Quantity = 1 } }
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            var detail = ctx.OrderDetails.First(od => od.OrderId == lastOrder.Id);
            Assert.Equal(40000m, detail.UnitPrice);
        }

        // TC33: OrderDetail.Quantity luu dung
        [Fact]
        public async Task TC33_CreateOrder_OrderDetail_QuantityCorrect()
        {
            var ctx = CreateDb(nameof(TC33_CreateOrder_OrderDetail_QuantityCorrect));
            var ctrl = CreateController(ctx);
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D003", Quantity = 3 } }
            });
            var lastOrder = ctx.Orders.OrderBy(o => o.OrderDate).Last();
            var detail = ctx.OrderDetails.First(od => od.OrderId == lastOrder.Id);
            Assert.Equal(3, detail.Quantity);
        }

        // ================================================================
        // NHOM 4: CREATEORDER - LOI VA BIEN PHAP (TC34-TC38)
        // ================================================================

        // TC34: Don hang rong -> success = false
        [Fact]
        public async Task TC34_CreateOrder_EmptyItems_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC34_CreateOrder_EmptyItems_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto>()
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("success").GetBoolean());
        }

        // TC35: Don hang rong -> khong luu them order vao DB
        [Fact]
        public async Task TC35_CreateOrder_EmptyItems_NoOrderSaved()
        {
            var ctx = CreateDb(nameof(TC35_CreateOrder_EmptyItems_NoOrderSaved));
            var ctrl = CreateController(ctx);
            var before = ctx.Orders.Count();
            await ctrl.CreateOrder(new CreateOrderRequest { Items = new List<OrderItemDto>() });
            Assert.Equal(before, ctx.Orders.Count());
        }

        // TC36: Request null -> success = false
        [Fact]
        public async Task TC36_CreateOrder_NullRequest_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC36_CreateOrder_NullRequest_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CreateOrder(null!) as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("success").GetBoolean());
        }

        // TC37: Chua dang nhap (session rong) -> success = false
        [Fact]
        public async Task TC37_CreateOrder_NotLoggedIn_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC37_CreateOrder_NotLoggedIn_ReturnsFalse));
            var ctrl = CreateController(ctx, null);
            var result = await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            }) as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("success").GetBoolean());
        }

        // TC38: Chua dang nhap -> khong luu order vao DB
        [Fact]
        public async Task TC38_CreateOrder_NotLoggedIn_NoOrderSaved()
        {
            var ctx = CreateDb(nameof(TC38_CreateOrder_NotLoggedIn_NoOrderSaved));
            var ctrl = CreateController(ctx, null);
            var before = ctx.Orders.Count();
            await ctrl.CreateOrder(new CreateOrderRequest
            {
                Items = new List<OrderItemDto> { new() { DrinkId = "D001", Quantity = 1 } }
            });
            Assert.Equal(before, ctx.Orders.Count());
        }

        // ================================================================
        // NHOM 5: CHECKVOUCHER (TC39-TC50)
        // ================================================================

        // TC39: Voucher hop le tra ve valid = true
        [Fact]
        public async Task TC39_CheckVoucher_ValidCode_ReturnsTrue()
        {
            var ctx = CreateDb(nameof(TC39_CheckVoucher_ValidCode_ReturnsTrue));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("WELCOME10") as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("valid").GetBoolean());
        }

        // TC40: Voucher tra ve dung discountType = "percent"
        [Fact]
        public async Task TC40_CheckVoucher_ReturnsDiscountType_Percent()
        {
            var ctx = CreateDb(nameof(TC40_CheckVoucher_ReturnsDiscountType_Percent));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("WELCOME10") as JsonResult;
            var data = ToJson(result!);
            Assert.Equal("percent", data.GetProperty("discountType").GetString());
        }

        // TC41: Voucher tra ve dung discountType = "fixed"
        [Fact]
        public async Task TC41_CheckVoucher_ReturnsDiscountType_Fixed()
        {
            var ctx = CreateDb(nameof(TC41_CheckVoucher_ReturnsDiscountType_Fixed));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("GIAM20K") as JsonResult;
            var data = ToJson(result!);
            Assert.Equal("fixed", data.GetProperty("discountType").GetString());
        }

        // TC42: Voucher tra ve dung discountValue
        // GIAM20K -> 20000
        [Fact]
        public async Task TC42_CheckVoucher_ReturnsDiscountValue_Correct()
        {
            var ctx = CreateDb(nameof(TC42_CheckVoucher_ReturnsDiscountValue_Correct));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("GIAM20K") as JsonResult;
            var data = ToJson(result!);
            Assert.Equal(20000m, data.GetProperty("discountValue").GetDecimal());
        }

        // TC43: Voucher tra ve truong "name" khong rong
        [Fact]
        public async Task TC43_CheckVoucher_ReturnsName_NotEmpty()
        {
            var ctx = CreateDb(nameof(TC43_CheckVoucher_ReturnsName_NotEmpty));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("WELCOME10") as JsonResult;
            var data = ToJson(result!);
            Assert.False(string.IsNullOrEmpty(data.GetProperty("name").GetString()));
        }

        // TC44: Voucher code khong ton tai -> valid = false
        [Fact]
        public async Task TC44_CheckVoucher_InvalidCode_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC44_CheckVoucher_InvalidCode_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("KHONGCO") as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("valid").GetBoolean());
        }

        // TC45: Voucher IsActive = false -> valid = false
        [Fact]
        public async Task TC45_CheckVoucher_Inactive_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC45_CheckVoucher_Inactive_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("POLY15") as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("valid").GetBoolean());
        }

        // TC46: Voucher code rong -> valid = false
        [Fact]
        public async Task TC46_CheckVoucher_EmptyCode_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC46_CheckVoucher_EmptyCode_ReturnsFalse));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("") as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("valid").GetBoolean());
        }

        // TC47: Voucher code viet thuong -> van tim duoc (ToUpper ben trong)
        [Fact]
        public async Task TC47_CheckVoucher_LowerCase_StillValid()
        {
            var ctx = CreateDb(nameof(TC47_CheckVoucher_LowerCase_StillValid));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("giam20k") as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("valid").GetBoolean());
        }

        // TC48: Voucher da het luot su dung -> valid = false
        [Fact]
        public async Task TC48_CheckVoucher_UsageLimitReached_ReturnsFalse()
        {
            var ctx = CreateDb(nameof(TC48_CheckVoucher_UsageLimitReached_ReturnsFalse));
            ctx.Vouchers.Add(new Voucher
            {
                Id = "VTEST01",
                Code = "HETLUOT",
                Name = "Test het luot",
                DiscountType = "fixed",
                DiscountValue = 10000,
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(5),
                UsageLimit = 3,
                UsedCount = 3,
                IsActive = true
            });
            await ctx.SaveChangesAsync();
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("HETLUOT") as JsonResult;
            var data = ToJson(result!);
            Assert.False(data.GetProperty("valid").GetBoolean());
        }

        // TC49: Voucher con luot su dung -> valid = true
        [Fact]
        public async Task TC49_CheckVoucher_UsageLimitNotReached_ReturnsTrue()
        {
            var ctx = CreateDb(nameof(TC49_CheckVoucher_UsageLimitNotReached_ReturnsTrue));
            var ctrl = CreateController(ctx);
            // LOYAL5: UsedCount=150, UsageLimit=9999 -> con luot
            var result = await ctrl.CheckVoucher("LOYAL5") as JsonResult;
            var data = ToJson(result!);
            Assert.True(data.GetProperty("valid").GetBoolean());
        }

        // TC50: Voucher % tra ve dung discountValue = 10
        [Fact]
        public async Task TC50_CheckVoucher_PercentVoucher_DiscountValueCorrect()
        {
            var ctx = CreateDb(nameof(TC50_CheckVoucher_PercentVoucher_DiscountValueCorrect));
            var ctrl = CreateController(ctx);
            var result = await ctrl.CheckVoucher("WELCOME10") as JsonResult;
            var data = ToJson(result!);
            Assert.Equal(10m, data.GetProperty("discountValue").GetDecimal());
        }
    }
}