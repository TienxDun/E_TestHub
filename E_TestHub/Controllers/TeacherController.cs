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

        public IActionResult ExamDetails(int examId)
        {
            // Placeholder for exam details page
            ViewBag.ExamId = examId;
            return View();
        }
    }
}