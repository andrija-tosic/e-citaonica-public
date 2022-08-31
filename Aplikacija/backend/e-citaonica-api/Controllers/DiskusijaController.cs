using AutoMapper;
using e_citaonica_api.Models;
using e_citaonica_api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace e_citaonica_api.Controllers;

[ApiController]
[Authorize]
[Route("diskusije")]
public class DiskusijaController : Controller
{
    private ECitaonicaContext _context;
    private readonly IMapper _mapper;
    private IDeleteService _deleteService;
    private IFileService _fileService;
    public DiskusijaController(ECitaonicaContext context, IMapper mapper, IDeleteService deleteService, IFileService fileService)
    {
        _context = context;
        _mapper = mapper;
        _deleteService = deleteService;
        _fileService = fileService;
    }

    [Authorize]
    [HttpGet("{id}/{depth}")]
    public async Task<ActionResult<Diskusija>> GetDiskusija(int id, int depth)
    {
        try
        {
            Diskusija? d = await _context.Diskusije.FindAsync(id);
            if (d is null)
                return BadRequest("Ne postoji diskusija");

            if (d is DiskusijaZadatak)
                return Ok(await GetDiskusijaZaZadatakPomocna(id, depth));
            else if (d is DiskusijaOblast)
            {
                return Ok(await GetDiskusijaZaOblastPomocna(id, depth));
            }
            else
                return BadRequest("Ne postoji diskusija");
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }

    }

