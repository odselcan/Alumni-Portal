using Alumni_Portal.Data;
using Alumni_Portal.Models;
using Alumni_Portal.Services;
using Alumni_Portal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alumni_Portal.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public ProfileController(AppDbContext context, EmailService emailService)
        {
            _context      = context;
            _emailService = emailService;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("admin"))
            {
                var userId    = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var adminUser = await _context.UserAccounts.FindAsync(userId);
                if (adminUser == null) return NotFound();

                var adminModel = new ProfileViewModel
                {
                    FirstName       = adminUser.FirstName,
                    LastName        = adminUser.LastName,
                    Email           = adminUser.Email,
                    Phone           = adminUser.Phone,
                    ProfileImageUrl = adminUser.ProfileImageUrl
                };
                ViewBag.IsAdmin = true;
                return View(adminModel);
            }

            var graduateId = GetGraduateId();
            if (graduateId == 0) return RedirectToAction("Index", "Login");

            var graduate = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .FirstOrDefaultAsync(g => g.GraduateId == graduateId);

            if (graduate == null) return NotFound();

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == graduate.Email);

            var model = new ProfileViewModel
            {
                FirstName        = graduate.FirstName,
                LastName         = graduate.LastName,
                Email            = graduate.Email,
                Phone            = graduate.Phone,
                Faculty          = graduate.Faculty,
                Department       = graduate.Department,
                GraduationDate   = graduate.GraduationDate,
                EmploymentStatus = graduate.GraduateCareer?.EmploymentStatus,
                CompanyName      = graduate.GraduateCareer?.CompanyName,
                Sector           = graduate.GraduateCareer?.Sector,
                Profession       = graduate.GraduateCareer?.Profession,
                Position         = graduate.GraduateCareer?.Position,
                StartYear        = graduate.GraduateCareer?.StartYear,
                ProfileImageUrl  = userAccount?.ProfileImageUrl
            };

            // Cascading dropdown için ViewBag verileri
            await LoadCareerDropdownsAsync(graduate.Faculty, model.Sector, model.Profession);

            ViewBag.IsAdmin = false;
            return View(model);
        }

        // POST: /Profile/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCareerDropdownsAsync(model.Faculty, model.Sector, model.Profession);
                return View(model);
            }

            if (User.IsInRole("admin"))
            {
                var userId    = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var adminUser = await _context.UserAccounts.FindAsync(userId);
                if (adminUser == null) return NotFound();

                adminUser.Phone      = model.Phone;
                adminUser.UpdateDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            var graduateId = GetGraduateId();
            if (graduateId == 0) return RedirectToAction("Index", "Login");

            var graduate = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .FirstOrDefaultAsync(g => g.GraduateId == graduateId);

            if (graduate == null) return NotFound();

            graduate.Email = model.Email;
            graduate.Phone = model.Phone;

            if (graduate.GraduateCareer == null)
            {
                var maxCareerId = await _context.GraduateCareers
                    .MaxAsync(c => (int?)c.CareerId) ?? 0;

                graduate.GraduateCareer = new GraduateCareer
                {
                    CareerId    = maxCareerId + 1,
                    GraduateId  = graduateId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsActive    = true
                };
                _context.GraduateCareers.Add(graduate.GraduateCareer);
            }

            graduate.GraduateCareer.EmploymentStatus = model.EmploymentStatus;
            graduate.GraduateCareer.CompanyName      = model.CompanyName;
            graduate.GraduateCareer.Sector           = model.Sector;
            graduate.GraduateCareer.Profession       = model.Profession;
            graduate.GraduateCareer.Position         = model.Position;
            graduate.GraduateCareer.StartYear        = model.StartYear;
            graduate.GraduateCareer.UpdatedDate      = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // POST: /Profile/UploadPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Lütfen bir resim seçin.";
                return RedirectToAction("Index");
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext     = Path.GetExtension(photo.FileName).ToLower();
            if (!allowed.Contains(ext))
            {
                TempData["Error"] = "Sadece JPG, PNG veya WEBP yükleyebilirsiniz.";
                return RedirectToAction("Index");
            }

            if (photo.Length > 2 * 1024 * 1024)
            {
                TempData["Error"] = "Dosya boyutu 2MB'dan küçük olmalıdır.";
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user   = await _context.UserAccounts.FindAsync(userId);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                    user.ProfileImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            var fileName = $"user_{userId}_{Guid.NewGuid():N}{ext}";
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
                await photo.CopyToAsync(stream);

            user.ProfileImageUrl = $"/uploads/{fileName}";
            user.UpdateDate      = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil fotoğrafı güncellendi.";
            return RedirectToAction("Index");
        }

        // GET: /Profile/ChangePassword
        public IActionResult ChangePassword() => View();

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user   = await _context.UserAccounts.FindAsync(userId);
            if (user == null) return NotFound();

            var currentHash = Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(model.CurrentPassword)
                )
            ).ToLower();

            if (user.PasswordHash != currentHash)
            {
                ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                return View(model);
            }

            user.PasswordHash = Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(model.NewPassword)
                )
            ).ToLower();
            user.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Index");
        }

        // ============================================================
        // YARDIMCI METOTLAR
        // ============================================================

        /// <summary>
        /// Cascading dropdown için ViewBag verilerini yükler.
        /// Sektörler fakülteye göre, Meslekler seçili sektörün fakültesine göre,
        /// Pozisyonlar seçili mesleğin fakültesine göre yüklenir.
        /// </summary>
        private async Task LoadCareerDropdownsAsync(
            string? faculty,
            string? selectedSector,
            string? selectedProfession)
        {
            if (string.IsNullOrEmpty(faculty))
            {
                ViewBag.Sectors     = new List<CareerSector>();
                ViewBag.Professions = new List<CareerProfession>();
                ViewBag.Positions   = new List<CareerPosition>();
                return;
            }

            // Sektörler → fakülteye göre
            ViewBag.Sectors = await _context.CareerSectors
                .Where(s => s.Faculty == faculty)
                .OrderBy(s => s.Name)
                .ToListAsync();

            // Meslekler → sektör seçildiyse yükle
            if (!string.IsNullOrEmpty(selectedSector))
            {
                var sector = await _context.CareerSectors
                    .FirstOrDefaultAsync(s => s.Name == selectedSector && s.Faculty == faculty);

                ViewBag.SelectedSectorId = sector?.Id;

                ViewBag.Professions = await _context.CareerProfessions
                    .Where(p => p.Faculty == faculty)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            else
            {
                ViewBag.Professions = new List<CareerProfession>();
            }

            // Pozisyonlar → meslek seçildiyse yükle
            if (!string.IsNullOrEmpty(selectedProfession))
            {
                var profession = await _context.CareerProfessions
                    .FirstOrDefaultAsync(p => p.Name == selectedProfession && p.Faculty == faculty);

                ViewBag.SelectedProfessionId = profession?.Id;

                ViewBag.Positions = await _context.CareerPositions
                    .Where(p => p.Faculty == faculty)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            else
            {
                ViewBag.Positions = new List<CareerPosition>();
            }
        }

        private int GetGraduateId()
        {
            var claim = User.FindFirst("GraduateId");
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}