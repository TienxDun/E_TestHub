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

        public IActionResult ExamSchedule()
        {
            return View();
        }

        public IActionResult ExamHistory()
        {
            return View();
        }

        public IActionResult ExamInfo(int examId)
        {
            ViewBag.ExamId = examId;
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