using e_citaonica_api.Models;
using e_citaonica_api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace e_citaonica_api.Controllers;

[ApiController]
[Authorize]
[Route("predmeti")]
public class PredmetController : ControllerBase
{
    private ECitaonicaContext _context;
    private IDeleteService _deleteService;

    public PredmetController(ECitaonicaContext context, IDeleteService deleteService)
    {
        _context = context;
        _deleteService = deleteService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Predmet>> GetPredmet(int id)
    {
        try
        {
            string email = HttpContext.User.FindFirstValue("preferred_username");

            var korisnik = await _context.Korisnici.Where(k => k.Email == email)
                .SingleAsync();

            var predmet = await _context.Predmeti.Where(p => p.Id == id)
                .Include(p => p.Profesori)
                .Include(p => p.Oblasti)
                .Include(p => p.Pratioci)
                .Include(p => p.Moduli)
                .SingleAsync();

            Console.WriteLine(email);

            return Ok(new
            {
                predmet.Id,
                predmet.Naziv,
                predmet.Opis,
                predmet.Godina,
                predmet.Semestar,
                Oblasti = predmet.Oblasti.OrderBy(o => o.RedniBr),
                predmet.Moduli,
                Profesori = predmet.Profesori,
                Pracen = predmet.Pratioci.Contains(korisnik)
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.StackTrace);
        }
    }

    //predmeti koje prati jedan korisnik
    [Authorize]
    [HttpGet("praceni/{korisnikId}")]
    public async Task<ActionResult<List<Predmet>>> PraceniPredmetiKorisnika(int korisnikId)
    {
        try
        {
            Korisnik k = await _context.Korisnici.Where(s => s.Id == korisnikId)
                .Include(s => s.PraceniPredmeti)
                .FirstAsync();

            var predmeti = k.PraceniPredmeti.Select(p => new
            {
                p.Id,
                p.Naziv,
                p.Godina,
                p.Semestar,
                Pracen = true
            }).OrderBy(p => p.Semestar);

            return Ok(predmeti);

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }



    //[Authorize]
    [HttpGet("prati-otprati/{korisnikId}/{predmetId}")]
    public async Task<ActionResult<List<Predmet>>> Prati(int korisnikId, int predmetId)
    {
        try
        {
            Korisnik k = await _context.Korisnici.Where(s => s.Id == korisnikId)
                .Include(s => s.PraceniPredmeti)
                .SingleAsync();

            Predmet p = await _context.Predmeti.FindAsync(predmetId);

            string response;

            if (!k.PraceniPredmeti.Contains(p))
            {
                k.PraceniPredmeti.Add(p);
                response = "Pratite predmet " + p.Naziv;
            }
            else
            {
                k.PraceniPredmeti.Remove(p);
                response = "Prestali ste da pratite predmet " + p.Naziv;
            }
            await _context.SaveChangesAsync();

            return Ok(new { msg = response });

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<Predmet>> DodajPredmet([FromBody] PredmetModel predmetModel)
    {
        try
        {
            Predmet predmet = new Predmet
            {
                Naziv = predmetModel.Naziv.Trim(),
                Godina = (int)Math.Ceiling(predmetModel.Semestar / 2.0),
                Semestar = predmetModel.Semestar,
                Opis = predmetModel.Opis.Trim()
            };

            predmet.Oblasti = predmetModel.Oblasti.Select(o => new Oblast
            {
                Naziv = o.Naziv.Trim(),
                RedniBr = o.RedniBr,
                Predmet = predmet,
                Diskusije = new List<DiskusijaOblast>()
            }).ToList();

            predmet.Profesori = await _context.Profesori.Where(p => predmetModel.ProfesoriIds.Contains(p.Id)).ToListAsync();

            predmet.Moduli = await _context.Moduli.Where(m => predmetModel.ModuliIds.Contains(m.Id)).ToListAsync();

            _context.Predmeti.Add(predmet);

            await _context.SaveChangesAsync();
            return Ok(predmet);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                msg = "Exception: " + e.Message
            });
        }
    }

    [HttpPut]
    public async Task<ActionResult<Predmet>> IzmeniPredmet([FromBody] PredmetModel editPredmetModel)
    {
        try
        {
            Predmet predmet = await _context.Predmeti.Where(p => p.Id == editPredmetModel.Id)
            .Include(p => p.Moduli)
            .Include(p => p.Profesori)
            .Include(p => p.Pratioci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Pratioci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Obavestenja)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Dodaci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Dodaci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Obavestenja)

            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Blanketi)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Oblasti)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Dodaci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Pratioci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Obavestenja)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Dodaci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Dodaci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Obavestenja)
            .AsSplitQuery().FirstAsync(); // desi se timeout bez .AsSplitQuery()

