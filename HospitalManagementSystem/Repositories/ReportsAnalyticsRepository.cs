using HospitalManagementSystem.Models;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Repositories
{
    public class ReportsAnalyticsRepository : IReportsAnalyticsRepository
    {
        private readonly string _imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly string _connectionstring;

        public ReportsAnalyticsRepository(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("DefaultConnection");

        }
          private string SavePhoto(IFormFile patient_img)
        {
            if (patient_img == null || patient_img.Length == 0)
            {
                Console.WriteLine("No file uploaded.");
                return null;
            }

            if (!Directory.Exists(_imageFilePath))
            {
                Directory.CreateDirectory(_imageFilePath);
                Console.WriteLine("Uploads folder created at: " + _imageFilePath);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(patient_img.FileName);
            string filePath = Path.Combine(_imageFilePath, fileName);
            Console.WriteLine("Saving file to: " + filePath);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    patient_img.CopyTo(stream);
                }

                string returnPath = "/uploads/" + fileName;
                Console.WriteLine("File saved successfully at: " + returnPath);
                return returnPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving file: " + ex.Message);
                return null;
            }
        }

        public void AddClinicalReport(ClinicalReport report, IFormFile imageFile)
        {
            var imagePath = imageFile != null ? SavePhoto(imageFile) : null;

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "INSERT INTO ReportsAnalytics.Clinical_Reports (patient_id, doctor_id, Diagnosis, TreatmentPlan, TreatmentStartDate, TreatmentEndDate, Outcome, Notes, ImagePath, CreatedAt) " +
                            "VALUES (@PatientID, @DoctorID, @Diagnosis, @TreatmentPlan, @TreatmentStartDate, @TreatmentEndDate, @Outcome, @Notes, @ImagePath, @CreatedAt)";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PatientID", report.PatientID);
                command.Parameters.AddWithValue("@DoctorID", report.DoctorID);
                command.Parameters.AddWithValue("@Diagnosis", report.Diagnosis);
                command.Parameters.AddWithValue("@TreatmentPlan", report.TreatmentPlan ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TreatmentStartDate", report.TreatmentStartDate);
                command.Parameters.AddWithValue("@TreatmentEndDate", (object)report.TreatmentEndDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Outcome", report.Outcome);
                command.Parameters.AddWithValue("@Notes", report.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ImagePath", imagePath ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the query synchronously
            }
        }

        public void DeleteClinicalReport(int reportId)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "DELETE FROM ReportsAnalytics.Clinical_Reports WHERE report_id = @ReportID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReportID", reportId);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the delete query
            }
        }



        public IEnumerable<ClinicalReport> GetAllClinicalReports()
        {
            var reports = new List<ClinicalReport>();

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "SELECT * FROM ReportsAnalytics.Clinical_Reports";

                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var report = new ClinicalReport
                        {
                            ReportID = (int)reader["report_id"],
                            PatientID = (int)reader["patient_id"],
                            DoctorID = (int)reader["doctor_id"],
                            Diagnosis = reader["diagnosis"].ToString(),
                            TreatmentPlan = reader["treatmentplan"].ToString(),
                            TreatmentStartDate = (DateTime)reader["TreatmentStartDate"],
                            TreatmentEndDate = reader["TreatmentEndDate"] as DateTime?,
                            Outcome = reader["Outcome"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            ImagePath = reader["ImagePath"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        reports.Add(report);
                    }
                }
            }
            return reports;
        }

        public ClinicalReport GetClinicalReportById(int reportId)
        {
            ClinicalReport report = null;

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "SELECT * FROM ReportsAnalytics.Clinical_Reports WHERE report_id = @ReportID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReportID", reportId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        report = new ClinicalReport
                        {
                            ReportID = (int)reader["report_id"],
                            PatientID = (int)reader["patient_id"],
                            DoctorID = (int)reader["doctor_id"],
                            Diagnosis = reader["diagnosis"].ToString(),
                            TreatmentPlan = reader["treatmentplan"].ToString(),
                            TreatmentStartDate = (DateTime)reader["TreatmentStartDate"],
                            TreatmentEndDate = reader["TreatmentEndDate"] as DateTime?,
                            Outcome = reader["Outcome"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            ImagePath = reader["ImagePath"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                    }
                }
            }
            return report;
        }

        public void AddFinancialReport(FinancialReport report)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "INSERT INTO ReportsAnalytics.Financial_Reports (ReportDate, TotalRevenue, TotalExpenses, Notes, CreatedAt) " +
                            "VALUES (@ReportDate, @TotalRevenue, @TotalExpenses, @Notes, @CreatedAt)";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReportDate", report.ReportDate);
                command.Parameters.AddWithValue("@TotalRevenue", report.TotalRevenue);
                command.Parameters.AddWithValue("@TotalExpenses", report.TotalExpenses);
                command.Parameters.AddWithValue("@Notes", report.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the query to insert the record
            }
        }


        public void AddPerformanceReport(PerformanceMonitoring report)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "INSERT INTO ReportsAnalytics.Performance_Reports (StaffID, DepartmentID, Rating, PatientFeedback, CasesHandled, CreatedAt) " +
                            "VALUES (@StaffID, @DepartmentID, @Rating, @PatientFeedback, @CasesHandled, @CreatedAt)";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", report.StaffID);
                command.Parameters.AddWithValue("@DepartmentID", report.DepartmentID);
                command.Parameters.AddWithValue("@Rating", report.Rating);
                command.Parameters.AddWithValue("@PatientFeedback", report.PatientFeedback ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CasesHandled", report.CasesHandled);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the query to insert the record
            }
        }


        public void DeleteFinancialReport(int reportId)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "DELETE FROM ReportsAnalytics.Financial_Reports WHERE financialreport_id = @ReportID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReportID", reportId);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the delete query
            }
        }

        public void DeletePerformanceReport(int performanceId)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "DELETE FROM ReportsAnalytics.Performance_Reports WHERE PerformanceID = @PerformanceID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PerformanceID", performanceId);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the delete query
            }
        }

        public IEnumerable<FinancialReport> GetAllFinancialReports()
        {
            var reports = new List<FinancialReport>();

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "SELECT * FROM ReportsAnalytics.Financial_Reports";

                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var report = new FinancialReport
                        {
                            ReportID = (int)reader["financialreport_id"],
                            ReportDate = (DateTime)reader["reportDate"],
                            TotalRevenue = (decimal)reader["TotalRevenue"],
                            TotalExpenses = (decimal)reader["TotalExpenses"],
                            Notes = reader["Notes"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        reports.Add(report);
                    }
                }
            }
            return reports;
        }

        public IEnumerable<PerformanceMonitoring> GetAllPerformanceReports()
        {
            var reports = new List<PerformanceMonitoring>();

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "SELECT * FROM ReportsAnalytics.Performance_Reports";

                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var report = new PerformanceMonitoring
                        {
                            PerformanceID = (int)reader["PerformanceID"],
                            StaffID = (int)reader["stff_id"],
                            DepartmentID = (int)reader["department_id"],
                            Rating = (int)reader["Rating"],
                            PatientFeedback = reader["PatientFeedback"].ToString(),
                            CasesHandled = (int)reader["CasesHandled"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        reports.Add(report);
                    }
                }
            }
            return reports;
        }

        public FinancialReport GetFinancialReportById(int reportId)
        {
            FinancialReport report = null;

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "SELECT * FROM ReportsAnalytics.Financial_Reports WHERE financialreport_id = @ReportID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReportID", reportId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        report = new FinancialReport
                        {
                            ReportID = (int)reader["financialreport_id"],
                            ReportDate = (DateTime)reader["reportDate"],
                            TotalRevenue = (decimal)reader["TotalRevenue"],
                            TotalExpenses = (decimal)reader["TotalExpenses"],
                            Notes = reader["Notes"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                    }
                }
            }
            return report;
        }

        public PerformanceMonitoring GetPerformanceReportById(int performanceId)
        {
            PerformanceMonitoring report = null;

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "SELECT * FROM ReportsAnalytics.Performance_Reports WHERE PerformanceID = @PerformanceID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PerformanceID", performanceId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        report = new PerformanceMonitoring
                        {
                            PerformanceID = (int)reader["PerformanceID"],
                            StaffID = (int)reader["stff_id"],
                            DepartmentID = (int)reader["department_id"],
                            Rating = (int)reader["Rating"],
                            PatientFeedback = reader["PatientFeedback"].ToString(),
                            CasesHandled = (int)reader["CasesHandled"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                    }
                }
            }
            return report;
        }

        public void UpdateClinicalReport(ClinicalReport report, IFormFile imageFile)
        {
            // Save the image and get the file path if a new image is uploaded.
            string imagePath = imageFile != null ? SavePhoto(imageFile) : null;

            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "UPDATE ReportsAnalytics.Clinical_Reports " +
                            "SET PatientID = @PatientID, DoctorID = @DoctorID, Diagnosis = @Diagnosis, TreatmentPlan = @TreatmentPlan, " +
                            "TreatmentStartDate = @TreatmentStartDate, TreatmentEndDate = @TreatmentEndDate, Outcome = @Outcome, " +
                            "Notes = @Notes, ImagePath = @ImagePath, UpdatedAt = @UpdatedAt " +
                            "WHERE ClinicalReportID = @ClinicalReportID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PatientID", report.PatientID);
                command.Parameters.AddWithValue("@DoctorID", report.DoctorID);
                command.Parameters.AddWithValue("@Diagnosis", report.Diagnosis);
                command.Parameters.AddWithValue("@TreatmentPlan", report.TreatmentPlan ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TreatmentStartDate", report.TreatmentStartDate);
                command.Parameters.AddWithValue("@TreatmentEndDate", (object)report.TreatmentEndDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Outcome", report.Outcome);
                command.Parameters.AddWithValue("@Notes", report.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ImagePath", imagePath ?? (object)DBNull.Value); // Use the new image path if available
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@ClinicalReportID", report.ReportID);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the update query
            }
        }


        public void UpdateFinancialReport(FinancialReport report)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "UPDATE ReportsAnalytics.Financial_Reports SET ReportDate = @ReportDate, TotalRevenue = @TotalRevenue, TotalExpenses = @TotalExpenses, Notes = @Notes WHERE financialreport_id = @ReportID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReportDate", report.ReportDate);
                command.Parameters.AddWithValue("@TotalRevenue", report.TotalRevenue);
                command.Parameters.AddWithValue("@TotalExpenses", report.TotalExpenses);
                command.Parameters.AddWithValue("@Notes", report.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ReportID", report.ReportID);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the update query
            }
        }

        public void UpdatePerformanceReport(PerformanceMonitoring report)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                var query = "UPDATE ReportsAnalytics.Performance_Reports SET StaffID = @StaffID, DepartmentID = @DepartmentID, Rating = @Rating, PatientFeedback = @PatientFeedback, CasesHandled = @CasesHandled WHERE PerformanceID = @PerformanceID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", report.StaffID);
                command.Parameters.AddWithValue("@DepartmentID", report.DepartmentID);
                command.Parameters.AddWithValue("@Rating", report.Rating);
                command.Parameters.AddWithValue("@PatientFeedback", report.PatientFeedback ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CasesHandled", report.CasesHandled);
                command.Parameters.AddWithValue("@PerformanceID", report.PerformanceID);

                connection.Open();
                command.ExecuteNonQuery(); // Execute the update query
            }
        }


    }
}
