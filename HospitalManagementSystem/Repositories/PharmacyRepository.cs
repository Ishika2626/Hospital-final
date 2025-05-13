using HospitalManagementSystem.Models;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Repositories
{
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly string _connectionString;

        public PharmacyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddAmbulance(Ambulance ambulance)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        INSERT INTO EmergencyAmbulanceManagement.ambulances 
        (AmbulanceNumber)
        VALUES (@AmbulanceNumber)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AmbulanceNumber", ambulance.AmbulanceNumber);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddAmbulanceRequest(AmbulanceRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        INSERT INTO EmergencyAmbulanceManagement.ambulance_requests 
        (PatientID, RequestTime, PickupLocation, Destination, AmbulanceID, Status, AssignedDriver, Notes)
        VALUES 
        (@PatientID, @RequestTime, @PickupLocation, @Destination, @AmbulanceID, @Status, @AssignedDriver, @Notes)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", request.PatientID);
                cmd.Parameters.AddWithValue("@RequestTime", request.RequestTime);
                cmd.Parameters.AddWithValue("@PickupLocation", request.PickupLocation);
                cmd.Parameters.AddWithValue("@Destination", request.Destination);
                cmd.Parameters.AddWithValue("@AmbulanceID", (object)request.AmbulanceID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", request.Status.ToString());
                cmd.Parameters.AddWithValue("@AssignedDriver", request.AssignedDriver);
                cmd.Parameters.AddWithValue("@Notes", (object)request.Notes ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddEmergencyCase(EmergencyCase emergencyCase)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        INSERT INTO EmergencyAmbulanceManagement.emergency_cases 
        (PatientID, ArrivalTime, SeverityLevel, Diagnosis, TreatmentGiven, DoctorID, Status, Notes)
        VALUES 
        (@PatientID, @ArrivalTime, @SeverityLevel, @Diagnosis, @TreatmentGiven, @DoctorID, @Status, @Notes)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", emergencyCase.PatientID);
                cmd.Parameters.AddWithValue("@ArrivalTime", emergencyCase.ArrivalTime);
                cmd.Parameters.AddWithValue("@SeverityLevel", emergencyCase.SeverityLevel.ToString());
                cmd.Parameters.AddWithValue("@Diagnosis", (object)emergencyCase.Diagnosis ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TreatmentGiven", (object)emergencyCase.TreatmentGiven ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DoctorID", emergencyCase.DoctorID);
                cmd.Parameters.AddWithValue("@Status", emergencyCase.Status.ToString());
                cmd.Parameters.AddWithValue("@Notes", (object)emergencyCase.Notes ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // Add a new medicine      
        public void AddMedicine(Medicine medicine)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {        
                string query = @"
        INSERT INTO PharmacyManagement.medicines 
        (medicine_name, generic_name, brand_name, dosage_form, strength, manufacturer, expiry_date, unit_price, stock_quantity)
        VALUES 
        (@medicine_name, @generic_name, @brand_name, @dosage_form, @strength, @manufacturer, @expiry_date, @unit_price, @stock_quantity)";

                SqlCommand cmd = new SqlCommand(query, conn);

                // Ensure expiry date is valid for SQL Server
                if (medicine.ExpiryDate < new DateTime(1753, 1, 1))
                    medicine.ExpiryDate = DateTime.Now.AddYears(1);

                // Adding parameters for the query
                cmd.Parameters.AddWithValue("@medicine_name", medicine.MedicineName);
                cmd.Parameters.AddWithValue("@generic_name", medicine.GenericName);
                cmd.Parameters.AddWithValue("@brand_name", (object)medicine.BrandName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dosage_form", medicine.DosageForm);
                cmd.Parameters.AddWithValue("@strength", medicine.Strength);
                cmd.Parameters.AddWithValue("@manufacturer", medicine.Manufacturer);
                cmd.Parameters.AddWithValue("@expiry_date", medicine.ExpiryDate);
                cmd.Parameters.AddWithValue("@unit_price", medicine.UnitPrice);
                cmd.Parameters.AddWithValue("@stock_quantity", medicine.StockQuantity);

                // Open connection and execute query
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }



        // Add a new pharmacy order
        public void AddPharmacyOrder(PharmacyOrder order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO PharmacyManagement.pharmacy_orders (medicine_id, order_quantity, order_date, order_status, supplier_name, total_cost, created_at, updated_at) VALUES (@medicine_id, @order_quantity, @order_date, @order_status, @supplier_name, @total_cost, @created_at, @updated_at)", connection);

                command.Parameters.AddWithValue("@medicine_id", order.MedicineId);
                command.Parameters.AddWithValue("@order_quantity", order.OrderQuantity);
                command.Parameters.AddWithValue("@order_date", order.OrderDate);
                command.Parameters.AddWithValue("@order_status", order.OrderStatus);
                command.Parameters.AddWithValue("@supplier_name", order.SupplierName);
                command.Parameters.AddWithValue("@total_cost", order.TotalCost);
                command.Parameters.AddWithValue("@created_at", order.CreatedAt);
                command.Parameters.AddWithValue("@updated_at", order.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Add a new pharmacy stock entry
        public void AddPharmacyStock(PharmacyStock stock)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
            INSERT INTO PharmacyManagement.pharmacy_stock 
            (medicine_id, stock_quantity, stock_in_date, stock_out_date, stock_balance, created_at, updated_at) 
            VALUES 
            (@medicine_id, @stock_quantity, @stock_in_date, @stock_out_date, @stock_balance, @created_at, @updated_at)",
                    connection);

                command.Parameters.AddWithValue("@medicine_id", stock.MedicineId);
                command.Parameters.AddWithValue("@stock_quantity", stock.StockQuantity);
                command.Parameters.AddWithValue("@stock_in_date", stock.StockInDate);
                command.Parameters.AddWithValue("@stock_out_date", (object)stock.StockOutDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@stock_balance", stock.StockBalance);  // Add this!
                command.Parameters.AddWithValue("@created_at", stock.CreatedAt);
                command.Parameters.AddWithValue("@updated_at", stock.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Add a new prescription
        public void AddPrescription(PharmacyPrescriptionEntity prescription)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO PharmacyManagement.prescriptions (patient_id, doctor_id, prescription_date, total_amount, status, created_at, updated_at) VALUES (@patient_id, @doctor_id, @prescription_date, @total_amount, @status, @created_at, @updated_at)", connection);

                command.Parameters.AddWithValue("@patient_id", prescription.PatientId);
                command.Parameters.AddWithValue("@doctor_id", prescription.DoctorId);
                command.Parameters.AddWithValue("@prescription_date", prescription.PrescriptionDate);
                command.Parameters.AddWithValue("@total_amount", prescription.TotalAmount);
                command.Parameters.AddWithValue("@status", prescription.Status);
                command.Parameters.AddWithValue("@created_at", prescription.CreatedAt);
                command.Parameters.AddWithValue("@updated_at", prescription.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteAmbulance(int ambulanceId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM EmergencyAmbulanceManagement.ambulances WHERE AmbulanceID = @AmbulanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AmbulanceID", ambulanceId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAmbulanceRequest(int requestId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM EmergencyAmbulanceManagement.ambulance_requests WHERE RequestID = @RequestID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RequestID", requestId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteEmergencyCase(int caseId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM EmergencyAmbulanceManagement.emergency_cases WHERE CaseID = @CaseID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CaseID", caseId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // Delete a medicine
        public void DeleteMedicine(int medicineId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM PharmacyManagement.medicines WHERE medicine_id = @medicine_id", connection);

                command.Parameters.AddWithValue("@medicine_id", medicineId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Delete a pharmacy order
        public void DeletePharmacyOrder(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM PharmacyManagement.pharmacy_orders WHERE order_id = @order_id", connection);

                command.Parameters.AddWithValue("@order_id", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Delete pharmacy stock entry
        public void DeletePharmacyStock(int stockId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM PharmacyManagement.pharmacy_stock WHERE stock_id = @stock_id", connection);

                command.Parameters.AddWithValue("@stock_id", stockId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Delete a prescription
        public void DeletePrescription(int prescriptionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM PharmacyManagement.prescriptions WHERE prescription_id = @prescription_id", connection);

                command.Parameters.AddWithValue("@prescription_id", prescriptionId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<Ambulance> GetAllAmbulances()
        {
            var ambulances = new List<Ambulance>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT AmbulanceID, AmbulanceNumber FROM EmergencyAmbulanceManagement.ambulances";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ambulances.Add(new Ambulance
                    {
                        AmbulanceID = reader.GetInt32(0),
                        AmbulanceNumber = reader.GetString(1)
                    });
                }
            }

            return ambulances;
        }

        public IEnumerable<AmbulanceRequest> GetAllAmbulanceRequests()
        {
            var requests = new List<AmbulanceRequest>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT RequestID, PatientID, RequestTime, PickupLocation, Destination,
                   AmbulanceID, Status, AssignedDriver, Notes
            FROM EmergencyAmbulanceManagement.ambulance_requests";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    requests.Add(new AmbulanceRequest
                    {
                        RequestID = reader.GetInt32(0),
                        PatientID = reader.GetInt32(1),
                        RequestTime = reader.GetDateTime(2),
                        PickupLocation = reader.GetString(3),
                        Destination = reader.GetString(4),
                        AmbulanceID = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                        Status = Enum.Parse<AmbulanceRequestStatus>(reader.GetString(6)),
                        AssignedDriver = reader.GetInt32(7),
                        Notes = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }

            return requests;
        }

        public IEnumerable<EmergencyCase> GetAllEmergencyCases()
        {
            var cases = new List<EmergencyCase>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT CaseID, PatientID, ArrivalTime, SeverityLevel,
                   Diagnosis, TreatmentGiven, DoctorID, Status, Notes
            FROM EmergencyAmbulanceManagement.emergency_cases";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cases.Add(new EmergencyCase
                    {
                        CaseID = reader.GetInt32(0),
                        PatientID = reader.GetInt32(1),
                        ArrivalTime = reader.GetDateTime(2),
                        SeverityLevel = Enum.Parse<SeverityLevel>(reader.GetString(3)),
                        Diagnosis = reader.IsDBNull(4) ? null : reader.GetString(4),
                        TreatmentGiven = reader.IsDBNull(5) ? null : reader.GetString(5),
                        DoctorID = reader.GetInt32(6),
                        Status = Enum.Parse<EmergencyCaseStatus>(reader.GetString(7)),
                        Notes = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }

            return cases;
        }

        // Get all medicines
        public IEnumerable<Medicine> GetAllMedicines()
        {
            var medicines = new List<Medicine>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.medicines", connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    medicines.Add(new Medicine
                    {
                        MedicineId = (int)reader["medicine_id"],
                        MedicineName = (string)reader["medicine_name"],
                        GenericName = (string)reader["generic_name"],
                        BrandName = reader["brand_name"] as string,
                        DosageForm = (string)reader["dosage_form"],
                        Strength = (string)reader["strength"],
                        Manufacturer = (string)reader["manufacturer"],
                        ExpiryDate = (DateTime)reader["expiry_date"],
                        UnitPrice = (decimal)reader["unit_price"],
                        StockQuantity = (int)reader["stock_quantity"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    });
                }
            }

            return medicines;
        }

        // Get all pharmacy orders
        public IEnumerable<PharmacyOrder> GetAllPharmacyOrders()
        {
            var orders = new List<PharmacyOrder>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.pharmacy_orders", connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new PharmacyOrder
                    {
                        OrderId = (int)reader["order_id"],
                        MedicineId = (int)reader["medicine_id"],
                        OrderQuantity = (int)reader["order_quantity"],
                        OrderDate = (DateTime)reader["order_date"],
                        OrderStatus = (string)reader["order_status"],
                        SupplierName = (string)reader["supplier_name"],
                        TotalCost = (decimal)reader["total_cost"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    });
                }
            }

            return orders;
        }

        // Get all pharmacy stock entries
        public IEnumerable<PharmacyStock> GetAllPharmacyStock()
        {
            var stocks = new List<PharmacyStock>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.pharmacy_stock", connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    stocks.Add(new PharmacyStock
                    {
                        StockId = (int)reader["stock_id"],
                        MedicineId = (int)reader["medicine_id"],
                        StockQuantity = (int)reader["stock_quantity"],
                        StockInDate = (DateTime)reader["stock_in_date"],
                        StockOutDate = reader["stock_out_date"] as DateTime?,
                        StockBalance = (int)reader["stock_balance"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    });
                }
            }

            return stocks;
        }

        // Get all prescriptions
        public IEnumerable<PharmacyPrescriptionEntity> GetAllPrescriptions()
        {
            var prescriptions = new List<PharmacyPrescriptionEntity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.prescriptions", connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    prescriptions.Add(new PharmacyPrescriptionEntity
                    {
                        PrescriptionId = (int)reader["prescription_id"],
                        PatientId = (int)reader["patient_id"],
                        DoctorId = (int)reader["doctor_id"],
                        PrescriptionDate = (DateTime)reader["prescription_date"],
                        TotalAmount = (decimal)reader["total_amount"],
                        Status = (string)reader["status"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    });
                }
            }

            return prescriptions;
        }

        public Ambulance GetAmbulanceById(int ambulanceId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT AmbulanceID, AmbulanceNumber FROM EmergencyAmbulancManagement.ambulances WHERE AmbulanceID = @AmbulanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AmbulanceID", ambulanceId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Ambulance
                        {
                            AmbulanceID = reader.GetInt32(0),
                            AmbulanceNumber = reader.GetString(1)
                        };
                    }
                }
            }
            return null; // Not found
        }

        public AmbulanceRequest GetAmbulanceRequestById(int requestId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT RequestID, PatientID, RequestTime, PickupLocation, Destination,
                   AmbulanceID, Status, AssignedDriver, Notes
            FROM EmergencyAmbulanceManagement.ambulance_requests
            WHERE RequestID = @RequestID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RequestID", requestId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AmbulanceRequest
                        {
                            RequestID = reader.GetInt32(0),
                            PatientID = reader.GetInt32(1),
                            RequestTime = reader.GetDateTime(2),
                            PickupLocation = reader.GetString(3),
                            Destination = reader.GetString(4),
                            AmbulanceID = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                            Status = (AmbulanceRequestStatus)reader.GetInt32(6),
                            AssignedDriver = reader.GetInt32(7),
                            Notes = reader.IsDBNull(8) ? null : reader.GetString(8)
                        };
                    }
                }
            }
            return null;
        }
        public EmergencyCase GetEmergencyCaseById(int caseId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT CaseID, PatientID, ArrivalTime, SeverityLevel,
                   Diagnosis, TreatmentGiven, DoctorID, Status, Notes
            FROM EmergencyAmbulanceManagement.emergency_cases
            WHERE CaseID = @CaseID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CaseID", caseId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new EmergencyCase
                        {
                            CaseID = reader.GetInt32(0),
                            PatientID = reader.GetInt32(1),
                            ArrivalTime = reader.GetDateTime(2),
                            SeverityLevel = Enum.Parse<SeverityLevel>(reader.GetString(3)),
                            Diagnosis = reader.IsDBNull(4) ? null : reader.GetString(4),
                            TreatmentGiven = reader.IsDBNull(5) ? null : reader.GetString(5),
                            DoctorID = reader.GetInt32(6),
                            Status = Enum.Parse<EmergencyCaseStatus>(reader.GetString(7)),
                            Notes = reader.IsDBNull(8) ? null : reader.GetString(8)
                        };
                    }
                }
            }
            return null;
        }

        // Get medicine by Id
        public Medicine GetMedicineById(int medicineId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.medicines WHERE medicine_id = @medicine_id", connection);
                command.Parameters.AddWithValue("@medicine_id", medicineId);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Medicine
                    {
                        MedicineId = (int)reader["medicine_id"],
                        MedicineName = (string)reader["medicine_name"],
                        GenericName = (string)reader["generic_name"],
                        BrandName = reader["brand_name"] as string,
                        DosageForm = (string)reader["dosage_form"],
                        Strength = (string)reader["strength"],
                        Manufacturer = (string)reader["manufacturer"],
                        ExpiryDate = (DateTime)reader["expiry_date"],
                        UnitPrice = (decimal)reader["unit_price"],
                        StockQuantity = (int)reader["stock_quantity"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    };
                }
            }

            return null;
        }

        public List<Medicine> GetMedicineList()
        {
            List<Medicine> medicines = new List<Medicine>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.medicines", connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var medicine = new Medicine
                        {
                            MedicineId = (int)reader["medicine_id"],
                            MedicineName = (string)reader["medicine_name"],
                            GenericName = (string)reader["generic_name"],
                            BrandName = reader["brand_name"] as string,
                            DosageForm = (string)reader["dosage_form"],
                            Strength = (string)reader["strength"],
                            Manufacturer = (string)reader["manufacturer"],
                            ExpiryDate = (DateTime)reader["expiry_date"],
                            UnitPrice = (decimal)reader["unit_price"],
                            StockQuantity = (int)reader["stock_quantity"],
                            CreatedAt = (DateTime)reader["created_at"],
                            UpdatedAt = (DateTime)reader["updated_at"]
                        };

                        medicines.Add(medicine);
                    }
                }
            }

            return medicines;
        }


        // Get pharmacy order by Id
        public PharmacyOrder GetPharmacyOrderById(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.pharmacy_orders WHERE order_id = @order_id", connection);
                command.Parameters.AddWithValue("@order_id", orderId);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new PharmacyOrder
                    {
                        OrderId = (int)reader["order_id"],
                        MedicineId = (int)reader["medicine_id"],
                        OrderQuantity = (int)reader["order_quantity"],
                        OrderDate = (DateTime)reader["order_date"],
                        OrderStatus = (string)reader["order_status"],
                        SupplierName = (string)reader["supplier_name"],
                        TotalCost = (decimal)reader["total_cost"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    };
                }
            }

            return null;
        }

        // Get pharmacy stock by Id
        public PharmacyStock GetPharmacyStockById(int stockId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.pharmacy_stock WHERE stock_id = @stock_id", connection);
                command.Parameters.AddWithValue("@stock_id", stockId);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new PharmacyStock
                    {
                        StockId = (int)reader["stock_id"],
                        MedicineId = (int)reader["medicine_id"],
                        StockQuantity = (int)reader["stock_quantity"],
                        StockInDate = (DateTime)reader["stock_in_date"],
                        StockOutDate = reader["stock_out_date"] as DateTime?,
                        StockBalance = (int)reader["stock_balance"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    };
                }
            }

            return null;
        }

        // Get prescription by Id
        public PharmacyPrescriptionEntity GetPrescriptionById(int prescriptionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM PharmacyManagement.prescriptions WHERE prescription_id = @prescription_id", connection);
                command.Parameters.AddWithValue("@prescription_id", prescriptionId);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new PharmacyPrescriptionEntity
                    {
                        PrescriptionId = (int)reader["prescription_id"],
                        PatientId = (int)reader["patient_id"],
                        DoctorId = (int)reader["doctor_id"],
                        PrescriptionDate = (DateTime)reader["prescription_date"],
                        TotalAmount = (decimal)reader["total_amount"],
                        Status = (string)reader["status"],
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    };
                }
            }

            return null;
        }

        public List<PharmacyPrescriptionEntity> GetPrescriptionsByPatientId(int patientId)
        {
            var prescriptions = new List<PharmacyPrescriptionEntity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM PharmacyManagement.prescriptions WHERE patient_id = @patient_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@patient_id", patientId);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var prescription = new PharmacyPrescriptionEntity
                    {
                        PrescriptionId = (int)reader["prescription_id"],
                        PatientId = (int)reader["patient_id"],
                        DoctorId = (int)reader["doctor_id"],
                        PrescriptionDate = (DateTime)reader["prescription_date"],
                        TotalAmount = (decimal)reader["total_amount"],
                        Status = reader["status"].ToString(),
                        CreatedAt = (DateTime)reader["created_at"],
                        UpdatedAt = (DateTime)reader["updated_at"]
                    };

                    prescriptions.Add(prescription);
                }
            }

            return prescriptions;
        }

        public void UpdateAmbulance(Ambulance ambulance)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE EmergencyAmbulanceManagement.ambulances
            SET AmbulanceNumber = @AmbulanceNumber
            WHERE AmbulanceID = @AmbulanceID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AmbulanceID", ambulance.AmbulanceID);
                cmd.Parameters.AddWithValue("@AmbulanceNumber", ambulance.AmbulanceNumber ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateAmbulanceRequest(AmbulanceRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE EmergencyAmbulanceManagement.ambulance_requests
            SET PatientID = @PatientID,
                RequestTime = @RequestTime,
                PickupLocation = @PickupLocation,
                Destination = @Destination,
                AmbulanceID = @AmbulanceID,
                Status = @Status,
                AssignedDriver = @AssignedDriver,
                Notes = @Notes
            WHERE RequestID = @RequestID";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@RequestID", request.RequestID);
                cmd.Parameters.AddWithValue("@PatientID", request.PatientID);
                cmd.Parameters.AddWithValue("@RequestTime", request.RequestTime);
                cmd.Parameters.AddWithValue("@PickupLocation", request.PickupLocation);
                cmd.Parameters.AddWithValue("@Destination", request.Destination);
                cmd.Parameters.AddWithValue("@AmbulanceID", (object?)request.AmbulanceID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", request.Status.ToString()); // Convert enum to string
                cmd.Parameters.AddWithValue("@AssignedDriver", request.AssignedDriver);
                cmd.Parameters.AddWithValue("@Notes", (object?)request.Notes ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateEmergencyCase(EmergencyCase emergencyCase)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE EmergencyAmbulanceManagement.emergency_cases
            SET PatientID = @PatientID,
                ArrivalTime = @ArrivalTime,
                SeverityLevel = @SeverityLevel,
                Diagnosis = @Diagnosis,
                TreatmentGiven = @TreatmentGiven,
                DoctorID = @DoctorID,
                Status = @Status,
                Notes = @Notes
            WHERE CaseID = @CaseID";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@CaseID", emergencyCase.CaseID);
                cmd.Parameters.AddWithValue("@PatientID", emergencyCase.PatientID);
                cmd.Parameters.AddWithValue("@ArrivalTime", emergencyCase.ArrivalTime);
                cmd.Parameters.AddWithValue("@SeverityLevel", emergencyCase.SeverityLevel.ToString()); // assuming string storage
                cmd.Parameters.AddWithValue("@Diagnosis", (object?)emergencyCase.Diagnosis ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TreatmentGiven", (object?)emergencyCase.TreatmentGiven ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DoctorID", emergencyCase.DoctorID);
                cmd.Parameters.AddWithValue("@Status", emergencyCase.Status.ToString()); // assuming string storage
                cmd.Parameters.AddWithValue("@Notes", (object?)emergencyCase.Notes ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        // Update medicine details
        public void UpdateMedicine(Medicine medicine)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE PharmacyManagement.medicines SET medicine_name = @medicine_name, generic_name = @generic_name, brand_name = @brand_name, dosage_form = @dosage_form, strength = @strength, manufacturer = @manufacturer, expiry_date = @expiry_date, unit_price = @unit_price, stock_quantity = @stock_quantity, updated_at = @updated_at WHERE medicine_id = @medicine_id", connection);

                command.Parameters.AddWithValue("@medicine_id", medicine.MedicineId);
                command.Parameters.AddWithValue("@medicine_name", medicine.MedicineName);
                command.Parameters.AddWithValue("@generic_name", medicine.GenericName);
                command.Parameters.AddWithValue("@brand_name", medicine.BrandName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@dosage_form", medicine.DosageForm);
                command.Parameters.AddWithValue("@strength", medicine.Strength);
                command.Parameters.AddWithValue("@manufacturer", medicine.Manufacturer);
                command.Parameters.AddWithValue("@expiry_date", medicine.ExpiryDate);
                command.Parameters.AddWithValue("@unit_price", medicine.UnitPrice);
                command.Parameters.AddWithValue("@stock_quantity", medicine.StockQuantity);
                command.Parameters.AddWithValue("@updated_at", medicine.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update pharmacy order details
        public void UpdatePharmacyOrder(PharmacyOrder order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE PharmacyManagement.pharmacy_orders SET medicine_id = @medicine_id, order_quantity = @order_quantity, order_date = @order_date, order_status = @order_status, supplier_name = @supplier_name, total_cost = @total_cost, updated_at = @updated_at WHERE order_id = @order_id", connection);

                command.Parameters.AddWithValue("@order_id", order.OrderId);
                command.Parameters.AddWithValue("@medicine_id", order.MedicineId);
                command.Parameters.AddWithValue("@order_quantity", order.OrderQuantity);
                command.Parameters.AddWithValue("@order_date", order.OrderDate);
                command.Parameters.AddWithValue("@order_status", order.OrderStatus);
                command.Parameters.AddWithValue("@supplier_name", order.SupplierName);
                command.Parameters.AddWithValue("@total_cost", order.TotalCost);
                command.Parameters.AddWithValue("@updated_at", order.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update pharmacy stock entry details 
        public void UpdatePharmacyStock(PharmacyStock stock)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE PharmacyManagement.pharmacy_stock SET medicine_id = @medicine_id, stock_quantity = @stock_quantity, stock_in_date = @stock_in_date, stock_out_date = @stock_out_date,stock_balance=@stock_balance, updated_at = @updated_at WHERE stock_id = @stock_id", connection);

                command.Parameters.AddWithValue("@stock_id", stock.StockId);
                command.Parameters.AddWithValue("@medicine_id", stock.MedicineId);
                command.Parameters.AddWithValue("@stock_quantity", stock.StockQuantity);
                command.Parameters.AddWithValue("@stock_in_date", stock.StockInDate);
                command.Parameters.AddWithValue("@stock_out_date", (object)stock.StockOutDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@stock_balance", stock.StockBalance);
                command.Parameters.AddWithValue("@updated_at", stock.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update prescription details
        public void UpdatePrescription(PharmacyPrescriptionEntity prescription)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE PharmacyManagement.prescriptions SET patient_id = @patient_id, doctor_id = @doctor_id, prescription_date = @prescription_date, total_amount = @total_amount, status = @status, updated_at = @updated_at WHERE prescription_id = @prescription_id", connection);

                command.Parameters.AddWithValue("@prescription_id", prescription.PrescriptionId);
                command.Parameters.AddWithValue("@patient_id", prescription.PatientId);
                command.Parameters.AddWithValue("@doctor_id", prescription.DoctorId);
                command.Parameters.AddWithValue("@prescription_date", prescription.PrescriptionDate);
                command.Parameters.AddWithValue("@total_amount", prescription.TotalAmount);
                command.Parameters.AddWithValue("@status", prescription.Status);
                command.Parameters.AddWithValue("@updated_at", prescription.UpdatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
