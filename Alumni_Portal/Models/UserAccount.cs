namespace Alumni_Portal.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserType { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Phone { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public string? LanguageCode { get; set; }
    }
}