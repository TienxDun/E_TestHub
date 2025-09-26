using System.Diagnostics;
using E_TestHub.Models;
using E_TestHub.Services;
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.AuthenticateAsync(model.Email, model.Password);
            
            if (user != null)
            {
                // Store user info in session (In production, use proper authentication)
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());

                // Redirect based on role
                return user.Role switch
                {
                    UserRole.Student => RedirectToAction("Dashboard", "Student"),
                    UserRole.Teacher => RedirectToAction("Dashboard", "Teacher"),
                    UserRole.Admin => RedirectToAction("Dashboard", "Admin"),
                    _ => RedirectToAction("Index")
                };
            }
            else
            {
                ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return View(model);
            }
        }

        // POST: /Home/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var result = await _userService.ResetPasswordAsync(model.Email);
            
            if (result)
            {
                ViewBag.Message = "Đã gửi email hướng dẫn đặt lại mật khẩu.";
            }
            else
            {
                ViewBag.Error = "Không tìm thấy email trong hệ thống.";
            }

            return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
