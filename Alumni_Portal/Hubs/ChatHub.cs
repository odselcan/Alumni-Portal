using Alumni_Portal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alumni_Portal.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(int receiverId, string content)
        {
            var senderIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (senderIdStr == null) return;

            var senderId = int.Parse(senderIdStr);

            var message = new Models.Message
            {
                SenderId   = senderId,
                ReceiverId = receiverId,
                Content    = content,
                SentAt     = DateTime.UtcNow,
                IsRead     = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var sender = await _context.UserAccounts.FindAsync(senderId);
            var senderName = $"{sender?.FirstName} {sender?.LastName}";

            // Alıcıya gönder
            await Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", new
            {
                id         = message.Id,
                senderId,
                senderName,
                content,
                sentAt     = message.SentAt.ToString("HH:mm"),
                isRead     = false
            });

            // Gönderene de gönder (kendi mesajını görsün)
            await Clients.Group($"user_{senderId}").SendAsync("ReceiveMessage", new
            {
                id         = message.Id,
                senderId,
                senderName,
                content,
                sentAt     = message.SentAt.ToString("HH:mm"),
                isRead     = false
            });
        }

        public async Task MarkAsRead(int senderId)
        {
            var receiverIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (receiverIdStr == null) return;

            var receiverId = int.Parse(receiverIdStr);

            var unread = await _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            foreach (var m in unread) m.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}