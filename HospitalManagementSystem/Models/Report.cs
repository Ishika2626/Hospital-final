namespace HospitalManagementSystem.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public string LabResults { get; set; }
        public string LabReportFile { get; set; }
    }
}
