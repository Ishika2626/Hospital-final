using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Repositories
{
    public interface IPharmacyRepository
    {
        // Medicines CRUD operations
        IEnumerable<Medicine> GetAllMedicines();
        Medicine GetMedicineById(int medicineId);
        void AddMedicine(Medicine medicine);
        void UpdateMedicine(Medicine medicine);
        void DeleteMedicine(int medicineId);

        // Prescriptions CRUD operations
        IEnumerable<PharmacyPrescriptionEntity> GetAllPrescriptions();
        PharmacyPrescriptionEntity GetPrescriptionById(int prescriptionId);
        void AddPrescription(PharmacyPrescriptionEntity prescription);
        void UpdatePrescription(PharmacyPrescriptionEntity prescription);
        void DeletePrescription(int prescriptionId);

        // Pharmacy Orders CRUD operations
        IEnumerable<PharmacyOrder> GetAllPharmacyOrders();
        PharmacyOrder GetPharmacyOrderById(int orderId);
        void AddPharmacyOrder(PharmacyOrder order);
        void UpdatePharmacyOrder(PharmacyOrder order);
        void DeletePharmacyOrder(int orderId);

        // Pharmacy Stock CRUD operations
        IEnumerable<PharmacyStock> GetAllPharmacyStock();
        PharmacyStock GetPharmacyStockById(int stockId);
        void AddPharmacyStock(PharmacyStock stock);
        void UpdatePharmacyStock(PharmacyStock stock);
        void DeletePharmacyStock(int stockId);


        List<Medicine> GetMedicineList();

        List<PharmacyPrescriptionEntity> GetPrescriptionsByPatientId(int patientId);

    }
}
