using System.Security.Claims;
using e_citaonica_api.Models;
using e_citaonica_api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_citaonica_api.Controllers;

[Authorize]
[ApiController]
[Route("blanketi")]
public class BlanketController : ControllerBase
{
    private ECitaonicaContext _context;
    private IDeleteService _deleteService;

    public BlanketController(ECitaonicaContext context, IDeleteService deleteService)
    {
        _context = context;
        _deleteService = deleteService;
    }

    [HttpGet("predmet/{predmetId}")]
    public async Task<ActionResult<List<Blanket>>> Blanketi(/*[FromBody] BlanketFilter filteri*/int predmetId)
    {
        try
        {
            var blanketi = _context.Blanketi.Where(b => b.Predmet.Id == predmetId)
            //&& b.Datum.Year == filteri.Godina
            //&& filteri.RokoviIds.Contains(b.IspitniRok.Id)
            //&& filteri.Tipovi.Contains(b.Tip))
                .Include(b => b.IspitniRok)
                .Select(b =>
                new
                {
                    b.Id,
                    b.IspitniRok,
                    b.Tip,
                    b.Datum
                }).OrderByDescending(b => b.Datum);

            return Ok(await blanketi.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = e.StackTrace });
        }

    }

    [HttpGet("{id}")]
    public async Task<ActionResult> BlanketFull(int id)
    {
        try
        {
            var blanket = await _context.Blanketi.Where(b => b.Id == id)
                .Include(b => b.Zadaci)
                .ThenInclude(z => z.Oblasti)
                .Include(b => b.Predmet)
                .Include(b => b.IspitniRok)
                .FirstAsync();
            return Ok(new
            {
                blanket.Id,
                blanket.Datum,
                blanket.IspitniRok,
                blanket.Tip,
                blanket.Predmet,
                Zadaci = blanket.Zadaci.Select(z => new
                {
                    Id = z.Id,
                    RedniBr = z.RedniBr,
                    Tekst = z.Tekst,
                    BrPoena = z.BrPoena,
                    Tip = z.Tip,
                    Oblasti = z.Oblasti.Select(o => new
                    {
                        o.Id,
                        o.Naziv
                    }),

                    ImaDiskusije = _context.DiskusijeZaZadatke
                        .Any(d => d.Zadatak == z),

                    ImaDiskusijeSaResenjima = _context.DiskusijeZaZadatke.Any(d => d.Zadatak == z && d.Zavrsena == true)
                })
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = e.Message });
        }
    }

    [HttpPut("izmeni")]
    [Authorize(Roles = "administrator, profesor")]
    public async Task<ActionResult> IzmeniBlanket([FromBody] BlanketModel bl)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Blanket? blanket = await _context.Blanketi
                .Include(b => b.Predmet).ThenInclude(p => p.Profesori)
                .Include(b => b.Zadaci)
                .ThenInclude(z => z.Oblasti)
                .Include(b => b.IspitniRok)
                .Where(b => b.Id == bl.Id).FirstAsync();
            // if (blanket == null) return BadRequest(new { msg = "Blanket ne postoji" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik.Tip != "administrator" && !blanket.Predmet.Profesori.Contains(korisnik))
            {
                return BadRequest(new { msg = "Nemate privilegije: niste profesor na predmetu." });
            }


            IspitniRok ispitniRok = await _context.IspitniRokovi.Where(ir => ir.Id == bl.IspitniRokId).FirstAsync();

            if (ispitniRok == null)
                return BadRequest(new { msg = "Nije pronadjen ispitni rok" });
            if (!bl.Zadaci.Any())
                return BadRequest(new { msg = "Blanket mora imati bar jedan zadatak" });

            blanket.IspitniRok = ispitniRok;
            blanket.Datum = bl.Datum;
            blanket.Tip = bl.Tip;

            var postojeci = bl.Zadaci.Where(z => z.Id != 0).Select(z => z.Id).ToList();
            var obrisani = blanket.Zadaci.Where(z => !postojeci.Contains(z.Id)).ToList();

            obrisani.ForEach((del) =>
            {
                blanket.Zadaci.Remove(del);
            });

            foreach (ZadatakModel zm in bl.Zadaci)
            {
                Zadatak? zad;
                if (zm.Id > 0)
                {
                    zad = blanket.Zadaci.Where(z => z.Id == zm.Id).FirstOrDefault();

                    if (zad == null) return BadRequest(new { msg = "Zadatak sa zadatim id-jem ne postoji" });

                    zad.BrPoena = zm.BrPoena;
                    zad.RedniBr = zm.RedniBr;
                    zad.Oblasti.Clear();
                    zad.Oblasti = await _context.Oblasti.Where(o => zm.OblastiIds.Contains(o.Id)).ToListAsync();
                    zad.Tip = zm.Tip;
                    zad.Tekst = zm.Tekst;

                    _context.Zadaci.Update(zad);
                }
                else
                {
                    zad = new Zadatak
                    {
                        BrPoena = zm.BrPoena,
                        RedniBr = zm.RedniBr,
                        Tekst = zm.Tekst,
                        Tip = zm.Tip,
                        Oblasti = await _context.Oblasti.Where(o => zm.OblastiIds.Contains(o.Id)).ToListAsync()
                    };

                    _context.Zadaci.Add(zad);
                    blanket.Zadaci.Add(zad);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                blanket.Id,
                blanket.Datum,
                blanket.IspitniRok,
                blanket.Tip,
                blanket.Predmet,
                Zadaci = blanket.Zadaci.Select(z => new
                {
                    Id = z.Id,
                    RedniBr = z.RedniBr,
                    Tekst = z.Tekst,
                    BrPoena = z.BrPoena,
                    Tip = z.Tip,
                    Oblasti = z.Oblasti.Select(o => new
                    {
                        o.Id,
                        o.Naziv
                    }),
                    ImaDiskusije = _context.DiskusijeZaZadatke.Any(d => d.Zadatak == z),

                    ImaDiskusijeSaResenjima = _context.DiskusijeZaZadatke.Any(d => d.Zadatak == z && d.Zavrsena == true)

                })
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    #region Kreiranje blanketa
    [HttpPost("dodaj-blanket")]
    [Authorize(Roles = "administrator, profesor")]
    public async Task<ActionResult> DodajBlanket([FromBody] BlanketModel blanketModel)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();


            IspitniRok ispitniRok = await _context.IspitniRokovi.Where(ir => ir.Id == blanketModel.IspitniRokId).FirstAsync();
            Predmet predmet = await _context.Predmeti.Where(p => p.Id == blanketModel.PredmetId)
            .Include(p => p.Profesori).FirstAsync();

            if (korisnik.Tip != "administrator" && !predmet.Profesori.Contains(korisnik))
            {
                return BadRequest(new { msg = "Nemate privilegije: niste profesor na predmetu." });
            }

            if (ispitniRok == null)
                return BadRequest(new { msg = "Nije pronadjen ispitni rok" });
            if (predmet == null)
                return BadRequest(new { msg = "Nije pronadjen predmet" });
            if (blanketModel.Zadaci.Count == 0)
                return BadRequest(new { msg = "Blanket mora imati bar jedan zadatak" });

            Blanket blanket = new()
            {
                Predmet = predmet,
                IspitniRok = ispitniRok,
                Tip = blanketModel.Tip,
                Datum = blanketModel.Datum,
                Zadaci = new List<Zadatak>()
            };
            _context.Blanketi.Add(blanket);

            foreach (ZadatakModel zm in blanketModel.Zadaci)
            {
                Zadatak zadatak = new()
                {
                    BrPoena = zm.BrPoena,
                    RedniBr = zm.RedniBr,
                    Tekst = zm.Tekst,
                    Tip = zm.Tip,
                    Oblasti = await _context.Oblasti.Where(o => zm.OblastiIds.Contains(o.Id)).ToListAsync()
                };

                _context.Zadaci.Add(zadatak);

                blanket.Zadaci.Add(zadatak);

            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                blanket.Id,
                blanket.Datum,
                blanket.IspitniRok,
                blanket.Tip,
                blanket.Predmet,
                Zadaci = blanket.Zadaci.Select(z => new
                {
                    Id = z.Id,
                    RedniBr = z.RedniBr,
                    Tekst = z.Tekst,
                    BrPoena = z.BrPoena,
                    Tip = z.Tip,
                    Oblasti = z.Oblasti.Select(o => new
                    {
                        o.Id,
                        o.Naziv
                    })
                })
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }


    #endregion


    [HttpDelete("{id}")]
    [Authorize(Roles = "administrator, profesor")]
    public async Task<ActionResult> ObrisiBlanket(int id)
    {
        try
        {

            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();
            var blanket = await _context.Blanketi.Where(b => b.Id == id)
                .Include(b => b.Zadaci)
                .Include(b => b.Predmet).ThenInclude(p => p.Profesori)
                .FirstAsync();

            if (korisnik.Tip != "administrator" && !blanket.Predmet.Profesori.Contains(korisnik))
            {
                return BadRequest(new { msg = "Nemate privilegije: niste profesor na predmetu." });
            }

            await _deleteService.ObrisiBlanket(blanket);
            await _context.SaveChangesAsync();

            return Ok(new { msg = "Blanket uspešno obrisan" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(new { msg = e.Message });
        }

    }

    [HttpPut("preporuceni-blanketi")]
    public async Task<ActionResult<List<Blanket>>> PreporuceniBlanketi([FromBody] int[] praceniPredmetiIds)
    {
        try
        {
            var predmeti = await _context.Predmeti
            .Where(p => praceniPredmetiIds.Contains(p.Id))
            .Include(p => p.Blanketi.OrderByDescending(b => b.Datum).Take(1))
            .ThenInclude(b => b.IspitniRok)
            .ToListAsync();

            var blanketi = predmeti.SelectMany(p => p.Blanketi);

            return Ok(blanketi);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(new
            {
                msg = e.Message
            });
        }

    }
}