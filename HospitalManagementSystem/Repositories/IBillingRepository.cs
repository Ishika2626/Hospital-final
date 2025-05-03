using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HospitalManagementSystem.Repositories
{
    public interface IBillingRepository
    {
        // Invoice
        IEnumerable<Invoice> GetAllInvoices();
        Invoice GetInvoiceById(int invoiceId);
        void AddInvoice(Invoice invoice);
        void UpdateInvoice(Invoice invoice);
        void DeleteInvoice(int invoiceId);

        // Insurance Integration
        IEnumerable<InsuranceIntegration> GetAllInsuranceClaims();
        InsuranceIntegration GetInsuranceClaimById(int claimId);
        void AddInsuranceClaim(InsuranceIntegration claim);
        void UpdateInsuranceClaim(InsuranceIntegration claim);
        void DeleteInsuranceClaim(int claimId);

        // Payment Tracking
        IEnumerable<PaymentTracking> GetAllPayments();
        PaymentTracking GetPaymentById(int paymentId);
        void AddPayment(PaymentTracking payment);
        void UpdatePayment(PaymentTracking payment);
        void DeletePayment(int paymentId);

        // Helper Methods for Dropdowns
        List<SelectListItem> GetPatientList();
        List<SelectListItem> GetInvoiceList();
        List<SelectListItem> GetPaymentMethods();
        List<SelectListItem> GetPaymentStatusList();
        List<SelectListItem> GetInsuranceProviders();
        List<SelectListItem> GetClaimStatusList();
    }
}
