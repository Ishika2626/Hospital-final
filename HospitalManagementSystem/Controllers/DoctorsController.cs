using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorRepository doctorRepository;
        public DoctorsController(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Doctors()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Doctors(Doctor doctor, IFormFile doctor_img)
        {
            if (ModelState.IsValid)
            {
                doctorRepository.AddDoctor(doctor, doctor_img);
                return RedirectToAction("DisplayDoctors");
            }
            return View(doctor); // In case of error, return the view with the model.
        }

        public IActionResult DisplayDoctors()
        {
            var doctors = doctorRepository.GetAllDoctors();
            return View(doctors);
        }


        [HttpGet]
        public IActionResult EditDoctor(int id)
        {

            var doctors = doctorRepository.GetDoctorById(id);
            if (doctors == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return View(doctors);
        }

        [HttpPost]
        public IActionResult EditDoctor(Doctor doctor, IFormFile doctor_img)
        {
            doctorRepository.UpdateDoctor(doctor, doctor_img);
            return RedirectToAction("DisplayDoctors");
        }

        public IActionResult DeleteDoctor(int id)
        {
            doctorRepository.DeleteDoctor(id);

            return RedirectToAction("DisplayDoctors");

        }


        public IActionResult Specialization()
        {
            return View();
        }
        public IActionResult DisplaySpecialization()
        {
            return View();
        }
        public IActionResult Schedule()
        {
            return View();
        }
        public IActionResult DisplaySchedule()
        {
            return View();
        }
        public IActionResult Appointment()
        {
            return View();
        }
        public IActionResult DisplayAppointment()
        {
            return View();
        }
        public IActionResult Prescription()
        {
            return View();
        }
        public IActionResult DisplayPrescription()
        {
            return View();
        }
        public IActionResult Notes()
        {
            return View();
        }
        public IActionResult DisplayNotes()
        {
            return View();
        }
        public IActionResult Reviews()
        {
            return View();
        }
        public IActionResult DisplayReviews()
        {
            return View();
        }
        public IActionResult Payments()
        {
            return View();
        }
        public IActionResult DisplayPayments()
        {
            return View();
        }



    }
}
