using Microsoft.AspNetCore.Mvc;
using E_TestHub.Models;
using E_TestHub.Services;
using System.Text.Json;

namespace E_TestHub.Controllers
{
    public class StudentController : BaseController
    {
        private readonly ISubmissionApiService _submissionService;
        private readonly IExamApiService _examService;
        private readonly IQuestionApiService _questionService;
        private readonly IExamScheduleApiService _scheduleService;
        private readonly IClassApiService _classApiService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            ISubmissionApiService submissionService,
            IExamApiService examService,
            IQuestionApiService questionService,
            IExamScheduleApiService scheduleService,
            IClassApiService classApiService,
            ILogger<StudentController> logger)
        {
            _submissionService = submissionService;
            _examService = examService;
            _questionService = questionService;
            _scheduleService = scheduleService;
            _classApiService = classApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get upcoming exams
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                var now = DateTime.Now;
                var upcomingExams = new List<dynamic>();

                foreach (var schedule in schedules.Where(s => !s.IsClosed && s.StartTime > now).Take(5))
                {
                    var exam = await _examService.GetExamByIdAsync(schedule.ExamId);
                    if (exam != null)
                    {
                        upcomingExams.Add(new
                        {
                            ExamId = exam.ApiId,
                            Title = exam.Title,
                            Subject = exam.SubjectId,
                            StartTime = schedule.StartTime,
                            Duration = exam.Duration
                        });
                    }
                }

                // Get recent submissions
                var submissions = await _submissionService.GetStudentSubmissionsAsync(studentId);
                var recentSubmissions = submissions?.OrderByDescending(s => s.CreatedAt).Take(5).ToList();

                // Calculate stats
                var totalExams = submissions?.Count ?? 0;
                var completedExams = submissions?.Count(s => s.Status == "submitted") ?? 0;
                var averageScore = submissions?.Where(s => s.Status == "submitted").Any() == true 
                    ? submissions.Where(s => s.Status == "submitted").Average(s => s.Score) 
                    : 0.0;

                ViewBag.UpcomingExams = upcomingExams;
                ViewBag.RecentSubmissions = recentSubmissions;
                ViewBag.TotalExams = totalExams;
                ViewBag.CompletedExams = completedExams;
                ViewBag.AverageScore = Math.Round(averageScore, 2);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student dashboard");
                return View();
            }
        }

        public async Task<IActionResult> Classes()
        {
            try
            {
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get all classes from API
                var allClasses = await _classApiService.GetAllClassesAsync();
                
                // Filter classes that this student is enrolled in
                var studentClasses = allClasses
                    .Where(c => c.Students != null && c.Students.Contains(studentId))
                    .ToList();

                ViewBag.Classes = studentClasses;

                return View("Classes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading classes for student");
                ViewBag.Classes = new List<Class>();
                return View("Classes");
            }
        }

        public async Task<IActionResult> MyExams()
        {
            try
            {
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get all exam schedules
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                
                // Get student's submissions to check exam status
                var submissions = await _submissionService.GetStudentSubmissionsAsync(studentId);
                
                // Create view models with status
                var examViewModels = new List<dynamic>();
                var now = DateTime.Now;

                foreach (var schedule in schedules)
                {
                    // Get exam details
                    var exam = await _examService.GetExamByIdAsync(schedule.ExamId);
                    if (exam == null) continue;

                    // Check if student has submitted this exam
                    var submission = submissions?.FirstOrDefault(s => s.ExamId == schedule.ExamId);
                    
                    string status;
                    string statusClass;
                    bool canTakeExam = false;

                    if (submission != null && submission.Status == "submitted")
                    {
                        status = "Đã hoàn thành";
                        statusClass = "completed";
                    }
                    else if (submission != null && submission.Status == "in-progress")
                    {
                        status = "Đang làm";
                        statusClass = "in-progress";
                        canTakeExam = !schedule.IsClosed && now >= schedule.StartTime && now <= schedule.EndTime;
                    }
                    else if (now < schedule.StartTime)
                    {
                        status = "Sắp diễn ra";
                        statusClass = "upcoming";
                    }
                    else if (now > schedule.EndTime || schedule.IsClosed)
                    {
                        status = "Đã đóng";
                        statusClass = "closed";
                    }
                    else
                    {
                        status = "Đang diễn ra";
                        statusClass = "in-progress";
                        canTakeExam = true;
                    }

                    examViewModels.Add(new
                    {
                        ExamId = exam.ApiId,
                        ExamName = exam.Title,
                        Subject = exam.SubjectId,
                        ScheduleId = schedule.ApiId,
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime,
                        Duration = exam.Duration,
                        TotalQuestions = exam.QuestionIds?.Count ?? 0,
                        Status = status,
                        StatusClass = statusClass,
                        CanTakeExam = canTakeExam,
                        Score = submission?.Score,
                        SubmissionId = submission?.ApiId
                    });
                }

                // Sort by start time
                ViewBag.Exams = examViewModels.OrderBy(e => e.StartTime).ToList();
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading my exams");
                TempData["Error"] = "Đã xảy ra lỗi khi tải danh sách kỳ thi.";
                return View();
            }
        }

        public async Task<IActionResult> Notifications()
        {
            try
            {
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                var notifications = new List<dynamic>();

                // Get upcoming exams for notifications
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                var now = DateTime.Now;
                
                if (schedules != null)
                {
                    // Upcoming exams (within next 7 days)
                    var upcomingExams = schedules.Where(s => 
                        !s.IsClosed && 
                        s.StartTime > now && 
                        s.StartTime <= now.AddDays(7)
                    ).OrderBy(s => s.StartTime).Take(3);

                    foreach (var schedule in upcomingExams)
                    {
                        var exam = await _examService.GetExamByIdAsync(schedule.ExamId);
                        if (exam != null)
                        {
                            var timeToExam = schedule.StartTime - now;
                            string timeText = timeToExam.Days > 0 
                                ? $"{timeToExam.Days} ngày nữa"
                                : timeToExam.Hours > 0 
                                    ? $"{timeToExam.Hours} giờ nữa"
                                    : $"{timeToExam.Minutes} phút nữa";

                            notifications.Add(new
                            {
                                Id = $"exam_{schedule.ApiId}",
                                Type = "exam",
                                Title = $"Bài thi {exam.Title} sắp diễn ra",
                                Message = $"Bài thi {exam.Title} sẽ diễn ra vào lúc {schedule.StartTime:HH:mm} ngày {schedule.StartTime:dd/MM/yyyy}. Thời gian làm bài: {exam.Duration} phút.",
                                Time = timeText,
                                Date = schedule.StartTime,
                                IsRead = false,
                                Icon = "fa-file-alt",
                                Link = $"/Student/ExamInfo?examId={exam.ApiId}",
                                LinkText = "Xem chi tiết"
                            });
                        }
                    }
                }

                // Get recent submissions for result notifications
                var submissions = await _submissionService.GetStudentSubmissionsAsync(studentId);
                if (submissions != null)
                {
                    var recentGraded = submissions
                        .Where(s => s.Status == "submitted" && s.SubmittedAt.HasValue)
                        .OrderByDescending(s => s.SubmittedAt)
                        .Take(3);

                    foreach (var submission in recentGraded)
                    {
                        var exam = await _examService.GetExamByIdAsync(submission.ExamId);
                        if (exam != null)
                        {
                            var timeAgo = now - (submission.SubmittedAt ?? submission.CreatedAt);
                            string timeText = timeAgo.Days > 0 
                                ? $"{timeAgo.Days} ngày trước"
                                : timeAgo.Hours > 0 
                                    ? $"{timeAgo.Hours} giờ trước"
                                    : $"{timeAgo.Minutes} phút trước";

                            notifications.Add(new
                            {
                                Id = $"result_{submission.ApiId}",
                                Type = "result",
                                Title = "Kết quả thi đã được công bố",
                                Message = $"Kết quả bài thi {exam.Title} của bạn đã được chấm. Điểm số: {submission.Score:F1}/10.",
                                Time = timeText,
                                Date = submission.SubmittedAt ?? submission.CreatedAt,
                                IsRead = true,
                                Icon = "fa-check-circle",
                                Link = $"/Student/ViewResults?submissionId={submission.ApiId}",
                                LinkText = "Xem chi tiết"
                            });
                        }
                    }
                }

                // Add system notifications (can be enhanced later with a notification system)
                if (notifications.Count == 0)
                {
                    notifications.Add(new
                    {
                        Id = "welcome",
                        Type = "system",
                        Title = "Chào mừng đến với E-TestHub",
                        Message = "Hệ thống thi trực tuyến E-TestHub. Bạn có thể xem lịch thi và tham gia các bài kiểm tra được phân công.",
                        Time = "Hôm nay",
                        Date = now,
                        IsRead = true,
                        Icon = "fa-info-circle",
                        Link = "/Student/MyExams",
                        LinkText = "Xem lịch thi"
                    });
                }

                ViewBag.Notifications = notifications.OrderBy(n => n.IsRead).ThenByDescending(n => n.Date).ToList();
                ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);
                ViewBag.TotalCount = notifications.Count;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications for student");
                ViewBag.Notifications = new List<dynamic>();
                ViewBag.UnreadCount = 0;
                ViewBag.TotalCount = 0;
                return View();
            }
        }

        public async Task<IActionResult> ExamInfo(string examId)
        {
            try
            {
                if (string.IsNullOrEmpty(examId))
                {
                    TempData["Error"] = "Không tìm thấy mã đề thi.";
                    return RedirectToAction("MyExams");
                }

                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get exam details
                var exam = await _examService.GetExamByIdAsync(examId);
                if (exam == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin đề thi.";
                    return RedirectToAction("MyExams");
                }

                // Get exam schedule
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                var schedule = schedules?.FirstOrDefault(s => s.ExamId == examId);

                // Check if student has submitted this exam
                var submission = await _submissionService.GetExamSubmissionAsync(examId, studentId);

                // Determine exam status
                string examStatus = "upcoming";
                var now = DateTime.Now;

                if (submission != null && submission.Status == "submitted")
                {
                    examStatus = "completed";
                }
                else if (submission != null && submission.Status == "in-progress")
                {
                    examStatus = "in-progress";
                }
                else if (schedule != null)
                {
                    if (now < schedule.StartTime)
                        examStatus = "upcoming";
                    else if (now > schedule.EndTime || schedule.IsClosed)
                        examStatus = "closed";
                    else
                        examStatus = "in-progress";
                }

                // Prepare view data
                ViewBag.ExamId = examId;
                ViewBag.ExamStatus = examStatus;
                ViewBag.Exam = exam;
                ViewBag.Schedule = schedule;
                ViewBag.Submission = submission;
                ViewBag.CanTakeExam = examStatus == "in-progress" && submission?.Status != "submitted";

                return View("ExamInfo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading exam info for examId {ExamId}", examId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải thông tin đề thi.";
                return RedirectToAction("MyExams");
            }
        }

        public IActionResult TakeExam(int examId)
        {
            return View();
        }

        public async Task<IActionResult> ViewResults(string submissionId)
        {
            try
            {
                if (string.IsNullOrEmpty(submissionId))
                {
                    TempData["Error"] = "Không tìm thấy bài làm.";
                    return RedirectToAction("ExamScores");
                }

                // Get submission details
                var submission = await _submissionService.GetSubmissionByIdAsync(submissionId);
                if (submission == null)
                {
                    TempData["Error"] = "Không tìm thấy bài làm.";
                    return RedirectToAction("ExamScores");
                }

                // Get exam details
                var exam = await _examService.GetExamByIdAsync(submission.ExamId);
                if (exam == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin đề thi.";
                    return RedirectToAction("ExamScores");
                }

                // Get questions for this exam
                var questions = new List<Question>();
                if (exam.QuestionIds != null && exam.QuestionIds.Any())
                {
                    foreach (var questionId in exam.QuestionIds)
                    {
                        var question = await _questionService.GetQuestionByIdAsync(questionId);
                        if (question != null)
                        {
                            questions.Add(question);
                        }
                    }
                }

                // Calculate metrics
                var totalQuestions = questions.Count;
                var correctAnswers = submission.Answers?.Count(a => a.Score > 0) ?? 0;
                var incorrectAnswers = totalQuestions - correctAnswers;
                var timeSpent = submission.SubmittedAt.HasValue 
                    ? (submission.SubmittedAt.Value - submission.CreatedAt).TotalMinutes 
                    : 0;
                var accuracy = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

                var examInfo = new
                {
                    Name = exam.Title,
                    Subject = exam.SubjectId,
                    ExamDate = submission.CreatedAt,
                    SubmittedAt = submission.SubmittedAt ?? submission.CreatedAt,
                    Duration = exam.Duration,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    IncorrectAnswers = incorrectAnswers,
                    Score = submission.Score,
                    MaxScore = 10.0
                };

                ViewBag.ExamInfo = examInfo;
                ViewBag.TimeSpent = Math.Round(timeSpent, 2);
                ViewBag.Accuracy = Math.Round(accuracy, 1);
                ViewBag.Questions = questions;
                ViewBag.Answers = submission.Answers ?? new List<SubmissionAnswer>();

                return View("ViewResults");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading exam results for submission {SubmissionId}", submissionId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải kết quả thi.";
                return RedirectToAction("ExamScores");
            }
        }

        public async Task<IActionResult> ExamScores()
        {
            try
            {
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get all submissions for this student
                var submissions = await _submissionService.GetStudentSubmissionsAsync(studentId);
                
                var examScores = new List<dynamic>();

                if (submissions != null && submissions.Any())
                {
                    foreach (var submission in submissions)
                    {
                        // Get exam details
                        var exam = await _examService.GetExamByIdAsync(submission.ExamId);
                        if (exam == null) continue;

                        // Determine status
                        string status = submission.Status == "submitted" ? "Đã chấm" : "Đang chờ chấm";
                        bool passed = submission.Score >= (exam.PassingScore ?? 5.0);

                        examScores.Add(new
                        {
                            ExamId = exam.ApiId,
                            ExamName = exam.Title,
                            Subject = exam.SubjectId,
                            ExamDate = submission.CreatedAt,
                            Score = submission.Score,
                            MaxScore = 10.0,
                            Status = status,
                            Passed = passed,
                            SubmittedAt = submission.SubmittedAt ?? submission.CreatedAt,
                            SubmissionId = submission.ApiId
                        });
                    }
                }

                ViewBag.ExamScores = examScores.OrderByDescending(e => e.ExamDate).ToList();
                
                // Calculate statistics
                var gradedExams = examScores.Where(e => e.Status == "Đã chấm").ToList();
                ViewBag.TotalExams = examScores.Count;
                ViewBag.GradedCount = gradedExams.Count;
                ViewBag.PendingCount = examScores.Count - gradedExams.Count;
                ViewBag.AverageScore = gradedExams.Any() ? (double)gradedExams.Average(e => (double)e.Score) : 0.0;
                ViewBag.PassedCount = gradedExams.Count(e => e.Passed);
                ViewBag.FailedCount = gradedExams.Count(e => !e.Passed);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading exam scores");
                TempData["Error"] = "Đã xảy ra lỗi khi tải điểm thi.";
                return View();
            }
        }

        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> ClassDetails(string classId)
        {
            try
            {
                if (string.IsNullOrEmpty(classId))
                {
                    TempData["Error"] = "Không tìm thấy mã lớp học.";
                    return RedirectToAction("Classes");
                }

                // Get class details from API
                var classDetails = await _classApiService.GetClassByIdAsync(classId);
                if (classDetails == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin lớp học.";
                    return RedirectToAction("Classes");
                }

                // Create class info object
                var classInfo = new
                {
                    Id = classDetails.ClassCode ?? classId,
                    Name = $"{classDetails.ClassCode} - {classDetails.Name}",
                    StudentCount = classDetails.StudentList?.Count ?? 0,
                    Year = classDetails.AcademicYear ?? "Chưa xác định",
                    Subject = classDetails.Name ?? "Chưa xác định",
                    Instructor = classDetails.Teacher?.FullName ?? "Chưa xác định"
                };

                // Prepare students list (hide sensitive info for student view)
                var studentsList = classDetails.StudentList?.Select(s => new
                {
                    Id = s.Id.ToString(),
                    Name = s.FullName ?? "Chưa cập nhật",
                    Email = !string.IsNullOrEmpty(s.Email) ? MaskEmail(s.Email) : "Chưa cập nhật", // Mask email for privacy
                    Phone = "***" // Hide phone for privacy
                }).ToList();

                ViewBag.ClassId = classId;
                ViewBag.ClassInfo = classInfo;
                ViewBag.Students = (IEnumerable<dynamic>?)studentsList ?? new List<dynamic>();
                ViewBag.TotalStudents = studentsList?.Count ?? 0;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading class details for classId {ClassId}", classId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải thông tin lớp học.";
                return RedirectToAction("Classes");
            }
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                return email;

            var parts = email.Split('@');
            if (parts[0].Length <= 3)
                return email;

            var masked = parts[0].Substring(0, 2) + "***" + parts[0].Substring(parts[0].Length - 1);
            return masked + "@" + parts[1];
        }

        #region Take Exam Actions

        /// <summary>
        /// GET: Load exam taking interface
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TakeExam(string examId)
        {
            try
            {
                _logger.LogInformation($"TakeExam GET called with examId: '{examId}'");
                
                if (string.IsNullOrEmpty(examId))
                {
                    TempData["Error"] = "Không tìm thấy mã đề thi.";
                    return RedirectToAction("MyExams");
                }

                // Get student ID from session (you'll need to implement this)
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return RedirectToAction("Login", "Home");
                }

                // Get exam details
                var exam = await _examService.GetExamByIdAsync(examId);
                if (exam == null)
                {
                    TempData["Error"] = "Không tìm thấy đề thi.";
                    return RedirectToAction("MyExams");
                }

                // Ensure ApiId is set (fallback to examId parameter)
                if (string.IsNullOrEmpty(exam.ApiId))
                {
                    _logger.LogWarning($"Exam.ApiId was null, setting to examId parameter: '{examId}'");
                    exam.ApiId = examId;
                }
                else
                {
                    _logger.LogInformation($"Exam.ApiId is set: '{exam.ApiId}'");
                }

                // Check if exam is published
                if (!exam.IsPublished)
                {
                    TempData["Error"] = "Đề thi chưa được công bố.";
                    return RedirectToAction("MyExams");
                }

                // Get exam questions - for now get all and filter
                var allQuestions = await _questionService.GetAllQuestionsAsync();
                var questions = allQuestions?.Where(q => exam.QuestionIds.Contains(q.ApiId ?? "")).ToList() ?? new List<Question>();
                if (questions == null || !questions.Any())
                {
                    TempData["Error"] = "Đề thi không có câu hỏi.";
                    return RedirectToAction("MyExams");
                }

                // Check if student has already submitted
                var existingSubmission = await _submissionService.GetExamSubmissionAsync(examId, studentId);
                if (existingSubmission != null && existingSubmission.IsGraded)
                {
                    TempData["Info"] = "Bạn đã hoàn thành bài thi này.";
                    return RedirectToAction("ExamScores", new { submissionId = existingSubmission.ApiId });
                }

                // Get exam schedule to check timing
                var allSchedules = await _scheduleService.GetAllSchedulesAsync();
                var schedule = allSchedules?.FirstOrDefault(s => s.ExamId == examId);
                
                var viewModel = new TakeExamViewModel
                {
                    Exam = exam,
                    Questions = questions,
                    ExistingSubmission = existingSubmission,
                    Schedule = schedule ?? new ExamSchedule(),
                    DurationMinutes = exam.Duration,
                    CanStart = true,
                    Message = ""
                };

                // Check schedule timing if exists
                if (schedule != null)
                {
                    var now = DateTime.Now;
                    
                    if (now < schedule.StartTime)
                    {
                        viewModel.CanStart = false;
                        viewModel.Message = $"Bài thi sẽ bắt đầu lúc {schedule.StartTime:dd/MM/yyyy HH:mm}";
                    }
                    else if (now > schedule.EndTime)
                    {
                        viewModel.CanStart = false;
                        viewModel.Message = "Thời gian làm bài đã hết.";
                    }
                    else if (schedule.IsClosed)
                    {
                        viewModel.CanStart = false;
                        viewModel.Message = "Bài thi đã được đóng.";
                    }
                }

                return View("TakeExam", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading take exam page for exam {ExamId}", examId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải đề thi.";
                return RedirectToAction("MyExams");
            }
        }

        /// <summary>
        /// POST: Start exam and create submission
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartExam([FromBody] StartExamRequest request)
        {
            try
            {
                var examId = request?.ExamId;
                _logger.LogInformation($"StartExam called with examId: '{examId}'");
                
                if (string.IsNullOrEmpty(examId))
                {
                    _logger.LogError("StartExam: examId is null or empty!");
                    return Json(new { success = false, message = "Mã đề thi không hợp lệ." });
                }
                
                var studentId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(studentId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                // Check if already started
                var existing = await _submissionService.GetExamSubmissionAsync(examId, studentId);
                if (existing != null)
                {
                    return Json(new { success = true, submissionId = existing.ApiId });
                }

                // Create new submission
                var submission = await _submissionService.StartExamAsync(examId, studentId);
                
                if (submission != null && !string.IsNullOrEmpty(submission.ApiId))
                {
                    _logger.LogInformation($"Student {studentId} started exam {examId}. Submission: {submission.ApiId}");
                    return Json(new { success = true, submissionId = submission.ApiId });
                }

                return Json(new { success = false, message = "Không thể bắt đầu làm bài." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting exam");
                return Json(new { success = false, message = "Đã xảy ra lỗi." });
            }
        }

        /// <summary>
        /// POST: Save answers (auto-save during exam)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAnswers([FromBody] SaveAnswersRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SubmissionId))
                {
                    return Json(new { success = false, message = "Không tìm thấy mã bài làm." });
                }

                var result = await _submissionService.SaveAnswersAsync(request.SubmissionId, request.Answers);
                
                if (result != null)
                {
                    return Json(new { success = true, message = "Đã lưu tự động." });
                }

                return Json(new { success = false, message = "Không thể lưu." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving answers for submission {SubmissionId}", request.SubmissionId);
                return Json(new { success = false, message = "Lỗi khi lưu." });
            }
        }

        /// <summary>
        /// POST: Submit final exam
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SubmissionId))
                {
                    return Json(new { success = false, message = "Không tìm thấy mã bài làm." });
                }

                var result = await _submissionService.SubmitExamAsync(request.SubmissionId, request.Answers);
                
                if (result != null)
                {
                    _logger.LogInformation($"Submission {request.SubmissionId} submitted successfully");
                    return Json(new { 
                        success = true, 
                        message = "Nộp bài thành công!", 
                        redirectUrl = Url.Action("ExamScores", new { submissionId = request.SubmissionId })
                    });
                }

                return Json(new { success = false, message = "Không thể nộp bài." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting exam for submission {SubmissionId}", request.SubmissionId);
                return Json(new { success = false, message = "Lỗi khi nộp bài." });
            }
        }

        #endregion
    }
}