namespace E_TestHub.Models
{
    // DTO cho request tạo user tới Node API (/api/users)
    public class UserCreateRequest
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        // student | teacher | admin
        public string role { get; set; } = string.Empty;
        public bool? isActive { get; set; }
        public string? classId { get; set; }
        public List<string>? teachingSubjects { get; set; }
    }

    // Response từ API
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    // DTO mapping cho User từ Node API MongoDB
    public class UserApiResponse
    {
        public string _id { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty; // student | teacher | admin
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public bool isActive { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string? classId { get; set; }
        public List<string>? teachingSubjects { get; set; }
    }

    // DTO response khi đăng nhập từ Node API: { token, user: {...} }
    public class LoginResponse
    {
        public string token { get; set; } = string.Empty;
        public UserApiResponse user { get; set; } = new UserApiResponse();
    }

    // Model cho API Exam nếu cần
    public class ExamApiResponse
    {
        public string _id { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public int duration { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isActive { get; set; }
    }

    // DTO for API Response from Node.js - Class
    public class ClassApiResponse
    {
        public string _id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string classCode { get; set; } = string.Empty;
        public string teacherId { get; set; } = string.Empty;
        public string courseId { get; set; } = string.Empty;
        public string academicYear { get; set; } = string.Empty;
        public List<string>? students { get; set; }
        public List<string>? courses { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    // DTO for Import Request with edited questions
    public class ImportRequest
    {
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}

