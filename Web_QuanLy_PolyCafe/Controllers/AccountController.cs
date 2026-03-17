//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Web_QuanLy_PolyCafe.Data;

//namespace Web_QuanLy_PolyCafe.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly PolyCafeDbContext _context;

//        public AccountController(PolyCafeDbContext context)
//        {
//            _context = context;
//        }

//        // GET: /Account/Login
//        public IActionResult Login()
//        {
//            // Nếu đã đăng nhập rồi thì chuyển thẳng vào POS
//            if (HttpContext.Session.GetString("UserId") != null)
//                return RedirectToAction("POS", "Order");

//            return View();
//        }

//        // POST: /Account/Login
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(string email, string password)
//        {
//            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
//            {
//                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu.";
//                ViewBag.Email = email;
//                return View();
//            }

//            // Tìm user theo email hoặc số điện thoại
//            var user = await _context.Users
//                .FirstOrDefaultAsync(u =>
//                    (u.Email == email.Trim() || u.Phone == email.Trim())
//                    && u.Password == password
//                    && u.IsActive);

//            if (user == null)
//            {
//                ViewBag.Error = "Email / số điện thoại hoặc mật khẩu không đúng.";
//                ViewBag.Email = email;
//                return View();
//            }

//            // Lưu session
//            HttpContext.Session.SetString("UserId", user.Id);
//            HttpContext.Session.SetString("FullName", user.FullName);
//            HttpContext.Session.SetString("Role", user.Role ? "Admin" : "Staff");

//            // Chuyển hướng vào POS
//            return RedirectToAction("POS", "Order");
//        }

//        // GET: /Account/Logout
//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login", "Account");
//        }

//        // GET: /Account/Register  (giữ lại route cũ, chưa implement)
//        public IActionResult Register()
//        {
//            return View();
//        }
//    }
//}
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Web_QuanLy_PolyCafe.Data;

//namespace Web_QuanLy_PolyCafe.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly PolyCafeDbContext _context;

//        public AccountController(PolyCafeDbContext context)
//        {
//            _context = context;
//        }

//        // GET: /Account/Login
//        public IActionResult Login()
//        {
//            if (HttpContext.Session.GetString("UserId") != null)
//                return RedirectToAction("POS", "Order");
//            return View();
//        }

//        // POST: /Account/Login
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(string email, string password)
//        {
//            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
//            {
//                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu.";
//                ViewBag.Email = email;
//                return View();
//            }

//            var user = await _context.Users
//                .FirstOrDefaultAsync(u =>
//                    (u.Email == email.Trim() || u.Phone == email.Trim())
//                    && u.Password == password
//                    && u.IsActive);

//            if (user == null)
//            {
//                ViewBag.Error = "Email / số điện thoại hoặc mật khẩu không đúng.";
//                ViewBag.Email = email;
//                return View();
//            }

//            HttpContext.Session.SetString("UserId", user.Id);
//            HttpContext.Session.SetString("FullName", user.FullName);
//            HttpContext.Session.SetString("Role", user.Role ? "Admin" : "Staff");

//            return RedirectToAction("POS", "Order");
//        }

//        // GET: /Account/Logout
//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login", "Account");
//        }

//        // GET: /Account/Register
//        public IActionResult Register()
//        {
//            return View();
//        }

//        // ============================================================
//        // GET: /Account/Profile
//        // ============================================================
//        public IActionResult Profile()
//        {
//            var userId = HttpContext.Session.GetString("UserId");
//            if (string.IsNullOrEmpty(userId))
//                return RedirectToAction("Login");

//            var user = _context.Users
//                .Include(u => u.Orders)
//                .FirstOrDefault(u => u.Id == userId);

//            if (user == null)
//                return RedirectToAction("Login");

//            return View(user);
//        }

//        // ============================================================
//        // POST: /Account/UpdateProfile  (AJAX JSON)
//        // ============================================================
//        [HttpPost]
//        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest dto)
//        {
//            var userId = HttpContext.Session.GetString("UserId");
//            if (string.IsNullOrEmpty(userId))
//                return Json(new { success = false, message = "Chưa đăng nhập" });

//            if (string.IsNullOrWhiteSpace(dto?.FullName))
//                return Json(new { success = false, message = "Họ tên không được để trống!" });

