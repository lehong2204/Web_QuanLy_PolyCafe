using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;

namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly PolyCafeDbContext _context;
        public UserController(PolyCafeDbContext context) => _context = context;

        private IActionResult? CheckAdmin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("POS", "Order", new { area = "" });
            return null;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var check = CheckAdmin(); if (check != null) return check;
            var q = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                q = q.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
            ViewBag.Search = search;
            return View(await q.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
            var myId = HttpContext.Session.GetString("UserId");
            if (id == myId) return Json(new { success = false, message = "Không thể khóa chính mình!" });
            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false });
            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isActive = user.IsActive });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRole(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
            var myId = HttpContext.Session.GetString("UserId");
            if (id == myId) return Json(new { success = false, message = "Không thể thay đổi role của chính mình!" });
            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false });
            user.Role = !user.Role;
            await _context.SaveChangesAsync();
            return Json(new { success = true, role = user.Role ? "Admin" : "Staff" });
        }
    }
}
