using System.ComponentModel.DataAnnotations;

namespace Alumni_Portal.ViewModels
{
    public class ProfileViewModel
    {
        // Kişisel Bilgiler
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Mezun Bilgileri
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public DateTime? GraduationDate { get; set; }

        // Kariyer Bilgileri
        public string? EmploymentStatus { get; set; }
        public string? CompanyName { get; set; }
        public string? Sector { get; set; }
        public string? Profession { get; set; }   // ← YENİ
        public string? Position { get; set; }
        public int? StartYear { get; set; }

        // Profil Fotoğrafı
        public string? ProfileImageUrl { get; set; }
    }
}