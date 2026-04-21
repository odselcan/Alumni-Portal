using Alumni_Portal.Data;
using Alumni_Portal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni_Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalGraduates = await _context.Graduates
                .CountAsync(g => g.IsActive);
            var employed = await _context.GraduateCareers
                .CountAsync(c => c.EmploymentStatus == "employed");
            var jobSeeking = await _context.GraduateCareers
                .CountAsync(c => c.EmploymentStatus == "job_seeking");
            var academic = await _context.GraduateCareers
                .CountAsync(c => c.EmploymentStatus == "academic");

            // Doğum günleri
            var today = DateTime.Today;
            var birthdays = await _context.Graduates
                .Where(g => g.IsActive
                         && g.BirthDate != null
                         && g.BirthDate.Value.Month == today.Month
                         && g.BirthDate.Value.Day == today.Day)
                .Select(g => new BirthdayViewModel
                {
                    FullName  = g.FirstName + " " + g.LastName,
                    BirthDate = g.BirthDate!.Value,
                    DaysLeft  = 0
                })
                .ToListAsync();

            // Duyurular
            var announcements = await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();

            ViewBag.TotalGraduates = totalGraduates;
            ViewBag.Employed       = employed;
            ViewBag.JobSeeking     = jobSeeking;
            ViewBag.Academic       = academic;
            ViewBag.Birthdays      = birthdays;
            ViewBag.Announcements  = announcements;
            ViewBag.UserName       = User.Identity!.Name;

            return View();
        }
    }
}