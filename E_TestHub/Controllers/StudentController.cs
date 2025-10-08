using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class StudentController : BaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Classes()
        {
            return View();
        }

        public IActionResult MyExams()
        {
            return View();
        }

        public IActionResult TestCalendar()
        {
            return View();
        }

        public IActionResult ExamInfo(int examId)
        {
            ViewBag.ExamId = examId;
            
            // Simulate exam status based on current date and exam schedule
            var currentDate = new DateTime(2025, 9, 28); // Fixed current date for demo
            string examStatus = "upcoming"; // Default
            
            // Mock exam data with different statuses
            switch (examId)
            {
                case 1: // ReactJS exam - in progress today (28/9/2025)
                    var examDate1 = new DateTime(2025, 9, 28);
                    var examEndTime1 = examDate1.AddHours(2); // Exam duration 2 hours
                    
                    if (currentDate.Date == examDate1.Date && currentDate <= examEndTime1)
                        examStatus = "in-progress";
                    else if (currentDate > examEndTime1)
                        examStatus = "completed";
                    else
                        examStatus = "upcoming";
                    break;
                    
                case 2: // History exam - upcoming (5/10/2025)
                    var examDate2 = new DateTime(2025, 10, 5);
                    var examEndTime2 = examDate2.AddHours(2);
                    
                    if (currentDate.Date == examDate2.Date && currentDate <= examEndTime2)
                        examStatus = "in-progress";
                    else if (currentDate > examEndTime2)
                        examStatus = "completed";
                    else
                        examStatus = "upcoming";
                    break;
                    
                default:
                    examStatus = "upcoming";
                    break;
            }
            
            ViewBag.ExamStatus = examStatus;
            return View();
        }

        public IActionResult TakeExam(int examId)
        {
            return View();
        }

        public IActionResult ViewResults()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}