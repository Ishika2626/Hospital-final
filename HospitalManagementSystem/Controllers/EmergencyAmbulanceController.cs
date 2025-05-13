using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace HospitalManagementSystem.Controllers
{
    public class EmergencyAmbulanceController : Controller
    {
        
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorrepository;
        private readonly IPharmacyRepository pharmacyRepository;
        private readonly IStaffRepository staffRepository;
        public EmergencyAmbulanceController(IPatientRepository patientRepository, IDoctorRepository doctorrepository, IPharmacyRepository pharmacyRepository, IStaffRepository staffRepository)
        {
            this.patientRepository = patientRepository;
            this.doctorrepository = doctorrepository;
            this.pharmacyRepository = pharmacyRepository;
          this.staffRepository = staffRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DisplayAmbulance()
        {
            var request = pharmacyRepository.GetAllAmbulances();
            return View(request);
        }

        [HttpGet]
        public IActionResult AmbulanceRequests()
        {

            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.ambulanceNumber = pharmacyRepository.GetAllAmbulances();
            ViewBag.Drivers = staffRepository.GetAll();
            return View();
        }
        [HttpPost]
        public IActionResult AmbulanceRequests(AmbulanceRequest request)
        {
            pharmacyRepository.AddAmbulanceRequest(request);
            return RedirectToAction("DisplayAmbulanceRequests");
        }

        public IActionResult DisplayAmbulanceRequests()
        {

            var request = pharmacyRepository.GetAllAmbulanceRequests();
            return View(request);
        }
        [HttpGet]
        public IActionResult EditAmbulanceRequests(int id)
        {
            var request = pharmacyRepository.GetAmbulanceRequestById(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.ambulanceNumber = pharmacyRepository.GetAllAmbulances();
            ViewBag.Drivers = staffRepository.GetAll();
            return View(request);
        }

        [HttpPost]
        public IActionResult EditAmbulanceRequests(AmbulanceRequest request)
        {
            pharmacyRepository.UpdateAmbulanceRequest(request);
            return RedirectToAction("DisplayAmbulanceRequests");
        }

        public IActionResult DeleteAmbulanceRequests(int id)
        {
            pharmacyRepository.DeleteAmbulanceRequest(id);

            return RedirectToAction("DisplayAmbulanceRequests");

        }

        [HttpGet]
        public IActionResult EmergencyCases()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.doctorName = doctorrepository.GetDoctorName();
            return View();
        }
        [HttpPost]
        public IActionResult EmergencyCases(EmergencyCase cases)
        {
            pharmacyRepository.AddEmergencyCase(cases);
            return RedirectToAction("DisplayEmergencyCases");
        }
        public IActionResult DisplayEmergencyCases()    
        {

            var cases = pharmacyRepository.GetAllEmergencyCases();
            return View(cases);
        }

        [HttpGet]
        public IActionResult EditEmergencyCases(int id)
        {
            var request = pharmacyRepository.GetEmergencyCaseById(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.doctorName = doctorrepository.GetDoctorName();

            return View(request);
        }

        [HttpPost]
        public IActionResult EditEmergencyCases(EmergencyCase cases)
        {
            pharmacyRepository.UpdateEmergencyCase(cases);
            return RedirectToAction("DisplayEmergencyCases");
        }

        public IActionResult DeleteEmergencyCases(int id)
        {
            pharmacyRepository.DeleteEmergencyCase(id);

            return RedirectToAction("DisplayEmergencyCases");

        }


    }
}
