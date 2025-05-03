using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class HelpDeskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult HelpDeskCallLogs()
        {
            return View();
        }
        public IActionResult DisplayHelpDeskCallLogs()
        {
            return View();
        }
        public IActionResult HelpDeskFeedback()
        {
            return View();
        }
        public IActionResult DisplayHelpDeskFeedback()
        {
            return View();
        }
        public IActionResult HelpDeskAppointmentRequests()
        {
            return View();
        }
        public IActionResult DisplayHelpDeskAppointmentRequests()
        {
            return View();
        }
    }
}
