using System;
using System.Collections.Generic;

namespace HospitalManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int DoctorsAvailable { get; set; }
        public int OccupiedBeds { get; set; }
        public int AppointmentsToday { get; set; }

        public List<AdmissionInfo> RecentAdmissions { get; set; }
        public List<AppointmentInfo> UpcomingAppointments { get; set; }
        public List<Appointment> AllAppointments { get; set; }
        public List<BedInfo> BedOccupancy { get; set; }
        public List<NewsItem> NewsItems { get; set; }
        public List<BillingInfo> BillingSummaries { get; set; }
         public List<PrescriptionViewModel> Prescriptions { get; set; }
    }

    // Represents recent patient admissions.
    public class AdmissionInfo
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public DateTime AdmissionDate { get; set; }
    }

    // Represents details of a patient's appointment.
    public class AppointmentInfo
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; } // Corrected from DoctorID to DoctorId
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // Added status field
    }

    // Represents information related to bed occupancy.
    public class BedInfo
    {
        public string Room { get; set; }
        public int AvailableBeds { get; set; }
        public int OccupiedBeds { get; set; }
    }

    // Represents a news item displayed on the dashboard.
    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    // Represents billing information for patients.
    public class BillingInfo
    {
        public string BillId { get; set; }
        public string PatientName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // Bill status (e.g., Paid, Unpaid)
    }
    public class PrescriptionViewModel
    {
        public int PrescriptionId { get; set; }
        public string Date { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }

        public decimal Amount { get; set; }


    }


}
