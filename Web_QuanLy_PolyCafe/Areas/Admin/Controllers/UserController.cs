//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Web_QuanLy_PolyCafe.Data;
//using Web_QuanLy_PolyCafe.Models;

//namespace Web_QuanLy_PolyCafe.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    public class UserController : Controller
//    {
//        private readonly PolyCafeDbContext _context;
//        public UserController(PolyCafeDbContext context) => _context = context;

//        private IActionResult? CheckAdmin()
//        {
//            if (HttpContext.Session.GetString("UserId") == null)
//                return RedirectToAction("Login", "Account", new { area = "" });
//            if (HttpContext.Session.GetString("Role") != "Admin")
//                return RedirectToAction("POS", "Order", new { area = "" });
//            return null;
//        }

//        public async Task<IActionResult> Index(string? search)
//        {
//            var check = CheckAdmin(); if (check != null) return check;
//            var q = _context.Users.AsQueryable();
//            if (!string.IsNullOrEmpty(search))
//                q = q.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
//            ViewBag.Search = search;
//            return View(await q.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToListAsync());
//        }

//        [HttpPost]
//        public async Task<IActionResult> ToggleActive(string id)
//        {
//            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
//            var myId = HttpContext.Session.GetString("UserId");
//            if (id == myId) return Json(new { success = false, message = "Không thể khóa chính mình!" });
//            var user = await _context.Users.FindAsync(id);
//            if (user == null) return Json(new { success = false });
//            user.IsActive = !user.IsActive;
//            await _context.SaveChangesAsync();
//            return Json(new { success = true, isActive = user.IsActive });
//        }

//        [HttpPost]
//        public async Task<IActionResult> ToggleRole(string id)
//        {
//            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
//            var myId = HttpContext.Session.GetString("UserId");
//            if (id == myId) return Json(new { success = false, message = "Không thể thay đổi role của chính mình!" });
//            var user = await _context.Users.FindAsync(id);
//            if (user == null) return Json(new { success = false });
//            user.Role = !user.Role;
//            await _context.SaveChangesAsync();
//            return Json(new { success = true, role = user.Role ? "Admin" : "Staff" });
//        }
//    }
//}
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

        // ============================================================
        // GET: /Admin/User/Index
        // ============================================================
        public async Task<IActionResult> Index(string? search)
        {
            var check = CheckAdmin(); if (check != null) return check;
            var q = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                q = q.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
            ViewBag.Search = search;
            return View(await q.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToListAsync());
        }

        // ============================================================
        // POST: /Admin/User/ToggleActive
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });
            var myId = HttpContext.Session.GetString("UserId");
            if (id == myId) return Json(new { success = false, message = "Không thể khoá chính mình!" });
            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false });
            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isActive = user.IsActive });
        }

        // ============================================================
        // POST: /Admin/User/ToggleRole
        // ============================================================
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

        // ============================================================
        // POST: /Admin/User/Create  (AJAX)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return Json(new { success = false, message = "Họ tên không được để trống!" });
            if (string.IsNullOrWhiteSpace(dto.Email))
                return Json(new { success = false, message = "Email không được để trống!" });
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return Json(new { success = false, message = "Mật khẩu phải từ 6 ký tự!" });

            // Kiểm tra email trùng
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email.Trim()))
                return Json(new { success = false, message = "Email đã tồn tại trong hệ thống!" });

            // Sinh ID mới
            var lastUser = await _context.Users
                .OrderByDescending(u => u.Id)
                .FirstOrDefaultAsync();
            int nextNum = 1;
            if (lastUser != null && lastUser.Id.StartsWith("U"))
                int.TryParse(lastUser.Id.Substring(1), out nextNum);
            nextNum++;
            var newId = "U" + nextNum.ToString("D3");

            var user = new User
            {
                Id        = newId,
                FullName  = dto.FullName.Trim(),
                Email     = dto.Email.Trim(),
                Password  = dto.Password,
                Phone     = dto.Phone?.Trim(),
                Address   = dto.Address?.Trim(),
                Role      = dto.Role,
                IsActive  = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success  = true,
                id       = user.Id,
                fullName = user.FullName,
                email    = user.Email,
                phone    = user.Phone ?? "—",
                role     = user.Role ? "Admin" : "Staff",
                createdAt = user.CreatedAt.ToString("dd/MM/yyyy")
            });
        }

        // ============================================================
        // POST: /Admin/User/Update  (AJAX)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var check = CheckAdmin(); if (check != null) return Json(new { success = false });

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return Json(new { success = false, message = "Họ tên không được để trống!" });

            var user = await _context.Users.FindAsync(dto.Id);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy tài khoản!" });

            // Kiểm tra email trùng với người khác
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email.Trim() && u.Id != dto.Id))
                return Json(new { success = false, message = "Email đã được dùng bởi tài khoản khác!" });

            user.FullName = dto.FullName.Trim();
            user.Email    = dto.Email.Trim();
            user.Phone    = dto.Phone?.Trim();
            user.Address  = dto.Address?.Trim();

            // Đổi mật khẩu nếu có nhập
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (dto.NewPassword.Length < 6)
                    return Json(new { success = false, message = "Mật khẩu mới phải từ 6 ký tự!" });
                user.Password = dto.NewPassword;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success  = true,
                fullName = user.FullName,
                email    = user.Email,
                phone    = user.Phone ?? "—"
            });
        }

        // ============================================================
        // GET: /Admin/User/GetById?id=xxx  (AJAX — load form sửa)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false });
            return Json(new
            {
                success = true,
                id       = user.Id,
                fullName = user.FullName,
                email    = user.Email,
                phone    = user.Phone ?? "",
                address  = user.Address ?? "",
                role     = user.Role
            });
        }
    }

    // ================================================================
    // DTOs
    // ================================================================
    public class UserCreateDto
    {
        public string FullName  { get; set; } = "";
        public string Email     { get; set; } = "";
        public string Password  { get; set; } = "";
        public string? Phone    { get; set; }
        public string? Address  { get; set; }
        public bool   Role      { get; set; }
    }

    public class UserUpdateDto
    {
        public string  Id          { get; set; } = "";
        public string  FullName    { get; set; } = "";
        public string  Email       { get; set; } = "";
        public string? Phone       { get; set; }
        public string? Address     { get; set; }
        public string? NewPassword { get; set; }
    }
}