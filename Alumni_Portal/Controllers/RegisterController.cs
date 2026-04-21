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

        // GET: /Register/Index
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Register/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = await _context.UserAccounts
                .AnyAsync(u => u.Email == model.Email);

            bool graduateEmailExists = await _context.Graduates
                .AnyAsync(g => g.Email == model.Email);

            if (emailExists || graduateEmailExists)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            // 1) Graduate kaydı — admin onayına kadar pasif
            var graduate = new Graduate
            {
                FirstName      = model.FirstName,
                LastName       = model.LastName,
                Email          = model.Email,
                BirthDate      = model.BirthDate,
                Faculty        = model.Faculty,
                Department     = model.Department,
                GraduationDate = model.GraduationDate,
                CreatedDate    = DateTime.UtcNow,
                UpdatedDate    = DateTime.UtcNow,
                IsActive       = false
            };

            _context.Graduates.Add(graduate);
            await _context.SaveChangesAsync();

            // created_by için user_schemas'tan user_id al
            int? createdBy = null;
            try
            {
                var schemaEntry = await _context.UserSchemas
                    .FirstOrDefaultAsync();
                createdBy = schemaEntry?.UserId;
            }
            catch { }

            // 2) Users kaydı
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