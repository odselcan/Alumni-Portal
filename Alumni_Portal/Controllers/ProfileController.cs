using Alumni_Portal.Data;
using Alumni_Portal.Models;
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

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            // Admin ise users tablosundan profil göster
            if (User.IsInRole("admin"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var adminUser = await _context.UserAccounts.FindAsync(userId);
                if (adminUser == null) return NotFound();

                var adminModel = new ProfileViewModel
                {
                    FirstName = adminUser.FirstName,
                    LastName  = adminUser.LastName,
                    Email     = adminUser.Email,
                    Phone     = adminUser.Phone
                };
                ViewBag.IsAdmin = true;
                return View(adminModel);
            }

            // Mezun ise graduate tablosundan profil göster
            var graduateId = GetGraduateId();
            if (graduateId == 0) return RedirectToAction("Index", "Login");

            var graduate = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .FirstOrDefaultAsync(g => g.GraduateId == graduateId);

            if (graduate == null) return NotFound();

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
                Position         = graduate.GraduateCareer?.Position,
                Sector           = graduate.GraduateCareer?.Sector,
                StartYear        = graduate.GraduateCareer?.StartYear
            };

            ViewBag.IsAdmin = false;
            return View(model);
        }

        // POST: /Profile/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Admin profil güncelleme
            if (User.IsInRole("admin"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var adminUser = await _context.UserAccounts.FindAsync(userId);
                if (adminUser == null) return NotFound();

                adminUser.Phone     = model.Phone;
                adminUser.UpdateDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            // Mezun profil güncelleme
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
            graduate.GraduateCareer.Position         = model.Position;
            graduate.GraduateCareer.Sector           = model.Sector;
            graduate.GraduateCareer.StartYear        = model.StartYear;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        private int GetGraduateId()
        {
            var claim = User.FindFirst("GraduateId");
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}