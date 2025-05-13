using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HospitalManagementSystem.Models
{
    [Table("Roles", Schema = "EmployeeManagement")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("role_name")]
        public string RoleName { get; set; }

        [Column("role_description", TypeName = "text")]
        public string RoleDescription { get; set; }
    }

   

    [Table("Employees", Schema = "EmployeeManagement")]
    public class Employee
    {
        [Key]
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(50)]
        [Column("gender")]
        public string Gender { get; set; }

        [StringLength(15)]
        [Phone]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(255)]
        [Column("address")]
        public string Address { get; set; }

        [Required]
        [Column("hire_date")]
        public DateTime HireDate { get; set; }


        [Column("role_id")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [Required]
        [StringLength(50)]
        [Column("employee_status")]
        public string EmployeeStatus { get; set; }


        // Add LoginId and Password fields
        [Required]
        [StringLength(50)]
        [Column("login_id")]
        public string LoginId { get; set; }

        [Required]
        [StringLength(255)] // Ensure a strong password policy, or you can encrypt the password
        [Column("password")]
        public string Password { get; set; }

        public string RoleName { get; set; }
    }

    [Table("Scheduling", Schema = "EmployeeManagement")]
    public class SchedulingModel
    {
        public int ScheduleId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }
        public string ShiftType { get; set; }

        // For display
        public string EmployeeName { get; set; }
    }
    [Table("Attendance", Schema = "EmployeeManagement")]
    public class AttendanceModel
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; } // yyyy-MM-dd
        public string AttendanceStatus { get; set; }

        public TimeSpan PunchInTime { get; set; } // Make PunchInTime nullable
        public TimeSpan? PunchOutTime { get; set; } // PunchOutTime is already nullable

        public string EmployeeName { get; set; }
    }

    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string EmployeeName { get; set; }
    }

    public class PerformanceReview
    {
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } // For display
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; } // For display
        public DateTime ReviewDate { get; set; }
        public string PerformanceRating { get; set; }
        public string Feedback { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        // Computed property for FullName
        public string FullName => $"{first_name} {last_name}";
    }

    public class EmployeeTraining
    {
        public int TrainingId { get; set; }
        public int EmployeeId { get; set; }
        public int TrainingTblId { get; set; }
        public DateTime TrainingStartDate { get; set; }
        public DateTime TrainingEndDate { get; set; }
        public string TrainingStatus { get; set; }
        public string TrainingProvider { get; set; }

        // Optional: for display
        public string EmployeeName { get; set; }
        public string TrainingName { get; set; }
    }


    public class Training
    {
        public int TrainingId { get; set; } // PK
        public string TrainingName { get; set; }
        public string TrainingDescription { get; set; }
        public string TrainingCategory { get; set; }

        public ICollection<EmployeeTraining> EmployeeTrainings { get; set; }
    }

    public class Payroll
    {
        public int EmployeeId { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonuses { get; set; }
        public decimal Overtime { get; set; }
        public decimal Deductions { get; set; }
        public decimal TotalSalary { get; set; }
        public DateTime PayDate { get; set; }
        public string PaymentMethod { get; set; }
    }


}
