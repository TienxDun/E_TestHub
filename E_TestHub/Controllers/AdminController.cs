using Microsoft.AspNetCore.Mvc;

namespace E_TestHub.Controllers
{
    public class AdminController : BaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            return View();
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult SystemSettings()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult SchoolManagement()
        {
            return View();
        }

        public IActionResult BackupRestore()
        {
            return View();
        }

        public IActionResult AuditLogs()
        {
            return View();
        }
    }
}