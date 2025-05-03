namespace HospitalManagementSystem.Models
{
    public class Doctor_Schedules
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public string Shift { get; set; } // Morning, Afternoon, Evening
        public string Status { get; set; } // A // Available, Absent, Busy
    }
}
