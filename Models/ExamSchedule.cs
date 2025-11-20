using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_TestHub.Models
{
    /// <summary>
    /// Model lịch thi - gán đề thi cho lớp học
    /// </summary>
    public class ExamSchedule
    {
        /// <summary>
        /// ID nội bộ (C#) - auto-increment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// MongoDB ObjectId (24-char hex string)
        /// </summary>
        [JsonPropertyName("_id")]
        public string? ApiId { get; set; }

        /// <summary>
        /// ID đề thi
        /// </summary>
        [Required(ErrorMessage = "Đề thi là bắt buộc")]
        public string ExamId { get; set; } = string.Empty;

        /// <summary>
        /// ID lớp học
        /// </summary>
        [Required(ErrorMessage = "Lớp học là bắt buộc")]
        public string ClassId { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        [Required(ErrorMessage = "Thời gian bắt đầu là bắt buộc")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        [Required(ErrorMessage = "Thời gian kết thúc là bắt buộc")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Đã đóng (không cho làm bài nữa)
        /// </summary>
        public bool IsClosed { get; set; } = false;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties (not stored in DB)
        public Exam? Exam { get; set; }
        public Class? Class { get; set; }
    }

    /// <summary>
    /// ViewModel để gán đề thi cho lớp học
    /// </summary>
    public class AssignExamViewModel
    {
        [Required(ErrorMessage = "Đề thi là bắt buộc")]
        public string ExamId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lớp học là bắt buộc")]
        public string ClassId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan StartTimeOfDay { get; set; } = new TimeSpan(8, 0, 0);

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "Giờ kết thúc là bắt buộc")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan EndTimeOfDay { get; set; } = new TimeSpan(23, 59, 0);

        // For display
        public string? ExamTitle { get; set; }
        public string? ClassName { get; set; }
    }
}
