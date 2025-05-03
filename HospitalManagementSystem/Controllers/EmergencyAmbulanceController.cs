using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class EmergencyAmbulanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AmbulanceRequests()
        {
            return View();
        }
        public IActionResult DisplayAmbulanceRequests()
        {
            return View();
        }
        public IActionResult EmergencyCases()
        {
            return View();
        }
        public IActionResult DisplayEmergencyCases()    
        {
            return View();
        }
    }
}
