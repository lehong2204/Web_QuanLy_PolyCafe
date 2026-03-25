using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;

namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VariantController : Controller
    {
        private readonly PolyCafeDbContext _context;
        public VariantController(PolyCafeDbContext context) => _context = context;

        private IActionResult? CheckAdmin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("POS", "Order", new { area = "" });
            return null;
        }

        // GET: /Admin/Variant
        public async Task<IActionResult> Index(string? search, string? drinkId)
        {
            var check = CheckAdmin(); if (check != null) return check;

            var q = _context.DrinkVariants
                .Include(v => v.Drink)
                    .ThenInclude(d => d.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                q = q.Where(v => v.VariantName.Contains(search) || v.Drink.Name.Contains(search));

            if (!string.IsNullOrEmpty(drinkId))
                q = q.Where(v => v.DrinkId == drinkId);

            ViewBag.Drinks = await _context.Drinks
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.DrinkId = drinkId;

            var variants = await q
                .OrderBy(v => v.Drink.Name)
                .ThenBy(v => v.VariantName)
                .ToListAsync();

            return View(variants);
        }

        // POST: /Admin/Variant/Add (AJAX JSON)
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddVariantRequest req)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            if (string.IsNullOrWhiteSpace(req.VariantName))
                return Json(new { success = false, message = "Tên biến thể không được trống!" });

            if (string.IsNullOrWhiteSpace(req.DrinkId))
                return Json(new { success = false, message = "Vui lòng chọn đồ uống!" });

            var drink = await _context.Drinks.FindAsync(req.DrinkId);
            if (drink == null)
                return Json(new { success = false, message = "Không tìm thấy đồ uống!" });

            var variant = new DrinkVariant
            {
                Id = "VAR" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                DrinkId = req.DrinkId,
                VariantName = req.VariantName.Trim(),
                ExtraPrice = req.ExtraPrice,
                IsActive = true
            };

            _context.DrinkVariants.Add(variant);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = variant.Id,
                drinkName = drink.Name,
                drinkImg = drink.ImageUrl,
                drinkId = drink.Id
            });
        }

        // POST: /Admin/Variant/Edit (AJAX JSON)
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditVariantRequest req)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var variant = await _context.DrinkVariants.FindAsync(req.Id);
            if (variant == null)
                return Json(new { success = false, message = "Không tìm thấy biến thể!" });

            if (string.IsNullOrWhiteSpace(req.VariantName))
                return Json(new { success = false, message = "Tên biến thể không được trống!" });

            variant.VariantName = req.VariantName.Trim();
            variant.ExtraPrice = req.ExtraPrice;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: /Admin/Variant/ToggleActive (AJAX JSON)
        [HttpPost]
        public async Task<IActionResult> ToggleActive([FromBody] ToggleActiveRequest req)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var variant = await _context.DrinkVariants.FindAsync(req.Id);
            if (variant == null)
                return Json(new { success = false, message = "Không tìm thấy biến thể!" });

            variant.IsActive = req.IsActive;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isActive = variant.IsActive });
        }

        // POST: /Admin/Variant/Delete (AJAX)
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var variant = await _context.DrinkVariants.FindAsync(id);
            if (variant == null)
                return Json(new { success = false, message = "Không tìm thấy!" });

            _context.DrinkVariants.Remove(variant);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

    // ===== DTOs =====

    public class EditVariantRequest
    {
        public string Id { get; set; } = "";
        public string VariantName { get; set; } = "";
        public decimal ExtraPrice { get; set; } = 0;
    }

    public class ToggleActiveRequest
    {
        public string Id { get; set; } = "";
        public bool IsActive { get; set; }
    }
}