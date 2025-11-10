using E_TestHub.Models;
using System.Text.Json;

namespace E_TestHub.Services
{
    /// <summary>
    /// Service để tương tác với Question API
    /// </summary>
    public interface IQuestionApiService
    {
        Task<List<Question>> GetAllQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(string id);
        Task<List<Question>> GetQuestionsBySubjectAsync(string subjectId);
        Task<List<Question>> GetQuestionsByExamAsync(string examId);
        Task<Question?> CreateQuestionAsync(Question question);
        Task<bool> UpdateQuestionAsync(Question question);
        Task<bool> DeleteQuestionAsync(string id);
    }

    public class QuestionApiService : IQuestionApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<QuestionApiService> _logger;

        public QuestionApiService(IApiService apiService, ILogger<QuestionApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả câu hỏi
        /// </summary>
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all questions from API");
                
                // Get raw JSON first to handle problematic enum values
                var rawJson = await _apiService.GetRawAsync("questions");
                if (string.IsNullOrEmpty(rawJson))
                {
                    _logger.LogWarning("Empty response from API");
                    return new List<Question>();
                }

                // Parse as dynamic to handle enum conversion issues
                var jsonDoc = System.Text.Json.JsonDocument.Parse(rawJson);
                var questions = new List<Question>();

                foreach (var element in jsonDoc.RootElement.EnumerateArray())
                {
                    try
                    {
                        var question = new Question
                        {
                            ApiId = element.GetProperty("_id").GetString(),
                            ExamId = element.TryGetProperty("examId", out var examIdProp) ? examIdProp.GetString() : null,
                            SubjectId = element.TryGetProperty("subjectId", out var subjectIdProp) ? (subjectIdProp.GetString() ?? "") : "",
                            Content = element.GetProperty("content").GetString() ?? "",
                            Options = element.TryGetProperty("options", out var optionsProp) 
                                ? optionsProp.EnumerateArray().Select(o => o.GetString() ?? "").ToList() 
                                : new List<string>(),
                            CorrectAnswer = element.TryGetProperty("correctAnswer", out var correctProp) ? correctProp.GetString() : null,
                            Score = element.TryGetProperty("score", out var scoreProp) ? scoreProp.GetDouble() : 1.0,
                            CreatedBy = element.TryGetProperty("createdBy", out var createdByProp) ? createdByProp.GetString() ?? "" : "",
                            CreatedAt = element.TryGetProperty("createdAt", out var createdAtProp) ? createdAtProp.GetDateTime() : DateTime.Now,
                        };

                        // Parse Type with fallback
                        if (element.TryGetProperty("type", out var typeProp))
                        {
                            var typeStr = typeProp.GetString()?.ToLower();
                            question.Type = typeStr switch
                            {
                                "multiple-choice" => QuestionType.MultipleChoice,
                                "multiplechoice" => QuestionType.MultipleChoice,
                                "essay" => QuestionType.Essay,
                                "true-false" => QuestionType.TrueFalse,
                                "truefalse" => QuestionType.TrueFalse,
                                _ => QuestionType.MultipleChoice // Default fallback
                            };
                        }

                        // Parse DifficultyLevel with fallback
                        if (element.TryGetProperty("difficultyLevel", out var diffProp))
                        {
                            var diffStr = diffProp.GetString()?.ToLower();
                            question.DifficultyLevel = diffStr switch
                            {
                                "easy" => DifficultyLevel.Easy,
                                "medium" => DifficultyLevel.Medium,
                                "hard" => DifficultyLevel.Hard,
                                _ => DifficultyLevel.Medium // Default fallback
                            };
                        }

                        questions.Add(question);
                        _logger.LogInformation($"Parsed question: {question.ApiId}, Type: {question.Type}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to parse question: {ex.Message}");
                        continue; // Skip problematic question
                    }
                }

                _logger.LogInformation($"Successfully fetched {questions.Count} questions");
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching questions: {ex.Message}");
                return new List<Question>();
            }
        }

        /// <summary>
        /// Lấy câu hỏi theo ID
        /// </summary>
        public async Task<Question?> GetQuestionByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                // Use GetRawAsync to handle enum conversion issues
                var rawJson = await _apiService.GetRawAsync($"questions/{id}");
                
                if (string.IsNullOrEmpty(rawJson))
                {
                    return null;
                }

                // Parse JSON manually to handle enum conversion
                var jsonDoc = System.Text.Json.JsonDocument.Parse(rawJson);
                var element = jsonDoc.RootElement;

                var question = new Question
                {
                    ApiId = element.GetProperty("_id").GetString(),
                    ExamId = element.TryGetProperty("examId", out var examIdProp) ? examIdProp.GetString() : null,
                    SubjectId = element.TryGetProperty("subjectId", out var subjectIdProp) ? (subjectIdProp.GetString() ?? "") : "",
                    Content = element.GetProperty("content").GetString() ?? "",
                    Options = element.TryGetProperty("options", out var optionsProp) 
                        ? optionsProp.EnumerateArray().Select(o => o.GetString() ?? "").ToList() 
                        : new List<string>(),
                    CorrectAnswer = element.TryGetProperty("correctAnswer", out var correctProp) ? correctProp.GetString() : null,
                    Score = element.TryGetProperty("score", out var scoreProp) ? scoreProp.GetDouble() : 1.0,
                    CreatedBy = element.TryGetProperty("createdBy", out var createdByProp) ? createdByProp.GetString() ?? "" : "",
                    CreatedAt = element.TryGetProperty("createdAt", out var createdAtProp) ? createdAtProp.GetDateTime() : DateTime.Now,
                };

                // Parse Type with fallback
                if (element.TryGetProperty("type", out var typeProp))
                {
                    var typeStr = typeProp.GetString()?.ToLower();
                    question.Type = typeStr switch
                    {
                        "multiple-choice" => QuestionType.MultipleChoice,
                        "multiplechoice" => QuestionType.MultipleChoice,
                        "essay" => QuestionType.Essay,
                        "true-false" => QuestionType.TrueFalse,
                        "truefalse" => QuestionType.TrueFalse,
                        _ => QuestionType.MultipleChoice
                    };
                }

                // Parse DifficultyLevel with fallback
                if (element.TryGetProperty("difficultyLevel", out var diffProp))
                {
                    var diffStr = diffProp.GetString()?.ToLower();
                    question.DifficultyLevel = diffStr switch
                    {
                        "easy" => DifficultyLevel.Easy,
                        "medium" => DifficultyLevel.Medium,
                        "hard" => DifficultyLevel.Hard,
                        _ => DifficultyLevel.Medium
                    };
                }
                
                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching question {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy câu hỏi theo môn học
        /// </summary>
        public async Task<List<Question>> GetQuestionsBySubjectAsync(string subjectId)
        {
            try
            {
                if (string.IsNullOrEmpty(subjectId))
                {
                    _logger.LogWarning("Subject ID is null or empty");
                    return new List<Question>();
                }

                _logger.LogInformation($"Fetching questions for subject: {subjectId}");
                
                // Get all questions and filter by subjectId
                var allQuestions = await GetAllQuestionsAsync();
                var filteredQuestions = allQuestions
                    .Where(q => q.SubjectId == subjectId)
                    .ToList();
                
                _logger.LogInformation($"Found {filteredQuestions.Count} questions for subject {subjectId}");
                return filteredQuestions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching questions for subject {subjectId}: {ex.Message}");
                return new List<Question>();
            }
        }

        /// <summary>
        /// Lấy câu hỏi theo đề thi
        /// </summary>
        public async Task<List<Question>> GetQuestionsByExamAsync(string examId)
        {
            try
            {
                if (string.IsNullOrEmpty(examId))
                {
                    _logger.LogWarning("Exam ID is null or empty");
                    return new List<Question>();
                }

                _logger.LogInformation($"Fetching questions for exam: {examId}");
                
                // Get all questions and filter by examId
                var allQuestions = await GetAllQuestionsAsync();
                var filteredQuestions = allQuestions
                    .Where(q => q.ExamId == examId)
                    .ToList();
                
                _logger.LogInformation($"Found {filteredQuestions.Count} questions for exam {examId}");
                return filteredQuestions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching questions for exam {examId}: {ex.Message}");
                return new List<Question>();
            }
        }

        /// <summary>
        /// Tạo câu hỏi mới
        /// </summary>
        public async Task<Question?> CreateQuestionAsync(Question question)
        {
            try
            {
                _logger.LogInformation($"Creating new question for subject: {question.SubjectId}");
                
                // Prepare request body - Convert enum to lowercase strings
                var requestBody = new
                {
                    subjectId = question.SubjectId,
                    examId = string.IsNullOrEmpty(question.ExamId) ? null : question.ExamId,
                    content = question.Content,
                    type = question.Type switch
                    {
                        QuestionType.MultipleChoice => "multiple-choice",
                        QuestionType.Essay => "essay",
                        QuestionType.TrueFalse => "true-false",
                        _ => "multiple-choice"
                    },
                    options = question.Options ?? new List<string>(),
                    correctAnswer = question.CorrectAnswer,
                    score = question.Score,
                    difficultyLevel = question.DifficultyLevel switch
                    {
                        DifficultyLevel.Easy => "easy",
                        DifficultyLevel.Medium => "medium",
                        DifficultyLevel.Hard => "hard",
                        _ => "medium"
                    },
                    createdBy = question.CreatedBy
                };

                _logger.LogInformation($"Request body: examId={requestBody.examId}, type={requestBody.type}, options count={requestBody.options.Count}");
                
                // Use raw JSON to handle enum conversion
                var rawResponse = await _apiService.PostRawAsync("questions", requestBody);
                
                if (string.IsNullOrEmpty(rawResponse))
                {
                    _logger.LogWarning("Empty response from POST /api/questions");
                    return null;
                }

                // Parse response manually to handle enum conversion
                var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse);
                var element = jsonDoc.RootElement;

                var createdQuestion = new Question
                {
                    ApiId = element.GetProperty("_id").GetString(),
                    ExamId = element.TryGetProperty("examId", out var examIdProp) ? examIdProp.GetString() : null,
                    SubjectId = element.TryGetProperty("subjectId", out var subjectIdProp) ? (subjectIdProp.GetString() ?? "") : "",
                    Content = element.GetProperty("content").GetString() ?? "",
                    Options = element.TryGetProperty("options", out var optionsProp) 
                        ? optionsProp.EnumerateArray().Select(o => o.GetString() ?? "").ToList() 
                        : new List<string>(),
                    CorrectAnswer = element.TryGetProperty("correctAnswer", out var correctProp) ? correctProp.GetString() : null,
                    Score = element.TryGetProperty("score", out var scoreProp) ? scoreProp.GetDouble() : 1.0,
                    CreatedBy = element.TryGetProperty("createdBy", out var createdByProp) ? createdByProp.GetString() ?? "" : "",
                    CreatedAt = element.TryGetProperty("createdAt", out var createdAtProp) ? createdAtProp.GetDateTime() : DateTime.Now,
                };

                // Parse Type with fallback
                if (element.TryGetProperty("type", out var typeProp))
                {
                    var typeStr = typeProp.GetString()?.ToLower();
                    createdQuestion.Type = typeStr switch
                    {
                        "multiple-choice" => QuestionType.MultipleChoice,
                        "essay" => QuestionType.Essay,
                        "true-false" => QuestionType.TrueFalse,
                        _ => QuestionType.MultipleChoice
                    };
                }

                // Parse DifficultyLevel with fallback
                if (element.TryGetProperty("difficultyLevel", out var diffProp))
                {
                    var diffStr = diffProp.GetString()?.ToLower();
                    createdQuestion.DifficultyLevel = diffStr switch
                    {
                        "easy" => DifficultyLevel.Easy,
                        "medium" => DifficultyLevel.Medium,
                        "hard" => DifficultyLevel.Hard,
                        _ => DifficultyLevel.Medium
                    };
                }

                _logger.LogInformation($"Question created successfully with ID: {createdQuestion.ApiId}");
                return createdQuestion;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating question: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cập nhật câu hỏi
        /// </summary>
        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            try
            {
                if (string.IsNullOrEmpty(question.ApiId))
                {
                    _logger.LogWarning("Cannot update question: ApiId is null or empty");
                    return false;
                }

                _logger.LogInformation($"Updating question with ID: {question.ApiId}");
                
                // Prepare request body - Convert enum to lowercase strings with proper format
                var requestBody = new
                {
                    subjectId = question.SubjectId,
                    examId = string.IsNullOrEmpty(question.ExamId) ? null : question.ExamId,
                    content = question.Content,
                    type = question.Type switch
                    {
                        QuestionType.MultipleChoice => "multiple-choice",
                        QuestionType.Essay => "essay",
                        QuestionType.TrueFalse => "true-false",
                        _ => "multiple-choice"
                    },
                    options = question.Options ?? new List<string>(),
                    correctAnswer = question.CorrectAnswer,
                    score = question.Score,
                    difficultyLevel = question.DifficultyLevel switch
                    {
                        DifficultyLevel.Easy => "easy",
                        DifficultyLevel.Medium => "medium",
                        DifficultyLevel.Hard => "hard",
                        _ => "medium"
                    }
                };

                _logger.LogInformation($"Update request: type={requestBody.type}, difficultyLevel={requestBody.difficultyLevel}");
                
                var response = await _apiService.PutAsync<object>("questions", question.ApiId, requestBody);
                
                if (response != null)
                {
                    _logger.LogInformation($"Question {question.ApiId} updated successfully");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating question {question.ApiId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa câu hỏi
        /// </summary>
        public async Task<bool> DeleteQuestionAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Cannot delete question: ID is null or empty");
                    return false;
                }

                _logger.LogInformation($"Deleting question with ID: {id}");
                var success = await _apiService.DeleteAsync("questions", id);
                
                if (success)
                {
                    _logger.LogInformation($"Question {id} deleted successfully");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting question {id}: {ex.Message}");
                return false;
            }
        }
    }
}
