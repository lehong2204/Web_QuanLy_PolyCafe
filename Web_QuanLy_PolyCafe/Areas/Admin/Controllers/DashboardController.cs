//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Web_QuanLy_PolyCafe.Data;

//namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    public class DashboardController : Controller
//    {
//        private readonly PolyCafeDbContext _context;
//        public DashboardController(PolyCafeDbContext context) => _context = context;

//        private IActionResult CheckAdmin()
//        {
//            if (HttpContext.Session.GetString("UserId") == null)
//                return RedirectToAction("Login", "Account", new { area = "" });
//            if (HttpContext.Session.GetString("Role") != "Admin")
//                return RedirectToAction("POS", "Order", new { area = "" });
//            return null!;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var check = CheckAdmin(); if (check != null) return check;

//            var today = DateTime.Today;
//            var thisMonth = new DateTime(today.Year, today.Month, 1);

//            ViewBag.TotalOrders     = await _context.Orders.CountAsync();
//            ViewBag.TodayOrders     = await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
//            ViewBag.TotalRevenue    = await _context.Orders.SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
//            ViewBag.MonthRevenue    = await _context.Orders
//                .Where(o => o.OrderDate >= thisMonth)
//                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
//            ViewBag.TotalDrinks     = await _context.Drinks.CountAsync(d => !d.IsDeleted);
//            ViewBag.TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted);
//            ViewBag.TotalUsers      = await _context.Users.CountAsync(u => u.IsActive);

//            // Top 5 món bán chạy
//            ViewBag.TopDrinks = await _context.OrderDetails
//                .Include(od => od.Drink)
//                .GroupBy(od => new { od.DrinkId, od.Drink!.Name })
//                .Select(g => new { g.Key.Name, Total = g.Sum(x => x.Quantity) })
//                .OrderByDescending(x => x.Total)
//                .Take(5)
//                .ToListAsync();

//            // Doanh thu 7 ngày gần nhất
//            ViewBag.RevenueChart = await _context.Orders
//                .Where(o => o.OrderDate.Date >= today.AddDays(-6))
//                .GroupBy(o => o.OrderDate.Date)
//                .Select(g => new { Date = g.Key, Revenue = g.Sum(x => x.TotalPrice) })
//                .OrderBy(x => x.Date)
//                .ToListAsync();

//            return View();
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;

namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly PolyCafeDbContext _context;
        public DashboardController(PolyCafeDbContext context) => _context = context;

        private IActionResult CheckAdmin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("POS", "Order", new { area = "" });
            return null!;
        }

        public async Task<IActionResult> Index()
        {
            var check = CheckAdmin(); if (check != null) return check;

            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // ===== STAT CARDS =====
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.TodayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
            ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
            ViewBag.MonthRevenue = await _context.Orders
                .Where(o => o.OrderDate >= thisMonth)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
            ViewBag.TotalDrinks = await _context.Drinks.CountAsync(d => !d.IsDeleted);
            ViewBag.TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted);
            ViewBag.TotalUsers = await _context.Users.CountAsync(u => u.IsActive);

            // ===== DOANH THU THEO THÁNG (12 tháng năm hiện tại) =====
            var revenueByMonth = await _context.Orders
                .Where(o => o.OrderDate.Year == today.Year)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new { Month = g.Key, Revenue = g.Sum(x => x.TotalPrice) })
                .ToListAsync();

            decimal[] monthlyRevenue = new decimal[12];
            foreach (var r in revenueByMonth)
                monthlyRevenue[r.Month - 1] = r.Revenue;
            ViewBag.MonthlyRevenue = monthlyRevenue;

            // ===== DOANH THU THEO NĂM =====
            var revenueByYear = await _context.Orders
                .GroupBy(o => o.OrderDate.Year)
                .Select(g => new { Year = g.Key, Revenue = g.Sum(x => x.TotalPrice) })
                .OrderBy(x => x.Year)
                .ToListAsync();

            ViewBag.YearLabels = revenueByYear.Select(x => x.Year).ToList();
            ViewBag.YearRevenue = revenueByYear.Select(x => x.Revenue).ToList();

            // ===== TOP 5 BÁN CHẠY =====
            var topDrinks = await _context.OrderDetails
                .Include(od => od.Drink)
                .GroupBy(od => new { od.DrinkId, od.Drink!.Name, od.Drink.ImageUrl })
                .Select(g => new
                {
                    g.Key.Name,
                    g.Key.ImageUrl,
                    Total = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();

            ViewBag.TopDrinkMain = topDrinks.FirstOrDefault();   // TOP 1 — dùng cho mini chart card
            ViewBag.TopDrinkList = topDrinks.Skip(1).ToList();    // TOP 2-5

            // ===== DOANH THU 7 NGÀY GẦN NHẤT =====
            ViewBag.RevenueChart = await _context.Orders
                .Where(o => o.OrderDate.Date >= today.AddDays(-6))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(x => x.TotalPrice) })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return View();
        }
    }
}