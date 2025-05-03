using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    [Table("appointment_scheduling", Schema = "AppointmentScheduling")]
    public class Appointment
    {
        [Key]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [ForeignKey("PatientRegistration")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; } = new PatientRegistration();

        [ForeignKey("Doctor")]
        [Column("doctor_id")]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } 

        [Required]
        [Column("appointment_date")]
        public DateTime AppointmentDate { get; set; }

        [StringLength(100)]
        [Column("appointment_status")]
        public string AppointmentStatus { get; set; } = "Scheduled";

        [Required]
        [Column("reason")]
        public string Reason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
    }

    [Table("doctor_availability", Schema = "AppointmentScheduling")]
    public class DoctorAvailability
    {
        [Key]
        [Column("availability_id")]
        public int AvailabilityId { get; set; }

        [ForeignKey("Doctor")]
        [Column("doctor_id")]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        [Column("available_date", TypeName = "date")]
        public DateTime AvailableDate { get; set; }

        [Required]
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        [StringLength(100)]
        [Column("status")]
        public string Status { get; set; } = "Available";
    }

    [Table("appointment_alerts", Schema = "AppointmentScheduling")]
    public class AppointmentAlert
    {
        [Key]
        [Column("alert_id")]
        public int AlertId { get; set; }

        [ForeignKey("Appointment")]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        [Required]
        [StringLength(100)]
        [Column("recipient_type")]
        public string RecipientType { get; set; }

        [Required]
        [Column("alert_message")]
        public string AlertMessage { get; set; }

        [StringLength(100)]
        [Column("sent_status")]
        public string SentStatus { get; set; } = "Pending";

        [Column("sent_at")]
        public DateTime? SentAt { get; set; }
    }
}
