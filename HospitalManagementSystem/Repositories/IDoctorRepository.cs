using System.Collections.Generic;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagementSystem.Repositories
{
    public interface IDoctorRepository
    {
        // CRUD for Doctor
        IEnumerable<Doctor> GetAllDoctors();
        Doctor GetDoctorById(int doctor_id);
        void AddDoctor(Doctor doctor, IFormFile doctor_img);
        void UpdateDoctor(Doctor doctor, IFormFile doctor_img);
        void DeleteDoctor(int doctor_id);

       
        // CRUD for DoctorSchedule
        IEnumerable<DoctorSchedule> GetDoctorSchedule(int doctor_id);
        void AddDoctorSchedule(DoctorSchedule schedule);
        void UpdateDoctorSchedule(DoctorSchedule schedule);
        void DeleteDoctorSchedule(int schedule_id);

        // CRUD for DoctorAppointment
        IEnumerable<DoctorAppointment> GetAppointmentsByDoctorId(int doctor_id);
        void AddDoctorAppointment(DoctorAppointment appointment);
        void UpdateDoctorAppointment(DoctorAppointment appointment);
        void DeleteDoctorAppointment(int appointment_id);

        // CRUD for DoctorPrescription
        IEnumerable<DoctorPrescription> GetPrescriptionsByAppointmentId(int appointment_id);
        void AddDoctorPrescription(DoctorPrescription prescription);
        void UpdateDoctorPrescription(DoctorPrescription prescription);
        void DeleteDoctorPrescription(int prescription_id);

        // CRUD for DoctorNote
        IEnumerable<DoctorNote> GetNotesByAppointmentId(int appointment_id);
        void AddDoctorNote(DoctorNote note);
        void UpdateDoctorNote(DoctorNote note);
        void DeleteDoctorNote(int note_id);

        // CRUD for DoctorReview
        IEnumerable<DoctorReview> GetReviewsByDoctorId(int doctor_id);
        void AddDoctorReview(DoctorReview review);
        void UpdateDoctorReview(DoctorReview review);
        void DeleteDoctorReview(int review_id);

        // CRUD for DoctorPayment
        IEnumerable<DoctorPayment> GetPaymentsByDoctorId(int doctor_id);
        void AddDoctorPayment(DoctorPayment payment);
        void UpdateDoctorPayment(DoctorPayment payment);
        void DeleteDoctorPayment(int payment_id);
        List<SelectListItem> GetDoctorName();
        public string GetDoctorNameById(int doctorId);

        IEnumerable<Department> GetDepartment();

    }
}
