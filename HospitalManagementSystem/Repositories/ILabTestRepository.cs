using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Repositories
{
    public interface ILabTestRepository
    {
        List<LabTestViewModel> GetAllLabTests();
        LabTestViewModel GetLabTestById(int testId);
        IEnumerable<TestBooking> GetAllBookings();
        TestBooking GetBookingById(int id);
        void AddBooking(TestBooking booking);
        void CollectSample(int bookingId);
        void ConfirmTest(int bookingId);
        IEnumerable<PatientTestViewModel> GetAllPatientTests();
        PatientTestViewModel GetPatientTestById(int id);
        LabReportViewModel GetReportById(int reportId);
        IEnumerable<TestBooking> GetConfirmBookings();
        IEnumerable<TestBooking> GetConfirmAndCollectedBookings();
        void AddLabReport(LabReport report);
        void UpdateLabReportPdfAndPath(int reportId, byte[] pdfData, string filePath);
        LabReportViewModel GetReportByReportId(int reportId);
        IEnumerable<LabReportViewModel> GetLabReports();
        List<LabReportViewModel2> GetReportsByPatientId(int patientId);
    }
}
