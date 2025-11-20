using E_TestHub.Models;
using System.Text;
using System.Text.Json;

namespace E_TestHub.Services
{
    public interface IClassApiService
    {
        Task<List<Class>> GetAllClassesAsync();
        Task<Class?> GetClassByIdAsync(string classId);
        Task<Class?> GetClassByCodeAsync(string classCode);
        Task<Class?> CreateClassAsync(CreateClassViewModel model);
        Task<bool> UpdateClassAsync(EditClassViewModel model);
        Task<bool> DeleteClassAsync(string classId);
        Task<bool> AddStudentsToClassAsync(string classId, List<string> studentIds);
        Task<bool> RemoveStudentFromClassAsync(string classId, string studentId);
        Task<bool> CheckClassCodeExistsAsync(string classCode, string? excludeId = null);
    }

    public class ClassApiService : IClassApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ClassApiService> _logger;
        private readonly string _baseUrl = "classes"; // No leading slash - ApiService BaseAddress already has /api

        public ClassApiService(IApiService apiService, ILogger<ClassApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            try
            {
                var apiResponse = await _apiService.GetAsync<List<ClassApiResponse>>(_baseUrl);
                
                return apiResponse?.Select(MapToClass).ToList() ?? new List<Class>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all classes");
                return new List<Class>();
            }
        }

        public async Task<Class?> GetClassByIdAsync(string classId)
        {
            try
            {
                var apiResponse = await _apiService.GetAsync<ClassApiResponse>($"{_baseUrl}/{classId}");
                return apiResponse != null ? MapToClass(apiResponse) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting class {classId}");
                return null;
            }
        }

        public async Task<Class?> GetClassByCodeAsync(string classCode)
        {
            try
            {
                var apiResponse = await _apiService.GetAsync<ClassApiResponse>($"{_baseUrl}/code/{classCode}");
                return apiResponse != null ? MapToClass(apiResponse) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting class with code {classCode}");
                return null;
            }
        }

        public async Task<Class?> CreateClassAsync(CreateClassViewModel model)
        {
            try
            {
                var request = new ClassCreateRequest
                {
                    name = model.Name,
                    classCode = model.ClassCode.ToUpper(),
                    teacherId = model.TeacherId,
                    courseId = model.CourseId,
                    academicYear = model.AcademicYear,
                    students = model.SelectedStudents,
                    courses = new List<string> { model.CourseId }
                };
                
                var apiResponse = await _apiService.PostAsync<ClassApiResponse>(_baseUrl, request);
                return apiResponse != null ? MapToClass(apiResponse) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class");
                return null;
            }
        }

        public async Task<bool> UpdateClassAsync(EditClassViewModel model)
        {
            try
            {
                var updateData = new
                {
                    name = model.Name,
                    classCode = model.ClassCode.ToUpper(),
                    teacherId = model.TeacherId,
                    courseId = model.CourseId,
                    academicYear = model.AcademicYear,
                    students = model.SelectedStudents,
                    courses = new List<string> { model.CourseId }
                };
                
                var response = await _apiService.PutAsync<ClassApiResponse>(_baseUrl, model.Id, updateData);
                return response != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating class {model.Id}");
                return false;
            }
        }

        public async Task<bool> DeleteClassAsync(string classId)
        {
            try
            {
                return await _apiService.DeleteAsync(_baseUrl, classId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting class {classId}");
                return false;
            }
        }

        public async Task<bool> AddStudentsToClassAsync(string classId, List<string> studentIds)
        {
            try
            {
                var request = new { studentIds };
                var response = await _apiService.PostAsync<object>($"{_baseUrl}/{classId}/students", request);
                return response != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding students to class {classId}");
                return false;
            }
        }

        public async Task<bool> RemoveStudentFromClassAsync(string classId, string studentId)
        {
            try
            {
                return await _apiService.DeleteAsync($"{_baseUrl}/{classId}/students", studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing student {studentId} from class {classId}");
                return false;
            }
        }

        public async Task<bool> CheckClassCodeExistsAsync(string classCode, string? excludeId = null)
        {
            try
            {
                var existingClass = await GetClassByCodeAsync(classCode);
                
                if (existingClass == null)
                    return false;
                
                if (!string.IsNullOrEmpty(excludeId) && existingClass.Id == excludeId)
                    return false;
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking class code existence");
                return false;
            }
        }

        // Helper method to map API response to Class model
        private Class MapToClass(ClassApiResponse apiResponse)
        {
            return new Class
            {
                Id = apiResponse._id,
                Name = apiResponse.name,
                ClassCode = apiResponse.classCode,
                TeacherId = apiResponse.teacherId,
                CourseId = apiResponse.courseId,
                AcademicYear = apiResponse.academicYear,
                Students = apiResponse.students ?? new List<string>(),
                Courses = apiResponse.courses ?? new List<string>(),
                CreatedAt = apiResponse.createdAt ?? DateTime.Now,
                UpdatedAt = apiResponse.updatedAt
            };
        }
    }
}
