using Microsoft.EntityFrameworkCore;
using ChemicalSDS.Data;
using ChemicalSDS.Models;

namespace ChemicalSDS.Services
{
    public class UserAuthService
    {
        private const int BcryptWorkFactor = 10;

        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public UserAuthService(AppDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        public static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password, BcryptWorkFactor);

        public static bool VerifyPassword(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            // Do not block login on a second DB round-trip
            _ = RecordLastLoginAsync(user.Id);

            return user;
        }

        private async Task RecordLastLoginAsync(int userId)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Users
                    .Where(u => u.Id == userId)
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastLoginDate, DateTime.UtcNow));
            }
            catch
            {
                // Non-critical; login must not fail if this update fails
            }
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
