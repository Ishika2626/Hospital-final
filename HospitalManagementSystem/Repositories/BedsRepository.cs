using HospitalManagementSystem.Models;
using Microsoft.Data.SqlClient;
using HospitalManagementSystem.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagementSystem.Repositories
{
    public class BedsRepository : IBedsRepository
    {
        private readonly string _imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        private readonly string _connectionString;
        public BedsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddBed(Beds bed)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Beds (RoomID, BedNumber, BedType, Status) " +
                               "VALUES (@RoomID, @BedNumber, @BedType, @Status)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@RoomID", bed.RoomID);
                cmd.Parameters.AddWithValue("@BedNumber", bed.BedNumber);
                cmd.Parameters.AddWithValue("@BedType", bed.BedType.ToString());
                cmd.Parameters.AddWithValue("@Status", bed.Status.ToString());

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void AddBedMaintenance(Bed_Maintenance maintenance)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Bed_Maintenance (BedID, StartDate, EndDate, Reason, Status) " +
                               "VALUES (@BedID, @StartDate, @EndDate, @Reason, @Status)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BedID", maintenance.BedID);
                cmd.Parameters.AddWithValue("@StartDate", maintenance.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", maintenance.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Reason", maintenance.Reason);
                cmd.Parameters.AddWithValue("@Status", maintenance.Status.ToString());


                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddBedOccupancyHistory(Bed_Occupancy_History history)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Bed_Occupancy_History (BedID, PatientID, StartDate, EndDate, Status) " +
                               "VALUES (@BedID, @PatientID, @StartDate, @EndDate, @Status)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BedID", history.BedID);
                cmd.Parameters.AddWithValue("@PatientID", history.PatientID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", history.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", history.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", history.Status.ToString());


                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddBedReservation(Bed_Reservation reservation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Bed_Reservation (PatientID, BedID, ReservedDate, Status, LastUpdated) " +
                               "VALUES (@PatientID, @BedID, @ReservedDate, @Status, @LastUpdated)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PatientID", reservation.PatientID);
                cmd.Parameters.AddWithValue("@BedID", reservation.BedID);
                cmd.Parameters.AddWithValue("@ReservedDate", reservation.ReservedDate);
                cmd.Parameters.AddWithValue("@Status", reservation.Status.ToString());
                cmd.Parameters.AddWithValue("@LastUpdated", reservation.LastUpdated);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddBedTransfer(Bed_Transfer transfer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Bed_Transfer (PatientID, OldBedID, NewBedID, TransferDate, Reason) " +
                               "VALUES (@PatientID, @OldBedID, @NewBedID, @TransferDate, @Reason)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PatientID", transfer.PatientID);
                cmd.Parameters.AddWithValue("@OldBedID", transfer.OldBedID);
                cmd.Parameters.AddWithValue("@NewBedID", transfer.NewBedID);
                cmd.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
                cmd.Parameters.AddWithValue("@Reason", transfer.Reason);


                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddPatientBedAllocation(Patient_Bed_Allocation allocation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Patient_Bed_Allocation (PatientID, BedID, CheckInDate, CheckOutDate, Status) " +
                               "VALUES (@PatientID, @BedID, @CheckInDate, @CheckOutDate, @Status)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PatientID", allocation.PatientID);
                cmd.Parameters.AddWithValue("@BedID", allocation.BedID);
                cmd.Parameters.AddWithValue("@CheckInDate", allocation.CheckInDate);
                cmd.Parameters.AddWithValue("@CheckOutDate", allocation.CheckOutDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", allocation.Status.ToString());

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddRoom(Rooms room, IFormFile room_img)
        {
            string photo = SavePhoto(room_img);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Rooms (RoomNumber, RoomType, Capacity, Status, room_img) " +
                               "VALUES (@RoomNumber, @RoomType, @Capacity, @Status, @room_img)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                cmd.Parameters.AddWithValue("@RoomType", room.RoomType.GetDisplayName()); // ✅ FIXED
                cmd.Parameters.AddWithValue("@Capacity", room.Capacity);
                cmd.Parameters.AddWithValue("@Status", room.Status.ToString());
               
                cmd.Parameters.AddWithValue("@room_img", string.IsNullOrEmpty(photo) ? (object)DBNull.Value : photo);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void AddRoomOccupancy(Room_Occupancy occupancy)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO bed_managment.Room_Occupancy (RoomID, CurrentOccupancy, MaxCapacity, Status, LastUpdated) " +
                               "VALUES (@RoomID, @CurrentOccupancy, @MaxCapacity, @Status, @LastUpdated)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@RoomID", occupancy.RoomID);
                cmd.Parameters.AddWithValue("@CurrentOccupancy", occupancy.CurrentOccupancy);
                cmd.Parameters.AddWithValue("@MaxCapacity", occupancy.MaxCapacity);
                cmd.Parameters.AddWithValue("@Status", occupancy.Status.ToString());
                cmd.Parameters.AddWithValue("@LastUpdated", occupancy.LastUpdated);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBed(int bedId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Beds WHERE BedID = @BedID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BedID", bedId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteBedMaintenance(int maintenanceId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Bed_Maintenance WHERE MaintenanceID = @MaintenanceID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MaintenanceID", maintenanceId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBedOccupancyHistory(int historyId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Bed_Occupancy_History WHERE HistoryID = @HistoryID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@HistoryID", historyId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBedReservation(int reservationId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Bed_Reservation WHERE ReservationID = @ReservationID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ReservationID", reservationId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBedTransfer(int transferId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Bed_Transfer WHERE TransferID = @TransferID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TransferID", transferId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeletePatientBedAllocation(int allocationId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Patient_Bed_Allocation WHERE AllocationID = @AllocationID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AllocationID", allocationId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

     public void DeleteRoom(int roomId)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"
        DELETE pa FROM patient.patient_admission pa
        JOIN bed_managment.Beds b ON pa.bed_id = b.BedID
        WHERE b.RoomID = @RoomID;

        DELETE br FROM bed_managment.Bed_Reservation br
        JOIN bed_managment.Beds b ON br.BedID = b.BedID
        WHERE b.RoomID = @RoomID;

        DELETE boh FROM bed_managment.Bed_Occupancy_History boh
        JOIN bed_managment.Beds b ON boh.BedID = b.BedID
        WHERE b.RoomID = @RoomID;

        DELETE bm FROM bed_managment.Bed_Maintenance bm
        JOIN bed_managment.Beds b ON bm.BedID = b.BedID
        WHERE b.RoomID = @RoomID;

        DELETE bt FROM bed_managment.Bed_Transfer bt
        JOIN bed_managment.Beds b1 ON bt.OldBedID = b1.BedID
        JOIN bed_managment.Beds b2 ON bt.NewBedID = b2.BedID
        WHERE b1.RoomID = @RoomID OR b2.RoomID = @RoomID;

        DELETE pba FROM bed_managment.Patient_Bed_Allocation pba
        JOIN bed_managment.Beds b ON pba.BedID = b.BedID
        WHERE b.RoomID = @RoomID;

        DELETE ro FROM bed_managment.Room_Occupancy ro
        WHERE ro.RoomID = @RoomID;

        DELETE b FROM bed_managment.Beds b
        WHERE b.RoomID = @RoomID;

        DELETE FROM bed_managment.Rooms
        WHERE RoomID = @RoomID;
        ";

        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@RoomID", roomId);

        connection.Open();
        cmd.ExecuteNonQuery();
    }
}





        public void DeleteRoomOccupancy(int roomOccupancyId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM bed_managment.Room_Occupancy WHERE RoomOccupancyID = @RoomOccupancyID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@RoomOccupancyID", roomOccupancyId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public IEnumerable<Bed_Maintenance> GetAllBedMaintenances()
        {
            var maintenances = new List<Bed_Maintenance>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Maintenance";
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        maintenances.Add(new Bed_Maintenance
                        {
                            MaintenanceID = reader.GetInt32(reader.GetOrdinal("MaintenanceID")),
                            BedID = reader.GetInt32(reader.GetOrdinal("BedID")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            Reason = reader.GetString(reader.GetOrdinal("Reason")),
                            Status = (MaintenanceStatus)Enum.Parse(typeof(MaintenanceStatus), reader.GetString(reader.GetOrdinal("Status")))
                        });
                    }
                }
            }

            return maintenances;
        }

        public IEnumerable<Bed_Occupancy_History> GetAllBedOccupancyHistories()
        {
            var histories = new List<Bed_Occupancy_History>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Occupancy_History";
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        histories.Add(new Bed_Occupancy_History
                        {
                            HistoryID = reader.GetInt32(reader.GetOrdinal("HistoryID")),
                            BedID = reader.GetInt32(reader.GetOrdinal("BedID")),
                            PatientID = reader.IsDBNull(reader.GetOrdinal("PatientID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PatientID")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            Status = (OccupancyStatus)Enum.Parse(typeof(OccupancyStatus), reader.GetString(reader.GetOrdinal("Status")))
                        });
                    }
                }
            }

            return histories;
        }

        public IEnumerable<Bed_Reservation> GetAllBedReservations()
        {
            var reservations = new List<Bed_Reservation>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Reservation";
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Bed_Reservation
                        {
                            ReservationID = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                            PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                            BedID = reader.GetInt32(reader.GetOrdinal("BedID")),
                            ReservedDate = reader.GetDateTime(reader.GetOrdinal("ReservedDate")),
                            Status = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), reader.GetString(reader.GetOrdinal("Status"))),
                            LastUpdated = reader.GetDateTime(reader.GetOrdinal("LastUpdated"))
                        });
                    }
                }
            }

            return reservations;
        }
        public IEnumerable<Beds> GetAllBeds()
        {
            var beds = new List<Beds>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Beds";
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bed = new Beds();

                        bed.BedID = reader.IsDBNull(reader.GetOrdinal("BedID")) ? 0 : reader.GetInt32(reader.GetOrdinal("BedID"));
                        bed.RoomID = reader.IsDBNull(reader.GetOrdinal("RoomID")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoomID"));
                        bed.BedNumber = reader.IsDBNull(reader.GetOrdinal("BedNumber")) ? "" : reader.GetString(reader.GetOrdinal("BedNumber"));

                        var bedTypeStr = reader.IsDBNull(reader.GetOrdinal("BedType")) ? "General" : reader.GetString(reader.GetOrdinal("BedType"));
                        bed.BedType = Enum.TryParse<BedType>(bedTypeStr, out var bedType) ? bedType : BedType.General;

                        var statusStr = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Available" : reader.GetString(reader.GetOrdinal("Status"));
                        bed.Status = Enum.TryParse<Status>(statusStr, out var status) ? status : Status.Available;

                        bed.LastUpdated = reader.IsDBNull(reader.GetOrdinal("LastUpdated")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastUpdated"));

                        beds.Add(bed);
                    }

                }
            }

            return beds;
        }

        public IEnumerable<Bed_Transfer> GetAllBedTransfers()
        {
            var transfers = new List<Bed_Transfer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Transfer";
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transfers.Add(new Bed_Transfer
                        {
                            TransferID = reader.GetInt32(reader.GetOrdinal("TransferID")),
                            PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                            OldBedID = reader.GetInt32(reader.GetOrdinal("OldBedID")),
                            NewBedID = reader.GetInt32(reader.GetOrdinal("NewBedID")),
                            TransferDate = reader.GetDateTime(reader.GetOrdinal("TransferDate")),
                            Reason = reader.GetString(reader.GetOrdinal("Reason"))
                        });
                    }
                }
            }

            return transfers;
        }

        public IEnumerable<Patient_Bed_Allocation> GetAllPatientBedAllocations()
        {
            var list = new List<Patient_Bed_Allocation>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Patient_Bed_Allocation";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Patient_Bed_Allocation
                    {
                        AllocationID = Convert.ToInt32(reader["AllocationID"]),
                        PatientID = Convert.ToInt32(reader["PatientID"]),
                        BedID = Convert.ToInt32(reader["BedID"]),
                        CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                        CheckOutDate = reader["CheckOutDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CheckOutDate"]),
                        Status = (AllocationStatus)Enum.Parse(typeof(AllocationStatus), reader["Status"].ToString())
                    });
                }
            }
            return list;
        }

        public IEnumerable<Room_Occupancy> GetAllRoomOccupancies()
        {
            var list = new List<Room_Occupancy>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Room_Occupancy";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Room_Occupancy
                    {
                        RoomOccupancyID = Convert.ToInt32(reader["RoomOccupancyID"]),
                        RoomID = Convert.ToInt32(reader["RoomID"]),
                        CurrentOccupancy = Convert.ToInt32(reader["CurrentOccupancy"]),
                        MaxCapacity = Convert.ToInt32(reader["MaxCapacity"]),
                        Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), reader["Status"].ToString()),
                        LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                    });
                }
            }
            return list;
        }

        public IEnumerable<Rooms> GetAllRooms()
        {
            var rooms = new List<Rooms>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Rooms";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rooms.Add(new Rooms
                    {
                        RoomID = Convert.ToInt32(reader["RoomID"]),
                        RoomNumber = reader["RoomNumber"].ToString(),
                        RoomType = GetRoomType(reader["RoomType"].ToString()), // Use the custom mapping function here

                        Capacity = Convert.ToInt32(reader["Capacity"]),
                        Status = (Status)Enum.Parse(typeof(Status), reader["Status"].ToString()),

                        
                        room_img = reader["room_img"]?.ToString(),
                        LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                    });
                }
            }
            return rooms;
        }

        private RoomType GetRoomType(string roomTypeString)
        {
            switch (roomTypeString.Trim()) // Trim any extra spaces
            {
                case "General Ward":
                    return RoomType.GeneralWard;
                case "ICU":
                    return RoomType.ICU;
                case "Private":
                    return RoomType.Private;
                case "Semi-Private":
                    return RoomType.SemiPrivate;
                default:
                    throw new ArgumentException($"Invalid room type: {roomTypeString}");
            }
        }


        public Beds GetBedById(int bedId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Beds WHERE BedID = @BedID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BedID", bedId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Beds
                        {
                            BedID = bedId,
                            RoomID = Convert.ToInt32(reader["RoomID"]),
                            BedNumber = reader["BedNumber"].ToString(),
                            BedType = (BedType)Enum.Parse(typeof(BedType), reader["BedType"].ToString()),
                            Status = (Status)Enum.Parse(typeof(Status), reader["Status"].ToString()),
                            LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                        };
                    }
                }
            }
            return null;
        }

        public Bed_Maintenance GetBedMaintenanceById(int maintenanceId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Maintenance WHERE MaintenanceID = @MaintenanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaintenanceID", maintenanceId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Bed_Maintenance
                        {
                            MaintenanceID = maintenanceId,
                            BedID = Convert.ToInt32(reader["BedID"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDate"]),
                            Reason = reader["Reason"]?.ToString(),
                            Status = (MaintenanceStatus)Enum.Parse(typeof(MaintenanceStatus), reader["Status"].ToString())
                        };
                    }
                }
            }
            return null;
        }
        public Bed_Occupancy_History GetBedOccupancyHistoryById(int historyId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Occupancy_History WHERE HistoryID = @HistoryID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HistoryID", historyId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Bed_Occupancy_History
                        {
                            HistoryID = historyId,
                            BedID = Convert.ToInt32(reader["BedID"]),
                            PatientID = reader["PatientID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["PatientID"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDate"]),
                            Status = (OccupancyStatus)Enum.Parse(typeof(OccupancyStatus), reader["Status"].ToString())
                        };
                    }
                }
            }
            return null;
        }


        public Bed_Reservation GetBedReservationById(int reservationId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Reservation WHERE ReservationID = @ReservationID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReservationID", reservationId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Bed_Reservation
                        {
                            ReservationID = reservationId,
                            PatientID = Convert.ToInt32(reader["PatientID"]),
                            BedID = Convert.ToInt32(reader["BedID"]),
                            ReservedDate = Convert.ToDateTime(reader["ReservedDate"]),
                            Status = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), reader["Status"].ToString()),
                            LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                        };
                    }
                }
            }
            return null;
        }


        public Bed_Transfer GetBedTransferById(int transferId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Bed_Transfer WHERE TransferID = @TransferID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TransferID", transferId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Bed_Transfer
                        {
                            TransferID = transferId,
                            PatientID = Convert.ToInt32(reader["PatientID"]),
                            OldBedID = Convert.ToInt32(reader["OldBedID"]),
                            NewBedID = Convert.ToInt32(reader["NewBedID"]),
                            TransferDate = Convert.ToDateTime(reader["TransferDate"]),
                            Reason = reader["Reason"]?.ToString()
                        };
                    }
                }
            }
            return null;
        }

        public Patient_Bed_Allocation GetPatientBedAllocationById(int allocationId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Patient_Bed_Allocation WHERE AllocationID = @AllocationID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AllocationID", allocationId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Patient_Bed_Allocation
                        {
                            AllocationID = allocationId,
                            PatientID = Convert.ToInt32(reader["PatientID"]),
                            BedID = Convert.ToInt32(reader["BedID"]),
                            CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                            CheckOutDate = reader["CheckOutDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CheckOutDate"]),
                            Status = (AllocationStatus)Enum.Parse(typeof(AllocationStatus), reader["Status"].ToString())
                        };
                    }
                }
            }
            return null;
        }


        public Rooms GetRoomById(int roomId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Rooms WHERE RoomID = @RoomID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomID", roomId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Rooms
                        {
                            RoomID = roomId,
                            RoomNumber = reader["RoomNumber"].ToString(),
                            RoomType = GetRoomType(reader["RoomType"].ToString()), // Use custom method here
                            Capacity = Convert.ToInt32(reader["Capacity"]),
                            Status = (Status)Enum.Parse(typeof(Status), reader["Status"].ToString()),
                          
                            room_img = reader["room_img"]?.ToString(),
                            LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                        };
                    }
                }
            }
            return null;
        }


        public Room_Occupancy GetRoomOccupancyById(int roomOccupancyId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM bed_managment.Room_Occupancy WHERE RoomOccupancyID = @RoomOccupancyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomOccupancyID", roomOccupancyId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Room_Occupancy
                        {
                            RoomOccupancyID = roomOccupancyId,
                            RoomID = Convert.ToInt32(reader["RoomID"]),
                            CurrentOccupancy = Convert.ToInt32(reader["CurrentOccupancy"]),
                            MaxCapacity = Convert.ToInt32(reader["MaxCapacity"]),
                            Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), reader["Status"].ToString()),
                            LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                        };
                    }
                }
            }
            return null;
        }

        public void UpdateBed(Beds bed)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Beds 
                         SET RoomID = @RoomID, BedNumber = @BedNumber, BedType = @BedType, 
                             Status = @Status, LastUpdated = @LastUpdated 
                         WHERE BedID = @BedID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomID", bed.RoomID);
                cmd.Parameters.AddWithValue("@BedNumber", bed.BedNumber);
                cmd.Parameters.AddWithValue("@BedType", bed.BedType.ToString());
                cmd.Parameters.AddWithValue("@Status", bed.Status.ToString());
                cmd.Parameters.AddWithValue("@LastUpdated", bed.LastUpdated);
                cmd.Parameters.AddWithValue("@BedID", bed.BedID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBedMaintenance(Bed_Maintenance maintenance)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Bed_Maintenance 
                         SET BedID = @BedID, StartDate = @StartDate, EndDate = @EndDate, 
                             Reason = @Reason, Status = @Status 
                         WHERE MaintenanceID = @MaintenanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BedID", maintenance.BedID);
                cmd.Parameters.AddWithValue("@StartDate", maintenance.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object)maintenance.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Reason", (object)maintenance.Reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", maintenance.Status.ToString());
                cmd.Parameters.AddWithValue("@MaintenanceID", maintenance.MaintenanceID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void UpdateBedOccupancyHistory(Bed_Occupancy_History history)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Bed_Occupancy_History 
                         SET BedID = @BedID, PatientID = @PatientID, StartDate = @StartDate, 
                             EndDate = @EndDate, Status = @Status 
                         WHERE HistoryID = @HistoryID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BedID", history.BedID);
                cmd.Parameters.AddWithValue("@PatientID", (object)history.PatientID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", history.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object)history.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", history.Status.ToString());
                cmd.Parameters.AddWithValue("@HistoryID", history.HistoryID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBedReservation(Bed_Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Bed_Reservation 
                         SET PatientID = @PatientID, BedID = @BedID, ReservedDate = @ReservedDate, 
                             Status = @Status, LastUpdated = @LastUpdated 
                         WHERE ReservationID = @ReservationID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", reservation.PatientID);
                cmd.Parameters.AddWithValue("@BedID", reservation.BedID);
                cmd.Parameters.AddWithValue("@ReservedDate", reservation.ReservedDate);
                cmd.Parameters.AddWithValue("@Status", reservation.Status.ToString());
                cmd.Parameters.AddWithValue("@LastUpdated", reservation.LastUpdated);
                cmd.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBedTransfer(Bed_Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Bed_Transfer 
                         SET PatientID = @PatientID, OldBedID = @OldBedID, 
                             NewBedID = @NewBedID, TransferDate = @TransferDate, Reason = @Reason 
                         WHERE TransferID = @TransferID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", transfer.PatientID);
                cmd.Parameters.AddWithValue("@OldBedID", transfer.OldBedID);
                cmd.Parameters.AddWithValue("@NewBedID", transfer.NewBedID);
                cmd.Parameters.AddWithValue("@TransferDate", transfer.TransferDate);
                cmd.Parameters.AddWithValue("@Reason", (object)transfer.Reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TransferID", transfer.TransferID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePatientBedAllocation(Patient_Bed_Allocation allocation)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Patient_Bed_Allocation 
                         SET PatientID = @PatientID, BedID = @BedID, CheckInDate = @CheckInDate, 
                             CheckOutDate = @CheckOutDate, Status = @Status 
                         WHERE AllocationID = @AllocationID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", allocation.PatientID);
                cmd.Parameters.AddWithValue("@BedID", allocation.BedID);
                cmd.Parameters.AddWithValue("@CheckInDate", allocation.CheckInDate);
                cmd.Parameters.AddWithValue("@CheckOutDate", (object)allocation.CheckOutDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", allocation.Status.ToString());
                cmd.Parameters.AddWithValue("@AllocationID", allocation.AllocationID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string GetRoomTypeDbValue(RoomType type)
        {
            return type switch
            {
                RoomType.GeneralWard => "General Ward",
                RoomType.ICU => "ICU",
                RoomType.Private => "Private",
                RoomType.SemiPrivate => "Semi-Private",
                _ => throw new ArgumentException("Invalid RoomType")
            };
        }

        public void UpdateRoom(Rooms room, IFormFile room_img)
        {
            string newPhotoPath = room.room_img; // Default to existing image

            if (room_img != null && room_img.Length > 0)
            {
                newPhotoPath = SavePhoto(room_img);

                if (string.IsNullOrEmpty(newPhotoPath))
                {
                    throw new Exception("Error saving photo.");
                }
            }
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Rooms 
                         SET RoomNumber = @RoomNumber, RoomType = @RoomType, Capacity = @Capacity, 
                             Status = @Status, room_img = @room_img 
                         WHERE RoomID = @RoomID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                cmd.Parameters.AddWithValue("@RoomType", GetRoomTypeDbValue(room.RoomType));
                cmd.Parameters.AddWithValue("@Capacity", room.Capacity);
                cmd.Parameters.AddWithValue("@Status", room.Status.ToString());
              
                cmd.Parameters.AddWithValue("@room_img", newPhotoPath ?? (object)DBNull.Value);
                
                cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                conn.Open();
                cmd.ExecuteNonQuery();
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

        public void UpdateRoomOccupancy(Room_Occupancy occupancy)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE bed_managment.Room_Occupancy 
                         SET RoomID = @RoomID, CurrentOccupancy = @CurrentOccupancy, 
                             MaxCapacity = @MaxCapacity, Status = @Status, LastUpdated = @LastUpdated 
                         WHERE RoomOccupancyID = @RoomOccupancyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomID", occupancy.RoomID);
                cmd.Parameters.AddWithValue("@CurrentOccupancy", occupancy.CurrentOccupancy);
                cmd.Parameters.AddWithValue("@MaxCapacity", occupancy.MaxCapacity);
                cmd.Parameters.AddWithValue("@Status", occupancy.Status.ToString());
                cmd.Parameters.AddWithValue("@LastUpdated", occupancy.LastUpdated);
                cmd.Parameters.AddWithValue("@RoomOccupancyID", occupancy.RoomOccupancyID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<SelectListItem> GetRoomTypes()
        {
            var list = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RoomID, RoomNumber FROM bed_managment.Rooms", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["RoomID"].ToString(),
                        Text = reader["RoomNumber"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
