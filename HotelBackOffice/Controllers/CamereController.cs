using HotelBackOffice.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Controllers
{
    public class CamereController : Controller
    {

        private readonly AppDbContext _context;

        public CamereController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> Index()
        {
            var camere = await _context.Camere
                .OrderBy(c => c.Numero)
                .ToListAsync();

            return View(camere);
        }


        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> GetDetailsPartial(int id)
        {
            var camera = await _context.Camere
                .Include(c => c.Prenotazioni)
                    .ThenInclude(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.CameraId == id);

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
                var esisteNumero = await _context.Camere
                    .AnyAsync(c => c.Numero == camera.Numero);

                if (esisteNumero)
                {
                    ModelState.AddModelError("Numero", "Esiste già una camera con questo numero");
                    return PartialView("_CreatePartial", camera);
                }

                _context.Add(camera);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Camera creata con successo!" });
            }

            return PartialView("_CreatePartial", camera);
        }


        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var camera = await _context.Camere.FindAsync(id);
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
                var esisteNumero = await _context.Camere
                    .AnyAsync(c => c.Numero == camera.Numero && c.CameraId != camera.CameraId);

                if (esisteNumero)
                {
                    ModelState.AddModelError("Numero", "Esiste già una camera con questo numero");
                    return PartialView("_EditPartial", camera);
                }

                try
                {
                    _context.Update(camera);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Camera modificata con successo!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CameraExists(camera.CameraId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return PartialView("_EditPartial", camera);
        }

        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetDeletePartial(int id)
        {
            var camera = await _context.Camere
                .Include(c => c.Prenotazioni)
                .FirstOrDefaultAsync(m => m.CameraId == id);

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
            var camera = await _context.Camere
                .Include(c => c.Prenotazioni)
                .FirstOrDefaultAsync(c => c.CameraId == id);

            if (camera == null)
            {
                return Json(new { success = false, message = "Camera non trovata" });
            }

            if (camera.Prenotazioni != null && camera.Prenotazioni.Any())
            {
                return Json(new { success = false, message = "Impossibile eliminare la camera. Ci sono prenotazioni associate." });
            }

            _context.Camere.Remove(camera);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Camera eliminata con successo!" });
        }

        private bool CameraExists(int id)
        {
            return _context.Camere.Any(e => e.CameraId == id);
        }
    }
}