using System.ComponentModel.DataAnnotations;

namespace Alumni_Portal.ViewModels
{
    public class ProfileViewModel
    {
        // Kişisel bilgiler (düzenlenemez)
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public DateTime? GraduationDate { get; set; }

        // İletişim bilgileri (düzenlenebilir)
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        // Kariyer bilgileri (düzenlenebilir)
        public string? EmploymentStatus { get; set; }

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        [StringLength(100)]
        public string? Sector { get; set; }

        [Range(1950, 2100, ErrorMessage = "Geçerli bir yıl giriniz.")]
        public int? StartYear { get; set; }
    }
}