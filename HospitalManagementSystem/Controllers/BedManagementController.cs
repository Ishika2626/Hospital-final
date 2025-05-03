using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class BedManagementController : Controller
    {
        private readonly IBedsRepository bedRepository;
        public BedManagementController(IBedsRepository bedRepository)
        {
            this.bedRepository = bedRepository;
        }
        
       
        [HttpGet]
        public IActionResult Beds()
        {
            ViewBag.RoomTypes = bedRepository.GetRoomTypes();
            return View();
        }
        [HttpPost]
        public IActionResult Beds(Beds beds)
        {
            
          
            bedRepository.AddBed(beds);
            return RedirectToAction("DisplayBeds");
        }
        public IActionResult DisplayBeds()
        {
            var beds = bedRepository.GetAllBeds();
            return View(beds);
        }
        [HttpGet]
        public IActionResult EditBeds(int id)
        {
            var beds = bedRepository.GetBedById(id);
            if (beds == null)
            {
                return NotFound(); 
            }
            ViewBag.RoomTypes = bedRepository.GetRoomTypes();
       
            return View(beds);
        }

        [HttpPost]
        public IActionResult EditBeds(Beds beds)
        {
            bedRepository.UpdateBed(beds);
            return RedirectToAction("DisplayBeds");
        }

        public IActionResult DeleteBeds(int id)
        {
            bedRepository.DeleteBed(id);

            return RedirectToAction("DisplayBeds");

        }


        [HttpGet]
        public IActionResult Rooms()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Rooms(Rooms rooms, IFormFile room_img)
        {
            bedRepository.AddRoom(rooms,room_img);
            return RedirectToAction("DisplayRooms");
        }
        public IActionResult DisplayRooms()
        {
            var rooms = bedRepository.GetAllRooms();
            return View(rooms);
        }
        [HttpGet]
        public IActionResult EditRooms(int id)
        {
            var rooms = bedRepository.GetRoomById(id);
            if (rooms == null)
            {
                return NotFound();
            }

            return View(rooms);
        }

        [HttpPost]
        public IActionResult EditRooms(Rooms rooms, IFormFile room_img)
        {
            bedRepository.UpdateRoom(rooms, room_img);
            return RedirectToAction("DisplayRooms");
        }

        public IActionResult DeleteRooms(int id)
        {
            bedRepository.DeleteRoom(id);

            return RedirectToAction("DisplayRooms");

        }
        [HttpGet]
        public IActionResult PatientBedAllocation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PatientBedAllocation(Patient_Bed_Allocation pba)
        {
            bedRepository.AddPatientBedAllocation(pba);
            return RedirectToAction("DisplayPatientBedAllocation");
        }
        public IActionResult DisplayPatientBedAllocation()
        {
            var pba = bedRepository.GetAllPatientBedAllocations();
            return View(pba);
        }
        [HttpGet]
        public IActionResult EditPatientBedAllocation(int id)
        {
            var pba = bedRepository.GetPatientBedAllocationById(id);
            if (pba == null)
            {
                return NotFound();
            }

            return View(pba);
        }

        [HttpPost]
        public IActionResult EditPatientBedAllocation(Patient_Bed_Allocation pba)
        {
            bedRepository.UpdatePatientBedAllocation(pba);
            return RedirectToAction("DisplayPatientBedAllocation");
        }

        public IActionResult DeletePatientBedAllocation(int id)
        {
            bedRepository.DeletePatientBedAllocation(id);
            return RedirectToAction("DisplayPatientBedAllocation");
        }
        [HttpGet]
        public IActionResult BedTransfers()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BedTransfers(Bed_Transfer bt)
        {
            bedRepository.AddBedTransfer(bt);
            return RedirectToAction("DisplayBedTransfers");
        }
        public IActionResult DisplayBedTransfers()
        {
            var bt= bedRepository.GetAllBedTransfers(); 
            return View(bt);    
        }
        [HttpGet]
        public IActionResult EditBedTransfers(int id)
        {
            var bt = bedRepository.GetBedTransferById(id);
            if (bt == null)
            {
                return NotFound();
            }

            return View(bt);
        }

        [HttpPost]
        public IActionResult EditBedTransfers(Bed_Transfer bt)
        {
            bedRepository.UpdateBedTransfer(bt);
            return RedirectToAction("DisplayBedTransfers");
        }

        public IActionResult DeleteBedTransfers(int id)
        {
            bedRepository.DeleteBedTransfer(id);
            return RedirectToAction("DisplayBedTransfers");
        }

        [HttpGet]
        public IActionResult BedMaintenance()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BedMaintenance(Bed_Maintenance bm)
        {
            bedRepository.AddBedMaintenance(bm);
            return RedirectToAction("DisplayBedMaintenance");
        }
        public IActionResult DisplayBedMaintenance()
        {
            var bm=bedRepository.GetAllBedMaintenances();
            return View(bm);
        }

        [HttpGet]
        public IActionResult EditBedMaintenance(int id)
        {
            var bm = bedRepository.GetBedMaintenanceById(id);
            if (bm == null)
            {
                return NotFound();
            }

            return View(bm);
        }

        [HttpPost]
        public IActionResult EditBedMaintenance(Bed_Maintenance bm)
        {
            bedRepository.UpdateBedMaintenance(bm);
            return RedirectToAction("DisplayBedMaintenance");
        }

        public IActionResult DeleteBedMaintenance(int id)
        {
            bedRepository.DeleteBedMaintenance(id);
            return RedirectToAction("DisplayBedMaintenance");
        }

        [HttpGet]
        public IActionResult BedOccupancyHistory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BedOccupancyHistory(Bed_Occupancy_History boh)
        {
            bedRepository.AddBedOccupancyHistory(boh);
            return RedirectToAction("DisplayBedOccupancyHistory");
        }
        public IActionResult DisplayBedOccupancyHistory()
        {
           var boh=bedRepository.GetAllBedOccupancyHistories();
            return View(boh);
        }
        [HttpGet]
        public IActionResult EditBedOccupancyHistory(int id)
        {
            var boh = bedRepository.GetBedOccupancyHistoryById(id);
            if (boh == null)
            {
                return NotFound();
            }

            return View(boh);
        }

        [HttpPost]
        public IActionResult EditBedOccupancyHistory(Bed_Occupancy_History boh)
        {
            bedRepository.UpdateBedOccupancyHistory(boh);
            return RedirectToAction("DisplayBedOccupancyHistory");
        }

        public IActionResult DeleteBedOccupancyHistory(int id)
        {
            bedRepository.DeleteBedOccupancyHistory(id);
            return RedirectToAction("DisplayBedOccupancyHistory");
        }

        [HttpGet]
        public IActionResult RoomOccupancy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RoomOccupancy(Room_Occupancy ro)
        {
            bedRepository.AddRoomOccupancy(ro);
            return RedirectToAction("DisplayRoomOccupancy");
        }
        public IActionResult DisplayRoomOccupancy()
        {
            var ro=bedRepository.GetAllRoomOccupancies();
            return View(ro);
        }
        [HttpGet]
        public IActionResult EditRoomOccupancy(int id)
        {
            var ro = bedRepository.GetRoomOccupancyById(id);
            if (ro == null)
            {
                return NotFound();
            }

            return View(ro);
        }

        [HttpPost]
        public IActionResult EditRoomOccupancy(Room_Occupancy ro)
        {
            bedRepository.UpdateRoomOccupancy(ro);
            return RedirectToAction("DisplayRoomOccupancy");
        }

        public IActionResult DeleteRoomOccupancy(int id)
        {
            bedRepository.DeleteRoomOccupancy(id);
            return RedirectToAction("DisplayRoomOccupancy");
        }

        [HttpGet]
        public IActionResult BedReservation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BedReservation(Bed_Reservation brs)
        {
            bedRepository.AddBedReservation(brs);
            return RedirectToAction("DisplayBedReservation");
        }
        public IActionResult DisplayBedReservation()
        {
           var brs=bedRepository.GetAllBedReservations();
            return View(brs);
        }
        [HttpGet]
        public IActionResult EditBedReservation(int id)
        {
            var brs = bedRepository.GetBedReservationById(id);
            if (brs == null)
            {
                return NotFound();
            }

            return View(brs);
        }

        [HttpPost]
        public IActionResult EditBedReservation(Bed_Reservation brs)
        {
            bedRepository.UpdateBedReservation(brs);
            return RedirectToAction("DisplayBedReservation");
        }

        public IActionResult DeleteBedReservation(int id)
        {
            bedRepository.DeleteBedReservation(id);
            return RedirectToAction("DisplayBedReservation");
        }

    }
}
