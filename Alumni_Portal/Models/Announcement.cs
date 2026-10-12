namespace Alumni_Portal.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? UpdateDate { get; set; }
        public int? OperationUserId { get; set; }
        public string? ArchiveAction { get; set; }
        public DateTime? ArchiveDate { get; set; }
    }
}