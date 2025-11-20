using E_TestHub.Models;

namespace E_TestHub.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<bool> ResetPasswordAsync(string email);
    }

    public class UserService : IUserService
    {
        private readonly IUserApiService _userApiService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserApiService userApiService, ILogger<UserService> logger)
        {
            _userApiService = userApiService;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var result = await _userApiService.AuthenticateAsync(email, password);
                if (result.user != null)
                {
                    _logger.LogInformation($"User {email} authenticated successfully");
                    return result.user;
                }
                
                _logger.LogWarning($"Authentication failed for user {email}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating user {email}");
                return null;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _userApiService.GetUserByIdAsync(userId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by ID {userId}");
                return null;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userApiService.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by email {email}");
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {
                return await _userApiService.CreateUserAsync(user, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user {user.Email}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                return await _userApiService.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {user.Id}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                return await _userApiService.DeleteUserAsync(userId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                return await _userApiService.GetUsersByRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting users by role {role}");
                return new List<User>();
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                // Get user first to validate email exists
                var user = await _userApiService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    // TODO: Implement proper password reset via API
                    // For now, return success (actual reset should be handled by API)
                    _logger.LogInformation($"Password reset requested for {email}");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for {email}");
                return false;
            }
        }
    }
}