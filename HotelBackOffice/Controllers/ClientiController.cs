using HotelBackOffice.Models.Entity;
using HotelBackOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBackOffice.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {
        private readonly ClientiService _clientiService;

        public ClientiController(ClientiService clientiService)
        {
            _clientiService = clientiService;
        }

        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> Index()
        {
            var clienti = await _clientiService.GetAllClientiAsync();
            return View(clienti);
        }

        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> GetDetailsPartial(int id)
        {
            var cliente = await _clientiService.GetClienteWithPrenotazioniAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", cliente);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public IActionResult GetCreatePartial()
        {
            return PartialView("_CreatePartial", new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> CreateCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                if (await _clientiService.EmailExistsAsync(cliente.Email))
                {
                    ModelState.AddModelError("Email", "Esiste già un cliente con questa email");
                    return PartialView("_CreatePartial", cliente);
                }

                var success = await _clientiService.CreateClienteAsync(cliente);
                if (success)
                {
                    return Json(new { success = true, message = "Cliente creato con successo!" });
                }
            }

            return PartialView("_CreatePartial", cliente);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var cliente = await _clientiService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> EditCliente(Cliente cliente)
        {
            ModelState.Remove("Prenotazioni");

            if (ModelState.IsValid)
            {
                if (await _clientiService.EmailExistsAsync(cliente.Email, cliente.ClienteId))
                {
                    ModelState.AddModelError("Email", "Esiste già un cliente con questa email");
                    return PartialView("_EditPartial", cliente);
                }

                var success = await _clientiService.UpdateClienteAsync(cliente);
                if (success)
                {
                    return Json(new { success = true, message = "Cliente modificato con successo!" });
                }
            }

            return PartialView("_EditPartial", cliente);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetDeletePartial(int id)
        {
            var cliente = await _clientiService.GetClienteWithPrenotazioniAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return PartialView("_DeletePartial", cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            if (await _clientiService.ClienteHasPrenotazioniAsync(id))
            {
                return Json(new { success = false, message = "Impossibile eliminare il cliente. Ci sono prenotazioni associate." });
            }

            var success = await _clientiService.DeleteClienteAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Cliente eliminato con successo!" });
            }

            return Json(new { success = false, message = "Cliente non trovato" });
        }
    }
}