    #region prijava
    [HttpPost("prijavi")]
    public async Task<ActionResult> PrijaviObjavu(PrijavaModel prijava)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null)
                return BadRequest(new { msg = "Korisnik ne postoji" });

            Objava objava = await _context.Objave.Where(o => o.Id == prijava.ObjavaId)
                .Include(o => o.Autor)
                .FirstAsync();


            var administrator = await _context.Korisnici.Where(k => k.Tip.CompareTo("administrator") == 0).FirstAsync();

            string msg = "";
            Predmet? predmet;
            if (objava is null)
                return BadRequest(new { msg = "Objava ne postoji" });
            else if (objava is Diskusija)
            {
                Diskusija? d = objava as Diskusija;
                msg = $"Korisnik {korisnik.Ime} je prijavio diskusiju \"{d?.Naslov}\" korisnika {objava.Autor.Ime} kao neprikladnu";
                predmet = await _context.Diskusije
                    .Include(di => di.Predmet)
                    .ThenInclude(p => p.Profesori)
                    .Where(di => di == d)
                    .Select(d => d.Predmet).FirstAsync();
            }
            else
            {
                msg = $"Korisnik {korisnik.Ime} je prijavio komentar korisnika {objava.Autor.Ime} kao neprikladan";
                predmet = await _context.Komentari
                    .Include(k => k.Diskusija)
                    .ThenInclude(d => d.Predmet)
                    .ThenInclude(d => d.Profesori)
                    .Where(k => k.Id == objava.Id)
                    .Select(k => k.Diskusija.Predmet)
                    .FirstAsync();
            }

            predmet?.Profesori.ForEach(p =>
            {
                _context.Obavestenja.Add(new Obavestenje
                {
                    DatumIVreme = DateTime.Now,
                    Korisnik = p,
                    Sadrzaj = $"{msg}\n Razlog: {prijava.Sadrzaj}",
                    Objava = objava
                });
            });

            _context.Obavestenja.Add(new Obavestenje
            {
                DatumIVreme = DateTime.Now,
                Korisnik = administrator,
                Sadrzaj = $"{msg}\n Razlog: {prijava.Sadrzaj}",
                Objava = objava
            });

            await _context.SaveChangesAsync();

            return Ok(new { msg = "Objava uspešno prijavljena" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion prijava

    [HttpPost("dodaj")]
    public async Task<ActionResult<Diskusija>> DodajDiskusiju([FromBody] DiskusijaModel diskusijafb)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email)
                .Include(k => k.PraceneDiskusije)
                .FirstAsync();

            if (korisnik == null)
                return BadRequest(new { msg = "Korisnik ne postoji" });

            if (String.IsNullOrEmpty(diskusijafb.Naslov))
                return BadRequest(new { msg = "Diskusija mora imati naslov" });

            Diskusija nova;
            if (diskusijafb.Tip == "oblast")
            {
                List<Oblast> oblasti = new();
                if (diskusijafb.OblastiIds != null)
                {
                    oblasti = await _context.Oblasti.Where(o => diskusijafb.OblastiIds.Contains(o.Id)).ToListAsync();
                    if (oblasti.Count == 0) return BadRequest(new { msg = "postojati bar jedna oblast" });
                }
                Predmet predmet = await _context.Oblasti.Where(o => o.Id == oblasti[0].Id)
                    .Include(o => o.Predmet)
                    .Select(o => o.Predmet)
                    .FirstAsync();

                if (predmet == null) return BadRequest(new { msg = "Predmet nije pronadjen" });

                nova = new DiskusijaOblast
                {
                    Predmet = predmet,
                    Autor = korisnik,
                    Oblasti = oblasti,
                    Naslov = diskusijafb.Naslov.Trim(),
                    Sadrzaj = diskusijafb.Sadrzaj,
                    DatumKreiranja = DateTime.Now,
                    DatumIzmene = DateTime.Now
                };
            }
            else if (diskusijafb.Tip == "zadatak")
            {
                var zadatak = await _context.Zadaci.FindAsync(diskusijafb.ZadatakId);

                if (zadatak == null)
                    return BadRequest(new { msg = "Zadatak sa zadatim ID-jem ne postoji" });

                Predmet predmet = await _context.Zadaci.Where(z => z.Id == zadatak.Id)
                    .Include(z => z.Oblasti)
                    .ThenInclude(o => o.Predmet)
                    .Select(z => z.Oblasti[0].Predmet)
                    .FirstAsync();

                if (predmet == null) return BadRequest(new { msg = "Predmet nije pronadjen" });

                nova = new DiskusijaZadatak
                {
                    Predmet = predmet,
                    Autor = korisnik,
                    Zadatak = zadatak,
                    Sadrzaj = diskusijafb.Sadrzaj,
                    Naslov = diskusijafb.Naslov.Trim(),
                    DatumKreiranja = DateTime.Now,
                    DatumIzmene = DateTime.Now
                };
            }
            else
            {
                return BadRequest(new { msg = "Nepoznat tip objave" });
            }

            diskusijafb.dodaci.ForEach(dod =>
            {
                _context.Dodaci.Add(new DodatakObjavi
                {
                    Objava = nova,
                    Naziv = dod.Naziv,
                    Sadrzaj = dod.Sadrzaj,
                });
            });

            _context.Diskusije.Add(nova);
            korisnik.PraceneDiskusije.Add(nova);

            var obavestiti = await _context.Studenti.Where(s => diskusijafb.OsobeIds.Contains(s.Id)).ToListAsync();

            obavestiti.ForEach(s =>
            {
                _context.Obavestenja.Add(new Obavestenje
                {
                    Korisnik = s,
                    Objava = nova,
                    DatumIVreme = DateTime.Now,
                    Sadrzaj = $"Korisnik {korisnik.Ime} je pokrenuo novu diskusiju \"{nova.Naslov}\" i očekuje Vašu pomoć!"
                });
            });

            await _context.SaveChangesAsync();

            return Ok(nova);


        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }

    [HttpPut("arhiviraj")]
    public async Task<ActionResult> ArhivirajDiskusiju([FromBody] int diskusijaId)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");
            var korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();
            if (korisnik == null) return BadRequest(new { msg = "Korisnik nije autorizovan" });

            var diskusija = await _context.Diskusije.Where(d => d.Id == diskusijaId)
                .Include(d => d.Autor)
                .Include(d => d.Predmet)
                .ThenInclude(p => p.Profesori)
                .FirstAsync();

            if (diskusija == null) return BadRequest(new { msg = "Diskusija ne postoji" });

            if (korisnik.Tip == "administrator" ||
                diskusija.Predmet.Profesori.FindIndex(p => p.Id == korisnik.Id) != -1)
            {
                diskusija.Arhivirana = !diskusija.Arhivirana;
            }

            if (diskusija.Arhivirana)
            {
                _context.Obavestenja.Add(new Obavestenje
                {
                    Korisnik = diskusija.Autor,
                    Objava = diskusija,
                    Sadrzaj = $"Korisnik {korisnik.Ime} je arhivirao Vašu diskusiju {diskusija.Naslov}",
                    DatumIVreme = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return Ok(diskusija.Arhivirana);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("izmeni-diskusiju")]
    public async Task<ActionResult> IzmenaDiskusije([FromBody] DiskusijaModel diskusijaModel)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null)
                return BadRequest(new { msg = "Korisnik ne postoji" });

            if (String.IsNullOrEmpty(diskusijaModel.Naslov))
                return BadRequest(new { msg = "Diskusija mora imati naslov" });


            if (diskusijaModel.Tip == "oblast")
            {
                if (diskusijaModel.OblastiIds == null) return BadRequest(new { msg = "Mora postojati bar jedna oblast" });
                DiskusijaOblast diskusijaOblast = await _context.DiskusijeZaOblasti
                    .Where(d => d.Id == diskusijaModel.Id)
                    .Include(d => d.Dodaci)
                    .Include(d => d.Oblasti)
                    .FirstAsync();

                if (diskusijaOblast == null)
                {
                    return BadRequest(new { msg = "Nije pronadjena diskusija" });
                }

                var oblasti = await _context.Oblasti
                    .Where(o => diskusijaModel.OblastiIds.Contains(o.Id)).ToListAsync();

                if (oblasti.Count > 0)
                {
                    diskusijaOblast.Oblasti.Clear();
                    diskusijaOblast.Oblasti = oblasti;
                }
                diskusijaOblast.Naslov = diskusijaModel.Naslov;
                diskusijaOblast.Sadrzaj = diskusijaModel.Sadrzaj;
                diskusijaOblast.DatumIzmene = DateTime.Now;

                var dodaci = diskusijaOblast.Dodaci.Where(d => !diskusijaModel.dodaci.Any(n => n.Id == d.Id));
                dodaci.ToList().ForEach(d => _fileService.DeleteFileNoAwait(d.Sadrzaj));
                _context.Dodaci.RemoveRange(dodaci);

                diskusijaModel.dodaci.Where(d => d.Id <= 0).ToList().ForEach(d =>
                {
                    _context.DodaciObjavi.Add(new DodatakObjavi
                    {
                        Objava = diskusijaOblast,
                        Naziv = d.Naziv,
                        Sadrzaj = d.Sadrzaj
                    });
                });

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    diskusijaOblast.DatumIzmene,
                    diskusijaOblast.Dodaci,
                    diskusijaOblast.Oblasti,
                });
            }
            else if (diskusijaModel.Tip == "zadatak")
            {
                DiskusijaZadatak diskusijaZadatak = await _context.DiskusijeZaZadatke
                    .Where(d => d.Id == diskusijaModel.Id)
                    .Include(d => d.Dodaci)
                    .FirstAsync();

                if (diskusijaZadatak == null)
                    return BadRequest(new { msg = "Nije pronadjena diskusija" });

                diskusijaZadatak.Sadrzaj = diskusijaModel.Sadrzaj;
                diskusijaZadatak.Naslov = diskusijaModel.Naslov;
                diskusijaZadatak.DatumIzmene = DateTime.Now;

                var dodaci = diskusijaZadatak.Dodaci.Where(d => !diskusijaModel.dodaci.Any(n => n.Id == d.Id));
                _context.Dodaci.RemoveRange(dodaci);
                diskusijaModel.dodaci.Where(d => d.Id <= 0).ToList().ForEach(d =>
                {
                    _context.DodaciObjavi.Add(new DodatakObjavi
                    {
                        Objava = diskusijaZadatak,
                        Naziv = d.Naziv,
                        Sadrzaj = d.Sadrzaj
                    });
                });

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    diskusijaZadatak.DatumIzmene,
                    diskusijaZadatak.Dodaci,
                });
            }
            else
            {
                return BadRequest(new { msg = "Nepoznat tip objave" });
            }



        }
        catch (Exception e)
        {

            return BadRequest(new { msg = "Exception: " + e.Message });

        }

    }
    //izmena pitanja/ diskusije

    #region Brisanje diskusije
    //brisanje diskusije

    [HttpDelete("{idObjave}")]
    public async Task<ActionResult> BrisanjeDiskusije(int idObjave)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)

                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            var diskusija = await _context.Diskusije
                .Include(d => d.Autor)
                .Include(d => d.Predmet).ThenInclude(p => p.Profesori)
                .Include(d => d.Pratioci)
                .Include(d => d.Komentari)
                .ThenInclude(k => k.Obavestenja)
                .Include(d => d.Komentari)
                .ThenInclude(k => k.Dodaci)
                .Include(d => d.Komentari)
                .ThenInclude(k => k.Zahvalnice)
                .Include(d => d.Zahvalnice)
                .Include(d => d.Dodaci)
                .Include(d => d.Obavestenja)
                .Include(d => (d as DiskusijaOblast).Oblasti)
                .Where(d => d.Id == idObjave)
                .FirstAsync();

            if (diskusija != null)
            {
                if (diskusija.Autor.Id != korisnik.Id && korisnik.Tip != "administrator" && !diskusija.Predmet.Profesori.Contains(korisnik))
                {
                    return BadRequest(new { msg = "Nemate privilegije: niste profesor na predmetu." });
                }
                await _deleteService.ObrisiDiskusiju(diskusija);
                await _context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno uklonjena diskusija" });
            }

            return BadRequest(new { msg = "Trazena diskusija ne postoji" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }
    #endregion

    #region Arhiviranje i Zavrsetak diskuije
    //arhiviranje doskusije

    [HttpPut("arhiviranje/{idObjave}")]

    public async Task<ActionResult<List<Diskusija>>> ArhiviranjeDiskusije(int idObjave)
    {
        try
        {
            Diskusija diskusija = await _context.Diskusije.Where(d => d.Id == idObjave).FirstAsync();

            if (diskusija != null)
            {
                diskusija.Arhivirana = true;
                await _context.SaveChangesAsync();

                return Ok(_context.Diskusije.Include(a => a.Autor)
                    .Where(d => d.Arhivirana == false)
                    .Select(p => new
                    {
                        Naslov = p.Naslov,
                        Sadrzaj = p.Sadrzaj,
                        ImeAutora = p.Autor.Ime,
                        DatumKreiranja = p.DatumKreiranja

                    }).ToList());
            }

            return BadRequest(new { msg = "Ne postoji trazena diskusija" });


        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }

    #endregion

    [HttpGet("predmet/{predmetId}")]
    [Authorize]
    public async Task<ActionResult> DiskusijePredmeta(int predmetId)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null) return BadRequest(new { msg = "Korisnik ne postoji" });

            var diskusijeO = await _context.DiskusijeZaOblasti
                .Where(d => d.Oblasti.Select(o => o.Predmet.Id).Contains(predmetId))
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Oblasti)
                .Include(d => d.Zahvalnice)
                .Select(d => new
                {
                    d.Id,
                    d.Oblasti,
                    Autor = new
                    {
                        d.Autor.Id,
                        d.Autor.Ime,
                        d.Autor.BrZahvalnica,
                        d.Autor.SlikaURL,
                        d.Autor.Tip
                    },
                    d.Zavrsena,
                    d.DatumIzmene,
                    d.Dodaci,
                    d.DatumKreiranja,
                    d.Arhivirana,
                    d.Sadrzaj,
                    d.Naslov,
                    BrZahvalnica = d.Zahvalnice.Count(),
                    Tip = "oblast",
                    BrKomentara = d.KomentariDiskusije.Count,
                    Zahvaljena = d.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    Pracena = d.Pratioci.Any(k => k == korisnik)
                })
                .ToListAsync();

            var diskusijeZ = await _context.DiskusijeZaZadatke
                .Where(d => d.Zadatak.Oblasti.Select(o => o.Predmet.Id).Contains(predmetId))
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Zadatak)
                .ThenInclude(z => z.Oblasti)
                .Select(d => new
                {
                    d.Id,
                    d.Zadatak,
                    Autor = new
                    {
                        d.Autor.Id,
                        d.Autor.Ime,
                        d.Autor.BrZahvalnica,
                        d.Autor.SlikaURL,
                        d.Autor.Tip
                    },
                    d.Zavrsena,
                    d.Dodaci,
                    d.Sadrzaj,
                    BrZahvalnica = d.Zahvalnice.Count(),
                    d.DatumKreiranja,
                    d.DatumIzmene,
                    d.Naslov,
                    d.Arhivirana,
                    Tip = "zadatak",
                    BrKomentara = d.KomentariDiskusije.Count,
                    Zahvaljena = d.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    Pracena = d.Pratioci.Any(k => k == korisnik)
                })
                .ToListAsync();

            var diskusije = ((IEnumerable<dynamic>)diskusijeO).Concat(diskusijeZ)
                            .OrderByDescending(d => ((dynamic)d).DatumKreiranja);

            return Ok(diskusije);

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }

    [HttpPut("zaprati")]
    public async Task<ActionResult> ZapratiOtprati([FromBody] int diskusijaId)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null)
                return BadRequest(new { msg = "Korisnik ne postoji" });

            var objava = await _context.Diskusije.Where(d => d.Id == diskusijaId)
                .Include(d => d.Pratioci)
                .FirstOrDefaultAsync();

            if (objava == null) return BadRequest(new { msg = "Doslo je do greske" });

            if (objava.Pratioci.Contains(korisnik))
            {
                objava.Pratioci.Remove(korisnik);
            }
            else
            {
                objava.Pratioci.Add(korisnik);
            }

            await _context.SaveChangesAsync();

            return Ok(new { msg = "" });
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [Authorize]
    private async Task<DiskusijaZadatakResponse?> GetDiskusijaZaZadatakPomocna(int id, int depth = 0)
    {
        try
        {
            if (depth < 0)
            {
                Console.WriteLine("Depth >= 0");
                return null;
            }

            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
            {
                Console.WriteLine("Korisnik nije autorizovan");
                return null;
            }

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null)
            {
                Console.WriteLine("Korisnik ne postoji");
                return null;
            }

            // depth = 0 => bez komentara
            // depth = 1 => samo direktni odgovori
            // depth = 2, 3, ... => ...
            DiskusijaZadatak diskusija = await _context.DiskusijeZaZadatke
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Pratioci)
                .Include(d => d.Zadatak)
                .ThenInclude(d => d.Oblasti)
                .Include(d => d.Zahvalnice)
                .ThenInclude(z => z.Korisnik) // za svaki slucaj, nzm da li je neophodno
                .SingleAsync(d => d.Id == id);

            DiskusijaZadatakResponse diskusijaResponse = _mapper.Map<DiskusijaZadatakResponse>(diskusija);

            diskusijaResponse.Pracena = diskusija.Pratioci.Contains(korisnik);
            diskusijaResponse.Zahvaljena = diskusija.Zahvalnice.Select(z => z.Korisnik).Contains(korisnik);

            if (depth != 0)
            {
                var komentari = await _context.Komentari.Where(k => k.Objava.Id == id)
                    .Select(GetKomentarProjection(depth, korisnik))
                    .ToListAsync();

                diskusijaResponse.Komentari = komentari;

            }
            else
            {
                diskusija.Komentari = new List<Komentar>();
            }

            var sviKomentari = await _context.Komentari.Where(k => k.Objava.Id == id)
                    .Select(GetKomentarProjection(5, korisnik)) // treba neki max depth
                    .ToListAsync();


            foreach (var kom in sviKomentari)
            {
                diskusijaResponse.BrKomentara += BrojKomentaraUStablu(kom);
            }

            diskusijaResponse.BrZahvalnica = await _context.Zahvalnice.Where(z => z.Objava.Id == id).CountAsync();

            return diskusijaResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    [Authorize]
    private async Task<DiskusijaOblastResponse?> GetDiskusijaZaOblastPomocna(int id, int depth = 0)
    {
        try
        {
            if (depth < 0)
            {
                Console.WriteLine("Depth >= 0");
                return null;
            }

            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
            {
                Console.WriteLine("Korisnik nije autorizovan");
                return null;
            }

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null)
            {
                Console.WriteLine("Korisnik ne postoji");
                return null;
            }

            // depth = 0 => bez komentara
            // depth = 1 => samo direktni odgovori
            // depth = 2, 3, ... => ...

            DiskusijaOblast diskusija = await _context.DiskusijeZaOblasti
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Pratioci)
                .Include(d => d.Oblasti)
                .Include(d => d.Zahvalnice)
                .ThenInclude(z => z.Korisnik) // za svaki slucaj, nzm da li je neophodno
                .SingleAsync(d => d.Id == id);

            DiskusijaOblastResponse diskusijaResponse = _mapper.Map<DiskusijaOblastResponse>(diskusija);

            diskusijaResponse.Pracena = diskusija.Pratioci.Contains(korisnik);
            diskusijaResponse.Zahvaljena = diskusija.Zahvalnice.Select(z => z.Korisnik).Contains(korisnik);

            if (depth != 0)
            {
                var komentari = await _context.Komentari.Where(k => k.Objava.Id == id)
                    .Select(GetKomentarProjection(depth, korisnik))
                    .ToListAsync();

                diskusijaResponse.Komentari = komentari;
            }
            else
            {
                diskusija.Komentari = new List<Komentar>();
            }

            var sviKomentari = await _context.Komentari.Where(k => k.Objava.Id == id)
                    .Select(GetKomentarProjection(5, korisnik)) // treba neki max depth
                    .ToListAsync();


            foreach (var kom in sviKomentari)
            {
                diskusijaResponse.BrKomentara += BrojKomentaraUStablu(kom);
            }

            diskusijaResponse.BrZahvalnica = await _context.Zahvalnice.Where(z => z.Objava.Id == id).CountAsync();

            return diskusijaResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    // rekurzivna funkcija
    private int BrojKomentaraUStablu(Komentar komentar)
    {
        int count = 1; // zbog root komentara

        if (komentar.Komentari is not null)
        {
            foreach (var kom in komentar.Komentari)
            {
                count += BrojKomentaraUStablu(kom);
            }
            return count;
        }
        else return count;
    }


    private static Expression<Func<Komentar, Komentar>> GetKomentarProjection(int maxDepth, Korisnik k, int currentDepth = 0)
    {
        currentDepth++;

        Expression<Func<Komentar, Komentar>> result = komentar => new Komentar()
        {
            Id = komentar.Id,
            Sadrzaj = komentar.Sadrzaj,
            PredlogResenja = komentar.PredlogResenja,
            Arhivirana = komentar.Arhivirana,
            Autor = komentar.Autor,
            BrZahvalnica = komentar.Zahvalnice.Count(),
            DatumIzmene = komentar.DatumIzmene,
            DatumKreiranja = komentar.DatumKreiranja,
            PotvrdilacResenja = komentar.PotvrdilacResenja,
            Objava = komentar.Objava,
            Prihvacen = komentar.Prihvacen,
            Zahvaljena = komentar.Zahvalnice.Any(z => z.Korisnik == k),
            Dodaci = komentar.Dodaci,
            Komentari = currentDepth == maxDepth
            ? new List<Komentar>() // ovde se staje ako je dostignuta zeljena dubina
            : komentar.Komentari.AsQueryable().Select(GetKomentarProjection(maxDepth, k, currentDepth)).ToList()
        };

        return result;
    }

    [HttpGet("zadatak/{zadatakId}")]
    [Authorize]
    public async Task<ActionResult<List<DiskusijaZadatak>>> DiskusijeZadatka(int zadatakId)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).SingleAsync();

            if (korisnik == null) return BadRequest(new { msg = "Korisnik ne postoji" });

            var diskusijeSaResenjima = await _context.DiskusijeZaZadatke
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Zadatak)
                .Where(d => d.Zadatak.Id == zadatakId)
                .Select(d => new
                {
                    d.Id,
                    d.Zadatak,
                    Autor = new
                    {
                        d.Autor.Id,
                        d.Autor.Ime,
                        d.Autor.BrZahvalnica,
                        d.Autor.SlikaURL,
                        d.Autor.Tip
                    },
                    d.Zavrsena,
                    d.Dodaci,
                    d.Sadrzaj,
                    d.BrZahvalnica,
                    d.DatumKreiranja,
                    d.DatumIzmene,
                    d.Naslov,
                    d.Arhivirana,
                    Tip = "zadatak",
                    BrKomentara = d.KomentariDiskusije.Count,
                    Zahvaljena = d.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    Pracena = d.Pratioci.Any(k => k == korisnik)
                })
                .ToListAsync();

            return Ok(diskusijeSaResenjima);

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }

    [HttpGet("resenja-zadatka/{zadatakId}")]
    [Authorize]
    public async Task<ActionResult<List<DiskusijaZadatak>>> DiskusijeSaResenjemZadatka(int zadatakId)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).SingleAsync();

            if (korisnik == null) return BadRequest(new { msg = "Korisnik ne postoji" });

            var diskusijeSaResenjima = await _context.DiskusijeZaZadatke
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Zadatak)
                .Where(d => d.Zadatak.Id == zadatakId && d.KomentariDiskusije.Any(k => k.PotvrdilacResenja != null))
                .Select(d => new
                {
                    d.Id,
                    d.Zadatak,
                    Autor = new
                    {
                        d.Autor.Id,
                        d.Autor.Ime,
                        d.Autor.BrZahvalnica,
                        d.Autor.SlikaURL,
                        d.Autor.Tip
                    },
                    d.Zavrsena,
                    d.Dodaci,
                    d.Sadrzaj,
                    d.BrZahvalnica,
                    d.DatumKreiranja,
                    d.DatumIzmene,
                    d.Naslov,
                    d.Arhivirana,
                    Tip = "zadatak",
                    BrKomentara = d.KomentariDiskusije.Count,
                    Zahvaljena = d.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    Pracena = d.Pratioci.Any(k => k == korisnik)
                })
                .ToListAsync();

            return Ok(diskusijeSaResenjima);

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception" + e.Message });
        }
    }

    [HttpGet("preporucene-diskusije")]
    [Authorize]
    public async Task<ActionResult> PreporuceneDiskusije(int? skip, DateTime? dated)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici
                .Where(k => k.Email == email)
                .Include(k => k.PraceniPredmeti)
                .FirstAsync();

            if (korisnik == null) return BadRequest(new { msg = "Korisnik ne postoji" });

            if (skip is null || skip < 0) skip = 0;

            var diskusije = _context.Diskusije
                .Where(d => korisnik.PraceniPredmeti.Contains(d.Predmet))
                .Where(d => (dated == null || d.DatumKreiranja < dated))
                .Include(d => d.Autor)
                .Include(d => (d as DiskusijaOblast).Oblasti)
                .Include(d => (d as DiskusijaZadatak).Zadatak)
                .Include(d => d.Predmet)
                .Select(d => new
                {
                    d.Id,
                    d.DatumIzmene,
                    d.DatumKreiranja,
                    d.Sadrzaj,
                    BrKomentara = d.KomentariDiskusije.Count,
                    BrZahvalnica = d.Zahvalnice.Count(),
                    Zahvaljena = d.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    Pracena = d.Pratioci.Any(k => k == korisnik),
                    Zavrsena = d.Zavrsena,
                    Naslov = d.Naslov,
                    Autor = new
                    {
                        d.Autor.Id,
                        d.Autor.Ime,
                        d.Autor.BrZahvalnica,
                        d.Autor.SlikaURL,
                        d.Autor.Tip
                    },
                    Predmet = new
                    {
                        d.Predmet.Id,
                        d.Predmet.Naziv
                    },
                    Tip = (d is DiskusijaOblast) ? "oblast" : "zadatak",
                    Oblasti = d is DiskusijaOblast ? (d as DiskusijaOblast).Oblasti : null,
                    Zadatak = d is DiskusijaZadatak ? (d as DiskusijaZadatak).Zadatak : null
                }).OrderByDescending(d => d.DatumKreiranja).Skip(skip.Value).Take(10);

            return Ok(diskusije);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                msg = "Exception" + e.Message
            });
        }
    }

}
