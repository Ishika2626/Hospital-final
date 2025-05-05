using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;


namespace HospitalManagementSystem.Controllers
{
    public class BillingPaymentsController : Controller
    {
        private readonly ILogger<BillingPaymentsController> _logger;
        private readonly RazorpayConfig _razorpayConfig;
        private readonly IPatientRepository patientRepository;
        private readonly IBillingRepository billingRepository;
        public BillingPaymentsController( IPatientRepository patientRepository, IBillingRepository billingRepository, ILogger<BillingPaymentsController> logger, RazorpayConfig razorpayConfig)
        {
           
            this.patientRepository = patientRepository;
            this.billingRepository = billingRepository;
            _logger = logger;
            _razorpayConfig = razorpayConfig;

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
        public IActionResult Invoicing(HospitalManagementSystem.Models.Invoice invoice)
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
        public IActionResult DisplayBill(int invoiceId)
        {
            var invoice = billingRepository.GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            var patient = patientRepository.GetById(invoice.PatientId);
            if (patient == null)
            {
                ViewBag.Patient = null; // Ensure null if patient not found
            }
            else
            {
                ViewBag.Patient = patient;
            }

            if (invoice.Amount >= 1.00m)
            {
                var options = new Dictionary<string, object>
        {
            { "amount", (int)(invoice.Amount * 100) }, // amount in paise
            { "currency", "INR" },
            { "receipt", $"INV{invoice.InvoiceId}" },
            { "payment_capture", 1 }
        };

                RazorpayClient client = new RazorpayClient(_razorpayConfig.Key, _razorpayConfig.Secret);
                Order order = client.Order.Create(options);

                string orderId = order["id"].ToString();  // Ensure order ID is a string
                ViewBag.OrderId = orderId;
                ViewBag.Key = _razorpayConfig.Key;

                // Store Razorpay order ID in invoice
                invoice.RazorpayOrderId = orderId;
                billingRepository.UpdateInvoice(invoice);
            }
            else
            {
                TempData["Error"] = "The invoice amount must be at least ₹1.";
            }

            ViewBag.Invoice = invoice;  // Make sure to pass the invoice object as well
            return View(invoice);
        }

        [HttpGet]
        public IActionResult ThankYou(string paymentId)
        {
            try
            {
                var razorpayClient = new RazorpayClient(_razorpayConfig.Key, _razorpayConfig.Secret);
                var paymentDetails = razorpayClient.Payment.Fetch(paymentId);

                ViewBag.PaymentId = paymentDetails["id"].ToString();
                ViewBag.Status = paymentDetails["status"].ToString();
                ViewBag.Amount = (Convert.ToDecimal(paymentDetails["amount"]) / 100).ToString("F2");

                return View();  // Return the ThankYou.cshtml view
            }
            catch (Exception)
            {
                TempData["Error"] = "Could not fetch payment details.";
                return RedirectToAction("DisplayBill");
            }
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
        public IActionResult EditInvoicing(HospitalManagementSystem.Models.Invoice invoices)
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
