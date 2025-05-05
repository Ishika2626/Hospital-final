using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace HospitalManagementSystem.Models
{
    [Table("doctors", Schema = "Doctors")]
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorId { get; set; }  // Primary Key, Doctor ID

        [Required]
        [StringLength(255)]
        public string FullName { get; set; }  // Full name of the doctor

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }  // Unique email address

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }  // Doctor's phone number

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }  // Gender of the doctor (Male, Female, Other)

        [Required]
        public DateTime DateOfBirth { get; set; }  // Date of birth

        [Required]
        [StringLength(255)]
        public string Qualification { get; set; }  // Doctor's qualification

        [Required]
        public int Experience { get; set; }  // Years of experience

        [Required]
        [StringLength(20)]
        public string Status { get; set; }  // Active, Inactive, or On Leave

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string Password { get; set; }  // Doctor's password (for authentication)

        [Required]
        public string Img { get; set; }  // Doctor's profile image (could be a file path or URL)

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }


    }

    [Table("Departments", Schema = "EmployeeManagement")]
    public class Department
    {
        [Key]
        [Column("department_id")]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("department_name")]
        public string DepartmentName { get; set; }
    }

  

    [Table("doctor_schedule", Schema = "Doctors")]
    public class DoctorSchedule
    {
        [Key]
        public int schedule_id { get; set; }

        [Required]
        public int doctor_id { get; set; }

        [Required]
        [MaxLength(20)]
        public string day_of_week { get; set; }

        [Required]
        public TimeSpan start_time { get; set; }

        [Required]
        public TimeSpan end_time { get; set; }
    }

    [Table("doctor_appointments", Schema = "Doctors")]
    public class DoctorAppointment
    {
        [Key]
        public int appointment_id { get; set; }

        [Required]
        public int doctor_id { get; set; }

        [Required]
        public int patient_id { get; set; }

        [Required]
        public DateTime appointment_date { get; set; }

        [Required]
        [MaxLength(20)]
        public string status { get; set; }
    }

    [Table("doctor_prescriptions", Schema = "Doctors")]
    public class DoctorPrescription
    {
        [Key]
        public int prescription_id { get; set; }

        [Required]
        public int appointment_id { get; set; }

        [Required]
        public string medication { get; set; }

        [Required]
        public string dosage { get; set; }

        [Required]
        public string instructions { get; set; }
    }

    [Table("doctor_notes", Schema = "Doctors")]
    public class DoctorNote
    {
        [Key]
        public int note_id { get; set; }

        [Required]
        public int appointment_id { get; set; }

        [Required]
        public string note { get; set; }
    }

    [Table("doctor_reviews", Schema = "Doctors")]
    public class DoctorReview
    {
        [Key]
        public int review_id { get; set; }

        [Required]
        public int doctor_id { get; set; }

        [Required]
        public int patient_id { get; set; }

        [Required]
        [Range(1, 5)]
        public int rating { get; set; }

        public string? comment { get; set; }

        [Required]
        public DateTime review_date { get; set; }
    }

    [Table("doctor_payments", Schema = "Doctors")]
    public class DoctorPayment
    {
        [Key]
        public int payment_id { get; set; }

        [Required]
        public int doctor_id { get; set; }

        [Required]
        public decimal consultation_fee { get; set; }

        [Required]
        public decimal procedure_fee { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal total_amount { get; set; }

        [Required]
        public DateTime payment_date { get; set; }
    }
}
