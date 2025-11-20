using Microsoft.AspNetCore.Mvc;
using E_TestHub.Models;
using E_TestHub.Services;

namespace E_TestHub.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IUserApiService _userApiService;
        private readonly IClassApiService _classApiService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserApiService userApiService, 
            IClassApiService classApiService,
            ILogger<AdminController> logger)
        {
            _userApiService = userApiService;
            _classApiService = classApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get real statistics from API
                var users = await _userApiService.GetAllUsersAsync();
                
                var stats = new
                {
                    TotalUsers = users?.Count ?? 0,
                    ActiveUsers = users?.Count(u => u.IsActive) ?? 0,
                    InactiveUsers = users?.Count(u => !u.IsActive) ?? 0,
                    TotalStudents = users?.Count(u => u.Role == UserRole.Student) ?? 0,
                    TotalTeachers = users?.Count(u => u.Role == UserRole.Teacher) ?? 0,
                    TotalAdmins = users?.Count(u => u.Role == UserRole.Admin) ?? 0,
                    NewUsersThisMonth = users?.Count(u => u.CreatedDate >= DateTime.Now.AddMonths(-1)) ?? 0
                };
                
                ViewBag.SystemStats = stats;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading dashboard: {ex.Message}");
                // Fallback to empty stats
                ViewBag.SystemStats = new
                {
                    TotalUsers = 0,
                    ActiveUsers = 0,
                    InactiveUsers = 0,
                    TotalStudents = 0,
                    TotalTeachers = 0,
                    TotalAdmins = 0,
                    NewUsersThisMonth = 0
                };
                return View();
            }
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate password confirmation
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            // Create new user object
            var newUser = new User
            {
                Email = model.Email,
                FullName = model.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Set role-specific properties
            switch (model.Role)
            {
                case UserRole.Student:
                    newUser.StudentId = model.StudentId;
                    newUser.Class = model.Class;
                    break;
                case UserRole.Teacher:
                    newUser.EmployeeId = model.EmployeeId;
                    newUser.Department = model.Department;
                    break;
                case UserRole.Admin:
                    // Admins may also have an EmployeeId in addition to Position
                    newUser.EmployeeId = model.EmployeeId;
                    newUser.Position = model.Position;
                    break;
            }

            // Add user via API
            var success = await _userApiService.CreateUserAsync(newUser, model.Password);

            if (success)
            {
                TempData["SuccessMessage"] = $"Người dùng {newUser.FullName} đã được tạo thành công!";
                return RedirectToAction("UserManagement");
            }
            else
            {
                ModelState.AddModelError("", "Không thể tạo người dùng. Vui lòng kiểm tra lại thông tin.");
                return View(model);
            }
        }

        public IActionResult SystemSettings()
        {
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            try
            {
                // Get real statistics from API
                var users = await _userApiService.GetAllUsersAsync();
                
                var stats = new
                {
                    TotalUsers = users?.Count ?? 0,
                    ActiveUsers = users?.Count(u => u.IsActive) ?? 0,
                    InactiveUsers = users?.Count(u => !u.IsActive) ?? 0,
                    TotalStudents = users?.Count(u => u.Role == UserRole.Student) ?? 0,
                    TotalTeachers = users?.Count(u => u.Role == UserRole.Teacher) ?? 0,
                    TotalAdmins = users?.Count(u => u.Role == UserRole.Admin) ?? 0,
                    TotalExams = 0, // TODO: Get from Exam API when available
                    CompletedExams = 0, // TODO: Get from Submission API when available
                    TotalClasses = 0, // TODO: Get from Class API when available
                    TotalSubjects = 0, // TODO: Get from Subject API when available
                    UsersByRole = new[]
                    {
                        new { Role = "Student", Count = users?.Count(u => u.Role == UserRole.Student) ?? 0 },
                        new { Role = "Teacher", Count = users?.Count(u => u.Role == UserRole.Teacher) ?? 0 },
                        new { Role = "Admin", Count = users?.Count(u => u.Role == UserRole.Admin) ?? 0 }
                    }
                };
                
                ViewBag.SystemStats = stats;
                
                // Set ViewBag for backward compatibility
                ViewBag.TotalUsers = stats.TotalUsers;
                ViewBag.ActiveUsers = stats.ActiveUsers;
                ViewBag.StudentsCount = stats.TotalStudents;
                ViewBag.TeachersCount = stats.TotalTeachers;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading reports: {ex.Message}");
                ViewBag.SystemStats = new
                {
                    TotalUsers = 0,
                    ActiveUsers = 0,
                    InactiveUsers = 0,
                    TotalStudents = 0,
                    TotalTeachers = 0,
                    TotalAdmins = 0,
                    TotalExams = 0,
                    CompletedExams = 0,
                    TotalClasses = 0,
                    TotalSubjects = 0,
                    UsersByRole = new object[] { }
                };
                
                ViewBag.TotalUsers = 0;
                ViewBag.ActiveUsers = 0;
                ViewBag.StudentsCount = 0;
                ViewBag.TeachersCount = 0;
                
                return View();
            }
        }

        public async Task<IActionResult> SchoolManagement()
        {
            try
            {
                // Check if user is authenticated
                var token = HttpContext.Session.GetString("ApiToken");
                _logger.LogInformation($"SchoolManagement - ApiToken exists: {!string.IsNullOrEmpty(token)}");
                
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No API token found in session for SchoolManagement");
                    TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Home");
                }
                
                var classes = await _classApiService.GetAllClassesAsync();
                _logger.LogInformation($"SchoolManagement - Retrieved {classes?.Count ?? 0} classes");
                
                // Populate teacher names
                var users = await _userApiService.GetAllUsersAsync();
                _logger.LogInformation($"SchoolManagement - Retrieved {users?.Count ?? 0} users");
                
                if (classes != null && users != null)
                {
                    foreach (var cls in classes)
                    {
                        if (!string.IsNullOrEmpty(cls.TeacherId))
                        {
                            cls.Teacher = users.FirstOrDefault(u => u.ApiId == cls.TeacherId);
                        }
                    }
                }
                
                ViewBag.Classes = classes ?? new List<Class>();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading school management data");
                TempData["ErrorMessage"] = $"Không thể tải dữ liệu: {ex.Message}. Vui lòng kiểm tra MongoDB API có đang chạy không.";
                ViewBag.Classes = new List<Class>();
                return View();
            }
        }

        public IActionResult AuditLogs()
        {
            return View();
        }

        public IActionResult TestApi()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { exists = false });
            }

            // Check via API
            var userFromApi = await _userApiService.GetUserByEmailAsync(email);
            return Json(new { exists = userFromApi != null });
        }

        public async Task<IActionResult> UserManagement()
        {
            try
            {
                // Get users from API
                var users = await _userApiService.GetAllUsersAsync();
                
                if (users == null)
                {
                    users = new List<User>();
                }

                ViewBag.Users = users;
                ViewBag.TotalUsers = users.Count;
                ViewBag.ActiveUsers = users.Count(u => u.IsActive);
                ViewBag.InactiveUsers = users.Count(u => !u.IsActive);
                ViewBag.StudentsCount = users.Count(u => u.Role == UserRole.Student);
                ViewBag.TeachersCount = users.Count(u => u.Role == UserRole.Teacher);
                ViewBag.AdminsCount = users.Count(u => u.Role == UserRole.Admin);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading user management: {ex.Message}");
                ViewBag.Users = new List<User>();
                ViewBag.TotalUsers = 0;
                ViewBag.ActiveUsers = 0;
                ViewBag.InactiveUsers = 0;
                ViewBag.StudentsCount = 0;
                ViewBag.TeachersCount = 0;
                ViewBag.AdminsCount = 0;
                return View();
            }
        }

        public IActionResult Profile()
        {
            return View();
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID người dùng không hợp lệ.";
                return RedirectToAction("UserManagement");
            }

            // Try to get user from API (id is MongoDB ObjectId)
            var user = await _userApiService.GetUserByIdAsync(id);

            // Fallback to AdminData if API fails (not applicable for MongoDB ObjectId)
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng trong hệ thống.";
                return RedirectToAction("UserManagement");
            }

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("UserManagement");
            }

            // Map User to EditUserViewModel
            var model = new EditUserViewModel
            {
                Id = user.Id,
                ApiId = user.ApiId, // CRITICAL: MongoDB ObjectId for updates
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                StudentId = user.StudentId,
                Class = user.Class,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                Position = user.Position
            };

            return View(model);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            // Debug logging
            _logger.LogInformation("=== EditUser POST START ===");
            _logger.LogInformation($"EditUser POST called - Email: {model?.Email}, ApiId: {model?.ApiId}, Id: {model?.Id}");
            
            // Remove Id validation errors (we only need ApiId)
            ModelState.Remove("Id");
            
            _logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");
            
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is INVALID!");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state != null && state.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            _logger.LogError($"Validation error on '{key}': {error.ErrorMessage} | Exception: {error.Exception?.Message}");
                        }
                    }
                }
                TempData["ErrorMessage"] = "Có lỗi validation. Vui lòng kiểm tra lại thông tin.";
                return View(model);
            }

            if (model == null)
            {
                _logger.LogError("Model is NULL!");
                return BadRequest();
            }

            // Map ViewModel to User
            var user = new User
            {
                Id = model.Id,
                ApiId = model.ApiId ?? string.Empty, // CRITICAL: MongoDB ObjectId for API update
                Email = model.Email,
                FullName = model.FullName,
                Role = model.Role,
                IsActive = model.IsActive,
                StudentId = model.StudentId,
                Class = model.Class,
                EmployeeId = model.EmployeeId,
                Department = model.Department,
                Position = model.Position
            };

            _logger.LogInformation($"Attempting to update user with ApiId: {user.ApiId}");
            
            // Update via MongoDB API (requires ApiId)
            var success = await _userApiService.UpdateUserAsync(user);

            if (success)
            {
                TempData["SuccessMessage"] = $"Người dùng {user.FullName} đã được cập nhật thành công!";
                return RedirectToAction("UserManagement");
            }
            else
            {
                ModelState.AddModelError("", "Không thể cập nhật người dùng. Vui lòng thử lại.");
                return View(model);
            }
        }

        // POST: Admin/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID người dùng không hợp lệ." });
            }

            // Get current user ID from session (MongoDB ObjectId)
            var currentUserApiId = HttpContext.Session.GetString("ApiId");
            if (currentUserApiId == id)
            {
                return Json(new { success = false, message = "Bạn không thể xóa tài khoản của chính mình." });
            }

            // Delete via API (id is MongoDB ObjectId)
            var success = await _userApiService.DeleteUserAsync(id);

            if (success)
            {
                return Json(new { success = true, message = "Người dùng đã được xóa thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Không thể xóa người dùng. Vui lòng thử lại." });
            }
        }

        // GET: Admin/UserStatistics
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var stats = await _userApiService.GetUserStatisticsAsync();
                
                if (stats == null || stats.Count == 0)
                {
                    // Get from users list if statistics endpoint not available
                    var users = await _userApiService.GetAllUsersAsync();
                    if (users != null)
                    {
                        stats = new Dictionary<string, object>
                        {
                            { "total", users.Count },
                            { "active", users.Count(u => u.IsActive) },
                            { "byRole", new[]
                                {
                                    new { role = "student", count = users.Count(u => u.Role == UserRole.Student) },
                                    new { role = "teacher", count = users.Count(u => u.Role == UserRole.Teacher) },
                                    new { role = "admin", count = users.Count(u => u.Role == UserRole.Admin) }
                                }
                            }
                        };
                    }
                    else
                    {
                        stats = new Dictionary<string, object>
                        {
                            { "total", 0 },
                            { "active", 0 },
                            { "byRole", new object[] { } }
                        };
                    }
                }

                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                return Json(new { error = "Failed to get statistics" });
            }
        }
    }
}