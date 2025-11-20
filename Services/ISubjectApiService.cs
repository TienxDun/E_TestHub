using E_TestHub.Models;

namespace E_TestHub.Services
{
    public interface ISubjectApiService
    {
        Task<List<Subject>> GetAllSubjectsAsync();
        Task<Subject?> GetSubjectByIdAsync(string subjectId);
        Task<Subject?> GetSubjectByCodeAsync(string code);
        Task<bool> CreateSubjectAsync(Subject subject);
        Task<bool> UpdateSubjectAsync(Subject subject);
        Task<bool> DeleteSubjectAsync(string subjectId);
        Task<bool> CheckSubjectCodeExistsAsync(string code, string? excludeId = null);
    }

    public class SubjectApiService : ISubjectApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SubjectApiService> _logger;

        public SubjectApiService(IApiService apiService, ILogger<SubjectApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all subjects from API");
                
                var response = await _apiService.GetAsync<List<SubjectApiResponse>>("subjects");

                if (response != null && response.Count > 0)
                {
                    return response.Select(MapToSubject).ToList();
                }

                _logger.LogWarning("No subjects found or API returned empty response");
                return new List<Subject>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subjects");
                return new List<Subject>();
            }
        }

        public async Task<Subject?> GetSubjectByIdAsync(string subjectId)
        {
            try
            {
                _logger.LogInformation($"Getting subject by ID: {subjectId}");
                
                var response = await _apiService.GetAsync<SubjectApiResponse>($"subjects/{subjectId}");

                if (response != null)
                {
                    return MapToSubject(response);
                }

                _logger.LogWarning($"Subject not found: {subjectId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subject by ID: {subjectId}");
                return null;
            }
        }

        public async Task<Subject?> GetSubjectByCodeAsync(string code)
        {
            try
            {
                _logger.LogInformation($"Getting subject by code: {code}");
                
                // Get all subjects and filter by code (since API may not have endpoint for code lookup)
                var allSubjects = await GetAllSubjectsAsync();
                return allSubjects.FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subject by code: {code}");
                return null;
            }
        }

        public async Task<bool> CreateSubjectAsync(Subject subject)
        {
            try
            {
                _logger.LogInformation($"Creating subject: {subject.Name} ({subject.Code})");

                var request = new SubjectCreateRequest
                {
                    name = subject.Name,
                    code = subject.Code.ToUpper(),
                    description = subject.Description
                };

                var response = await _apiService.PostAsync<SubjectApiResponse>("subjects", request);

                if (response != null)
                {
                    _logger.LogInformation($"Subject created successfully: {subject.Code}");
                    return true;
                }

                _logger.LogWarning($"Failed to create subject: {subject.Code}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating subject: {subject.Code}");
                return false;
            }
        }

        public async Task<bool> UpdateSubjectAsync(Subject subject)
        {
            try
            {
                _logger.LogInformation($"Updating subject: {subject.Id}");

                var updateData = new
                {
                    name = subject.Name,
                    code = subject.Code.ToUpper(),
                    description = subject.Description
                };

                var response = await _apiService.PutAsync<SubjectApiResponse>("subjects", subject.Id, updateData);

                if (response != null)
                {
                    _logger.LogInformation($"Subject updated successfully: {subject.Id}");
                    return true;
                }

                _logger.LogWarning($"Failed to update subject: {subject.Id}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subject: {subject.Id}");
                return false;
            }
        }

        public async Task<bool> DeleteSubjectAsync(string subjectId)
        {
            try
            {
                _logger.LogInformation($"Deleting subject: {subjectId}");

                var result = await _apiService.DeleteAsync("subjects", subjectId);

                if (result)
                {
                    _logger.LogInformation($"Subject deleted successfully: {subjectId}");
                }
                else
                {
                    _logger.LogWarning($"Failed to delete subject: {subjectId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subject: {subjectId}");
                return false;
            }
        }

        public async Task<bool> CheckSubjectCodeExistsAsync(string code, string? excludeId = null)
        {
            try
            {
                var allSubjects = await GetAllSubjectsAsync();
                
                if (string.IsNullOrEmpty(excludeId))
                {
                    return allSubjects.Any(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    return allSubjects.Any(s => 
                        s.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && 
                        s.Id != excludeId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking subject code exists: {code}");
                return false;
            }
        }

        // Helper method to map API response to Subject model
        private Subject MapToSubject(SubjectApiResponse apiResponse)
        {
            return new Subject
            {
                Id = apiResponse._id,
                Name = apiResponse.name,
                Code = apiResponse.code,
                Description = apiResponse.description,
                CreatedAt = apiResponse.createdAt ?? DateTime.Now,
                UpdatedAt = apiResponse.updatedAt
            };
        }
    }
}
