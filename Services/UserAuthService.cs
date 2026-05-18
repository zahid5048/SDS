using Microsoft.EntityFrameworkCore;
using ChemicalSDS.Data;
using ChemicalSDS.Models;

namespace ChemicalSDS.Services
{
    public class UserAuthService
    {
        private readonly AppDbContext _context;

        public UserAuthService(AppDbContext context)
        {
            _context = context;
        }

        public static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public static bool VerifyPassword(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task EnsureAdminSeedAsync()
        {
            if (await _context.Users.AnyAsync())
                return;

            _context.Users.Add(new User
            {
                Username = "admin",
                FullName = "System Administrator",
                Email = "admin@ciwce.gov.pk",
                PasswordHash = HashPassword("admin123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null)
        {
            var query = _context.Users.Where(u => u.Username == username);
            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}
