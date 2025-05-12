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

        // Ambulance Requests CRUD operations
        IEnumerable<AmbulanceRequest> GetAllAmbulanceRequests();
        AmbulanceRequest GetAmbulanceRequestById(int requestId);
        void AddAmbulanceRequest(AmbulanceRequest request);
        void UpdateAmbulanceRequest(AmbulanceRequest request);
        void DeleteAmbulanceRequest(int requestId);

        // Emergency Cases CRUD operations
        IEnumerable<EmergencyCase> GetAllEmergencyCases();
        EmergencyCase GetEmergencyCaseById(int caseId);
        void AddEmergencyCase(EmergencyCase emergencyCase);
        void UpdateEmergencyCase(EmergencyCase emergencyCase);
        void DeleteEmergencyCase(int caseId);

        // Ambulance CRUD operations
        IEnumerable<Ambulance> GetAllAmbulances();
        Ambulance GetAmbulanceById(int ambulanceId);
        void AddAmbulance(Ambulance ambulance);
        void UpdateAmbulance(Ambulance ambulance);
        void DeleteAmbulance(int ambulanceId);

    }
}
