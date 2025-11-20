using E_TestHub.Models;
using System.Text.Json;

namespace E_TestHub.Services
{
    /// <summary>
    /// Implementation of Submission API Service
    /// Handles all submission-related operations with MongoDB backend
    /// </summary>
    public class SubmissionApiService : ISubmissionApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SubmissionApiService> _logger;
        private const string SUBMISSIONS_ENDPOINT = "submissions";

        public SubmissionApiService(IApiService apiService, ILogger<SubmissionApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Start a new exam attempt for a student
        /// Creates a new submission record in the database
        /// </summary>
        public async Task<Submission?> StartExamAsync(string examId, string studentId)
        {
            try
            {
                _logger.LogInformation($"Starting exam {examId} for student {studentId}");

                var requestData = new
                {
                    examId,
                    studentId,
                    userId = studentId, // Both fields for compatibility
                    answers = new List<object>(),
                    score = 0.0,
                    status = "pending",
                    isGraded = false,
                    submittedAt = (DateTime?)null
                };

                var response = await _apiService.PostAsync<Submission>(SUBMISSIONS_ENDPOINT, requestData);
                
                if (response != null)
                {
                    _logger.LogInformation($"API Response deserialized - ApiId: '{response.ApiId}', UserId: '{response.UserId}', ExamId: '{response.ExamId}', Status: '{response.Status}'");
                    
                    // Map MongoDB _id to ApiId if not already set
                    if (string.IsNullOrEmpty(response.ApiId))
                    {
                        _logger.LogError("CRITICAL: Response ApiId is empty! Check JsonPropertyName mapping for _id field");
                        return null;
                    }

                    _logger.LogInformation($"Successfully started exam {examId} for student {studentId}. Submission ID: {response.ApiId}");
                    return response;
                }

                _logger.LogWarning($"Failed to start exam {examId} for student {studentId} - API returned null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting exam {examId} for student {studentId}: {ex.Message}");
                return null; // Don't throw, return null
            }
        }

        /// <summary>
        /// Get a specific submission by ID
        /// </summary>
        public async Task<Submission?> GetSubmissionByIdAsync(string submissionId)
        {
            try
            {
                _logger.LogInformation($"Fetching submission {submissionId}");

                var submission = await _apiService.GetAsync<Submission>($"{SUBMISSIONS_ENDPOINT}/{submissionId}");
                
                if (submission != null)
                {
                    // Ensure ApiId is populated
                    if (string.IsNullOrEmpty(submission.ApiId))
                    {
                        submission.ApiId = submissionId;
                    }
                    
                    _logger.LogInformation($"Successfully fetched submission {submissionId}");
                }
                else
                {
                    _logger.LogWarning($"Submission {submissionId} not found");
                }

                return submission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting submission {submissionId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Save answers during exam (auto-save functionality)
        /// Updates the submission without marking as submitted
        /// </summary>
        public async Task<Submission?> SaveAnswersAsync(string submissionId, List<SubmissionAnswer> answers)
        {
            try
            {
                _logger.LogInformation($"Saving {answers.Count} answers for submission {submissionId}");

                var requestData = new
                {
                    answers = answers.Select(a => new
                    {
                        questionId = a.QuestionId,
                        selectedOption = a.SelectedOption,
                        score = a.Score
                    }).ToList()
                };

                var response = await _apiService.PutAsync<Submission>(SUBMISSIONS_ENDPOINT, submissionId, requestData);
                
                if (response != null)
                {
                    _logger.LogInformation($"Successfully saved answers for submission {submissionId}. Total answers: {answers.Count}");
                    return response;
                }

                _logger.LogWarning($"Failed to save answers for submission {submissionId} - API returned null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving answers for submission {submissionId}: {ex.Message}");
                throw new ApplicationException($"Failed to save answers: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Submit final exam with all answers
        /// Marks submission as completed and calculates score
        /// </summary>
        public async Task<Submission?> SubmitExamAsync(string submissionId, List<SubmissionAnswer> answers)
        {
            try
            {
                _logger.LogInformation($"Submitting exam for submission {submissionId} with {answers.Count} answers");

                var requestData = new
                {
                    answers = answers.Select(a => new
                    {
                        questionId = a.QuestionId,
                        selectedOption = a.SelectedOption,
                        score = a.Score
                    }).ToList(),
                    status = "graded",
                    isGraded = true,
                    submittedAt = DateTime.UtcNow
                };

                var response = await _apiService.PutAsync<Submission>(SUBMISSIONS_ENDPOINT, submissionId, requestData);
                
                if (response != null)
                {
                    _logger.LogInformation($"Successfully submitted exam for submission {submissionId}. Score: {response.Score}");
                    return response;
                }

                _logger.LogWarning($"Failed to submit exam for submission {submissionId} - API returned null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error submitting exam for submission {submissionId}: {ex.Message}");
                throw new ApplicationException($"Failed to submit exam: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all submissions for a specific student
        /// </summary>
        public async Task<List<Submission>> GetStudentSubmissionsAsync(string studentId)
        {
            try
            {
                _logger.LogInformation($"Fetching all submissions for student {studentId}");

                var submissions = await _apiService.GetAsync<List<Submission>>($"{SUBMISSIONS_ENDPOINT}/user/{studentId}");
                
                if (submissions != null && submissions.Any())
                {
                    _logger.LogInformation($"Successfully fetched {submissions.Count} submissions for student {studentId}");
                    return submissions;
                }

                _logger.LogInformation($"No submissions found for student {studentId}");
                return new List<Submission>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting submissions for student {studentId}: {ex.Message}");
                return new List<Submission>();
            }
        }

        /// <summary>
        /// Get a specific student's submission for a specific exam
        /// </summary>
        public async Task<Submission?> GetExamSubmissionAsync(string examId, string studentId)
        {
            try
            {
                _logger.LogInformation($"Fetching submission for exam {examId} and student {studentId}");

                // Get all submissions for this exam
                var allSubmissions = await _apiService.GetAsync<List<Submission>>($"{SUBMISSIONS_ENDPOINT}/exam/{examId}");
                
                if (allSubmissions != null && allSubmissions.Any())
                {
                    // Find submission for this student (check both StudentId and UserId for compatibility)
                    var studentSubmission = allSubmissions.FirstOrDefault(s => 
                        s.StudentId == studentId || s.UserId == studentId);
                    
                    if (studentSubmission != null)
                    {
                        _logger.LogInformation($"Found submission for exam {examId} and student {studentId}");
                        return studentSubmission;
                    }
                }

                _logger.LogInformation($"No submission found for exam {examId} and student {studentId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting exam submission for exam {examId} and student {studentId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check if a student has already submitted a specific exam
        /// </summary>
        public async Task<bool> HasSubmittedExamAsync(string examId, string studentId)
        {
            try
            {
                _logger.LogInformation($"Checking if student {studentId} has submitted exam {examId}");

                var submission = await GetExamSubmissionAsync(examId, studentId);
                
                bool hasSubmitted = submission != null && submission.IsGraded;
                
                _logger.LogInformation($"Student {studentId} has{(hasSubmitted ? "" : " not")} submitted exam {examId}");
                
                return hasSubmitted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking submission status for exam {examId} and student {studentId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all submissions for a specific exam (useful for teachers)
        /// </summary>
        public async Task<List<Submission>> GetExamSubmissionsAsync(string examId)
        {
            try
            {
                _logger.LogInformation($"Fetching all submissions for exam {examId}");

                var submissions = await _apiService.GetAsync<List<Submission>>($"{SUBMISSIONS_ENDPOINT}/exam/{examId}");
                
                if (submissions != null && submissions.Any())
                {
                    _logger.LogInformation($"Successfully fetched {submissions.Count} submissions for exam {examId}");
                    return submissions;
                }

                _logger.LogInformation($"No submissions found for exam {examId}");
                return new List<Submission>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting submissions for exam {examId}: {ex.Message}");
                return new List<Submission>();
            }
        }

        /// <summary>
        /// Get submission statistics (if backend supports it)
        /// </summary>
        public async Task<object?> GetSubmissionStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching submission statistics");

                var stats = await _apiService.GetAsync<object>($"{SUBMISSIONS_ENDPOINT}/statistics");
                
                if (stats != null)
                {
                    _logger.LogInformation("Successfully fetched submission statistics");
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting submission statistics: {ex.Message}");
                return null;
            }
        }
        
        public async Task<List<Submission>> GetAllSubmissionsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all submissions");

                var submissions = await _apiService.GetAsync<List<Submission>>(SUBMISSIONS_ENDPOINT);
                
                if (submissions != null && submissions.Any())
                {
                    _logger.LogInformation($"Successfully fetched {submissions.Count} submissions");
                    return submissions;
                }

                _logger.LogWarning("No submissions found");
                return new List<Submission>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all submissions: {ex.Message}");
                return new List<Submission>();
            }
        }
    }
}
