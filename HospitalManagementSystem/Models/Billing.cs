using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class RazorpayConfig
    {
        public string Key { get; set; }
        public string Secret { get; set; }
    }
   



    [Table("invoicing", Schema = "BillingPayment")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [ForeignKey("Patient")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [Column("description")]
        [Required]
        public string Description { get; set; }

        [Column("amount")]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Column("discount")]
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;

        [Column("tax")]
        [Range(0, double.MaxValue)]
        public decimal Tax { get; set; } = 0;

        [Column("total_amount")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount => Amount - Discount + Tax;

        [Column("invoice_date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("status")]
        [StringLength(100)]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Add a new field for Razorpay Order ID
        [Column("razorpay_order_id")]
        [StringLength(255)] // String length based on Razorpay order ID length
        public string RazorpayOrderId { get; set; }
      

    }

    [Table("insurance_integration", Schema = "BillingPayment")]
    public class InsuranceIntegration
    {
        [Key]
        [Column("insurance_claim_id")]
        public int InsuranceClaimId { get; set; }

        [ForeignKey("Patient")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [Column("insurance_provider")]
        [Required]
        [StringLength(255)]
        public string InsuranceProvider { get; set; }

        [Column("claim_number")]
        [Required]
        [StringLength(100)]
        public string ClaimNumber { get; set; }

        [Column("claim_amount")]
        [Range(0, double.MaxValue)]
        public decimal ClaimAmount { get; set; }

        [Column("claim_status")]
        [StringLength(100)]
        public string ClaimStatus { get; set; }

        [Column("settlement_amount")]
        [Range(0, double.MaxValue)]
        public decimal SettlementAmount { get; set; } = 0;

        [Column("settlement_date")]
        public DateTime? SettlementDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    [Table("payment_tracking", Schema = "BillingPayment")]
    public class PaymentTracking
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [ForeignKey("Patient")]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public PatientRegistration Patient { get; set; }

        [ForeignKey("Invoice")]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Column("payment_amount")]
        [Range(0, double.MaxValue)]
        public decimal PaymentAmount { get; set; }

        [Column("payment_method")]
        [Required]
        [StringLength(100)]
        public string PaymentMethod { get; set; }

        [Column("payment_status")]
        [StringLength(100)]
        public string PaymentStatus { get; set; }

        [Column("transaction_id")]
        [StringLength(100)]
        public string TransactionId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
