using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly string _imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly string _connectionstring;

        public DoctorRepository(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("DefaultConnection");
        }

        // CRUD for Doctor
        public IEnumerable<Doctor> GetAllDoctors()
        {
            var doctorList = new List<Doctor>();
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT d.*, dep.department_name\r\nFROM Doctors.doctors d\r\nJOIN EmployeeManagement.Departments dep ON d.department_id = dep.department_id\r\n";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doctorList.Add(new Doctor
                    {
                        DoctorId = (int)reader["doctor_id"],
                        FullName = reader["full_name"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        Gender = reader["gender"].ToString(),
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        Qualification = reader["Qualification"].ToString(),
                        Experience = (int)reader["Experience"],
                        Status = reader["Status"].ToString(),
                        Password = reader["Password"].ToString(),

                        // Manually populate navigation property
                        Department = new Department
                        {
                            DepartmentId = Convert.ToInt32(reader["department_id"]),
                            DepartmentName = reader["department_name"].ToString()
                        },
                        Img = reader["profile_photo"].ToString()

                    });
                }
            }
            return doctorList;
        }

        public Doctor GetDoctorById(int doctorId)
        {
            Doctor doctor = null;
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM Doctors.doctors WHERE doctor_id = @doctor_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@doctor_id", doctorId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doctor = new Doctor
                    {
                        DoctorId = (int)reader["doctor_id"],
                        FullName = reader["full_name"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        Gender = reader["gender"].ToString(),
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        Qualification = reader["Qualification"].ToString(),
                        Experience = (int)reader["Experience"],
                        Status = reader["Status"].ToString(),
                        Password = reader["Password"].ToString(),
                        DepartmentId = (int)reader["department_id"],
                        Img = reader["profile_photo"].ToString()
                    };
                }
            }
            return doctor;
        }

        public void AddDoctor(Doctor doctor, IFormFile doctor_img)
        {
            string photoPath = SavePhoto(doctor_img);

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"INSERT INTO Doctors.doctors
                        (full_name, email, phone_number, gender, date_of_birth, qualification, experience, status, password, department_id, profile_photo)
                        VALUES
                        (@FullName, @Email, @PhoneNumber, @Gender, @DateOfBirth, @Qualification, @Experience, @Status, @Password, @DepartmentId, @Img)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@FullName", doctor.FullName);
                cmd.Parameters.AddWithValue("@Email", doctor.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", doctor.PhoneNumber);
                cmd.Parameters.AddWithValue("@Gender", doctor.Gender);
                cmd.Parameters.AddWithValue("@DateOfBirth", doctor.DateOfBirth);
                cmd.Parameters.AddWithValue("@Qualification", doctor.Qualification);
                cmd.Parameters.AddWithValue("@Experience", doctor.Experience);
                cmd.Parameters.AddWithValue("@Status", doctor.Status);
                cmd.Parameters.AddWithValue("@Password", doctor.Password);
                cmd.Parameters.AddWithValue("@DepartmentId", doctor.DepartmentId);
                cmd.Parameters.AddWithValue("@Img", string.IsNullOrEmpty(photoPath) ? (object)DBNull.Value : photoPath);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDoctor(Doctor doctor, IFormFile doctor_img)
        {
            string newPhotoPath = doctor.Img; // Use existing image if no new image uploaded

            if (doctor_img != null && doctor_img.Length > 0)
            {
                newPhotoPath = SavePhoto(doctor_img);

                if (string.IsNullOrEmpty(newPhotoPath))
                {
                    throw new Exception("Error saving photo."); // Throw an exception if photo saving fails
                }
            }

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = @"UPDATE Doctors.doctors
                         SET full_name = @FullName,
                             email = @Email,
                             phone_number = @PhoneNumber,
                             gender = @Gender,
                             date_of_birth = @DateOfBirth,
                             qualification = @Qualification,
                             experience = @Experience,
                             status = @Status,
                             password = @Password,
                             department_id = @DepartmentId,
                             profile_photo = @Img
                         WHERE doctor_id = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@DoctorId", doctor.DoctorId);
                cmd.Parameters.AddWithValue("@FullName", doctor.FullName);
                cmd.Parameters.AddWithValue("@Email", doctor.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", doctor.PhoneNumber);
                cmd.Parameters.AddWithValue("@Gender", doctor.Gender);
                cmd.Parameters.AddWithValue("@DateOfBirth", doctor.DateOfBirth);
                cmd.Parameters.AddWithValue("@Qualification", doctor.Qualification);
                cmd.Parameters.AddWithValue("@Experience", doctor.Experience);
                cmd.Parameters.AddWithValue("@Status", doctor.Status);
                cmd.Parameters.AddWithValue("@Password", doctor.Password);
                cmd.Parameters.AddWithValue("@DepartmentId", doctor.DepartmentId);
                cmd.Parameters.AddWithValue("@Img", string.IsNullOrEmpty(newPhotoPath) ? (object)DBNull.Value : newPhotoPath);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void DeleteDoctor(int doctorId)
        {
            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                con.Open();

                // First delete patient admissions related to the doctor
                string deleteAdmissions = "DELETE FROM patient.patient_admission WHERE doctor_id = @doctor_id";
                SqlCommand cmd1 = new SqlCommand(deleteAdmissions, con);
                cmd1.Parameters.AddWithValue("@doctor_id", doctorId);
                cmd1.ExecuteNonQuery();

                // Then delete the doctor
                string deleteDoctor = "DELETE FROM Doctors.doctors WHERE doctor_id = @doctor_id";
                SqlCommand cmd2 = new SqlCommand(deleteDoctor, con);
                cmd2.Parameters.AddWithValue("@doctor_id", doctorId);
                cmd2.ExecuteNonQuery();
            }
        }

        private string SavePhoto(IFormFile doctor_img)
        {
            if (doctor_img == null || doctor_img.Length == 0)
            {
                Console.WriteLine("No file uploaded.");
                return null; // No file uploaded, return null
            }

            if (!Directory.Exists(_imageFilePath))
            {
                Directory.CreateDirectory(_imageFilePath); // Ensure the upload directory exists
                Console.WriteLine("Uploads folder created at: " + _imageFilePath);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(doctor_img.FileName);
            string filePath = Path.Combine(_imageFilePath, fileName);
            Console.WriteLine("Saving file to: " + filePath);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    doctor_img.CopyTo(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while saving file: " + e.Message);
                return null; // Return null if an error occurs
            }

            return "/uploads/" + fileName; // Return the correct file path
        }


        public void DeleteDoctorSpecialization(int specialization_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorSchedule> GetDoctorSchedule(int doctor_id)
        {
            throw new NotImplementedException();
        }

        public void AddDoctorSchedule(DoctorSchedule schedule)
        {
            throw new NotImplementedException();
        }

        public void UpdateDoctorSchedule(DoctorSchedule schedule)
        {
            throw new NotImplementedException();
        }

        public void DeleteDoctorSchedule(int schedule_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorAppointment> GetAppointmentsByDoctorId(int doctor_id)
        {
            throw new NotImplementedException();
        }

        public void AddDoctorAppointment(DoctorAppointment appointment)
        {
            throw new NotImplementedException();
        }

        public void UpdateDoctorAppointment(DoctorAppointment appointment)
        {
            throw new NotImplementedException();
        }

        public void DeleteDoctorAppointment(int appointment_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorPrescription> GetPrescriptionsByAppointmentId(int appointment_id)
        {
            throw new NotImplementedException();
        }

        public void AddDoctorPrescription(DoctorPrescription prescription)
        {
            throw new NotImplementedException();
        }

        public void UpdateDoctorPrescription(DoctorPrescription prescription)
        {
            throw new NotImplementedException();
        }

        public void DeleteDoctorPrescription(int prescription_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorNote> GetNotesByAppointmentId(int appointment_id)
        {
            throw new NotImplementedException();
        }

        public void AddDoctorNote(DoctorNote note)
        {
            throw new NotImplementedException();
        }

        public void UpdateDoctorNote(DoctorNote note)
        {
            throw new NotImplementedException();
        }

        public void DeleteDoctorNote(int note_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorReview> GetReviewsByDoctorId(int doctor_id)
        {
            throw new NotImplementedException();
        }

        public void AddDoctorReview(DoctorReview review)
        {
            throw new NotImplementedException();
        }

        public void UpdateDoctorReview(DoctorReview review)
        {
            throw new NotImplementedException();
        }

        public void DeleteDoctorReview(int review_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorPayment> GetPaymentsByDoctorId(int doctor_id)
        {
            throw new NotImplementedException();
        }

        public void AddDoctorPayment(DoctorPayment payment)
        {
            throw new NotImplementedException();
        }

        public void UpdateDoctorPayment(DoctorPayment payment)
        {
            throw new NotImplementedException();
        }

        public void DeleteDoctorPayment(int payment_id)
        {
            throw new NotImplementedException();
        }

      
        public List<SelectListItem> GetDoctorName()
        {
            List<SelectListItem> doctorList = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SELECT doctor_id, full_name FROM Doctors.doctors", con);
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

                con.Close();
            }

            return doctorList;
        }

        public string GetDoctorNameById(int doctorId)
        {
            string doctorName = string.Empty;

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SELECT full_name FROM Doctors.doctors WHERE doctor_id = @DoctorId", con);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    doctorName = result.ToString();
                }
                con.Close();
            }

            return doctorName;
        }


        public IEnumerable<Department> GetDepartment()
        {
            List<Department> departments = new List<Department>();

            using (SqlConnection con = new SqlConnection(_connectionstring))
            {
                string query = "SELECT * FROM EmployeeManagement.Departments";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Use while to read all rows from the data reader
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            DepartmentId = Convert.ToInt32(reader["department_id"]),
                            DepartmentName = reader["department_name"].ToString()
                        });
                    }
                }
            }

            return departments;
        }

    }

}
