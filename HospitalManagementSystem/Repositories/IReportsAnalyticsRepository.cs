using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace HospitalManagementSystem.Repositories
{
    public interface IReportsAnalyticsRepository
    {
        // Clinical Reports
        IEnumerable<ClinicalReport> GetAllClinicalReports();
        ClinicalReport GetClinicalReportById(int reportId);
        void AddClinicalReport(ClinicalReport report, IFormFile imageFile);
        void UpdateClinicalReport(ClinicalReport report, IFormFile imageFile);
        void DeleteClinicalReport(int reportId);

        // Financial Reports
        IEnumerable<FinancialReport> GetAllFinancialReports();
        FinancialReport GetFinancialReportById(int reportId);
        void AddFinancialReport(FinancialReport report);
        void UpdateFinancialReport(FinancialReport report);
        void DeleteFinancialReport(int reportId);

        // Performance Monitoring
        IEnumerable<PerformanceMonitoring> GetAllPerformanceReports();
        PerformanceMonitoring GetPerformanceReportById(int performanceId);
        void AddPerformanceReport(PerformanceMonitoring report);
        void UpdatePerformanceReport(PerformanceMonitoring report);
        void DeletePerformanceReport(int performanceId);
    }
}
