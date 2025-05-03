using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class StaffHomeController : Controller
    {
        private readonly IStaffRepository staffRepository;
        public StaffHomeController(IStaffRepository staffRepository)
        {
            this.staffRepository = staffRepository;
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

        //[HttpGet]
        //public IActionResult MarkAttendance()
        //{
        //    var employeeId = HttpContext.Session.GetInt32("EmployeeId");

        //    if (!employeeId.HasValue)
        //    {
        //        TempData["ErrorMessage"] = "You must be logged in to mark attendance!";
        //        return RedirectToAction("Login", "RegisterLogin");
        //    }

        //    ViewData["EmployeeId"] = employeeId.Value;

        //    return View();
        //}

        //// POST: /Attendance/Mark
        //[HttpPost]
        //public IActionResult MarkAttendance(AttendanceModel attendance)
        //{
        //    if (attendance.EmployeeId <= 0)
        //    {
        //        TempData["ErrorMessage"] = "Invalid employee data!";
        //        return RedirectToAction("Mark");
        //    }

        //    try
        //    {
        //        // Call the repository to insert attendance
        //        staffRepository.MarkAttendance(attendance);

        //        TempData["SuccessMessage"] = "Attendance marked successfully!";
        //        return RedirectToAction("MarkAttendance");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error: {ex.Message}";
        //        return RedirectToAction("MarkAttendance");
        //    }
        //}

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
            return View();
        }
        public IActionResult ReceptionistAddAppointments()
        {
            return View();
        }
        public IActionResult ReceptionistPatientRecords()
        {
            return View();
        }
        public IActionResult ReceptionistVisitorsLog()
        {
            return View();
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
            return View();
        }
        public IActionResult LTAddNewLabTest()
        {
            return View();
        }
        public IActionResult LTPerformLabTest()
        {
            return View();
        }
        public IActionResult LTViewLabTest()
        {
            return View();
        }
        public IActionResult LTTestReports()
        {
            return View();
        }
        public IActionResult LTAddTestReports()
        {
            return View();
        }
        public IActionResult LTViewTestReports()
        {
            return View();
        }
        public IActionResult LTEditTestReports()
        {
            return View();
        }
        public IActionResult LTDeleteTestReports()
        {
            return View();
        }
        public IActionResult LTLabInventory()
        {
            return View();
        }
        public IActionResult LTLabInventoryAdd()
        {
            return View();
        }
        public IActionResult LTLabInventoryView()
        {
            return View();
        }
        public IActionResult LTLabInventoryEdit()
        {
            return View();
        }
        public IActionResult LTLabInventoryDelete()
        {
            return View();
        }
        public IActionResult LTManagePatietSamples()
        {
            return View();
        }
        public IActionResult LTManagePatietSamplesAdd()
        {
            return View();
        }
        public IActionResult LTManagePatietSamplesVew()
        {
            return View();
        }
        public IActionResult LTManagePatietSamplesEdit()
        {
            return View();
        }
        public IActionResult LTManagePatietSamplesDelete()
        {
            return View();
        }
        public IActionResult LTEquipmentMaintenance()
        {
            return View();
        }
        public IActionResult LTEquipmentMaintenanceAdd()
        {
            return View();
        }
        public IActionResult LTEquipmentMaintenanceView()
        {
            return View();
        }
        public IActionResult LTEquipmentMaintenanceDelete()
        {
            return View();
        }
        public IActionResult LTEquipmentMaintenanceEdit()
        {
            return View();
        }
        public IActionResult LTTestRequests()
        {
            return View();
        }
        public IActionResult LTTestRequestsProcess()
        {
            return View();
        }
        public IActionResult LTTestRequestsView()
        {
            return View();
        }

        public IActionResult LTTestRequestsDelete()
        {
            return View();
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

        public IActionResult HRPayroll()
        {
            return View();
        }
        [HttpGet]
        public ActionResult MarkAttendance()
        {
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
            {
                return RedirectToAction("Login"); // or show an error
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


        public IActionResult HRPerformance()
        {
            return View();
        }
        public IActionResult HRTraining()
        {
            return View();
        }
        public IActionResult HRComplaints()
        {
            return View();
        }
        public IActionResult HRLeaves()
        {
            return View();
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
        public IActionResult BBSDonors()
        {
            return View();
        }
        public IActionResult BBSInventory()
        {
            return View();
        }
        public IActionResult BBSTesting()
        {
            return View();
        }
        public IActionResult BBSRequests()
        {
            return View();
        }
        public IActionResult BBSEmergency()
        {
            return View();
        }
        public IActionResult BBSReports()
        {
            return View();
        }

    }

}
