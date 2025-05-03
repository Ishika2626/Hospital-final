using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class LaboratoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LabTests()
        {
            return View();
        }
        public IActionResult DisplayLabTests()
        {
            return View();
        }
        public IActionResult PatientTests()
        {
            return View();
        }
        public IActionResult DisplayPatientTests()
        {
            return View();
        }
        public IActionResult LabReports()
        {
            return View();
        }
        public IActionResult DisplayLabReports()
        {
            return View();
        }

    }
}
