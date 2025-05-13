using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HospitalManagementSystem.Repositories
{
    public class LaboratoryController : Controller
    {
        private readonly ILabTestRepository _labTestRepository;
        private readonly IPatientRepository _petientRepository;
        private readonly IDoctorRepository doctorRepository;

        // Constructor to inject the repository
        public LaboratoryController(ILabTestRepository labTestRepository, IPatientRepository petientRepository, IDoctorRepository doctorRepository)
        {
            _labTestRepository = labTestRepository;
            _petientRepository = petientRepository;
            this.doctorRepository = doctorRepository;
        }

        // Action to view all lab tests
        public IActionResult DisplayLabTests()
        {
            List<LabTestViewModel> labTests = _labTestRepository.GetAllLabTests();
            return View(labTests);
        }

        // Action to view the details of a single lab test
        public IActionResult LabTestDetails(int testId)
        {
            LabTestViewModel labTest = _labTestRepository.GetLabTestById(testId);
            if (labTest == null)
            {
                return NotFound();
            }
            return View(labTest);
        }

        public IActionResult GetAllBookings()
        {
            var bookings = _labTestRepository.GetAllBookings();
            ViewBag.LabTests = _labTestRepository.GetAllLabTests();
            ViewBag.Patients = _petientRepository.GetAll();
            ViewBag.Doctors = doctorRepository.GetAllDoctors();
            return View(bookings);
        }



        [HttpPost]
        public IActionResult CreateBooking(TestBooking booking, string source)
        {
            _labTestRepository.AddBooking(booking);

            TempData["BookingMessage"] = "Test booking successfully completed!";

            if (source == "user")
            {
                // Redirect back to Laboratory view with success message
                return RedirectToAction("Laboratory", "UserHome");

            }

            // Admin flow: redirect to the list page
            return RedirectToAction("GetAllBookings");
        }

        [HttpPost]
        public IActionResult CollectSample(int bookingId)
        {
            _labTestRepository.CollectSample(bookingId);
            return RedirectToAction("GetAllBookings");
        }

        [HttpPost]
        public IActionResult ConfirmTest(int bookingId)
        {
            _labTestRepository.ConfirmTest(bookingId);
            return RedirectToAction("GetAllBookings");
        }
        // GET: PatientTests
        public IActionResult PatientTestList()
        {
            var patientTests = _labTestRepository.GetAllPatientTests();
            return View(patientTests);
        }

        public IActionResult Details(int id)
        {
            var patientTest = _labTestRepository.GetPatientTestById(id);

            if (patientTest == null)
            {
                return NotFound();
            }

            // Log or debug to verify if all properties are being set correctly
            Console.WriteLine(patientTest.TestCategory); // Example to check if TestCategory is properly fetched

            return PartialView("_PatientTestDetails", patientTest);
        }

        public IActionResult ViewReport(int id)
        {
            var report = _labTestRepository.GetReportByReportId(id); // Fetch the report by ID
            if (report == null)
                return NotFound();

            // Return a view with the report data (LabReportViewModel)
            var reportViewModel = new LabReportViewModel
            {
                ReportId = report.ReportId,
                TestName = report.TestName,
                ReportStatus = report.ReportStatus,
                ReportDate = report.ReportDate,
                TestResult = report.TestResult,
                Findings = report.Findings,
                DoctorNotes = report.DoctorNotes,
                ReportFilePath = report.ReportFilePath,
                PatientName = report.PatientName,
                Gender = report.Gender,
                DOB = report.DOB
            };

            return View(reportViewModel);
        }

        //// POST: /LabReport/UploadPdf
        //[HttpPost]
        //public async Task<IActionResult> UploadPdf(int reportId, IFormFile reportPdf)
        //{
        //    if (reportPdf != null && reportPdf.Length > 0)
        //    {
        //        // Generate path: wwwroot/UploadedReports/filename.pdf
        //        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedReports");
        //        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

        //        string uniqueFileName = Path.GetFileName(reportPdf.FileName);
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        // Save the file to disk
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await reportPdf.CopyToAsync(stream);
        //        }

        //        // Read file bytes
        //        byte[] pdfBytes;
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await reportPdf.CopyToAsync(memoryStream);
        //            pdfBytes = memoryStream.ToArray();
        //        }

        //        // Save to DB
        //        string dbPath = $"/UploadedReports/{uniqueFileName}"; // For URL usage
        //        _labTestRepository.UpdateLabReportPdfAndPath(reportId, pdfBytes, dbPath);

        //        TempData["Success"] = "PDF uploaded successfully!";
        //    }
        //    else
        //    {
        //        TempData["Error"] = "Please select a valid PDF file.";
        //    }

        //    return RedirectToAction("ViewReport", new { id = reportId });
        //}
        [HttpPost]
        public IActionResult SaveReportAsPdf(int reportId)
        {
            var report = _labTestRepository.GetReportByReportId(reportId);
            if (report == null)
                return NotFound();

            byte[] pdfBytes;

            // Generate PDF using iTextSharp
            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40f, 40f, 40f, 40f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);

                doc.Add(new Paragraph("Lab Report", titleFont));
                doc.Add(new Paragraph($"Report ID: {report.ReportId}", bodyFont));
                doc.Add(new Paragraph($"Patient Name: {report.PatientName}", bodyFont));
                doc.Add(new Paragraph($"Gender: {report.Gender}", bodyFont));
                doc.Add(new Paragraph($"Date of Birth: {report.DOB:yyyy-MM-dd}", bodyFont));
                doc.Add(new Paragraph($"Report Date: {report.ReportDate:yyyy-MM-dd}", bodyFont));
                doc.Add(new Paragraph($"Test Name: {report.TestName}", bodyFont));
                doc.Add(new Paragraph($"Test Result: {report.TestResult}", bodyFont));
                doc.Add(new Paragraph($"Findings: {report.Findings}", bodyFont));
                doc.Add(new Paragraph($"Doctor Notes: {report.DoctorNotes}", bodyFont));
                doc.Add(new Paragraph($"Status: {report.ReportStatus}", bodyFont));

                doc.Close();
                pdfBytes = ms.ToArray();
            }

            // Save PDF to disk
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedReports");
            Directory.CreateDirectory(folderPath);

            string fileName = $"Report_{reportId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(filePath, pdfBytes);

            // Save to DB
            string dbPath = $"/UploadedReports/{fileName}";
            _labTestRepository.UpdateLabReportPdfAndPath(reportId, pdfBytes, dbPath);

            TempData["Success"] = "Report successfully generated and saved as PDF.";
            return RedirectToAction("ViewReport", new { id = reportId });
        }

        // Action to display lab reports
        public IActionResult DisplayAllReports()
        {
            IEnumerable<LabReportViewModel> labReports = _labTestRepository.GetLabReports();
            return View(labReports);
        }
        public IActionResult GetBookingById(int id)
        {
            var booking = _labTestRepository.GetBookingById(id);
            return View(booking);
        }
    }
}
