using System.ComponentModel.DataAnnotations;

namespace E_TestHub.Models
{
    // Subject model matching MongoDB schema
    public class Subject
    {
        public string Id { get; set; } = string.Empty; // MongoDB _id
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // DTO for API Response from Node.js
    public class SubjectApiResponse
    {
        public string _id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string? description { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    // DTO for Create Subject Request
    public class SubjectCreateRequest
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string? description { get; set; }
    }

    // ViewModel for Create Subject
    public class CreateSubjectViewModel
    {
        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên môn học phải từ 2 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã môn học là bắt buộc")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Mã môn học phải từ 2 đến 20 ký tự")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Mã môn học chỉ được chứa chữ in hoa và số")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
    }

    // ViewModel for Edit Subject
    public class EditSubjectViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên môn học phải từ 2 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã môn học là bắt buộc")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Mã môn học phải từ 2 đến 20 ký tự")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Mã môn học chỉ được chứa chữ in hoa và số")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
    }
}
