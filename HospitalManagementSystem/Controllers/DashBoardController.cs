using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HospitalManagementSystem.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository doctorRepository;
        private readonly IBedsRepository bedsRepository;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IBillingRepository billingRepository;
        private readonly IStaffRepository staffRepository;
        public DashBoardController(IPatientRepository patientRepo, IDoctorRepository doctorRepository, IBedsRepository bedsRepository, IAppointmentRepository appointmentRepository,IBillingRepository billingRepository, IStaffRepository staffRepository)
        {
            _patientRepo = patientRepo;
            this.doctorRepository = doctorRepository;
            this.bedsRepository = bedsRepository;
            this.appointmentRepository = appointmentRepository;
            this.billingRepository = billingRepository;
            this.staffRepository = staffRepository;
        }


        public IActionResult adminDashBoard()
        {
            var totalPatients = _patientRepo.GetAll().Count();
            var totalDoctors = doctorRepository.GetAllDoctors().Count();
            var totalBeds = bedsRepository.GetAllBeds().Count();
            var totalappoinments = appointmentRepository.GetAllAppointments().Count();
          
            var recentAdmissions = _patientRepo.GetAllpatient_admission()
                                               .OrderByDescending(a => a.admission_date)
                                               .Take(3)
                                               .ToList();

            string patientName = HttpContext.Session.GetString("PatientName"); // example

            var upcomingAppointment = appointmentRepository.GetAppointmentsByPatientName(patientName)
                                                 .OrderByDescending(a => a.AppointmentDate)
                                                 .Take(3)
                                                 .ToList();
            var departments = staffRepository.GetAllDepartments();
            var billingSummaries = billingRepository.GetAllInvoices()
                            .OrderByDescending(b => b.InvoiceDate)
            .Take(5) // Take top 5 recent invoices
                            .Select(b => new BillingInfo
                            {
                                BillId = b.InvoiceId.ToString(),
                                PatientName = _patientRepo.GetPatientNameById(b.PatientId),
                                Amount = b.Amount,
                                Status = b.Status // assuming your model has a Status field
                            }).ToList();


            var model = new DashboardViewModel
            {
                TotalPatients = totalPatients,
                DoctorsAvailable = totalDoctors,
                OccupiedBeds = totalBeds,
                AppointmentsToday = totalappoinments,

                RecentAdmissions = recentAdmissions.Select(adm => new AdmissionInfo
                {
                    PatientId = adm.patient_id.ToString(),
                    Name = _patientRepo.GetPatientNameById(adm.patient_id), // <-- Using your method
                    AdmissionDate = adm.admission_date
                }).ToList(),

                UpcomingAppointments = upcomingAppointment.Select(appt => new AppointmentInfo
                {
                    AppointmentId = appt.AppointmentId,
                    PatientId = appt.PatientId,
                    DoctorId = appt.DoctorId,
                    PatientName = appt.PatientName,
                    DoctorName = appt.DoctorName,  
                    AppointmentDate = appt.AppointmentDate,
                    Reason=appt.Reason,
                    Status=appt.Status
                }).ToList(),

                Departments = departments.Select(apt=>new Department
               {
                   DepartmentId=apt.DepartmentId,
                   DepartmentName=apt.DepartmentName,
               }).ToList(),

                BillingSummaries = billingSummaries
            };

            return View(model);
        }

        public IActionResult adminProfile()
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

        [HttpPost]
        public IActionResult UpdateProfile(Employee model)
        {

            staffRepository.Updateprofile(model); // Calls the Update method in the repository
                                                  // Optionally, you can update session values
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("PhoneNumber", model.PhoneNumber);
            HttpContext.Session.SetString("Address", model.Address);

            // Provide feedback to the user
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("adminProfile");


        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {

            int empId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;

            bool success = staffRepository.ChangePassword(empId, currentPassword, newPassword);

            if (success)
                TempData["Message"] = "Password updated successfully.";
            else
                TempData["Error"] = "Incorrect current password.";

            return RedirectToAction("adminProfile"); // or wherever your profile page is
        }



        public IActionResult adminSetting()
        {
            return View();
        }
        public IActionResult adminNotification()
        {
            return View();
        }

    }
}
