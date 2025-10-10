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
            // Placeholder for exam details page
            ViewBag.ExamId = examId;
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
    }
}