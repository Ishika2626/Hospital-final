using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IDoctorRepository doctorRepository;
        public DoctorsController(IDoctorRepository doctorRepository, IStaffRepository staffRepository)
        {
            this.doctorRepository = doctorRepository;
            _staffRepository = staffRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Doctors()
        {
            var departments = doctorRepository.GetDepartment();

            ViewBag.getDepartment = departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName
            }).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Doctors(Doctor doctor, IFormFile doctor_img)
        {
          
                doctorRepository.AddDoctor(doctor, doctor_img);
                return RedirectToAction("DisplayDoctors");
         
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

        [HttpGet]
        public IActionResult Departments()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Departments(Department department)
        {
            if (ModelState.IsValid)
            {
                _staffRepository.AddDepartment(department);
                return RedirectToAction("DisplayDepartments");
            }

            var departments = _staffRepository.GetAllDepartments();
            return View("DisplayDepartments", departments);
        }
        public IActionResult DisplayDepartments()
        {
            var departments = _staffRepository.GetAllDepartments();
            return View(departments);
        }

      

        [HttpPost]
        public IActionResult EditDepartment(Department dept)
        {
            _staffRepository.UpdateDepartment(dept);
            return RedirectToAction("DisplayDepartments");
        }

        [HttpPost]
        public IActionResult DeleteDepartment(int id)
        {
            _staffRepository.DeleteDepartment(id);
            return RedirectToAction("DisplayDepartments");
        }


    }
}
