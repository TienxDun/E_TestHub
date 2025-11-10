using Microsoft.AspNetCore.Mvc;
using E_TestHub.Models;
using E_TestHub.Services;

namespace E_TestHub.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly IQuestionApiService _questionApiService;
        private readonly IExamApiService _examApiService;
        private readonly ISubjectApiService _subjectApiService;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(
            IQuestionApiService questionApiService,
            IExamApiService examApiService,
            ISubjectApiService subjectApiService,
            ILogger<TeacherController> logger)
        {
            _questionApiService = questionApiService;
            _examApiService = examApiService;
            _subjectApiService = subjectApiService;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        #region Question Bank Management - Phase 2

        /// <summary>
        /// GET: Hiển thị danh sách câu hỏi với filters
        /// </summary>
        public async Task<IActionResult> QuestionBank(string? subjectId = null, int? difficulty = null, int? type = null)
        {
            try
            {
                var questions = await _questionApiService.GetAllQuestionsAsync();
                var subjects = await _subjectApiService.GetAllSubjectsAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(subjectId))
                {
                    questions = questions.Where(q => q.SubjectId == subjectId).ToList();
                }
                if (difficulty.HasValue)
                {
                    questions = questions.Where(q => (int)q.DifficultyLevel == difficulty.Value).ToList();
                }
                if (type.HasValue)
                {
                    questions = questions.Where(q => (int)q.Type == type.Value).ToList();
                }

                // Populate SubjectName for display
                foreach (var question in questions)
                {
                    var subject = subjects.FirstOrDefault(s => s.Id == question.SubjectId);
                    question.SubjectName = subject?.Name ?? "N/A";
                }

                ViewBag.Questions = questions;
                ViewBag.Subjects = subjects;
                ViewBag.SelectedSubjectId = subjectId;
                ViewBag.SelectedDifficulty = difficulty;
                ViewBag.SelectedType = type;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading questions: {ex.Message}");
                ViewBag.Questions = new List<Question>();
                ViewBag.Subjects = new List<Subject>();
                TempData["ErrorMessage"] = "Không thể tải danh sách câu hỏi.";
                return View();
            }
        }

        /// <summary>
        /// GET: Hiển thị form tạo câu hỏi mới
        /// </summary>
        public async Task<IActionResult> CreateQuestion()
        {
            try
            {
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create question form: {ex.Message}");
                TempData["ErrorMessage"] = "Không thể tải form tạo câu hỏi.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// POST: Tạo câu hỏi mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(CreateQuestionViewModel model)
        {
            try
            {
                // Custom validation for CorrectAnswer
                if (model.Type == QuestionType.MultipleChoice && 
                    (model.Options == null || !model.Options.Contains(model.CorrectAnswer ?? "")))
                {
                    ModelState.AddModelError("CorrectAnswer", "Đáp án đúng phải là một trong các lựa chọn.");
                }

                if (!ModelState.IsValid)
                {
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }

                // Convert ViewModel to Question
                var question = new Question
                {
                    SubjectId = model.SubjectId,
                    Content = model.Content,
                    Type = model.Type,
                    Options = model.Options ?? new List<string>(),
                    CorrectAnswer = model.CorrectAnswer,
                    Score = model.Score,
                    DifficultyLevel = model.DifficultyLevel
                };

                var result = await _questionApiService.CreateQuestionAsync(question);

                if (result != null)
                {
                    TempData["SuccessMessage"] = "Tạo câu hỏi thành công!";
                    return RedirectToAction("QuestionBank");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo câu hỏi. Vui lòng thử lại.";
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating question: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo câu hỏi.";
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;
                return View(model);
            }
        }

        /// <summary>
        /// GET: Hiển thị form chỉnh sửa câu hỏi
        /// </summary>
        public async Task<IActionResult> EditQuestion(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "ID câu hỏi không hợp lệ.";
                    return RedirectToAction("QuestionBank");
                }

                var question = await _questionApiService.GetQuestionByIdAsync(id);

                if (question == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi.";
                    return RedirectToAction("QuestionBank");
                }

                var model = new EditQuestionViewModel
                {
                    Id = question.Id,
                    ApiId = question.ApiId,
                    SubjectId = question.SubjectId,
                    Content = question.Content,
                    Type = question.Type,
                    Options = question.Options,
                    CorrectAnswer = question.CorrectAnswer,
                    Score = question.Score,
                    DifficultyLevel = question.DifficultyLevel
                };

                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit question form: {ex.Message}");
                TempData["ErrorMessage"] = "Không thể tải form chỉnh sửa câu hỏi.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// POST: Cập nhật câu hỏi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(EditQuestionViewModel model)
        {
            try
            {
                ModelState.Remove("Id");

                if (model.Type == QuestionType.MultipleChoice && 
                    (model.Options == null || !model.Options.Contains(model.CorrectAnswer ?? "")))
                {
                    ModelState.AddModelError("CorrectAnswer", "Đáp án đúng phải là một trong các lựa chọn.");
                }

                if (!ModelState.IsValid)
                {
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }

                // Convert ViewModel to Question
                var question = new Question
                {
                    ApiId = model.ApiId,
                    SubjectId = model.SubjectId,
                    Content = model.Content,
                    Type = model.Type,
                    Options = model.Options ?? new List<string>(),
                    CorrectAnswer = model.CorrectAnswer,
                    Score = model.Score,
                    DifficultyLevel = model.DifficultyLevel
                };

                var result = await _questionApiService.UpdateQuestionAsync(question);

                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật câu hỏi thành công!";
                    return RedirectToAction("QuestionBank");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật câu hỏi.";
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating question: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật câu hỏi.";
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;
                return View(model);
            }
        }

        /// <summary>
        /// POST: Xóa câu hỏi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID câu hỏi không hợp lệ." });
                }

                var success = await _questionApiService.DeleteQuestionAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Câu hỏi đã được xóa thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa câu hỏi. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting question {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa câu hỏi." });
            }
        }

        #endregion

        public IActionResult ExamManagement()
        {
            // Demo data for recent exams (4 most recent)
            var recentExams = new List<dynamic>
            {
                new { Id = 4, Name = "XSTK - 4", QuestionCount = 60, CreatedDate = new DateTime(2025, 9, 10) },
                new { Id = 3, Name = "XSTK - 3", QuestionCount = 120, CreatedDate = new DateTime(2025, 9, 7) },
                new { Id = 2, Name = "XSTK - 2", QuestionCount = 60, CreatedDate = new DateTime(2025, 8, 30) },
                new { Id = 1, Name = "XSTK - 1", QuestionCount = 60, CreatedDate = new DateTime(2025, 8, 15) }
            };

            // Demo data for all exams table
            var allExams = new List<dynamic>
            {
                new { Id = 1, Name = "XSTK-1", SubmittedCount = 60, Status = "Đã xuất bản", AssignedTo = "Tất cả mọi người" },
                new { Id = 2, Name = "XSTK-2", SubmittedCount = 0, Status = "", AssignedTo = "" },
                new { Id = 3, Name = "XSTK-3", SubmittedCount = 0, Status = "", AssignedTo = "" },
                new { Id = 4, Name = "XSTK-4", SubmittedCount = 40, Status = "Đã xuất bản", AssignedTo = "Tất cả mọi người" }
            };

            ViewBag.RecentExams = recentExams;
            ViewBag.AllExams = allExams;

            return View();
        }

        public IActionResult CreateExam()
        {
            // Demo questions for question bank
            var questions = new List<dynamic>
            {
                new { Id = 1, Text = "Xác suất của biến cố chắc chắn bằng bao nhiêu?", Subject = "Xác suất thống kê", Difficulty = "easy", DifficultyLabel = "Dễ", Points = 1 },
                new { Id = 2, Text = "Công thức tính phương sai của biến ngẫu nhiên X là gì?", Subject = "Xác suất thống kê", Difficulty = "medium", DifficultyLabel = "Trung bình", Points = 2 },
                new { Id = 3, Text = "Định lý giới hạn trung tâm được áp dụng trong trường hợp nào?", Subject = "Xác suất thống kê", Difficulty = "hard", DifficultyLabel = "Khó", Points = 3 },
                new { Id = 4, Text = "Phân phối chuẩn có các tham số nào?", Subject = "Xác suất thống kê", Difficulty = "medium", DifficultyLabel = "Trung bình", Points = 2 },
                new { Id = 5, Text = "Kỳ vọng của tổng hai biến ngẫu nhiên độc lập bằng gì?", Subject = "Xác suất thống kê", Difficulty = "easy", DifficultyLabel = "Dễ", Points = 1 },
                new { Id = 6, Text = "Hệ số tương quan Pearson đo lường điều gì?", Subject = "Xác suất thống kê", Difficulty = "medium", DifficultyLabel = "Trung bình", Points = 2 },
                new { Id = 7, Text = "Giải thích khái niệm kiểm định giả thuyết thống kê?", Subject = "Xác suất thống kê", Difficulty = "hard", DifficultyLabel = "Khó", Points = 3 },
                new { Id = 8, Text = "Phân phối nhị thức được áp dụng trong trường hợp nào?", Subject = "Xác suất thống kê", Difficulty = "medium", DifficultyLabel = "Trung bình", Points = 2 },
                new { Id = 9, Text = "Công thức tính xác suất có điều kiện P(A|B) là gì?", Subject = "Xác suất thống kê", Difficulty = "easy", DifficultyLabel = "Dễ", Points = 1 },
                new { Id = 10, Text = "Phân tích ưu nhược điểm của các phương pháp ước lượng tham số", Subject = "Xác suất thống kê", Difficulty = "hard", DifficultyLabel = "Khó", Points = 3 },
                new { Id = 11, Text = "Độ lệch chuẩn được tính bằng công thức nào?", Subject = "Xác suất thống kê", Difficulty = "easy", DifficultyLabel = "Dễ", Points = 1 },
                new { Id = 12, Text = "Hàm mật độ xác suất của phân phối chuẩn có dạng như thế nào?", Subject = "Xác suất thống kê", Difficulty = "medium", DifficultyLabel = "Trung bình", Points = 2 }
            };

            ViewBag.Questions = questions;
            return View();
        }

        public IActionResult ViewResults()
        {
            // Demo data for subject results summary
            var subjectResults = new List<dynamic>
            {
                new { 
                    SubjectCode = "CSDLPT", 
                    SubjectName = "Cơ sở dữ liệu phân tán", 
                    ExamCount = 12, 
                    AverageScore = 5.5 
                },
                new { 
                    SubjectCode = "CNPMNC", 
                    SubjectName = "Công nghệ PM NC", 
                    ExamCount = 12, 
                    AverageScore = 4.5 
                },
                new { 
                    SubjectCode = "XSTK", 
                    SubjectName = "Xác suất thống kê", 
                    ExamCount = 6, 
                    AverageScore = 2.2 
                },
                new { 
                    SubjectCode = "TATO1", 
                    SubjectName = "Tiếng Anh TQ 1", 
                    ExamCount = 6, 
                    AverageScore = 10.0 
                }
            };

            // Calculate summary statistics
            ViewBag.TotalSubjects = subjectResults.Count;
            ViewBag.TotalExams = subjectResults.Sum(s => (int)s.ExamCount);
            ViewBag.SubjectResults = subjectResults;

            return View();
        }

        public IActionResult SubjectExamDetails(string subjectCode)
        {
            // Demo data for subject info
            var subjectInfo = subjectCode switch
            {
                "CSDLPT" => new { Code = "CSDLPT", Name = "Cơ sở dữ liệu phân tán", ExamCount = 12, AverageScore = 5.5 },
                "CNPMNC" => new { Code = "CNPMNC", Name = "Công nghệ PM NC", ExamCount = 12, AverageScore = 4.5 },
                "XSTK" => new { Code = "XSTK", Name = "Xác suất thống kê", ExamCount = 6, AverageScore = 2.2 },
                "TATO1" => new { Code = "TATO1", Name = "Tiếng Anh TQ 1", ExamCount = 6, AverageScore = 10.0 },
                _ => new { Code = subjectCode, Name = "Unknown Subject", ExamCount = 0, AverageScore = 0.0 }
            };

            // Demo data for exams in this subject
            var exams = new List<dynamic>
            {
                new { Id = 1, Name = $"{subjectCode} - Kiểm tra giữa kỳ", Date = new DateTime(2025, 9, 15), Duration = 90, StudentCount = 60 },
                new { Id = 2, Name = $"{subjectCode} - Kiểm tra thường xuyên 1", Date = new DateTime(2025, 8, 30), Duration = 45, StudentCount = 58 },
                new { Id = 3, Name = $"{subjectCode} - Bài tập lớn", Date = new DateTime(2025, 8, 20), Duration = 120, StudentCount = 60 },
            };

            // Demo data for student results
            var studentResults = new List<dynamic>
            {
                new { StudentId = "2151012001", StudentName = "Nguyễn Văn A", Exam1 = 8.5, Exam2 = 7.0, Exam3 = 9.0, Average = 8.2 },
                new { StudentId = "2151012002", StudentName = "Trần Thị B", Exam1 = 6.5, Exam2 = 7.5, Exam3 = 8.0, Average = 7.3 },
                new { StudentId = "2151012003", StudentName = "Lê Văn C", Exam1 = 5.0, Exam2 = 6.0, Exam3 = 5.5, Average = 5.5 },
                new { StudentId = "2151012004", StudentName = "Phạm Thị D", Exam1 = 9.0, Exam2 = 9.5, Exam3 = 8.5, Average = 9.0 },
                new { StudentId = "2151012005", StudentName = "Hoàng Văn E", Exam1 = 4.5, Exam2 = 5.0, Exam3 = 4.0, Average = 4.5 },
                new { StudentId = "2151012006", StudentName = "Đỗ Thị F", Exam1 = 7.5, Exam2 = 8.0, Exam3 = 7.0, Average = 7.5 },
                new { StudentId = "2151012007", StudentName = "Vũ Văn G", Exam1 = 3.5, Exam2 = 4.0, Exam3 = 3.0, Average = 3.5 },
                new { StudentId = "2151012008", StudentName = "Bùi Thị H", Exam1 = 8.0, Exam2 = 8.5, Exam3 = 9.0, Average = 8.5 },
            };

            ViewBag.SubjectCode = subjectCode;
            ViewBag.SubjectInfo = subjectInfo;
            ViewBag.Exams = exams;
            ViewBag.StudentResults = studentResults;

            return View();
        }

        public IActionResult ManageClasses()
        {
            return View();
        }

        public IActionResult ClassDetails(string classId)
        {
            // Demo data for class information
            var classInfo = classId switch
            {
                "PM233H" => new { Id = "PM233H", Name = "PM233H - Xác suất thống kê", StudentCount = 360, Year = "2023 - 2027", Subject = "Xác suất thống kê" },
                "TM231H" => new { Id = "TM231H", Name = "TM231H - Toán cao cấp", StudentCount = 36, Year = "2023 - 2027", Subject = "Toán cao cấp" },
                "SE214H" => new { Id = "SE214H", Name = "SE214H - Kỹ thuật phần mềm", StudentCount = 120, Year = "2023 - 2027", Subject = "Kỹ thuật phần mềm" },
                "IT001" => new { Id = "IT001", Name = "IT001 - Tin học đại cương", StudentCount = 180, Year = "2023 - 2027", Subject = "Tin học đại cương" },
                "CS101" => new { Id = "CS101", Name = "CS101 - Nhập môn lập trình", StudentCount = 95, Year = "2023 - 2027", Subject = "Lập trình" },
                "DB234" => new { Id = "DB234", Name = "DB234 - Cơ sở dữ liệu", StudentCount = 150, Year = "2023 - 2027", Subject = "Cơ sở dữ liệu" },
                "AI202" => new { Id = "AI202", Name = "AI202 - Trí tuệ nhân tạo", StudentCount = 80, Year = "2024 - 2028", Subject = "Trí tuệ nhân tạo" },
                "DS101" => new { Id = "DS101", Name = "DS101 - Khoa học dữ liệu", StudentCount = 110, Year = "2024 - 2028", Subject = "Khoa học dữ liệu" },
                _ => new { Id = classId, Name = $"Lớp {classId}", StudentCount = 0, Year = "Unknown", Subject = "Unknown" }
            };

            // Demo data for students in this class
            var students = new List<dynamic>
            {
                new { Id = "2151012001", Name = "Nguyễn Văn A", Email = "2151012001@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012002", Name = "Trần Thị B", Email = "2151012002@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012003", Name = "Lê Văn C", Email = "2151012003@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012004", Name = "Phạm Thị D", Email = "2151012004@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012005", Name = "Hoàng Văn E", Email = "2151012005@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012006", Name = "Đỗ Thị F", Email = "2151012006@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012007", Name = "Vũ Văn G", Email = "2151012007@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012008", Name = "Bùi Thị H", Email = "2151012008@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012009", Name = "Đinh Văn I", Email = "2151012009@student.hcmus.edu.vn", Status = "Active" },
                new { Id = "2151012010", Name = "Cao Thị J", Email = "2151012010@student.hcmus.edu.vn", Status = "Active" }
            };

            // Demo data for exams assigned to this class
            var exams = new List<dynamic>
            {
                new { Id = 1, Name = $"{classInfo.Subject} - Giữa kỳ", Date = new DateTime(2025, 10, 15), Status = "upcoming", SubmittedCount = 0, TotalStudents = classInfo.StudentCount },
                new { Id = 2, Name = $"{classInfo.Subject} - Cuối kỳ", Date = new DateTime(2025, 12, 20), Status = "upcoming", SubmittedCount = 0, TotalStudents = classInfo.StudentCount },
                new { Id = 3, Name = $"{classInfo.Subject} - Bài tập 1", Date = new DateTime(2025, 9, 10), Status = "completed", SubmittedCount = classInfo.StudentCount, TotalStudents = classInfo.StudentCount },
                new { Id = 4, Name = $"{classInfo.Subject} - Bài tập 2", Date = new DateTime(2025, 9, 25), Status = "completed", SubmittedCount = classInfo.StudentCount - 5, TotalStudents = classInfo.StudentCount }
            };

            // Calculate statistics
            var completedExams = exams.Where(e => e.Status == "completed").ToList();
            var totalSubmissions = completedExams.Sum(e => (int)e.SubmittedCount);
            var averageSubmissionRate = completedExams.Any() ? (double)totalSubmissions / (completedExams.Count * classInfo.StudentCount) * 100 : 0;

            ViewBag.ClassId = classId;
            ViewBag.ClassInfo = classInfo;
            ViewBag.Students = students;
            ViewBag.Exams = exams;
            ViewBag.TotalExams = exams.Count;
            ViewBag.CompletedExams = completedExams.Count;
            ViewBag.AverageSubmissionRate = Math.Round(averageSubmissionRate, 1);

            return View();
        }

        public IActionResult GradeExams()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult ExamDetails(int examId)
        {
            // Demo data for exam details
            var exam = new
            {
                Id = examId,
                Name = examId switch
                {
                    1 => "Kiểm tra giữa kỳ - Xác suất thống kê",
                    2 => "Bài tập tuần 5 - Đại số tuyến tính",
                    _ => $"Đề thi số {examId}"
                },
                Subject = examId switch
                {
                    1 => "Xác suất thống kê",
                    2 => "Đại số tuyến tính",
                    _ => "Môn học không xác định"
                },
                Duration = 90, // minutes
                TotalQuestions = examId switch
                {
                    1 => 45,
                    2 => 30,
                    _ => 20
                },
                TotalPoints = examId switch
                {
                    1 => 100,
                    2 => 50,
                    _ => 100
                },
                Status = "published", // or "draft"
                CreatedDate = DateTime.Now.AddDays(-7),
                ScheduledDate = DateTime.Now.AddDays(7),
                Class = "PM233H",
                Description = "Đề thi giữa kỳ với các câu hỏi trắc nghiệm và tự luận."
            };

            // Demo questions
            var questions = new[]
            {
                new { Id = 1, Type = "multiple-choice", Content = "Câu hỏi 1: Xác suất của sự kiện A là?", Points = 2.5 },
                new { Id = 2, Type = "multiple-choice", Content = "Câu hỏi 2: Phương sai của biến ngẫu nhiên X là?", Points = 2.5 },
                new { Id = 3, Type = "essay", Content = "Câu hỏi 3: Giải thích khái niệm kỳ vọng toán học.", Points = 5.0 },
                new { Id = 4, Type = "multiple-choice", Content = "Câu hỏi 4: Phân phối chuẩn là phân phối nào?", Points = 2.5 },
                new { Id = 5, Type = "multiple-choice", Content = "Câu hỏi 5: Độ lệch chuẩn là căn bậc hai của?", Points = 2.5 }
            };

            ViewBag.Exam = exam;
            ViewBag.Questions = questions;
            return View();
        }

        public IActionResult CreateExamSuccess(string examName, string status, int questionCount, int totalPoints)
        {
            // Pass exam creation info to success page
            ViewBag.ExamName = examName;
            ViewBag.Status = status; // "published" or "draft"
            ViewBag.QuestionCount = questionCount;
            ViewBag.TotalPoints = totalPoints;
            ViewBag.CreatedDate = DateTime.Now;
            
            return View();
        }

        public IActionResult ViewStudentExam(string studentId, int examId)
        {
            // Demo data for student info
            var studentInfo = new
            {
                StudentId = studentId,
                StudentName = studentId switch
                {
                    "2151012001" => "Nguyễn Văn A",
                    "2151012002" => "Trần Thị B",
                    "2151012003" => "Lê Văn C",
                    "2151012004" => "Phạm Thị D",
                    "2151012005" => "Hoàng Văn E",
                    "2151012006" => "Đỗ Thị F",
                    "2151012007" => "Vũ Văn G",
                    "2151012008" => "Bùi Thị H",
                    _ => "Unknown Student"
                }
            };

            // Demo data for exam info
            var examInfo = new
            {
                ExamId = examId,
                Name = "Công nghệ phần mềm - Kiểm tra giữa kỳ",
                Subject = "Công nghệ phần mềm",
                ExamDate = new DateTime(2025, 9, 28, 14, 0, 0),
                SubmittedAt = new DateTime(2025, 9, 28, 15, 45, 0),
                Duration = 120, // minutes
                TotalQuestions = 12,
                CorrectAnswers = 8,
                IncorrectAnswers = 4,
                Score = 8.0,
                MaxScore = 10.0
            };

            // Calculate statistics
            var timeSpent = (examInfo.SubmittedAt - examInfo.ExamDate).TotalMinutes;
            var accuracy = Math.Round((double)examInfo.CorrectAnswers / examInfo.TotalQuestions * 100, 1);

            ViewBag.StudentInfo = studentInfo;
            ViewBag.ExamInfo = examInfo;
            ViewBag.TimeSpent = Math.Round(timeSpent, 0);
            ViewBag.Accuracy = accuracy;

            return View();
        }
    }
}