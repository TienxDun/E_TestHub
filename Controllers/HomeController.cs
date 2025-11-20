using System.Diagnostics;
using E_TestHub.Services;
using E_TestHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiService _userApiService;

        public HomeController(ILogger<HomeController> logger, IUserApiService userApiService)
        {
            _logger = logger;
            _userApiService = userApiService;
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
        public async Task<IActionResult> Login(string email, string password, string selectedRole)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }
            if (string.IsNullOrEmpty(selectedRole))
            {
                ViewBag.Error = "Vui lòng chọn loại tài khoản.";
                return View();
            }

            // Chỉ dùng API thật
            var (user, token) = await _userApiService.AuthenticateAsync(email, password);

            if (user != null)
            {
                // So khớp role người dùng chọn với role từ API
                var selected = selectedRole.ToLower();
                var matched = (selected == "admin" && user.Role == UserRole.Admin)
                              || (selected == "teacher" && user.Role == UserRole.Teacher)
                              || (selected == "student" && user.Role == UserRole.Student);
                if (!matched)
                {
                    ViewBag.Error = "Vai trò không đúng với tài khoản. Vui lòng chọn đúng loại tài khoản.";
                    return View();
                }

                // Đăng nhập thành công, lưu thông tin user vào session
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserId", user.ApiId ?? user.Id.ToString()); // Use MongoDB ObjectId as primary UserId
                HttpContext.Session.SetString("ApiId", user.ApiId ?? ""); // MongoDB ObjectId
                
                // CRITICAL: Save API token to session for subsequent API calls
                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("ApiToken", token);
                }

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

        // API endpoint for JavaScript login
        [HttpPost]
        [Route("api/auth/login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
            }
            
            if (string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { success = false, message = "Vui lòng chọn loại tài khoản." });
            }

            // Call Node.js API to authenticate
            var (user, token) = await _userApiService.AuthenticateAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Email hoặc mật khẩu không đúng." });
            }

            // Check if role matches
            var roleMatches = request.Role.ToLower() switch
            {
                "admin" => user.Role == UserRole.Admin,
                "teacher" => user.Role == UserRole.Teacher,
                "student" => user.Role == UserRole.Student,
                _ => false
            };

            if (!roleMatches)
            {
                return Unauthorized(new { success = false, message = "Bạn không có quyền đăng nhập với loại tài khoản đã chọn." });
            }

            // Set session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserId", user.ApiId ?? user.Id.ToString()); // Use MongoDB ObjectId as primary UserId
            HttpContext.Session.SetString("ApiId", user.ApiId ?? ""); // MongoDB ObjectId
            
            // CRITICAL: Save API token to session
            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Session.SetString("ApiToken", token);
            }

            // Return success with redirect URL
            var redirectUrl = user.Role switch
            {
                UserRole.Admin => "/Admin/Dashboard",
                UserRole.Teacher => "/Teacher/Dashboard",
                UserRole.Student => "/Student/Dashboard",
                _ => "/"
            };

            return Ok(new 
            { 
                success = true, 
                role = user.Role.ToString().ToLower(),
                redirectUrl = redirectUrl,
                token = "session-based" // For compatibility
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // DTO for API login
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
