using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NobetciEczaneGorsel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NobetciEczaneGorsel.Data;

namespace NobetciEczaneGorsel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Home/GetEczaneData
        [HttpGet("Home/GetEczaneData")]
        public async Task<IActionResult> GetEczaneData([FromQuery] int? ilId = null)
        {
            try
            {
                // Bug�n�n tarihini al
                string bugun;
                int saat = DateTime.Now.Hour;
                if (saat >= 8)
                {
                    bugun = DateTime.Now.ToString("dd/MM/yyyy").Replace('.', '/');
                }
                else
                {
                    bugun = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy").Replace('.', '/');
                }

                // Veritaban�ndan eczaneleri al
                var eczaneler = await _context.Eczaneler.Where(e => e.IlId == ilId).Where(e => e.Tarih == bugun).ToListAsync();

                // Sonu� setini JSON format�nda d�nd�r
                if (eczaneler.Any())
                {
                    return Json(eczaneler.Select(e => new
                    {
                        e.Id,
                        e.Isim,
                        Il = e.Il?.IlAdi,  // �l nesnesinden il ad�n� al
                        e.Ilce,
                        e.Telefon,
                        e.Adres,
                        e.Enlem,
                        e.Boylam,
                        e.Tarih,
                        e.KayitZamani
                    }));
                }
                else
                {
                    // Hi� eczane bulunamad�ysa
                    _logger.LogWarning("Veritaban�nda hi� eczane kayd� bulunamad�");
                    return Json(new List<object>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eczane verilerini getirirken hata olu�tu");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: Home/GetIller
        [HttpGet("Home/GetIller")]
        public async Task<IActionResult> GetIller()
        {
            try
            {
                // T�m illeri getir
                var iller = await _context.Iller
                    .OrderBy(i => i.IlAdi)
                    .Select(i => new
                    {
                        id = i.Id,
                        ilAdi = i.IlAdi
                    })
                    .ToListAsync();

                return Json(iller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�lleri getirirken hata olu�tu");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}