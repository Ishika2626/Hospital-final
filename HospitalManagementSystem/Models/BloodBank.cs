 using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace hospitalManagementSystem.Models
    {
        [Table("donors", Schema = "blood_bank")]
        public class Donor
        {
            [Key]
            public int Id { get; set; }

            [Display(Name = "Campaign")]
            public int? CampaignId { get; set; }

            [Required, Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required, Display(Name = "Date of Birth"), DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required, Display(Name = "Blood Type")]
            public string BloodType { get; set; }

            [Required, Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public string Address { get; set; }

            [Display(Name = "Last Donation Date"), DataType(DataType.Date)]
            public DateTime? LastDonationDate { get; set; }

            [Display(Name = "Eligibility")]
            public string EligibilityStatus { get; set; }
        }
    }


