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

        public IActionResult Dashboard_GV()
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
