using System;
using System.Collections.Generic;
using System.Linq;

namespace E_TestHub.Models
{
    public static class AdminData
    {
        // Static list - kept as fallback structure when API is unavailable
        // Data now comes from MongoDB API (IUserApiService)
        private static List<User> _users = new List<User>();

        // User Management Data - Cấu trúc tương thích với MongoDB schema
        public static List<User> GetUsers()
        {
            return _users.ToList();
        }

        public static User? GetUserById(int id)
        {
            return GetUsers().FirstOrDefault(u => u.Id == id);
        }

        public static List<User> GetUsersByRole(UserRole role)
        {
            return GetUsers().Where(u => u.Role == role).ToList();
        }

        public static List<User> GetActiveUsers()
        {
            return GetUsers().Where(u => u.IsActive).ToList();
        }

        public static List<User> GetInactiveUsers()
        {
            return GetUsers().Where(u => !u.IsActive).ToList();
        }

        public static List<User> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetUsers();

            return GetUsers().Where(u =>
                u.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (u.StudentId != null && u.StudentId.Contains(searchTerm)) ||
                (u.EmployeeId != null && u.EmployeeId.Contains(searchTerm)) ||
                u.Id.ToString().Contains(searchTerm)
            ).ToList();
        }

        // Add new user method
        public static bool AddUser(User newUser)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.FullName))
                return false;

            // Check for email duplicates
            if (_users.Any(u => u.Email.ToLower() == newUser.Email.ToLower()))
                return false;

            // Check for role-specific duplicates
            if (newUser.Role == UserRole.Student && !string.IsNullOrWhiteSpace(newUser.StudentId))
            {
                if (_users.Any(u => u.StudentId == newUser.StudentId))
                    return false;
            }

            if ((newUser.Role == UserRole.Teacher || newUser.Role == UserRole.Admin) && !string.IsNullOrWhiteSpace(newUser.EmployeeId))
            {
                if (_users.Any(u => u.EmployeeId == newUser.EmployeeId))
                    return false;
            }

            // Generate new ID (handle empty list)
            newUser.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            newUser.CreatedDate = DateTime.Now;
            newUser.IsActive = true;

            // Add to the list
            _users.Add(newUser);

            return true;
        }

        // Update user method
        public static bool UpdateUser(User updatedUser)
        {
            // Validation
            if (updatedUser == null || updatedUser.Id <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(updatedUser.Email) ||
                string.IsNullOrWhiteSpace(updatedUser.FullName))
                return false;

            // Find existing user
            var existingUser = _users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser == null)
                return false;

            // Check for email duplicates (excluding current user)
            if (_users.Any(u => u.Id != updatedUser.Id && u.Email.ToLower() == updatedUser.Email.ToLower()))
                return false;

            // Update user properties
            existingUser.Email = updatedUser.Email;
            existingUser.FullName = updatedUser.FullName;
            existingUser.Role = updatedUser.Role;
            existingUser.IsActive = updatedUser.IsActive;
            existingUser.StudentId = updatedUser.StudentId;
            existingUser.Class = updatedUser.Class;
            existingUser.EmployeeId = updatedUser.EmployeeId;
            existingUser.Department = updatedUser.Department;
            existingUser.Position = updatedUser.Position;

            return true;
        }

        // Delete user method (soft delete - set IsActive to false)
        public static bool DeleteUser(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false;

            // Soft delete
            user.IsActive = false;
            return true;
        }

        // Hard delete user method (permanently remove from list)
        public static bool HardDeleteUser(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false;

            _users.Remove(user);
            return true;
        }

        // Clear all users (for testing purposes)
        public static void ClearAllUsers()
        {
            _users.Clear();
        }

        // System Statistics
        public static SystemStats GetSystemStats()
        {
            var users = GetUsers();
            return new SystemStats
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive),
                InactiveUsers = users.Count(u => !u.IsActive),
                StudentsCount = users.Count(u => u.Role == UserRole.Student),
                TeachersCount = users.Count(u => u.Role == UserRole.Teacher),
                AdminsCount = users.Count(u => u.Role == UserRole.Admin),
                TotalExams = 45, // Placeholder value - will be replaced with actual exam count
                CompletedExams = 32, // Placeholder value - will be replaced with actual completed exam count
                LastUpdated = DateTime.Now
            };
        }
    }

    public class SystemStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int AdminsCount { get; set; }
        public int TotalExams { get; set; }
        public int CompletedExams { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
