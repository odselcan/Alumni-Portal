using System.ComponentModel.DataAnnotations;

namespace Alumni_Portal.ViewModels
{
    public class AnnouncementViewModel
    {
        public int AnnouncementId { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "İçerik zorunludur.")]
        public string Content { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public DateTime? CreateDate { get; set; }
    }
}