namespace HospitalManagementSystem.Models
{
    public class LabTestViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string TestDescription { get; set; }
        public string TestCategory { get; set; }
        public string TestType { get; set; }
        public int TestDuration { get; set; } // Duration in minutes
        public decimal TestCost { get; set; }
        public string TestMethod { get; set; }
    }

   

    public class TestBooking
    {
        public int BookingId { get; set; }
        public int PatientId { get; set; }
        public int TestId { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime? SampleCollectedDate { get; set; }
        public string? AdditionalNotes { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public int DoctorId { get; set; }

        // Related info
        public string TestType { get; set; } = string.Empty;
        public virtual PatientRegistration Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
        public string? ResultStatus { get; set; }  // Add ResultStatus here
    }

    public class PatientTest
    {
        public int PatientTestId { get; set; }
        public int TestOrderId { get; set; }
        public bool ResultReceived { get; set; }
        public string ResultStatus { get; set; }
    }
    public class PatientTestViewModel
    {
        // Patient Test details
        public DateTime TestDate { get; set; }
        public DateTime? SampleCollectedDate { get; set; }
        public string TestName { get; set; }
        public string TestType { get; set; }
        public string TestCategory { get; set; }
        public int TestDuration { get; set; }
        public decimal TestCost { get; set; }

        // Additional Booking Details (if needed)
        public string BookingStatus { get; set; }
        public string AdditionalNotes { get; set; }

        // To Display Information of a Patient (if needed)
        public int PatientTestId { get; set; }
        public int TestOrderId { get; set; }
        public string ResultStatus { get; set; }
        public bool ResultReceived { get; set; }
        public int? ReportId { get; set; } // Inside PatientTestViewModel
    }
    public class LabReportViewModel
    {
        public int ReportId { get; set; }
        public string TestName { get; set; }
        public string ReportStatus { get; set; }
        public DateTime ReportDate { get; set; }
        public string TestResult { get; set; } // Stored as JSON or formatted string
        public string Findings { get; set; }
        public string DoctorNotes { get; set; }
        public string ReportFilePath { get; set; }
        public byte[] ReportPdf { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public int PatientTestId { get; set; }
    }

    public class LabReport
    {
        public int ReportId { get; set; }
        public int PatientTestId { get; set; }
        public DateTime ReportDate { get; set; }
        public string TestResult { get; set; }
        public string Findings { get; set; }
        public string ReportStatus { get; set; }
        public string DoctorNotes { get; set; }
        public int LabTechnicianId { get; set; }
        public string ReportType { get; set; }
        public string ReportFilePath { get; set; }
        public byte[] ReportPdf { get; set; }
    }
    public class LabReportViewModel2
    {
        public int ReportId { get; set; }
        public string ReportPdf { get; set; }
        public int TestOrderId { get; set; }
        public int PatientId { get; set; }
        public string TestName { get; set; }
        public string DoctorName { get; set; }
    }

}
