using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_TestHub.Models
{
    /// <summary>
    /// Represents a student's exam submission
    /// </summary>
    public class Submission
    {
        [JsonPropertyName("_id")]
        public string ApiId { get; set; } = string.Empty; // MongoDB _id
        
        public string UserId { get; set; } = string.Empty;
        
        public string StudentId { get; set; } = string.Empty;
        
        public string ExamId { get; set; } = string.Empty;
        
        public List<SubmissionAnswer> Answers { get; set; } = new List<SubmissionAnswer>();
        
        public DateTime? SubmittedAt { get; set; }
        
        public double Score { get; set; }
        
        public string Status { get; set; } = "pending"; // pending, graded, reviewed
        
        public bool IsGraded { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties (populated from API)
        public Exam? Exam { get; set; }
        public User? Student { get; set; }
    }
    
    /// <summary>
    /// Represents an answer to a single question
    /// </summary>
    public class SubmissionAnswer
    {
        public string QuestionId { get; set; } = string.Empty;
        
        public string SelectedOption { get; set; } = string.Empty; // A, B, C, D
        
        public double Score { get; set; }
        
        // Populated for review
        public Question? Question { get; set; }
    }
    
    /// <summary>
    /// ViewModel for starting an exam
    /// </summary>
    public class StartExamRequest
    {
        [Required]
        public string ExamId { get; set; } = string.Empty;
        
        [Required]
        public string StudentId { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// ViewModel for saving answers during exam
    /// </summary>
    public class SaveAnswersRequest
    {
        [Required]
        public string SubmissionId { get; set; } = string.Empty;
        
        [Required]
        public List<SubmissionAnswer> Answers { get; set; } = new List<SubmissionAnswer>();
    }
    
    /// <summary>
    /// ViewModel for submitting final exam
    /// </summary>
    public class SubmitExamRequest
    {
        [Required]
        public string SubmissionId { get; set; } = string.Empty;
        
        [Required]
        public List<SubmissionAnswer> Answers { get; set; } = new List<SubmissionAnswer>();
    }
    
    /// <summary>
    /// ViewModel for Take Exam page
    /// </summary>
    public class TakeExamViewModel
    {
        public Exam Exam { get; set; } = new Exam();
        
        public List<Question> Questions { get; set; } = new List<Question>();
        
        public Submission? ExistingSubmission { get; set; }
        
        public ExamSchedule Schedule { get; set; } = new ExamSchedule();
        
        public int DurationMinutes { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public bool CanStart { get; set; }
        
        public string Message { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// ViewModel for Exam Results page
    /// </summary>
    public class ExamResultViewModel
    {
        public Submission Submission { get; set; } = new Submission();
        
        public Exam Exam { get; set; } = new Exam();
        
        public List<QuestionResultDetail> QuestionResults { get; set; } = new List<QuestionResultDetail>();
        
        public ExamResultStatistics Statistics { get; set; } = new ExamResultStatistics();
        
        public double ClassAverage { get; set; }
        
        public int Rank { get; set; }
        
        public int TotalStudents { get; set; }
    }
    
    /// <summary>
    /// Detail of a single question result
    /// </summary>
    public class QuestionResultDetail
    {
        public Question Question { get; set; } = new Question();
        
        public string SelectedOption { get; set; } = string.Empty;
        
        public string CorrectOption { get; set; } = string.Empty;
        
        public bool IsCorrect { get; set; }
        
        public double Score { get; set; }
    }
    
    /// <summary>
    /// Statistics for exam result
    /// </summary>
    public class ExamResultStatistics
    {
        public int TotalQuestions { get; set; }
        
        public int CorrectAnswers { get; set; }
        
        public int WrongAnswers { get; set; }
        
        public int UnansweredQuestions { get; set; }
        
        public double AccuracyPercentage { get; set; }
        
        public TimeSpan TimeSpent { get; set; }
        
        // By difficulty
        public int EasyCorrect { get; set; }
        public int EasyTotal { get; set; }
        
        public int MediumCorrect { get; set; }
        public int MediumTotal { get; set; }
        
        public int HardCorrect { get; set; }
        public int HardTotal { get; set; }
    }
    
    /// <summary>
    /// Teacher view model for viewing all subject results
    /// </summary>
    public class SubjectResultViewModel
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int ExamCount { get; set; }
        public double AverageScore { get; set; }
        public int SubmissionCount { get; set; }
    }
    
    /// <summary>
    /// Teacher view model for viewing student exam details
    /// </summary>
    public class StudentExamViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int Duration { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public double Score { get; set; }
        public double MaxScore { get; set; }
        public double AccuracyPercentage { get; set; }
        public double TimeSpentMinutes { get; set; }
        public List<QuestionResult> Questions { get; set; } = new List<QuestionResult>();
    }
    
    /// <summary>
    /// Individual question result for teacher view
    /// </summary>
    public class QuestionResult
    {
        public string QuestionId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string StudentAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public double Score { get; set; }
    }
}
