using System.Diagnostics;
using E_TestHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        public IActionResult Login(string email, string password)
        {
            // Demo: kiểm tra tài khoản mẫu
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Tài khoản mẫu: admin@example.com / 123456
            if (email == "admin@example.com" && password == "123456")
            {
                // Đăng nhập thành công, chuyển hướng đến Dashboard
                return RedirectToAction("Dashboard");
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

        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
