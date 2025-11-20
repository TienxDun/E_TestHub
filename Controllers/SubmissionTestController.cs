using E_TestHub.Models;
using E_TestHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    /// <summary>
    /// Test controller to verify SubmissionApiService functionality
    /// Remove this after testing is complete
    /// </summary>
    public class SubmissionTestController : Controller
    {
        private readonly ISubmissionApiService _submissionService;
        private readonly ILogger<SubmissionTestController> _logger;

        public SubmissionTestController(
            ISubmissionApiService submissionService,
            ILogger<SubmissionTestController> logger)
        {
            _submissionService = submissionService;
            _logger = logger;
        }

        /// <summary>
        /// Test page to verify submission service
        /// Access: /SubmissionTest/Index
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Test: Get all submissions for a student
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestGetStudentSubmissions(string studentId)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    return Json(new { success = false, message = "StudentId is required" });
                }

                var submissions = await _submissionService.GetStudentSubmissionsAsync(studentId);
                
                return Json(new
                {
                    success = true,
                    count = submissions.Count,
                    submissions = submissions.Select(s => new
                    {
                        id = s.ApiId,
                        examId = s.ExamId,
                        score = s.Score,
                        status = s.Status,
                        isGraded = s.IsGraded,
                        submittedAt = s.SubmittedAt,
                        createdAt = s.CreatedAt
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing GetStudentSubmissions");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Test: Get submission by ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestGetSubmissionById(string submissionId)
        {
            try
            {
                if (string.IsNullOrEmpty(submissionId))
                {
                    return Json(new { success = false, message = "SubmissionId is required" });
                }

                var submission = await _submissionService.GetSubmissionByIdAsync(submissionId);
                
                if (submission == null)
                {
                    return Json(new { success = false, message = "Submission not found" });
                }

                return Json(new
                {
                    success = true,
                    submission = new
                    {
                        id = submission.ApiId,
                        examId = submission.ExamId,
                        studentId = submission.StudentId,
                        score = submission.Score,
                        status = submission.Status,
                        isGraded = submission.IsGraded,
                        answersCount = submission.Answers.Count,
                        submittedAt = submission.SubmittedAt,
                        createdAt = submission.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing GetSubmissionById");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Test: Check if student has submitted exam
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestHasSubmitted(string examId, string studentId)
        {
            try
            {
                if (string.IsNullOrEmpty(examId) || string.IsNullOrEmpty(studentId))
                {
                    return Json(new { success = false, message = "ExamId and StudentId are required" });
                }

                var hasSubmitted = await _submissionService.HasSubmittedExamAsync(examId, studentId);
                
                return Json(new
                {
                    success = true,
                    examId = examId,
                    studentId = studentId,
                    hasSubmitted = hasSubmitted
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing HasSubmitted");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Test: Get all submissions for an exam
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestGetExamSubmissions(string examId)
        {
            try
            {
                if (string.IsNullOrEmpty(examId))
                {
                    return Json(new { success = false, message = "ExamId is required" });
                }

                var submissions = await _submissionService.GetExamSubmissionsAsync(examId);
                
                return Json(new
                {
                    success = true,
                    examId = examId,
                    count = submissions.Count,
                    submissions = submissions.Select(s => new
                    {
                        id = s.ApiId,
                        studentId = s.StudentId,
                        score = s.Score,
                        status = s.Status,
                        isGraded = s.IsGraded,
                        submittedAt = s.SubmittedAt
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing GetExamSubmissions");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Test: Start a new exam (use with caution - creates data)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TestStartExam([FromBody] StartExamRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ExamId) || string.IsNullOrEmpty(request.StudentId))
                {
                    return Json(new { success = false, message = "ExamId and StudentId are required" });
                }

                var submission = await _submissionService.StartExamAsync(request.ExamId, request.StudentId);
                
                if (submission == null)
                {
                    return Json(new { success = false, message = "Failed to start exam" });
                }

                return Json(new
                {
                    success = true,
                    message = "Exam started successfully",
                    submission = new
                    {
                        id = submission.ApiId,
                        examId = submission.ExamId,
                        studentId = submission.StudentId,
                        status = submission.Status,
                        createdAt = submission.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing StartExam");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Test: Get submission statistics
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestGetStatistics()
        {
            try
            {
                var stats = await _submissionService.GetSubmissionStatisticsAsync();
                
                return Json(new
                {
                    success = true,
                    statistics = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing GetStatistics");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
