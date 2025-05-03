using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class TelemedicineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TelemedicineSessions()
        {
            return View();
        }
        public IActionResult DisplayTelemedicineSessions()
        {
            return View();
        }
        public IActionResult VideoCallLogs()
        {
            return View();
        }
        public IActionResult DisplayVideoCallLogs()
        {
            return View();
        }
        public IActionResult OnlinePrescriptions()
        {
            return View();
        }
        public IActionResult DisplayOnlinePrescriptions()
        {
            return View();
        }
    }
}
