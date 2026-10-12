using Alumni_Portal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni_Portal.Controllers
{
    [Authorize]
    public class CareerController : Controller
    {
        private readonly AppDbContext _context;

        public CareerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Career/GetProfessions?sectorId=5
        // Seçilen sektörün fakültesine ait meslekleri döner
        [HttpGet]
        public async Task<IActionResult> GetProfessions(int sectorId)
        {
            var sector = await _context.CareerSectors.FindAsync(sectorId);
            if (sector == null)
                return Json(new List<object>());

            var professions = await _context.CareerProfessions
                .Where(p => p.Faculty == sector.Faculty)
                .OrderBy(p => p.Name)
                .Select(p => new { id = p.Id, name = p.Name })
                .ToListAsync();

            return Json(professions);
        }

        // GET: /Career/GetPositions?professionId=12
        // Seçilen mesleğin fakültesine ait pozisyonları döner
        [HttpGet]
        public async Task<IActionResult> GetPositions(int professionId)
        {
            var profession = await _context.CareerProfessions.FindAsync(professionId);
            if (profession == null)
                return Json(new List<object>());

            var positions = await _context.CareerPositions
                .Where(p => p.Faculty == profession.Faculty)
                .OrderBy(p => p.Name)
                .Select(p => new { id = p.Id, name = p.Name })
                .ToListAsync();

            return Json(positions);
        }
    }
}
