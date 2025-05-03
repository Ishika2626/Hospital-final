using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HospitalManagementSystem.Repositories
{
    public interface IBedsRepository
    {
        // Beds
        IEnumerable<Beds> GetAllBeds();
        Beds GetBedById(int bedId);
        void AddBed(Beds bed);
        void UpdateBed(Beds bed);
        void DeleteBed(int bedId);

        // Rooms
        IEnumerable<Rooms> GetAllRooms();
        Rooms GetRoomById(int roomId);
        void AddRoom(Rooms room, IFormFile room_img);
        void UpdateRoom(Rooms room, IFormFile room_img);
        void DeleteRoom(int roomId);

        // Patient Bed Allocation
        IEnumerable<Patient_Bed_Allocation> GetAllPatientBedAllocations();
        Patient_Bed_Allocation GetPatientBedAllocationById(int allocationId);
        void AddPatientBedAllocation(Patient_Bed_Allocation allocation);
        void UpdatePatientBedAllocation(Patient_Bed_Allocation allocation);
        void DeletePatientBedAllocation(int allocationId);

        // Bed Transfer
        IEnumerable<Bed_Transfer> GetAllBedTransfers();
        Bed_Transfer GetBedTransferById(int transferId);
        void AddBedTransfer(Bed_Transfer transfer);
        void UpdateBedTransfer(Bed_Transfer transfer);
        void DeleteBedTransfer(int transferId);

        // Bed Maintenance
        IEnumerable<Bed_Maintenance> GetAllBedMaintenances();
        Bed_Maintenance GetBedMaintenanceById(int maintenanceId);
        void AddBedMaintenance(Bed_Maintenance maintenance);
        void UpdateBedMaintenance(Bed_Maintenance maintenance);
        void DeleteBedMaintenance(int maintenanceId);

            // Bed Occupancy History
            IEnumerable<Bed_Occupancy_History> GetAllBedOccupancyHistories();
            Bed_Occupancy_History GetBedOccupancyHistoryById(int historyId);
            void AddBedOccupancyHistory(Bed_Occupancy_History history);
            void UpdateBedOccupancyHistory(Bed_Occupancy_History history);
            void DeleteBedOccupancyHistory(int historyId);

        // Room Occupancy
        IEnumerable<Room_Occupancy> GetAllRoomOccupancies();
        Room_Occupancy GetRoomOccupancyById(int roomOccupancyId);
        void AddRoomOccupancy(Room_Occupancy occupancy);
        void UpdateRoomOccupancy(Room_Occupancy occupancy);
        void DeleteRoomOccupancy(int roomOccupancyId);

        // Bed Reservation
        IEnumerable<Bed_Reservation> GetAllBedReservations();
        Bed_Reservation GetBedReservationById(int reservationId);
        void AddBedReservation(Bed_Reservation reservation);
        void UpdateBedReservation(Bed_Reservation reservation);
        void DeleteBedReservation(int reservationId);

        List<SelectListItem> GetRoomTypes();
    }
}

 