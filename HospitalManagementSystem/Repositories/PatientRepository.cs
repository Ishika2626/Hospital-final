

using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace HospitalManagementSystem.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        
        private readonly string _connectionstring;

        public PatientRepository(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("DefaultConnection");
           
        }

        public IEnumerable<PatientRegistration> GetAll()
        {
            var patientList = new List<PatientRegistration>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_registration";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    patientList.Add(new PatientRegistration
                    {
                        PatientId = (int)reader["patient_id"],
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString(),
                        Gender = reader["gender"].ToString(),
                        Dob = (DateTime)reader["dob"],
                        Address = reader["address"].ToString(),
                        Email = reader["email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        EmergencyContactName = reader["emergency_contact_name"].ToString(),
                        EmergencyContactPhone = reader["emergency_contact_phone"].ToString(),
                        MaritalStatus = reader["marital_status"].ToString(),
                        Nationality = reader["nationality"].ToString(),
                        BloodGroup = reader["blood_group"].ToString(),
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"],
                        Password = reader["password"].ToString(),
                        patient_img = reader["patient_img"].ToString()

                    });
                }
            }
            return patientList;
        }

        public PatientRegistration GetById(int patient_id)
        {
            PatientRegistration patientList = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_registration WHERE patient_id = @patient_id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@patient_id", patient_id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    patientList = new PatientRegistration
                    {
                        PatientId = (int)reader["patient_id"],
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString(),
                        Gender = reader["gender"].ToString(),
                        Dob = (DateTime)reader["dob"],
                        Address = reader["address"].ToString(),
                        Email = reader["email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        EmergencyContactName = reader["emergency_contact_name"].ToString(),
                        EmergencyContactPhone = reader["emergency_contact_phone"].ToString(),
                        MaritalStatus = reader["marital_status"].ToString(),
                        Nationality = reader["nationality"].ToString(),
                        BloodGroup = reader["blood_group"].ToString(),
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"],
                        Password = reader["password"].ToString(),
                        patient_img = reader["patient_img"].ToString()

                    };
                }
            }
            return patientList;
        }
       

        public void Delete(int patient_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM  patient.patient_registration WHERE patient_id = @patient_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@patient_id", patient_id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }  
        }

        public void Update(PatientRegistration patient, IFormFile patient_img)
        {
            string newPhotoPath = patient.patient_img; // Default to existing image

            if (patient_img != null && patient_img.Length > 0)
            {
                newPhotoPath = SavePhoto(patient_img);

                if (string.IsNullOrEmpty(newPhotoPath))
                {
                    throw new Exception("Error saving photo.");
                }
            }

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            UPDATE [patient].[patient_registration]
            SET
                first_name = @first_name,
                last_name = @last_name,
                gender = @gender,
                dob = @dob,
                address = @address,
                email = @email,
                phone_number = @phone_number,
                emergency_contact_name = @emergency_contact_name,
                emergency_contact_phone = @emergency_contact_phone,
                marital_status = @marital_status,
                nationality = @nationality,
                blood_group = @blood_group,
                password = @password,
                patient_img = @patient_img,
                updated_at = GETDATE()
            WHERE patient_id = @patient_id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@patient_id", patient.PatientId); // Add this line

                cmd.Parameters.AddWithValue("@first_name", patient.FirstName);
                cmd.Parameters.AddWithValue("@last_name", patient.LastName);
                cmd.Parameters.AddWithValue("@gender", patient.Gender);
                cmd.Parameters.AddWithValue("@dob", patient.Dob);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(patient.Address) ? (object)DBNull.Value : patient.Address);
                cmd.Parameters.AddWithValue("@email", patient.Email);
                cmd.Parameters.AddWithValue("@phone_number", patient.PhoneNumber);
                cmd.Parameters.AddWithValue("@emergency_contact_name", patient.EmergencyContactName);
                cmd.Parameters.AddWithValue("@emergency_contact_phone", patient.EmergencyContactPhone);
                cmd.Parameters.AddWithValue("@marital_status", patient.MaritalStatus);
                cmd.Parameters.AddWithValue("@nationality", patient.Nationality);
                cmd.Parameters.AddWithValue("@blood_group", patient.BloodGroup);
                cmd.Parameters.AddWithValue("@password", patient.Password);

                cmd.Parameters.AddWithValue("@patient_img",newPhotoPath ?? (object)DBNull.Value);
               

                con.Open();
                cmd.ExecuteNonQuery();

            }
        }

        public void Add(PatientRegistration patient, IFormFile patient_img)
        {
            string photo = SavePhoto(patient_img);
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            INSERT INTO [patient].[patient_registration] 
            (
                first_name,
                last_name,
                gender,
                dob,
                address,
                email,
                phone_number,
                emergency_contact_name,
                emergency_contact_phone,
                marital_status,
                nationality,
                blood_group,
                password,
                patient_img,
                created_at,
                updated_at
            )
            VALUES 
            (
                @first_name,
                @last_name,
                @gender,
                @dob,
                @address,
                @email,
                @phone_number,
                @emergency_contact_name,
                @emergency_contact_phone,
                @marital_status,
                @nationality,
                @blood_group,
                @password,
                @patient_img,
                GETDATE(),
                GETDATE()
            )";

                SqlCommand cmd = new SqlCommand(query, con);


                cmd.Parameters.AddWithValue("@first_name", patient.FirstName);
                cmd.Parameters.AddWithValue("@last_name", patient.LastName);
                cmd.Parameters.AddWithValue("@gender", patient.Gender);
                cmd.Parameters.AddWithValue("@dob", patient.Dob);
                cmd.Parameters.AddWithValue("@address",patient.Address);
                cmd.Parameters.AddWithValue("@email", patient.Email);
                cmd.Parameters.AddWithValue("@phone_number", patient.PhoneNumber);
                cmd.Parameters.AddWithValue("@emergency_contact_name", patient.EmergencyContactName);
                cmd.Parameters.AddWithValue("@emergency_contact_phone", patient.EmergencyContactPhone);
                cmd.Parameters.AddWithValue("@marital_status", patient.MaritalStatus);
                cmd.Parameters.AddWithValue("@nationality", patient.Nationality);
                cmd.Parameters.AddWithValue("@blood_group", patient.BloodGroup);
                cmd.Parameters.AddWithValue("@password", patient.Password);

                cmd.Parameters.AddWithValue("@patient_img", string.IsNullOrEmpty(photo) ? (object)DBNull.Value : photo);

                con.Open();
                cmd.ExecuteNonQuery();
                
            }
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

        //patient_admission
        public IEnumerable<patient_admission> GetAllpatient_admission()
        {
            var admissionList = new List<patient_admission>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_admission";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    admissionList.Add(new patient_admission
                    {
                        admission_id = (int)reader["admission_id"],
                        patient_id = (int)reader["patient_id"],
                        admission_date = (DateTime)reader["admission_date"],
                        discharge_date = reader["discharge_date"] != DBNull.Value ? (DateTime?)reader["discharge_date"] : null,
                        room_id = reader["room_id"] != DBNull.Value ? (int?)reader["room_id"] : null,
                        bed_id = reader["bed_id"] != DBNull.Value ? (int?)reader["bed_id"] : null,
                        discharge_reason = reader["discharge_reason"]?.ToString(),
                        status = reader["status"].ToString()
                    });
                }
            }
            return admissionList;
        }

        public patient_admission Getpatient_admissionById(int admission_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_admission WHERE admission_id = @admission_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@admission_id", admission_id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new patient_admission
                    {
                        admission_id = (int)reader["admission_id"],
                        patient_id = (int)reader["patient_id"],
                        admission_date = (DateTime)reader["admission_date"],
                        discharge_date = reader["discharge_date"] != DBNull.Value ? (DateTime?)reader["discharge_date"] : null,
                        room_id = reader["room_id"] != DBNull.Value ? (int?)reader["room_id"] : null,
                        bed_id = reader["bed_id"] != DBNull.Value ? (int?)reader["bed_id"] : null,
                        discharge_reason = reader["discharge_reason"]?.ToString(),
                        status = reader["status"].ToString()
                    };
                }
            }
            return null;
        }
        public void Addpatient_admission(patient_admission admission)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            INSERT INTO patient.patient_admission 
            (
                patient_id, admission_date, discharge_date, 
                room_id, bed_id, discharge_reason, status
            )
            VALUES 
            (
                @patient_id, @admission_date, @discharge_date, 
                @room_id, @bed_id, @discharge_reason, @status
            )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@patient_id", admission.patient_id);
                cmd.Parameters.AddWithValue("@admission_date", admission.admission_date);
                cmd.Parameters.AddWithValue("@discharge_date", admission.discharge_date ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@room_id", admission.room_id);
                cmd.Parameters.AddWithValue("@bed_id", admission.bed_id);

                // Handle discharge_reason, set to DBNull.Value if null
                cmd.Parameters.AddWithValue("@discharge_reason", string.IsNullOrEmpty(admission.discharge_reason) ? (object)DBNull.Value : admission.discharge_reason);

                cmd.Parameters.AddWithValue("@status", admission.status);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Updatepatient_admission(patient_admission admission)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            UPDATE patient.patient_admission
            SET 
                patient_id = @patient_id,
                admission_date = @admission_date,
                discharge_date = @discharge_date,
                room_id = @room_id,
                bed_id = @bed_id,
                discharge_reason = @discharge_reason,
                status = @status
            WHERE 
                admission_id = @admission_id";

                SqlCommand cmd = new SqlCommand(query, con);


                cmd.Parameters.AddWithValue("@admission_id", admission.admission_id);
                cmd.Parameters.AddWithValue("@patient_id", admission.patient_id);
                cmd.Parameters.AddWithValue("@admission_date", admission.admission_date);
                cmd.Parameters.AddWithValue("@discharge_date", (object?)admission.discharge_date ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@room_id", (object?)admission.room_id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@bed_id", (object?)admission.bed_id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@discharge_reason", (object?)admission.discharge_reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@status", admission.status);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Deletepatient_admission(int admission_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM patient.patient_admission WHERE admission_id = @admission_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@admission_id", admission_id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<patient_records> GetAllpatient_records()
        {
            var recordsList = new List<patient_records>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_records";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    recordsList.Add(new patient_records
                    {
                        RecordId = (int)reader["record_id"],
                        PatientId = (int)reader["patient_id"],
                        Diagnosis = reader["diagnosis"].ToString(),
                        Treatment = reader["treatment"].ToString(),
                        TestResults = reader["test_results"]?.ToString(),
                        MedicationsPrescribed = reader["medications_prescribed"]?.ToString(),
                        VisitDate = (DateTime)reader["visit_date"]
                    });
                }
            }
            return recordsList;
        }

        public patient_records Getpatient_recordsById(int record_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_records WHERE record_id = @record_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@record_id", record_id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new patient_records
                    {
                        RecordId = (int)reader["record_id"],
                        PatientId = (int)reader["patient_id"],
                        Diagnosis = reader["diagnosis"].ToString(),
                        Treatment = reader["treatment"].ToString(),
                        TestResults = reader["test_results"]?.ToString(),
                        MedicationsPrescribed = reader["medications_prescribed"]?.ToString(),
                        VisitDate = (DateTime)reader["visit_date"]
                    };
                }
            }
            return null;
        }

        public void Addpatient_records(patient_records records)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            INSERT INTO patient.patient_records
            (
                patient_id,
                diagnosis,
                treatment,
                test_results,
                medications_prescribed,
                visit_date
            )
            VALUES
            (
                @patient_id,
                @diagnosis,
                @treatment,
                @test_results,
                @medications_prescribed,
                @visit_date
            )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@patient_id", records.PatientId);
                cmd.Parameters.AddWithValue("@diagnosis", records.Diagnosis);
                cmd.Parameters.AddWithValue("@treatment", records.Treatment);
                cmd.Parameters.AddWithValue("@test_results",records.TestResults);
                cmd.Parameters.AddWithValue("@medications_prescribed",records.MedicationsPrescribed);
                cmd.Parameters.AddWithValue("@visit_date", records.VisitDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Updatepatient_records(patient_records records)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            UPDATE patient.patient_records
            SET
                patient_id = @patient_id,
                diagnosis = @diagnosis,
                treatment = @treatment,
                test_results = @test_results,
                medications_prescribed = @medications_prescribed,
                visit_date = @visit_date
            WHERE
                record_id = @record_id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@record_id", records.RecordId);
                cmd.Parameters.AddWithValue("@patient_id", records.PatientId);
                cmd.Parameters.AddWithValue("@diagnosis", records.Diagnosis);
                cmd.Parameters.AddWithValue("@treatment", records.Treatment);
                cmd.Parameters.AddWithValue("@test_results", (object?)records.TestResults ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@medications_prescribed", (object?)records.MedicationsPrescribed ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@visit_date", records.VisitDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Deletepatient_records(int record_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM patient.patient_records WHERE record_id = @record_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@record_id", record_id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<patient_insurance> GetAllpatient_insurance()
        {
            var insuranceList = new List<patient_insurance>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_insurance";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    insuranceList.Add(new patient_insurance
                    {
                        InsuranceId = (int)reader["insurance_id"],
                        PatientId = (int)reader["patient_id"],
                        InsuranceProvider = reader["insurance_provider"].ToString(),
                        PolicyNumber = reader["policy_number"].ToString(),
                        CoverageType = reader["coverage_type"].ToString(),
                        CoverageAmount = (decimal)reader["coverage_amount"],
                        ValidUntil = (DateTime)reader["valid_until"]
                    });
                }
            }
            return insuranceList;
        }

        public patient_insurance Getpatient_insuranceById(int insurance_id)
        {
            patient_insurance insurance = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_insurance WHERE insurance_id = @insurance_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@insurance_id", insurance_id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    insurance = new patient_insurance
                    {
                        InsuranceId = (int)reader["insurance_id"],
                        PatientId = (int)reader["patient_id"],
                        InsuranceProvider = reader["insurance_provider"].ToString(),
                        PolicyNumber = reader["policy_number"].ToString(),
                        CoverageType = reader["coverage_type"].ToString(),
                        CoverageAmount = (decimal)reader["coverage_amount"],
                        ValidUntil = (DateTime)reader["valid_until"]
                    };
                }
            }
            return insurance;
        }

        public void Addpatient_insurance(patient_insurance insurance)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            INSERT INTO patient.patient_insurance
            (
                patient_id,
                insurance_provider,
                policy_number,
                coverage_type,
                coverage_amount,
                valid_from,
                valid_until
            )
            VALUES
            (
                @patient_id,
                @insurance_provider,
                @policy_number,
                @coverage_type,
                @coverage_amount,
                @valid_from,
                @valid_until
            )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@patient_id", insurance.PatientId);
                cmd.Parameters.AddWithValue("@insurance_provider", insurance.InsuranceProvider);
                cmd.Parameters.AddWithValue("@policy_number", insurance.PolicyNumber);
                cmd.Parameters.AddWithValue("@coverage_type", insurance.CoverageType);
                cmd.Parameters.AddWithValue("@coverage_amount", insurance.CoverageAmount);
                cmd.Parameters.AddWithValue("@valid_from", insurance.ValidFrom);
                cmd.Parameters.AddWithValue("@valid_until", insurance.ValidUntil);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Updatepatient_insurance(patient_insurance insurance)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"
            UPDATE patient.patient_insurance
            SET
                patient_id = @patient_id,
                insurance_provider = @insurance_provider,
                policy_number = @policy_number,
                coverage_type = @coverage_type,
                coverage_amount = @coverage_amount,
                valid_from = @valid_from,
                valid_until = @valid_until
            WHERE
                insurance_id = @insurance_id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@insurance_id", insurance.InsuranceId);
                cmd.Parameters.AddWithValue("@patient_id", insurance.PatientId);
                cmd.Parameters.AddWithValue("@insurance_provider", insurance.InsuranceProvider);
                cmd.Parameters.AddWithValue("@policy_number", insurance.PolicyNumber);
                cmd.Parameters.AddWithValue("@coverage_type", insurance.CoverageType);
                cmd.Parameters.AddWithValue("@coverage_amount", insurance.CoverageAmount);
                cmd.Parameters.AddWithValue("@valid_from", insurance.ValidFrom);
                cmd.Parameters.AddWithValue("@valid_until", insurance.ValidUntil);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Deletepatient_insurance(int insurance_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM patient.patient_insurance WHERE insurance_id = @insurance_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@insurance_id", insurance_id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public PatientRegistration Authenticate(string email, string password)
        {
            PatientRegistration patient = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_registration WHERE email = @Email AND password = @Password";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    patient = new PatientRegistration
                    {
                        PatientId = (int)reader["patient_id"],
                        FirstName = reader["first_name"].ToString(),
                        LastName = reader["last_name"].ToString(),
                        Gender = reader["gender"].ToString(),
                        Dob = (DateTime)reader["dob"],
                        Address = reader["address"].ToString(),
                        Email = reader["email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        EmergencyContactName = reader["emergency_contact_name"].ToString(),
                        EmergencyContactPhone = reader["emergency_contact_phone"].ToString(),
                        MaritalStatus = reader["marital_status"].ToString(),
                        Nationality = reader["nationality"].ToString(),
                        BloodGroup = reader["blood_group"].ToString(),
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"],
                        Password = reader["password"].ToString(),
                        patient_img = reader["patient_img"].ToString()
                    };
                }
            }
            return patient;

        }

        public IEnumerable<patient_documents> GetAllpatient_documents()
        {
            var documents = new List<patient_documents>();

            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_documents";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        documents.Add(new patient_documents
                        {
                            DocumentId = Convert.ToInt32(reader["document_id"]),
                            PatientId = Convert.ToInt32(reader["patient_id"]),
                            DocumentType = reader["document_type"].ToString(),
                            DocumentName = reader["document_name"].ToString(),
                            DocumentNumber = reader["document_number"].ToString(),
                            DocumentPath = reader["document_path"].ToString(),
                            UploadDate = Convert.ToDateTime(reader["upload_date"]),
                            Remarks = reader["remarks"].ToString(),
                            UploadedBy = reader["uploaded_by"].ToString()  // <-- Add this line
                        });

                    }
                }
            }

            return documents;
        }

        public patient_documents Getpatient_documentById(int document_id)
        {
            patient_documents document = null;

            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_documents WHERE document_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", document_id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        document = new patient_documents
                        {
                            DocumentId = Convert.ToInt32(reader["document_id"]),
                            PatientId = Convert.ToInt32(reader["patient_id"]),
                            DocumentType = reader["document_type"].ToString(),
                            DocumentName = reader["document_name"].ToString(),
                            DocumentNumber = reader["document_number"].ToString(),
                            DocumentPath = reader["document_path"].ToString(),
                            UploadDate = Convert.ToDateTime(reader["upload_date"]),
                            Remarks = reader["remarks"].ToString()
                        };
                    }
                }
            }

            return document;
        }

        // Method to add a new patient document
        public void Addpatient_document(patient_documents document, IFormFile patient_img, int uploadedBy)
        {
            string filePath = null;

            if (patient_img != null && patient_img.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(patient_img.FileName);
                string savePath = Path.Combine(_imageFilePath, fileName);

                if (!Directory.Exists(_imageFilePath))
                    Directory.CreateDirectory(_imageFilePath);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    patient_img.CopyTo(stream);
                }

                filePath = Path.Combine("uploads", fileName);
            }

            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                string query = @"INSERT INTO patient.patient_documents 
                         (patient_id, document_type, document_name, document_number, document_path, uploaded_by, upload_date, remarks)
                         VALUES 
                         (@patient_id, @document_type, @document_name, @document_number, @document_path, @uploaded_by, @upload_date, @remarks)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patient_id", document.PatientId);
                cmd.Parameters.AddWithValue("@document_type", document.DocumentType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@document_name", document.DocumentName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@document_number", document.DocumentNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@document_path", string.IsNullOrEmpty(filePath) ? (object)DBNull.Value : filePath);
                cmd.Parameters.AddWithValue("@uploaded_by", uploadedBy);
                cmd.Parameters.AddWithValue("@upload_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@remarks", document.Remarks ?? (object)DBNull.Value);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    throw new Exception("Document insert failed.");
                }
            }
        }


        public void Updatepatient_document(patient_documents document, IFormFile patient_img, int uploadedBy)
        {
            string filePath = document.DocumentPath; // Keep existing path

            // If a new image is uploaded, generate the new file path
            if (patient_img != null && patient_img.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(patient_img.FileName);
                string savePath = Path.Combine(_imageFilePath, fileName);

                if (!Directory.Exists(_imageFilePath))
                {
                    Directory.CreateDirectory(_imageFilePath);
                }

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    patient_img.CopyTo(stream);
                }

                // New file path if an image is uploaded
                filePath = Path.Combine("uploads", fileName);
            }

            // Check if filePath is null or empty before executing SQL
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("Document path is required but was not supplied.");
            }

            // Now proceed with the SQL update
            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                string query = @"UPDATE patient.patient_documents SET 
                            document_type = @document_type,
                            document_name = @document_name,
                            document_number = @document_number,
                            document_path = @document_path,
                            uploaded_by = @uploaded_by,
                            remarks = @remarks
                        WHERE document_id = @document_id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@document_type", document.DocumentType);
                cmd.Parameters.AddWithValue("@document_name", document.DocumentName);
                cmd.Parameters.AddWithValue("@document_number", document.DocumentNumber);
                cmd.Parameters.AddWithValue("@document_path", filePath); // Ensure filePath is supplied
                cmd.Parameters.AddWithValue("@uploaded_by", uploadedBy);
                cmd.Parameters.AddWithValue("@remarks", document.Remarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@document_id", document.DocumentId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void Deletepatient_document(int document_id)
        {
            string filePath = "";

            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                // Get the file path first
                string getQuery = "SELECT document_path FROM patient.patient_documents WHERE document_id = @id";
                SqlCommand getCmd = new SqlCommand(getQuery, conn);
                getCmd.Parameters.AddWithValue("@id", document_id);

                conn.Open();
                var result = getCmd.ExecuteScalar();
                if (result != null)
                {
                    filePath = result.ToString();
                    string fullPath = Path.Combine(_imageFilePath, Path.GetFileName(filePath));
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }

                // Then delete the record
                string deleteQuery = "DELETE FROM patient.patient_documents WHERE document_id = @id";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@id", document_id);
                deleteCmd.ExecuteNonQuery();
            }
        }
        public List<SelectListItem> GetRoomTypes()
        {
            var list = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RoomID, RoomType FROM bed_managment.Rooms", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["RoomID"].ToString(),
                        Text = reader["RoomType"].ToString()
                    });
                }
            }
            return list;
        }

        public List<SelectListItem> GetBedTypes()
        {
            var list = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT BedID, BedType FROM bed_managment.Beds", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["BedID"].ToString(),
                        Text = reader["BedType"].ToString()
                    });
                }
            }
            return list;
        }

        public List<SelectListItem> GetPatientName()
        {
            List<SelectListItem> patientList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SELECT patient_id, first_name FROM patient.patient_registration", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    patientList.Add(new SelectListItem
                    {
                        Value = reader["patient_id"].ToString(),
                        Text = reader["first_name"].ToString()
                    });
                }

                con.Close();
            }

            return patientList;
        }
        public string GetPatientNameById(int patientId)
        {
            string patientName = "Unknown"; // default in case of null

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT first_name FROM patient.patient_registration WHERE patient_id = @patientId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@patientId", patientId);

                con.Open();
                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    patientName = result.ToString();
                }
            }

            return patientName;
        }

        public IEnumerable<patient_visits> GetAllpatient_visits()
        {
            var visits = new List<patient_visits>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT visit_id, patient_id, visit_date, department, doctor_id, visit_reason " +
                               "FROM patient.patient_visits";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        visits.Add(new patient_visits
                        {
                            VisitId = reader.GetInt32(reader.GetOrdinal("visit_id")),
                            PatientId = reader.GetInt32(reader.GetOrdinal("patient_id")),
                            VisitDate = reader.GetDateTime(reader.GetOrdinal("visit_date")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            DoctorId = reader.GetInt32(reader.GetOrdinal("doctor_id")),
                            VisitReason = reader.GetString(reader.GetOrdinal("visit_reason"))
                        });
                    }
                }
            }

            return visits;
        }

        public patient_visits Getpatient_visitsById(int visit_id)
        {
            patient_visits visit = null;

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT visit_id, patient_id, visit_date, department, doctor_id, visit_reason " +
                               "FROM patient.patient_visits WHERE visit_id = @visitId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@visitId", visit_id);

                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        visit = new patient_visits
                        {
                            VisitId = reader.GetInt32(reader.GetOrdinal("visit_id")),
                            PatientId = reader.GetInt32(reader.GetOrdinal("patient_id")),
                            VisitDate = reader.GetDateTime(reader.GetOrdinal("visit_date")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            DoctorId = reader.GetInt32(reader.GetOrdinal("doctor_id")),
                            VisitReason = reader.GetString(reader.GetOrdinal("visit_reason"))
                        };
                    }
                }
            }

            return visit;
        }

        public void Addpatient_visits(patient_visits visit)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "INSERT INTO patient.patient_visits (patient_id, visit_date, department, doctor_id, visit_reason) " +
                               "VALUES (@patientId, @visitDate, @department, @doctorId, @visitReason)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@patientId", visit.PatientId);
                cmd.Parameters.AddWithValue("@visitDate", visit.VisitDate);
                cmd.Parameters.AddWithValue("@department", visit.Department);
                cmd.Parameters.AddWithValue("@doctorId", visit.DoctorId);
                cmd.Parameters.AddWithValue("@visitReason", visit.VisitReason);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Updatepatient_visits(patient_visits visit)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "UPDATE patient.patient_visits SET " +
                               "patient_id = @patientId, " +
                               "visit_date = @visitDate, " +
                               "department = @department, " +
                               "doctor_id = @doctorId, " +
                               "visit_reason = @visitReason " +
                               "WHERE visit_id = @visitId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@visitId", visit.VisitId);
                cmd.Parameters.AddWithValue("@patientId", visit.PatientId);
                cmd.Parameters.AddWithValue("@visitDate", visit.VisitDate);
                cmd.Parameters.AddWithValue("@department", visit.Department);
                cmd.Parameters.AddWithValue("@doctorId", visit.DoctorId);
                cmd.Parameters.AddWithValue("@visitReason", visit.VisitReason);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Deletepatient_visits(int visit_id)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "DELETE FROM patient.patient_visits WHERE visit_id = @visitId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@visitId", visit_id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public PatientRegistration GetPatientByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_registration WHERE email = @Email";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Assuming PatientRegistration has properties like Id, Email, Password, etc.
                    PatientRegistration patient = new PatientRegistration
                    {
                        PatientId = Convert.ToInt32(reader["patient_id"]),
                        Email = reader["email"].ToString(),
                        Password = reader["password"].ToString()  // Assuming the password is stored as plain text (which is insecure)
                    };
                    return patient;
                }
                else
                {
                    return null; // Return null if no matching patient is found
                }
            }
        }

        public void UpdatePassword(int patientId, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "UPDATE patient.patient_registration SET password = @NewPassword WHERE patient_id = @PatientId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NewPassword", newPassword);  // Ideally hash the password before saving
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<patient_documents> GetDocumentsByPatientId(int patientId)
        {
            List<patient_documents> documents = new List<patient_documents>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM patient.patient_documents WHERE patient_id = @PatientId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var document = new patient_documents
                        {
                            DocumentId = Convert.ToInt32(reader["document_id"]),
                            PatientId = Convert.ToInt32(reader["patient_id"]),
                            DocumentType = reader["document_type"].ToString(),
                            DocumentName = reader["document_name"].ToString(),
                            DocumentNumber = reader["document_number"].ToString(),
                            DocumentPath = reader["document_path"].ToString(),
                            UploadedBy = reader["uploaded_by"].ToString(),
                            UploadDate = Convert.ToDateTime(reader["upload_date"]),
                            Remarks = reader["remarks"].ToString()
                        };

                        documents.Add(document);
                    }
                }
            }

            return documents;
        }


    }


}

