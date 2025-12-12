
using HotelBackOffice.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {
        private readonly AppDbContext _context;

        public ClientiController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> Index()
        {
            var clienti = await _context.Clienti
                .OrderBy(c => c.Cognome)
                .ThenBy(c => c.Nome)
                .ToListAsync();

            return View(clienti);
        }


        [Authorize(Roles = "Amministratore,Receptionist,Visualizzatore")]
        public async Task<IActionResult> GetDetailsPartial(int id)
        {
            var cliente = await _context.Clienti
                .Include(c => c.Prenotazioni)
                    .ThenInclude(p => p.Camera)
                .FirstOrDefaultAsync(m => m.ClienteId == id);

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
        public async Task<IActionResult> CreateCliente([Bind("Nome,Cognome,Email,Telefono")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var esisteEmail = await _context.Clienti
                    .AnyAsync(c => c.Email == cliente.Email);

                if (esisteEmail)
                {
                    ModelState.AddModelError("Email", "Esiste già un cliente con questa email");
                    return PartialView("_CreatePartial", cliente);
                }

                _context.Add(cliente);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cliente creato con successo!" });
            }

            return PartialView("_CreatePartial", cliente);
        }


        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var cliente = await _context.Clienti.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return PartialView("_EditPartial", cliente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> EditCliente([Bind("ClienteId,Nome,Cognome,Email,Telefono")] Cliente cliente)
        {
    
            ModelState.Remove("Prenotazioni");

            if (ModelState.IsValid)
            {
                var esisteEmail = await _context.Clienti
                    .AnyAsync(c => c.Email == cliente.Email && c.ClienteId != cliente.ClienteId);

                if (esisteEmail)
                {
                    ModelState.AddModelError("Email", "Esiste già un cliente con questa email");
                    return PartialView("_EditPartial", cliente);
                }

                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cliente modificato con successo!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.ClienteId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return PartialView("_EditPartial", cliente);
        }


        [Authorize(Roles = "Amministratore,Receptionist")]
        public async Task<IActionResult> GetDeletePartial(int id)
        {
            var cliente = await _context.Clienti
                .Include(c => c.Prenotazioni)
                .FirstOrDefaultAsync(m => m.ClienteId == id);

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
            var cliente = await _context.Clienti
                .Include(c => c.Prenotazioni)
                .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (cliente == null)
            {
                return Json(new { success = false, message = "Cliente non trovato" });
            }

            if (cliente.Prenotazioni != null && cliente.Prenotazioni.Any())
            {
                return Json(new { success = false, message = "Impossibile eliminare il cliente. Ci sono prenotazioni associate." });
            }

            _context.Clienti.Remove(cliente);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cliente eliminato con successo!" });
        }

        private bool ClienteExists(int id)
        {
            return _context.Clienti.Any(e => e.ClienteId == id);
        }
    }
}