//            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
//            if (user == null)
//                return Json(new { success = false, message = "Không tìm thấy tài khoản" });

//            user.FullName = dto.FullName.Trim();
//            user.Phone = dto.Phone?.Trim();
//            user.Address = dto.Address?.Trim();
//            _context.SaveChanges();

//            // Cập nhật lại session
//            HttpContext.Session.SetString("FullName", user.FullName);

//            return Json(new { success = true });
//        }

//        // ============================================================
//        // POST: /Account/ChangePassword  (AJAX JSON)
//        // ============================================================
//        [HttpPost]
//        public IActionResult ChangePassword([FromBody] ChangePasswordRequest dto)
//        {
//            var userId = HttpContext.Session.GetString("UserId");
//            if (string.IsNullOrEmpty(userId))
//                return Json(new { success = false, message = "Chưa đăng nhập" });

//            if (dto == null || string.IsNullOrWhiteSpace(dto.OldPassword) ||
//                string.IsNullOrWhiteSpace(dto.NewPassword))
//                return Json(new { success = false, message = "Vui lòng điền đầy đủ!" });

//            if (dto.NewPassword.Length < 6)
//                return Json(new { success = false, message = "Mật khẩu mới phải từ 6 ký tự!" });

//            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
//            if (user == null)
//                return Json(new { success = false, message = "Không tìm thấy tài khoản" });

//            if (user.Password != dto.OldPassword)
//                return Json(new { success = false, message = "Mật khẩu hiện tại không đúng!" });

//            user.Password = dto.NewPassword;
//            _context.SaveChanges();

//            return Json(new { success = true });
//        }
//    }

//    // ================================================================
//    // DTOs — đặt cùng file cho tiện, hoặc tách ra Models/DTOs nếu muốn
//    // ================================================================
//    public class UpdateProfileRequest
//    {
//        public string FullName { get; set; } = "";
//        public string? Phone { get; set; }
//        public string? Address { get; set; }
//    }

//    public class ChangePasswordRequest
//    {
//        public string OldPassword { get; set; } = "";
//        public string NewPassword { get; set; } = "";
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;

namespace Web_QuanLy_PolyCafe.Controllers
{
    public class AccountController : Controller
    {
        private readonly PolyCafeDbContext _context;

        public AccountController(PolyCafeDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToAction("POS", "Order");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu.";
                ViewBag.Email = email;
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    (u.Email == email.Trim() || u.Phone == email.Trim())
                    && u.Password == password
                    && u.IsActive);

            if (user == null)
            {
                ViewBag.Error = "Email / số điện thoại hoặc mật khẩu không đúng.";
                ViewBag.Email = email;
                return View();
            }

            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role ? "Admin" : "Staff");

            return RedirectToAction("POS", "Order");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // ============================================================
        // GET: /Account/Profile
        // ============================================================
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var user = _context.Users
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // ============================================================
        // POST: /Account/UpdateProfile  (AJAX JSON)
        // ============================================================
        [HttpPost]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest dto)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Chưa đăng nhập" });

            if (string.IsNullOrWhiteSpace(dto?.FullName))
                return Json(new { success = false, message = "Họ tên không được để trống!" });

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy tài khoản" });

            user.FullName = dto.FullName.Trim();
            user.Phone = dto.Phone?.Trim();
            user.Address = dto.Address?.Trim();
            _context.SaveChanges();

            // Cập nhật lại session
            HttpContext.Session.SetString("FullName", user.FullName);

            return Json(new { success = true });
        }

        // ============================================================
        // POST: /Account/ChangePassword  (AJAX JSON)
        // ============================================================
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest dto)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Chưa đăng nhập" });

            if (dto == null || string.IsNullOrWhiteSpace(dto.OldPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
                return Json(new { success = false, message = "Vui lòng điền đầy đủ!" });

            if (dto.NewPassword.Length < 6)
                return Json(new { success = false, message = "Mật khẩu mới phải từ 6 ký tự!" });

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy tài khoản" });

            // Web nội bộ — không cần kiểm tra mật khẩu cũ
            user.Password = dto.NewPassword;
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }

    // ================================================================
    // DTOs — đặt cùng file cho tiện, hoặc tách ra Models/DTOs nếu muốn
    // ================================================================
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}