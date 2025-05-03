using System;
using System.Collections.Generic;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using HospitalManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HospitalManagementSystem.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private string _connectionString;
        public StaffRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public List<PatientRegistration> GetAssignedPatients(int nurseId)
        {
            var patients = new List<PatientRegistration>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT *
            FROM EmployeeManagement.NursePatientAssignment npa
            INNER JOIN patient.patient_registration pr ON npa.patient_id = pr.patient_id
            WHERE npa.employee_id = @NurseId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NurseId", nurseId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var patient = new PatientRegistration
                            {
                                PatientId = Convert.ToInt32(reader["patient_id"]),
                                FirstName = reader["first_name"].ToString(),
                                LastName = reader["last_name"].ToString(),
                                Gender = reader["gender"].ToString(),
                                Dob = Convert.ToDateTime(reader["dob"]),
                                Address = reader["address"].ToString(),
                                Email = reader["email"].ToString(),
                                PhoneNumber = reader["phone_number"].ToString(),
                                EmergencyContactName = reader["emergency_contact_name"].ToString(),
                                EmergencyContactPhone = reader["emergency_contact_phone"].ToString(),
                                MaritalStatus = reader["marital_status"].ToString(),
                                Nationality = reader["nationality"].ToString(),
                                BloodGroup = reader["blood_group"]?.ToString(),
                                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                UpdatedAt = Convert.ToDateTime(reader["updated_at"]),
                                Password = reader["password"].ToString(),
                                patient_img = reader["patient_img"]?.ToString()
                            };

                            patients.Add(patient);
                        }
                    }
                }
            }

            return patients;
        }




        public void Add(Employee employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        INSERT INTO EmployeeManagement.Employees 
        (first_name, last_name, date_of_birth, gender, phone_number, email, address, hire_date, role_id, employee_status, login_id, password)
        VALUES 
        (@FirstName, @LastName, @DateOfBirth, @Gender, @PhoneNumber, @Email, @Address, @HireDate, @RoleId, @EmployeeStatus, @LoginId, @Password)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)employee.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@Address", (object)employee.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                cmd.Parameters.AddWithValue("@RoleId", (object)employee.RoleId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeStatus", employee.EmployeeStatus);
                cmd.Parameters.AddWithValue("@LoginId", employee.LoginId);
                cmd.Parameters.AddWithValue("@Password", employee.Password);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddDepartment(Department department)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO EmployeeManagement.Departments (department_name) VALUES (@name)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", department.DepartmentName);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void AddRole(Role role)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO EmployeeManagement.Roles (role_name, role_description) VALUES (@name, @desc)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", role.RoleName);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(role.RoleDescription) ? DBNull.Value : (object)role.RoleDescription);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM EmployeeManagement.Employees WHERE employee_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteDepartment(int departmentId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM EmployeeManagement.Departments WHERE department_id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", departmentId);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void DeleteRole(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM EmployeeManagement.Roles WHERE role_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public IEnumerable<Employee> GetAll()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ee.*,er.role_name
                               FROM EmployeeManagement.Employees ee
                               inner join EmployeeManagement.Roles er
                              on ee.role_id = er.role_id
                                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(MapEmployee(reader));
                }
            }

            return employees;
        }

        public List<Department> GetAllDepartments()
        {
            var departments = new List<Department>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT department_id, department_name FROM EmployeeManagement.Departments";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new Department
                    {
                        DepartmentId = (int)reader["department_id"],
                        DepartmentName = reader["department_name"].ToString()
                    });
                }
                con.Close();
            }

            return departments;
        }

        public IEnumerable<Role> GetAllRoles()
        {
            var roles = new List<Role>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT role_id, role_name, role_description FROM EmployeeManagement.Roles";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(new Role
                    {
                        RoleId = (int)reader["role_id"],
                        RoleName = reader["role_name"].ToString(),
                        RoleDescription = reader["role_description"]?.ToString()
                    });
                }
                conn.Close();
            }

            return roles;
        }

        public Employee GetById(int id)
        {
            Employee employee = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM EmployeeManagement.Employees WHERE employee_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    employee = new Employee
                    {
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        FirstName = reader["first_name"]?.ToString(),
                        LastName = reader["last_name"]?.ToString(),
                        DateOfBirth = reader["date_of_birth"] != DBNull.Value ? Convert.ToDateTime(reader["date_of_birth"]) : DateTime.MinValue,
                        Gender = reader["gender"]?.ToString(),
                        PhoneNumber = reader["phone_number"] != DBNull.Value ? reader["phone_number"].ToString() : string.Empty, // Default to empty string
                        Email = reader["email"]?.ToString() ?? string.Empty,  // Default to empty string
                        Address = reader["address"] != DBNull.Value ? reader["address"].ToString() : string.Empty, // Default to empty string
                        HireDate = reader["hire_date"] != DBNull.Value ? Convert.ToDateTime(reader["hire_date"]) : DateTime.MinValue,

                        RoleId = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0,
                        EmployeeStatus = reader["employee_status"]?.ToString() ?? string.Empty, // Default to empty string

                        // Adding new fields LoginId and Password
                        LoginId = reader["login_id"]?.ToString() ?? string.Empty, // Default to empty string
                        Password = reader["password"]?.ToString() ?? string.Empty // Default to empty string
                    };
                }
            }
            return employee;
        }



        public Department GetDepartmentById(int departmentId)
        {
            Department department = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT department_id, department_name FROM EmployeeManagement.Departments WHERE department_id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", departmentId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    department = new Department
                    {
                        DepartmentId = (int)reader["department_id"],
                        DepartmentName = reader["department_name"].ToString()
                    };
                }
                con.Close();
            }

            return department;
        }

        public Role GetRoleById(int id)
        {
            Role role = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT role_id, role_name, role_description FROM EmployeeManagement.Roles WHERE role_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    role = new Role
                    {
                        RoleId = (int)reader["role_id"],
                        RoleName = reader["role_name"].ToString(),
                        RoleDescription = reader["role_description"]?.ToString()
                    };
                }
                conn.Close();
            }

            return role;
        }

        public void Update(Employee employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        UPDATE EmployeeManagement.Employees SET
            first_name = @FirstName,
            last_name = @LastName,
            phone_number = @PhoneNumber,
            email = @Email,
            address = @Address,
            employee_status = @EmployeeStatus
        WHERE employee_id = @EmployeeId";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)employee.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@Address", (object)employee.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeStatus", employee.EmployeeStatus);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDepartment(Department department)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE EmployeeManagement.Departments SET department_name = @name WHERE department_id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", department.DepartmentName);
                cmd.Parameters.AddWithValue("@id", department.DepartmentId);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void UpdateRole(Role role)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE EmployeeManagement.Roles SET role_name = @name, role_description = @desc WHERE role_id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", role.RoleName);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(role.RoleDescription) ? DBNull.Value : (object)role.RoleDescription);
                cmd.Parameters.AddWithValue("@id", role.RoleId);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        private Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = Convert.ToInt32(reader["employee_id"]),
                FirstName = reader["first_name"]?.ToString(),
                LastName = reader["last_name"]?.ToString(),
                DateOfBirth = reader["date_of_birth"] != DBNull.Value ? Convert.ToDateTime(reader["date_of_birth"]) : DateTime.MinValue,
                Gender = reader["gender"]?.ToString(),
                PhoneNumber = reader["phone_number"] != DBNull.Value ? reader["phone_number"].ToString() : string.Empty, // Default to empty string
                Email = reader["email"]?.ToString() ?? string.Empty,  // Default to empty string
                Address = reader["address"] != DBNull.Value ? reader["address"].ToString() : string.Empty, // Default to empty string
                HireDate = reader["hire_date"] != DBNull.Value ? Convert.ToDateTime(reader["hire_date"]) : DateTime.MinValue,

                RoleId = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0,
                EmployeeStatus = reader["employee_status"]?.ToString() ?? string.Empty, // Default to empty string

                LoginId = reader["login_id"]?.ToString() ?? string.Empty, // Default to empty string
                Password = reader["password"]?.ToString() ?? string.Empty, // Default to empty string
                                                                           // New properties from JOINs
                RoleName = reader["role_name"]?.ToString() ?? string.Empty,


            };
        }


        public Employee Login(string loginId, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM EmployeeManagement.Employees ee
                        INNER JOIN EmployeeManagement.Roles er
                        ON ee.role_id = er.role_id
                        WHERE ee.login_id = @LoginID AND ee.password = @Password";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@LoginID", loginId);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Employee
                        {
                            EmployeeId = Convert.ToInt32(reader["employee_id"]),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                            Gender = reader["gender"].ToString(),
                            PhoneNumber = reader["phone_number"]?.ToString(),
                            Email = reader["email"]?.ToString(),
                            Address = reader["address"]?.ToString(),
                            HireDate = Convert.ToDateTime(reader["hire_date"]),
                            RoleId = Convert.ToInt32(reader["role_id"]),
                            RoleName = reader["role_name"].ToString(),  // From Roles table
                            EmployeeStatus = reader["employee_status"].ToString(),
                            LoginId = reader["login_id"].ToString(),
                            Password = reader["password"].ToString()
                        };

                    }
                }
                catch (SqlException ex)
                {
                    // Log SQL-specific errors
                    Console.WriteLine($"SQL Error during login: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    // Log general errors
                    Console.WriteLine($"General Error during login: {ex.Message}");
                    return null;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return null; // Authentication failed
        }

        public void Updateprofile(Employee model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE EmployeeManagement.Employees SET " +
                                             "email = @Email, phone_number = @PhoneNumber, address = @Address " +
                                             "WHERE employee_id = @EmployeeId", connection);

                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                command.Parameters.AddWithValue("@Address", model.Address);
                command.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);

                // Execute the command
                command.ExecuteNonQuery();
            }
        }


        public bool ChangePassword(int employeeId, string currentPassword, string newPassword)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Check if current password matches
                string checkQuery = @"SELECT COUNT(*) FROM EmployeeManagement.Employees 
                              WHERE employee_id = @EmployeeId AND password = @CurrentPassword";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                checkCmd.Parameters.AddWithValue("@CurrentPassword", currentPassword);

                conn.Open();
                int count = (int)checkCmd.ExecuteScalar();

                if (count == 1)
                {
                    // If match, update to new password
                    string updateQuery = @"UPDATE EmployeeManagement.Employees 
                                   SET password = @NewPassword 
                                   WHERE employee_id = @EmployeeId";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    updateCmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    return updateCmd.ExecuteNonQuery() > 0;
                }

                return false; // Password mismatch
            }
        }

        public Employee GetByLoginIdOrEmail(string input)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT e.*, r.role_name
            FROM EmployeeManagement.Employees e
            INNER JOIN EmployeeManagement.Roles r ON e.role_id = r.role_id
            WHERE e.login_id = @input OR e.email = @input";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@input", input);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Employee
                    {
                        EmployeeId = Convert.ToInt32(dr["employee_id"]),
                        LoginId = dr["login_id"].ToString(),
                        Email = dr["email"].ToString(),
                        Password = dr["password"].ToString(),
                        RoleName = dr["role_name"].ToString()
                    };
                }
            }
            return null;
        }

        public bool UpdatePassword(string loginId, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE EmployeeManagement.Employees SET Password = @password WHERE login_id = @loginId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", newPassword);
                cmd.Parameters.AddWithValue("@loginId", loginId);
                con.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }


        public List<PatientStatusReport> GetPatientStatusReport(int nurseId)
        {
            var reportList = new List<PatientStatusReport>();
            string query = @"
        SELECT 
            pr.patient_id, 
            pr.first_name, 
            pr.last_name, 
            pr.gender, 
            pr.phone_number, 
            pr.blood_group, 
            ps.Condition, 
            ps.Status, 
            ps.CheckupDone
        FROM 
            EmployeeManagement.NursePatientAssignment npa
        INNER JOIN 
            patient.patient_registration pr ON npa.patient_id = pr.patient_id
        INNER JOIN 
            patient.PatientStatus ps ON pr.patient_id = ps.PatientId
        WHERE 
            npa.employee_id = @NurseId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NurseId", nurseId);  

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var report = new PatientStatusReport
                    {
                        PatientId = Convert.ToInt32(reader["patient_id"]),
                        Name = reader["first_name"].ToString() + " " + reader["last_name"].ToString(),
                        Gender = reader["gender"].ToString(),
                        Phone = reader["phone_number"].ToString(),
                        BloodGroup = reader["blood_group"].ToString(),
                        Condition = reader["Condition"].ToString(),
                        Status = reader["Status"].ToString(),
                        CheckupDone = Convert.ToBoolean(reader["CheckupDone"])
                    };

                    reportList.Add(report);
                }

                reader.Close();
            }

            return reportList;
        }

        public void SavePatientVitals(PatientVitals vitals)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO patient.PatientVitals 
                         (PatientId, BloodPressure, HeartRate, Temperature, RecordedAt)
                         VALUES (@PatientId, @BloodPressure, @HeartRate, @Temperature, @RecordedAt)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", vitals.PatientId);
                cmd.Parameters.AddWithValue("@BloodPressure", vitals.BloodPressure);
                cmd.Parameters.AddWithValue("@HeartRate", vitals.HeartRate);
                cmd.Parameters.AddWithValue("@Temperature", vitals.Temperature);
                cmd.Parameters.AddWithValue("@RecordedAt", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<PatientVitalsReport> GetVitalsReport()
        {
            List<PatientVitalsReport> report = new List<PatientVitalsReport>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                                p.first_name AS PatientName,
                                v.Temperature,
                                v.BloodPressure,
                                v.HeartRate,
                                v.RecordedAt
                             FROM 
                                patient.PatientVitals v
                             JOIN 
                                patient.patient_registration p ON v.PatientId = p.patient_id
                             ORDER BY 
                                v.RecordedAt DESC;";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        report.Add(new PatientVitalsReport
                        {
                            PatientName = reader["PatientName"].ToString(),
                            Temperature = Convert.ToDecimal(reader["Temperature"]),
                            BloodPressure = reader["BloodPressure"].ToString(),
                            HeartRate = Convert.ToInt32(reader["HeartRate"]),
                            RecordedAt = Convert.ToDateTime(reader["RecordedAt"])
                        });
                    }
                }
            }

            return report;
        }
       

        public List<DischargedPatientViewModel> GetDischargedPatientsByNurse(int nurseId)
            {
                var dischargedPatients = new List<DischargedPatientViewModel>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                   SELECT 
    pa.admission_id AS AdmissionId,
    pr.first_name + ' ' + pr.last_name AS PatientName,
    d.full_name AS DoctorName,
    dg.diagnosis_text AS Diagnosis,
    pa.discharge_date AS DischargeDate,
    pa.discharge_reason AS DischargeReason,   -- ✅ Add this line
    pr.patient_id AS PatientId
FROM EmployeeManagement.NursePatientAssignment npa
JOIN patient.patient_registration pr ON npa.patient_id = pr.patient_id
JOIN patient.patient_admission pa ON pr.patient_id = pa.patient_id
JOIN patient.Diagnosis dg ON pr.patient_id = dg.patient_id
JOIN Doctors.doctors d ON dg.doctor_id = d.doctor_id
WHERE npa.employee_id = @employeeId
"; 


                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@employeeId", nurseId);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var patient = new DischargedPatientViewModel
                                {
                                    AdmissionId =Convert.ToInt32( reader["AdmissionId"].ToString()),
                                    PatientName = reader["PatientName"].ToString(),
                                    DoctorName = reader["DoctorName"].ToString(),
                                    Diagnosis = reader["Diagnosis"].ToString(),
                                    DischargeDate = reader["DischargeDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DischargeDate"]),
                                    DischargeReason = reader["DischargeReason"].ToString(),
                                     // Ensure you're fetching PatientId as well
                                    PatientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientId"))
                                };
                                dischargedPatients.Add(patient);
                            }
                        }
                    }
                }

                return dischargedPatients;
            }


        public void UpdateDischargeDetails(int admissionId, DateTime dischargeDate, string dischargeReason, int? doctorId)
         {
            using (  var connection = new SqlConnection(_connectionString))
                                                 {
                string query = "UPDATE patient.patient_admission " +
                               "SET discharge_date = @DischargeDate, discharge_reason = @DischargeReason, status = 'Discharged', doctor_id = @DoctorId " +
                               "WHERE admission_id = @AdmissionId";

                using (var command = new SqlCommand(query, connection))
                 {
                               command.Parameters.AddWithValue("@AdmissionId", admissionId);
                    command.Parameters.AddWithValue("@DischargeDate", dischargeDate);
                    command.Parameters.AddWithValue("@DischargeReason", dischargeReason);
                    command.Parameters.AddWithValue("@DoctorId", doctorId ?? (object)DBNull.Value);  // Use DBNull if doctorId is null

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<Doctor> GetAllDoctors()
        {
            var doctors = new List<Doctor>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT doctor_id, full_name FROM Doctors.doctors WHERE status = 'Active'";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctors.Add(new Doctor
                            {
                                DoctorId = (int)reader["doctor_id"],
                                FullName = reader["full_name"].ToString()
                            });
                        }
                    }
                }
            }

            return doctors;
        }

        public List<HiringManagement> GetAllJobs()
        {
            List<HiringManagement> jobs = new List<HiringManagement>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Hiring.HiringManagement";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jobs.Add(new HiringManagement
                    {
                        JobId = (int)reader["JobId"],
                        Position = reader["Position"].ToString(),
                        Department = reader["Department"].ToString(),
                        Vacancies = (int)reader["Vacancies"],
                        PostedDate = (DateTime)reader["PostedDate"],
                        Status = reader["Status"].ToString(),
                        Description = reader["Description"].ToString()
                    });
                }
            }
            return jobs;
        }

        public List<HiringManagement> GetAllOpenJobs()
        {
            List<HiringManagement> jobs = new List<HiringManagement>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Hiring.HiringManagement where Status='Open'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jobs.Add(new HiringManagement
                    {
                        JobId = (int)reader["JobId"],
                        Position = reader["Position"].ToString(),
                        Department = reader["Department"].ToString(),
                        Vacancies = (int)reader["Vacancies"],
                        PostedDate = (DateTime)reader["PostedDate"],
                        Status = reader["Status"].ToString(),
                        Description = reader["Description"].ToString()
                    });
                }
            }
            return jobs;
        }

        public void AddJob(HiringManagement job)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Hiring.HiringManagement (Position, Department, Vacancies, PostedDate, Status)
                             VALUES (@Position, @Department, @Vacancies, @PostedDate, @Status)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Position", job.Position);
                cmd.Parameters.AddWithValue("@Department", job.Department);
                cmd.Parameters.AddWithValue("@Vacancies", job.Vacancies);
                cmd.Parameters.AddWithValue("@PostedDate", job.PostedDate);
                cmd.Parameters.AddWithValue("@Status", job.Status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public HiringManagement GetJobById(int jobId)
        {
            HiringManagement job = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM HiringManagement WHERE JobId = @JobId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@JobId", jobId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    job = new HiringManagement
                    {
                        JobId = (int)reader["JobId"],
                        Position = reader["Position"].ToString(),
                        Department = reader["Department"].ToString(),
                        Vacancies = (int)reader["Vacancies"],
                        PostedDate = (DateTime)reader["PostedDate"],
                        Status = reader["Status"].ToString()
                    };
                }
            }
            return job;
        }

        public void UpdateJob(HiringManagement job)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE HiringManagement SET 
                             Position = @Position, 
                             Department = @Department, 
                             Vacancies = @Vacancies, 
                             Status = @Status
                             WHERE JobId = @JobId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Position", job.Position);
                cmd.Parameters.AddWithValue("@Department", job.Department);
                cmd.Parameters.AddWithValue("@Vacancies", job.Vacancies);
                cmd.Parameters.AddWithValue("@Status", job.Status);
                cmd.Parameters.AddWithValue("@JobId", job.JobId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteJob(int jobId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Hiring.HiringManagement WHERE JobId = @JobId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@JobId", jobId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void CloseJob(int jobId)
        {
            string query = "UPDATE Hiring.HiringManagement SET Status = 'Closed' WHERE JobId = @JobId AND Status = 'Open'";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@JobId", jobId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error closing job posting: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateVacancies(int jobId, int vacancies)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Hiring.HiringManagement SET Vacancies = @Vacancies WHERE JobId = @JobId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Vacancies", vacancies);
                cmd.Parameters.AddWithValue("@JobId", jobId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<SchedulingModel> GetAllSchedules()
        {
            var list = new List<SchedulingModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeManagement.Scheduling", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SchedulingModel
                    {
                        ScheduleId = Convert.ToInt32(reader["schedule_id"]),
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        ShiftDate = Convert.ToDateTime(reader["shift_date"]),
                        ShiftStartTime = (TimeSpan)reader["shift_start_time"],
                        ShiftEndTime = (TimeSpan)reader["shift_end_time"],
                        ShiftType = reader["shift_type"].ToString()
                    });
                }
            }
            return list;
        }

        public SchedulingModel GetScheduleById(int id)
        {
            SchedulingModel schedule = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeManagement.Scheduling WHERE schedule_id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    schedule = new SchedulingModel
                    {
                        ScheduleId = Convert.ToInt32(reader["schedule_id"]),
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        ShiftDate = Convert.ToDateTime(reader["shift_date"]),
                        ShiftStartTime = (TimeSpan)reader["shift_start_time"],
                        ShiftEndTime = (TimeSpan)reader["shift_end_time"],
                        ShiftType = reader["shift_type"].ToString()
                    };
                }
            }
            return schedule;
        }

        public void InsertSchedule(SchedulingModel schedule)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"INSERT INTO EmployeeManagement.Scheduling 
                (employee_id, shift_date, shift_start_time, shift_end_time, shift_type)
                VALUES (@eid, @date, @start, @end, @type)", con);

                cmd.Parameters.AddWithValue("@eid", schedule.EmployeeId);
                cmd.Parameters.AddWithValue("@date", schedule.ShiftDate);
                cmd.Parameters.AddWithValue("@start", schedule.ShiftStartTime);
                cmd.Parameters.AddWithValue("@end", schedule.ShiftEndTime);
                cmd.Parameters.AddWithValue("@type", schedule.ShiftType);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateSchedule(SchedulingModel schedule)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"UPDATE EmployeeManagement.Scheduling SET 
                employee_id = @eid, shift_date = @date, shift_start_time = @start, 
                shift_end_time = @end, shift_type = @type 
                WHERE schedule_id = @id", con);

                cmd.Parameters.AddWithValue("@id", schedule.ScheduleId);
                cmd.Parameters.AddWithValue("@eid", schedule.EmployeeId);
                cmd.Parameters.AddWithValue("@date", schedule.ShiftDate);
                cmd.Parameters.AddWithValue("@start", schedule.ShiftStartTime);
                cmd.Parameters.AddWithValue("@end", schedule.ShiftEndTime);
                cmd.Parameters.AddWithValue("@type", schedule.ShiftType);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSchedule(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM EmployeeManagement.Scheduling WHERE schedule_id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<AttendanceModel> GetAllAttendance()
        {
            var attendanceList = new List<AttendanceModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        SELECT 
            a.attendance_id,
            a.employee_id,
            e.first_name + ' ' + e.last_name AS employee_name,
            a.date,
            a.attendance_status,
            a.time_in,
            a.time_out
        FROM EmployeeManagement.Attendance a
        INNER JOIN EmployeeManagement.Employees e ON a.employee_id = e.employee_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        attendanceList.Add(new AttendanceModel
                        {
                            AttendanceId = Convert.ToInt32(reader["attendance_id"]),
                            EmployeeId = Convert.ToInt32(reader["employee_id"]),
                            EmployeeName = reader["employee_name"].ToString(),
                            Date = Convert.ToDateTime(reader["date"]),
                            AttendanceStatus = reader["attendance_status"].ToString(),
                            PunchInTime = TryParseTime(reader["time_in"].ToString()) ?? TimeSpan.Zero,  // Default to TimeSpan.Zero if null
                            PunchOutTime = TryParseTime(reader["time_out"].ToString()) ?? TimeSpan.Zero // Default to TimeSpan.Zero if null
                        });
                    }
                }
            }

            return attendanceList;
        }

        private TimeSpan? TryParseTime(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                return null; // Return null if empty or invalid
            }

            if (TimeSpan.TryParse(timeString, out TimeSpan result))
            {
                return result;
            }

            return null; // Return null if parsing fails
        }


        public void MarkAttendance(AttendanceModel attendance)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO EmployeeManagement.Attendance 
            (employee_id, date, attendance_status, time_in, time_out, notes) 
            VALUES 
            (@EmployeeId, @Date, @AttendanceStatus, @TimeIn, @TimeOut, @Notes)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", attendance.EmployeeId);
                    cmd.Parameters.AddWithValue("@Date", attendance.Date.Date); // only date part

                    cmd.Parameters.AddWithValue("@AttendanceStatus", attendance.AttendanceStatus);
                    cmd.Parameters.AddWithValue("@TimeIn", attendance.PunchInTime);
                    cmd.Parameters.AddWithValue("@TimeOut",
                        attendance.PunchOutTime.HasValue ? attendance.PunchOutTime.Value : (object)DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public AttendanceModel GetAttendanceForToday(int employeeId, DateTime date)
        {
            AttendanceModel model = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
        SELECT attendance_status, time_in, time_out
        FROM EmployeeManagement.Attendance
        WHERE employee_id = @EmployeeId AND date = @Date";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Date", date.Date); // Ensure only date part is used

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new AttendanceModel
                    {
                        EmployeeId = employeeId,
                        Date = date,
                        AttendanceStatus = reader["attendance_status"].ToString(),
                        PunchInTime = TimeSpan.Parse(reader["time_in"].ToString()),
                        PunchOutTime = reader["time_out"] == DBNull.Value
                            ? (TimeSpan?)null
                            : TimeSpan.Parse(reader["time_out"].ToString())
                    };
                }
            }

            return model;
        }



        public void InsertPunchIn(AttendanceModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string insertQuery = @"
                INSERT INTO EmployeeManagement.Attendance 
                (employee_id, date, attendance_status, time_in)
                VALUES (@EmployeeId, @Date, @AttendanceStatus, @PunchInTime)";

                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@AttendanceStatus", model.AttendanceStatus ?? "Present");
                cmd.Parameters.AddWithValue("@PunchInTime", model.PunchInTime);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePunchOut(AttendanceModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string updateQuery = @"
                UPDATE EmployeeManagement.Attendance
                SET time_out = @PunchOutTime
                WHERE employee_id = @EmployeeId AND date = @Date";

                SqlCommand cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@PunchOutTime", model.PunchOutTime);
                cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
                cmd.Parameters.AddWithValue("@Date", model.Date);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

