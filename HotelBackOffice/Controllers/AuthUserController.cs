using HotelBackOffice.Models.Dto;
using HotelBackOffice.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace HotelBackOffice.Controllers
{
    [Authorize]
    public class AuthUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthUserController(
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSave(RegisterDto registerDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser user = new AppUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = registerDto.Email,
                        UserName = registerDto.Email,
                        Nome = registerDto.Name,
                        Cognome = registerDto.Surname,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                    };

                    IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

                    if (result.Succeeded)
                    {
                        string[] roleNames = { "Amministratore", "Receptionist", "Visualizzatore" };

                        foreach (var roleName in roleNames)
                        {
                            var roleExist = await this._roleManager.RoleExistsAsync(roleName);
                            if (!roleExist)
                            {
                                await this._roleManager.CreateAsync(new IdentityRole(roleName));
                            }
                        }

                        if (registerDto.Email == "alandonati7@gmail.com")
                        {
                            await this._userManager.AddToRoleAsync(user, "Amministratore");
                        }
                        else
                        {
                            await this._userManager.AddToRoleAsync(user, "Visualizzatore");
                        }

                        return RedirectToAction("Login");
                    }
                    else
                    {

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View("Register");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginRequest(LoginRequest login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser user = await this._userManager.FindByEmailAsync(login.Email);

                    if (user is not null)
                    {
                        Microsoft.AspNetCore.Identity.SignInResult result = await this._signInManager.PasswordSignInAsync(
                            user,
                            login.Password,
                            isPersistent: false,
                            lockoutOnFailure: false
                            );

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
