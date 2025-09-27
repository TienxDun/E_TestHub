using System.Diagnostics;
using E_TestHub.Services;
using E_TestHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        // POST: /Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var user = await _userService.AuthenticateAsync(email, password);
            if (user != null)
            {
                // Đăng nhập thành công, lưu thông tin user vào session
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserId", user.Id.ToString());

                // Chuyển hướng đến Dashboard phù hợp với role
                return user.Role switch
                {
                    UserRole.Admin => RedirectToAction("Dashboard", "Admin"),
                    UserRole.Teacher => RedirectToAction("Dashboard", "Teacher"),
                    UserRole.Student => RedirectToAction("Dashboard", "Student"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            else
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng.";
                return View();
            }
        }

        // GET: /Home/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Home/Register
        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(fullName))
            {
                ViewBag.Error = "Vui lòng nhập họ tên.";
                return View();
            }

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng nhập email.";
                return View();
            }

            if (string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập mật khẩu.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            // Demo: Giả lập đăng ký thành công
            ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
            return View();
        }

        // GET: /Home/Logout
        public IActionResult Logout()
        {
            // Xóa tất cả session data
            HttpContext.Session.Clear();
            
            // Chuyển hướng về trang chủ
            return RedirectToAction("Login", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
