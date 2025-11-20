using Microsoft.AspNetCore.Mvc;
using E_TestHub.Models;
using E_TestHub.Services;

namespace E_TestHub.Controllers
{
    public class SubjectController : BaseController
    {
        private readonly ISubjectApiService _subjectApiService;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectApiService subjectApiService, ILogger<SubjectController> logger)
        {
            _subjectApiService = subjectApiService;
            _logger = logger;
        }

        // GET: Subject/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                
                ViewBag.TotalSubjects = subjects.Count;
                ViewBag.Subjects = subjects;

                return View(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading subjects");
                TempData["ErrorMessage"] = "Không thể tải danh sách môn học. Vui lòng thử lại.";
                return View(new List<Subject>());
            }
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if code already exists
                var codeExists = await _subjectApiService.CheckSubjectCodeExistsAsync(model.Code);
                if (codeExists)
                {
                    ModelState.AddModelError("Code", "Mã môn học đã tồn tại trong hệ thống.");
                    return View(model);
                }

                // Create subject
                var subject = new Subject
                {
                    Name = model.Name,
                    Code = model.Code.ToUpper(),
                    Description = model.Description
                };

                var success = await _subjectApiService.CreateSubjectAsync(subject);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Môn học {subject.Name} ({subject.Code}) đã được tạo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo môn học. Vui lòng thử lại.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo môn học.");
                return View(model);
            }
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID môn học không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var subject = await _subjectApiService.GetSubjectByIdAsync(id);

                if (subject == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy môn học.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new EditSubjectViewModel
                {
                    Id = subject.Id,
                    Name = subject.Name,
                    Code = subject.Code,
                    Description = subject.Description
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading subject for edit: {id}");
                TempData["ErrorMessage"] = "Không thể tải thông tin môn học.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditSubjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if code already exists (excluding current subject)
                var codeExists = await _subjectApiService.CheckSubjectCodeExistsAsync(model.Code, model.Id);
                if (codeExists)
                {
                    ModelState.AddModelError("Code", "Mã môn học đã được sử dụng bởi môn học khác.");
                    return View(model);
                }

                // Update subject
                var subject = new Subject
                {
                    Id = model.Id,
                    Name = model.Name,
                    Code = model.Code.ToUpper(),
                    Description = model.Description
                };

                var success = await _subjectApiService.UpdateSubjectAsync(subject);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Môn học {subject.Name} đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật môn học. Vui lòng thử lại.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật môn học.");
                return View(model);
            }
        }

        // POST: Subject/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID môn học không hợp lệ." });
            }

            try
            {
                var success = await _subjectApiService.DeleteSubjectAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Môn học đã được xóa thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa môn học. Có thể môn học đang được sử dụng." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subject: {id}");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa môn học." });
            }
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID môn học không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var subject = await _subjectApiService.GetSubjectByIdAsync(id);

                if (subject == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy môn học.";
                    return RedirectToAction(nameof(Index));
                }

                return View(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading subject details: {id}");
                TempData["ErrorMessage"] = "Không thể tải thông tin môn học.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Subject/CheckCodeExists
        [HttpPost]
        public async Task<IActionResult> CheckCodeExists(string code, string? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(new { exists = false });
            }

            try
            {
                var exists = await _subjectApiService.CheckSubjectCodeExistsAsync(code, excludeId);
                return Json(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking code exists: {code}");
                return Json(new { exists = false, error = true });
            }
        }
    }
}
