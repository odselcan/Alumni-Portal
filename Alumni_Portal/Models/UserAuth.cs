namespace Alumni_Portal.Models
{
    public class UserAuth
    {
        public int UserId { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpires { get; set; }
        public string? TempPassword { get; set; }
        public bool MustChangePassword { get; set; }
        public int? PasswordChangedBy { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsStaff { get; set; }
        public bool IsSuperuser { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}