using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Models;

namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DrinkController : Controller
    {
        private readonly PolyCafeDbContext _context;
        public DrinkController(PolyCafeDbContext context) => _context = context;

        private IActionResult? CheckAdmin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("POS", "Order", new { area = "" });
            return null;
        }

        // GET: /Admin/Drink
        public async Task<IActionResult> Index(string? search, string? catId, string? status)
        {
            var check = CheckAdmin(); if (check != null) return check;

            // Lấy TẤT CẢ kể cả IsDeleted để hiển thị tab Đã ẩn
            var q = _context.Drinks
                .Include(d => d.Category)
                .Include(d => d.DrinkVariants.Where(v => v.IsActive))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                q = q.Where(d => d.Name.Contains(search));

            if (!string.IsNullOrEmpty(catId))
                q = q.Where(d => d.CategoryId == catId);

            // Lọc theo status nếu có
            if (status == "selling")
                q = q.Where(d => d.IsAvailable && !d.IsDeleted);
            else if (status == "paused")
                q = q.Where(d => !d.IsAvailable && !d.IsDeleted);
            else if (status == "hidden")
                q = q.Where(d => d.IsDeleted);

            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            ViewBag.Search = search;
            ViewBag.CatId = catId;
            ViewBag.Status = status;

            return View(await q.OrderBy(d => d.CategoryId).ThenBy(d => d.Name).ToListAsync());
        }

        // GET: /Admin/Drink/Create
        public async Task<IActionResult> Create()
        {
            var check = CheckAdmin(); if (check != null) return check;
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            return View();
        }

        // POST: /Admin/Drink/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Drink model, IFormFile? imageFile)
        {
            var check = CheckAdmin(); if (check != null) return check;

            model.Id = "DRK" + Guid.NewGuid().ToString("N")[..8].ToUpper();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(imageFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "drinks");
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, fileName);
                using var fs = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(fs);
                model.ImageUrl = fileName;
            }

            _context.Drinks.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã thêm đồ uống \"{model.Name}\"!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Drink/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var check = CheckAdmin(); if (check != null) return check;
            var drink = await _context.Drinks
                .Include(d => d.DrinkVariants)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (drink == null) return NotFound();
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            return View(drink);
        }

        // POST: /Admin/Drink/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Drink model, IFormFile? imageFile)
        {
            var check = CheckAdmin(); if (check != null) return check;

            var drink = await _context.Drinks.FindAsync(model.Id);
            if (drink == null) return NotFound();

            drink.Name = model.Name;
            drink.Description = model.Description;
            drink.Price = model.Price;
            drink.CategoryId = model.CategoryId;
            drink.IsAvailable = model.IsAvailable;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(imageFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "drinks");
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, fileName);
                using var fs = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(fs);
                drink.ImageUrl = fileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật \"{drink.Name}\"!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Drink/SetStatus  (AJAX JSON)
        // isAvailable=true,  isDeleted=false  → Đang bán
        // isAvailable=false, isDeleted=false  → Tạm ngừng (vẫn hiện ở tab active)
        // isAvailable=false, isDeleted=true   → Ngừng bán / Ẩn (chuyển sang tab Đã ẩn)
        [HttpPost]
        public async Task<IActionResult> SetStatus([FromBody] SetStatusRequest req)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var drink = await _context.Drinks.FindAsync(req.Id);
            if (drink == null)
                return Json(new { success = false, message = "Không tìm thấy đồ uống!" });

            drink.IsAvailable = req.IsAvailable;
            drink.IsDeleted = req.IsDeleted;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: /Admin/Drink/ToggleAvailable  (giữ lại để tương thích cũ)
        [HttpPost]
        public async Task<IActionResult> ToggleAvailable(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var drink = await _context.Drinks.FindAsync(id);
            if (drink == null) return Json(new { success = false });

            drink.IsAvailable = !drink.IsAvailable;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isAvailable = drink.IsAvailable });
        }

        // POST: /Admin/Drink/Delete  — xoá cứng khỏi DB
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var drink = await _context.Drinks.FindAsync(id);
            if (drink == null) return Json(new { success = false, message = "Không tìm thấy!" });

            _context.Drinks.Remove(drink);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: /Admin/Drink/AddVariant  (AJAX JSON)
        [HttpPost]
        public async Task<IActionResult> AddVariant([FromBody] AddVariantRequest req)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            if (string.IsNullOrWhiteSpace(req.VariantName))
                return Json(new { success = false, message = "Tên variant không được trống!" });

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
            return Json(new { success = true, id = variant.Id });
        }

        // POST: /Admin/Drink/DeleteVariant  (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteVariant(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            var variant = await _context.DrinkVariants.FindAsync(id);
            if (variant == null) return Json(new { success = false });

            _context.DrinkVariants.Remove(variant);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

    // ===== DTOs =====

    public class SetStatusRequest
    {
        public string Id { get; set; } = "";
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class AddVariantRequest
    {
        public string DrinkId { get; set; } = "";
        public string VariantName { get; set; } = "";
        public decimal ExtraPrice { get; set; } = 0;
    }
}