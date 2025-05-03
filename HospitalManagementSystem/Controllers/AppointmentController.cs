using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorrepository;
        public AppointmentController(IAppointmentRepository appointmentRepository,IPatientRepository patientRepository, IDoctorRepository doctorrepository)
        {
            this.appointmentRepository = appointmentRepository;
            this.patientRepository = patientRepository;
            this.doctorrepository = doctorrepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    
        [HttpGet]
        public IActionResult AppointmentScheduling()
        {
            var patientId = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientId))
            {
                // Redirect to login and pass returnUrl for redirection back after login
                return RedirectToAction("PatientLogin", "Patient", new { returnUrl = Url.Action("AppointmentScheduling", "Appointment") });
            }

            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AppointmentScheduling(Appointment ap)
        {
            var patientId = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientId))
            {
                return RedirectToAction("PatientLogin", "Patient", new { returnUrl = Url.Action("AppointmentScheduling", "Appointment") });
            }

            // You can optionally set the patientId to the appointment if needed
            ap.PatientId = Convert.ToInt32(patientId);

            appointmentRepository.AddAppointment(ap);
            return RedirectToAction("PatientDashboard", "Patient");
        }
        [HttpGet]
        public IActionResult EditAppointmentScheduling(int id)
        {
            var ap = appointmentRepository.GetAppointmentById(id);
            if (ap == null)
            {
                return NotFound();
            }
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(ap);
        }

        [HttpPost]
        public IActionResult EditAppointmentScheduling(Appointment ap)
        {
            appointmentRepository.UpdateAppointment(ap);
            return RedirectToAction("DisplayAppointmentScheduling");
        }

        public IActionResult DeleteAppointmentScheduling(int id)
        {
            appointmentRepository.DeleteAppointment(id);

            return RedirectToAction("DisplayAppointmentScheduling");

        }

        public IActionResult DisplayAppointmentScheduling()
        {
            var AppointmentScheduling = appointmentRepository.GetAllAppointments();
            return View(AppointmentScheduling);
        }
        [HttpGet]
        public IActionResult DoctorAvailability()
        {
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            return View();
        }
        [HttpPost]
        public IActionResult DoctorAvailability(DoctorAvailability da)
        {
            appointmentRepository.AddDoctorAvailability(da);
            return RedirectToAction("DisplayDoctorAvailability");
        }
        public IActionResult DisplayDoctorAvailability()
        {
            var DoctorAvailability = appointmentRepository.GetAllDoctorAvailabilities();
            return View(DoctorAvailability);
        }

        [HttpGet]
        public IActionResult EditDoctorAvailability(int id)
        {
            var ap = appointmentRepository.GetDoctorAvailabilityById(id);
            if (ap == null)
            {
                return NotFound();
            }
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(ap);
        }

        [HttpPost]
        public IActionResult EditDoctorAvailability(DoctorAvailability ap)
        {
            appointmentRepository.UpdateDoctorAvailability(ap);
            return RedirectToAction("DisplayDoctorAvailability");
        }

        public IActionResult DeleteDoctorAvailability(int id)
        {
            appointmentRepository.DeleteDoctorAvailability(id);

            return RedirectToAction("DisplayDoctorAvailability");

        }


        [HttpGet]
        public IActionResult AppointmentAlerts()
        {
            ViewBag.appoinementID = appointmentRepository.GetAppointmentId();
            return View();
        }
        [HttpPost]
        public IActionResult AppointmentAlerts(AppointmentAlert aa)
        {
            appointmentRepository.AddAppointmentAlert(aa);
            return RedirectToAction("DisplayAppointmentAlerts");
        }
        public IActionResult DisplayAppointmentAlerts()
        {
            var AppointmentAlerts = appointmentRepository.GetAllAppointmentAlerts();
            return View(AppointmentAlerts);
        }
        [HttpGet]
        public IActionResult EditAppointmentAlerts(int id)
        {
            var ap = appointmentRepository.GetAppointmentAlertById(id);
            if (ap == null)
            {
                return NotFound();
            }
            ViewBag.appoinementID = appointmentRepository.GetAppointmentId();
            return View(ap);
        }

        [HttpPost]
        public IActionResult EditAppointmentAlerts(AppointmentAlert ap)
        {
            appointmentRepository.UpdateAppointmentAlert(ap);
            return RedirectToAction("DisplayAppointmentAlerts");
        }

        public IActionResult DeleteAppointmentAlerts(int id)
        {
            appointmentRepository.DeleteAppointmentAlert(id);

            return RedirectToAction("DisplayAppointmentAlerts");

        }

       

    }
}
