using HotelBackOffice.Models.Dto;
using HotelBackOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBackOffice.Controllers
{
    [Authorize(Roles = "Amministratore")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllUsersWithRolesAsync();
            return View(users);
        }

        public IActionResult GetCreatePartial()
        {
            return PartialView("_CreatePartial", new CreateDipendenteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDipendente(CreateDipendenteDto model)
        {
            if (ModelState.IsValid)
            {
                var (success, errors) = await _adminService.CreateUserAsync(model);

                if (success)
                {
                    return Json(new { success = true, message = "Dipendente creato con successo!" });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return PartialView("_CreatePartial", model);
        }

        public async Task<IActionResult> GetEditPartial(string id)
        {
            var model = await _adminService.GetUserForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDipendente(EditDipendenteDto model)
        {
            if (ModelState.IsValid)
            {
                var (success, errors) = await _adminService.UpdateUserAsync(model);

                if (success)
                {
                    return Json(new { success = true, message = "Dipendente modificato con successo!" });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return PartialView("_EditPartial", model);
        }

        public async Task<IActionResult> GetDeletePartial(string id)
        {
            var model = await _adminService.GetUserForDeleteAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("_DeletePartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDipendente(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (success, message) = await _adminService.DeleteUserAsync(id, currentUserId);

            return Json(new { success, message });
        }
    }
}