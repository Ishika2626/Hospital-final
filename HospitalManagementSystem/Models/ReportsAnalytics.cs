using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    [Table("Clinical_Reports", Schema = "ReportsAnalytics")]
    public class ClinicalReport
    {
        [Key]
        [Column("report_id")]
        public int ReportID { get; set; }

        [Required]
        [ForeignKey("patient_id")]
        [Column("patient_id")]
        public int PatientID { get; set; }

        [Required]
        [ForeignKey("doctor_id")]
        [Column("doctor_id")]
        public int DoctorID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("diagnosis")]
        public string Diagnosis { get; set; }

        [Column("treatmentplan")]
        public string TreatmentPlan { get; set; }

        [Required]
        [Column("TreatmentStartDate")]
        public DateTime TreatmentStartDate { get; set; }

        [Column("TreatmentEndDate")]
        public DateTime? TreatmentEndDate { get; set; }

        [Required]
        [Column("Outcome")]
        public string Outcome { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("ImagePath")]
        public string ImagePath { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual PatientRegistration Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }

    [Table("Financial_Reports", Schema = "ReportsAnalytics")]
    public class FinancialReport
    {
        [Key]
        [Column("financialreport_id")]
        public int ReportID { get; set; }

        [Required]
        [Column("reportDate")]
        public DateTime ReportDate { get; set; }

        [Required]
        [Column("TotalRevenue")]
        public decimal TotalRevenue { get; set; }

        [Required]
        [Column("TotalExpenses")]
        public decimal TotalExpenses { get; set; }

        [NotMapped]
        public decimal NetProfit => TotalRevenue - TotalExpenses;

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    [Table("Performance_Reports", Schema = "ReportsAnalytics")]
    public class PerformanceMonitoring
    {
        [Key]
        [Column("PerformanceID")]
        public int PerformanceID { get; set; }

        [Required]
        [ForeignKey("stff_id")]
        [Column("stff_id")]
        public int StaffID { get; set; }

        [Required]
        [ForeignKey("department_id")]
        [Column("department_id")]
        public int DepartmentID { get; set; }

        [Range(1, 5)]
        [Column("Rating")]
        public int Rating { get; set; }

        [Column("PatientFeedback")]
        public string PatientFeedback { get; set; }

        [Required]
        [Column("CasesHandled")]
        public int CasesHandled { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Employee Staff { get; set; }
        public virtual Department Department { get; set; }
    }
}
