using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;

namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly PolyCafeDbContext _context;
        public CategoryController(PolyCafeDbContext context) => _context = context;

        private IActionResult? CheckAdmin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("POS", "Order", new { area = "" });
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var check = CheckAdmin(); if (check != null) return check;
            var cats = await _context.Categories
                .Include(c => c.Drinks!.Where(d => !d.IsDeleted))
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            return View(cats);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category model)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
            if (string.IsNullOrWhiteSpace(model.Name))
                return Json(new { success = false, message = "Tên không được để trống" });
            model.Id = "CAT" + Guid.NewGuid().ToString("N")[..6].ToUpper();
            _context.Categories.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = model.Id, name = model.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Category model)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
            var cat = await _context.Categories.FindAsync(model.Id);
            if (cat == null) return Json(new { success = false });
            cat.Name        = model.Name;
            cat.Description = model.Description;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return Json(new { success = false });
            cat.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
