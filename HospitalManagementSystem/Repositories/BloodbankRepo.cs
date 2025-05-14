namespace HospitalManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using hospitalManagementSystem.Models;
    using HospitalManagementSystem.Repositories;

    public class BloodbankRepo : IBloodbankRepo
    {
        private readonly string _connectionString;

        public BloodbankRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Add a new donor
        public void AddDonor(Donor donor)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO blood_bank.donors 
                                 (campaign_id, full_name, date_of_birth, gender, blood_type, phone_number, email, address, last_donation_date, eligibility_status)
                                 VALUES
                                 (@CampaignId, @FullName, @DateOfBirth, @Gender, @BloodType, @PhoneNumber, @Email, @Address, @LastDonationDate, @EligibilityStatus)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CampaignId", (object)donor.CampaignId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FullName", donor.FullName);
                cmd.Parameters.AddWithValue("@DateOfBirth", donor.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", donor.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BloodType", donor.BloodType);
                cmd.Parameters.AddWithValue("@PhoneNumber", donor.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", donor.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", donor.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastDonationDate", (object)donor.LastDonationDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EligibilityStatus", donor.EligibilityStatus);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateEligibility(int id, string eligibilityStatus)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE blood_bank.donors SET eligibility_status = @EligibilityStatus WHERE id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EligibilityStatus", eligibilityStatus);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        // Update an existing donor
        public void UpdateDonor(Donor donor)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE blood_bank.donors SET
                                 campaign_id = @CampaignId,
                                 full_name = @FullName,
                                 date_of_birth = @DateOfBirth,
                                 gender = @Gender,
                                 blood_type = @BloodType,
                                 phone_number = @PhoneNumber,
                                 email = @Email,
                                 address = @Address,
                                 last_donation_date = @LastDonationDate,
                                 eligibility_status = @EligibilityStatus
                                 WHERE id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", donor.Id);
                cmd.Parameters.AddWithValue("@CampaignId", (object)donor.CampaignId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FullName", donor.FullName);
                cmd.Parameters.AddWithValue("@DateOfBirth", donor.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", donor.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BloodType", donor.BloodType);
                cmd.Parameters.AddWithValue("@PhoneNumber", donor.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", donor.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", donor.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastDonationDate", (object)donor.LastDonationDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EligibilityStatus", donor.EligibilityStatus);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a donor by ID
        public void DeleteDonor(int donorId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE blood_bank.donors SET IsDeleted = 1 WHERE id = @Id"; // Soft delete
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", donorId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get a donor by ID
        public Donor GetDonorById(int donorId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM blood_bank.donors WHERE id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", donorId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Donor donor = new Donor
                    {
                        Id = (int)reader["id"],
                        CampaignId = reader["campaign_id"] as int?,
                        FullName = reader["full_name"].ToString(),
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        Gender = reader["gender"].ToString(),
                        BloodType = reader["blood_type"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        Email = reader["email"].ToString(),
                        Address = reader["address"].ToString(),
                        LastDonationDate = reader["last_donation_date"] as DateTime?,
                        EligibilityStatus = reader["eligibility_status"].ToString()
                    };

                    return donor;
                }
                return null;
            }
        }

        // Get all donors
        public List<Donor> GetAllDonors()
        {
            List<Donor> donors = new List<Donor>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM blood_bank.donors"; // Exclude deleted donors
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Donor donor = new Donor
                    {
                        Id = (int)reader["id"],
                        CampaignId = reader["campaign_id"] as int?,
                        FullName = reader["full_name"].ToString(),
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        Gender = reader["gender"].ToString(),
                        BloodType = reader["blood_type"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        Email = reader["email"].ToString(),
                        Address = reader["address"].ToString(),
                        LastDonationDate = reader["last_donation_date"] as DateTime?,
                        EligibilityStatus = reader["eligibility_status"].ToString()
                    };

                    donors.Add(donor);
                }
            }

            return donors;
        }
    }
}
