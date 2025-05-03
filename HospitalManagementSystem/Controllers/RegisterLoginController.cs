using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Controllers
{
    public class RegisterLoginController : Controller
    {
        private readonly IStaffRepository staffRepository;

        public RegisterLoginController(IStaffRepository staffRepository)
        {
            this.staffRepository = staffRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Login(Employee model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = staffRepository.Login(model.LoginId, model.Password);

        //        if (user != null)
        //        {
        //            HttpContext.Session.SetString("LoginID", user.LoginId);
        //            HttpContext.Session.SetString("Role", user.RoleName);

        //            switch (user.RoleName.ToLower())
        //            {
        //                case "Nurse":
        //                    return RedirectToAction("NurseDashboard", "StaffHome");

        //                case "Receptionist":
        //                    return RedirectToAction("ReceptionistDashboard", "StaffHome");

        //                case "Pharmacist":
        //                    return RedirectToAction("PharmacistDashboard", "StaffHome");

        //                case "Lab Technician":
        //                    return RedirectToAction("LabTechnicianDashboard", "StaffHome");

        //                case "HR Manager":
        //                    return RedirectToAction("HRManagerDashboard", "StaffHome");

        //                case "Billing Staff":
        //                    return RedirectToAction("BillingStaffDashboard", "StaffHome");

        //                case "Emergency Staff":
        //                    return RedirectToAction("EmergencyStaffDashboard", "StaffHome");

        //                case "Blood Bank Staff":
        //                    return RedirectToAction("BloodBankStaffDashboard", "StaffHome");

        //                case "Admin":
        //                    return RedirectToAction("adminDashBoard", "DashBoard");

        //                default:
        //                    // Unknown role - maybe go to a generic home page
        //                    return RedirectToAction("Index", "Home");
        //            }
        //        }

        //        ModelState.AddModelError("", "Invalid login credentials.");
        //    }

        //    return View(model);
        //}


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string input)
        {
            var user = staffRepository.GetByLoginIdOrEmail(input);

            if (user == null)
            {
                ModelState.AddModelError("", "No user found with that Login ID or Email.");
                return View();
            }

            TempData["LoginId"] = user.LoginId;
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData["LoginId"] == null)
                return RedirectToAction("Login");

            ViewBag.LoginId = TempData["LoginId"].ToString();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string loginId, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                ViewBag.LoginId = loginId;
                return View();
            }

            bool result = staffRepository.UpdatePassword(loginId, newPassword);
            if (result)
            {
                TempData["Message"] = "Password updated successfully.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Failed to update password.");
            return View();
        }
    }
}

