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

            // 1) Graduate kaydı
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
    IsActive       = false  // Admin onayına kadar pasif
};

            _context.Graduates.Add(graduate);
            await _context.SaveChangesAsync();

            // Username otomatik oluştur
            var username = model.Email.Split('@')[0];

            // 2) Users kaydı
            var sql = @"
INSERT INTO public.users 
(username, first_name, last_name, email, password_hash, user_type, is_active, create_date, update_date, created_by)
VALUES 
(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8,
(SELECT user_id FROM public.user_schemas WHERE schema_name = current_user))";

            await _context.Database.ExecuteSqlRawAsync(sql,
                username,
                model.FirstName,
                model.LastName,
                model.Email,
                HashPassword(model.Password),
                "graduate",
                true,
                DateTime.UtcNow,
                DateTime.UtcNow);

            TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
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