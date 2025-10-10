using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class StudentController : BaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Classes()
        {
            return View();
        }

        public IActionResult MyExams()
        {
            return View();
        }

        public IActionResult TestCalendar()
        {
            return View();
        }

        public IActionResult ExamInfo(int examId)
        {
            ViewBag.ExamId = examId;
            
            // Simulate exam status based on current date and exam schedule
            var currentDate = new DateTime(2025, 9, 28); // Fixed current date for demo
            string examStatus = "upcoming"; // Default
            
            // Mock exam data with different statuses
            switch (examId)
            {
                case 1: // ReactJS exam - in progress today (28/9/2025)
                    var examDate1 = new DateTime(2025, 9, 28);
                    var examEndTime1 = examDate1.AddHours(2); // Exam duration 2 hours
                    
                    if (currentDate.Date == examDate1.Date && currentDate <= examEndTime1)
                        examStatus = "in-progress";
                    else if (currentDate > examEndTime1)
                        examStatus = "completed";
                    else
                        examStatus = "upcoming";
                    break;
                    
                case 2: // History exam - upcoming (5/10/2025)
                    var examDate2 = new DateTime(2025, 10, 5);
                    var examEndTime2 = examDate2.AddHours(2);
                    
                    if (currentDate.Date == examDate2.Date && currentDate <= examEndTime2)
                        examStatus = "in-progress";
                    else if (currentDate > examEndTime2)
                        examStatus = "completed";
                    else
                        examStatus = "upcoming";
                    break;
                    
                default:
                    examStatus = "upcoming";
                    break;
            }
            
            ViewBag.ExamStatus = examStatus;
            return View();
        }

        public IActionResult TakeExam(int examId)
        {
            return View();
        }

        public IActionResult ViewResults()
        {
            return View();
        }

        public IActionResult ExamScores()
        {
            // Demo data: Exam scores for student
            var examScores = new List<dynamic>
            {
                new {
                    ExamId = 1,
                    ExamName = "Kiểm tra giữa kỳ - Lập trình Web",
                    Subject = "Lập trình Web",
                    ExamDate = new DateTime(2025, 9, 15),
                    Score = 8.5,
                    MaxScore = 10.0,
                    Status = "Đã chấm",
                    Passed = true,
                    SubmittedAt = new DateTime(2025, 9, 15, 10, 30, 0)
                },
                new {
                    ExamId = 2,
                    ExamName = "Bài thi cuối kỳ - Cơ sở dữ liệu",
                    Subject = "Cơ sở dữ liệu",
                    ExamDate = new DateTime(2025, 9, 20),
                    Score = 9.0,
                    MaxScore = 10.0,
                    Status = "Đã chấm",
                    Passed = true,
                    SubmittedAt = new DateTime(2025, 9, 20, 14, 45, 0)
                },
                new {
                    ExamId = 3,
                    ExamName = "Kiểm tra thường xuyên - Toán cao cấp",
                    Subject = "Toán cao cấp",
                    ExamDate = new DateTime(2025, 9, 25),
                    Score = 6.5,
                    MaxScore = 10.0,
                    Status = "Đã chấm",
                    Passed = true,
                    SubmittedAt = new DateTime(2025, 9, 25, 9, 15, 0)
                },
                new {
                    ExamId = 4,
                    ExamName = "Bài thi ReactJS Framework",
                    Subject = "Lập trình Web",
                    ExamDate = new DateTime(2025, 9, 28),
                    Score = 0.0,
                    MaxScore = 10.0,
                    Status = "Đang chờ chấm",
                    Passed = false,
                    SubmittedAt = new DateTime(2025, 9, 28, 11, 0, 0)
                },
                new {
                    ExamId = 5,
                    ExamName = "Kiểm tra giữa kỳ - Xác suất thống kê",
                    Subject = "Xác suất thống kê",
                    ExamDate = new DateTime(2025, 8, 10),
                    Score = 7.0,
                    MaxScore = 10.0,
                    Status = "Đã chấm",
                    Passed = true,
                    SubmittedAt = new DateTime(2025, 8, 10, 15, 20, 0)
                },
                new {
                    ExamId = 6,
                    ExamName = "Bài thi cuối kỳ - Hệ điều hành",
                    Subject = "Hệ điều hành",
                    ExamDate = new DateTime(2025, 8, 25),
                    Score = 5.5,
                    MaxScore = 10.0,
                    Status = "Đã chấm",
                    Passed = false,
                    SubmittedAt = new DateTime(2025, 8, 25, 16, 30, 0)
                }
            };

            ViewBag.ExamScores = examScores;
            
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

        public IActionResult Profile()
        {
            return View();
        }
    }
}