            if (predmet == null)
            {
                return BadRequest(new { msg = "Nije pronadjen predmet" });
            }

            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();
            if (korisnik.Tip != "administrator" && !predmet.Profesori.Contains(korisnik))
            {
                return BadRequest(new { msg = "Nemate privilegije: niste profesor na predmetu." });
            }

            predmet.Naziv = editPredmetModel.Naziv;
            predmet.Semestar = editPredmetModel.Semestar;
            predmet.Godina = (int)Math.Ceiling(editPredmetModel.Semestar / 2.0);
            predmet.Opis = editPredmetModel.Opis;

            var preostaleOblasti = editPredmetModel.Oblasti.Where(o => o.Id > 0);

            foreach (var preostalaOblast in preostaleOblasti)
            {
                var oblastZaUpdate = await _context.Oblasti.FindAsync(preostalaOblast.Id);

                oblastZaUpdate.Naziv = preostalaOblast.Naziv;
                oblastZaUpdate.RedniBr = preostalaOblast.RedniBr;
                _context.Update(oblastZaUpdate);
            }

            IEnumerable<Oblast>? oblastiZaBrisanje = null;

            if (predmet.Oblasti is not null)
            {
                if (preostaleOblasti is not null)
                    oblastiZaBrisanje = predmet.Oblasti.Where(p => !preostaleOblasti.Select(o => o.Id).Contains(p.Id));

                Console.WriteLine(predmet.Oblasti[0].Zadaci.Count());

                if (oblastiZaBrisanje is not null)
                {
                    foreach (var oblast in oblastiZaBrisanje)
                    {
                        await _deleteService.ObrisiOblast(oblast);
                    }
                }
            }

            if (predmet.Oblasti is null)
                predmet.Oblasti = new List<Oblast>();

            var oblastiZaDodavanje = editPredmetModel.Oblasti.Where(o => o.Id <= 0);
            foreach (var oblast in oblastiZaDodavanje)
            {
                predmet.Oblasti.Add(new Oblast
                {
                    Naziv = oblast.Naziv,
                    Diskusije = default!,
                    Predmet = predmet,
                    RedniBr = oblast.RedniBr,
                    Zadaci = default!
                });
            }

            var moduli = await _context.Moduli.Where(m => editPredmetModel.ModuliIds.Contains(m.Id)).ToListAsync();
            predmet.Moduli = moduli;

            var profesoriZaBrisanjeIds = predmet.Profesori.Where(p => !editPredmetModel.ProfesoriIds.Contains(p.Id)).Select(p => p.Id);
            var profesoriZaBrisanje = await _context.Profesori
            .Where(p => profesoriZaBrisanjeIds.Contains(p.Id))
            .Include(p => p.PraceniPredmeti)
            .ToListAsync();

            foreach (var prof in profesoriZaBrisanje)
            {
                if (prof.PraceniPredmeti.Contains(predmet))
                {
                    prof.PraceniPredmeti.Remove(predmet);
                }
            }

            var profesori = await _context.Profesori
                .Where(p => editPredmetModel.ProfesoriIds.Contains(p.Id))
                .Include(p => p.PraceniPredmeti)
                .ToListAsync();

            predmet.Profesori = profesori;

