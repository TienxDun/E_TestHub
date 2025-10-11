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

        public IActionResult Notifications()
        {
            // Demo notifications data
            var notifications = new List<dynamic>
            {
                new 
                { 
                    Id = 1, 
                    Type = "exam", 
                    Title = "Bài thi ReactJS sắp diễn ra", 
                    Message = "Bài thi ReactJS của lớp SE114.O11 sẽ diễn ra vào lúc 14:00 ngày 28/09/2025. Thời gian làm bài: 90 phút.",
                    Time = "2 giờ trước",
                    Date = new DateTime(2025, 9, 28, 12, 0, 0),
                    IsRead = false,
                    Icon = "fa-file-alt",
                    Link = "/Student/ExamInfo?examId=1",
                    LinkText = "Xem chi tiết"
                },
                new 
                { 
                    Id = 2, 
                    Type = "result", 
                    Title = "Kết quả thi đã được công bố", 
                    Message = "Kết quả bài thi Lịch sử Việt Nam của bạn đã được chấm. Điểm số: 8.5/10. Xem chi tiết tại trang Kết quả thi.",
                    Time = "1 ngày trước",
                    Date = new DateTime(2025, 9, 27, 16, 30, 0),
                    IsRead = true,
                    Icon = "fa-check-circle",
                    Link = "/Student/ViewResults?studentId=2151012001&examId=3",
                    LinkText = "Xem chi tiết"
                },
                new 
                { 
                    Id = 3, 
                    Type = "schedule", 
                    Title = "Lịch thi mới được cập nhật", 
                    Message = "Lịch thi môn Toán cao cấp đã được cập nhật. Thời gian thi: 10:00 ngày 05/10/2025. Vui lòng kiểm tra lại lịch thi của bạn.",
                    Time = "2 ngày trước",
                    Date = new DateTime(2025, 9, 26, 9, 0, 0),
                    IsRead = true,
                    Icon = "fa-calendar-alt",
                    Link = "/Student/MyExams",
                    LinkText = "Xem chi tiết"
                },
                new 
                { 
                    Id = 4, 
                    Type = "system", 
                    Title = "Hệ thống bảo trì", 
                    Message = "Hệ thống sẽ bảo trì từ 22:00 - 02:00 đêm nay. Trong thời gian này, bạn sẽ không thể truy cập hệ thống. Vui lòng hoàn thành bài thi trước thời gian bảo trì.",
                    Time = "3 ngày trước",
                    Date = new DateTime(2025, 9, 25, 18, 0, 0),
                    IsRead = true,
                    Icon = "fa-exclamation-triangle",
                    Link = "/Student/Dashboard",
                    LinkText = "Xem chi tiết"
                },
                new 
                { 
                    Id = 5, 
                    Type = "exam", 
                    Title = "Nhắc nhở: Bài thi Cơ sở dữ liệu", 
                    Message = "Bài thi Cơ sở dữ liệu sẽ diễn ra vào 08:00 ngày 10/10/2025. Đây là môn thi quan trọng, vui lòng chuẩn bị kỹ càng.",
                    Time = "5 ngày trước",
                    Date = new DateTime(2025, 9, 23, 14, 0, 0),
                    IsRead = true,
                    Icon = "fa-file-alt",
                    Link = "/Student/ExamInfo?examId=2",
                    LinkText = "Xem chi tiết"
                },
                new 
                { 
                    Id = 6, 
                    Type = "announcement", 
                    Title = "Thông báo từ giáo vụ", 
                    Message = "Lịch thi học kỳ 1 năm học 2025-2026 đã được công bố. Sinh viên vui lòng truy cập trang web để xem lịch thi chi tiết và chuẩn bị ôn tập.",
                    Time = "1 tuần trước",
                    Date = new DateTime(2025, 9, 21, 10, 0, 0),
                    IsRead = true,
                    Icon = "fa-bullhorn",
                    Link = "/Student/MyExams",
                    LinkText = "Xem chi tiết"
                }
            };

            ViewBag.Notifications = notifications;
            ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);
            ViewBag.TotalCount = notifications.Count;

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
            // Demo data for exam information
            var examInfo = new
            {
                Name = "Bài thi ReactJS Framework",
                Subject = "Lập trình Web",
                ExamDate = new DateTime(2025, 9, 28, 9, 0, 0),
                SubmittedAt = new DateTime(2025, 9, 28, 9, 35, 0),
                Duration = 45,
                TotalQuestions = 12,
                CorrectAnswers = 8,
                IncorrectAnswers = 4,
                Score = 8.0,
                MaxScore = 10.0
            };

            // Calculate metrics
            var timeSpent = (examInfo.SubmittedAt - examInfo.ExamDate).TotalMinutes;
            var accuracy = (double)examInfo.CorrectAnswers / examInfo.TotalQuestions * 100;

            ViewBag.ExamInfo = examInfo;
            ViewBag.TimeSpent = Math.Round(timeSpent, 2);
            ViewBag.Accuracy = Math.Round(accuracy, 1);

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

        public IActionResult ClassDetails(string classId)
        {
            // Demo data for class information
            var classInfo = classId switch
            {
                "PM233H" => new { Id = "PM233H", Name = "PM233H - Xác suất thống kê", StudentCount = 360, Year = "2023 - 2027", Subject = "Xác suất thống kê", Instructor = "TS. Nguyễn Văn A" },
                "TM231H" => new { Id = "TM231H", Name = "TM231H - Toán cao cấp", StudentCount = 36, Year = "2023 - 2027", Subject = "Toán cao cấp", Instructor = "PGS.TS. Trần Thị B" },
                "SE214H" => new { Id = "SE214H", Name = "SE214H - Kỹ thuật phần mềm", StudentCount = 120, Year = "2023 - 2027", Subject = "Kỹ thuật phần mềm", Instructor = "TS. Lê Văn C" },
                "IT001" => new { Id = "IT001", Name = "IT001 - Tin học đại cương", StudentCount = 180, Year = "2023 - 2027", Subject = "Tin học đại cương", Instructor = "ThS. Phạm Thị D" },
                "CS101" => new { Id = "CS101", Name = "CS101 - Nhập môn lập trình", StudentCount = 95, Year = "2023 - 2027", Subject = "Lập trình", Instructor = "TS. Hoàng Văn E" },
                "DB234" => new { Id = "DB234", Name = "DB234 - Cơ sở dữ liệu", StudentCount = 150, Year = "2023 - 2027", Subject = "Cơ sở dữ liệu", Instructor = "PGS.TS. Đỗ Thị F" },
                "AI202" => new { Id = "AI202", Name = "AI202 - Trí tuệ nhân tạo", StudentCount = 80, Year = "2024 - 2028", Subject = "Trí tuệ nhân tạo", Instructor = "TS. Vũ Văn G" },
                "DS101" => new { Id = "DS101", Name = "DS101 - Khoa học dữ liệu", StudentCount = 110, Year = "2024 - 2028", Subject = "Khoa học dữ liệu", Instructor = "ThS. Bùi Thị H" },
                _ => new { Id = classId, Name = $"Lớp {classId}", StudentCount = 0, Year = "Unknown", Subject = "Unknown", Instructor = "Unknown" }
            };

            // Demo data for students in this class (showing 20 students)
            var students = new List<dynamic>
            {
                new { Id = "2151012001", Name = "Nguyễn Văn A", Email = "2151012001@student.edu.vn", Phone = "0123456789" },
                new { Id = "2151012002", Name = "Trần Thị B", Email = "2151012002@student.edu.vn", Phone = "0123456790" },
                new { Id = "2151012003", Name = "Lê Văn C", Email = "2151012003@student.edu.vn", Phone = "0123456791" },
                new { Id = "2151012004", Name = "Phạm Thị D", Email = "2151012004@student.edu.vn", Phone = "0123456792" },
                new { Id = "2151012005", Name = "Hoàng Văn E", Email = "2151012005@student.edu.vn", Phone = "0123456793" },
                new { Id = "2151012006", Name = "Đỗ Thị F", Email = "2151012006@student.edu.vn", Phone = "0123456794" },
                new { Id = "2151012007", Name = "Vũ Văn G", Email = "2151012007@student.edu.vn", Phone = "0123456795" },
                new { Id = "2151012008", Name = "Bùi Thị H", Email = "2151012008@student.edu.vn", Phone = "0123456796" },
                new { Id = "2151012009", Name = "Đinh Văn I", Email = "2151012009@student.edu.vn", Phone = "0123456797" },
                new { Id = "2151012010", Name = "Cao Thị J", Email = "2151012010@student.edu.vn", Phone = "0123456798" },
                new { Id = "2151012011", Name = "Đặng Văn K", Email = "2151012011@student.edu.vn", Phone = "0123456799" },
                new { Id = "2151012012", Name = "Mai Thị L", Email = "2151012012@student.edu.vn", Phone = "0123456800" },
                new { Id = "2151012013", Name = "Ngô Văn M", Email = "2151012013@student.edu.vn", Phone = "0123456801" },
                new { Id = "2151012014", Name = "Phan Thị N", Email = "2151012014@student.edu.vn", Phone = "0123456802" },
                new { Id = "2151012015", Name = "Tô Văn O", Email = "2151012015@student.edu.vn", Phone = "0123456803" },
                new { Id = "2151012016", Name = "Võ Thị P", Email = "2151012016@student.edu.vn", Phone = "0123456804" },
                new { Id = "2151012017", Name = "Dương Văn Q", Email = "2151012017@student.edu.vn", Phone = "0123456805" },
                new { Id = "2151012018", Name = "Lý Thị R", Email = "2151012018@student.edu.vn", Phone = "0123456806" },
                new { Id = "2151012019", Name = "Trương Văn S", Email = "2151012019@student.edu.vn", Phone = "0123456807" },
                new { Id = "2151012020", Name = "Huỳnh Thị T", Email = "2151012020@student.edu.vn", Phone = "0123456808" }
            };

            ViewBag.ClassId = classId;
            ViewBag.ClassInfo = classInfo;
            ViewBag.Students = students;
            ViewBag.TotalStudents = students.Count;

            return View();
        }
    }
}