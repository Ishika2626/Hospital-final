
using HospitalManagementSystem;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using hospitalManagementSystem.Models;

namespace HospitalManagementSystem.Controllers
{
    public class BloodBankController : Controller
    {
        private static IBloodbankRepo _bloodbankRepo;
        public BloodBankController(IBloodbankRepo bloodbankRepo)
        {
            _bloodbankRepo = bloodbankRepo;
        }
        // View all donors
        public ActionResult GetAllDonors()
        {
            var donors = _bloodbankRepo.GetAllDonors();
            return View(donors);
        }

        // View details of a single donor
        public ActionResult Details(int id)
        {
            Donor donor = _bloodbankRepo.GetDonorById(id);
            if (donor == null)
            {
                return NotFound();
            }
            return View(donor);
        }

        [HttpPost]
        public IActionResult AddDonor(Donor donor)
        {
            _bloodbankRepo.AddDonor(donor);
            return RedirectToAction("GetAllDonors");
        }

        // Update an existing donor
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Donor donor = _bloodbankRepo.GetDonorById(id);
            if (donor == null)
            {
                return NotFound();
            }
            return View(donor);
        }

        [HttpPost]
        public IActionResult UpdateEligibility(int id, string eligibilityStatus)
        {
            _bloodbankRepo.UpdateEligibility(id, eligibilityStatus);
            return RedirectToAction("GetAllDonors");
        }

        [HttpPost]
        public ActionResult Edit(Donor donor)
        {
            if (ModelState.IsValid)
            {
                _bloodbankRepo.UpdateDonor(donor);
                return RedirectToAction("Index");
            }

            return View(donor);
        }

        // Delete a donor (soft delete)
        public ActionResult Delete(int id)
        {
            _bloodbankRepo.DeleteDonor(id);
            return RedirectToAction("Index");
        }
    
       

        public IActionResult Donors()
        {
            return View();
        }
        public IActionResult DisplayDonors()
        {
            return View();
        }
        public IActionResult BloodBags()
        {
            return View();
        }
        public IActionResult DisplayBloodBags()
        {
            return View();
        }
        public IActionResult BloodRequests()
        {
            return View();
        }
        public IActionResult DisplayBloodRequests()
        {
            return View();
        }
        public IActionResult BloodTransfusions()
        {
            return View();
        }
        public IActionResult DisplayBloodTransfusions()
        {
            return View();
        }
        public IActionResult BloodDonationHistory()
        {
            return View();
        }
        public IActionResult DisplayBloodDonationHistory()
        {
            return View();
        }
        public IActionResult Inventory()
        {
            return View();
        }
        public IActionResult DisplayInventory()
        {
            return View();
        }
        public IActionResult BloodTests()
        {
            return View();
        }
        public IActionResult DisplayBloodTests()
        {
            return View();
        }
        public IActionResult BloodStorage()
        {
            return View();
        }
        public IActionResult DisplayBloodStorage()
        {
            return View();
        }
        public IActionResult UserBloodRequest()
        {
            return View();
        }

        public IActionResult UserDonorRegistre()
        {
            return View();
        }

        public IActionResult campaign()
        {
            return View();
        }

        public IActionResult EductionBloodBank()
        {
            return View();
        }
    }
}
