using E_TestHub.Models;

namespace E_TestHub.Services
{
    public interface IUserApiService
    {
        Task<(User? user, string? token)> AuthenticateAsync(string email, string password);
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<Dictionary<string, object>> GetUserStatisticsAsync();
    }

    public class UserApiService : IUserApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(IApiService apiService, ILogger<UserApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<(User? user, string? token)> AuthenticateAsync(string email, string password)
        {
            try
            {
                // Gọi API login: POST /api/auth/login -> { token, user }
                var loginData = new { email, password };
                _logger.LogInformation($"Attempting authentication for: {email}");
                
                var response = await _apiService.PostAsync<LoginResponse>("auth/login", loginData);

                if (response != null && response.user != null)
                {
                    _logger.LogInformation($"Response received - Token: {response.token?.Substring(0, 10)}..., User role: {response.user.role}");
                    
                    // CRITICAL: Set token for subsequent API requests in this scope
                    if (!string.IsNullOrEmpty(response.token))
                    {
                        _apiService.SetAuthToken(response.token);
                        _logger.LogInformation("Auth token saved for future API requests");
                    }
                    
                    var user = MapToUser(response.user);
                    _logger.LogInformation($"User authenticated successfully: {email}, Role: {user.Role}");
                    
                    // Return both user and token so caller can save token in Session
                    return (user, response.token);
                }

                _logger.LogWarning($"Authentication failed - Response null or user null for: {email}");
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating user: {email}");
                return (null, null);
            }
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            try
            {
                // Gọi API: GET /api/users/:id
                var response = await _apiService.GetAsync<UserApiResponse>($"users/{userId}");
                return response != null ? MapToUser(response) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by ID: {userId}");
                return null;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                // Node API không có endpoint get-by-email mặc định -> lấy toàn bộ và lọc
                var all = await GetAllUsersAsync();
                return all.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by email: {email}");
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {
                // Map User sang UserCreateRequest theo Node API
                var names = SplitFullName(user.FullName);
                var req = new UserCreateRequest
                {
                    email = user.Email,
                    password = password,
                    firstName = names.firstName,
                    lastName = names.lastName,
                    role = MapRoleToApi(user.Role),
                    isActive = user.IsActive
                };

                // Gọi API: POST /api/users
                var response = await _apiService.PostAsync<UserApiResponse>("users", req);
                
                if (response != null)
                {
                    _logger.LogInformation($"User created successfully: {user.Email}");
                    return true;
                }
                
                _logger.LogWarning($"Failed to create user: {user.Email}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user: {user.Email}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                // Validate ApiId (MongoDB ObjectId)
                if (string.IsNullOrEmpty(user.ApiId))
                {
                    _logger.LogError($"Cannot update user: ApiId is null or empty for user {user.Email}");
                    return false;
                }

                // Update các trường cơ bản theo schema Node API
                var names = SplitFullName(user.FullName);
                var updateBody = new
                {
                    firstName = names.firstName,
                    lastName = names.lastName,
                    email = user.Email,
                    role = MapRoleToApi(user.Role),
                    isActive = user.IsActive
                };

                // Gọi API: PUT /api/users/:id (use MongoDB ObjectId)
                var response = await _apiService.PutAsync<UserApiResponse>("users", user.ApiId, updateBody);
                
                if (response != null)
                {
                    _logger.LogInformation($"User updated successfully: {user.Email}");
                    return true;
                }
                
                _logger.LogWarning($"Failed to update user: {user.Email}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user: {user.Email}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                // Gọi API: DELETE /api/users/:id
                var result = await _apiService.DeleteAsync("users", userId);
                
                if (result)
                {
                    _logger.LogInformation($"User deleted successfully: {userId}");
                }
                else
                {
                    _logger.LogWarning($"Failed to delete user: {userId}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user: {userId}");
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                // Node API không có endpoint theo role mặc định -> lấy toàn bộ và lọc
                var all = await GetAllUsersAsync();
                return all.Where(u => u.Role == role).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting users by role: {role}");
                return new List<User>();
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                // Gọi API: GET /api/users
                var response = await _apiService.GetAsync<List<UserApiResponse>>("users");

                if (response != null)
                {
                    return response.Select(MapToUser).ToList();
                }

                return new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new List<User>();
            }
        }

        // Helper: role string -> enum
        private UserRole MapRoleFromApi(string apiRole)
        {
            return apiRole?.ToLower() switch
            {
                "admin" => UserRole.Admin,
                "teacher" => UserRole.Teacher,
                "student" => UserRole.Student,
                _ => UserRole.Student
            };
        }

        // Helper: enum -> role string
        private string MapRoleToApi(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "admin",
                UserRole.Teacher => "teacher",
                UserRole.Student => "student",
                _ => "student"
            };
        }

        private (string firstName, string lastName) SplitFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return ("", "");
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return (parts[0], "");
            var lastName = parts[^1];
            var firstName = string.Join(" ", parts.Take(parts.Length - 1));
            return (firstName, lastName);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                // Note: Backend API cần có endpoint PUT /api/users/:id/password
                // Với body: { currentPassword, newPassword }
                var changePasswordData = new
                {
                    currentPassword,
                    newPassword
                };

                var response = await _apiService.PutAsync<object>($"users/{userId}/password", userId.ToString(), changePasswordData);
                
                if (response != null)
                {
                    _logger.LogInformation($"Password changed successfully for user: {userId}");
                    return true;
                }
                
                _logger.LogWarning($"Failed to change password for user: {userId}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password for user: {userId}");
                return false;
            }
        }

        public async Task<Dictionary<string, object>> GetUserStatisticsAsync()
        {
            try
            {
                // Gọi API: GET /api/users/statistics
                var response = await _apiService.GetAsync<Dictionary<string, object>>("users/statistics");
                
                if (response != null)
                {
                    return response;
                }

                return new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                return new Dictionary<string, object>();
            }
        }

        // Helper method để map từ MongoDB response sang User model
        private User MapToUser(UserApiResponse apiResponse)
        {
            try
            {
                // Chuyển _id string thành int ổn định (hash)
                var id = string.IsNullOrEmpty(apiResponse._id) ? 0 : apiResponse._id.GetHashCode();

                return new User
                {
                    Id = id,
                    ApiId = apiResponse._id,
                    Email = apiResponse.email,
                    FullName = $"{apiResponse.firstName} {apiResponse.lastName}".Trim(),
                    Role = MapRoleFromApi(apiResponse.role),
                    IsActive = apiResponse.isActive,
                    CreatedDate = apiResponse.createdAt ?? DateTime.Now,
                    // Map Student specific properties
                    StudentCode = apiResponse._id, // Temporary: use _id as StudentCode if not provided
                    Class = apiResponse.classId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping API response to User");
                throw;
            }
        }
    }
}

