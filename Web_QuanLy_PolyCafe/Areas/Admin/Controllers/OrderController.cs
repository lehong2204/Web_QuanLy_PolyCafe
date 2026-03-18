using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;

namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly PolyCafeDbContext _context;
        public OrderController(PolyCafeDbContext context) => _context = context;

        private IActionResult? CheckAdmin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("POS", "Order", new { area = "" });
            return null;
        }

        public async Task<IActionResult> Index(string? search, string? date)
        {
            var check = CheckAdmin(); if (check != null) return check;

            var q = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)!
                .ThenInclude(od => od.Drink)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                q = q.Where(o => o.Id.Contains(search) || o.User!.FullName.Contains(search));

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d))
                q = q.Where(o => o.OrderDate.Date == d.Date);

            ViewBag.Search = search;
            ViewBag.Date   = date;
            return View(await q.OrderByDescending(o => o.OrderDate).ToListAsync());
        }

        public async Task<IActionResult> Detail(string id)
        {
            var check = CheckAdmin(); if (check != null) return check;
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Voucher)
                .Include(o => o.OrderDetails)!
                .ThenInclude(od => od.Drink)
                .Include(o => o.OrderDetails)!
                .ThenInclude(od => od.Variant)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
