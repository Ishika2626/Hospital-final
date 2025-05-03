using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class RolesController : Controller
    {
        private static IStaffRepository _staffRepository;

        public RolesController(IStaffRepository _staffRepository)
        {
            _staffRepository = _staffRepository;
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                _staffRepository.AddRole(role);
                return RedirectToAction(nameof(Index)); // Refresh the page after adding the role
            }
            return View(role); // Return the view with validation errors
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Role role)
        {
            if (id != role.RoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _staffRepository.UpdateRole(role);
                return RedirectToAction(nameof(Index)); // Refresh the page after editing the role
            }
            return View(role); // Return the view with validation errors
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _staffRepository.DeleteRole(id);
            return RedirectToAction(nameof(Index)); // Refresh the page after deleting the role
        }
    }
}
