using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alumni_Portal.Models
{
    [Table("password_reset_tokens", Schema = "public")]
    public class PasswordResetToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("platform_user_id")]
        public int PlatformUserId { get; set; }

        [Column("token")]
        public string Token { get; set; } = null!;

        [Column("hashed_token")]
        public string? HashedToken { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; } = false;

        [Column("create_date")]
        public DateTime? CreateDate { get; set; }

        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }
    }
}