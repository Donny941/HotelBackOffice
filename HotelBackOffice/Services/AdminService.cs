using HotelBackOffice.Models.Dto;
using HotelBackOffice.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Services
{
    public class AdminService
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserWithRoleDto>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
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

            return userWithRoles;
        }

        public async Task<AppUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<(bool success, List<string> errors)> CreateUserAsync(CreateDipendenteDto model)
        {
            var errors = new List<string>();

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                errors.Add("Esiste già un utente con questa email");
                return (false, errors);
            }

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
                await _userManager.AddToRoleAsync(user, model.Ruolo);
                return (true, errors);
            }

            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return (false, errors);
        }

        public async Task<(bool success, List<string> errors)> UpdateUserAsync(EditDipendenteDto model)
        {
            var errors = new List<string>();
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                errors.Add("Utente non trovato");
                return (false, errors);
            }

            user.Nome = model.Nome;
            user.Cognome = model.Cognome;
            user.Email = model.Email;
            user.UserName = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return (false, errors);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Ruolo);

            return (true, errors);
        }

        public async Task<(bool success, string message)> DeleteUserAsync(string id, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return (false, "Utente non trovato");
            }

            if (currentUserId == user.Id)
            {
                return (false, "Non puoi eliminare il tuo account!");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return (true, "Dipendente eliminato con successo!");
            }

            return (false, "Errore durante l'eliminazione");
        }

        public async Task<EditDipendenteDto?> GetUserForEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new EditDipendenteDto
            {
                Id = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                Cognome = user.Cognome,
                Ruolo = roles.FirstOrDefault() ?? "Visualizzatore"
            };
        }

        public async Task<UserWithRoleDto?> GetUserForDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserWithRoleDto
            {
                Id = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                Cognome = user.Cognome,
                Ruolo = roles.FirstOrDefault() ?? "Nessun ruolo"
            };
        }
    }
}