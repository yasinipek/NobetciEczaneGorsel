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

        public IActionResult Privacy()
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
                string bugun = DateTime.Now.ToString("dd.MM.yyyy");

                // �li�kisel verileri include ederek eczaneleri sorgula
                var query = _context.Eczaneler
                    .Include(e => e.Il)
                    .Where(e => e.Tarih == bugun);

                // �l filtresi varsa uygula
                if (ilId.HasValue)
                {
                    query = query.Where(e => e.IlId == ilId.Value);
                }

                // Veritaban�ndan eczaneleri al
                var eczaneler = await query.ToListAsync();

                // E�er hi� veri gelmiyorsa, veritaban�nda bug�ne ait kay�t olmayabilir
                if (eczaneler.Count == 0)
                {
                    // Test i�in en son tarihli kay�tlar� getir
                    var sonTarih = await _context.Eczaneler.OrderByDescending(e => e.KayitZamani)
                        .Select(e => e.Tarih)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(sonTarih))
                    {
                        // �li�kisel verileri include ederek son tarihli eczaneleri �ek
                        query = _context.Eczaneler
                            .Include(e => e.Il)
                            .Where(e => e.Tarih == sonTarih);

                        // �l filtresi varsa uygula
                        if (ilId.HasValue)
                        {
                            query = query.Where(e => e.IlId == ilId.Value);
                        }

                        eczaneler = await query.ToListAsync();
                        _logger.LogInformation($"Bug�ne ait kay�t bulunamad�. Son tarih: {sonTarih}, Kay�t say�s�: {eczaneler.Count}");
                    }
                }

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

        // GET: Home/GetBugunNobetciIller
        [HttpGet("Home/GetBugunNobetciIller")]
        public async Task<IActionResult> GetBugunNobetciIller()
        {
            try
            {
                // Bug�n�n tarihini al
                string bugun = DateTime.Now.ToString("dd.MM.yyyy");

                // Bug�n n�bet�i olan eczanelerin illerini distincte olarak getir
                var nobetciIller = await _context.Eczaneler
                    .Where(e => e.Tarih == bugun)
                    .Select(e => e.Il)
                    .Distinct()
                    .OrderBy(i => i.IlAdi)
                    .Select(i => new
                    {
                        id = i.Id,
                        ilAdi = i.IlAdi
                    })
                    .ToListAsync();

                return Json(nobetciIller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bug�n n�bet�i illeri getirirken hata olu�tu");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}