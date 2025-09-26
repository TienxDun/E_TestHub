namespace E_TestHub.Models
{
    public enum UserRole
    {
        Student = 1,
        Teacher = 2,
        Admin = 3
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }
        
        // Student specific properties
        public string? StudentId { get; set; }
        public string? Class { get; set; }
        
        // Teacher specific properties
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }
        
        // Admin specific properties
        public string? Position { get; set; }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; } = string.Empty;
    }

    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string RoleDisplayName => UserRole switch
        {
            UserRole.Student => "Sinh Viên",
            UserRole.Teacher => "Giáo Viên",
            UserRole.Admin => "Quản Trị Viên",
            _ => "Không xác định"
        };
    }
}