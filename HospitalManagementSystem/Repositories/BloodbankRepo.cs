namespace HospitalManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using hospitalManagementSystem.Models;
    using HospitalManagementSystem.Repositories;
    using Microsoft.AspNetCore.Mvc;
    

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
                DateTime? lastDonationDate = donor.LastDonationDate;
                string eligibility = "Eligible";

                if (lastDonationDate.HasValue)
                {
                    eligibility = (DateTime.Now - lastDonationDate.Value).TotalDays >= 90 ? "Eligible" : "Not Eligible";
                }

                cmd.Parameters.AddWithValue("@EligibilityStatus", eligibility);

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

        public List<Donor> GetPagedDonors(int page, int pageSize, string search)
        {
            List<Donor> donors = new List<Donor>();
            int offset = (page - 1) * pageSize;

            string query = @"
        SELECT * FROM blood_bank.donors
        WHERE full_name LIKE @search or blood_type LIKE @search
        ORDER BY full_name
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        donors.Add(new Donor
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FullName = reader["full_name"].ToString(),
                            BloodType = reader["blood_type"].ToString(),
                            PhoneNumber = reader["phone_number"].ToString(),
                            EligibilityStatus = reader["eligibility_status"].ToString()
                        });
                    }
                }
            }

            return donors;
        }

        public int GetDonorCount(string search)
        {
            string query = "SELECT COUNT(*) FROM blood_bank.donors WHERE full_name LIKE @search or blood_type LIKE @search";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void AddDonationWithBag(int donorId, string bloodType)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Insert into BloodBags
                    string insertBagSql = @"
                    INSERT INTO blood_bank.blood_bags 
                    (donor_id, blood_type, collection_date, expiry_date, status, created_at, is_tested)
                    OUTPUT INSERTED.id
                    VALUES (@DonorId, @BloodType, @Collected, @Expiry, 'Available', GETDATE(), 0)";

                    SqlCommand cmdBag = new SqlCommand(insertBagSql, conn, transaction);
                    cmdBag.Parameters.AddWithValue("@DonorId", donorId);
                    cmdBag.Parameters.AddWithValue("@BloodType", bloodType);
                    cmdBag.Parameters.AddWithValue("@Collected", DateTime.Now);
                    cmdBag.Parameters.AddWithValue("@Expiry", DateTime.Now.AddDays(35));

                    int bloodBagId = (int)cmdBag.ExecuteScalar();

                    // Insert into BloodDonationHistory
                    string insertHistorySql = @"
                    INSERT INTO blood_bank.blood_donation_history 
                    (donor_id, donation_date, blood_bag_id, created_at)
                    VALUES (@DonorId, @Date, @BagId, GETDATE())";

                    SqlCommand cmdHistory = new SqlCommand(insertHistorySql, conn, transaction);
                    cmdHistory.Parameters.AddWithValue("@DonorId", donorId);
                    cmdHistory.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmdHistory.Parameters.AddWithValue("@BagId", bloodBagId);

                    cmdHistory.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<BloodDonationHistory> GetAllDonationHistory()
        {
            var list = new List<BloodDonationHistory>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                SELECT h.id, h.donor_id, h.donation_date, h.blood_bag_id, h.created_at,
                       b.blood_type, b.status
                FROM blood_bank.blood_donation_history h
                JOIN blood_bank.blood_bags b ON h.blood_bag_id = b.id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var history = new BloodDonationHistory
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            DonorId = Convert.ToInt32(reader["donor_id"]),
                            DonationDate = Convert.ToDateTime(reader["donation_date"]),
                            BloodBagId = Convert.ToInt32(reader["blood_bag_id"]),
                            CreatedAt = reader["created_at"] as DateTime?,
                            BloodBag = new BloodBag
                            {
                                Id = Convert.ToInt32(reader["blood_bag_id"]),
                                BloodType = reader["blood_type"].ToString(),
                                Status = reader["status"].ToString()
                            }
                        };
                        list.Add(history);
                    }
                }
            }

            return list;
        }
        public IEnumerable<BloodBag> GetAllBloodBags()
        {
            var bags = new List<BloodBag>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT id, donor_id, blood_type, collection_date, expiry_date, status, created_at, is_tested
            FROM blood_bank.blood_bags";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bag = new BloodBag
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            DonorId = Convert.ToInt32(reader["donor_id"]),
                            BloodType = reader["blood_type"].ToString(),
                            CollectionDate = Convert.ToDateTime(reader["collection_date"]),
                            ExpiryDate = Convert.ToDateTime(reader["expiry_date"]),
                            Status = reader["status"].ToString(),
                            CreatedAt = reader["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["created_at"]),
                            IsTested = reader["is_tested"] != DBNull.Value && (bool)reader["is_tested"]
                        };
                        bags.Add(bag);
                    }
                }
            }

            return bags;
        }

        public IEnumerable<BloodBag> GetBloodBagsPagedWithSearch(string searchTerm, int page, int pageSize)
        {
            var bags = new List<BloodBag>();
            int offset = (page - 1) * pageSize;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT id, donor_id, blood_type, collection_date, expiry_date, status, created_at, is_tested
            FROM blood_bank.blood_bags
            WHERE blood_type LIKE @Search
            ORDER BY id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bags.Add(new BloodBag
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                DonorId = Convert.ToInt32(reader["donor_id"]),
                                BloodType = reader["blood_type"].ToString(),
                                CollectionDate = Convert.ToDateTime(reader["collection_date"]),
                                ExpiryDate = Convert.ToDateTime(reader["expiry_date"]),
                                Status = reader["status"].ToString(),
                                CreatedAt = reader["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["created_at"]),
                                IsTested = reader["is_tested"] != DBNull.Value && (bool)reader["is_tested"]
                            });
                        }
                    }
                }
            }

            return bags;
        }

        public int GetTotalBloodBagsCountWithSearch(string searchTerm)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM blood_bank.blood_bags WHERE blood_type LIKE @Search";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public List<string> GetMatchingBloodTypes(string term)
        {
            var results = new List<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT DISTINCT blood_type FROM blood_bank.blood_bags WHERE blood_type LIKE @term + '%'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@term", term);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader["blood_type"].ToString());
                        }
                    }
                }
            }

            return results;
        }

        public List<Donor> GetEligibleDonorsByBloodType(string bloodType)
        {
            List<Donor> donors = new List<Donor>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
        SELECT id,full_name
        FROM blood_bank.donors
        WHERE blood_type = @BloodType AND eligibility_status = 'Eligible'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BloodType", bloodType);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            donors.Add(new Donor
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                FullName = reader["full_name"].ToString()
                            });
                        }
                    }
                }
            }

            return donors;
        }

        public void RecordDonation(int donorId, string bloodType)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Step 0: Check if the donor has donated in the last 90 days
                    string checkSql = @"
                SELECT COUNT(*) 
                FROM blood_bank.blood_donation_history
                WHERE donor_id = @DonorId AND donation_date >= DATEADD(DAY, -90, GETDATE());";

                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@DonorId", donorId);
                        int recentDonationCount = (int)checkCmd.ExecuteScalar();

                        if (recentDonationCount > 0)
                        {
                            throw new InvalidOperationException("Donor has already donated within the last 90 days.");
                        }
                    }

                    // Step 1: Insert into blood_bags and get the inserted blood bag ID
                    string insertBagSql = @"
                INSERT INTO blood_bank.blood_bags (donor_id, blood_type, collection_date, expiry_date, status)
                OUTPUT INSERTED.id
                VALUES (@DonorId, @BloodType, GETDATE(), DATEADD(DAY, 35, GETDATE()), 'Available');";

                    int bloodBagId;
                    using (SqlCommand cmd = new SqlCommand(insertBagSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DonorId", donorId);
                        cmd.Parameters.AddWithValue("@BloodType", bloodType);
                        bloodBagId = (int)cmd.ExecuteScalar();
                    }

                    // Step 2: Insert into blood_donation_history
                    string insertHistorySql = @"
                INSERT INTO blood_bank.blood_donation_history (donor_id, donation_date, blood_bag_id)
                VALUES (@DonorId, GETDATE(), @BloodBagId);";

                    using (SqlCommand cmd = new SqlCommand(insertHistorySql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DonorId", donorId);
                        cmd.Parameters.AddWithValue("@BloodBagId", bloodBagId);
                        cmd.ExecuteNonQuery();
                    }

                    // Step 3: Update donor's last_donation_date (optional)
                    string updateDonorSql = @"
                UPDATE blood_bank.donors
                SET last_donation_date = GETDATE()
                WHERE id = @DonorId;"; // ✅ Fix: removed erroneous `and` in original SQL

                    using (SqlCommand cmd = new SqlCommand(updateDonorSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DonorId", donorId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }



    }
}
