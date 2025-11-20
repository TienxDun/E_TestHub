using Microsoft.AspNetCore.Mvc;
using E_TestHub.Models;
using E_TestHub.Services;

namespace E_TestHub.Controllers
{
    public class ClassController : BaseController
    {
        private readonly IClassApiService _classApiService;
        private readonly IUserApiService _userApiService;
        private readonly ISubjectApiService _subjectApiService;
        private readonly ILogger<ClassController> _logger;

        public ClassController(
            IClassApiService classApiService,
            IUserApiService userApiService,
            ISubjectApiService subjectApiService,
            ILogger<ClassController> logger)
        {
            _classApiService = classApiService;
            _userApiService = userApiService;
            _subjectApiService = subjectApiService;
            _logger = logger;
        }

        // GET: Class
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("ApiToken");
                _logger.LogInformation($"ClassController.Index - ApiToken exists: {!string.IsNullOrEmpty(token)}");
                
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No API token found in session");
                    TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Home");
                }
                
                var classes = await _classApiService.GetAllClassesAsync();
                _logger.LogInformation($"ClassController.Index - Retrieved {classes?.Count ?? 0} classes");
                
                if (classes == null || !classes.Any())
                {
                    _logger.LogWarning("No classes returned from API");
                    TempData["InfoMessage"] = "Chưa có lớp học nào. Hãy tạo lớp học mới.";
                    return View(new List<Class>());
                }
                
                // Populate teacher names
                var users = await _userApiService.GetAllUsersAsync();
                foreach (var cls in classes)
                {
                    if (!string.IsNullOrEmpty(cls.TeacherId))
                    {
                        cls.Teacher = users?.FirstOrDefault(u => u.ApiId == cls.TeacherId);
                    }
                }
                
                return View(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading classes");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}. Kiểm tra MongoDB API có chạy không?";
                return View(new List<Class>());
            }
        }

        // GET: Class/Create
        public async Task<IActionResult> Create()
        {
await LoadDropdownData();
            return View(new CreateClassViewModel());
        }

        // POST: Class/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClassViewModel model)
        {
if (ModelState.IsValid)
            {
                try
                {
                    // Check if class code already exists
                    var exists = await _classApiService.CheckClassCodeExistsAsync(model.ClassCode);
                    if (exists)
                    {
                        ModelState.AddModelError("ClassCode", "Mã lớp học đã tồn tại");
                        await LoadDropdownData();
                        return View(model);
                    }

                    var createdClass = await _classApiService.CreateClassAsync(model);
                    
                    if (createdClass != null)
                    {
                        TempData["SuccessMessage"] = "Tạo lớp học thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể tạo lớp học. Vui lòng thử lại.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating class");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo lớp học.");
                }
            }

            await LoadDropdownData();
            return View(model);
        }

        // GET: Class/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var classData = await _classApiService.GetClassByIdAsync(id);
                
                if (classData == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new EditClassViewModel
                {
                    Id = classData.Id,
                    Name = classData.Name,
                    ClassCode = classData.ClassCode,
                    TeacherId = classData.TeacherId,
                    CourseId = classData.CourseId,
                    AcademicYear = classData.AcademicYear,
                    SelectedStudents = classData.Students
                };

                await LoadDropdownData();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading class {id}");
                TempData["ErrorMessage"] = "Không thể tải thông tin lớp học.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Class/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditClassViewModel model)
        {
if (ModelState.IsValid)
            {
                try
                {
                    // Check if class code exists for another class
                    var exists = await _classApiService.CheckClassCodeExistsAsync(model.ClassCode, model.Id);
                    if (exists)
                    {
                        ModelState.AddModelError("ClassCode", "Mã lớp học đã tồn tại cho lớp khác");
                        await LoadDropdownData();
                        return View(model);
                    }

                    var success = await _classApiService.UpdateClassAsync(model);
                    
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Cập nhật lớp học thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể cập nhật lớp học. Vui lòng thử lại.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating class {model.Id}");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật lớp học.");
                }
            }

            await LoadDropdownData();
            return View(model);
        }

        // POST: Class/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _classApiService.DeleteClassAsync(id);
                
                if (success)
                {
                    return Json(new { success = true, message = "Xóa lớp học thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa lớp học. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting class {id}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa lớp học." });
            }
        }

        // GET: Class/Details/5
        public async Task<IActionResult> Details(string id)
        {
if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var classData = await _classApiService.GetClassByIdAsync(id);
                
                if (classData == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học.";
                    return RedirectToAction(nameof(Index));
                }

                // Load teacher info
                if (!string.IsNullOrEmpty(classData.TeacherId))
                {
                    var users = await _userApiService.GetAllUsersAsync();
                    classData.Teacher = users.FirstOrDefault(u => u.ApiId == classData.TeacherId);
                }

                // Load student list
                if (classData.Students.Any())
                {
                    var allUsers = await _userApiService.GetAllUsersAsync();
                    classData.StudentList = allUsers
                        .Where(u => classData.Students.Contains(u.ApiId))
                        .ToList();
                }

                return View(classData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading class details {id}");
                TempData["ErrorMessage"] = "Không thể tải thông tin lớp học.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Class/AddStudents
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudents(string classId, List<string> studentIds)
        {
            try
            {
                var success = await _classApiService.AddStudentsToClassAsync(classId, studentIds);
                
                if (success)
                {
                    return Json(new { success = true, message = "Thêm sinh viên thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể thêm sinh viên. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding students to class {classId}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm sinh viên." });
            }
        }

        // POST: Class/RemoveStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(string classId, string studentId)
        {
            try
            {
                var success = await _classApiService.RemoveStudentFromClassAsync(classId, studentId);
                
                if (success)
                {
                    return Json(new { success = true, message = "Xóa sinh viên khỏi lớp thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa sinh viên. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing student {studentId} from class {classId}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sinh viên." });
            }
        }

        // AJAX: Check if class code exists
        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string classCode, string? excludeId = null)
        {
            var exists = await _classApiService.CheckClassCodeExistsAsync(classCode, excludeId);
            return Json(new { exists });
        }

        // Helper method to load dropdown data
        private async Task LoadDropdownData()
        {
            try
            {
                // Load teachers
                var allUsers = await _userApiService.GetAllUsersAsync();
                ViewBag.Teachers = allUsers.Where(u => u.Role == UserRole.Teacher).ToList();
                ViewBag.Students = allUsers.Where(u => u.Role == UserRole.Student).ToList();

                // Load subjects (courses)
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dropdown data");
                ViewBag.Teachers = new List<User>();
                ViewBag.Students = new List<User>();
                ViewBag.Subjects = new List<Subject>();
            }
        }
    }
}

