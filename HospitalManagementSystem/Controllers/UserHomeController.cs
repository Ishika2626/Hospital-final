using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Controllers
{
    public class UserHomeController : Controller
    {
        private readonly IStaffRepository _staffrepo;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorrepository;
        private readonly IBedsRepository bedRepository;
        public UserHomeController(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IDoctorRepository doctorrepository, IBedsRepository bedRepository,IStaffRepository _staffrepo)
        {
            
            this.appointmentRepository = appointmentRepository;
            this.patientRepository = patientRepository;
            this.doctorrepository = doctorrepository;
            this.bedRepository = bedRepository;
            this._staffrepo = _staffrepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UserHome()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUS()
        {
            return View();
        }
        public IActionResult Doctors()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Appointments()
        {
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }
        [HttpPost]
        public IActionResult Appointments(Appointment ap)
        {
            if (ModelState.IsValid)
            {
                appointmentRepository.AddAppointment(ap);
                return RedirectToAction("DisplayAppointmentScheduling");
            }

            // Repopulate ViewBag if form is redisplayed
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();

            return View(ap);
        }


        public IActionResult BillingInsurance()
        {
            return View();
        }
        public IActionResult Pharmacy()
        {
            return View();
        }
        public IActionResult ReportsDiagnostics()
        {
            return View();
        }
        public IActionResult EmergencyServices()
        {
            return View();
        }
        public IActionResult PatientProfile()
        {
            ViewBag.LoginID = HttpContext.Session.GetString("LoginID");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.PhoneNumber = HttpContext.Session.GetString("PhoneNumber");
            ViewBag.Address = HttpContext.Session.GetString("Address");
            ViewBag.EmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            ViewBag.Gender = HttpContext.Session.GetString("Gender");
            ViewBag.DateOfBirth = HttpContext.Session.GetString("DateOfBirth");
            ViewBag.HireDate = HttpContext.Session.GetString("HireDate");

            return View();
        }
        public IActionResult PatientsVisitors()
        {
            var rooms = bedRepository.GetAllRooms(); 

            return View(rooms);
        }
        public IActionResult GovernmentSchemes()
        {
            return View();
        }
        public IActionResult EmpanelledCorporates()
        {
            return View();
        }
        public IActionResult TPACashlessTieUps()
        {
            return View();
        }
        public IActionResult BloodBank()
        {
            return View();
        }

        public IActionResult Laboratory()
        {
            return View();
        }
        public IActionResult Careers()
        {
            var jobs = _staffrepo.GetAllOpenJobs();
            return View(jobs);
        }
    }
}
