using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class SecurityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UserRoles()
        {
            return View();
        }
        public IActionResult DisplayUserRoles()
        {
            return View();
        }
        public IActionResult AuditLogs()
        {
            return View();
        }
        public IActionResult DisplayAuditLogs()
        {
            return View();
        }
        public IActionResult DataSecurity()
        {
            return View();
        }
        public IActionResult DisplayDataSecurity()
        {
            return View();
        }
    }
}
