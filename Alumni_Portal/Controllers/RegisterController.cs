using Alumni_Portal.Data;
using Alumni_Portal.Models;
using Alumni_Portal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Alumni_Portal.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<List<Faculty>> GetFacultiesWithDepartments()
        {
            return await _context.Faculties
                .Where(f => f.IsActive)
                .Include(f => f.Departments)
                .OrderBy(f => f.Id)
                .ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.Faculties = await GetFacultiesWithDepartments();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = await GetFacultiesWithDepartments();
                return View(model);
            }

            // ASIL KONTROL: Graduate tablosunda email var mı?
            // (users tablosundaki kontrolü kaldırdık — orphan kayıt olabilir, onu reuse edeceğiz)
            bool graduateEmailExists = await _context.Graduates
                .AnyAsync(g => g.Email == model.Email);

            if (graduateEmailExists)
            {
                ViewBag.Faculties = await GetFacultiesWithDepartments();
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            DateTime? graduationDate = null;
            if (model.GraduationYear.HasValue)
            {
                int month = model.GraduationSemester == "bahar" ? 6 : 12;
                graduationDate = new DateTime(model.GraduationYear.Value, month, 1);
            }

            // 1. Graduate kaydını ekle
            var graduate = new Graduate
            {
                FirstName      = model.FirstName,
                LastName       = model.LastName,
                Email          = model.Email,
                BirthDate      = model.BirthDate,
                Faculty        = model.Faculty,
                Department     = model.Department,
                GraduationDate = graduationDate,
                CreatedDate    = DateTime.UtcNow,
                UpdatedDate    = DateTime.UtcNow,
                IsActive       = false
            };

            _context.Graduates.Add(graduate);
            await _context.SaveChangesAsync();

            int? createdBy = null;
            try
            {
                var schemaEntry = await _context.UserSchemas.FirstOrDefaultAsync();
                createdBy = schemaEntry?.UserId;
            }
            catch { }

            // 2. users tablosunda orphan kayıt var mı kontrol et
            var existingUser = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                // ORPHAN KAYIT — daha önce mezunluğu silinmiş ama users'ta kalmış
                // Yeniden kullan (UPDATE)
                existingUser.Username     = model.Email.Split('@')[0];
                existingUser.FirstName    = model.FirstName;
                existingUser.LastName     = model.LastName;
                existingUser.PasswordHash = HashPassword(model.Password);
                existingUser.UserType     = "graduate";
                existingUser.IsActive     = true;
                existingUser.UpdatedBy    = createdBy;
                existingUser.UpdateDate   = DateTime.UtcNow;

                _context.UserAccounts.Update(existingUser);
                await _context.SaveChangesAsync();

                // user_auth tablosunda da orphan varsa onu da güncelle
                try
                {
                    var existingAuth = await _context.UserAuths
                        .FirstOrDefaultAsync(a => a.UserId == existingUser.Id);

                    if (existingAuth != null)
                    {
                        existingAuth.PasswordHash = HashPassword(model.Password);
                        _context.UserAuths.Update(existingAuth);
                        await _context.SaveChangesAsync();
                    }
                }
                catch { /* user_auth yoksa veya yetki yoksa sessiz geç */ }
            }
            else
            {
                // Yeni kullanıcı — normal akış
                var userAccount = new UserAccount
                {
                    Username     = model.Email.Split('@')[0],
                    FirstName    = model.FirstName,
                    LastName     = model.LastName,
                    Email        = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    UserType     = "graduate",
                    IsActive     = true,
                    CreatedBy    = createdBy,
                    CreateDate   = DateTime.UtcNow,
                    UpdateDate   = DateTime.UtcNow
                };

                _context.UserAccounts.Add(userAccount);
                await _context.SaveChangesAsync();
            }

            TempData["Pending"] = "Kayıt talebiniz alındı. Admin onayından sonra giriş yapabilirsiniz.";
            return RedirectToAction("Index", "Login");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}