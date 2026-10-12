using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alumni_Portal.Models
{
    [Table("messages", Schema = "belek_graduate")]
    public class Message
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sender_id")]
        public int SenderId { get; set; }

        [Column("receiver_id")]
        public int ReceiverId { get; set; }

        [Column("content")]
        public string Content { get; set; } = null!;

        [Column("sent_at")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Column("is_read")]
        public bool IsRead { get; set; } = false;
    }
}