using System.ComponentModel.DataAnnotations;

namespace E_TestHub.Models
{
    // Class model matching MongoDB schema
    public class Class
    {
        public string Id { get; set; } = string.Empty; // MongoDB _id
        public string Name { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<string> Students { get; set; } = new List<string>(); // Student IDs
        public List<string> Courses { get; set; } = new List<string>(); // Course IDs
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties (populated from API)
        public User? Teacher { get; set; }
        public List<User> StudentList { get; set; } = new List<User>();
    }

    // DTO for Create Class Request
    public class ClassCreateRequest
    {
        public string name { get; set; } = string.Empty;
        public string classCode { get; set; } = string.Empty;
        public string teacherId { get; set; } = string.Empty;
        public string courseId { get; set; } = string.Empty;
        public string academicYear { get; set; } = string.Empty;
        public List<string>? students { get; set; }
        public List<string>? courses { get; set; }
    }

    // ViewModel for Create Class
    public class CreateClassViewModel
    {
        [Required(ErrorMessage = "Tên lớp học là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên lớp học phải từ 2 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp học là bắt buộc")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Mã lớp học phải từ 2 đến 20 ký tự")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Mã lớp học chỉ được chứa chữ in hoa và số")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        public string TeacherId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string CourseId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Năm học là bắt buộc")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Năm học phải có định dạng YYYY-YYYY (VD: 2024-2025)")]
        public string AcademicYear { get; set; } = string.Empty;

        // For multi-select
        public List<string> SelectedStudents { get; set; } = new List<string>();
    }

    // ViewModel for Edit Class
    public class EditClassViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên lớp học là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên lớp học phải từ 2 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp học là bắt buộc")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Mã lớp học phải từ 2 đến 20 ký tự")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Mã lớp học chỉ được chứa chữ in hoa và số")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        public string TeacherId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string CourseId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Năm học là bắt buộc")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Năm học phải có định dạng YYYY-YYYY (VD: 2024-2025)")]
        public string AcademicYear { get; set; } = string.Empty;

        public List<string> SelectedStudents { get; set; } = new List<string>();
    }

    // ViewModel for adding students to class
    public class AddStudentsViewModel
    {
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public List<string> StudentIds { get; set; } = new List<string>();
    }
}
