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


}
