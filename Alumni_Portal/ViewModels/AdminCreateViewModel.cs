using System.ComponentModel.DataAnnotations;

namespace Alumni_Portal.ViewModels
{
    public class AdminCreateViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}