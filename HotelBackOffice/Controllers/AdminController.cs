using HotelBackOffice.Models.Dto;
using HotelBackOffice.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Controllers
{
    [Authorize(Roles = "Amministratore")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Lista dipendenti
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            // Aggiungi i ruoli a ogni utente
            var userWithRoles = new List<UserWithRoleDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add(new UserWithRoleDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nome = user.Nome,
                    Cognome = user.Cognome,
                    Ruolo = roles.FirstOrDefault() ?? "Nessun ruolo"
                });
            }

            return View(userWithRoles);
        }

        // GET: Form crea dipendente
        public IActionResult GetCreatePartial()
        {
            return PartialView("_CreatePartial", new CreateDipendenteDto());
        }

        // POST: Crea dipendente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDipendente(CreateDipendenteDto model)
        {
            if (ModelState.IsValid)
            {
                // Verifica che l'email non esista già
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    ModelState.AddModelError("Email", "Esiste già un utente con questa email");
                    return PartialView("_CreatePartial", model);
                }

                // Crea il nuovo utente
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assegna il ruolo
                    await _userManager.AddToRoleAsync(user, model.Ruolo);

                    return Json(new { success = true, message = "Dipendente creato con successo!" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return PartialView("_CreatePartial", model);
        }

        // GET: Form modifica ruolo
        public async Task<IActionResult> GetEditPartial(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditDipendenteDto
            {
                Id = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                Cognome = user.Cognome,
                Ruolo = roles.FirstOrDefault() ?? "Visualizzatore"
            };

            return PartialView("_EditPartial", model);
        }

        // POST: Modifica dipendente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDipendente(EditDipendenteDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                // Aggiorna dati utente
                user.Nome = model.Nome;
                user.Cognome = model.Cognome;
                user.Email = model.Email;
                user.UserName = model.Email;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return PartialView("_EditPartial", model);
                }

                // Aggiorna ruolo
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Ruolo);

                return Json(new { success = true, message = "Dipendente modificato con successo!" });
            }

            return PartialView("_EditPartial", model);
        }

        // GET: Form elimina
        public async Task<IActionResult> GetDeletePartial(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserWithRoleDto
            {
                Id = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                Cognome = user.Cognome,
                Ruolo = roles.FirstOrDefault() ?? "Nessun ruolo"
            };

            return PartialView("_DeletePartial", model);
        }

        // POST: Elimina dipendente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDipendente(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Utente non trovato" });
            }

            // Non permettere di eliminare se stesso
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id == user.Id)
            {
                return Json(new { success = false, message = "Non puoi eliminare il tuo account!" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Dipendente eliminato con successo!" });
            }

            return Json(new { success = false, message = "Errore durante l'eliminazione" });
        }
    }
}