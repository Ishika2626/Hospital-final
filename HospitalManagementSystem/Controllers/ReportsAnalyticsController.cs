using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class ReportsAnalyticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ClinicalReports()
        {
            return View();
        }
        public IActionResult DisplayClinicalReports()
        {
            return View();
        }
        public IActionResult FinancialReports()
        {
            return View();
        }
        public IActionResult DisplayFinancialReports()
        {
            return View();
        }
        public IActionResult PerformanceMonitoring()
        {
            return View();
        }
        public IActionResult DisplayPerformanceMonitoring()
        {
            return View();
        }
    }
}
