using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class TeacherController : BaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult QuestionBank()
        {
            return View();
        }

        public IActionResult CreateQuestion()
        {
            return View();
        }

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