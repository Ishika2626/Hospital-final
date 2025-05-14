using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace HospitalManagementSystem.Controllers
{
    public class StaffHomeController : Controller
    {
        private readonly IStaffRepository staffRepository;
        private readonly ILabTestRepository labTestRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IAppointmentRepository appointmentRepository;
        public StaffHomeController(IStaffRepository staffRepository, IAppointmentRepository appointmentRepository, ILabTestRepository labTestRepository, IPatientRepository patientRepository, IDoctorRepository doctorRepository)
        {
            this.staffRepository = staffRepository;
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
            this.labTestRepository = labTestRepository;
            this.appointmentRepository = appointmentRepository;
        }

        public IActionResult ReceptionistDashboard()
        {
            return View();
        }
        public IActionResult NurseDashboard()
        {
            // Step 1: Get Nurse's EmployeeId from the session
            var idString = HttpContext.Session.GetInt32("EmployeeId");

            if (idString == null)
            {
                ViewBag.Error = "Session value 'EmployeeId' is missing.";
                return RedirectToAction("Login", "Account"); // or redirect to the login page
            }

            int nurseId = idString.Value;

            // Step 2: Fetch assigned patients based on NurseId
            var patients = staffRepository.GetAssignedPatients(nurseId);

            // Debugging output
            Console.WriteLine("Nurse ID: " + nurseId);
            Console.WriteLine("Assigned Patients: " + (patients == null ? "null" : patients.Count.ToString()));

            // Step 3: Check if no patients were fetched or if patients are null
            if (patients == null || !patients.Any())
            {
                ViewBag.Error = "No patients assigned to you, or data could not be fetched.";
                return View(new List<PatientRegistration>());
            }

            // Step 4: Return the view with the list of patients
            return View(patients);  // You send the list of patients to the view
        }
        public IActionResult PharmacistDashboard()
        {
            return View();
        }
        public IActionResult LabTechnicianDashboard()
        {
            return View();
        }
        public IActionResult HRManagerDashboard()
        {
            return View();
        }
        public IActionResult BillingStaffDashboard()
        {
            return View();

        }
        public IActionResult EmergencyStaffDashboard()
        {
            return View();

        }
        public IActionResult BloodBankStaffDashboard()
        {
            return View();

        }

     

        public IActionResult StaffProfile()
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
            return RedirectToAction("StaffProfile");


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

            return RedirectToAction("StaffProfile"); // or wherever your profile page is
        }



        public IActionResult NurseMonitorPatient()
        {
            // Step 1: Get Nurse's EmployeeId from the session
            var idString = HttpContext.Session.GetInt32("EmployeeId");

            if (idString == null)
            {
                ViewBag.Error = "Session value 'EmployeeId' is missing.";
                return RedirectToAction("Login", "Account"); // Redirect to the login page if EmployeeId is missing
            }

            int nurseId = idString.Value;

            // Step 2: Fetch the patient status report for the nurse using the nurseId
            var report = staffRepository.GetPatientStatusReport(nurseId);

            // Step 3: Return the view with the report data
            return View(report);
        }

        [HttpPost]
        public IActionResult SaveVitals(PatientVitals model)
        {

            staffRepository.SavePatientVitals(model);
            Console.WriteLine($"PatientId: {model.PatientId}, BP: {model.BloodPressure}, HR: {model.HeartRate}, Temp: {model.Temperature}");
            return RedirectToAction("NurseMonitorPatient", "StaffHome");
        }



        public IActionResult NurseRecordVitals()
        {
            var data = staffRepository.GetVitalsReport();
            return View(data);
        }
        // GET: NurseAssistDischarge
        public IActionResult NurseAssistDischarge()
        {
            var idString = HttpContext.Session.GetInt32("EmployeeId");

            if (idString == null)
            {
                ViewBag.Error = "Session value 'EmployeeId' is missing.";
                return RedirectToAction("Login", "Account"); // Redirect to login if EmployeeId is missing
            }

            int nurseId = idString.Value;
            var data = staffRepository.GetDischargedPatientsByNurse(nurseId);
            ViewBag.Doctors = staffRepository.GetAllDoctors();
            return View(data);
        }


        // Process discharge action
        [HttpPost]
        public IActionResult Discharge(DischargeViewModel model)
        {
            // Update the discharge details
            staffRepository.UpdateDischargeDetails(
              model.AdmissionId,
              model.DischargeDate,
              model.DischargeReason,
              model.DoctorId
          );

            // Reload the updated patient list
            var nurseId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var data = staffRepository.GetDischargedPatientsByNurse(nurseId);
            ViewBag.Doctors = staffRepository.GetAllDoctors();

            // Return the NurseAssistDischarge view with refreshed data
            return View("NurseAssistDischarge", data);
        }



        public IActionResult ReceptionistManageAppointments()
        {
            var app=appointmentRepository.GetAllAppointments();
            ViewBag.Doctors = doctorRepository.GetAllDoctors();
            ViewBag.Patients = patientRepository.GetAll();
            return View(app);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReceptionistAddAppointments(Appointment appointment)
        {
            appointmentRepository.AddAppointment(appointment);
            ViewBag.Doctors = doctorRepository.GetAllDoctors();
            ViewBag.Patients = patientRepository.GetAll();
            return RedirectToAction("ReceptionistManageAppointments");
        }
        [HttpPost]
        public IActionResult RescheduleAppointment(int appointmentId, DateTime newDate)
        {
            appointmentRepository.UpdateAppointmentDate(appointmentId, newDate);
            return RedirectToAction("ReceptionistManageAppointments");
        }
        public IActionResult ReceptionistPatientRecords()
        {
            var patients = patientRepository.GetAll();
            return View(patients);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddPatient(PatientRegistration patient, IFormFile patient_img)
        {
            patientRepository.Add(patient, patient_img);
            return RedirectToAction("ReceptionistPatientRecords");
        }
        [HttpGet]
        public JsonResult GetDoctorsByDepartment(int departmentId)
        {
            var doctors = staffRepository.GetDoctorsByDepartment(departmentId)
                .Select(d => new { id = d.DoctorId, name = d.FullName }).ToList();

            return Json(doctors); // This will send a JSON response to the client
        }
        public IActionResult ReceptionistVisitorsLog()
        {
            var visits = patientRepository.GetAllpatient_visits();

            // Fetch patients and doctors from repository
            var patients = patientRepository.GetAll();
            var doctors = doctorRepository.GetAllDoctors();
            var departments = staffRepository.GetAllDepartments();

            // Assign them to ViewBag (still raw collections)
            ViewBag.patientName = patients;
            ViewBag.doctorName = doctors;
            ViewBag.Departments = departments;

            return View(visits);
        }
        [HttpPost]
        public ActionResult AddReceptionistVisitorsLog(patient_visits visit)
        {
            patientRepository.Addpatient_visits(visit);
            ViewBag.patientName = patientRepository.GetAll();
            ViewBag.doctorName = doctorRepository.GetAllDoctors();
            ViewBag.Departments = staffRepository.GetAllDepartments();
            return RedirectToAction("ReceptionistVisitorsLog");
        }
        public IActionResult PharmacistManageMedicines()
        {
            return View();
        }
        public IActionResult PharmacistAddMedicines()
        {
            return View();
        }
        public IActionResult PharmacistInventoryManagement()
        {
            return View();
        }
        public IActionResult PharmacistOrderStockInventory()
        {
            return View();
        }
        public IActionResult PharmacistPrescriptionHandling()
        {
            return View();
        }
        public IActionResult PharmacistMedicineRequest()
        {
            return View();
        }
        public IActionResult LTConductLabTest()
        {
            var bookings = labTestRepository.GetConfirmAndCollectedBookings();
            return View(bookings);
        }
        [HttpPost]
        public ActionResult Createreport(LabReport model)
        {

            model.ReportDate = DateTime.Now;

            var labtechid = HttpContext.Session.GetInt32("EmployeeId");
            model.LabTechnicianId = (int)labtechid;
            labTestRepository.AddLabReport(model);

            // Provide feedback to the user
            TempData["Message"] = "Lab report added successfully.";

            // Redirect to the page where lab tests are listed or reviewed
            return RedirectToAction("LTConductLabTest", "StaffHome");

        }


        public IActionResult LTTestReports()
        {
            IEnumerable<LabReportViewModel> labReports = labTestRepository.GetLabReports();
            return View(labReports);
        }


        public IActionResult LTManagePatietSamples()
        {
            var bookings = labTestRepository.GetConfirmBookings();
            return View(bookings);
        }
        [HttpPost]
        public IActionResult CollectSample(int bookingId)
        {
            labTestRepository.CollectSample(bookingId);
            return RedirectToAction("LTManagePatietSamples");
        }

        public IActionResult LTTestRequests()
        {
            var bookings = labTestRepository.GetAllBookings();
            return View(bookings);
        }
        [HttpPost]
        public IActionResult ConfirmTest(int bookingId)
        {
            labTestRepository.ConfirmTest(bookingId);
            return RedirectToAction("GetAllBookings");
        }

        public IActionResult HREmployee()
        {
            var roles = staffRepository.GetAllRoles();  // Assuming this fetches roles from the database
            ViewData["Roles"] = new SelectList(roles, "RoleId", "RoleName");
            var emp = staffRepository.GetAll();
            return View(emp);
        }

        private string GenerateLoginId(Employee employee, string roleName)
        {
            string fullName = (employee.FirstName + employee.LastName).ToLower().Replace(" ", "");
            string rolePart = roleName.ToLower().Replace(" ", "");
            return $"{rolePart}.{fullName}";
        }

        [HttpPost]
        public IActionResult CreateEmp(Employee employee)
        {

            // ✅ Step 1: Get role object using RoleId
            Role role = staffRepository.GetRoleById(Convert.ToInt16(employee.RoleId));
            if (role == null)
            {
                TempData["Error"] = "Invalid Role selected.";
                return View("HREmployee", employee); // Pass the same model back if invalid
            }

            // ✅ Step 2: Generate LoginId and Password using role.RoleName
            string loginId = GenerateLoginId(employee, role.RoleName);
            employee.LoginId = loginId;
            employee.Password = loginId;

            // ✅ Step 3: Insert employee with credentials
            staffRepository.Add(employee);



            // Step 4: Fetch the updated list of employees
            var employees = staffRepository.GetAll(); // Get the updated list of employees
            TempData["Success"] = $"Employee Created. Login ID & Password: {loginId}";
            return RedirectToAction("HREmployee"); // Return the list of employees to the view



        }
        [HttpPost]
        public IActionResult EditEmp(Employee employee)
        {
            staffRepository.Update(employee);
            return RedirectToAction("HREmployee");

        }
        [HttpPost]
        public IActionResult DeleteEmployee(int EmployeeId)
        {
            staffRepository.Delete(EmployeeId); // <- your repository method to delete
            return RedirectToAction("HREmployee"); // Redirect back to the employee list
        }


        public IActionResult HRHiring()
        {
            var jobs = staffRepository.GetAllJobs();
            return View(jobs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HiringManagement hiring)
        {
            try
            {
                // Directly add the job posting to the database without model validation
                staffRepository.AddJob(hiring);

                // Set a success message in TempData and redirect to the job list page
                TempData["SuccessMessage"] = "Job posting created successfully!";
                return RedirectToAction("HRHiring"); // Redirect to the jobs list after success
            }
            catch (Exception ex)
            {
                // Handle any errors during the process
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("HRHiring"); // Redirect on error
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Close(int id)
        {
            try
            {
                staffRepository.CloseJob(id); // Close the job using the repository

                TempData["SuccessMessage"] = "Job posting closed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("HRHiring");
        }
        [HttpPost]
        public IActionResult UpdateVacancies(int jobId, int vacancies)
        {
            staffRepository.UpdateVacancies(jobId, vacancies);
            return RedirectToAction("HRHiring"); // or your grid page action
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            staffRepository.DeleteJob(id);
            return RedirectToAction("HRHiring");
        }

        public IActionResult HRScheduling()
        {
            var schedules = staffRepository.GetAllSchedules();
            var employees = staffRepository.GetAll(); // Assuming you have a repository for employees

            ViewBag.Employees = employees;
            return View(schedules);
        }

        [HttpPost]
        public ActionResult CreateSchedule(SchedulingModel schedule)
        {
            staffRepository.InsertSchedule(schedule);  // Call the insert method of your repository
            return RedirectToAction("HRScheduling");
        }

        [HttpPost]
        public IActionResult UpdateSchedule(SchedulingModel model)
        {
            try
            {
                // Call the repository to update the schedule (void method)
                staffRepository.UpdateSchedule(model);

                // Redirect to the list page if update is successful
                return RedirectToAction("HRScheduling");
            }
            catch (Exception ex)
            {
                // Log the exception (you can log it in a file or a logging system)
                Console.WriteLine($"Error: {ex.Message}");

                // Return the same view with the error message
                ModelState.AddModelError("", "An error occurred while updating the schedule. Please try again.");
                return View(model); // Return the model to the view with the error message
            }
        }

        [HttpPost]
        public IActionResult DeleteSchedule(int ScheduleId)
        {
            try
            {
                // Call the repository to delete the schedule
                staffRepository.DeleteSchedule(ScheduleId);

                // Set success message and redirect to the ShiftScheduling page
                TempData["SuccessMessage"] = "Schedule deleted successfully.";
            }
            catch (Exception)
            {
                // If an exception is caught, set an error message
                TempData["ErrorMessage"] = "Failed to delete the schedule.";
            }

            return RedirectToAction("HRScheduling");
        }


        [HttpGet]
        public ActionResult MarkAttendance()
        {
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");
            string role = HttpContext.Session.GetString("Role"); // may be null if not admin

            if (employeeId == null)
            {
                return RedirectToAction("Login","RegisterLogin");
            }

            // Only override layout if admin
            if (role == "Admin")
            {
                ViewBag.Layout = "~/Views/Shared/admin.cshtml";
            }
            else
            {
                ViewBag.Layout = "~/Views/Shared/_stafflayout.cshtml";
            }

                DateTime today = DateTime.Today;

            var model = staffRepository.GetAttendanceForToday(employeeId.Value, today)
                ?? new AttendanceModel
                {
                    EmployeeId = employeeId.Value,
                    Date = today,
                    AttendanceStatus = "Present"
                };

            return View(model);
        }



        [HttpPost]
        public ActionResult MarkAttendance(AttendanceModel model)
        {
            var existing = staffRepository.GetAttendanceForToday(model.EmployeeId, model.Date);

            if (existing == null)
            {
                staffRepository.InsertPunchIn(model);
            }
            else if (existing.PunchOutTime == null)
            {
                staffRepository.UpdatePunchOut(model);
            }

            return RedirectToAction("MarkAttendance");
        }

        [HttpGet]
        public IActionResult HRAttendance()
        {
            var attendanceList = staffRepository.GetAllAttendance();
            return View(attendanceList);
        }



        [HttpGet]
        public IActionResult ApplyLeave()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ApplyLeave(LeaveRequest model)
        {

            // ✅ Get employee ID from session
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
            {
                ModelState.AddModelError("", "Session expired. Please log in again.");
                return View(model);
            }

            // Set required fields
            model.EmployeeId = employeeId.Value;
            model.Status = "Pending";

            try
            {
                // ✅ Call the repository method (which returns void)
                staffRepository.ApplyLeave(model); // No need for success check here

                // If no exception was thrown, we consider the request submitted
                TempData["Success"] = "✅ Leave request submitted!";
                return RedirectToAction("ApplyLeave"); // Redirect to prevent duplicate submissions
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Error submitting leave: " + ex.Message);
                return View(model); // Return the model with error message
            }
        }

        public IActionResult ViewLeaveRequests()
        {
            var requests = staffRepository.GetAllLeaveRequests();
            return View(requests);
        }

        [HttpPost]
        public IActionResult ApproveLeave(int id)
        {
            staffRepository.UpdateLeaveStatus(id, "Approved");
            return RedirectToAction("ViewLeaveRequests");
        }

        [HttpPost]
        public IActionResult RejectLeave(int id)
        {
            staffRepository.UpdateLeaveStatus(id, "Rejected");
            return RedirectToAction("ViewLeaveRequests");
        }

        public IActionResult ViewReview()
        {
            var loginId = HttpContext.Session.GetString("LoginID");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(loginId) || role != "HR Manager")
            {
                return RedirectToAction("Login", "Account");
            }

            var reviews = staffRepository.GetAllReviews();

            // Fetch employees excluding those with 'HR' role
            var employees = new List<SelectListItem>();
            string connectionString = "Data Source=SSMLEC_01\\MSSQLSERVERDEV22;Initial Catalog=hospitalManagementSystem;Persist Security Info=True;User ID=sa;Password=Admin@1234;Encrypt=False;Trust Server Certificate=True";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT employee_id, first_name, last_name FROM EmployeeManagement.Employees WHERE role_id != @ExcludedRole";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExcludedRole", 5);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fullName = $"{reader["first_name"]} {reader["last_name"]}";
                            employees.Add(new SelectListItem
                            {
                                Value = reader["employee_id"].ToString(),
                                Text = fullName
                            });
                        }
                    }
                }
            }

            ViewBag.Employees = employees;

            // Set reviewer info
            ViewBag.ReviewerId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            ViewBag.ReviewerName = HttpContext.Session.GetString("FullName") ?? "Unknown";

            return View(reviews);
        }

        [HttpPost]
        public IActionResult AddReview(PerformanceReview review)
        {
            // Call the repository method to add the review
            staffRepository.AddReview(review);
            TempData["Success"] = "Review added successfully.";
            TempData["Error"] = "Please fill in all required fields.";
            return RedirectToAction("ViewReview");
        }

        // Display all employee training records
        public IActionResult HRTraining()
        {
            var trainings = staffRepository.GetAllEmpTraining(); // Get all training records
                                                                 // Check if there's a session message to display
            ViewBag.Message = HttpContext.Session.GetString("Message");

            // Clear the session message after displaying it
            HttpContext.Session.Remove("Message");
            ViewBag.Employees = staffRepository.GetAll();
            ViewBag.Trainings = staffRepository.GetAllTrainings();  // Get available training programs for the dropdown
            return View(trainings);
        }

        // Handle the POST request to create a new EmployeeTraining record
        [HttpPost]
        public IActionResult Createtraining(EmployeeTraining training)
        {

            staffRepository.AddEmpTraining(training);
            TempData["Message"] = "Training record created successfully!";

            // If the model state is not valid, return the view with the existing data
            ViewBag.Employees = staffRepository.GetAll();  // Repopulate dropdown lists if there's an error
            ViewBag.Trainings = staffRepository.GetAllTrainings();
            return RedirectToAction("HRTraining");
        }

        [HttpPost]
        public IActionResult UpdateTrainingStatus(int TrainingId, string trainingStatus)
        {
            try
            {
                if (TrainingId <= 0 || string.IsNullOrEmpty(trainingStatus))
                {
                    TempData["Message"] = "Invalid input.";
                    return RedirectToAction("HRTraining");
                }

                staffRepository.UpdateTrainingStatus(TrainingId, trainingStatus);

                TempData["Message"] = "Training status updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
            }

            return RedirectToAction("HRTraining");
        }

        // This method will get the payroll data and filter it based on query parameters
        [HttpGet]
        public IActionResult HRPayroll()
        {

            // If no filter, return all payroll data
            var allPayrollData = staffRepository.GenerateMonthlyPayroll(DateTime.Now);
            return View(allPayrollData);

        }
        [HttpPost]
        public IActionResult HRPayroll(DateTime? payDate)
        {

            // Otherwise, filter based on the parameters
            var filteredPayrollData = staffRepository.GenerateMonthlyPayroll(payDate.Value);
            return View(filteredPayrollData);
        }

        // Action for downloading payroll data as PDF
        public IActionResult DownloadPdf(DateTime? payDate)
        {
            // Check if payDate is provided; otherwise, default to current date
            if (!payDate.HasValue)
            {
                payDate = DateTime.Now;
            }

            // Get payroll data for the given date
            var payrollData = staffRepository.GenerateMonthlyPayroll(payDate.Value);

            // Generate PDF from the payroll data (your existing PDF generation logic)
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, memoryStream);

                // Open the document
                document.Open();

                // Add title to the document
                document.Add(new Paragraph("Payroll Report"));

                // Create the table to hold the payroll data
                var table = new PdfPTable(7); // 7 columns in the table (excluding payment method)

                // Add headers to the table
                table.AddCell("Employee ID");
                table.AddCell("Base Salary");
                table.AddCell("Bonuses");
                table.AddCell("Overtime");
                table.AddCell("Deductions");
                table.AddCell("Total Salary");
                table.AddCell("Pay Date");

                // Add payroll data to the table
                foreach (var payroll in payrollData)
                {
                    table.AddCell(payroll.EmployeeId.ToString());
                    table.AddCell(payroll.BaseSalary.ToString("C"));
                    table.AddCell(payroll.Bonuses.ToString("C"));
                    table.AddCell(payroll.Overtime.ToString("C"));
                    table.AddCell(payroll.Deductions.ToString("C"));
                    table.AddCell(payroll.TotalSalary.ToString("C"));
                    table.AddCell(payroll.PayDate.ToString("yyyy-MM-dd"));
                }

                // Add the table to the document
                document.Add(table);

                // Close the document
                document.Close();

                // Return the PDF as a downloadable file
                return File(memoryStream.ToArray(), "application/pdf", "PayrollReport.pdf");
            }
        }

        public IActionResult SalarySlip(DateTime payDate)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (employeeId == null)
            {
                return RedirectToAction("Login", "RegisterLogin"); // or appropriate controller
            }

            var payroll = staffRepository.GetSalarySlip(employeeId.Value, payDate);

            if (payroll == null)
            {
                return NotFound("Salary slip not found.");
            }

            return View(payroll);
        }





        public IActionResult DownloadSalarySlip(DateTime payDate)
        {
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (!employeeId.HasValue)
                return RedirectToAction("Login", "RegisterLogin");

            var payroll = staffRepository.GetSalarySlip(employeeId.Value, payDate);

            if (payroll == null)
                return NotFound("Salary slip not found.");

            // Generate PDF using iTextSharp
            using (var stream = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                document.Add(new Paragraph($"Salary Slip for Employee ID: {payroll.EmployeeId}"));
                document.Add(new Paragraph($"Pay Date: {payroll.PayDate:yyyy-MM-dd}"));
                document.Add(new Paragraph($"Base Salary: {payroll.BaseSalary:C}"));
                document.Add(new Paragraph($"Bonuses: {payroll.Bonuses:C}"));
                document.Add(new Paragraph($"Overtime: {payroll.Overtime:C}"));
                document.Add(new Paragraph($"Deductions: {payroll.Deductions:C}"));
                document.Add(new Paragraph($"Total Salary: {payroll.TotalSalary:C}"));

                document.Close();

                return File(stream.ToArray(), "application/pdf", "SalarySlip.pdf");
            }
        }
        public IActionResult BSBilling()
        {
            return View();
        }
        public IActionResult BSInvoices()
        {
            return View();
        }
        public IActionResult BSInsurance()
        {
            return View();
        }
        public IActionResult BSPayments()
        {
            return View();
        }
        public IActionResult BSRefunds()
        {
            return View();

        }
        public IActionResult BSReports()
        {
            return View();

        }
        public IActionResult BSDues()
        {
            return View();
        }
        public IActionResult ESAlerts()
        {
            return View();
        }
        public IActionResult ESTriage()
        {
            return View();
        }
        public IActionResult ESBeds()
        {
            return View();
        }
        public IActionResult ESDispatch()
        {
            return View();
        }
        public IActionResult ESDoctors()
        {
            return View();
        }
        public IActionResult ESReports()
        {
            return View();
        }
        

    }

}
