namespace Alumni_Portal.Models
{
    public class GraduateCareer
    {
        public int CareerId { get; set; }
        public int GraduateId { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public string? Sector { get; set; }
        public int? StartYear { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? OperationUserId { get; set; }
        public string? ArchiveAction { get; set; }
        public DateTime? ArchiveDate { get; set; }

        // Navigation property
        public Graduate? Graduate { get; set; }
    }
}