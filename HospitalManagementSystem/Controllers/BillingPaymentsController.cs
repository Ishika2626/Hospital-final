using System.Reflection.Metadata;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
            // Fetch the invoice by ID
            var invoice = billingRepository.GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                return NotFound(); // Return 404 if invoice not found
            }

            // Fetch the associated patient using the PatientId from the invoice
            var patient = patientRepository.GetById(invoice.PatientId);
            if (patient == null)
            {
                ViewBag.Patient = null; // Set ViewBag.Patient to null if patient not found
            }
            else
            {
                ViewBag.Patient = patient;
            }

            // If the invoice amount is greater than or equal to ₹1, proceed with Razorpay order creation
            if (invoice.Amount >= 1.00m)
            {
                var options = new Dictionary<string, object>
            {
                { "amount", (int)(invoice.Amount * 100) }, // Convert amount to paise
                { "currency", "INR" },
                { "receipt", $"INV{invoice.InvoiceId}" },
                { "payment_capture", 1 }
            };

                // Initialize Razorpay client
                RazorpayClient client = new RazorpayClient(_razorpayConfig.Key, _razorpayConfig.Secret);
                Order order = client.Order.Create(options);

                // Store Razorpay order ID in ViewBag to pass it to the view
                string orderId = order["id"].ToString();
                ViewBag.OrderId = orderId;
                ViewBag.Key = _razorpayConfig.Key;

                // Update the invoice with the Razorpay order ID
                invoice.RazorpayOrderId = orderId;
                billingRepository.UpdateInvoice(invoice); // Save the Razorpay order ID in your database
            }
            else
            {
                TempData["Error"] = "The invoice amount must be at least ₹1."; // Show error message if the invoice amount is less than ₹1
            }

            // Return the view with the invoice model
            return View(invoice);
        }


        [HttpGet]
        public IActionResult ThankYou(string paymentId)
        {
            try
            {
                var razorpayClient = new RazorpayClient(_razorpayConfig.Key, _razorpayConfig.Secret);
                var paymentDetails = razorpayClient.Payment.Fetch(paymentId);

                string orderId = paymentDetails["order_id"].ToString();  // Razorpay order ID

                // Mark the invoice as paid using the Razorpay Order ID
                billingRepository.MarkInvoiceAsPaid(orderId);

                // Retrieve invoice by Razorpay order ID to display details
                var invoice = billingRepository.GetInvoiceByRazorpayOrderId(orderId);

                ViewBag.PaymentId = paymentId;
                ViewBag.Invoice = invoice;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not fetch payment details.";
                return RedirectToAction("DisplayBill", new { invoiceId = 0 }); // fallback
            }
        }



        [HttpGet]
        public IActionResult DownloadInvoice(int invoiceId)
        {
            var invoice = billingRepository.GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(PageSize.A4, 40f, 40f, 60f, 40f);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add hospital logo (optional)
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "hospital-logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    var logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleToFit(100f, 100f);
                    logo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    document.Add(logo);
                }

                // Add header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

                document.Add(new Paragraph("INVOICE", headerFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph($"Date: {DateTime.Now:dd-MM-yyyy HH:mm}", normalFont));
                document.Add(new Paragraph("\n"));

                // Add invoice details in table format
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;

                table.AddCell(new PdfPCell(new Phrase("Invoice ID", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase(invoice.InvoiceId.ToString(), normalFont)) { Border = 0 });

                table.AddCell(new PdfPCell(new Phrase("Patient ID", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase(invoice.PatientId.ToString(), normalFont)) { Border = 0 });

                table.AddCell(new PdfPCell(new Phrase("Description", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase(invoice.Description, normalFont)) { Border = 0 });

                table.AddCell(new PdfPCell(new Phrase("Amount", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase($"₹{invoice.Amount:F2}", normalFont)) { Border = 0 });

                table.AddCell(new PdfPCell(new Phrase("Discount", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase($"₹{invoice.Discount:F2}", normalFont)) { Border = 0 });

                table.AddCell(new PdfPCell(new Phrase("Tax", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase($"₹{invoice.Tax:F2}", normalFont)) { Border = 0 });

                table.AddCell(new PdfPCell(new Phrase("Total Amount", boldFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase($"₹{invoice.TotalAmount:F2}", normalFont)) { Border = 0 });

                document.Add(table);

                document.Add(new Paragraph("\nThank you for your payment!", normalFont));

                document.Close();
                writer.Close();

                return File(memoryStream.ToArray(), "application/pdf", $"Invoice_{invoice.InvoiceId}.pdf");
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
