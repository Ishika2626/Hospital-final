using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagementSystem.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly IPharmacyRepository pharmacyRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IBillingRepository billingRepository;
        private readonly IDoctorRepository doctorRepository;
        public PharmacyController(IPharmacyRepository pharmacyRepository,IPatientRepository patientRepository, IBillingRepository billingRepository, IDoctorRepository doctorRepository)
        {
            this.pharmacyRepository = pharmacyRepository;
            this.patientRepository = patientRepository;
            this.billingRepository = billingRepository;
            this.doctorRepository = doctorRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Medicines()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Medicines(Medicine medicine)
        {
            pharmacyRepository.AddMedicine(medicine);
            return RedirectToAction("DisplayMedicines");
          
        }
        public IActionResult DisplayMedicines()
        {
            var medicine = pharmacyRepository.GetAllMedicines();
            return View(medicine);
        }
        [HttpGet]
        public IActionResult EditMedicines(int id)
        {
            var medicine = pharmacyRepository.GetMedicineById(id);
            if (medicine == null)
            {
                return NotFound();
            }
            
            return View(medicine);
        }

        [HttpPost]
        public IActionResult EditMedicines(Medicine medicine)
        {
            pharmacyRepository.UpdateMedicine(medicine);
            return RedirectToAction("DisplayMedicines");
        }

        public IActionResult DeleteMedicines(int id)
        {
            pharmacyRepository.DeleteMedicine(id);

            return RedirectToAction("DisplayMedicines");

        }
        [HttpGet]
        public IActionResult Prescriptions()
        {
            ViewBag.doctorName = doctorRepository.GetDoctorName();
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }
        [HttpPost]
        public IActionResult Prescriptions(PharmacyPrescriptionEntity pp)
        {
            pharmacyRepository.AddPrescription(pp);
            return RedirectToAction("DisplayPrescriptions");
        }
        public IActionResult DisplayPrescriptions()
        {
            var medicine = pharmacyRepository.GetAllPrescriptions();
            return View(medicine);
        }
        [HttpGet]
        public IActionResult EditPrescriptions(int id)
        {
            var medicine = pharmacyRepository.GetPrescriptionById(id);
            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        [HttpPost]
        public IActionResult EditPrescriptions(PharmacyPrescriptionEntity medicine)
        {
            pharmacyRepository.UpdatePrescription(medicine);
            return RedirectToAction("DisplayPrescriptions");
        }

        public IActionResult DeletePrescriptions(int id)
        {
            pharmacyRepository.DeletePrescription(id);

            return RedirectToAction("DisplayPrescriptions");

        }


        [HttpGet]
        public IActionResult PharmacyOrders()
        {
            var medicines = pharmacyRepository.GetMedicineList();

            ViewBag.medicineName = medicines.Select(m => new SelectListItem
            {
                Value = m.MedicineId.ToString(),
                Text = m.MedicineName
            }).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult PharmacyOrders(PharmacyOrder po)
        {
            pharmacyRepository.AddPharmacyOrder(po);
            return RedirectToAction("DisplayPharmacyOrders");
        }
        public IActionResult DisplayPharmacyOrders()
        {
            var medicine = pharmacyRepository.GetAllPharmacyOrders();
            return View(medicine);
        }

        [HttpGet]
        public IActionResult EditPharmacyOrders(int id)
        {
            var medicine = pharmacyRepository.GetPharmacyOrderById(id);
            if (medicine == null)
            {
                return NotFound();
            }
            var medicines = pharmacyRepository.GetMedicineList();

            ViewBag.medicineName = medicines.Select(m => new SelectListItem
            {
                Value = m.MedicineId.ToString(),
                Text = m.MedicineName
            }).ToList();
            return View(medicine);
        }

        [HttpPost]
        public IActionResult EditPharmacyOrders(PharmacyOrder medicine)
        {
            pharmacyRepository.UpdatePharmacyOrder(medicine);
            return RedirectToAction("DisplayPharmacyOrders");
        }

        public IActionResult DeletePharmacyOrders(int id)
        {
            pharmacyRepository.DeletePharmacyOrder(id);

            return RedirectToAction("DisplayPharmacyOrders");

        }


        [HttpGet]
        public IActionResult PharmacyStock()
        {
            var medicines = pharmacyRepository.GetMedicineList();

            ViewBag.medicineName = medicines.Select(m => new SelectListItem
            {
                Value = m.MedicineId.ToString(),
                Text = m.MedicineName
            }).ToList();
            return View();
        }
        [HttpPost]
        public IActionResult PharmacyStock(PharmacyStock ps)
        {
            pharmacyRepository.AddPharmacyStock(ps);
            return RedirectToAction("DisplayPharmacyStock");
        }
        public IActionResult DisplayPharmacyStock()
        {
            var medicine = pharmacyRepository.GetAllPharmacyStock();
            return View(medicine);
        }

        [HttpGet]
        public IActionResult EditPharmacyStock(int id)
        {
            var medicine = pharmacyRepository.GetPharmacyStockById(id);
            if (medicine == null)
            {
                return NotFound();
            }
            var medicines = pharmacyRepository.GetMedicineList();

            ViewBag.medicineName = medicines.Select(m => new SelectListItem
            {
                Value = m.MedicineId.ToString(),
                Text = m.MedicineName
            }).ToList();
            return View(medicine);
        }

        [HttpPost]
        public IActionResult EditPharmacyStock(PharmacyStock medicine)
        {
            pharmacyRepository.UpdatePharmacyStock(medicine);
            return RedirectToAction("DisplayPharmacyStock");
        }

        public IActionResult DeletePharmacyStock(int id)
        {
            pharmacyRepository.DeletePharmacyStock(id);

            return RedirectToAction("DisplayPharmacyStock");

        }
    }
}
