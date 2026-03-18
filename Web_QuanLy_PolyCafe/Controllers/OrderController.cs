
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;

namespace Web_QuanLy_PolyCafe.Controllers
{
    public class OrderController : Controller
    {
        private readonly PolyCafeDbContext _context;

        public OrderController(PolyCafeDbContext context)
        {
            _context = context;
        }

        // GET: /Order/POS
        public async Task<IActionResult> POS()
        {
            // ✅ Bảo vệ: chưa đăng nhập thì về Login
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var drinks = await _context.Drinks
                .Include(d => d.Category)
                .Include(d => d.DrinkVariants.Where(v => v.IsActive))
                .Where(d => !d.IsDeleted && d.IsAvailable)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            var vouchers = await _context.Vouchers
                .Where(v => v.IsActive)
                .ToListAsync();

            // ✅ Đếm số đơn hàng hiện tại để hiển thị số đơn tiếp theo đúng
            var orderCount = await _context.Orders.CountAsync();

            ViewBag.Categories = categories;
            ViewBag.Vouchers = vouchers;
            ViewBag.OrderCount = orderCount; // truyền xuống View
            return View(drinks);
        }

        // POST: /Order/CreateOrder
        // ✅ Bỏ [ValidateAntiForgeryToken] vì gọi bằng fetch/JSON không gửi được token
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            // ✅ Kiểm tra đăng nhập
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });

            if (request == null || request.Items == null || !request.Items.Any())
                return Json(new { success = false, message = "Đơn hàng trống." });

            // Tính tổng tiền
            decimal subtotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in request.Items)
            {
                var drink = await _context.Drinks.FindAsync(item.DrinkId);
                if (drink == null) continue;

                decimal unitPrice = drink.Price;
                if (!string.IsNullOrEmpty(item.VariantId))
                {
                    var variant = await _context.DrinkVariants.FindAsync(item.VariantId);
                    if (variant != null) unitPrice += variant.ExtraPrice;
                }

                subtotal += unitPrice * item.Quantity;
                orderDetails.Add(new OrderDetail
                {
                    Id = "OD" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    DrinkId = item.DrinkId,
                    VariantId = string.IsNullOrEmpty(item.VariantId) ? null : item.VariantId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            // Áp dụng voucher
            decimal discountAmount = 0;
            string? voucherId = null;

            if (!string.IsNullOrEmpty(request.VoucherCode))
            {
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.Code == request.VoucherCode.ToUpper() && v.IsActive);

                if (voucher != null)
                {
                    voucherId = voucher.Id;
                    if (voucher.DiscountType == "percent")
                        discountAmount = subtotal * voucher.DiscountValue / 100;
                    else
                        discountAmount = voucher.DiscountValue;

                    voucher.UsedCount++;
                }
            }

            decimal totalPrice = Math.Max(0, subtotal - discountAmount);

            // ✅ Tạo orderId unique dựa theo timestamp + random để tránh trùng
            var orderId = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss") +
                          new Random().Next(10, 99).ToString();

            var order = new Order
            {
                Id = orderId,
                UserId = userId,           // ✅ Lấy từ Session, không hardcode
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                DiscountAmount = discountAmount,
                Status = "Completed",
                VoucherId = voucherId
            };

            foreach (var od in orderDetails)
                od.OrderId = orderId;

            _context.Orders.Add(order);
            _context.OrderDetails.AddRange(orderDetails);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi lưu DB: " + ex.Message });
            }

            // ✅ Trả về orderCount mới để JS cập nhật số đơn
            var newOrderCount = await _context.Orders.CountAsync();

            return Json(new
            {
                success = true,
                orderId = orderId,
                total = totalPrice,
                orderCount = newOrderCount   // số đơn mới nhất từ DB
            });
        }

        // GET: /Order/CheckVoucher?code=xxx
        [HttpGet]
        public async Task<IActionResult> CheckVoucher(string code)
        {
            if (string.IsNullOrEmpty(code))
                return Json(new { valid = false, message = "Mã trống" });

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == code.ToUpper() && v.IsActive);

            if (voucher == null)
                return Json(new { valid = false, message = "Mã không hợp lệ hoặc đã hết hạn" });

            if (voucher.UsedCount >= voucher.UsageLimit)
                return Json(new { valid = false, message = "Mã đã hết lượt sử dụng" });

            return Json(new
            {
                valid = true,
                discountType = voucher.DiscountType,
                discountValue = voucher.DiscountValue,
                name = voucher.Name,
                message = $"Áp dụng: {voucher.Name}"
            });
        }
    }

    // DTO
    public class CreateOrderRequest
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public string? VoucherCode { get; set; }
    }

    public class OrderItemDto
    {
        public string DrinkId { get; set; } = "";
        public string? VariantId { get; set; }
        public int Quantity { get; set; }
    }
}