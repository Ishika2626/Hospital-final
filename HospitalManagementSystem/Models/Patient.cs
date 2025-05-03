using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    [Table("patient_registration", Schema = "patient")]
    public class PatientRegistration
    {
        [Key]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(10)]
        [Column("gender")]
        public string Gender { get; set; }

        [Required]
        [Column("dob")]
        public DateTime Dob { get; set; }


        [Column("address")]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        [Phone]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        [Column("emergency_contact_name")]
        public string EmergencyContactName { get; set; }

        [Required]
        [StringLength(15)]
        [Phone]
        [Column("emergency_contact_phone")]
        public string EmergencyContactPhone { get; set; }

        [Required]
        [StringLength(100)]
        [Column("marital_status")]
        public string MaritalStatus { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nationality")]
        public string Nationality { get; set; }

        [StringLength(5)]
        [Column("blood_group")]
        public string BloodGroup { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(6)]
        [Column("password")]
        public string Password { get; set; }

        public string patient_img { get; set; }


    }


    [Table("patient_admission", Schema = "patient")]
    public class patient_admission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int admission_id { get; set; }

        [Required]
        public int patient_id { get; set; }

        [Required]
        public DateTime admission_date { get; set; }

        public DateTime? discharge_date { get; set; }

        public int? room_id { get; set; }

        public int? bed_id { get; set; }

        public string? discharge_reason { get; set; }

        [Required]
        [MaxLength(100)]
        public string status { get; set; }

        // Foreign key for doctor
        public int? DoctorId { get; set; }  // Linking to Doctor table

        // Navigation properties
        public virtual PatientRegistration Patient { get; set; }
        public virtual Rooms Room { get; set; }
        public virtual Beds Bed { get; set; }

        // Navigation property for Doctor (if you want to fetch doctor details)
        public virtual Doctor Doctor { get; set; }  // Assuming you have a Doctor class
    }

    [Table("patient_records", Schema = "patient")]
    public class patient_records
    {
        [Key]
        [Column("record_id")]
        public int RecordId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [Required]
        [Column("diagnosis")]
        public string Diagnosis { get; set; }

        [Required]
        [Column("treatment")]
        public string Treatment { get; set; }

        [Column("test_results")]
        public string TestResults { get; set; }

        [Column("medications_prescribed")]
        public string MedicationsPrescribed { get; set; }

        [Column("visit_date")]
        public DateTime VisitDate { get; set; } = DateTime.Now;
    }

    [Table("patient_visits", Schema = "patient")]
    public class patient_visits
    {
        [Key]
        [Column("visit_id")]
        public int VisitId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [Column("visit_date")]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        [Column("department")]
        public string Department { get; set; }

        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Required]
        [Column("visit_reason")]
        public string VisitReason { get; set; }
    }

    [Table("patient_insurance", Schema = "patient")]
    public class patient_insurance
    {
        [Key]
        [Column("insurance_id")]
        public int InsuranceId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [Required]
        [StringLength(255)]
        [Column("insurance_provider")]
        public string InsuranceProvider { get; set; }

        [Required]
        [StringLength(100)]
        [Column("policy_number")]
        public string PolicyNumber { get; set; }

        [Required]
        [StringLength(50)]
        [Column("coverage_type")]
        public string CoverageType { get; set; }

        [Column("coverage_amount", TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal CoverageAmount { get; set; }

        [Required]
        [Column("valid_from", TypeName = "date")]
        public DateTime ValidFrom { get; set; }

        [Required]
        [Column("valid_until", TypeName = "date")]
        public DateTime ValidUntil { get; set; }
    }

    [Table("patient_documents", Schema = "patient")]
    public class patient_documents
    {
        [Key]
        [Column("document_id")]
        public int DocumentId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [Required]
        [StringLength(100)]
        [Column("document_type")]
        public string DocumentType { get; set; }

        [Required]
        [StringLength(255)]
        [Column("document_name")]
        public string DocumentName { get; set; }

        [Required]
        [StringLength(255)]
        [Column("document_number")]
        public string DocumentNumber { get; set; }

        [Required]
        [StringLength(255)]
        [Column("document_path")]
        public string DocumentPath { get; set; }

        [Required]
        [Column("uploaded_by")]
        public string UploadedBy { get; set; }


        [Column("upload_date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Column("remarks")]
        public string Remarks { get; set; }
    }

    public class PatientLogin
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }


    [Table("patient_vitals", Schema = "patient")]
    public class PatientVitals
    {
        [Key]
        [Column("vitals_id")]
        public int VitalsId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [StringLength(20)]
        [Column("blood_pressure")]
        public string BloodPressure { get; set; }

        [Column("heart_rate")]
        public int? HeartRate { get; set; }

        [Column("temperature", TypeName = "decimal(4,1)")]
        public decimal? Temperature { get; set; }

        [Column("recorded_at")]
        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }

    [Table("patient_status", Schema = "patient")]
    public class PatientStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [StringLength(100)]
        [Column("condition")]
        public string Condition { get; set; }

        [StringLength(50)]
        [Column("status")]
        public string Status { get; set; }

        [Column("checkup_done")]
        public bool CheckupDone { get; set; }
    }

    public class PatientStatusReport
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string BloodGroup { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public bool CheckupDone { get; set; }
    }
    public class PatientVitalsReport
    {
        public string PatientName { get; set; }
        public decimal Temperature { get; set; }
        public string BloodPressure { get; set; }
        public int HeartRate { get; set; }
        public DateTime RecordedAt { get; set; }
    }
    // Models/DischargedPatientViewModel.cs
    public class DischargedPatientViewModel
    {

        public int AdmissionId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Diagnosis { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DischargeReason { get; set; }
        public int PatientId { get; set; }
    }
    public class DischargeViewModel
    {
        public int AdmissionId { get; set; }
        public int PatientId { get; set; }
        public DateTime DischargeDate { get; set; }
        public string DischargeReason { get; set; }
        public int DoctorId { get; set; }
    }



}
