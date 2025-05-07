using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace HospitalManagementSystem.Controllers
{
    public class ReportsAnalyticsController : Controller
    {
       
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorrepository;
        private readonly IStaffRepository staffRepository;
        private readonly IReportsAnalyticsRepository reportsAnalyticsRepository;
        public ReportsAnalyticsController(IPatientRepository patientRepository, IDoctorRepository doctorrepository, IReportsAnalyticsRepository reportsAnalyticsRepository, IStaffRepository staffRepository)
        {
            this.patientRepository = patientRepository;
            this.doctorrepository = doctorrepository;
            this.reportsAnalyticsRepository = reportsAnalyticsRepository;
            this.staffRepository = staffRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ClinicalReports()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            return View();
        }
        [HttpPost]
        public IActionResult ClinicalReports(ClinicalReport reports, IFormFile imageFile)
        {
            reportsAnalyticsRepository.AddClinicalReport(reports, imageFile);
            Console.WriteLine($"imageFile is null: {imageFile == null}");

            return RedirectToAction("DisplayClinicalReports");
        }

        [HttpGet]
        public IActionResult EditClinicalReports(int id)
        {
            var reports = reportsAnalyticsRepository.GetClinicalReportById(id);
            if (reports == null)
            {
                return NotFound(); // Return 404 if not found
            }

            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            return View(reports);
        }
        [HttpPost]
        public IActionResult EditClinicalReports(ClinicalReport reports, IFormFile image)
        {
            reportsAnalyticsRepository.UpdateClinicalReport(reports, image);
            return RedirectToAction("DisplayClinicalReports");
        }
        public IActionResult DeleteClinicalReports(int id)
        {
            reportsAnalyticsRepository.DeleteClinicalReport(id);

            return RedirectToAction("DisplayClinicalReports");

        }
        public IActionResult DisplayClinicalReports()
        {
            var reports = reportsAnalyticsRepository.GetAllClinicalReports();
            return View(reports);
        }
        [HttpGet]
        public IActionResult FinancialReports()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FinancialReports(FinancialReport fr)
        {
            reportsAnalyticsRepository.AddFinancialReport(fr);
            return RedirectToAction("DisplayFinancialReports");
        }
        public IActionResult DisplayFinancialReports()
        {
            var reports = reportsAnalyticsRepository.GetAllFinancialReports();
            return View(reports);
        }
        [HttpGet]
        public IActionResult EditFinancialReports(int id)
        {
            var reports = reportsAnalyticsRepository.GetFinancialReportById(id);
            if (reports == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return View(reports);
        }
        [HttpPost]
        public IActionResult EditFinancialReports(FinancialReport reports)
        {
            reportsAnalyticsRepository.UpdateFinancialReport(reports);
            return RedirectToAction("DisplayFinancialReports");
        }
        public IActionResult DeleteFinancialReports(int id)
        {
            reportsAnalyticsRepository.DeleteFinancialReport(id);

            return RedirectToAction("DisplayFinancialReports");

        }
        [HttpGet]
        public IActionResult PerformanceMonitoring()
        {
            ViewBag.staffName = staffRepository.GetEmployee();
            ViewBag.departmentName = doctorrepository.GetDepartment();
            return View();
        }
        [HttpPost]
        public IActionResult PerformanceMonitoring(PerformanceMonitoring pm)
        {
           
            reportsAnalyticsRepository.AddPerformanceReport(pm);
            return RedirectToAction("DisplayPerformanceMonitoring");
        }
        public IActionResult DisplayPerformanceMonitoring()
        {

            var reports = reportsAnalyticsRepository.GetAllPerformanceReports();
            return View(reports);
        }

        [HttpGet]
        public IActionResult EditPerformanceMonitoring(int id)
        {
            var reports = reportsAnalyticsRepository.GetPerformanceReportById(id);
            if (reports == null)
            {
                return NotFound(); // Return 404 if not found
            }
            ViewBag.staffName = staffRepository.GetEmployee();
            ViewBag.departmentName = doctorrepository.GetDepartment();
            return View(reports);
        }
        [HttpPost]
        public IActionResult EditPerformanceMonitoring(PerformanceMonitoring reports)
        {
            reportsAnalyticsRepository.UpdatePerformanceReport(reports);
            return RedirectToAction("DisplayPerformanceMonitoring");
        }
        public IActionResult DeletePerformanceMonitoring(int id)
        {
            reportsAnalyticsRepository.DeletePerformanceReport(id);

            return RedirectToAction("DisplayPerformanceMonitoring");

        }

    }
}
