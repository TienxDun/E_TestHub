using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class TeacherController : BaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult QuestionBank()
        {
            return View();
        }

        public IActionResult CreateQuestion()
        {
            return View();
        }

        public IActionResult ExamManagement()
        {
            return View();
        }

        public IActionResult CreateExam()
        {
            return View();
        }

        public IActionResult ViewResults()
        {
            return View();
        }

        public IActionResult ManageClasses()
        {
            return View();
        }

        public IActionResult GradeExams()
        {
            return View();
        }
    }
}