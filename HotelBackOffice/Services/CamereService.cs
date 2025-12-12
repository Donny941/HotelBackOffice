using HotelBackOffice.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Services
{
    public class CamereService
    {
        private readonly AppDbContext _context;

        public CamereService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Camera>> GetAllCamereAsync()
        {
            return await _context.Camere
                .OrderBy(c => c.Numero)
                .ToListAsync();
        }

        public async Task<Camera?> GetCameraByIdAsync(int id)
        {
            return await _context.Camere.FindAsync(id);
        }

        public async Task<Camera?> GetCameraWithPrenotazioniAsync(int id)
        {
            return await _context.Camere
                .Include(c => c.Prenotazioni)
                    .ThenInclude(p => p.Cliente)
                .FirstOrDefaultAsync(c => c.CameraId == id);
        }

        public async Task<bool> CreateCameraAsync(Camera camera)
        {
            _context.Camere.Add(camera);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCameraAsync(Camera camera)
        {
            _context.Camere.Update(camera);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCameraAsync(int id)
        {
            var camera = await GetCameraByIdAsync(id);
            if (camera == null) return false;

            _context.Camere.Remove(camera);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> NumeroExistsAsync(string numero, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Camere
                    .AnyAsync(c => c.Numero == numero && c.CameraId != excludeId.Value);
            }
            return await _context.Camere.AnyAsync(c => c.Numero == numero);
        }

        public async Task<bool> CameraHasPrenotazioniAsync(int id)
        {
            return await _context.Camere
                .Where(c => c.CameraId == id)
                .SelectMany(c => c.Prenotazioni)
                .AnyAsync();
        }
    }
}