using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Repositories
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly string _connectionString;

        // Constructor to inject connection string
        public LabTestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        // Method to fetch all lab tests
        public List<LabTestViewModel> GetAllLabTests()
        {
            List<LabTestViewModel> tests = new List<LabTestViewModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM LaboratoryManagement.Lab_Tests";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        tests.Add(new LabTestViewModel
                        {
                            TestId = reader.GetInt32(0),
                            TestName = reader.GetString(1),
                            TestDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                            TestCategory = reader.GetString(3),
                            TestType = reader.GetString(4),
                            TestDuration = reader.GetInt32(5),
                            TestCost = reader.GetDecimal(6),
                            TestMethod = reader.GetString(7)
                        });
                    }
                }
            }

            return tests;
        }



        // Method to fetch a single lab test by ID
        public LabTestViewModel GetLabTestById(int testId)
        {
            LabTestViewModel test = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM LaboratoryManagement.Lab_Tests WHERE test_id = @TestId";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestId", testId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        test = new LabTestViewModel
                        {
                            TestId = reader.GetInt32(0),
                            TestName = reader.GetString(1),
                            TestDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                            TestCategory = reader.GetString(3),
                            TestType = reader.GetString(4),
                            TestDuration = reader.GetInt32(5),
                            TestCost = reader.GetDecimal(6),
                            TestMethod = reader.GetString(7)
                        };
                    }
                }
            }

            return test;
        }


            public IEnumerable<TestBooking> GetAllBookings()
            {
                var bookings = new List<TestBooking>();
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"SELECT booking_id, patient_id, test_id, test_date, sample_collected_date, 
                                                additional_notes, booking_status, doctor_id, test_type 
                                         FROM LaboratoryManagement.Test_Bookings", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var booking = new TestBooking
                    {
                        BookingId = Convert.ToInt32(reader["booking_id"]),
                        PatientId = Convert.ToInt32(reader["patient_id"]),
                        TestId = Convert.ToInt32(reader["test_id"]),
                        TestDate = Convert.ToDateTime(reader["test_date"]),
                        SampleCollectedDate = reader["sample_collected_date"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(reader["sample_collected_date"]),
                        AdditionalNotes = reader["additional_notes"]?.ToString(),
                        BookingStatus = reader["booking_status"].ToString()!,
                        DoctorId = Convert.ToInt32(reader["doctor_id"]),
                        TestType = reader["test_type"].ToString()!
                    };

                    bookings.Add(booking);
                }

                return bookings;
            }
        public IEnumerable<TestBooking> GetConfirmBookings()
        {
            var bookings = new List<TestBooking>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"SELECT booking_id, patient_id, test_id, test_date, sample_collected_date, 
                                                additional_notes, booking_status, doctor_id, test_type 
                                         FROM LaboratoryManagement.Test_Bookings
                                        where booking_status = 'Confirmed'", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var booking = new TestBooking
                {
                    BookingId = Convert.ToInt32(reader["booking_id"]),
                    PatientId = Convert.ToInt32(reader["patient_id"]),
                    TestId = Convert.ToInt32(reader["test_id"]),
                    TestDate = Convert.ToDateTime(reader["test_date"]),
                    SampleCollectedDate = reader["sample_collected_date"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["sample_collected_date"]),
                    AdditionalNotes = reader["additional_notes"]?.ToString(),
                    BookingStatus = reader["booking_status"].ToString()!,
                    DoctorId = Convert.ToInt32(reader["doctor_id"]),
                    TestType = reader["test_type"].ToString()!
                };

                bookings.Add(booking);
            }

            return bookings;
        }

        public IEnumerable<TestBooking> GetConfirmAndCollectedBookings()
        {
            var bookings = new List<TestBooking>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
        SELECT tb.booking_id, tb.patient_id, tb.test_id, tb.test_date, tb.sample_collected_date, 
               tb.additional_notes, tb.booking_status, tb.doctor_id, tb.test_type, pt.result_status
        FROM LaboratoryManagement.Test_Bookings tb
INNER JOIN LaboratoryManagement.Patient_Tests pt 
    ON tb.booking_id = pt.test_order_id
WHERE tb.booking_status = 'Confirmed' 
  AND tb.sample_collected_date IS NOT NULL", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var booking = new TestBooking
                {
                    BookingId = Convert.ToInt32(reader["booking_id"]),
                    PatientId = Convert.ToInt32(reader["patient_id"]),
                    TestId = Convert.ToInt32(reader["test_id"]),
                    TestDate = Convert.ToDateTime(reader["test_date"]),
                    SampleCollectedDate = reader["sample_collected_date"] == DBNull.Value
        ? null
        : Convert.ToDateTime(reader["sample_collected_date"]),
                    AdditionalNotes = reader["additional_notes"]?.ToString(),
                    BookingStatus = reader["booking_status"].ToString()!,
                    DoctorId = Convert.ToInt32(reader["doctor_id"]),
                    TestType = reader["test_type"].ToString()!,
                    ResultStatus = reader["result_status"]?.ToString()
                };

                bookings.Add(booking);
            }

            return bookings;
        }

        public TestBooking? GetBookingById(int id)
        {
            TestBooking? booking = null;
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM LaboratoryManagement.Test_Bookings WHERE booking_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                booking = new TestBooking
                {
                    BookingId = (int)reader["booking_id"],
                    PatientId = (int)reader["patient_id"],
                    TestId = (int)reader["test_id"],
                    TestDate = (DateTime)reader["test_date"],
                    SampleCollectedDate = reader["sample_collected_date"] == DBNull.Value
                        ? null
                        : (DateTime?)reader["sample_collected_date"],
                    AdditionalNotes = reader["additional_notes"]?.ToString(),
                    BookingStatus = reader["booking_status"].ToString()!,
                    DoctorId = (int)reader["doctor_id"],
                    TestType = reader["test_type"].ToString()!
                };
            }

            return booking;
        }

        public void AddBooking(TestBooking booking)
        {
            using var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(@"INSERT INTO LaboratoryManagement.Test_Bookings 
        (patient_id, test_id, test_date, sample_collected_date, additional_notes, booking_status, doctor_id, test_type) 
        VALUES (@PatientId, @TestId, @TestDate, @SampleCollectedDate, @AdditionalNotes, @BookingStatus, @DoctorId, @TestType)", conn);

            cmd.Parameters.AddWithValue("@PatientId", booking.PatientId);
            cmd.Parameters.AddWithValue("@TestId", booking.TestId);
            cmd.Parameters.AddWithValue("@TestDate", booking.TestDate);
            cmd.Parameters.AddWithValue("@SampleCollectedDate", (object?)booking.SampleCollectedDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AdditionalNotes", (object?)booking.AdditionalNotes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BookingStatus", booking.BookingStatus);
            cmd.Parameters.AddWithValue("@DoctorId", booking.DoctorId);
            cmd.Parameters.AddWithValue("@TestType", booking.TestType);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void CollectSample(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Update Test_Bookings with sample collected date
                string updateBookingQuery = @"
                    UPDATE [LaboratoryManagement].[Test_Bookings]
                    SET sample_collected_date = @CollectedDate
                    WHERE booking_id = @BookingId AND sample_collected_date IS NULL";

                using (SqlCommand updateBookingCmd = new SqlCommand(updateBookingQuery, conn))
                {
                    updateBookingCmd.Parameters.AddWithValue("@CollectedDate", DateTime.Now);
                    updateBookingCmd.Parameters.AddWithValue("@BookingId", bookingId);
                    updateBookingCmd.ExecuteNonQuery();
                }

                // Check if Patient_Tests record exists
                string checkPatientTestQuery = @"SELECT COUNT(*) 
                                        FROM LaboratoryManagement.Patient_Tests 
                                        WHERE test_order_id = @BookingId";

                using (SqlCommand checkPatientTestCmd = new SqlCommand(checkPatientTestQuery, conn))
                {
                    checkPatientTestCmd.Parameters.AddWithValue("@BookingId", bookingId);
                    int count = (int)checkPatientTestCmd.ExecuteScalar();

                    // If no Patient_Tests record exists, insert it
                    if (count == 0)
                    {
                        string insertPatientTestQuery = @"
                            INSERT INTO [LaboratoryManagement].[Patient_Tests]
                            (test_order_id, result_status, result_received)
                            VALUES (@BookingId, 'Pending', 0)";

                        using (SqlCommand insertPatientTestCmd = new SqlCommand(insertPatientTestQuery, conn))
                        {
                            insertPatientTestCmd.Parameters.AddWithValue("@BookingId", bookingId);
                            insertPatientTestCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void ConfirmTest(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                    string query = @"UPDATE LaboratoryManagement.Test_Bookings 
                                 SET booking_status = 'Confirmed' 
                                 WHERE booking_id = @BookingId AND booking_status != 'Confirmed'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public IEnumerable<PatientTestViewModel> GetAllPatientTests()
        {
            List<PatientTestViewModel> patientTestViewModels = new List<PatientTestViewModel>();

            // Define the SQL query to get the required data from the database
            string query = @"
                        SELECT 
                    pt.patient_test_id, 
                    pt.test_order_id, 
                    pt.result_received, 
                    pt.result_status,
                    lt.test_name,
                    lt.test_type,
                    lt.test_category,
                    lt.test_duration,
                    lt.test_cost,
                    lr.report_id
                FROM LaboratoryManagement.Patient_Tests pt
                INNER JOIN LaboratoryManagement.Test_Bookings tb ON pt.test_order_id = tb.booking_id
                INNER JOIN LaboratoryManagement.Lab_Tests lt ON tb.test_id = lt.test_id
                LEFT JOIN LaboratoryManagement.Lab_Reports lr ON pt.patient_test_id = lr.patient_test_id
";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var patientTestViewModel = new PatientTestViewModel
                            {
                                
                                PatientTestId = reader.GetInt32(reader.GetOrdinal("patient_test_id")),
                                TestOrderId = reader.GetInt32(reader.GetOrdinal("test_order_id")),
                                ResultReceived = reader.GetBoolean(reader.GetOrdinal("result_received")),
                                ResultStatus = reader.GetString(reader.GetOrdinal("result_status")),
                                TestName = reader.GetString(reader.GetOrdinal("test_name")),
                                TestType = reader.GetString(reader.GetOrdinal("test_type")),
                                TestCategory = reader.GetString(reader.GetOrdinal("test_category")),
                                TestDuration = reader.GetInt32(reader.GetOrdinal("test_duration")),
                                TestCost = reader.GetDecimal(reader.GetOrdinal("test_cost")),
                                ReportId = reader.IsDBNull(reader.GetOrdinal("report_id"))
    ? 0 // or any default int value
    : reader.GetInt32(reader.GetOrdinal("report_id"))

                            };
                            patientTestViewModels.Add(patientTestViewModel);
                        }
                    }
                }
            }

            // Return a list of PatientTestViewModel instead of PatientTest
            return patientTestViewModels;
        }


        public PatientTestViewModel GetPatientTestById(int id)
        {
            PatientTestViewModel patientTest = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT pt.patient_test_id,  -- PatientTestId
                   tb.booking_id AS test_order_id,  -- TestOrderId
                   tb.test_date, 
                   tb.sample_collected_date, 
                   lt.test_name, 
                   lt.test_type, 
                   lt.test_category, 
                   lt.test_duration, 
                   lt.test_cost,
                   tb.booking_status,
                   tb.additional_notes,
                   pt.result_status,  -- ResultStatus from Patient_Tests
                   pt.result_received  -- ResultReceived from Patient_Tests
            FROM LaboratoryManagement.Patient_Tests pt
            INNER JOIN LaboratoryManagement.Test_Bookings tb ON pt.test_order_id = tb.booking_id
            INNER JOIN LaboratoryManagement.Lab_Tests lt ON tb.test_id = lt.test_id
            WHERE pt.patient_test_id = @id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            patientTest = new PatientTestViewModel
                            {
                                PatientTestId = reader.GetInt32(0),  // PatientTestId
                                TestOrderId = reader.GetInt32(1),   // TestOrderId
                                TestDate = reader.GetDateTime(2),
                                SampleCollectedDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                TestName = reader.GetString(4),
                                TestType = reader.GetString(5),
                                TestCategory = reader.GetString(6),
                                TestDuration = reader.GetInt32(7),
                                TestCost = reader.GetDecimal(8),
                                BookingStatus = reader.GetString(9),
                                AdditionalNotes = reader.IsDBNull(10) ? null : reader.GetString(10),
                                ResultStatus = reader.IsDBNull(11) ? "not received" : reader.GetString(11),  // Handle result status
                                ResultReceived = reader.IsDBNull(12) ? false : reader.GetBoolean(12)  // Handle result received
                            };
                        }
                    }
                }
            }

            return patientTest;
        }


        public LabReportViewModel GetReportById(int reportId)
        {
            LabReportViewModel report = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    lr.report_id,
                    lr.report_date,
                    lr.test_result,
                    lr.findings,
                    lr.report_status,
                    lr.doctor_notes,
                    lr.report_file_path,
                    lr.report_pdf,
                    lt.test_name,
                    pr.first_name + ' ' + pr.last_name AS patient_name,
                    pr.gender,
                    pr.dob
                FROM LaboratoryManagement.Lab_Reports lr
                INNER JOIN LaboratoryManagement.Patient_Tests pt ON lr.patient_test_id = pt.patient_test_id
                INNER JOIN LaboratoryManagement.Test_Bookings tb ON pt.test_order_id = tb.booking_id
                INNER JOIN LaboratoryManagement.Lab_Tests lt ON tb.test_id = lt.test_id
                INNER JOIN patient.patient_registration pr ON tb.patient_id = pr.patient_id
                WHERE lr.patient_test_id = @patient_test_id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@patient_test_id", reportId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            report = new LabReportViewModel
                            {
                                ReportId = reader.GetInt32(0),
                                ReportDate = reader.GetDateTime(1),
                                TestResult = reader.GetString(2),
                                Findings = reader.GetString(3),
                                ReportStatus = reader.GetString(4),
                                DoctorNotes = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ReportFilePath = reader.IsDBNull(6) ? null : reader.GetString(6),
                                ReportPdf = reader.IsDBNull(7) ? null : (byte[])reader["report_pdf"],
                                TestName = reader.GetString(8),
                                PatientName = reader.GetString(9),
                                Gender = reader.GetString(10),
                                DOB = reader.GetDateTime(11)
                            };
                        }
                    }
                }
            }

            return report;
        }

        public LabReportViewModel GetReportByReportId(int reportId)
        {
            LabReportViewModel report = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    lr.report_id,
                    lr.report_date,
                    lr.test_result,
                    lr.findings,
                    lr.report_status,
                    lr.doctor_notes,
                    lr.report_file_path,
                    lr.report_pdf,
                    lt.test_name,
                    pr.first_name + ' ' + pr.last_name AS patient_name,
                    pr.gender,
                    pr.dob
                FROM LaboratoryManagement.Lab_Reports lr
                INNER JOIN LaboratoryManagement.Patient_Tests pt ON lr.patient_test_id = pt.patient_test_id
                INNER JOIN LaboratoryManagement.Test_Bookings tb ON pt.test_order_id = tb.booking_id
                INNER JOIN LaboratoryManagement.Lab_Tests lt ON tb.test_id = lt.test_id
                INNER JOIN patient.patient_registration pr ON tb.patient_id = pr.patient_id
                WHERE lr.report_id = @report_id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@report_id", reportId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            report = new LabReportViewModel
                            {
                                ReportId = reader.GetInt32(0),
                                ReportDate = reader.GetDateTime(1),
                                TestResult = reader.GetString(2),
                                Findings = reader.GetString(3),
                                ReportStatus = reader.GetString(4),
                                DoctorNotes = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ReportFilePath = reader.IsDBNull(6) ? null : reader.GetString(6),
                                ReportPdf = reader.IsDBNull(7) ? null : (byte[])reader["report_pdf"],
                                TestName = reader.GetString(8),
                                PatientName = reader.GetString(9),
                                Gender = reader.GetString(10),
                                DOB = reader.GetDateTime(11)
                            };
                        }
                    }
                }
            }

            return report;
        }

        public void AddLabReport(LabReport report)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Insert lab report
                string insertQuery = @"
            INSERT INTO [LaboratoryManagement].[Lab_Reports]
            (
                patient_test_id, report_date, test_result, findings,
                report_status, doctor_notes, lab_technician_id
            )
            VALUES
            (
                @PatientTestId, @ReportDate, @TestResult, @Findings,
                @ReportStatus, @DoctorNotes, @LabTechnicianId
            )";

                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@PatientTestId", report.PatientTestId);
                    insertCmd.Parameters.AddWithValue("@ReportDate", report.ReportDate);
                    insertCmd.Parameters.AddWithValue("@TestResult", report.TestResult);
                    insertCmd.Parameters.AddWithValue("@Findings", report.Findings);
                    insertCmd.Parameters.AddWithValue("@ReportStatus", report.ReportStatus);
                    insertCmd.Parameters.AddWithValue("@DoctorNotes", report.DoctorNotes ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@LabTechnicianId", report.LabTechnicianId);

                    insertCmd.ExecuteNonQuery();
                }

                // Update patient_test status
                string updateQuery = @"
            UPDATE [LaboratoryManagement].[Patient_Tests]
            SET result_status = 'Completed', result_received = 1
            WHERE patient_test_id = @PatientTestId";

                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@PatientTestId", report.PatientTestId);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }


        public void UpdateLabReportPdfAndPath(int reportId, byte[] pdfData, string filePath)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE [LaboratoryManagement].[Lab_Reports]
            SET 
                report_pdf = @ReportPdf,
                report_file_path = @ReportFilePath
            WHERE 
                report_id = @ReportId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReportPdf", pdfData ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReportFilePath", filePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReportId", reportId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<LabReportViewModel> GetLabReports()
        {
            var labReports = new List<LabReportViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT r.report_id, r.report_file_path AS reportpdf, r.patient_test_id
                              FROM LaboratoryManagement.Lab_Reports r
                              INNER JOIN LaboratoryManagement.Patient_Tests pt ON r.patient_test_id = pt.patient_test_id
                              WHERE r.report_file_path IS NOT NULL";

                var command = new SqlCommand(query, connection);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var labReport = new LabReportViewModel
                        {
                            ReportId = reader.GetInt32(reader.GetOrdinal("report_id")),
                            ReportFilePath = reader.GetString(reader.GetOrdinal("reportpdf")),
                            PatientTestId = reader.GetInt32(reader.GetOrdinal("patient_test_id"))
                        };
                        labReports.Add(labReport);
                    }
                }
            }

            return labReports;
        }

        public List<LabReportViewModel2> GetReportsByPatientId(int patientId)
        {
            var reports = new List<LabReportViewModel2>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT 
                r.report_id, 
                r.report_file_path, 
                pt.test_order_id, 
                tb.patient_id, 
                lt.test_name, 
                d.full_name 
            FROM 
                LaboratoryManagement.Lab_Reports r
            INNER JOIN 
                LaboratoryManagement.Patient_Tests pt ON r.patient_test_id = pt.patient_test_id
            INNER JOIN 
                LaboratoryManagement.Test_Bookings tb ON pt.test_order_id = tb.booking_id
            INNER JOIN 
                LaboratoryManagement.Lab_Tests lt ON tb.test_id = lt.test_id
            INNER JOIN 
                Doctors.doctors d ON tb.doctor_id = d.doctor_id
            WHERE 
                tb.patient_id = @PatientId
                AND r.report_file_path IS NOT NULL";

                // Open the connection
                connection.Open();

                // Create and execute the SQL command
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);

                    // Execute the query and get a SqlDataReader
                    using (var reader = command.ExecuteReader())
                    {
                        // Loop through the results and manually map them to the view model
                        while (reader.Read())
                        {
                            var report = new LabReportViewModel2
                            {
                                ReportId = reader.GetInt32(reader.GetOrdinal("report_id")),
                                ReportPdf = reader.GetString(reader.GetOrdinal("report_file_path")),
                                TestOrderId = reader.GetInt32(reader.GetOrdinal("test_order_id")),
                                PatientId = reader.GetInt32(reader.GetOrdinal("patient_id")),
                                TestName = reader.GetString(reader.GetOrdinal("test_name")),
                                DoctorName = reader.GetString(reader.GetOrdinal("full_name"))
                            };

                            // Add the report to the list
                            reports.Add(report);
                        }
                    }
                }
            }

            // Return the list of reports
            return reports;
        }



    }
}
