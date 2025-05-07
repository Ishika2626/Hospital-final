using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly string _connectionString;

        public BillingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddInsuranceClaim(InsuranceIntegration claim)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO BillingPayment.insurance_integration 
                            (patient_id, insurance_provider, claim_number, claim_amount, claim_status, settlement_amount, settlement_date) 
                             VALUES (@PatientId, @InsuranceProvider, @ClaimNumber, @ClaimAmount, @ClaimStatus, @SettlementAmount, @SettlementDate)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", claim.PatientId);
                    cmd.Parameters.AddWithValue("@InsuranceProvider", claim.InsuranceProvider);
                    cmd.Parameters.AddWithValue("@ClaimNumber", claim.ClaimNumber);
                    cmd.Parameters.AddWithValue("@ClaimAmount", claim.ClaimAmount);
                    cmd.Parameters.AddWithValue("@ClaimStatus", claim.ClaimStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SettlementAmount", claim.SettlementAmount);
                    cmd.Parameters.AddWithValue("@SettlementDate", (object)claim.SettlementDate ?? DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }




        public void AddInvoice(Invoice invoice)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                decimal totalAmount = invoice.Amount - invoice.Discount + invoice.Tax;

                string query = @"INSERT INTO BillingPayment.invoicing 
                         (patient_id, description, amount, discount, tax, invoice_date, status, total_amount, due_date) 
                         VALUES 
                         (@PatientId, @Description, @Amount, @Discount, @Tax, @InvoiceDate, @Status, @TotalAmount, @DueDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", invoice.PatientId);
                cmd.Parameters.AddWithValue("@Description", invoice.Description);
                cmd.Parameters.AddWithValue("@Amount", invoice.Amount);
                cmd.Parameters.AddWithValue("@Discount", invoice.Discount);
                cmd.Parameters.AddWithValue("@Tax", invoice.Tax);
                cmd.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                cmd.Parameters.AddWithValue("@Status", invoice.Status);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@DueDate", invoice.DueDate);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateInvoice(Invoice invoice)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Validate InvoiceDate and DueDate
                if (invoice.InvoiceDate < new DateTime(1753, 1, 1) || invoice.InvoiceDate > new DateTime(9999, 12, 31))
                {
                    // Set to a default valid date if outside SQL Server's valid range
                    invoice.InvoiceDate = DateTime.Now;  // Default to current date
                }

                if (invoice.DueDate < new DateTime(1753, 1, 1) || invoice.DueDate > new DateTime(9999, 12, 31))
                {
                    // Set to a default valid date if outside SQL Server's valid range
                    invoice.DueDate = DateTime.Now;  // Default to current date
                }

                string query = @"UPDATE BillingPayment.invoicing SET 
            patient_id = @PatientId,
            description = @Description,
            amount = @Amount,
            discount = @Discount,
            tax = @Tax,
            invoice_date = @InvoiceDate,
            status = @Status,
            total_amount = @TotalAmount,
            due_date = @DueDate,
            razorpay_order_id = @RazorpayOrderId
        WHERE invoice_id = @InvoiceId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InvoiceId", invoice.InvoiceId);
                cmd.Parameters.AddWithValue("@PatientId", invoice.PatientId);
                cmd.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                cmd.Parameters.AddWithValue("@DueDate", invoice.DueDate);
                cmd.Parameters.AddWithValue("@Amount", invoice.Amount);
                cmd.Parameters.AddWithValue("@Discount", invoice.Discount);
                cmd.Parameters.AddWithValue("@Tax", invoice.Tax);
                cmd.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                cmd.Parameters.AddWithValue("@Description", invoice.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", invoice.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RazorpayOrderId", invoice.RazorpayOrderId ?? (object)DBNull.Value); // Razorpay order ID

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }



        public void AddPayment(PaymentTracking payment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO BillingPayment.payment_tracking (patient_id, invoice_id, payment_date, payment_amount, payment_method, payment_status, transaction_id) 
                         VALUES (@PatientId, @InvoiceId, @PaymentDate, @PaymentAmount, @PaymentMethod, @PaymentStatus, @TransactionId)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", payment.PatientId);
                cmd.Parameters.AddWithValue("@InvoiceId", payment.InvoiceId);
                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                cmd.Parameters.AddWithValue("@PaymentAmount", payment.PaymentAmount);
                cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
                cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
                cmd.Parameters.AddWithValue("@TransactionId", payment.TransactionId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void DeleteInsuranceClaim(int claimId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM BillingPayment.InsuranceIntegration WHERE ClaimId = @ClaimId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClaimId", claimId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteInvoice(int invoiceId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // First, delete the related records in payment_tracking
                string deleteTrackingQuery = "DELETE FROM BillingPayment.payment_tracking WHERE invoice_id = @InvoiceId";
                SqlCommand deleteTrackingCmd = new SqlCommand(deleteTrackingQuery, conn);
                deleteTrackingCmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                conn.Open();
                deleteTrackingCmd.ExecuteNonQuery();

                // Then, delete the invoice record
                string deleteInvoiceQuery = "DELETE FROM BillingPayment.invoicing WHERE invoice_id = @InvoiceId";
                SqlCommand deleteInvoiceCmd = new SqlCommand(deleteInvoiceQuery, conn);
                deleteInvoiceCmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                deleteInvoiceCmd.ExecuteNonQuery();
            }
        }



        public void DeletePayment(int paymentId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM BillingPayment.payment_tracking WHERE payment_id = @PaymentId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentId", paymentId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public IEnumerable<InsuranceIntegration> GetAllInsuranceClaims()
        {
            List<InsuranceIntegration> claims = new List<InsuranceIntegration>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BillingPayment.insurance_integration";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        claims.Add(new InsuranceIntegration
                        {
                            InsuranceClaimId = (int)reader["insurance_claim_id"],
                            PatientId = (int)reader["patient_id"],
                            InsuranceProvider = reader["insurance_provider"]?.ToString(),
                            ClaimNumber = reader["claim_number"]?.ToString(),
                            ClaimAmount = (decimal)reader["claim_amount"],
                            ClaimStatus = reader["claim_status"]?.ToString(),
                            SettlementAmount = (decimal)reader["settlement_amount"],
                            SettlementDate = reader["settlement_date"] == DBNull.Value ? null : (DateTime?)reader["settlement_date"]
                        });
                    }
                }
            }

            return claims;
        }


        public IEnumerable<Invoice> GetAllInvoices()
        {
            List<Invoice> invoices = new List<Invoice>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BillingPayment.invoicing";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Log column names to debug
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine(reader.GetName(i));
                    }

                    while (reader.Read())
                    {
                        // Handle possible null values using GetValue and DBNull
                        invoices.Add(new Invoice
                        {
                            InvoiceId = (int)reader["invoice_id"],
                            PatientId = (int)reader["patient_id"],
                            Description = reader["description"].ToString(),
                            Amount = (decimal)reader["amount"],
                            Discount = (decimal)reader["discount"],
                            Tax = (decimal)reader["tax"],
                           
                            InvoiceDate =  (DateTime)reader["invoice_date"],
                            DueDate =  (DateTime)reader["due_date"],
                            Status = reader["status"].ToString()
                        });
                    }
                }
            }

            return invoices;
        }


        public IEnumerable<PaymentTracking> GetAllPayments()
        {
            List<PaymentTracking> payments = new List<PaymentTracking>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BillingPayment.payment_tracking";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new PaymentTracking
                        {
                            PaymentId = (int)reader["payment_id"],
                            PatientId = (int)reader["patient_id"],
                            InvoiceId = (int)reader["invoice_id"],
                            PaymentDate = (DateTime)reader["payment_date"],
                            PaymentAmount = (decimal)reader["payment_amount"],
                            PaymentMethod = reader["payment_method"].ToString(),
                            PaymentStatus = reader["payment_status"].ToString(),
                            TransactionId = reader["transaction_id"].ToString()
                        });
                    }
                }
            }

            return payments;
        }

        public List<SelectListItem> GetClaimStatusList()
        {
            List<SelectListItem> statusList = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT StatusId, StatusName FROM BillingPayment.ClaimStatuses"; // Adjust table name and columns as needed
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        statusList.Add(new SelectListItem
                        {
                            Value = reader["StatusId"].ToString(),
                            Text = reader["StatusName"].ToString()
                        });
                    }
                }
            }

            return statusList;
        }


        public InsuranceIntegration GetInsuranceClaimById(int claimId)
        {
            InsuranceIntegration claim = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BillingPayment.InsuranceIntegration WHERE ClaimId = @ClaimId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClaimId", claimId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        claim = new InsuranceIntegration
                        {
                            InsuranceClaimId = (int)reader["ClaimId"],
                            PatientId = (int)reader["PatientId"],
                            InsuranceProvider = reader["InsuranceProvider"].ToString(),
                            ClaimNumber = reader["ClaimNumber"].ToString(),
                            ClaimAmount = (decimal)reader["ClaimAmount"],
                            ClaimStatus = reader["ClaimStatus"].ToString(),
                            SettlementAmount = (decimal)reader["SettlementAmount"],
                            SettlementDate = reader["SettlementDate"] as DateTime?
                        };
                    }
                }
            }

            return claim;
        }


        public List<SelectListItem> GetInsuranceProviders()
        {
            List<SelectListItem> insuranceProviders = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT ProviderId, ProviderName FROM BillingPayment.InsuranceProviders"; // Adjust table and column names if necessary
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        insuranceProviders.Add(new SelectListItem
                        {
                            Value = reader["ProviderId"].ToString(),
                            Text = reader["ProviderName"].ToString()
                        });
                    }
                }
            }

            return insuranceProviders;
        }


        public Invoice GetInvoiceById(int invoiceId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BillingPayment.invoicing WHERE invoice_id = @InvoiceId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Invoice
                        {
                            InvoiceId = (int)reader["invoice_id"],
                            PatientId = (int)reader["patient_id"],
                            InvoiceDate = (DateTime)reader["invoice_date"],
                            Amount = (decimal)reader["Amount"],
                            Discount = (decimal)reader["discount"],
                            Tax = (decimal)reader["tax"],
                            Description = reader["Description"].ToString(),
                            Status = reader["Status"].ToString()
                        };
                    }
                }
            }
            return null;
        }


        public List<SelectListItem> GetInvoiceList()
        {
            var items = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT invoice_id, description FROM BillingPayment.invoicing";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Value = reader["invoice_id"].ToString(),
                            Text = $"{reader["invoice_id"]} - {reader["description"]}"
                        });
                    }
                }
            }

            return items;
        }


        public List<SelectListItem> GetPatientList()
        {
            List<SelectListItem> patients = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT PatientId, PatientName FROM patients"; // Adjust table and column names if needed
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        patients.Add(new SelectListItem
                        {
                            Value = reader["PatientId"].ToString(),
                            Text = reader["PatientName"].ToString()
                        });
                    }
                }
            }

            return patients;
        }

        public PaymentTracking GetPaymentById(int paymentId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM BillingPayment.PaymentTracking WHERE PaymentId = @PaymentId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentId", paymentId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PaymentTracking
                        {
                            PaymentId = (int)reader["PaymentId"],
                            PatientId = (int)reader["PatientId"],
                            InvoiceId = (int)reader["InvoiceId"],
                            PaymentDate = (DateTime)reader["PaymentDate"],
                            PaymentAmount = (decimal)reader["PaymentAmount"],
                            PaymentMethod = reader["PaymentMethod"].ToString(),
                            PaymentStatus = reader["PaymentStatus"].ToString(),
                            TransactionId = reader["TransactionId"].ToString()
                        };
                    }
                }
            }
            return null; // Return null if no payment was found
        }

        public List<SelectListItem> GetPaymentMethods()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "Cash", Text = "Cash" },
        new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
        new SelectListItem { Value = "DebitCard", Text = "Debit Card" },
        new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" }
    };
        }


        public List<SelectListItem> GetPaymentStatusList()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "Pending", Text = "Pending" },
        new SelectListItem { Value = "Completed", Text = "Completed" },
        new SelectListItem { Value = "Failed", Text = "Failed" },
        new SelectListItem { Value = "Refunded", Text = "Refunded" }
    };
        }


        public void UpdateInsuranceClaim(InsuranceIntegration claim)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE BillingPayment.insurance_integration SET
    patient_id = @PatientId,
    insurance_provider = @InsuranceProvider,
    claim_number = @ClaimNumber,
    claim_amount = @ClaimAmount,
    claim_status = @ClaimStatus,
    settlement_amount = @SettlementAmount,
    settlement_date = @SettlementDate
    WHERE insurance_claim_id = @ClaimId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClaimId", claim.InsuranceClaimId);
                cmd.Parameters.AddWithValue("@PatientId", claim.PatientId);
                cmd.Parameters.AddWithValue("@InsuranceProvider", claim.InsuranceProvider);
                cmd.Parameters.AddWithValue("@ClaimNumber", claim.ClaimNumber);
                cmd.Parameters.AddWithValue("@ClaimAmount", claim.ClaimAmount);
                cmd.Parameters.AddWithValue("@ClaimStatus", claim.ClaimStatus);
                cmd.Parameters.AddWithValue("@SettlementAmount", claim.SettlementAmount);
                cmd.Parameters.AddWithValue("@SettlementDate", (object)claim.SettlementDate ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


      

        public void UpdatePayment(PaymentTracking payment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // SQL query to update payment information
                string query = @"
            UPDATE BillingPayment.payment_tracking 
            SET 
                patient_id = @PatientId,
                invoice_id = @InvoiceId,
                payment_date = @PaymentDate,
                payment_amount = @PaymentAmount,
                payment_method = @PaymentMethod,
                payment_status = @PaymentStatus,
                transaction_id = @TransactionId
            WHERE payment_id = @PaymentId";

                // Create and configure SQL command
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentId", payment.PaymentId);
                cmd.Parameters.AddWithValue("@PatientId", payment.PatientId);
                cmd.Parameters.AddWithValue("@InvoiceId", payment.InvoiceId);
                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                cmd.Parameters.AddWithValue("@PaymentAmount", payment.PaymentAmount);
                cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
                cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
                cmd.Parameters.AddWithValue("@TransactionId", payment.TransactionId);

                // Open connection and execute the command
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void MarkInvoiceAsPaid(string razorpayOrderId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE BillingPayment.invoicing 
            SET status = 'Paid', updated_at = GETDATE()
            WHERE razorpay_order_id = @razorpayOrderId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@razorpayOrderId", razorpayOrderId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Invoice GetInvoiceByRazorpayOrderId(string razorpayOrderId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT invoice_id, patient_id, invoice_date, description, amount, discount, tax, total_amount, razorpay_order_id, status
                         FROM BillingPayment.invoicing
                         WHERE razorpay_order_id = @razorpayOrderId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@razorpayOrderId", razorpayOrderId);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Invoice
                        {
                            InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                            PatientId = Convert.ToInt32(reader["patient_id"]),
                            InvoiceDate = Convert.ToDateTime(reader["invoice_date"]),
                            Description = reader["description"].ToString(),
                            Amount = Convert.ToDecimal(reader["amount"]),
                            Discount = Convert.ToDecimal(reader["discount"]),
                            Tax = Convert.ToDecimal(reader["tax"]),
                            
                            RazorpayOrderId = reader["razorpay_order_id"].ToString(),
                            Status = reader["status"].ToString()
                        };
                    }
                }
            }
            return null; // Invoice not found
        }

    }
}
