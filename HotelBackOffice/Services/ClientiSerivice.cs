using HotelBackOffice.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace HotelBackOffice.Services
{
    public class ClientiService
    {
        private readonly AppDbContext _context;

        public ClientiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllClientiAsync()
        {
            return await _context.Clienti
                .OrderBy(c => c.Cognome)
                .ThenBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return await _context.Clienti.FindAsync(id);
        }

        public async Task<Cliente?> GetClienteWithPrenotazioniAsync(int id)
        {
            return await _context.Clienti
                .Include(c => c.Prenotazioni)
                    .ThenInclude(p => p.Camera)
                .FirstOrDefaultAsync(c => c.ClienteId == id);
        }

        public async Task<bool> CreateClienteAsync(Cliente cliente)
        {
            _context.Clienti.Add(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateClienteAsync(Cliente cliente)
        {
            _context.Clienti.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await GetClienteByIdAsync(id);
            if (cliente == null) return false;

            _context.Clienti.Remove(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Clienti
                    .AnyAsync(c => c.Email == email && c.ClienteId != excludeId.Value);
            }
            return await _context.Clienti.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> ClienteHasPrenotazioniAsync(int id)
        {
            return await _context.Clienti
                .Where(c => c.ClienteId == id)
                .SelectMany(c => c.Prenotazioni)
                .AnyAsync();
        }
    }
}