using Microsoft.AspNetCore.Mvc;
using E_TestHub.Models;
using E_TestHub.Services;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Word = DocumentFormat.OpenXml.Wordprocessing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using Path = System.IO.Path;

namespace E_TestHub.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly IQuestionApiService _questionApiService;
        private readonly ISubjectApiService _subjectApiService;
        private readonly IExamApiService _examApiService;
        private readonly IClassApiService _classApiService;
        private readonly IExamScheduleApiService _examScheduleApiService;
        private readonly ISubmissionApiService _submissionApiService;
        private readonly IUserApiService _userApiService;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(
            IQuestionApiService questionApiService,
            ISubjectApiService subjectApiService,
            IExamApiService examApiService,
            IClassApiService classApiService,
            IExamScheduleApiService examScheduleApiService,
            ISubmissionApiService submissionApiService,
            IUserApiService userApiService,
            ILogger<TeacherController> logger)
        {
            _questionApiService = questionApiService;
            _subjectApiService = subjectApiService;
            _examApiService = examApiService;
            _classApiService = classApiService;
            _examScheduleApiService = examScheduleApiService;
            _submissionApiService = submissionApiService;
            _userApiService = userApiService;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        #region Question Bank Management

        /// <summary>
        /// Hiển thị ngân hàng câu hỏi
        /// </summary>
        public async Task<IActionResult> QuestionBank(string? subjectId = null, string? difficulty = null, string? type = null)
        {
            try
            {
                // Get all questions
                var questions = await _questionApiService.GetAllQuestionsAsync();

                // Filter by subject if provided
                if (!string.IsNullOrEmpty(subjectId))
                {
                    questions = questions.Where(q => q.SubjectId == subjectId).ToList();
                }

                // Filter by difficulty if provided
                if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<DifficultyLevel>(difficulty, true, out var difficultyLevel))
                {
                    questions = questions.Where(q => q.DifficultyLevel == difficultyLevel).ToList();
                }

                // Filter by type if provided
                if (!string.IsNullOrEmpty(type) && Enum.TryParse<QuestionType>(type, true, out var questionType))
                {
                    questions = questions.Where(q => q.Type == questionType).ToList();
                }

                // Get subjects for dropdown
                var subjects = await _subjectApiService.GetAllSubjectsAsync();

                // Populate SubjectName for each question
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
                _logger.LogError($"Error loading question bank: {ex.Message}");
                ViewBag.Questions = new List<Question>();
                ViewBag.Subjects = new List<Subject>();
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
                // Get subjects for dropdown
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create question form: {ex.Message}");
                ViewBag.Subjects = new List<Subject>();
                return View();
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
                // Custom validation: CorrectAnswer required for MultipleChoice and TrueFalse
                if ((model.Type == QuestionType.MultipleChoice || model.Type == QuestionType.TrueFalse) 
                    && string.IsNullOrWhiteSpace(model.CorrectAnswer))
                {
                    ModelState.AddModelError("CorrectAnswer", "Vui lòng chọn đáp án đúng");
                }

                // Custom validation: Options required for MultipleChoice
                if (model.Type == QuestionType.MultipleChoice)
                {
                    var validOptions = model.Options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
                    if (validOptions.Count < 2)
                    {
                        ModelState.AddModelError("Options", "Câu hỏi trắc nghiệm phải có ít nhất 2 đáp án");
                    }
                }
                
                if (!ModelState.IsValid)
                {
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }

                // Get teacher ID from session
                var teacherId = HttpContext.Session.GetString("ApiId");
                if (string.IsNullOrEmpty(teacherId))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin giảng viên. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Home");
                }

                // Map ViewModel to Question
                var question = new Question
                {
                    SubjectId = model.SubjectId,
                    Content = model.Content,
                    Type = model.Type,
                    Options = model.Options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList(),
                    CorrectAnswer = model.CorrectAnswer,
                    Score = model.Score,
                    DifficultyLevel = model.DifficultyLevel,
                    CreatedBy = teacherId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Create question via API
                var createdQuestion = await _questionApiService.CreateQuestionAsync(question);

                if (createdQuestion != null)
                {
                    TempData["SuccessMessage"] = "Câu hỏi đã được tạo thành công!";
                    return RedirectToAction("QuestionBank");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo câu hỏi. Vui lòng thử lại.");
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating question: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo câu hỏi.");
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

                // Get question by ID
                var question = await _questionApiService.GetQuestionByIdAsync(id);

                if (question == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi.";
                    return RedirectToAction("QuestionBank");
                }

                // Map Question to EditQuestionViewModel
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

                // Get subjects for dropdown
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit question form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin câu hỏi.";
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
                // Remove Id validation (we only need ApiId)
                ModelState.Remove("Id");

                if (!ModelState.IsValid)
                {
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }

                // Map ViewModel to Question
                var question = new Question
                {
                    ApiId = model.ApiId,
                    SubjectId = model.SubjectId,
                    Content = model.Content,
                    Type = model.Type,
                    Options = model.Options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList(),
                    CorrectAnswer = model.CorrectAnswer,
                    Score = model.Score,
                    DifficultyLevel = model.DifficultyLevel,
                    UpdatedAt = DateTime.Now
                };

                // Update question via API
                var success = await _questionApiService.UpdateQuestionAsync(question);

                if (success)
                {
                    TempData["SuccessMessage"] = "Câu hỏi đã được cập nhật thành công!";
                    return RedirectToAction("QuestionBank");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật câu hỏi. Vui lòng thử lại.");
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating question: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật câu hỏi.");
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

        public async Task<IActionResult> ViewResults()
        {
            try
            {
                var teacherId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(teacherId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get all subjects taught by this teacher
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                
                // Get all exams created by this teacher
                var exams = await _examApiService.GetAllExamsAsync();
                
                // Get all submissions
                var allSubmissions = await _submissionApiService.GetAllSubmissionsAsync();

                // Calculate results by subject
                var subjectResults = new List<SubjectResultViewModel>();
                
                foreach (var subject in subjects)
                {
                    // Get exams for this subject
                    var subjectExams = exams.Where(e => e.SubjectId == subject.Id).ToList();
                    
                    if (subjectExams.Any())
                    {
                        // Get submissions for these exams
                        var examIds = subjectExams.Select(e => e.ApiId).ToList();
                        var subjectSubmissions = allSubmissions
                            .Where(s => examIds.Contains(s.ExamId) && s.IsGraded)
                            .ToList();

                        if (subjectSubmissions.Any())
                        {
                            var avgScore = subjectSubmissions.Average(s => s.Score);
                            
                            subjectResults.Add(new SubjectResultViewModel
                            {
                                SubjectCode = subject.Code,
                                SubjectName = subject.Name,
                                ExamCount = subjectExams.Count,
                                AverageScore = Math.Round(avgScore, 1),
                                SubmissionCount = subjectSubmissions.Count
                            });
                        }
                    }
                }

                // Calculate summary statistics
                ViewBag.TotalSubjects = subjectResults.Count;
                ViewBag.TotalExams = subjectResults.Sum(s => s.ExamCount);
                ViewBag.SubjectResults = subjectResults;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading view results");
                TempData["ErrorMessage"] = "Không thể tải kết quả thi. Vui lòng thử lại.";
                return RedirectToAction("Dashboard");
            }
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

        public async Task<IActionResult> ViewStudentExam(string studentId, string examId)
        {
            try
            {
                // Get submission
                var submission = await _submissionApiService.GetExamSubmissionAsync(examId, studentId);
                if (submission == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bài thi của học sinh này.";
                    return RedirectToAction("ViewResults");
                }

                // Get exam details
                var exam = await _examApiService.GetExamByIdAsync(examId);
                if (exam == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đề thi.";
                    return RedirectToAction("ViewResults");
                }

                // Get subject
                var subject = await _subjectApiService.GetSubjectByIdAsync(exam.SubjectId);

                // Get student info (from User API)
                var student = await _userApiService.GetUserByIdAsync(studentId);
                
                // Get questions
                var allQuestions = await _questionApiService.GetAllQuestionsAsync();
                var questions = allQuestions.Where(q => exam.QuestionIds.Contains(q.ApiId ?? "")).ToList();

                // Calculate statistics
                var correctAnswers = 0;
                var incorrectAnswers = 0;
                var questionResults = new List<QuestionResult>();

                foreach (var question in questions)
                {
                    var studentAnswer = submission.Answers.FirstOrDefault(a => a.QuestionId == question.ApiId);
                    var isCorrect = studentAnswer?.SelectedOption == question.CorrectAnswer;
                    
                    if (isCorrect) correctAnswers++;
                    else incorrectAnswers++;

                    questionResults.Add(new QuestionResult
                    {
                        QuestionId = question.ApiId ?? "",
                        Content = question.Content,
                        Options = question.Options,
                        CorrectAnswer = question.CorrectAnswer ?? "",
                        StudentAnswer = studentAnswer?.SelectedOption ?? "",
                        IsCorrect = isCorrect,
                        Score = isCorrect ? (10.0 / questions.Count) : 0
                    });
                }

                var accuracy = questions.Count > 0 ? Math.Round((double)correctAnswers / questions.Count * 100, 1) : 0;
                var timeSpent = submission.SubmittedAt.HasValue && submission.CreatedAt != default 
                    ? Math.Round((submission.SubmittedAt.Value - submission.CreatedAt).TotalMinutes, 0) 
                    : 0;

                // Create view model
                var viewModel = new StudentExamViewModel
                {
                    StudentId = studentId,
                    StudentName = student?.FullName ?? "Unknown Student",
                    ExamName = exam.Title,
                    SubjectName = subject?.Name ?? "Unknown Subject",
                    ExamDate = submission.CreatedAt,
                    SubmittedAt = submission.SubmittedAt,
                    Duration = exam.Duration,
                    TotalQuestions = questions.Count,
                    CorrectAnswers = correctAnswers,
                    IncorrectAnswers = incorrectAnswers,
                    Score = submission.Score,
                    MaxScore = 10.0,
                    AccuracyPercentage = accuracy,
                    TimeSpentMinutes = timeSpent,
                    Questions = questionResults
                };

                ViewBag.StudentInfo = new { StudentId = viewModel.StudentId, StudentName = viewModel.StudentName };
                ViewBag.ExamInfo = new 
                { 
                    Subject = viewModel.SubjectName,
                    ExamDate = viewModel.ExamDate,
                    SubmittedAt = viewModel.SubmittedAt,
                    TotalQuestions = viewModel.TotalQuestions,
                    CorrectAnswers = viewModel.CorrectAnswers,
                    IncorrectAnswers = viewModel.IncorrectAnswers,
                    Score = viewModel.Score,
                    MaxScore = viewModel.MaxScore
                };
                ViewBag.TimeSpent = viewModel.TimeSpentMinutes;
                ViewBag.Accuracy = viewModel.AccuracyPercentage;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error viewing student exam: StudentId={studentId}, ExamId={examId}");
                TempData["ErrorMessage"] = "Không thể tải chi tiết bài thi. Vui lòng thử lại.";
                return RedirectToAction("ViewResults");
            }
        }

        #region Exam Management

        /// <summary>
        /// Hiển thị danh sách đề thi
        /// </summary>
        public async Task<IActionResult> ExamManagement(string? subjectId = null, bool? isPublished = null)
        {
            try
            {
                // Get all exams
                var exams = await _examApiService.GetAllExamsAsync();

                // Filter by subject if provided
                if (!string.IsNullOrEmpty(subjectId))
                {
                    exams = exams.Where(e => e.SubjectId == subjectId).ToList();
                }

                // Filter by published status if provided
                if (isPublished.HasValue)
                {
                    exams = exams.Where(e => e.IsPublished == isPublished.Value).ToList();
                }

                // Get subjects for filter dropdown
                var subjects = await _subjectApiService.GetAllSubjectsAsync();

                // Populate SubjectName for each exam
                foreach (var exam in exams)
                {
                    var subject = subjects.FirstOrDefault(s => s.Id == exam.SubjectId);
                    if (subject != null)
                    {
                        exam.SubjectName = subject.Name;
                    }
                }

                // Calculate statistics
                var totalExams = exams.Count;
                var draftExams = exams.Count(e => !e.IsPublished);
                var publishedExams = exams.Count(e => e.IsPublished);
                var lockedExams = exams.Count(e => e.IsLocked);

                ViewBag.Exams = exams;
                ViewBag.Subjects = subjects;
                ViewBag.TotalExams = totalExams;
                ViewBag.DraftExams = draftExams;
                ViewBag.PublishedExams = publishedExams;
                ViewBag.LockedExams = lockedExams;
                ViewBag.SelectedSubject = subjectId;
                ViewBag.SelectedIsPublished = isPublished;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading exam management: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách đề thi.";
                return View();
            }
        }

        /// <summary>
        /// GET: Hiển thị form tạo đề thi
        /// </summary>
        public async Task<IActionResult> CreateExam()
        {
            try
            {
                // Get subjects for dropdown
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create exam form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo đề thi.";
                return RedirectToAction("ExamManagement");
            }
        }

        /// <summary>
        /// POST: Tạo đề thi mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(CreateExamViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }

                // Create exam object from ViewModel
                var exam = new Exam
                {
                    Title = model.Title,
                    Description = model.Description,
                    SubjectId = model.SubjectId,
                    Duration = model.Duration,
                    MaxAttempts = model.MaxAttempts,
                    PassingScore = model.PassingScore,
                    TeacherId = HttpContext.Session.GetString("ApiId") ?? "", // MongoDB ObjectId
                    IsPublished = false
                };

                // Create exam
                var createdExam = await _examApiService.CreateExamAsync(exam);

                if (createdExam != null)
                {
                    TempData["SuccessMessage"] = "Đề thi đã được tạo thành công!";
                    return RedirectToAction("ExamManagement");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo đề thi. Vui lòng thử lại.");
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating exam: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo đề thi.");
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;
                return View(model);
            }
        }

        /// <summary>
        /// GET: Hiển thị form chỉnh sửa đề thi
        /// </summary>
        public async Task<IActionResult> EditExam(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "ID đề thi không hợp lệ.";
                    return RedirectToAction("ExamManagement");
                }

                // Get exam by ID
                var exam = await _examApiService.GetExamByIdAsync(id);

                if (exam == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đề thi.";
                    return RedirectToAction("ExamManagement");
                }

                // Map Exam to EditExamViewModel
                var model = new EditExamViewModel
                {
                    Id = exam.Id,
                    ApiId = exam.ApiId,
                    Title = exam.Title,
                    Description = exam.Description,
                    SubjectId = exam.SubjectId,
                    Duration = exam.Duration,
                    MaxAttempts = exam.MaxAttempts,
                    PassingScore = exam.PassingScore,
                    IsPublished = exam.IsPublished,
                    IsLocked = exam.IsLocked
                };

                // Get subjects for dropdown
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit exam form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa.";
                return RedirectToAction("ExamManagement");
            }
        }

        /// <summary>
        /// POST: Cập nhật đề thi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExam(EditExamViewModel model)
        {
            try
            {
                _logger.LogInformation($"EditExam POST called - ApiId: {model.ApiId}, Title: {model.Title}");
                
                // Remove Id validation since we use ApiId for updates
                ModelState.Remove("Id");
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning($"  - {error.ErrorMessage}");
                    }
                    
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }

                // Create Exam object from ViewModel
                var exam = new Exam
                {
                    Id = model.Id,
                    ApiId = model.ApiId,
                    Title = model.Title,
                    Description = model.Description,
                    SubjectId = model.SubjectId,
                    Duration = model.Duration,
                    MaxAttempts = model.MaxAttempts,
                    PassingScore = model.PassingScore,
                    IsPublished = model.IsPublished,
                    IsLocked = model.IsLocked,
                    TeacherId = HttpContext.Session.GetString("ApiId") ?? "" // MongoDB ObjectId
                };

                // Update exam
                var success = await _examApiService.UpdateExamAsync(exam);

                if (success)
                {
                    TempData["SuccessMessage"] = "Đề thi đã được cập nhật thành công!";
                    return RedirectToAction("ExamManagement");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật đề thi. Vui lòng thử lại.");
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating exam: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật đề thi.");
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects;
                return View(model);
            }
        }

        /// <summary>
        /// POST: Xóa đề thi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExam(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID đề thi không hợp lệ." });
                }

                var success = await _examApiService.DeleteExamAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Đề thi đã được xóa thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa đề thi. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting exam {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa đề thi." });
            }
        }

        /// <summary>
        /// POST: Xuất bản đề thi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishExam(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID đề thi không hợp lệ." });
                }

                var success = await _examApiService.PublishExamAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Đề thi đã được xuất bản thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xuất bản đề thi. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing exam {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xuất bản đề thi." });
            }
        }

        /// <summary>
        /// GET: Exam Builder - Thêm câu hỏi vào đề thi
        /// </summary>
        public async Task<IActionResult> ExamBuilder(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "ID đề thi không hợp lệ.";
                    return RedirectToAction("ExamManagement");
                }

                // Get exam details
                var exam = await _examApiService.GetExamByIdAsync(id);
                if (exam == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đề thi.";
                    return RedirectToAction("ExamManagement");
                }

                // Get all questions from question bank
                var allQuestions = await _questionApiService.GetAllQuestionsAsync();
                
                // Filter questions by same subject as exam
                var subjectQuestions = allQuestions
                    .Where(q => q.SubjectId == exam.SubjectId)
                    .OrderBy(q => q.CreatedAt)
                    .ToList();

                // Get subjects for display
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                var subject = subjects.FirstOrDefault(s => s.Id == exam.SubjectId);

                ViewBag.Exam = exam;
                ViewBag.SubjectName = subject?.Name ?? "N/A";
                ViewBag.SelectedQuestionIds = exam.QuestionIds ?? new List<string>();

                return View(subjectQuestions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading exam builder for exam {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải trang thêm câu hỏi.";
                return RedirectToAction("ExamManagement");
            }
        }

        /// <summary>
        /// POST: Lưu câu hỏi đã chọn vào đề thi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveExamQuestions(string examId, List<string> questionIds)
        {
            try
            {
                _logger.LogInformation($"SaveExamQuestions called - ExamId: {examId}, QuestionIds count: {questionIds?.Count ?? 0}");

                if (string.IsNullOrEmpty(examId))
                {
                    return Json(new { success = false, message = "ID đề thi không hợp lệ." });
                }

                if (questionIds == null || questionIds.Count == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 câu hỏi." });
                }

                // Get exam to update
                var exam = await _examApiService.GetExamByIdAsync(examId);
                if (exam == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đề thi." });
                }

                // Update question IDs
                exam.QuestionIds = questionIds;

                // Calculate total score from selected questions
                var allQuestions = await _questionApiService.GetAllQuestionsAsync();
                var selectedQuestions = allQuestions.Where(q => !string.IsNullOrEmpty(q.ApiId) && questionIds.Contains(q.ApiId)).ToList();
                var totalScore = selectedQuestions.Sum(q => q.Score);

                exam.TotalScore = totalScore;

                // Update exam
                var success = await _examApiService.UpdateExamAsync(exam);

                if (success)
                {
                    _logger.LogInformation($"Successfully updated exam {examId} with {questionIds.Count} questions, total score: {totalScore}");
                    return Json(new { 
                        success = true, 
                        message = $"Đã lưu {questionIds.Count} câu hỏi vào đề thi. Tổng điểm: {totalScore}",
                        totalScore = totalScore,
                        questionCount = questionIds.Count
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật đề thi. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving exam questions: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu câu hỏi." });
            }
        }

        #endregion

        #region Exam Assignment (Schedule)

        /// <summary>
        /// GET: Gán đề thi cho lớp học
        /// </summary>
        public async Task<IActionResult> AssignExam(string? examId = null)
        {
            try
            {
                // Get all published exams
                var allExams = await _examApiService.GetAllExamsAsync();
                var publishedExams = allExams.Where(e => e.IsPublished).ToList();

                // Get all classes
                var classes = await _classApiService.GetAllClassesAsync();

                // Get subjects for display
                var subjects = await _subjectApiService.GetAllSubjectsAsync();

                ViewBag.Exams = publishedExams;
                ViewBag.Classes = classes;
                ViewBag.Subjects = subjects;

                // If examId provided, pre-select it
                if (!string.IsNullOrEmpty(examId))
                {
                    ViewBag.SelectedExamId = examId;
                }

                return View(new AssignExamViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading assign exam page: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải trang gán đề thi.";
                return RedirectToAction("ExamManagement");
            }
        }

        /// <summary>
        /// POST: Lưu phân công đề thi cho lớp học
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignExam(AssignExamViewModel model)
        {
            try
            {
                ModelState.Remove("ExamTitle");
                ModelState.Remove("ClassName");

                if (!ModelState.IsValid)
                {
                    var allExams = await _examApiService.GetAllExamsAsync();
                    var publishedExams = allExams.Where(e => e.IsPublished).ToList();
                    var classes = await _classApiService.GetAllClassesAsync();
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();

                    ViewBag.Exams = publishedExams;
                    ViewBag.Classes = classes;
                    ViewBag.Subjects = subjects;

                    return View(model);
                }

                // Combine date and time
                var startTime = model.StartDate.Date + model.StartTimeOfDay;
                var endTime = model.EndDate.Date + model.EndTimeOfDay;

                // Validate times
                if (endTime <= startTime)
                {
                    ModelState.AddModelError("", "Thời gian kết thúc phải sau thời gian bắt đầu.");
                    
                    var allExams = await _examApiService.GetAllExamsAsync();
                    var publishedExams = allExams.Where(e => e.IsPublished).ToList();
                    var classes = await _classApiService.GetAllClassesAsync();
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();

                    ViewBag.Exams = publishedExams;
                    ViewBag.Classes = classes;
                    ViewBag.Subjects = subjects;

                    return View(model);
                }

                // Create exam schedule
                var schedule = new ExamSchedule
                {
                    ExamId = model.ExamId,
                    ClassId = model.ClassId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsClosed = false
                };

                var success = await _examScheduleApiService.CreateScheduleAsync(schedule);

                if (success)
                {
                    TempData["SuccessMessage"] = "Đã gán đề thi cho lớp học thành công!";
                    return RedirectToAction("ExamAssignments");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể gán đề thi. Vui lòng thử lại.");
                    
                    var allExams = await _examApiService.GetAllExamsAsync();
                    var publishedExams = allExams.Where(e => e.IsPublished).ToList();
                    var classes = await _classApiService.GetAllClassesAsync();
                    var subjects = await _subjectApiService.GetAllSubjectsAsync();

                    ViewBag.Exams = publishedExams;
                    ViewBag.Classes = classes;
                    ViewBag.Subjects = subjects;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning exam: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gán đề thi.";
                return RedirectToAction("ExamManagement");
            }
        }

        /// <summary>
        /// GET: Danh sách phân công đề thi
        /// </summary>
        public async Task<IActionResult> ExamAssignments()
        {
            try
            {
                // Get all schedules
                var schedules = await _examScheduleApiService.GetAllSchedulesAsync();

                // Get all exams, classes, subjects for display
                var exams = await _examApiService.GetAllExamsAsync();
                var classes = await _classApiService.GetAllClassesAsync();
                var subjects = await _subjectApiService.GetAllSubjectsAsync();

                // Populate navigation properties
                foreach (var schedule in schedules)
                {
                    schedule.Exam = exams.FirstOrDefault(e => e.ApiId == schedule.ExamId);
                    schedule.Class = classes.FirstOrDefault(c => c.Id == schedule.ClassId);
                    
                    if (schedule.Exam != null)
                    {
                        var subject = subjects.FirstOrDefault(s => s.Id == schedule.Exam.SubjectId);
                        schedule.Exam.SubjectName = subject?.Name;
                    }
                }

                return View(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading exam assignments: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách phân công đề thi.";
                return View(new List<ExamSchedule>());
            }
        }

        /// <summary>
        /// POST: Xóa phân công đề thi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignment(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID phân công không hợp lệ." });
                }

                var success = await _examScheduleApiService.DeleteScheduleAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Đã xóa phân công đề thi thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa phân công. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting assignment {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa phân công." });
            }
        }

        /// <summary>
        /// POST: Đóng phân công đề thi (không cho làm bài nữa)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseAssignment(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID phân công không hợp lệ." });
                }

                var success = await _examScheduleApiService.CloseScheduleAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Đã đóng phân công đề thi!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể đóng phân công. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error closing assignment {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi đóng phân công." });
            }
        }

        #endregion

        #region Import/Export Questions

        /// <summary>
        /// Tải file template Excel mẫu để import câu hỏi
        /// </summary>
        [HttpGet]
        public IActionResult DownloadQuestionTemplate()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                    {
                        // Tạo workbook
                        var workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        // Tạo worksheet
                        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        // Tạo sheets
                        var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        var sheet = new Sheet()
                        {
                            Id = workbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Questions"
                        };
                        sheets.Append(sheet);

                        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        if (sheetData == null)
                        {
                            sheetData = new SheetData();
                            worksheetPart.Worksheet.Append(sheetData);
                        }

                        // Tạo header row
                        var headerRow = new Row() { RowIndex = 1 };
                        headerRow.Append(
                            CreateCell("A", 1, "Question Text", CellValues.String),
                            CreateCell("B", 1, "Option A", CellValues.String),
                            CreateCell("C", 1, "Option B", CellValues.String),
                            CreateCell("D", 1, "Option C", CellValues.String),
                            CreateCell("E", 1, "Option D", CellValues.String),
                            CreateCell("F", 1, "Correct Answer (A/B/C/D)", CellValues.String),
                            CreateCell("G", 1, "Subject Code", CellValues.String),
                            CreateCell("H", 1, "Difficulty (Easy/Medium/Hard)", CellValues.String),
                            CreateCell("I", 1, "Chapter", CellValues.String),
                            CreateCell("J", 1, "Points", CellValues.String)
                        );
                        sheetData.Append(headerRow);

                        // Thêm 1 dòng mẫu
                        var sampleRow = new Row() { RowIndex = 2 };
                        sampleRow.Append(
                            CreateCell("A", 2, "Câu hỏi mẫu: ASP.NET Core là gì?", CellValues.String),
                            CreateCell("B", 2, "Một framework web", CellValues.String),
                            CreateCell("C", 2, "Một ngôn ngữ lập trình", CellValues.String),
                            CreateCell("D", 2, "Một database", CellValues.String),
                            CreateCell("E", 2, "Một IDE", CellValues.String),
                            CreateCell("F", 2, "A", CellValues.String),
                            CreateCell("G", 2, "CNPM", CellValues.String),
                            CreateCell("H", 2, "Easy", CellValues.String),
                            CreateCell("I", 2, "1", CellValues.String),
                            CreateCell("J", 2, "10", CellValues.String)
                        );
                        sheetData.Append(sampleRow);

                        workbookPart.Workbook.Save();
                    }

                    var content = memoryStream.ToArray();
                    var fileName = $"QuestionTemplate_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating template: {ex.Message}");
                TempData["Error"] = "Không thể tạo file template. Vui lòng thử lại.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// Helper method để tạo cell trong Excel
        /// </summary>
        private Cell CreateCell(string columnName, uint rowIndex, string text, CellValues dataType)
        {
            return new Cell()
            {
                CellReference = columnName + rowIndex,
                DataType = dataType,
                CellValue = new CellValue(text)
            };
        }

        /// <summary>
        /// Tải file template PDF mẫu để import câu hỏi
        /// </summary>
        [HttpGet]
        public IActionResult DownloadQuestionTemplatePdf()
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(QuestPDF.Helpers.Colors.White);

                        page.Content().Column(column =>
                        {
                            // Title
                            column.Item().AlignCenter().Text("TEMPLATE IMPORT CÂU HỎI")
                                .FontSize(18).Bold();

                            column.Item().PaddingVertical(10);

                            // Instructions
                            column.Item().Text("Hướng dẫn:")
                                .FontSize(12).Bold();
                            column.Item().Text("• Mỗi câu hỏi bắt đầu bằng 'Câu X:' (X là số thứ tự)")
                                .FontSize(10);
                            column.Item().Text("• Các đáp án bắt đầu bằng A., B., C., D.")
                                .FontSize(10);
                            column.Item().Text("• Metadata nằm trong ngoặc vuông [Đáp án: ... | Môn: ... | Độ khó: ... | Điểm: ...]")
                                .FontSize(10);

                            column.Item().PaddingVertical(10);

                            // Sample Question 1
                            column.Item().Text("Câu 1: JavaScript là ngôn ngữ gì?")
                                .FontSize(11).Bold();
                            column.Item().Text("A. Ngôn ngữ biên dịch")
                                .FontSize(10);
                            column.Item().Text("B. Ngôn ngữ thông dịch")
                                .FontSize(10);
                            column.Item().Text("C. Ngôn ngữ máy")
                                .FontSize(10);
                            column.Item().Text("D. Ngôn ngữ assembly")
                                .FontSize(10);
                            column.Item().Text("[Đáp án: B | Môn: CNPMNC | Độ khó: Easy | Điểm: 1]")
                                .FontSize(9).Italic();

                            column.Item().PaddingVertical(10);

                            // Sample Question 2
                            column.Item().Text("Câu 2: Database nào phù hợp nhất cho dữ liệu time series?")
                                .FontSize(11).Bold();
                            column.Item().Text("A. MongoDB")
                                .FontSize(10);
                            column.Item().Text("B. InfluxDB")
                                .FontSize(10);
                            column.Item().Text("C. PostgreSQL")
                                .FontSize(10);
                            column.Item().Text("D. Redis")
                                .FontSize(10);
                            column.Item().Text("[Đáp án: B | Môn: CNPMNC | Độ khó: Medium | Điểm: 2]")
                                .FontSize(9).Italic();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                var fileName = $"QuestionTemplate_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating PDF template: {ex.Message}");
                TempData["Error"] = "Không thể tạo file template PDF. Vui lòng thử lại.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// Tải file template Word mẫu để import câu hỏi
        /// </summary>
        [HttpGet]
        public IActionResult DownloadQuestionTemplateWord()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                    {
                        // Tạo main document part
                        var mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Word.Document();
                        var body = new Word.Body();

                        // Tiêu đề
                        var titleParagraph = new Word.Paragraph(
                            new Word.ParagraphProperties(
                                new Word.Justification() { Val = Word.JustificationValues.Center },
                                new Word.SpacingBetweenLines() { After = "240" }
                            ),
                            new Word.Run(
                                new Word.RunProperties(
                                    new Word.Bold(),
                                    new Word.FontSize() { Val = "32" }
                                ),
                                new Word.Text("TEMPLATE IMPORT CÂU HỎI")
                            )
                        );
                        body.Append(titleParagraph);

                        // Hướng dẫn
                        var instructionParagraph = new Word.Paragraph(
                            new Word.ParagraphProperties(
                                new Word.SpacingBetweenLines() { After = "240" }
                            ),
                            new Word.Run(
                                new Word.RunProperties(
                                    new Word.Italic(),
                                    new Word.FontSize() { Val = "20" }
                                ),
                                new Word.Text("Hướng dẫn: Mỗi câu hỏi được định dạng như mẫu bên dưới. Vui lòng tuân thủ format để import thành công.")
                            )
                        );
                        body.Append(instructionParagraph);

                        // Đường kẻ phân cách
                        body.Append(new Word.Paragraph(new Word.Run(new Word.Text("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"))));

                        // Câu hỏi mẫu
                        AddSampleQuestion(body, 1, "ASP.NET Core là gì?",
                            "A. Một framework web",
                            "B. Một ngôn ngữ lập trình",
                            "C. Một database",
                            "D. Một IDE",
                            "A",
                            "CNPMNC",
                            "Easy",
                            "1",
                            "10");

                        body.Append(new Word.Paragraph(new Word.Run(new Word.Text("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"))));

                        AddSampleQuestion(body, 2, "MongoDB là loại database gì?",
                            "A. Relational Database",
                            "B. NoSQL Database",
                            "C. Graph Database",
                            "D. Time Series Database",
                            "B",
                            "CNPMNC",
                            "Medium",
                            "2",
                            "15");

                        mainPart.Document.Append(body);
                        mainPart.Document.Save();
                    }

                    var content = memoryStream.ToArray();
                    var fileName = $"QuestionTemplate_{DateTime.Now:yyyyMMddHHmmss}.docx";
                    return File(content, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating Word template: {ex.Message}");
                TempData["Error"] = "Không thể tạo file template Word. Vui lòng thử lại.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// Helper method để thêm câu hỏi mẫu vào Word document
        /// </summary>
        private void AddSampleQuestion(Word.Body body, int questionNumber, string questionText,
            string optionA, string optionB, string optionC, string optionD,
            string correctAnswer, string subjectCode, string difficulty, string chapter, string points)
        {
            // Số câu hỏi
            body.Append(new Word.Paragraph(
                new Word.ParagraphProperties(new Word.SpacingBetweenLines() { After = "120" }),
                new Word.Run(
                    new Word.RunProperties(new Word.Bold(), new Word.FontSize() { Val = "24" }),
                    new Word.Text($"Câu {questionNumber}:")
                )
            ));

            // Nội dung câu hỏi
            body.Append(new Word.Paragraph(
                new Word.ParagraphProperties(new Word.SpacingBetweenLines() { After = "120" }),
                new Word.Run(new Word.Text(questionText))
            ));

            // Các đáp án
            body.Append(new Word.Paragraph(new Word.Run(new Word.Text(optionA))));
            body.Append(new Word.Paragraph(new Word.Run(new Word.Text(optionB))));
            body.Append(new Word.Paragraph(new Word.Run(new Word.Text(optionC))));
            body.Append(new Word.Paragraph(new Word.Run(new Word.Text(optionD))));

            // Metadata
            body.Append(new Word.Paragraph(
                new Word.ParagraphProperties(new Word.SpacingBetweenLines() { Before = "120", After = "120" }),
                new Word.Run(
                    new Word.RunProperties(new Word.Italic(), new Word.Color() { Val = "666666" }),
                    new Word.Text($"[Đáp án: {correctAnswer} | Môn: {subjectCode} | Độ khó: {difficulty} | Chương: {chapter} | Điểm: {points}]")
                )
            ));
        }

        /// <summary>
        /// Xuất câu hỏi ra file Excel
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportQuestionsExcel(string? subjectCode = null)
        {
            try
            {
                // Lấy danh sách câu hỏi từ API
                var questions = await _questionApiService.GetAllQuestionsAsync();
                
                // Lọc theo môn học nếu có
                if (!string.IsNullOrEmpty(subjectCode))
                {
                    questions = questions.Where(q => q.SubjectId == subjectCode).ToList();
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                    {
                        var workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        var sheet = new Sheet()
                        {
                            Id = workbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Questions"
                        };
                        sheets.Append(sheet);

                        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        if (sheetData == null)
                        {
                            sheetData = new SheetData();
                            worksheetPart.Worksheet.Append(sheetData);
                        }

                        // Header row
                        var headerRow = new Row() { RowIndex = 1 };
                        headerRow.Append(
                            CreateCell("A", 1, "Question Text", CellValues.String),
                            CreateCell("B", 1, "Option A", CellValues.String),
                            CreateCell("C", 1, "Option B", CellValues.String),
                            CreateCell("D", 1, "Option C", CellValues.String),
                            CreateCell("E", 1, "Option D", CellValues.String),
                            CreateCell("F", 1, "Correct Answer", CellValues.String),
                            CreateCell("G", 1, "Subject Code", CellValues.String),
                            CreateCell("H", 1, "Difficulty", CellValues.String),
                            CreateCell("I", 1, "Chapter", CellValues.String),
                            CreateCell("J", 1, "Points", CellValues.String)
                        );
                        sheetData.Append(headerRow);

                        // Data rows
                        uint rowIndex = 2;
                        foreach (var question in questions)
                        {
                            var dataRow = new Row() { RowIndex = rowIndex };
                            dataRow.Append(
                                CreateCell("A", rowIndex, question.Content ?? "", CellValues.String),
                                CreateCell("B", rowIndex, question.Options.Count > 0 ? question.Options[0] : "", CellValues.String),
                                CreateCell("C", rowIndex, question.Options.Count > 1 ? question.Options[1] : "", CellValues.String),
                                CreateCell("D", rowIndex, question.Options.Count > 2 ? question.Options[2] : "", CellValues.String),
                                CreateCell("E", rowIndex, question.Options.Count > 3 ? question.Options[3] : "", CellValues.String),
                                CreateCell("F", rowIndex, question.CorrectAnswer ?? "", CellValues.String),
                                CreateCell("G", rowIndex, question.SubjectId ?? "", CellValues.String),
                                CreateCell("H", rowIndex, question.DifficultyLevel.ToString(), CellValues.String),
                                CreateCell("I", rowIndex, "", CellValues.String), // Chapter - không có trong model
                                CreateCell("J", rowIndex, question.Score.ToString(), CellValues.String)
                            );
                            sheetData.Append(dataRow);
                            rowIndex++;
                        }

                        workbookPart.Workbook.Save();
                    }

                    var content = memoryStream.ToArray();
                    var fileName = string.IsNullOrEmpty(subjectCode) 
                        ? $"AllQuestions_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                        : $"Questions_{subjectCode}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting questions to Excel: {ex.Message}");
                TempData["Error"] = "Không thể xuất file Excel. Vui lòng thử lại.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// Xuất câu hỏi ra file Word
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportQuestionsWord(string? subjectCode = null)
        {
            try
            {
                // Lấy danh sách câu hỏi từ API
                var questions = await _questionApiService.GetAllQuestionsAsync();
                
                // Lọc theo môn học nếu có
                if (!string.IsNullOrEmpty(subjectCode))
                {
                    questions = questions.Where(q => q.SubjectId == subjectCode).ToList();
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                    {
                        var mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Word.Document();
                        var body = new Word.Body();

                        // Tiêu đề
                        var titleParagraph = new Word.Paragraph(
                            new Word.ParagraphProperties(
                                new Word.Justification() { Val = Word.JustificationValues.Center },
                                new Word.SpacingBetweenLines() { After = "400" }
                            ),
                            new Word.Run(
                                new Word.RunProperties(
                                    new Word.Bold(),
                                    new Word.FontSize() { Val = "36" }
                                ),
                                new Word.Text("NGÂN HÀNG CÂU HỎI")
                            )
                        );
                        body.Append(titleParagraph);

                        // Thông tin
                        var infoParagraph = new Word.Paragraph(
                            new Word.ParagraphProperties(
                                new Word.Justification() { Val = Word.JustificationValues.Center },
                                new Word.SpacingBetweenLines() { After = "400" }
                            ),
                            new Word.Run(
                                new Word.RunProperties(
                                    new Word.Italic(),
                                    new Word.FontSize() { Val = "20" }
                                ),
                                new Word.Text($"Tổng số câu hỏi: {questions.Count} | Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            )
                        );
                        body.Append(infoParagraph);

                        body.Append(new Word.Paragraph(new Word.Run(new Word.Text("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"))));

                        // Thêm từng câu hỏi
                        int questionNumber = 1;
                        foreach (var question in questions)
                        {
                            // Số câu hỏi
                            body.Append(new Word.Paragraph(
                                new Word.ParagraphProperties(
                                    new Word.SpacingBetweenLines() { Before = "240", After = "120" }
                                ),
                                new Word.Run(
                                    new Word.RunProperties(new Word.Bold(), new Word.FontSize() { Val = "24" }),
                                    new Word.Text($"Câu {questionNumber}:")
                                )
                            ));

                            // Nội dung câu hỏi
                            body.Append(new Word.Paragraph(
                                new Word.ParagraphProperties(new Word.SpacingBetweenLines() { After = "120" }),
                                new Word.Run(new Word.Text(question.Content ?? ""))
                            ));

                            // Các đáp án
                            if (question.Options != null && question.Options.Count > 0)
                            {
                                for (int i = 0; i < question.Options.Count; i++)
                                {
                                    var optionLetter = ((char)('A' + i)).ToString();
                                    var isCorrect = question.CorrectAnswer == optionLetter;
                                    
                                    var optionRun = new Word.Run(new Word.Text($"{optionLetter}. {question.Options[i]}"));
                                    if (isCorrect)
                                    {
                                        optionRun.RunProperties = new Word.RunProperties(
                                            new Word.Bold(),
                                            new Word.Color() { Val = "217346" }
                                        );
                                    }
                                    
                                    body.Append(new Word.Paragraph(optionRun));
                                }
                            }

                            // Metadata
                            body.Append(new Word.Paragraph(
                                new Word.ParagraphProperties(
                                    new Word.SpacingBetweenLines() { Before = "120", After = "240" }
                                ),
                                new Word.Run(
                                    new Word.RunProperties(
                                        new Word.Italic(),
                                        new Word.FontSize() { Val = "18" },
                                        new Word.Color() { Val = "666666" }
                                    ),
                                    new Word.Text($"[Đáp án: {question.CorrectAnswer} | Môn: {question.SubjectId} | Độ khó: {question.DifficultyLevel} | Điểm: {question.Score}]")
                                )
                            ));

                            body.Append(new Word.Paragraph(new Word.Run(new Word.Text("────────────────────────────────────────────────────────────────────────"))));
                            
                            questionNumber++;
                        }

                        mainPart.Document.Append(body);
                        mainPart.Document.Save();
                    }

                    var content = memoryStream.ToArray();
                    var fileName = string.IsNullOrEmpty(subjectCode) 
                        ? $"AllQuestions_{DateTime.Now:yyyyMMddHHmmss}.docx"
                        : $"Questions_{subjectCode}_{DateTime.Now:yyyyMMddHHmmss}.docx";
                    
                    return File(content, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting questions to Word: {ex.Message}");
                TempData["Error"] = "Không thể xuất file Word. Vui lòng thử lại.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// Xuất câu hỏi ra file PDF
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportQuestionsPdf(string? subjectCode = null)
        {
            try
            {
                // Lấy danh sách câu hỏi từ API
                var questions = await _questionApiService.GetAllQuestionsAsync();
                
                // Lọc theo môn học nếu có
                if (!string.IsNullOrEmpty(subjectCode))
                {
                    questions = questions.Where(q => q.SubjectId == subjectCode).ToList();
                }

                using (var memoryStream = new MemoryStream())
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(2, Unit.Centimetre);
                            page.DefaultTextStyle(x => x.FontSize(11));

                            // Header
                            page.Header().Column(column =>
                            {
                                column.Item().AlignCenter().Text("NGÂN HÀNG CÂU HỎI")
                                    .Bold().FontSize(20);
                                column.Item().AlignCenter().Text($"Tổng số câu hỏi: {questions.Count} | Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .Italic().FontSize(10);
                                column.Item().PaddingVertical(10).LineHorizontal(1);
                            });

                            // Content
                            page.Content().Column(column =>
                            {
                                int questionNumber = 1;
                                foreach (var question in questions)
                                {
                                    column.Item().PaddingTop(10).Column(questionColumn =>
                                    {
                                        // Số câu hỏi
                                        questionColumn.Item().Text($"Câu {questionNumber}:")
                                            .Bold().FontSize(12);

                                        // Nội dung câu hỏi
                                        questionColumn.Item().PaddingTop(5).Text(question.Content ?? "")
                                            .FontSize(11);

                                        // Các đáp án
                                        if (question.Options != null && question.Options.Count > 0)
                                        {
                                            questionColumn.Item().PaddingTop(5).Column(optionsColumn =>
                                            {
                                                for (int i = 0; i < question.Options.Count; i++)
                                                {
                                                    var optionLetter = ((char)('A' + i)).ToString();
                                                    var isCorrect = question.CorrectAnswer == optionLetter;
                                                    
                                                    var text = optionsColumn.Item().Text($"{optionLetter}. {question.Options[i]}");
                                                    if (isCorrect)
                                                    {
                                                        text.Bold().FontColor("#217346");
                                                    }
                                                }
                                            });
                                        }

                                        // Metadata
                                        questionColumn.Item().PaddingTop(5).Text(
                                            $"[Đáp án: {question.CorrectAnswer} | Môn: {question.SubjectId} | Độ khó: {question.DifficultyLevel} | Điểm: {question.Score}]"
                                        ).Italic().FontSize(9).FontColor("#666666");

                                        questionColumn.Item().PaddingTop(10).LineHorizontal(0.5f).LineColor("#CCCCCC");
                                    });

                                    questionNumber++;
                                }
                            });

                            // Footer
                            page.Footer().AlignCenter().Text(text =>
                            {
                                text.Span("Trang ");
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                        });
                    });

                    document.GeneratePdf(memoryStream);
                    
                    var content = memoryStream.ToArray();
                    var fileName = string.IsNullOrEmpty(subjectCode) 
                        ? $"AllQuestions_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                        : $"Questions_{subjectCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    
                    return File(content, "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting questions to PDF: {ex.Message}");
                TempData["Error"] = "Không thể xuất file PDF. Vui lòng thử lại.";
                return RedirectToAction("QuestionBank");
            }
        }

        /// <summary>
        /// Import câu hỏi từ file Excel/Word và hiển thị preview
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ImportQuestions(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file để import." });
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".docx" && extension != ".pdf")
            {
                return Json(new { success = false, message = "Chỉ chấp nhận file .xlsx, .docx hoặc .pdf" });
            }

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "File không được vượt quá 5MB." });
            }

            try
            {
                var questions = new List<Question>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    if (extension == ".xlsx")
                    {
                        questions = ParseExcelFile(stream);
                    }
                    else if (extension == ".docx")
                    {
                        questions = ParseWordFile(stream);
                    }
                    else if (extension == ".pdf")
                    {
                        questions = ParsePdfFile(stream);
                    }
                }

                if (questions.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy câu hỏi nào trong file." });
                }

                // Lưu vào Session để Preview (tránh TempData cookie quá lớn)
                HttpContext.Session.SetString("ImportedQuestions", System.Text.Json.JsonSerializer.Serialize(questions));
                
                return Json(new { 
                    success = true, 
                    message = $"Đọc thành công {questions.Count} câu hỏi.",
                    redirectUrl = Url.Action("PreviewImport", "Teacher")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error importing questions: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi khi đọc file: {ex.Message}" });
            }
        }

        /// <summary>
        /// Parse file Excel
        /// </summary>
        private List<Question> ParseExcelFile(Stream stream)
        {
            var questions = new List<Question>();

            using (var document = SpreadsheetDocument.Open(stream, false))
            {
                var workbookPart = document.WorkbookPart;
                if (workbookPart == null) return questions;
                
                var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (worksheetPart == null) return questions;

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                if (sheetData == null) return questions;

                var rows = sheetData.Elements<Row>().ToList();
                
                // Skip header row (row 1)
                for (int i = 1; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var cells = row.Elements<Cell>().ToList();

                    if (cells.Count < 7) continue; // Cần ít nhất 7 cột

                    var question = new Question
                    {
                        Content = GetCellValue(workbookPart, cells, 0), // Column A
                        Options = new List<string>
                        {
                            GetCellValue(workbookPart, cells, 1), // Column B
                            GetCellValue(workbookPart, cells, 2), // Column C
                            GetCellValue(workbookPart, cells, 3), // Column D
                            GetCellValue(workbookPart, cells, 4)  // Column E
                        },
                        CorrectAnswer = GetCellValue(workbookPart, cells, 5), // Column F
                        SubjectId = GetCellValue(workbookPart, cells, 6), // Column G
                        DifficultyLevel = ParseDifficulty(GetCellValue(workbookPart, cells, 7)), // Column H
                        Score = ParseScore(GetCellValue(workbookPart, cells, 9)), // Column J
                        Type = QuestionType.MultipleChoice,
                        CreatedBy = User.FindFirst("UserId")?.Value ?? "",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Validate
                    if (!string.IsNullOrWhiteSpace(question.Content) && 
                        !string.IsNullOrWhiteSpace(question.SubjectId))
                    {
                        questions.Add(question);
                    }
                }
            }

            return questions;
        }

        /// <summary>
        /// Parse file Word
        /// </summary>
        private List<Question> ParseWordFile(Stream stream)
        {
            var questions = new List<Question>();

            using (var document = WordprocessingDocument.Open(stream, false))
            {
                var body = document.MainDocumentPart?.Document.Body;
                if (body == null) return questions;

                var paragraphs = body.Elements<Word.Paragraph>().ToList();
                Question? currentQuestion = null;
                int optionIndex = 0;

                foreach (var paragraph in paragraphs)
                {
                    var text = paragraph.InnerText.Trim();
                    if (string.IsNullOrWhiteSpace(text) || text.StartsWith("━")) continue;

                    // Phát hiện câu hỏi mới (bắt đầu bằng "Câu")
                    if (text.StartsWith("Câu ") && text.Contains(":"))
                    {
                        // Lưu câu hỏi trước đó
                        if (currentQuestion != null && !string.IsNullOrWhiteSpace(currentQuestion.Content))
                        {
                            questions.Add(currentQuestion);
                        }

                        currentQuestion = new Question
                        {
                            Options = new List<string>(),
                            Type = QuestionType.MultipleChoice,
                            CreatedBy = User.FindFirst("UserId")?.Value ?? "",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        optionIndex = 0;
                        continue;
                    }

                    if (currentQuestion == null) continue;

                    // Nội dung câu hỏi (dòng sau "Câu X:")
                    if (string.IsNullOrWhiteSpace(currentQuestion.Content) && !text.StartsWith("["))
                    {
                        currentQuestion.Content = text;
                        continue;
                    }

                    // Đáp án (A. B. C. D.)
                    if (text.Length > 2 && char.IsLetter(text[0]) && text[1] == '.')
                    {
                        if (optionIndex < 4)
                        {
                            currentQuestion.Options.Add(text.Substring(3).Trim());
                            optionIndex++;
                        }
                        continue;
                    }

                    // Metadata [Đáp án: A | Môn: CNPM | ...]
                    if (text.StartsWith("[") && text.EndsWith("]"))
                    {
                        ParseMetadata(currentQuestion, text);
                    }
                }

                // Thêm câu hỏi cuối cùng
                if (currentQuestion != null && !string.IsNullOrWhiteSpace(currentQuestion.Content))
                {
                    questions.Add(currentQuestion);
                }
            }

            return questions;
        }

        /// <summary>
        /// Get cell value từ Excel
        /// </summary>
        private string GetCellValue(WorkbookPart workbookPart, List<Cell> cells, int index)
        {
            if (index >= cells.Count) return string.Empty;

            var cell = cells[index];
            var value = cell.CellValue?.Text ?? string.Empty;

            // Handle shared strings
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var stringTable = workbookPart.SharedStringTablePart?.SharedStringTable;
                if (stringTable != null)
                {
                    value = stringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }

            return value;
        }

        /// <summary>
        /// Parse difficulty từ string
        /// </summary>
        private DifficultyLevel ParseDifficulty(string difficulty)
        {
            return difficulty?.ToLower() switch
            {
                "easy" or "dễ" => DifficultyLevel.Easy,
                "hard" or "khó" => DifficultyLevel.Hard,
                _ => DifficultyLevel.Medium
            };
        }

        /// <summary>
        /// Parse score từ string
        /// </summary>
        private double ParseScore(string score)
        {
            if (double.TryParse(score, out var result))
            {
                return result;
            }
            return 1.0; // Default
        }

        /// <summary>
        /// Parse PDF file và trích xuất câu hỏi
        /// </summary>
        private List<Question> ParsePdfFile(Stream stream)
        {
            var questions = new List<Question>();
            Question? currentQuestion = null;
            int optionIndex = 0;

            try
            {
                using (PdfReader reader = new PdfReader(stream))
                {
                    StringBuilder fullText = new StringBuilder();
                    
                    // Đọc toàn bộ text từ PDF
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        fullText.Append(PdfTextExtractor.GetTextFromPage(reader, page));
                        fullText.Append("\n");
                    }

                    var lines = fullText.ToString().Split('\n');
                    
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                        // Detect question: "Câu 1:", "Câu 2:", etc.
                        if (System.Text.RegularExpressions.Regex.IsMatch(trimmedLine, @"^Câu\s+\d+:"))
                        {
                            // Save previous question
                            if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.Content))
                            {
                                questions.Add(currentQuestion);
                            }

                            // Start new question
                            currentQuestion = new Question
                            {
                                Content = System.Text.RegularExpressions.Regex.Replace(trimmedLine, @"^Câu\s+\d+:\s*", "").Trim(),
                                Options = new List<string>(),
                                Type = QuestionType.MultipleChoice,
                                DifficultyLevel = DifficultyLevel.Medium,
                                Score = 1.0
                            };
                            optionIndex = 0;
                        }
                        // Detect options: A., B., C., D.
                        else if (currentQuestion != null && System.Text.RegularExpressions.Regex.IsMatch(trimmedLine, @"^[A-D]\."))
                        {
                            var optionText = System.Text.RegularExpressions.Regex.Replace(trimmedLine, @"^[A-D]\.\s*", "").Trim();
                            currentQuestion.Options.Add(optionText);
                            optionIndex++;
                        }
                        // Detect metadata: [Đáp án: A | Môn: ... | Độ khó: ... | Điểm: ...]
                        else if (currentQuestion != null && trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                        {
                            ParseMetadata(currentQuestion, trimmedLine);
                        }
                        // Continue current question content
                        else if (currentQuestion != null && currentQuestion.Options.Count == 0)
                        {
                            currentQuestion.Content += " " + trimmedLine;
                        }
                    }

                    // Add last question
                    if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.Content))
                    {
                        questions.Add(currentQuestion);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error parsing PDF: {ex.Message}");
            }

            return questions;
        }

        /// <summary>
        /// Parse metadata từ Word
        /// </summary>
        private void ParseMetadata(Question question, string metadata)
        {
            // [Đáp án: A | Môn: CNPM | Độ khó: Easy | Chương: 1 | Điểm: 10]
            var parts = metadata.Trim('[', ']').Split('|');
            
            foreach (var part in parts)
            {
                var keyValue = part.Split(':');
                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                if (key.Contains("Đáp án"))
                {
                    question.CorrectAnswer = value;
                }
                else if (key.Contains("Môn"))
                {
                    question.SubjectId = value;
                }
                else if (key.Contains("Độ khó"))
                {
                    question.DifficultyLevel = ParseDifficulty(value);
                }
                else if (key.Contains("Điểm"))
                {
                    question.Score = ParseScore(value);
                }
            }
        }

        /// <summary>
        /// Hiển thị preview import
        /// </summary>
        public IActionResult PreviewImport()
        {
            var questionsJson = HttpContext.Session.GetString("ImportedQuestions");
            if (string.IsNullOrEmpty(questionsJson))
            {
                TempData["Error"] = "Không có dữ liệu để preview. Vui lòng import lại.";
                return RedirectToAction("QuestionBank");
            }

            var questions = System.Text.Json.JsonSerializer.Deserialize<List<Question>>(questionsJson);
            
            // Data remains in session for confirm action
            
            return View(questions);
        }

        /// <summary>
        /// Xác nhận import và lưu vào database
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ConfirmImport([FromBody] ImportRequest request)
        {
            try
            {
                List<Question> questions;
                
                // If edited questions are sent from client, use those; otherwise read from session
                if (request?.Questions != null && request.Questions.Count > 0)
                {
                    questions = request.Questions;
                    _logger.LogInformation($"Using {questions.Count} edited questions from client");
                }
                else
                {
                    var questionsJson = HttpContext.Session.GetString("ImportedQuestions");
                    if (string.IsNullOrEmpty(questionsJson))
                    {
                        return Json(new { success = false, message = "Không có dữ liệu để import." });
                    }
                    
                    questions = System.Text.Json.JsonSerializer.Deserialize<List<Question>>(questionsJson) ?? new List<Question>();
                    _logger.LogInformation($"Using {questions.Count} questions from session");
                }
                
                // Clear session after reading
                HttpContext.Session.Remove("ImportedQuestions");
                
                if (questions == null || questions.Count == 0)
                {
                    return Json(new { success = false, message = "Không có câu hỏi để import." });
                }

                // Get current user ID for createdBy
                var currentUserId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại." });
                }

                // Get all subjects để map SubjectId (name -> _id)
                var subjects = await _subjectApiService.GetAllSubjectsAsync();
                var subjectMap = subjects.ToDictionary(s => s.Name, s => s.Id);
                
                _logger.LogInformation($"Subject mapping loaded: {string.Join(", ", subjectMap.Keys)}");
                _logger.LogInformation($"Current UserId: {currentUserId}");

                // Bulk insert qua API
                int successCount = 0;
                var errors = new List<string>();

                foreach (var question in questions)
                {
                    try
                    {
                        var originalSubject = question.SubjectId;
                        
                        // Map SubjectId từ name sang ObjectId
                        if (!string.IsNullOrEmpty(question.SubjectId))
                        {
                            if (subjectMap.ContainsKey(question.SubjectId))
                            {
                                question.SubjectId = subjectMap[question.SubjectId];
                                _logger.LogInformation($"Mapped subject '{originalSubject}' to '{question.SubjectId}'");
                            }
                            else
                            {
                                _logger.LogWarning($"Subject '{question.SubjectId}' not found in database. Question will be skipped.");
                                errors.Add($"Câu '{question.Content?.Substring(0, Math.Min(30, question.Content.Length))}...': Môn học '{originalSubject}' không tồn tại.");
                                continue; // Skip this question
                            }
                        }
                        
                        // Set CreatedBy
                        question.CreatedBy = currentUserId;
                        
                        // Clear ExamId nếu empty
                        if (string.IsNullOrEmpty(question.ExamId))
                        {
                            question.ExamId = null;
                        }
                        
                        _logger.LogInformation($"Importing question: Subject={question.SubjectId}, CreatedBy={question.CreatedBy}");

                        await _questionApiService.CreateQuestionAsync(question);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Câu '{question.Content?.Substring(0, Math.Min(50, question.Content.Length))}...': {ex.Message}");
                    }
                }

                if (successCount > 0)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Import thành công {successCount}/{questions.Count} câu hỏi.",
                        errors = errors
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = "Không thể import câu hỏi nào.",
                        errors = errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error confirming import: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi khi import: {ex.Message}" });
            }
        }

        #endregion
    }
}