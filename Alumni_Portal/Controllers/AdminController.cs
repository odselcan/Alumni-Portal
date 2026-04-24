using Alumni_Portal.Data;
using Alumni_Portal.Models;
using Alumni_Portal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace Alumni_Portal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Mezun Listesi
        public async Task<IActionResult> Index(string? search, string? faculty, string? employmentStatus)
        {
            var adminEmails = await _context.UserAccounts
                .Where(u => u.UserType == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var query = _context.Graduates
                .Include(g => g.GraduateCareer)
                .Where(g => g.IsActive && !adminEmails.Contains(g.Email!))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(g =>
                    g.FirstName!.Contains(search) ||
                    g.LastName!.Contains(search) ||
                    g.Email!.Contains(search));

            if (!string.IsNullOrEmpty(faculty))
                query = query.Where(g => g.Faculty == faculty);

            if (!string.IsNullOrEmpty(employmentStatus))
                query = query.Where(g => g.GraduateCareer!.EmploymentStatus == employmentStatus);

            var graduates = await query.ToListAsync();

            var faculties = await _context.Graduates
                .Where(g => g.Faculty != null && g.IsActive && !adminEmails.Contains(g.Email!))
                .Select(g => g.Faculty!)
                .Distinct()
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.SelectedFaculty = faculty;
            ViewBag.SelectedStatus = employmentStatus;
            ViewBag.Faculties = faculties;

            return View(graduates);
        }

        // Mezun Sil
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var graduate = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .FirstOrDefaultAsync(g => g.GraduateId == id);

            if (graduate == null)
                return NotFound();

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == graduate.Email);

            if (graduate.GraduateCareer != null)
                _context.GraduateCareers.Remove(graduate.GraduateCareer);

            _context.Graduates.Remove(graduate);

            if (userAccount != null)
            {
                var userAuth = await _context.UserAuths
                    .FirstOrDefaultAsync(a => a.UserId == userAccount.Id);
                if (userAuth != null)
                    _context.UserAuths.Remove(userAuth);

                _context.UserAccounts.Remove(userAccount);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Mezun başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // İstatistikler
        public async Task<IActionResult> Statistics()
        {
            var adminEmails = await _context.UserAccounts
                .Where(u => u.UserType == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var totalGraduates = await _context.Graduates
                .CountAsync(g => g.IsActive && !adminEmails.Contains(g.Email!));
            var employed = await _context.GraduateCareers
                .CountAsync(c => c.EmploymentStatus == "employed");
            var jobSeeking = await _context.GraduateCareers
                .CountAsync(c => c.EmploymentStatus == "job_seeking");
            var academic = await _context.GraduateCareers
                .CountAsync(c => c.EmploymentStatus == "academic");
            var unspecified = totalGraduates - employed - jobSeeking - academic;

            var byFaculty = await _context.Graduates
                .Where(g => g.Faculty != null && g.IsActive && !adminEmails.Contains(g.Email!))
                .GroupBy(g => g.Faculty)
                .Select(g => new { Faculty = g.Key, Count = g.Count() })
                .ToListAsync();

            var byDepartment = await _context.Graduates
                .Where(g => g.Department != null && g.IsActive && !adminEmails.Contains(g.Email!))
                .GroupBy(g => g.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToListAsync();

            var bySector = await _context.GraduateCareers
                .Where(c => c.Sector != null)
                .GroupBy(c => c.Sector)
                .Select(g => new { Sector = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.TotalGraduates = totalGraduates;
            ViewBag.Employed = employed;
            ViewBag.JobSeeking = jobSeeking;
            ViewBag.Academic = academic;
            ViewBag.Unspecified = unspecified;
            ViewBag.ByFaculty = byFaculty;
            ViewBag.ByDepartment = byDepartment;
            ViewBag.BySector = bySector;

            return View();
        }

        // Excel Export
        public async Task<IActionResult> ExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var adminEmails = await _context.UserAccounts
                .Where(u => u.UserType == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var graduates = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .Where(g => g.IsActive && !adminEmails.Contains(g.Email!))
                .ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Mezunlar");

            var headers = new[] { "Ad", "Soyad", "E-posta", "Telefon", "Fakülte", "Bölüm", "Mezuniyet", "Çalışma Durumu", "Şirket", "Pozisyon", "Sektör" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(108, 99, 255));
                ws.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            for (int i = 0; i < graduates.Count; i++)
            {
                var g = graduates[i];
                var row = i + 2;
                ws.Cells[row, 1].Value = g.FirstName;
                ws.Cells[row, 2].Value = g.LastName;
                ws.Cells[row, 3].Value = g.Email;
                ws.Cells[row, 4].Value = g.Phone;
                ws.Cells[row, 5].Value = g.Faculty;
                ws.Cells[row, 6].Value = g.Department;
                ws.Cells[row, 7].Value = g.GraduationDate?.ToString("MM/yyyy");
                ws.Cells[row, 8].Value = g.GraduateCareer?.EmploymentStatus switch
                {
                    "employed" => "Çalışıyor",
                    "job_seeking" => "İş Arıyor",
                    "academic" => "Akademik",
                    _ => "Belirtilmemiş"
                };
                ws.Cells[row, 9].Value = g.GraduateCareer?.CompanyName;
                ws.Cells[row, 10].Value = g.GraduateCareer?.Position;
                ws.Cells[row, 11].Value = g.GraduateCareer?.Sector;

                if (i % 2 == 0)
                {
                    ws.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 255));
                }
            }

            ws.Cells.AutoFitColumns();

            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Mezunlar_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // PDF Export
        public async Task<IActionResult> ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var adminEmails = await _context.UserAccounts
                .Where(u => u.UserType == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var graduates = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .Where(g => g.IsActive && !adminEmails.Contains(g.Email!))
                .ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Mezun Listesi")
                            .FontSize(18).Bold().FontColor("#6c63ff");
                        col.Item().Text($"Oluşturulma: {DateTime.Now:dd.MM.yyyy HH:mm}")
                            .FontSize(9).FontColor("#888888");
                        col.Item().Height(10);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                        });

                        var headerBg = QuestPDF.Helpers.Colors.Purple.Medium;
                        table.Header(header =>
                        {
                            foreach (var h in new[] { "Ad Soyad", "E-posta", "Fakülte/Bölüm", "Mezuniyet", "Durum", "Şirket", "Pozisyon" })
                            {
                                header.Cell().Background(headerBg).Padding(6)
                                    .Text(h).FontColor(QuestPDF.Helpers.Colors.White).Bold();
                            }
                        });

                        bool isEven = false;
                        foreach (var g in graduates)
                        {
                            var bg = isEven ? "#f5f5ff" : "#ffffff";
                            isEven = !isEven;

                            var status = g.GraduateCareer?.EmploymentStatus switch
                            {
                                "employed" => "Çalışıyor",
                                "job_seeking" => "İş Arıyor",
                                "academic" => "Akademik",
                                _ => "Belirtilmemiş"
                            };

                            table.Cell().Background(bg).Padding(6).Text($"{g.FirstName} {g.LastName}");
                            table.Cell().Background(bg).Padding(6).Text(g.Email ?? "-");
                            table.Cell().Background(bg).Padding(6).Text($"{g.Faculty ?? "-"} / {g.Department ?? "-"}");
                            table.Cell().Background(bg).Padding(6).Text(g.GraduationDate?.ToString("MM/yyyy") ?? "-");
                            table.Cell().Background(bg).Padding(6).Text(status);
                            table.Cell().Background(bg).Padding(6).Text(g.GraduateCareer?.CompanyName ?? "-");
                            table.Cell().Background(bg).Padding(6).Text(g.GraduateCareer?.Position ?? "-");
                        }
                    });

                    page.Footer().AlignRight()
                        .Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });

            var bytes = pdf.GeneratePdf();
            return File(bytes, "application/pdf", $"Mezunlar_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // Onay Bekleyenler
        public async Task<IActionResult> PendingApprovals()
        {
            var adminEmails = await _context.UserAccounts
                .Where(u => u.UserType == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var pending = await _context.Graduates
                .Where(g => g.IsActive == false && !adminEmails.Contains(g.Email!))
                .OrderByDescending(g => g.CreatedDate)
                .ToListAsync();

            return View(pending);
        }

        // Onayla
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var graduate = await _context.Graduates.FindAsync(id);
            if (graduate == null) return NotFound();

            graduate.IsActive = true;
            graduate.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{graduate.FirstName} {graduate.LastName} onaylandı.";
            return RedirectToAction("PendingApprovals");
        }

        // Reddet
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var graduate = await _context.Graduates
                .Include(g => g.GraduateCareer)
                .FirstOrDefaultAsync(g => g.GraduateId == id);

            if (graduate == null) return NotFound();

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == graduate.Email);

            if (graduate.GraduateCareer != null)
                _context.GraduateCareers.Remove(graduate.GraduateCareer);

            _context.Graduates.Remove(graduate);

            if (userAccount != null)
            {
                var userAuth = await _context.UserAuths
                    .FirstOrDefaultAsync(a => a.UserId == userAccount.Id);
                if (userAuth != null)
                    _context.UserAuths.Remove(userAuth);

                _context.UserAccounts.Remove(userAccount);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Kayıt reddedildi ve silindi.";
            return RedirectToAction("PendingApprovals");
        }

        // Admin Oluştur - GET
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // Admin Oluştur - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(AdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = await _context.UserAccounts
                .AnyAsync(u => u.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            int? createdBy = null;
            try
            {
                var schemaEntry = await _context.UserSchemas.FirstOrDefaultAsync();
                createdBy = schemaEntry?.UserId;
            }
            catch { }

            var userAccount = new UserAccount
            {
                Username     = model.Email.Split('@')[0],
                FirstName    = model.FirstName,
                LastName     = model.LastName,
                Email        = model.Email,
                PasswordHash = HashPassword(model.Password),
                UserType     = "admin",
                IsActive     = true,
                CreatedBy    = createdBy,
                CreateDate   = DateTime.UtcNow,
                UpdateDate   = DateTime.UtcNow
            };

            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{model.FirstName} {model.LastName} admin olarak eklendi.";
            return RedirectToAction("Index");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}