using Alumni_Portal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alumni_Portal.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Chat/Index
        public async Task<IActionResult> Index(int? userId)
        {
            var myIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (myIdStr == null) return RedirectToAction("Index", "Login");
            var myId = int.Parse(myIdStr);

            // Tüm mezunlar (admin hariç, kendisi hariç)
            var graduates = await _context.UserAccounts
                .Where(u => u.UserType == "graduate" && u.IsActive && u.Id != myId)
                .OrderBy(u => u.FirstName)
                .ToListAsync();

            // Seçili kullanıcı
            Models.UserAccount? selectedUser = null;
            List<Models.Message> messages = new();

            if (userId.HasValue)
            {
                selectedUser = await _context.UserAccounts.FindAsync(userId.Value);

                messages = await _context.Messages
                    .Where(m => (m.SenderId == myId && m.ReceiverId == userId.Value) ||
                                (m.SenderId == userId.Value && m.ReceiverId == myId))
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                // Okundu işaretle
                var unread = messages.Where(m => m.ReceiverId == myId && !m.IsRead).ToList();
                foreach (var m in unread) m.IsRead = true;
                if (unread.Any()) await _context.SaveChangesAsync();
            }

            // Okunmamış mesaj sayıları
            var unreadCounts = await _context.Messages
                .Where(m => m.ReceiverId == myId && !m.IsRead)
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SenderId, x => x.Count);

            ViewBag.MyId         = myId;
            ViewBag.Graduates    = graduates;
            ViewBag.SelectedUser = selectedUser;
            ViewBag.Messages     = messages;
            ViewBag.UnreadCounts = unreadCounts;

            return View();
        }
    }
}