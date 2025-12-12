using HotelBackOffice.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Services
{
    public class PrenotazioniService
    {
        private readonly AppDbContext _context;

        public PrenotazioniService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prenotazione>> GetAllPrenotazioniAsync()
        {
            return await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .OrderByDescending(p => p.DataInizio)
                .ToListAsync();
        }

        public async Task<Prenotazione?> GetPrenotazioneByIdAsync(int id)
        {
            return await _context.Prenotazioni.FindAsync(id);
        }

        public async Task<Prenotazione?> GetPrenotazioneWithDetailsAsync(int id)
        {
            return await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .FirstOrDefaultAsync(p => p.PrenotazioneId == id);
        }

        public async Task<bool> CreatePrenotazioneAsync(Prenotazione prenotazione)
        {
            _context.Prenotazioni.Add(prenotazione);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePrenotazioneAsync(Prenotazione prenotazione)
        {
            _context.Prenotazioni.Update(prenotazione);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePrenotazioneAsync(int id)
        {
            var prenotazione = await GetPrenotazioneByIdAsync(id);
            if (prenotazione == null) return false;

            _context.Prenotazioni.Remove(prenotazione);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsCameraDisponibileAsync(int cameraId, DateTime dataInizio, DateTime dataFine, int? excludePrenotazioneId = null)
        {
            var query = _context.Prenotazioni
                .Where(p => p.CameraId == cameraId &&
                           p.Stato != "Cancellata" &&
                           ((p.DataInizio <= dataInizio && p.DataFine > dataInizio) ||
                            (p.DataInizio < dataFine && p.DataFine >= dataFine) ||
                            (p.DataInizio >= dataInizio && p.DataFine <= dataFine)));

            if (excludePrenotazioneId.HasValue)
            {
                query = query.Where(p => p.PrenotazioneId != excludePrenotazioneId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<List<Cliente>> GetAllClientiAsync()
        {
            return await _context.Clienti.ToListAsync();
        }

        public async Task<List<Camera>> GetAllCamereAsync()
        {
            return await _context.Camere.ToListAsync();
        }
    }
}