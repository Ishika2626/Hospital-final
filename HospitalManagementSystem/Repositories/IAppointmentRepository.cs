using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HospitalManagementSystem.Repositories
{
    public interface IAppointmentRepository
    {
        // Appointment
        IEnumerable<Appointment> GetAllAppointments();
        Appointment GetAppointmentById(int appointmentId);
        void AddAppointment(Appointment appointment);
        void UpdateAppointment(Appointment appointment);
        void DeleteAppointment(int appointmentId);

        // Doctor Availability
        IEnumerable<DoctorAvailability> GetAllDoctorAvailabilities();
        DoctorAvailability GetDoctorAvailabilityById(int availabilityId);
        void AddDoctorAvailability(DoctorAvailability availability);
        void UpdateDoctorAvailability(DoctorAvailability availability);
        void DeleteDoctorAvailability(int availabilityId);

        // Appointment Alerts
        IEnumerable<AppointmentAlert> GetAllAppointmentAlerts();
        AppointmentAlert GetAppointmentAlertById(int alertId);
        void AddAppointmentAlert(AppointmentAlert alert);
        void UpdateAppointmentAlert(AppointmentAlert alert);
        void DeleteAppointmentAlert(int alertId);

        // Helper Methods for Dropdowns or Select Lists
        List<SelectListItem> GetDoctorList();
        List<SelectListItem> GetPatientList();
        List<SelectListItem> GetAppointmentStatusList();

        List<SelectListItem> GetAppointmentId();


        List<Appointment> GetAppointmentsByPatientId(int patientId);
        List<AppointmentInfo> GetAppointmentsByPatientName(string patientName);

    }
}
