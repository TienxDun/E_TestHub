using E_TestHub.Models;

namespace E_TestHub.Services
{
    /// <summary>
    /// Service interface for managing exam submissions
    /// Provides methods for creating, updating, and retrieving student exam submissions
    /// </summary>
    public interface ISubmissionApiService
    {
        /// <summary>
        /// Start a new exam attempt for a student
        /// Creates a new submission record in the database
        /// </summary>
        /// <param name="examId">The ID of the exam</param>
        /// <param name="studentId">The ID of the student</param>
        /// <returns>The created submission object or null if failed</returns>
        Task<Submission?> StartExamAsync(string examId, string studentId);
        
        /// <summary>
        /// Get a specific submission by its ID
        /// </summary>
        /// <param name="submissionId">The ID of the submission</param>
        /// <returns>The submission object or null if not found</returns>
        Task<Submission?> GetSubmissionByIdAsync(string submissionId);
        
        /// <summary>
        /// Save answers during exam (auto-save functionality)
        /// Updates the submission without marking as submitted
        /// </summary>
        /// <param name="submissionId">The ID of the submission</param>
        /// <param name="answers">List of answers to save</param>
        /// <returns>The updated submission object or null if failed</returns>
        Task<Submission?> SaveAnswersAsync(string submissionId, List<SubmissionAnswer> answers);
        
        /// <summary>
        /// Submit final exam with all answers
        /// Marks submission as completed and calculates score
        /// </summary>
        /// <param name="submissionId">The ID of the submission</param>
        /// <param name="answers">List of final answers</param>
        /// <returns>The updated submission object with calculated score or null if failed</returns>
        Task<Submission?> SubmitExamAsync(string submissionId, List<SubmissionAnswer> answers);
        
        /// <summary>
        /// Get all submissions for a specific student
        /// Useful for viewing student's exam history
        /// </summary>
        /// <param name="studentId">The ID of the student</param>
        /// <returns>List of submissions or empty list if none found</returns>
        Task<List<Submission>> GetStudentSubmissionsAsync(string studentId);
        
        /// <summary>
        /// Get a specific student's submission for a specific exam
        /// </summary>
        /// <param name="examId">The ID of the exam</param>
        /// <param name="studentId">The ID of the student</param>
        /// <returns>The submission object or null if not found</returns>
        Task<Submission?> GetExamSubmissionAsync(string examId, string studentId);
        
        /// <summary>
        /// Check if a student has already submitted a specific exam
        /// </summary>
        /// <param name="examId">The ID of the exam</param>
        /// <param name="studentId">The ID of the student</param>
        /// <returns>True if student has submitted, false otherwise</returns>
        Task<bool> HasSubmittedExamAsync(string examId, string studentId);

        /// <summary>
        /// Get all submissions for a specific exam
        /// Useful for teachers to view all student submissions
        /// </summary>
        /// <param name="examId">The ID of the exam</param>
        /// <returns>List of submissions or empty list if none found</returns>
        Task<List<Submission>> GetExamSubmissionsAsync(string examId);

        /// <summary>
        /// Get submission statistics
        /// Provides overall statistics about submissions
        /// </summary>
        /// <returns>Statistics object or null if not available</returns>
        Task<object?> GetSubmissionStatisticsAsync();
        
        /// <summary>
        /// Get all submissions
        /// Useful for teachers to view all submissions across all exams
        /// </summary>
        /// <returns>List of all submissions or empty list if none found</returns>
        Task<List<Submission>> GetAllSubmissionsAsync();
    }
}
