
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
        

        public IActionResult GetAllDonors(string search = "", int page = 1)
        {
            int pageSize = 10;
            var donors = _bloodbankRepo.GetPagedDonors(page, pageSize, search);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(_bloodbankRepo.GetDonorCount(search) / (double)pageSize);
            ViewBag.Search = search;
            return View(donors);  // should be List<Donor>
        }

        public PartialViewResult DonorsTablePartial(string search = "", int page = 1)
        {
            int pageSize = 10;
            var donors = _bloodbankRepo.GetPagedDonors(page, pageSize, search);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(_bloodbankRepo.GetDonorCount(search) / (double)pageSize);
            ViewBag.Search = search;
            return PartialView("_DonorTablePartial", donors);  // same
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

        public IActionResult Donors(string search = "", int page = 1)
        {
            int pageSize = 10;
            var donors = _bloodbankRepo.GetPagedDonors(page, pageSize, search);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(_bloodbankRepo.GetDonorCount(search) / (double)pageSize);
            ViewBag.Search = search;
            return View(donors);
        }

        public IActionResult DonorsPartial(string search = "", int page = 1)
        {
            int pageSize = 10;
            var donors = _bloodbankRepo.GetPagedDonors(page, pageSize, search);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(_bloodbankRepo.GetDonorCount(search) / (double)pageSize);
            ViewBag.Search = search;
            return PartialView("_DonorTablePartial", donors);
        }


        [HttpGet]
        public IActionResult BloodBags(string searchTerm = "", int page = 1)
        {
            int pageSize = 10;

            var pagedBags = _bloodbankRepo.GetBloodBagsPagedWithSearch(searchTerm, page, pageSize);
            int totalItems = _bloodbankRepo.GetTotalBloodBagsCountWithSearch(searchTerm);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Donors = _bloodbankRepo.GetAllDonors();

            return View(pagedBags);
        }

        [HttpGet]
        public JsonResult AutoSearch(string term)
        {
            var matches = _bloodbankRepo.GetMatchingBloodTypes(term);
            return Json(matches);
        }


        [HttpPost]
        public IActionResult AddDonation(int DonorId, string BloodType)
        {
            try
            {
                _bloodbankRepo.RecordDonation(DonorId, BloodType);
                TempData["Success"] = "Donation recorded successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to record donation: " + ex.Message;
            }

            return RedirectToAction("BloodBags");
        }

        [HttpGet]
        public IActionResult GetEligibleDonorsByBloodType(string bloodType)
        {
            if (string.IsNullOrEmpty(bloodType))
                return BadRequest("Blood type is required");

            var donors = _bloodbankRepo.GetEligibleDonorsByBloodType(bloodType);
            return Json(donors); // Returns List<Donor> as JSON
        }

        public IActionResult DonationHistory()
        {
            var data = _bloodbankRepo.GetAllDonationHistory();
            return View(data);
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
