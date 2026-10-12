using System.ComponentModel.DataAnnotations;

namespace Alumni_Portal.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
        [Display(Name = "Mevcut Şifre")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = "";
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email girin.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";
    }

    public class ResetPasswordViewModel
    {
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = "";
    }
}