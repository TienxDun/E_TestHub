using System.ComponentModel.DataAnnotations;

namespace E_TestHub.Models
{
    /// <summary>
    /// Model đề thi
    /// </summary>
    public class Exam
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
        /// Tiêu đề đề thi
        /// </summary>
        [Required(ErrorMessage = "Tiêu đề đề thi là bắt buộc")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả đề thi
        /// </summary>
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        /// <summary>
        /// ID môn học
        /// </summary>
        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string SubjectId { get; set; } = string.Empty;

        /// <summary>
        /// ID giảng viên tạo đề thi
        /// </summary>
        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        public string TeacherId { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian làm bài (phút)
        /// </summary>
        [Required(ErrorMessage = "Thời gian làm bài là bắt buộc")]
        [Range(1, 300, ErrorMessage = "Thời gian làm bài phải từ 1 đến 300 phút")]
        public int Duration { get; set; } = 60;

        /// <summary>
        /// Danh sách ID câu hỏi trong đề thi
        /// </summary>
        public List<string> QuestionIds { get; set; } = new List<string>();

        /// <summary>
        /// Số lần làm tối đa (null = không giới hạn)
        /// </summary>
        [Range(1, 10, ErrorMessage = "Số lần làm tối đa phải từ 1 đến 10")]
        public int? MaxAttempts { get; set; }

        /// <summary>
        /// Điểm đạt (null = không yêu cầu)
        /// </summary>
        [Range(0, 100, ErrorMessage = "Điểm đạt phải từ 0 đến 100")]
        public double? PassingScore { get; set; }

        /// <summary>
        /// Đã xuất bản (sinh viên có thể thấy)
        /// </summary>
        public bool IsPublished { get; set; } = false;

        /// <summary>
        /// Đã khóa (không thể chỉnh sửa)
        /// </summary>
        public bool IsLocked { get; set; } = false;

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
        /// Tên giảng viên (để hiển thị, không lưu DB)
        /// </summary>
        public string? TeacherName { get; set; }

        /// <summary>
        /// Số lượng câu hỏi (để hiển thị, không lưu DB)
        /// </summary>
        public int QuestionCount => QuestionIds?.Count ?? 0;

        /// <summary>
        /// Tổng điểm (để hiển thị, không lưu DB)
        /// </summary>
        public double TotalScore { get; set; }
    }

    /// <summary>
    /// ViewModel để tạo đề thi mới
    /// </summary>
    public class CreateExamViewModel
    {
        [Required(ErrorMessage = "Tiêu đề đề thi là bắt buộc")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thời gian làm bài là bắt buộc")]
        [Range(1, 300, ErrorMessage = "Thời gian làm bài phải từ 1 đến 300 phút")]
        public int Duration { get; set; } = 60;

        [Range(1, 10, ErrorMessage = "Số lần làm tối đa phải từ 1 đến 10")]
        public int? MaxAttempts { get; set; }

        [Range(0, 100, ErrorMessage = "Điểm đạt phải từ 0 đến 100")]
        public double? PassingScore { get; set; }
    }

    /// <summary>
    /// ViewModel để chỉnh sửa đề thi
    /// </summary>
    public class EditExamViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ApiId là bắt buộc")]
        public string? ApiId { get; set; }

        [Required(ErrorMessage = "Tiêu đề đề thi là bắt buộc")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thời gian làm bài là bắt buộc")]
        [Range(1, 300, ErrorMessage = "Thời gian làm bài phải từ 1 đến 300 phút")]
        public int Duration { get; set; }

        [Range(1, 10, ErrorMessage = "Số lần làm tối đa phải từ 1 đến 10")]
        public int? MaxAttempts { get; set; }

        [Range(0, 100, ErrorMessage = "Điểm đạt phải từ 0 đến 100")]
        public double? PassingScore { get; set; }

        public bool IsPublished { get; set; }

        public bool IsLocked { get; set; }
    }

    /// <summary>
    /// ViewModel để xây dựng đề thi (chọn câu hỏi)
    /// </summary>
    public class ExamBuilderViewModel
    {
        public string ExamId { get; set; } = string.Empty;
        public string ExamTitle { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public List<string> SelectedQuestionIds { get; set; } = new List<string>();
        public List<Question> AvailableQuestions { get; set; } = new List<Question>();
        public List<Question> SelectedQuestions { get; set; } = new List<Question>();
        public double TotalScore { get; set; }
    }

    /// <summary>
    /// Response model cho exam statistics
    /// </summary>
    public class ExamStatistics
    {
        public int Total { get; set; }
        public int Published { get; set; }
        public int Locked { get; set; }
        public List<SubjectDistribution> SubjectDistribution { get; set; } = new List<SubjectDistribution>();
    }

    public class SubjectDistribution
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
