using ChemicalSDS.Data;
using ChemicalSDS.Models;
using Microsoft.EntityFrameworkCore;

namespace ChemicalSDS.Services
{
    public class ChemicalDeletionService
    {
        private readonly AppDbContext _context;

        public ChemicalDeletionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Chemical?> SoftDeleteAsync(int id, string? deletedBy)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null || chemical.IsDeleted)
                return null;

            chemical.IsDeleted = true;
            chemical.DeletedAt = DateTime.UtcNow;
            chemical.DeletedBy = deletedBy;
            await _context.SaveChangesAsync();
            return chemical;
        }

        public async Task<Chemical?> RestoreAsync(int id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null || !chemical.IsDeleted)
                return null;

            chemical.IsDeleted = false;
            chemical.DeletedAt = null;
            chemical.DeletedBy = null;
            await _context.SaveChangesAsync();
            return chemical;
        }

        public async Task<Chemical?> PermanentDeleteAsync(int id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null || !chemical.IsDeleted)
                return null;

            _context.Chemicals.Remove(chemical);
            await _context.SaveChangesAsync();
            return chemical;
        }
    }
}
