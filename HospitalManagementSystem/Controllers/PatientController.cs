using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagementSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorrepository;
        private readonly IBedsRepository bedsRepository;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IBillingRepository billingRepository;
        private readonly IPharmacyRepository pharmacyRepository;
        private readonly ILabTestRepository labTestRepository;
        public PatientController(IPatientRepository patientRepository, ILabTestRepository labTestRepository, IDoctorRepository doctorrepository, IBedsRepository bedsRepository, IAppointmentRepository appointmentRepository, IBillingRepository billingRepository, IPharmacyRepository pharmacyRepository, IWebHostEnvironment env)
        {
            this.patientRepository = patientRepository;
            this.doctorrepository = doctorrepository;
            this.bedsRepository = bedsRepository;
            this.appointmentRepository = appointmentRepository;
            this.billingRepository = billingRepository;
            this.pharmacyRepository = pharmacyRepository;
            this.labTestRepository = labTestRepository;
            _env = env;
        }



        public IActionResult DisplayPatient()
        {
            var patients = patientRepository.GetAll();
            return View(patients);
        }


        [HttpGet]
        public IActionResult AddPatient()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddPatient(PatientRegistration patient, IFormFile patient_img)
        {
            patientRepository.Add(patient, patient_img);
            return RedirectToAction("DisplayPatient");
        }


        [HttpGet]
        public IActionResult EditPatient(int id)
        {
            var patient = patientRepository.GetById(id);
            if (patient == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return View(patient);
        }

        [HttpPost]
        public IActionResult EditPatient(PatientRegistration patient, IFormFile patient_img)
        {
            patientRepository.Update(patient, patient_img);
            return RedirectToAction("DisplayPatient");
        }

        public IActionResult DeletePatient(int id)
        {
            patientRepository.Delete(id);

            return RedirectToAction("DisplayPatient");

        }
        [HttpGet]
        public IActionResult AdmissionDischarge()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.RoomTypes = patientRepository.GetRoomTypes();
            ViewBag.BedTypes = patientRepository.GetBedTypes();
            return View();
        }
        [HttpPost]
        public IActionResult AdmissionDischarge(patient_admission admission)
        {

            patientRepository.Addpatient_admission(admission);
            return RedirectToAction("DisplayAdmissionDischarge");
        }
        public IActionResult DisplayAdmissionDischarge()
        {

            var admission = patientRepository.GetAllpatient_admission();
            return View(admission);
        }
        [HttpGet]
        public IActionResult EditAdmission(int id)
        {
            var admission = patientRepository.Getpatient_admissionById(id);
            if (admission == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.RoomTypes = patientRepository.GetRoomTypes();
            ViewBag.BedTypes = patientRepository.GetBedTypes();

            return View(admission);
        }

        [HttpPost]
        public IActionResult EditAdmission(patient_admission admission)
        {
            patientRepository.Updatepatient_admission(admission);
            return RedirectToAction("DisplayAdmissionDischarge");
        }

        public IActionResult DeleteAdmission(int id)
        {
            patientRepository.Deletepatient_admission(id);

            return RedirectToAction("DisplayAdmissionDischarge");

        }

        [HttpGet]
        public IActionResult PatientRecords()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }
        [HttpPost]
        public IActionResult PatientRecords(patient_records records)
        {
            patientRepository.Addpatient_records(records);
            return RedirectToAction("DisplayPatientRecords");
        }
        public IActionResult DisplayPatientRecords()
        {
            var records = patientRepository.GetAllpatient_records();
            return View(records);
        }
        [HttpGet]
        public IActionResult EditRecords(int id)
        {
            var records = patientRepository.Getpatient_recordsById(id);
            if (records == null)
            {
                return NotFound(); // Return 404 if not found
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(records);
        }

        [HttpPost]
        public IActionResult EditRecords(patient_records records)
        {
            patientRepository.Updatepatient_records(records);
            return RedirectToAction("DisplayPatientRecords");
        }

        public IActionResult DeleteRecords(int id)
        {
            patientRepository.Deletepatient_records(id);

            return RedirectToAction("DisplayPatientRecords");

        }
        [HttpGet]
        public IActionResult PatientVisits()
        {
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }
        [HttpPost]
        public IActionResult PatientVisits(patient_visits visits)
        {
            patientRepository.Addpatient_visits(visits);
            return RedirectToAction("DisplayPatientVisits");
        }
        public IActionResult DisplayPatientVisits()
        {
            var visits = patientRepository.GetAllpatient_visits();
            return View(visits);
        }
        [HttpGet]
        public IActionResult EditPatientVisits(int id)
        {
            var visits = patientRepository.Getpatient_visitsById(id);
            if (visits == null)
            {
                return NotFound(); // Return 404 if not found
            }
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(visits);
        }

        [HttpPost]
        public IActionResult EditPatientVisits(patient_visits visits)
        {
            patientRepository.Updatepatient_visits(visits);
            return RedirectToAction("DisplayPatientVisits");
        }

        public IActionResult DeletePatientVisits(int id)
        {
            patientRepository.Deletepatient_visits(id);

            return RedirectToAction("DisplayPatientVisits");

        }


        [HttpGet]
        public IActionResult PatientInsurance()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }
        [HttpPost]
        public IActionResult PatientInsurance(patient_insurance insurance)
        {
            patientRepository.Addpatient_insurance(insurance);
            return RedirectToAction("DisplayPatientInsurance");
        }
        public IActionResult DisplayPatientInsurance()
        {
            var insurance = patientRepository.GetAllpatient_insurance();
            return View(insurance);
        }
        [HttpGet]
        public IActionResult EditInsurance(int id)
        {
            var insurance = patientRepository.Getpatient_insuranceById(id);
            if (insurance == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(insurance);
        }

        [HttpPost]
        public IActionResult EditInsurance(patient_insurance insurance)
        {
            patientRepository.Updatepatient_insurance(insurance);
            return RedirectToAction("DisplayPatientInsurance");
        }

        public IActionResult DeleteInsurance(int id)
        {
            patientRepository.Deletepatient_insurance(id);

            return RedirectToAction("DisplayPatientInsurance");

        }

        [HttpGet]
        public IActionResult PatientDocuments()
        {
            int patientId = Convert.ToInt32(HttpContext.Session.GetString("PatientId")); // or use your login logic
            var model = new patient_documents
            {
                PatientId = patientId
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult PatientDocuments(patient_documents documents, IFormFile DocumentFile)
        {
            int patientId = Convert.ToInt32(HttpContext.Session.GetString("PatientId"));
            documents.PatientId = patientId;
            int uploadedBy = patientId;
            patientRepository.Addpatient_document(documents, DocumentFile, uploadedBy);

            return RedirectToAction("DocumentsDetails");
        }



        public IActionResult DisplayPatientDocuments()
        {
            var documents = patientRepository.GetAllpatient_documents();
            return View(documents);
        }
        [HttpGet]
        public IActionResult EditPatientDocuments(int id)
        {
            var documents = patientRepository.Getpatient_documentById(id);
            if (documents == null)
            {
                return NotFound(); // Return 404 if not found
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(documents);
        }

        [HttpPost]
        public IActionResult EditPatientDocuments(patient_documents documents, IFormFile DocumentFile, int uploadedBy)
        {
            patientRepository.Updatepatient_document(documents, DocumentFile, uploadedBy);
            return RedirectToAction("DisplayPatientDocuments");
        }

        public IActionResult DeletePatientDocuments(int id)
        {
            patientRepository.Deletepatient_document(id);

            return RedirectToAction("DisplayPatientDocuments");

        }
        public IActionResult DownloadDocument(string filename)
        {
            // Ensure the file path is valid and accessible
            var path = Path.Combine(_env.WebRootPath, "uploads", filename); // Assuming your uploads are in wwwroot/uploads

            if (!System.IO.File.Exists(path))
            {
                return NotFound(); // Return 404 if file doesn't exist
            }

            var contentType = "application/octet-stream"; // Default MIME type
            return File(System.IO.File.ReadAllBytes(path), contentType, filename);
        }


        public IActionResult ViewDocument(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || filename.Contains(".."))
            {
                return BadRequest("Invalid file name.");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Document not found");
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            // Set content disposition to 'inline' to open in browser
            Response.Headers.Add("Content-Disposition", "inline; filename=" + filename);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType);
        }


        [HttpGet]
        public IActionResult PatientRegister()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult PatientRegister(PatientRegistration patient, IFormFile patient_img)
        {
            patientRepository.Add(patient, patient_img);
            return RedirectToAction("PatientLogin");
        }
        [HttpGet]
        public IActionResult PatientLogin(string returnUrl = null)
        {
            if (HttpContext.Session.GetString("PatientId") != null)
            {
                // Redirect the user to the dashboard if already logged in
                return Redirect(returnUrl ?? Url.Action("PatientDashboard", "Patient"));
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientLogin(PatientLogin model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Authenticate the patient using the repository
            var patient = patientRepository.Authenticate(model.Email, model.Password);

            if (patient == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            // Store the patient details in session
            HttpContext.Session.SetString("PatientId", patient.PatientId.ToString());
            HttpContext.Session.SetString("PatientName", patient.FirstName);

            // Redirect to the original returnUrl or the dashboard
            return Redirect(returnUrl ?? Url.Action("PatientDashboard", "Patient"));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PatientLogin", "Patient");
        }

        public IActionResult PatientDashboard()
        {
            var patientIdString = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientIdString))
            {
                return RedirectToAction("PatientLogin");
            }

            int patientId = Convert.ToInt32(patientIdString);
            var patient = patientRepository.GetById(patientId);

            if (patient == null)
            {
                return RedirectToAction("PatientLogin");
            }


            // 🧠 Set patient profile ViewBag
            ViewBag.PatientId = patient.PatientId;
            ViewBag.PatientName = $"{patient.FirstName} {patient.LastName}";
            ViewBag.DOB = patient.Dob;
            ViewBag.Gender = patient.Gender;
            ViewBag.MaritalStatus = patient.MaritalStatus;
            ViewBag.Email = patient.Email;
            ViewBag.PhoneNumber = patient.PhoneNumber;
            ViewBag.Address = patient.Address;
            ViewBag.EmergencyContactName = patient.EmergencyContactName;
            ViewBag.EmergencyContactPhone = patient.EmergencyContactPhone;
            ViewBag.RegisteredAt = patient.CreatedAt;
            ViewBag.UpdatedAt = patient.UpdatedAt;
            ViewBag.ProfileImage = patient.patient_img;

            // 👨‍⚕️ Fetch all doctors
            var doctors = doctorrepository.GetAllDoctors() ?? new List<Doctor>();
            var doctorDictionary = doctors.ToDictionary(d => d.DoctorId, d => d.FullName);

            // 📅 Upcoming appointments
            var appointmentsRaw = appointmentRepository.GetAllAppointments()
                .Where(v => v.PatientId == patientId && v.AppointmentDate >= DateTime.Now)
                .Select(v => new AppointmentInfo
                {
                    AppointmentDate = v.AppointmentDate,
                    DoctorName = v.Doctor.FullName ?? "N/A",  // Use DoctorName from SQL or EF
                    Reason = v.Reason
                })
                .ToList();

            ViewBag.appointmentsRaw = appointmentsRaw;


            // 🏥 Patient visits
            var visits = patientRepository.GetAllpatient_visits()
                .Where(v => v.PatientId == patientId)
                .Select(v => new
                {
                    Date = v.VisitDate.ToString("dd-MMM-yyyy"),
                    Department = v.Department,
                    Summary = v.VisitReason
                })
                .ToList();
            ViewBag.Visits = visits;



            // 💳 Billing
            // 💳 Billing
            var bills = billingRepository.GetAllInvoices()
                .Where(r => r.PatientId == patientId && r.Status != "Paid") // Only show unpaid
                .Select(r => new
                {
                    InvoiceId = r.InvoiceId,
                    Date = r.InvoiceDate.ToString("dd-MMM-yyyy"),
                    Amount = r.Amount,
                    Description = r.Description,
                    Status = r.Status // Include Status in the anonymous object
                })
                .ToList();

            ViewBag.Bills = bills;



            // 🛡️ Insurance
            var insurance = patientRepository.GetAllpatient_insurance()
     .FirstOrDefault(i => i.PatientId == patientId);

            if (insurance != null)
            {
                ViewBag.Insurance = new
                {
                    Provider = insurance.InsuranceProvider,
                    PolicyNumber = insurance.PolicyNumber,
                    CoverageType = insurance.CoverageType,
                    CoverageAmount = insurance.CoverageAmount,
                    ValidFrom = insurance.ValidFrom.ToString("dd MMM yyyy"),
                    ValidTill = insurance.ValidUntil.ToString("dd MMM yyyy")
                };
            }
            else
            {
                ViewBag.Insurance = null;
            }

            // 📄 Patient Documents
            var patientDocuments = patientRepository.GetDocumentsByPatientId(patientId) ?? new List<patient_documents>();
            ViewBag.PatientDocuments = patientDocuments;

            // 📦 Dashboard model
            var model = new DashboardViewModel
            {
                UpcomingAppointments = appointmentsRaw
            };

            // Get the list of patient reports from the database as LabReportViewModel2
            var patientReports = labTestRepository.GetReportsByPatientId(patientId) ?? new List<LabReportViewModel2>();

            // Pass the list of reports to the view using ViewBag
            ViewBag.PatientReports = patientReports;
            return View(model);
        }





        public IActionResult AppointmentDetails()
        {
            var patientIdString = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientIdString))
            {
                return RedirectToAction("PatientLogin");
            }

            int patientId = Convert.ToInt32(patientIdString);

            var appointments = appointmentRepository.GetAppointmentsByPatientId(patientId);

            if (appointments == null || !appointments.Any())
            {
                ViewBag.Message = "No appointments found.";
                return View(new List<Appointment>()); // Return empty list to avoid null reference
            }

            return View(appointments);
        }
     
        public IActionResult VisitsDetails()
        {
            var patientIdString = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientIdString))
            {
                return RedirectToAction("PatientLogin");
            }

            int patientId = Convert.ToInt32(patientIdString);

            var visits = patientRepository.GetAllpatient_visits()
                .Where(v => v.PatientId == patientId)
                .ToList();

            return View(visits); // Create VisitsDetails.cshtml and pass List<VisitModel>
        }
        public IActionResult InsuranceDetails()
        {
            var patientIdString = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientIdString))
            {
                return RedirectToAction("PatientLogin");
            }

            int patientId = Convert.ToInt32(patientIdString);

            // Get all insurance entries for this patient
            var insuranceList = patientRepository.GetAllpatient_insurance()
                .Where(i => i.PatientId == patientId)
                .ToList();

            return View(insuranceList); // Create InsuranceDetails.cshtml, use model: List<patient_insurance>
        }

        public IActionResult DocumentsDetails()
        {
            var patientIdString = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientIdString))
            {
                return RedirectToAction("PatientLogin");
            }

            int patientId = Convert.ToInt32(patientIdString);
            var documents = patientRepository.GetDocumentsByPatientId(patientId).ToList();

            return View(documents);
        }


        public IActionResult LabReportsDetails()
        {
            var patientIdString = HttpContext.Session.GetString("PatientId");

            if (string.IsNullOrEmpty(patientIdString))
            {
                return RedirectToAction("PatientLogin");
            }

            int patientId = Convert.ToInt32(patientIdString);

            var reports = labTestRepository.GetReportsByPatientId(patientId).ToList();

            return View(reports);
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        // Handle Reset Password Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("ForgotPassword");
            }

            // Logic to reset the password
            var patient = patientRepository.GetPatientByEmail(email); // Use your service to get the patient by email
            if (patient == null)
            {
                ModelState.AddModelError("", "Email not found.");
                return View("ForgotPassword");
            }

            // Reset password in the database (you may hash the password)
            patientRepository.UpdatePassword(patient.PatientId, newPassword);

            TempData["PasswordResetSuccess"] = "Your password has been successfully reset.";
            return RedirectToAction("PatientLogin");
        }



   

    }
}

