using HotelBackOffice.Models.Entity;
using HotelBackOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBackOffice.Controllers
{
    [Authorize]
    public class CamereController : Controller
    {
        private readonly CamereService _camereService;

        public CamereController(CamereService camereService)
        {
            _camereService = camereService;
        }

        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> Index()
        {
            var camere = await _camereService.GetAllCamereAsync();
            return View(camere);
        }

        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> GetDetailsPartial(int id)
        {
            var camera = await _camereService.GetCameraWithPrenotazioniAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", camera);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public IActionResult GetCreatePartial()
        {
            return PartialView("_CreatePartial", new Camera());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> CreateCamera(Camera camera)
        {
            ModelState.Remove("Prenotazioni");

            if (ModelState.IsValid)
            {
                if (await _camereService.NumeroExistsAsync(camera.Numero))
                {
                    ModelState.AddModelError("Numero", "Esiste già una camera con questo numero");
                    return PartialView("_CreatePartial", camera);
                }

                var success = await _camereService.CreateCameraAsync(camera);
                if (success)
                {
                    return Json(new { success = true, message = "Camera creata con successo!" });
                }
            }

            return PartialView("_CreatePartial", camera);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var camera = await _camereService.GetCameraByIdAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", camera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> EditCamera(Camera camera)
        {
            ModelState.Remove("Prenotazioni");

            if (ModelState.IsValid)
            {
                if (await _camereService.NumeroExistsAsync(camera.Numero, camera.CameraId))
                {
                    ModelState.AddModelError("Numero", "Esiste già una camera con questo numero");
                    return PartialView("_EditPartial", camera);
                }

                var success = await _camereService.UpdateCameraAsync(camera);
                if (success)
                {
                    return Json(new { success = true, message = "Camera modificata con successo!" });
                }
            }

            return PartialView("_EditPartial", camera);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetDeletePartial(int id)
        {
            var camera = await _camereService.GetCameraWithPrenotazioniAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return PartialView("_DeletePartial", camera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            if (await _camereService.CameraHasPrenotazioniAsync(id))
            {
                return Json(new { success = false, message = "Impossibile eliminare la camera. Ci sono prenotazioni associate." });
            }

            var success = await _camereService.DeleteCameraAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Camera eliminata con successo!" });
            }

            return Json(new { success = false, message = "Camera non trovata" });
        }
    }
}