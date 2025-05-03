using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionstring;

        public AppointmentRepository(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("DefaultConnection");
        }

        // Add Appointment
        public void AddAppointment(Appointment appointment)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                // Direct SQL Insert Query
                string query = @"
        INSERT INTO AppointmentScheduling.appointment_scheduling
        (
            patient_id,
            doctor_id,
            appointment_date,
            appointment_status,
            reason,
            created_at,
            updated_at
        )
        VALUES
        (
            @patient_id,
            @doctor_id,
            @appointment_date,
            @appointment_status,
            @reason,
            GETDATE(),
            GETDATE()
        )";

                SqlCommand cmd = new SqlCommand(query, con);

                // Add parameters to the SQL query
                cmd.Parameters.AddWithValue("@patient_id", appointment.PatientId);
                cmd.Parameters.AddWithValue("@doctor_id", appointment.DoctorId);
                cmd.Parameters.AddWithValue("@appointment_date", appointment.AppointmentDate);

                // If AppointmentStatus is null, set a default value ("Pending")
                cmd.Parameters.AddWithValue("@appointment_status",
                    appointment.AppointmentStatus ?? "Pending"); // Set default if null

                cmd.Parameters.AddWithValue("@reason", appointment.Reason);

                // Open the connection and execute the query
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }



        public void AddAppointmentAlert(AppointmentAlert alert)
        {
            // Ensure RecipientType is not null or empty
            if (string.IsNullOrEmpty(alert.RecipientType))
            {
                throw new ArgumentException("RecipientType cannot be null or empty.");
            }

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                // Define the SQL query to insert the alert into the database
                string query = "INSERT INTO AppointmentScheduling.appointment_alerts (appointment_id, recipient_type, alert_message, sent_status, sent_at) " +
                               "VALUES (@appointment_id, @recipient_type, @alert_message, @sent_status, @sent_at)";

                SqlCommand cmd = new SqlCommand(query, con);

                // Add parameters to avoid SQL injection
                cmd.Parameters.AddWithValue("@appointment_id", alert.AppointmentId);
                cmd.Parameters.AddWithValue("@recipient_type", alert.RecipientType); // Ensure this value is not null or empty
                cmd.Parameters.AddWithValue("@alert_message", alert.AlertMessage);
                cmd.Parameters.AddWithValue("@sent_status", alert.SentStatus);

                // If SentAt is null, pass DBNull.Value to the SQL parameter
                cmd.Parameters.AddWithValue("@sent_at", alert.SentAt.HasValue ? (object)alert.SentAt.Value : DBNull.Value);

                // Open the connection and execute the query
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }



        // Add Doctor Availability (without using stored procedure)
        public void AddDoctorAvailability(DoctorAvailability availability)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"INSERT INTO AppointmentScheduling.doctor_availability 
                         (doctor_id, available_date, start_time, end_time, status)
                         VALUES (@doctor_id, @available_date, @start_time, @end_time, @status)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@doctor_id", availability.DoctorId);
                cmd.Parameters.AddWithValue("@available_date", availability.AvailableDate);
                cmd.Parameters.AddWithValue("@start_time", availability.StartTime);
                cmd.Parameters.AddWithValue("@end_time", availability.EndTime);
                cmd.Parameters.AddWithValue("@status", availability.Status);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // Delete Appointment
        public void DeleteAppointment(int appointmentId)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM AppointmentScheduling.appointment_scheduling WHERE appointment_id = @appointment_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@appointment_id", appointmentId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete Appointment Alert
        public void DeleteAppointmentAlert(int alertId)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM AppointmentScheduling.appointment_alerts WHERE alert_id = @alert_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@alert_id", alertId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete Doctor Availability
        public void DeleteDoctorAvailability(int availabilityId)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM AppointmentScheduling.doctor_availability WHERE availability_id = @availability_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@availability_id", availabilityId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get all Appointments
        public IEnumerable<Appointment> GetAllAppointments()
        {
            var appointments = new List<Appointment>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                // Join the doctor data in the query
                string query = @"
            SELECT a.*, d.full_name AS DoctorName
            FROM AppointmentScheduling.appointment_scheduling a
            LEFT JOIN Doctors.doctors d ON a.doctor_id = d.doctor_id";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    appointments.Add(new Appointment
                    {
                        AppointmentId = (int)reader["appointment_id"],
                        PatientId = (int)reader["patient_id"],
                        DoctorId = (int)reader["doctor_id"],
                        AppointmentDate = (DateTime)reader["appointment_date"],
                        AppointmentStatus = reader["appointment_status"].ToString(),
                        Reason = reader["reason"].ToString(),
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"],
                        Doctor = new Doctor
                        {
                            DoctorId = (int)reader["doctor_id"],
                            FullName = reader["DoctorName"].ToString()  // Manually map Doctor's name
                        }
                    });
                }
            }
            return appointments;
        }

        // Get Appointment by ID
        public Appointment GetAppointmentById(int appointmentId)
        {
            Appointment appointment = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM AppointmentScheduling.appointment_scheduling WHERE appointment_id = @appointment_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@appointment_id", appointmentId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    appointment = new Appointment
                    {
                        AppointmentId = (int)reader["appointment_id"],
                        PatientId = (int)reader["patient_id"],
                        DoctorId = (int)reader["doctor_id"],
                        AppointmentDate = (DateTime)reader["appointment_date"],
                        AppointmentStatus = reader["appointment_status"].ToString(),
                        Reason = reader["reason"].ToString(),
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    };
                }
            }
            return appointment;
        }

        // Get all Doctor Availabilities
        public IEnumerable<DoctorAvailability> GetAllDoctorAvailabilities()
        {
            var availabilities = new List<DoctorAvailability>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM AppointmentScheduling.doctor_availability";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    availabilities.Add(new DoctorAvailability
                    {
                        AvailabilityId = (int)reader["availability_id"],
                        DoctorId = (int)reader["doctor_id"],
                        AvailableDate = (DateTime)reader["available_date"],
                        StartTime = (TimeSpan)reader["start_time"],
                        EndTime = (TimeSpan)reader["end_time"],
                        Status = reader["status"].ToString()
                    });
                }
            }
            return availabilities;
        }

        // Get Doctor Availability by ID
        public DoctorAvailability GetDoctorAvailabilityById(int availabilityId)
        {
            DoctorAvailability availability = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM AppointmentScheduling.doctor_availability WHERE availability_id = @availability_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@availability_id", availabilityId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    availability = new DoctorAvailability
                    {
                        AvailabilityId = (int)reader["availability_id"],
                        DoctorId = (int)reader["doctor_id"],
                        AvailableDate = (DateTime)reader["available_date"],
                        StartTime = (TimeSpan)reader["start_time"],
                        EndTime = (TimeSpan)reader["end_time"],
                        Status = reader["status"].ToString()
                    };
                }
            }
            return availability;
        }

        // Get all Appointment Alerts
        public IEnumerable<AppointmentAlert> GetAllAppointmentAlerts()
        {
            var alerts = new List<AppointmentAlert>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM AppointmentScheduling.appointment_alerts";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    alerts.Add(new AppointmentAlert
                    {
                        AlertId = (int)reader["alert_id"],
                        AppointmentId = (int)reader["appointment_id"],
                        RecipientType = reader["recipient_type"].ToString(),
                        AlertMessage = reader["alert_message"].ToString(),
                        SentStatus = reader["sent_status"].ToString(),
                        SentAt = reader["sent_at"] as DateTime?
                    });
                }
            }
            return alerts;
        }

        // Get Appointment Status List (Example)
        public List<SelectListItem> GetAppointmentStatusList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Scheduled", Value = "Scheduled" },
                new SelectListItem { Text = "Completed", Value = "Completed" },
                new SelectListItem { Text = "Cancelled", Value = "Cancelled" }
            };
        }

        // Get Doctor List for SelectList
        public List<SelectListItem> GetDoctorList()
        {
            var doctorList = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT doctor_id, full_name FROM Doctors.doctors";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    doctorList.Add(new SelectListItem
                    {
                        Value = reader["doctor_id"].ToString(),
                        Text = reader["full_name"].ToString()
                    });
                }
            }
            return doctorList;
        }

        // Get Patient List for SelectList (Example)
        public List<SelectListItem> GetPatientList()
        {
            var patientList = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT patient_id, full_name FROM Patients.patient_registration";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    patientList.Add(new SelectListItem
                    {
                        Value = reader["patient_id"].ToString(),
                        Text = reader["full_name"].ToString()
                    });
                }
            }
            return patientList;
        }

        // Update Appointment
        public void UpdateAppointment(Appointment appointment)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"UPDATE AppointmentScheduling.appointment_scheduling
                         SET patient_id = @patient_id,
                             doctor_id = @doctor_id,
                             appointment_date = @appointment_date,
                             appointment_status = @appointment_status,
                             reason = @reason
                         WHERE appointment_id = @appointment_id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@appointment_id", appointment.AppointmentId);
                cmd.Parameters.AddWithValue("@patient_id", appointment.PatientId);
                cmd.Parameters.AddWithValue("@doctor_id", appointment.DoctorId);
                cmd.Parameters.AddWithValue("@appointment_date", appointment.AppointmentDate);
                cmd.Parameters.AddWithValue("@appointment_status", appointment.AppointmentStatus);
                cmd.Parameters.AddWithValue("@reason", appointment.Reason);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // Update Appointment Alert
        public void UpdateAppointmentAlert(AppointmentAlert alert)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                // Write the SQL UPDATE query
                string query = @"
            UPDATE AppointmentScheduling.appointment_alerts
            SET 
                appointment_id = @appointment_id,
                recipient_type = @recipient_type,
                alert_message = @alert_message,
                sent_status = @sent_status
            WHERE alert_id = @alert_id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Add parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@alert_id", alert.AlertId);
                cmd.Parameters.AddWithValue("@appointment_id", alert.AppointmentId);
                cmd.Parameters.AddWithValue("@recipient_type", alert.RecipientType);
                cmd.Parameters.AddWithValue("@alert_message", alert.AlertMessage);
                cmd.Parameters.AddWithValue("@sent_status", alert.SentStatus);

                // Open the connection and execute the query
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }



        // Update Doctor Availability
        public void UpdateDoctorAvailability(DoctorAvailability availability)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            UPDATE AppointmentScheduling.doctor_availability
            SET 
                doctor_id = @doctor_id,
                available_date = @available_date,
                start_time = @start_time,
                end_time = @end_time,
                status = @status
            WHERE availability_id = @availability_id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@availability_id", availability.AvailabilityId);
                    cmd.Parameters.AddWithValue("@doctor_id", availability.DoctorId);
                    cmd.Parameters.AddWithValue("@available_date", availability.AvailableDate);
                    cmd.Parameters.AddWithValue("@start_time", availability.StartTime);
                    cmd.Parameters.AddWithValue("@end_time", availability.EndTime);
                    cmd.Parameters.AddWithValue("@status", availability.Status);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get Appointment Alert by ID
        public AppointmentAlert GetAppointmentAlertById(int alertId)
        {
            AppointmentAlert alert = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM AppointmentScheduling.appointment_alerts WHERE alert_id = @alert_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@alert_id", alertId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    alert = new AppointmentAlert
                    {
                        AlertId = (int)reader["alert_id"],
                        AppointmentId = (int)reader["appointment_id"],
                        RecipientType = reader["recipient_type"].ToString(),
                        AlertMessage = reader["alert_message"].ToString(),
                        SentStatus = reader["sent_status"].ToString(),
                        SentAt = reader["sent_at"] as DateTime?
                    };
                }
            }
            return alert;
        }

        public List<SelectListItem> GetAppointmentId()
        {
            List<SelectListItem> appointmentList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                // Query to get appointment IDs and any other necessary fields from your appointment table
                string query = "SELECT appointment_id, CONCAT('Appointment ', appointment_id) AS display_text FROM AppointmentScheduling.appointment_scheduling";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Adding each appointment ID to the list
                    appointmentList.Add(new SelectListItem
                    {
                        Value = reader["appointment_id"].ToString(),
                        Text = reader["display_text"].ToString()
                    });
                }
            }

            return appointmentList;
        }

        public List<Appointment> GetAppointmentsByPatientId(int patientId)
        {
            List<Appointment> appointments = new List<Appointment>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
           SELECT 
                a.patient_id,
                a.appointment_id,
                a.appointment_date,
                a.reason,
                a.doctor_id,
                d.full_name AS doctor_name
           FROM 
                AppointmentScheduling.appointment_scheduling a
           INNER JOIN 
                Doctors.doctors d ON a.doctor_id = d.doctor_id
           WHERE 
                a.patient_id = @PatientId"; // Ensure @PatientId is used correctly

                SqlCommand cmd = new SqlCommand(query, con);
                // Pass the actual patientId as a parameter to the query
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    appointments.Add(new Appointment
                    {
                        AppointmentId = Convert.ToInt32(reader["appointment_id"]),
                        AppointmentDate = Convert.ToDateTime(reader["appointment_date"]),
                        DoctorId = Convert.ToInt32(reader["doctor_id"]),
                        Reason = reader["reason"].ToString(),
                        Doctor = new Doctor
                        {
                            DoctorId = Convert.ToInt32(reader["doctor_id"]),
                            FullName = reader["doctor_name"].ToString()
                        }
                    });
                }
            }

            return appointments;
        }
        public List<AppointmentInfo> GetAppointmentsByPatientName(string patientName)
        {
            List<AppointmentInfo> appointments = new List<AppointmentInfo>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            SELECT 
                a.appointment_id,
                a.patient_id,
                a.doctor_id,
                a.appointment_date,
                a.reason,
                a.appointment_status,
                p.first_name,
                p.last_name,
                d.full_name AS doctor_name
            FROM AppointmentScheduling.appointment_scheduling a
            INNER JOIN patient.patient_registration p ON a.patient_id = p.patient_id
            INNER JOIN Doctors.doctors d ON a.doctor_id = d.doctor_id
            WHERE CONCAT(p.first_name, ' ', p.last_name) LIKE @PatientName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientName", "%" + patientName + "%"); // Important: Add % for wildcard match
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            appointments.Add(new AppointmentInfo
                            {
                                AppointmentId = Convert.ToInt32(reader["appointment_id"]),
                                PatientId = Convert.ToInt32(reader["patient_id"]),
                                DoctorId = Convert.ToInt32(reader["doctor_id"]),
                                AppointmentDate = Convert.ToDateTime(reader["appointment_date"]),
                                Reason = reader["reason"].ToString(),
                                Status = reader["appointment_status"].ToString(),
                                PatientName = reader["first_name"].ToString() + " " + reader["last_name"].ToString(),
                                DoctorName = reader["doctor_name"].ToString()
                            });
                        }
                    }
                }
            }

            return appointments;
        }

    }
}
 