            foreach (var profesor in predmet.Profesori)
            {
                if (!profesor.PraceniPredmeti.Contains(predmet))
                {
                    profesor.PraceniPredmeti.Add(predmet);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                predmet.Id,
                predmet.Naziv,
                predmet.Moduli,
                predmet.Profesori,
                predmet.Godina,
                predmet.Semestar,
                predmet.Opis,
                Oblasti = predmet.Oblasti.OrderBy(o => o.RedniBr)
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    private async Task ObrisiPodstablo(Komentar kom)
    {
        var children = await _context.Komentari
            .Where(k => k.Objava == kom)
            .Include(k => k.Zahvalnice)
            .Include(k => k.Dodaci)
            .Include(k => k.Obavestenja)
            .ToListAsync();
        foreach (var child in children.ToList())
        {
            await ObrisiPodstablo(child);
        }

        if (kom.Zahvalnice is not null)
            _context.Zahvalnice.RemoveRange(kom.Zahvalnice);

        if (kom.Dodaci is not null)
            _context.Dodaci.RemoveRange(kom.Dodaci);

        if (kom.Obavestenja is not null)
            _context.Obavestenja.RemoveRange(kom.Obavestenja);
        _context.Komentari.Remove(kom);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult> ObrisiPredmet(int id)
    {
        try
        {

            Predmet predmet = await _context.Predmeti.Where(p => p.Id == id)
            .Include(p => p.Moduli)
            .Include(p => p.Profesori)
            .Include(p => p.Pratioci)
            .Include(p => p.Blanketi).ThenInclude(b => b.Zadaci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Blanketi)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Oblasti)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Dodaci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Pratioci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Obavestenja)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Dodaci)

            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Dodaci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Zadaci).ThenInclude(z => z.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Obavestenja)

            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Pratioci)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Komentari)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Zahvalnice)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Obavestenja)
            .Include(p => p.Oblasti).ThenInclude(o => o.Diskusije).ThenInclude(d => d.Dodaci)

            .Include(p => p.Diskusije).ThenInclude(d => d.Pratioci)
            .Include(p => p.Diskusije).ThenInclude(d => d.Komentari)
            .Include(p => p.Diskusije).ThenInclude(d => d.Zahvalnice)
            .Include(p => p.Diskusije).ThenInclude(d => d.Obavestenja)
            .Include(p => p.Diskusije).ThenInclude(d => d.Dodaci)
             .Include(p => p.Diskusije).ThenInclude(d => (d as DiskusijaOblast).Oblasti)

            .Include(p => p.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Dodaci)
            .Include(p => p.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Zahvalnice)
            .Include(p => p.Diskusije).ThenInclude(d => d.Komentari).ThenInclude(k => k.Obavestenja)
            .AsSplitQuery().FirstAsync();

            if (predmet == null)
            {
                return BadRequest(new { msg = "Nije pronadjen predmet" });
            }

            await _deleteService.ObrisiPredmet(predmet);
            await _context.SaveChangesAsync();

            return Ok(new { msg = "Uspešno obrisan predmet" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [HttpGet("zadaci/{id}")]
    public async Task<ActionResult<List<Zadatak>>> GetZadaci(int id)
    {
        try
        {
            List<Zadatak> zadaci = await _context.Zadaci
                .Include(z => z.Dodaci)
                .Include(z => z.Oblasti)
                .Where(z => z.Oblasti.Select(o => o.Predmet.Id).Contains(id))
                // .Where(b => b.Blanketi.Select(b => b.Predmet.Id).Contains(id))
                .ToListAsync();

            if (zadaci is null)
            {
                return BadRequest(new { msg = "Nije pronadjen predmet" });
            }

            return Ok(zadaci.Select(z => new
            {
                z.Id,
                z.Tekst,
                z.Tip,
                z.BrPoena,
                z.Oblasti,
                z.Dodaci,
                ImaDiskusije = _context.DiskusijeZaZadatke
                        .Any(d => d.Zadatak == z),

                ImaDiskusijeSaResenjima = _context.DiskusijeZaZadatke.Any(d => d.Zadatak == z && d.Zavrsena == true)

            }));
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [HttpPost("generisi-blanket")]
    public async Task<ActionResult<Blanket>> GenerisiBlanket([FromBody] GenerisiBlanketModel generisiModel)
    {
        try
        {
            var oblasti = await _context.Oblasti.Where(o => generisiModel.OblastiIds.Contains(o.Id)).ToListAsync();

            List<Zadatak> zadaci = new List<Zadatak>();

            Random rnd = new Random();

            int i = 1;

            foreach (var oblast in oblasti)
            {
                var dostupniZadaciIzOblasti = await _context.Zadaci
                .Where(z => z.Oblasti.Contains(oblast) && generisiModel.Tipovi.Contains(z.Tip))
                .Include(z => z.Oblasti)
                .ToListAsync();

                // jer zadatak moze da se ponovi iz druge oblasti
                var filtriraniZadaci = dostupniZadaciIzOblasti.Where(z => !zadaci.Contains(z)).ToList();

                if (filtriraniZadaci.Count > 0)
                {
                    Zadatak zadatak = filtriraniZadaci[rnd.Next(filtriraniZadaci.Count)];
                    zadatak.RedniBr = i++;
                    zadaci.Add(zadatak);
                }
            }

            if (zadaci.Count == 0)
            {
                return BadRequest(new { msg = "Nema zadataka iz ove oblasti." });
            }

            return Ok(new
            {
                Datum = DateTime.Now,
                Id = 0,
                IspitniRok = new IspitniRok { Id = 0, Naziv = "Vežbanje" },
                Predmet = await _context.Predmeti.FindAsync(generisiModel.PredmetId),
                Tip = "Vežbanje",
                Zadaci = zadaci.Select(z => new
                {
                    z.Id,
                    z.RedniBr,
                    z.Tekst,
                    z.BrPoena,
                    z.Tip,
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
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }


    [HttpGet("najaktivniji-studenti/{id}")]
    public async Task<ActionResult<List<Student>>> NajaktivnijiStudenti(int id)
    {
        try
        {
            var diskusijePredmeta = await _context.Diskusije
            .Include(d => d.Autor)
            .Where(d => d.Predmet.Id == id && d.Autor is Student)
            .ToListAsync();

            var komentariPredmeta = await _context.Komentari
            .Include(k => k.Autor)
            .Where(k => k.Diskusija.Predmet.Id == id && k.Autor is Student)
            .ToListAsync();

            var a = ((IEnumerable<dynamic>)diskusijePredmeta).Concat(komentariPredmeta);

            var d = a
            .Select(dp => new
            {
                Student = dp.Autor,
            })
            .GroupBy(dp => dp.Student)
            .Select(dp => new
            {
                Student = dp.Key,
                BrObjava = diskusijePredmeta.Where(d => d.Autor.Id == dp.Key.Id).Count()
                + komentariPredmeta.Where(d => d.Autor.Id == dp.Key.Id).Count()
            })
            .OrderByDescending(dp => dp.BrObjava)
            .Take(10);

            return Ok(d);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                msg = "Exception: " + e.Message
            });
        }
    }

    [HttpGet("preporuke/{id}")]
    public async Task<ActionResult> Preporuke(int id, [FromQuery] string? query = null, [FromQuery] string? exclude = null)
    {
        string email = HttpContext.User.FindFirstValue("preferred_username");

        var korisnik = await _context.Korisnici.Where(k => k.Email == email).Select(k => k.Id).SingleAsync();

        var excludeInts = new List<int>();
        excludeInts.Add(korisnik);
        if (exclude != null)
            foreach (string num in exclude.Split('i'))
            {
                int res;
                if (int.TryParse(num, out res))
                    excludeInts.Add(res);
            }

        var predmet = await _context.Predmeti
            .Where(p => p.Id == id)
            .Include(p => p.Pratioci
                .Where(pr => pr is Student
                && (String.IsNullOrEmpty(query) || pr.Ime!.Contains(query)) &&
                !excludeInts.Contains(pr.Id)))
            .ThenInclude(p => p.Objave.Where(o => o is Komentar && ((Komentar)o).Diskusija.Predmet.Id == id))
            .ThenInclude(o => o.Zahvalnice)
            .FirstAsync();

        var pas = predmet.Pratioci.Select(p => new
        {
            Student = p,
            Komentari = p.Objave.Select(k => new
            {
                Komentar = k,
                BrZahvalnica = k.Zahvalnice.Count()
            })
        }).Select(p => new
        {
            p.Student.Ime,
            p.Student.Id,
            p.Student.SlikaURL,
            BrPrihvacenih = p.Komentari.Where(k => (k.Komentar as Komentar).Prihvacen == true).Count(),
            BrKomentara = p.Komentari.Count(),
            BrZahvalnica = p.Komentari.Select(k => k.BrZahvalnica).Sum()
        }).Select(p => new
        {
            p.Ime,
            p.Id,
            p.SlikaURL,
            p.BrKomentara,
            p.BrZahvalnica,
            Score = (double)(0.4 * p.BrZahvalnica + 0.6 * p.BrPrihvacenih) / (p.BrKomentara + 1) + 1
        }).OrderByDescending(r => r.Score).Take(5);

        return Ok(
            pas
        );
    }
}

