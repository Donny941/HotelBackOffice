using HotelBackOffice.Models.Entity;
using HotelBackOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBackOffice.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {
        private readonly PrenotazioniService _prenotazioniService;

        public PrenotazioniController(PrenotazioniService prenotazioniService)
        {
            _prenotazioniService = prenotazioniService;
        }

        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> Index()
        {
            var prenotazioni = await _prenotazioniService.GetAllPrenotazioniAsync();
            return View(prenotazioni);
        }

        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> GetDetailsPartial(int id)
        {
            var prenotazione = await _prenotazioniService.GetPrenotazioneWithDetailsAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", prenotazione);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetCreatePartial()
        {
            await LoadDropdowns();
            return PartialView("_CreatePartial", new Prenotazione());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> CreatePrenotazione(Prenotazione prenotazione)
        {
            ModelState.Remove("Cliente");
            ModelState.Remove("Camera");
            ModelState.Remove("CreataDa");

            if (ModelState.IsValid)
            {
                if (prenotazione.DataFine <= prenotazione.DataInizio)
                {
                    ModelState.AddModelError("DataFine", "La data fine deve essere successiva alla data inizio");
                    await LoadDropdowns();
                    return PartialView("_CreatePartial", prenotazione);
                }

                var disponibile = await _prenotazioniService.IsCameraDisponibileAsync(
                    prenotazione.CameraId,
                    prenotazione.DataInizio,
                    prenotazione.DataFine);

                if (!disponibile)
                {
                    ModelState.AddModelError("CameraId", "Camera non disponibile per le date selezionate");
                    await LoadDropdowns();
                    return PartialView("_CreatePartial", prenotazione);
                }

                var success = await _prenotazioniService.CreatePrenotazioneAsync(prenotazione);
                if (success)
                {
                    return Json(new { success = true, message = "Prenotazione creata con successo!" });
                }
            }

            await LoadDropdowns();
            return PartialView("_CreatePartial", prenotazione);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var prenotazione = await _prenotazioniService.GetPrenotazioneByIdAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }

            await LoadDropdowns(prenotazione.ClienteId, prenotazione.CameraId);
            return PartialView("_EditPartial", prenotazione);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> EditPrenotazione(Prenotazione prenotazione)
        {
            ModelState.Remove("Cliente");
            ModelState.Remove("Camera");
            ModelState.Remove("CreataDa");

            if (ModelState.IsValid)
            {
                if (prenotazione.DataFine <= prenotazione.DataInizio)
                {
                    ModelState.AddModelError("DataFine", "La data fine deve essere successiva alla data inizio");
                    await LoadDropdowns(prenotazione.ClienteId, prenotazione.CameraId);
                    return PartialView("_EditPartial", prenotazione);
                }

                var disponibile = await _prenotazioniService.IsCameraDisponibileAsync(
                    prenotazione.CameraId,
                    prenotazione.DataInizio,
                    prenotazione.DataFine,
                    prenotazione.PrenotazioneId);

                if (!disponibile)
                {
                    ModelState.AddModelError("CameraId", "Camera non disponibile per le date selezionate");
                    await LoadDropdowns(prenotazione.ClienteId, prenotazione.CameraId);
                    return PartialView("_EditPartial", prenotazione);
                }

                var success = await _prenotazioniService.UpdatePrenotazioneAsync(prenotazione);
                if (success)
                {
                    return Json(new { success = true, message = "Prenotazione modificata con successo!" });
                }
            }

            await LoadDropdowns(prenotazione.ClienteId, prenotazione.CameraId);
            return PartialView("_EditPartial", prenotazione);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetDeletePartial(int id)
        {
            var prenotazione = await _prenotazioniService.GetPrenotazioneWithDetailsAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }
            return PartialView("_DeletePartial", prenotazione);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> DeletePrenotazione(int id)
        {
            var success = await _prenotazioniService.DeletePrenotazioneAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Prenotazione eliminata con successo!" });
            }
            return Json(new { success = false, message = "Prenotazione non trovata" });
        }

        private async Task LoadDropdowns(int? clienteId = null, int? cameraId = null)
        {
            var clienti = await _prenotazioniService.GetAllClientiAsync();
            var camere = await _prenotazioniService.GetAllCamereAsync();

            ViewBag.Clienti = new SelectList(clienti, "ClienteId", "Cognome", clienteId);
            ViewBag.Camere = new SelectList(camere, "CameraId", "Numero", cameraId);
        }
    }
}