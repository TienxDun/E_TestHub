using System.ComponentModel.DataAnnotations;

namespace E_TestHub.Models
{
    /// <summary>
    /// Loại câu hỏi
    /// </summary>
    public enum QuestionType
    {
        MultipleChoice = 1,  // Trắc nghiệm
        Essay = 2,           // Tự luận
        TrueFalse = 3        // Đúng/Sai
    }

    /// <summary>
    /// Độ khó của câu hỏi
    /// </summary>
    public enum DifficultyLevel
    {
        Easy = 1,    // Dễ
        Medium = 2,  // Trung bình
        Hard = 3     // Khó
    }

    /// <summary>
    /// Model câu hỏi trong ngân hàng câu hỏi
    /// </summary>
    public class Question
    {
        /// <summary>
        /// ID nội bộ (C#) - auto-increment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// MongoDB ObjectId (24-char hex string) - CRITICAL for API operations
        /// </summary>
        public string? ApiId { get; set; }

        /// <summary>
        /// ID đề thi (nếu câu hỏi đã được gán vào đề thi)
        /// </summary>
        public string? ExamId { get; set; }

        /// <summary>
        /// ID môn học
        /// </summary>
        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string SubjectId { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung câu hỏi (hỗ trợ HTML/Markdown)
        /// </summary>
        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Nội dung câu hỏi phải từ 10 đến 5000 ký tự")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Loại câu hỏi (Trắc nghiệm, Tự luận, Đúng/Sai)
        /// </summary>
        [Required(ErrorMessage = "Loại câu hỏi là bắt buộc")]
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

        /// <summary>
        /// Các đáp án (chỉ áp dụng cho trắc nghiệm và đúng/sai)
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// Đáp án đúng ("A", "B", "C", "D" hoặc "True", "False")
        /// </summary>
        public string? CorrectAnswer { get; set; }

        /// <summary>
        /// Điểm của câu hỏi
        /// </summary>
        [Required(ErrorMessage = "Điểm là bắt buộc")]
        [Range(0.5, 100, ErrorMessage = "Điểm phải từ 0.5 đến 100")]
        public double Score { get; set; } = 1.0;

        /// <summary>
        /// Độ khó
        /// </summary>
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;

        /// <summary>
        /// ID giảng viên tạo câu hỏi
        /// </summary>
        [Required(ErrorMessage = "Người tạo là bắt buộc")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian cập nhật gần nhất
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Tên môn học (để hiển thị, không lưu DB)
        /// </summary>
        public string? SubjectName { get; set; }

        /// <summary>
        /// Tên giảng viên tạo (để hiển thị, không lưu DB)
        /// </summary>
        public string? CreatedByName { get; set; }
    }

    /// <summary>
    /// ViewModel để tạo câu hỏi mới
    /// </summary>
    public class CreateQuestionViewModel
    {
        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Nội dung câu hỏi phải từ 10 đến 5000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại câu hỏi là bắt buộc")]
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

        public List<string> Options { get; set; } = new List<string> { "", "", "", "" };

        public string? CorrectAnswer { get; set; }

        [Required(ErrorMessage = "Điểm là bắt buộc")]
        [Range(0.5, 100, ErrorMessage = "Điểm phải từ 0.5 đến 100")]
        public double Score { get; set; } = 1.0;

        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    }

    /// <summary>
    /// ViewModel để chỉnh sửa câu hỏi
    /// </summary>
    public class EditQuestionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ApiId là bắt buộc")]
        public string? ApiId { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Nội dung câu hỏi phải từ 10 đến 5000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại câu hỏi là bắt buộc")]
        public QuestionType Type { get; set; }

        public List<string> Options { get; set; } = new List<string>();

        public string? CorrectAnswer { get; set; }

        [Required(ErrorMessage = "Điểm là bắt buộc")]
        [Range(0.5, 100, ErrorMessage = "Điểm phải từ 0.5 đến 100")]
        public double Score { get; set; }

        public DifficultyLevel DifficultyLevel { get; set; }
    }
}
