namespace HospitalManagementSystem.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Notes { get; set; }
        public int PatientId { get; set; }
    }
}
