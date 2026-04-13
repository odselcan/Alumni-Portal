namespace Alumni_Portal.Models
{
    public class Graduate
    {
        public int GraduateId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public DateTime? GraduationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? BirthDate { get; set; }
        public int? OperationUserId { get; set; }
        public string? ArchiveAction { get; set; }
        public DateTime? ArchiveDate { get; set; }

        // Navigation properties
        public GraduateCareer? GraduateCareer { get; set; }
    }
}