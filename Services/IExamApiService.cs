using E_TestHub.Models;
using System.Text.Json;

namespace E_TestHub.Services
{
    /// <summary>
    /// Service để tương tác với Exam API
    /// </summary>
    public interface IExamApiService
    {
        Task<List<Exam>> GetAllExamsAsync();
        Task<Exam?> GetExamByIdAsync(string id);
        Task<List<Exam>> GetExamsByTeacherAsync(string teacherId);
        Task<List<Exam>> GetExamsBySubjectAsync(string subjectId);
        Task<Exam?> CreateExamAsync(Exam exam);
        Task<bool> UpdateExamAsync(Exam exam);
        Task<bool> DeleteExamAsync(string id);
        Task<bool> PublishExamAsync(string id);
        Task<bool> LockExamAsync(string id);
        Task<ExamStatistics?> GetExamStatisticsAsync();
    }

    public class ExamApiService : IExamApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ExamApiService> _logger;

        public ExamApiService(IApiService apiService, ILogger<ExamApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả đề thi
        /// </summary>
        public async Task<List<Exam>> GetAllExamsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all exams from API");
                var response = await _apiService.GetAsync<List<Exam>>("exams");
                
                if (response != null)
                {
                    _logger.LogInformation($"Successfully fetched {response.Count} exams");
                }
                
                return response ?? new List<Exam>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching exams: {ex.Message}");
                return new List<Exam>();
            }
        }

        /// <summary>
        /// Lấy đề thi theo ID
        /// </summary>
        public async Task<Exam?> GetExamByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Exam ID is null or empty");
                    return null;
                }

                _logger.LogInformation($"Fetching exam with ID: {id}");
                var exam = await _apiService.GetAsync<Exam>($"exams/{id}");
                
                if (exam != null && string.IsNullOrEmpty(exam.ApiId))
                {
                    exam.ApiId = id;
                }
                
                return exam;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching exam {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy đề thi theo giảng viên
        /// </summary>
        public async Task<List<Exam>> GetExamsByTeacherAsync(string teacherId)
        {
            try
            {
                if (string.IsNullOrEmpty(teacherId))
                {
                    _logger.LogWarning("Teacher ID is null or empty");
                    return new List<Exam>();
                }

                _logger.LogInformation($"Fetching exams for teacher: {teacherId}");
                
                // Get all exams and filter by teacherId
                var allExams = await GetAllExamsAsync();
                var filteredExams = allExams
                    .Where(e => e.TeacherId == teacherId)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList();
                
                _logger.LogInformation($"Found {filteredExams.Count} exams for teacher {teacherId}");
                return filteredExams;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching exams for teacher {teacherId}: {ex.Message}");
                return new List<Exam>();
            }
        }

        /// <summary>
        /// Lấy đề thi theo môn học
        /// </summary>
        public async Task<List<Exam>> GetExamsBySubjectAsync(string subjectId)
        {
            try
            {
                if (string.IsNullOrEmpty(subjectId))
                {
                    _logger.LogWarning("Subject ID is null or empty");
                    return new List<Exam>();
                }

                _logger.LogInformation($"Fetching exams for subject: {subjectId}");
                
                // Get all exams and filter by subjectId
                var allExams = await GetAllExamsAsync();
                var filteredExams = allExams
                    .Where(e => e.SubjectId == subjectId)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList();
                
                _logger.LogInformation($"Found {filteredExams.Count} exams for subject {subjectId}");
                return filteredExams;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching exams for subject {subjectId}: {ex.Message}");
                return new List<Exam>();
            }
        }

        /// <summary>
        /// Tạo đề thi mới
        /// </summary>
        public async Task<Exam?> CreateExamAsync(Exam exam)
        {
            try
            {
                _logger.LogInformation($"Creating new exam: {exam.Title}");
                
                // Prepare request body
                var requestBody = new
                {
                    title = exam.Title,
                    description = exam.Description,
                    subjectId = exam.SubjectId,
                    teacherId = exam.TeacherId,
                    duration = exam.Duration,
                    questionIds = exam.QuestionIds,
                    maxAttempts = exam.MaxAttempts,
                    passingScore = exam.PassingScore,
                    isPublished = exam.IsPublished,
                    isLocked = exam.IsLocked
                };

                var response = await _apiService.PostAsync<Exam>("exams", requestBody);
                
                if (response != null)
                {
                    _logger.LogInformation($"Exam created successfully with ID: {response.ApiId}");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating exam: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cập nhật đề thi
        /// </summary>
        public async Task<bool> UpdateExamAsync(Exam exam)
        {
            try
            {
                if (string.IsNullOrEmpty(exam.ApiId))
                {
                    _logger.LogWarning("Cannot update exam: ApiId is null or empty");
                    return false;
                }

                _logger.LogInformation($"Updating exam with ID: {exam.ApiId}");
                
                // Prepare request body
                var requestBody = new
                {
                    title = exam.Title,
                    description = exam.Description,
                    subjectId = exam.SubjectId,
                    duration = exam.Duration,
                    questionIds = exam.QuestionIds,
                    maxAttempts = exam.MaxAttempts,
                    passingScore = exam.PassingScore,
                    isPublished = exam.IsPublished,
                    isLocked = exam.IsLocked
                };

                var response = await _apiService.PutAsync<object>("exams", exam.ApiId, requestBody);
                
                if (response != null)
                {
                    _logger.LogInformation($"Exam {exam.ApiId} updated successfully");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating exam {exam.ApiId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa đề thi
        /// </summary>
        public async Task<bool> DeleteExamAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Cannot delete exam: ID is null or empty");
                    return false;
                }

                _logger.LogInformation($"Deleting exam with ID: {id}");
                var success = await _apiService.DeleteAsync("exams", id);
                
                if (success)
                {
                    _logger.LogInformation($"Exam {id} deleted successfully");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting exam {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xuất bản đề thi (sinh viên có thể thấy)
        /// </summary>
        public async Task<bool> PublishExamAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Cannot publish exam: ID is null or empty");
                    return false;
                }

                _logger.LogInformation($"Publishing exam with ID: {id}");
                
                // Update exam with isPublished = true
                var requestBody = new { isPublished = true };
                var response = await _apiService.PutAsync<object>("exams", id, requestBody);
                
                if (response != null)
                {
                    _logger.LogInformation($"Exam {id} published successfully");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing exam {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Khóa đề thi (không thể chỉnh sửa)
        /// </summary>
        public async Task<bool> LockExamAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Cannot lock exam: ID is null or empty");
                    return false;
                }

                _logger.LogInformation($"Locking exam with ID: {id}");
                
                // Update exam with isLocked = true
                var requestBody = new { isLocked = true };
                var response = await _apiService.PutAsync<object>("exams", id, requestBody);
                
                if (response != null)
                {
                    _logger.LogInformation($"Exam {id} locked successfully");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error locking exam {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy thống kê đề thi
        /// </summary>
        public async Task<ExamStatistics?> GetExamStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching exam statistics from API");
                var response = await _apiService.GetAsync<ExamStatistics>("exams/statistics");
                
                if (response != null)
                {
                    _logger.LogInformation($"Successfully fetched exam statistics");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching exam statistics: {ex.Message}");
                return null;
            }
        }
    }
}
