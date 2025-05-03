using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class BillingPaymentsController : Controller
    {
        
        private readonly IPatientRepository patientRepository;
        private readonly IBillingRepository billingRepository;
        public BillingPaymentsController( IPatientRepository patientRepository, IBillingRepository billingRepository)
        {
           
            this.patientRepository = patientRepository;
            this.billingRepository = billingRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpGet]
        public IActionResult Invoicing()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }

        [HttpPost]
        public IActionResult Invoicing(Invoice invoice)
        {
            billingRepository.AddInvoice(invoice);
            return RedirectToAction("DisplayInvoicing");
        }
        public IActionResult DisplayInvoicing()
        {
            var invoices = billingRepository.GetAllInvoices();
            return View(invoices);

        }

        [HttpGet]
        public IActionResult EditInvoicing(int id)
        {
            var invoices = billingRepository.GetInvoiceById(id);
            if (invoices == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(invoices);
        }

        [HttpPost]
        public IActionResult EditInvoicing(Invoice invoices)
        {
            billingRepository.UpdateInvoice(invoices);
            return RedirectToAction("DisplayInvoicing");
        }

        public IActionResult DeleteInvoicing(int id)
        {
            billingRepository.DeleteInvoice(id);

            return RedirectToAction("DisplayInvoicing");

        }

        [HttpGet]
        public IActionResult InsuranceIntegration()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            return View();
        }

        [HttpPost]
        public IActionResult InsuranceIntegration(InsuranceIntegration invoice)
        {
            billingRepository.AddInsuranceClaim(invoice);
            return RedirectToAction("DisplayInsuranceIntegration");
        }
        [HttpGet]
        public IActionResult EditInsuranceIntegration(int id)
        {
            var invoices = billingRepository.GetInsuranceClaimById(id);
            if (invoices == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            return View(invoices);
        }

        [HttpPost]
        public IActionResult EditInsuranceIntegration(InsuranceIntegration invoices)
        {
            billingRepository.UpdateInsuranceClaim(invoices);
            return RedirectToAction("DisplayInsuranceIntegration");
        }

        public IActionResult DeleteInsuranceIntegration(int id)
        {
            billingRepository.DeleteInsuranceClaim(id);

            return RedirectToAction("DisplayInsuranceIntegration");

        }
        public IActionResult DisplayInsuranceIntegration()
        {
            var insuranceData =  billingRepository.GetAllInsuranceClaims();
            return View(insuranceData);
        }



        [HttpGet]
        public IActionResult PaymentTracking()
        {
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.invoicename = billingRepository.GetInvoiceList();
            return View();
        }

        [HttpPost]
        public IActionResult PaymentTracking(PaymentTracking invoice)
        {
            billingRepository.AddPayment(invoice);
            return RedirectToAction("DisplayPaymentTracking");
        }
     
        public IActionResult DisplayPaymentTracking()
        {
            var payments =  billingRepository.GetAllPayments();
            return View(payments);
        }
        [HttpGet]
        public IActionResult EditPaymentTracking(int id)
        {
            var invoices = billingRepository.GetPaymentById(id);
            if (invoices == null)
            {
                return NotFound();
            }
            ViewBag.patientName = patientRepository.GetPatientName();
            ViewBag.invoicename = billingRepository.GetInvoiceList();
            return View(invoices);
        }

        [HttpPost]
        public IActionResult EditPaymentTracking(PaymentTracking invoices)
        {
            billingRepository.UpdatePayment(invoices);
            return RedirectToAction("DisplayPaymentTracking");
        }

        public IActionResult DeletePaymentTracking(int id)
        {
            billingRepository.DeletePayment(id);

            return RedirectToAction("DisplayPaymentTracking");

        }
    }
}
