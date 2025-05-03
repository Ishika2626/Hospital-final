using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Repositories
{
    public interface IPatientRepository
    {
        IEnumerable<PatientRegistration> GetAll();
        PatientRegistration GetById(int patient_id);
        void Add(PatientRegistration patient, IFormFile patient_img);
        void Update(PatientRegistration patient, IFormFile patient_img);
        void Delete(int patient_id);

        //admission

        IEnumerable<patient_admission> GetAllpatient_admission();
        patient_admission Getpatient_admissionById(int admission_id);
        void Addpatient_admission(patient_admission admission);
        void Updatepatient_admission(patient_admission admission);
        void Deletepatient_admission(int admission_id);

        //patient_records
        IEnumerable<patient_records> GetAllpatient_records();
        patient_records Getpatient_recordsById(int record_id);
        void Addpatient_records(patient_records records);
        void Updatepatient_records(patient_records records);
        void Deletepatient_records(int record_id);

        //insurance
        IEnumerable<patient_insurance> GetAllpatient_insurance();
        patient_insurance Getpatient_insuranceById(int insurance_id);
        void Addpatient_insurance(patient_insurance insurance);
        void Updatepatient_insurance(patient_insurance insurance);
        void Deletepatient_insurance(int insurance_id);

        //login
        PatientRegistration Authenticate(string email, string password);

        // Patient Documents
        IEnumerable<patient_documents> GetAllpatient_documents();
        patient_documents Getpatient_documentById(int document_id);
        void Addpatient_document(patient_documents document, IFormFile document_file, int uploadedBy);
        void Updatepatient_document(patient_documents document, IFormFile document_file, int uploadedBy);
        void Deletepatient_document(int document_id);

        //room bed
        List<SelectListItem> GetRoomTypes();
        List<SelectListItem> GetBedTypes();
        List<SelectListItem> GetPatientName();
        public string GetPatientNameById(int patientId);

        //visits
        IEnumerable<patient_visits> GetAllpatient_visits();
        patient_visits Getpatient_visitsById(int visit_id);
        void Addpatient_visits(patient_visits visit);
        void Updatepatient_visits(patient_visits visit);
        void Deletepatient_visits(int visit_id);



        PatientRegistration GetPatientByEmail(string email);  // Get patient by email
        void UpdatePassword(int patientId, string newPassword);  // Update password

        IEnumerable<patient_documents> GetDocumentsByPatientId(int patientId);

     
        }
}
