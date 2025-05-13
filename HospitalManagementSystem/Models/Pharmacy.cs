using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    [Table("medicines", Schema = "PharmacyManagement")]
    public class Medicine
    {
        [Key]
        [Column("medicine_id")]
        public int MedicineId { get; set; }

        [Column("medicine_name")]
        [Required]
        [StringLength(255)]
        public string MedicineName { get; set; }

        [Column("generic_name")]
        [Required]
        [StringLength(255)]
        public string GenericName { get; set; }

        [Column("brand_name")]
        [StringLength(255)]
        public string BrandName { get; set; }

        [Column("dosage_form")]
        [Required]
        [StringLength(100)]
        public string DosageForm { get; set; }

        [Column("strength")]
        [Required]
        [StringLength(50)]
        public string Strength { get; set; }

        [Column("manufacturer")]
        [Required]
        [StringLength(255)]
        public string Manufacturer { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(1); // default to 1 year in future


        [Column("unit_price")]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Column("stock_quantity")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    [Table("prescriptions", Schema = "PharmacyManagement")]
    public class PharmacyPrescriptionEntity
    {
        [Key]
        [Column("prescription_id")]
        public int PrescriptionId { get; set; }

        [ForeignKey("Patient")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [ForeignKey("Doctor")]
        [Column("doctor_id")]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Column("prescription_date")]
        public DateTime PrescriptionDate { get; set; } = DateTime.Now;

        [Column("total_amount")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Column("status")]
        [StringLength(100)]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }


    [Table("pharmacy_orders", Schema = "PharmacyManagement")]
    public class PharmacyOrder
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [ForeignKey("Medicine")]
        [Column("medicine_id")]
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }

        [Column("order_quantity")]
        [Range(0, int.MaxValue)]
        public int OrderQuantity { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column("order_status")]
        [StringLength(100)]
        public string OrderStatus { get; set; }

        [Column("supplier_name")]
        [Required]
        [StringLength(255)]
        public string SupplierName { get; set; }

        [Column("total_cost")]
        [Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    [Table("pharmacy_stock", Schema = "PharmacyManagement")]
    public class PharmacyStock
    {
        [Key]
        [Column("stock_id")]
        public int StockId { get; set; }

        [ForeignKey("Medicine")]
        [Column("medicine_id")]
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }

        [Column("stock_quantity")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Column("stock_in_date")]
        public DateTime StockInDate { get; set; } = DateTime.Now;

        [Column("stock_out_date")]
        public DateTime? StockOutDate { get; set; }

        [Column("stock_balance")]
        [Range(0, int.MaxValue)]
        public int StockBalance { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }





    [Table("ambulance_requests", Schema = "EmergencyAmbulanceManagement")]
    public class AmbulanceRequest
    {
        [Key]
        [Column("RequestID")]
        public int RequestID { get; set; }

        [ForeignKey("Patient")]
        [Column("PatientID")]
        public int PatientID { get; set; }
        public PatientRegistration Patient { get; set; }

        [Column("RequestTime")]
        public DateTime RequestTime { get; set; } = DateTime.Now;

        [Column("PickupLocation")]
        [Required]
        [StringLength(255)]
        public string PickupLocation { get; set; }

        [Column("Destination")]
        [Required]
        [StringLength(255)]
        public string Destination { get; set; }


        [ForeignKey("Ambulance")]
        [Column("AmbulanceID")]
        public int? AmbulanceID { get; set; }
        public Ambulance Ambulance { get; set; }

        [Column("Status")]
        public AmbulanceRequestStatus Status { get; set; } = AmbulanceRequestStatus.Pending;

        [ForeignKey("Staff")]
        [Column("AssignedDriver")]
        public int AssignedDriver { get; set; }
        public Employee AssignedStaff { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }
    }

    public enum AmbulanceRequestStatus
    {
        Pending,
        Accepted,
        InTransit,
        Completed,
        Cancelled
    }


    // Emergency Cases Model
    [Table("emergency_cases", Schema = "EmergencyAmbulanceManagement")]
    public class EmergencyCase
    {
        [Key]
        [Column("CaseID")]
        public int CaseID { get; set; }

        [ForeignKey("Patient")]
        [Column("PatientID")]
        public int PatientID { get; set; }
        public PatientRegistration Patient { get; set; }

        [Column("ArrivalTime")]
        public DateTime ArrivalTime { get; set; } = DateTime.Now;

        [Column("SeverityLevel")]
        [Required]
        public SeverityLevel SeverityLevel { get; set; }

        [Column("Diagnosis")]
        [StringLength(255)]
        public string Diagnosis { get; set; }

        [Column("TreatmentGiven")]
        public string TreatmentGiven { get; set; }

        [ForeignKey("Doctor")]
        [Column("DoctorID")]
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        [Column("Status")]
        [Required]
        public EmergencyCaseStatus Status { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }
    }

    public enum SeverityLevel
    {
        Low,
        Moderate,
        Severe,
        Critical
    }

    public enum EmergencyCaseStatus
    {
        Admitted,
        TreatedAndDischarged,
        Transferred,
        Deceased
    }

    // Ambulance Model (just to complete foreign key reference)
    [Table("ambulances", Schema = "EmergencyManagement")]
    public class Ambulance
    {
        [Key]
        [Column("AmbulanceID")]
        public int AmbulanceID { get; set; }

        [Column("AmbulanceNumber")]
        [StringLength(50)]
        public string AmbulanceNumber { get; set; }

        
    }


}
