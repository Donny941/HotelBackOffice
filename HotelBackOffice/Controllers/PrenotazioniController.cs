
using HotelBackOffice.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {
        private readonly AppDbContext _context;

        public PrenotazioniController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> Index()
        {
            var prenotazioni = await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .OrderByDescending(p => p.DataInizio)
                .ToListAsync();

            return View(prenotazioni);
        }


        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> GetDetailsPartial(int id)
        {
            var prenotazione = await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .FirstOrDefaultAsync(p => p.PrenotazioneId == id);

            if (prenotazione == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", prenotazione);
        }


        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetCreatePartial()
        {

            ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
            ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");

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
                    ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
                    ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");
                    return PartialView("_CreatePartial", prenotazione);
                }


                var cameraOccupata = await _context.Prenotazioni
                    .AnyAsync(p => p.CameraId == prenotazione.CameraId &&
                                   p.Stato != "Cancellata" &&
                                   ((p.DataInizio <= prenotazione.DataInizio && p.DataFine > prenotazione.DataInizio) ||
                                    (p.DataInizio < prenotazione.DataFine && p.DataFine >= prenotazione.DataFine) ||
                                    (p.DataInizio >= prenotazione.DataInizio && p.DataFine <= prenotazione.DataFine)));

                if (cameraOccupata)
                {
                    ModelState.AddModelError("CameraId", "Camera non disponibile per le date selezionate");
                    ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
                    ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");
                    return PartialView("_CreatePartial", prenotazione);
                }

                _context.Add(prenotazione);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Prenotazione creata con successo!" });
            }

            ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
            ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");
            return PartialView("_CreatePartial", prenotazione);
        }


        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }

            ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome", prenotazione.ClienteId);
            ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero", prenotazione.CameraId);

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
                    ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
                    ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");
                    return PartialView("_EditPartial", prenotazione);
                }

                // Verifica disponibilità camera (escludendo la prenotazione corrente)
                var cameraOccupata = await _context.Prenotazioni
                    .AnyAsync(p => p.CameraId == prenotazione.CameraId &&
                                   p.PrenotazioneId != prenotazione.PrenotazioneId &&
                                   p.Stato != "Cancellata" &&
                                   ((p.DataInizio <= prenotazione.DataInizio && p.DataFine > prenotazione.DataInizio) ||
                                    (p.DataInizio < prenotazione.DataFine && p.DataFine >= prenotazione.DataFine) ||
                                    (p.DataInizio >= prenotazione.DataInizio && p.DataFine <= prenotazione.DataFine)));

                if (cameraOccupata)
                {
                    ModelState.AddModelError("CameraId", "Camera non disponibile per le date selezionate");
                    ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
                    ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");
                    return PartialView("_EditPartial", prenotazione);
                }

                try
                {
                    _context.Update(prenotazione);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Prenotazione modificata con successo!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrenotazioneExists(prenotazione.PrenotazioneId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Clienti = new SelectList(await _context.Clienti.ToListAsync(), "ClienteId", "Cognome");
            ViewBag.Camere = new SelectList(await _context.Camere.ToListAsync(), "CameraId", "Numero");
            return PartialView("_EditPartial", prenotazione);
        }


        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetDeletePartial(int id)
        {
            var prenotazione = await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .FirstOrDefaultAsync(p => p.PrenotazioneId == id);

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
            var prenotazione = await _context.Prenotazioni.FindAsync(id);

            if (prenotazione == null)
            {
                return Json(new { success = false, message = "Prenotazione non trovata" });
            }

            _context.Prenotazioni.Remove(prenotazione);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Prenotazione eliminata con successo!" });
        }

        private bool PrenotazioneExists(int id)
        {
            return _context.Prenotazioni.Any(e => e.PrenotazioneId == id);
        }
    }
}