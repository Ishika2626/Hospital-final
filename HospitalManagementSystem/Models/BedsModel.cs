using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
   
        // Beds Table
        [Table("Beds", Schema = "bed_managment")]
        public class Beds
        {
            [Key]
            public int BedID { get; set; }

            [Required]
            public int RoomID { get; set; }

            [Required]
            [StringLength(20)]
            public string BedNumber { get; set; }

            [Required]
            [EnumDataType(typeof(BedType))]
            public BedType BedType { get; set; }

            [Required]
            [EnumDataType(typeof(Status))]
            public Status Status { get; set; }

            public DateTime LastUpdated { get; set; } = DateTime.Now;

            [ForeignKey("RoomID")]
            public virtual Rooms Room { get; set; }
        }

        // Rooms Table
        [Table("Rooms", Schema = "bed_managment")]
        public class Rooms
        {
            [Key]
            public int RoomID { get; set; }

            [Required]
            [StringLength(20)]
            public string RoomNumber { get; set; }

            [Required]
            [EnumDataType(typeof(RoomType))]
            public RoomType RoomType { get; set; }

            [Required]
            public int Capacity { get; set; }

            [Required]
            [EnumDataType(typeof(Status))]
            public Status Status { get; set; }

            public DateTime LastUpdated { get; set; } = DateTime.Now;


            public string room_img { get; set; }

        }

        // Patient Bed Allocation Table
        [Table("Patient_Bed_Allocation", Schema = "bed_managment")]
        public class Patient_Bed_Allocation
        {
            [Key]
            public int AllocationID { get; set; }

            [Required]
            public int PatientID { get; set; }

            [Required]
            public int BedID { get; set; }

            public DateTime CheckInDate { get; set; } = DateTime.Now;

            public DateTime? CheckOutDate { get; set; }

            [Required]
            [EnumDataType(typeof(AllocationStatus))]
            public AllocationStatus Status { get; set; }

            [ForeignKey("PatientID")]
            public virtual PatientRegistration Patient { get; set; }

            [ForeignKey("BedID")]
            public virtual Beds Bed { get; set; }
        }

        // Bed Transfer Table
        [Table("Bed_Transfer", Schema = "bed_managment")]
        public class Bed_Transfer
        {
            [Key]
            public int TransferID { get; set; }

            [Required]
            public int PatientID { get; set; }

            [Required]
            public int OldBedID { get; set; }

            [Required]
            public int NewBedID { get; set; }

            public DateTime TransferDate { get; set; } = DateTime.Now;

            public string Reason { get; set; }

            [ForeignKey("PatientID")]
            public virtual PatientRegistration Patient { get; set; }

            [ForeignKey("OldBedID")]
            public virtual Beds OldBed { get; set; }

            [ForeignKey("NewBedID")]
            public virtual Beds NewBed { get; set; }
        }

        // Bed Maintenance Table
        [Table("Bed_Maintenance", Schema = "bed_managment")]
        public class Bed_Maintenance
        {
            [Key]
            public int MaintenanceID { get; set; }

            [Required]
            public int BedID { get; set; }

            public DateTime StartDate { get; set; } = DateTime.Now;

            public DateTime? EndDate { get; set; }

            public string Reason { get; set; }

            [Required]
            [EnumDataType(typeof(MaintenanceStatus))]
            public MaintenanceStatus Status { get; set; }

            [ForeignKey("BedID")]
            public virtual Beds Bed { get; set; }
        }

        // Bed Occupancy History Table
        [Table("Bed_Occupancy_History", Schema = "bed_managment")]
        public class Bed_Occupancy_History
        {
            [Key]
            public int HistoryID { get; set; }

            [Required]
            public int BedID { get; set; }

            public int? PatientID { get; set; }

            public DateTime StartDate { get; set; } = DateTime.Now;

            public DateTime? EndDate { get; set; }

            [Required]
            [EnumDataType(typeof(OccupancyStatus))]
            public OccupancyStatus Status { get; set; }

            [ForeignKey("BedID")]
            public virtual Beds Bed { get; set; }

            [ForeignKey("PatientID")]
            public virtual PatientRegistration Patient { get; set; }
        }

        // Room Occupancy Table
        [Table("Room_Occupancy", Schema = "bed_managment")]
        public class Room_Occupancy
        {
            [Key]
            public int RoomOccupancyID { get; set; }

            [Required]
            public int RoomID { get; set; }

            public int CurrentOccupancy { get; set; }

            public int MaxCapacity { get; set; }

            [Required]
            [EnumDataType(typeof(RoomStatus))]
            public RoomStatus Status { get; set; }

            public DateTime LastUpdated { get; set; } = DateTime.Now;

            [ForeignKey("RoomID")]
            public virtual Rooms Room { get; set; }
        }

        // Bed Reservation Table
        [Table("Bed_Reservation", Schema = "bed_managment")]
        public class Bed_Reservation
        {
            [Key]
            public int ReservationID { get; set; }

            [Required]
            public int PatientID { get; set; }

            [Required]
            public int BedID { get; set; }

            public DateTime ReservedDate { get; set; } = DateTime.Now;

            [Required]
            [EnumDataType(typeof(ReservationStatus))]
            public ReservationStatus Status { get; set; }

            public DateTime LastUpdated { get; set; } = DateTime.Now;

            [ForeignKey("PatientID")]
            public virtual PatientRegistration Patient { get; set; }

            [ForeignKey("BedID")]
            public virtual Beds Bed { get; set; }
        }

        // Enum for Bed Type
        public enum BedType
        {
            General,
            ICU,
            Emergency,
            Pediatric,
            Maternity
    }

    // Enum for Room Type
 

public enum RoomType
    {
        [Display(Name = "General Ward")]
        GeneralWard,

        [Display(Name = "ICU")]
        ICU,

        [Display(Name = "Private")]
        Private,

        [Display(Name = "Semi-Private")]
        SemiPrivate
    }



    // Enum for Status (used in Beds, Rooms, etc.)
    public enum Status
        {
            Available,
            Occupied,
            Reserved,
            Maintenance,
            Full
        }

        // Enum for Allocation Status
        public enum AllocationStatus
        {
            Active,
            Discharged,
            Transferred
        }

        // Enum for Maintenance Status
        public enum MaintenanceStatus
        {
            InProgress,
            Completed,
            Pending,
            Unknown
        }

        // Enum for Occupancy Status
        public enum OccupancyStatus
        {
            Occupied,
            Released
        }

        // Enum for Room Status
        public enum RoomStatus
        {
            Available,
            Full,
            Maintenance
        }

        // Enum for Reservation Status
        public enum ReservationStatus
        {
            Pending,
            Confirmed,
            Cancelled
        }
    
}
