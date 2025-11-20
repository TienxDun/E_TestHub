using E_TestHub.Models;
using System.Text.Json;

namespace E_TestHub.Services
{
    public interface IExamScheduleApiService
    {
        Task<List<ExamSchedule>> GetAllSchedulesAsync();
        Task<List<ExamSchedule>> GetSchedulesByClassIdAsync(string classId);
        Task<List<ExamSchedule>> GetSchedulesByExamIdAsync(string examId);
        Task<ExamSchedule?> GetScheduleByIdAsync(string id);
        Task<bool> CreateScheduleAsync(ExamSchedule schedule);
        Task<bool> UpdateScheduleAsync(ExamSchedule schedule);
        Task<bool> DeleteScheduleAsync(string id);
        Task<bool> CloseScheduleAsync(string id);
    }

    public class ExamScheduleApiService : IExamScheduleApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ExamScheduleApiService> _logger;

        public ExamScheduleApiService(IApiService apiService, ILogger<ExamScheduleApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<List<ExamSchedule>> GetAllSchedulesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all exam schedules from API");
                var response = await _apiService.GetAsync<List<ExamSchedule>>("schedules");
                
                if (response != null)
                {
                    _logger.LogInformation($"Successfully fetched {response.Count} exam schedules");
                }
                
                return response ?? new List<ExamSchedule>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting exam schedules: {ex.Message}");
                return new List<ExamSchedule>();
            }
        }

        public async Task<List<ExamSchedule>> GetSchedulesByClassIdAsync(string classId)
        {
            try
            {
                // API doesn't support filter yet, so get all and filter client-side
                var allSchedules = await GetAllSchedulesAsync();
                return allSchedules.Where(s => s.ClassId == classId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting schedules for class {classId}: {ex.Message}");
                return new List<ExamSchedule>();
            }
        }

        public async Task<List<ExamSchedule>> GetSchedulesByExamIdAsync(string examId)
        {
            try
            {
                // API doesn't support filter yet, so get all and filter client-side
                var allSchedules = await GetAllSchedulesAsync();
                return allSchedules.Where(s => s.ExamId == examId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting schedules for exam {examId}: {ex.Message}");
                return new List<ExamSchedule>();
            }
        }

        public async Task<ExamSchedule?> GetScheduleByIdAsync(string id)
        {
            try
            {
                var schedule = await _apiService.GetAsync<ExamSchedule>($"schedules/{id}");
                if (schedule != null && string.IsNullOrEmpty(schedule.ApiId))
                {
                    schedule.ApiId = id;
                }
                return schedule;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting schedule {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateScheduleAsync(ExamSchedule schedule)
        {
            try
            {
                var requestBody = new
                {
                    examId = schedule.ExamId,
                    classId = schedule.ClassId,
                    startTime = schedule.StartTime,
                    endTime = schedule.EndTime,
                    isClosed = schedule.IsClosed
                };

                _logger.LogInformation($"Creating exam schedule - ExamId: {schedule.ExamId}, ClassId: {schedule.ClassId}");
                var response = await _apiService.PostAsync<ExamSchedule>("schedules", requestBody);

                if (response != null)
                {
                    _logger.LogInformation("Exam schedule created successfully");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating exam schedule: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateScheduleAsync(ExamSchedule schedule)
        {
            try
            {
                if (string.IsNullOrEmpty(schedule.ApiId))
                {
                    _logger.LogWarning("Cannot update schedule: ApiId is null or empty");
                    return false;
                }

                var requestBody = new
                {
                    examId = schedule.ExamId,
                    classId = schedule.ClassId,
                    startTime = schedule.StartTime,
                    endTime = schedule.EndTime,
                    isClosed = schedule.IsClosed
                };

                var response = await _apiService.PutAsync<ExamSchedule>("schedules", schedule.ApiId, requestBody);

                if (response != null)
                {
                    _logger.LogInformation($"Exam schedule {schedule.ApiId} updated successfully");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating exam schedule: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteScheduleAsync(string id)
        {
            try
            {
                var result = await _apiService.DeleteAsync("schedules", id);
                if (result)
                {
                    _logger.LogInformation($"Exam schedule {id} deleted successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting exam schedule {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CloseScheduleAsync(string id)
        {
            try
            {
                var requestBody = new { isClosed = true };

                var response = await _apiService.PutAsync<ExamSchedule>("schedules", id, requestBody);

                if (response != null)
                {
                    _logger.LogInformation($"Exam schedule {id} closed successfully");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error closing exam schedule {id}: {ex.Message}");
                return false;
            }
        }
    }
}
