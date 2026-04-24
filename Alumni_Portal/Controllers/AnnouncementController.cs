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
    public class AnnouncementController : Controller
    {
        private readonly AppDbContext _context;

        public AnnouncementController(AppDbContext context)
        {
            _context = context;
        }
         // GET: /Announcement/Detail/5
public async Task<IActionResult> Detail(int id)
{
    var announcement = await _context.Announcements
        .FirstOrDefaultAsync(a => a.AnnouncementId == id && a.IsActive);

    if (announcement == null) return NotFound();

    return View(announcement);
}
        // GET: /Announcement/Index
        public async Task<IActionResult> Index()
        {
            var announcements = await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();

            return View(announcements);
        }

        // GET: /Announcement/Create
        public IActionResult Create()
        {
            return View(new AnnouncementViewModel());
        }

        // POST: /Announcement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var announcement = new Announcement
            {
                Title      = model.Title,
                Content    = model.Content,
                IsActive   = model.IsActive,
                CreatedBy  = userId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        // GET: /Announcement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            var model = new AnnouncementViewModel
            {
                AnnouncementId = announcement.AnnouncementId,
                Title          = announcement.Title,
                Content        = announcement.Content,
                IsActive       = announcement.IsActive,
                CreateDate     = announcement.CreateDate
            };

            return View(model);
        }

        // POST: /Announcement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            announcement.Title      = model.Title;
            announcement.Content    = model.Content;
            announcement.IsActive   = model.IsActive;
            announcement.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // POST: /Announcement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            announcement.IsActive   = false;
            announcement.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru silindi.";
            return RedirectToAction("Index");
        }
    }
}