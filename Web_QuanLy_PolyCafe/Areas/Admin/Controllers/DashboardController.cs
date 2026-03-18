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

            ViewBag.TotalOrders     = await _context.Orders.CountAsync();
            ViewBag.TodayOrders     = await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
            ViewBag.TotalRevenue    = await _context.Orders.SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
            ViewBag.MonthRevenue    = await _context.Orders
                .Where(o => o.OrderDate >= thisMonth)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
            ViewBag.TotalDrinks     = await _context.Drinks.CountAsync(d => !d.IsDeleted);
            ViewBag.TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted);
            ViewBag.TotalUsers      = await _context.Users.CountAsync(u => u.IsActive);

            // Top 5 món bán chạy
            ViewBag.TopDrinks = await _context.OrderDetails
                .Include(od => od.Drink)
                .GroupBy(od => new { od.DrinkId, od.Drink!.Name })
                .Select(g => new { g.Key.Name, Total = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();

            // Doanh thu 7 ngày gần nhất
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
