using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{


    [Table("HiringManagement", Schema = "RecruitmentManagement")]
    public class HiringManagement
    {
        [Key]
        [Column("job_id")]
        public int JobId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("position")]
        public string Position { get; set; }

        [Required]
        [StringLength(255)]
        [Column("department")]
        public string Department { get; set; }

        [Required]
        [Column("vacancies")]
        public int Vacancies { get; set; }

        [Required]
        [Column("posted_date")]
        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        [Column("status")]
        public string Status { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
