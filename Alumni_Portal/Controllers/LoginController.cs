using Alumni_Portal.Data;
using Alumni_Portal.Services;
using Alumni_Portal.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Alumni_Portal.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext  _context;
        private readonly EmailService  _emailService;
        private readonly IMemoryCache  _cache;

        public LoginController(AppDbContext context, EmailService emailService, IMemoryCache cache)
        {
            _context      = context;
            _emailService = emailService;
            _cache        = cache;
        }

        // GET: /Login/Index
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Login/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hashedPassword = HashPassword(model.Password);

            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == model.Email
                                       && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                return View(model);
            }

            var graduate = await _context.Graduates
                .FirstOrDefaultAsync(g => g.Email == user.Email);

            if (user.UserType != "admin")
            {
                if (graduate == null || !graduate.IsActive)
                {
                    ModelState.AddModelError("", "Hesabınız henüz onaylanmamış. Lütfen admin onayını bekleyiniz.");
                    return View(model);
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.UserType ?? "graduate"),
                new Claim("GraduateId", graduate?.GraduateId.ToString() ?? "0")
            };

            var identity  = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc   = DateTime.UtcNow.AddHours(8)
            });

            return user.UserType == "admin"
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Home");
        }

        // GET: /Login/ForgotPassword
        public IActionResult ForgotPassword() => View();

        // POST: /Login/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive == true);

            if (user != null)
            {
                // Token üret
                var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

                // Token'ı memory cache'e kaydet — DB'ye INSERT yetkisi olmadığı için
                var cacheKey = $"reset_token_{model.Email}";
                _cache.Set(cacheKey, rawToken, TimeSpan.FromHours(1));

                // Reset linki oluştur
                var resetLink = Url.Action("ResetPassword", "Login",
                    new { token = rawToken, email = model.Email }, Request.Scheme);

                var body = $@"
                    <h3>Şifre Sıfırlama - Alumni Portal</h3>
                    <p>Merhaba {user.FirstName},</p>
                    <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                    <p><a href='{resetLink}' style='background:#cc0000;color:white;padding:10px 20px;
                       text-decoration:none;border-radius:5px;display:inline-block;'>
                       Şifremi Sıfırla</a></p>
                    <p>Bu link <b>1 saat</b> geçerlidir.</p>
                    <p>Bu isteği siz yapmadıysanız lütfen dikkate almayın.</p>";

                await _emailService.SendAsync(model.Email, "Alumni Portal - Şifre Sıfırlama", body);
            }

            TempData["Info"] = "Email adresiniz kayıtlıysa sıfırlama linki gönderildi.";
            return RedirectToAction("ForgotPassword");
        }

        // GET: /Login/ResetPassword
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "Geçersiz link.";
                return RedirectToAction("Index");
            }

            // Cache'den token kontrol et
            var cacheKey = $"reset_token_{email}";
            if (!_cache.TryGetValue(cacheKey, out string? cachedToken) || cachedToken != token)
            {
                TempData["Error"] = "Geçersiz veya süresi dolmuş link.";
                return RedirectToAction("Index");
            }

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        // POST: /Login/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                TempData["Error"] = "Geçersiz link.";
                return RedirectToAction("Index");
            }

            // Cache'den token kontrol et
            var cacheKey = $"reset_token_{model.Email}";
            if (!_cache.TryGetValue(cacheKey, out string? cachedToken) || cachedToken != model.Token)
            {
                TempData["Error"] = "Geçersiz veya süresi dolmuş link.";
                return RedirectToAction("Index");
            }

            // Şifreyi güncelle
            user.PasswordHash = HashPassword(model.NewPassword);
            user.UpdateDate   = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Token'ı cache'den sil (tek kullanım)
            _cache.Remove(cacheKey);

            TempData["Success"] = "Şifreniz başarıyla sıfırlandı. Giriş yapabilirsiniz.";
            return RedirectToAction("Index");
        }

        // POST: /Login/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Login");
        }

        public IActionResult AccessDenied() => View();

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}