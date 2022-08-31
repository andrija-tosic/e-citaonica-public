using e_citaonica_api.Models;
using e_citaonica_api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace e_citaonica_api.Controllers;

[ApiController]
[Authorize]
[Route("komentari")]
public class KomentarController : Controller
{
    private ECitaonicaContext _context;
    private IDeleteService _deleteService;
    private IFileService _fileService;
    public KomentarController(ECitaonicaContext context, IDeleteService deleteService, IFileService fileService)
    {
        _context = context;
        _deleteService = deleteService;
        _fileService = fileService;
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> ObrisiKomentar(int id)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)

                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik? korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            Komentar kom = await _context.Komentari
                .Where(k => k.Id == id)
                .Include(k => k.Diskusija).ThenInclude(d => d.Predmet).ThenInclude(p => p.Profesori)
                .Include(k => k.Diskusija).ThenInclude(d => d.KomentariDiskusije).ThenInclude(kd => kd.PotvrdilacResenja)
                .Include(k => k.Autor)
                .Include(k => k.Zahvalnice)
                .Include(k => k.Dodaci)
                .Include(k => k.Obavestenja)
                .FirstAsync();

            if (kom.Autor.Id != korisnik.Id && korisnik.Tip != "administrator" && !kom.Diskusija.Predmet.Profesori.Contains(korisnik))
            {
                return BadRequest(new { msg = "Nemate privilegije: niste profesor na predmetu." });
            }


            //if (kom == null) return BadRequest(new { msg = "Komentar ne postoji" });
            await _deleteService.ObrisiPodstablo(kom);

            kom.Diskusija.Zavrsena = kom.Diskusija.KomentariDiskusije.Any(kd => kd.PotvrdilacResenja != null);
            await _context.SaveChangesAsync();

            return Ok(new { msg = "Komentar obrisan" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return BadRequest(new { msg = e.Message });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> DodajKomentar([FromBody] KomentarModel novi)
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

            var objava = await _context.Objave.Where(o => o.Id == novi.ObjavaId)
                .Include(o => o.Autor)
                .FirstAsync();

            if (objava == null)
                return BadRequest(new { msg = "Komentarisana nepostojeca objava" });

            Diskusija? diskusija;
            if (objava is Diskusija)
            {
                diskusija = objava as Diskusija; //root komentar se dodaje
            }
            else if (objava is Komentar)
            {
                diskusija = await _context.Komentari.Where(k => k.Id == objava.Id)
                    .Include(k => k.Diskusija)
                    .Select(k => k.Diskusija)
                    .FirstAsync();
            }
            else
            {
                return BadRequest(new { msg = "Greska neka sta znam" });
            }
            if (diskusija == null)
                return BadRequest(new { msg = "Greska neka sta znam" });

            Komentar komentar = new Komentar
            {
                Autor = korisnik,
                Objava = objava,
                Diskusija = diskusija,
                DatumKreiranja = DateTime.Now,
                DatumIzmene = DateTime.Now,
                Sadrzaj = novi.Sadrzaj,
                BrZahvalnica = 0,
                PredlogResenja = novi.PredlogResenja,
                Prihvacen = false
            };

            _context.Komentari.Add(komentar);

            novi.Dodaci.ForEach(dod =>
            {
                _context.Dodaci.Add(new DodatakObjavi
                {
                    Naziv = dod.Naziv,
                    Objava = komentar,
                    Sadrzaj = dod.Sadrzaj
                });
            });

            Obavestenje obavestenje = new Obavestenje
            {
                DatumIVreme = DateTime.Now,
                Objava = komentar
            };

            if (objava is Diskusija)
            {
                Diskusija odgovorDiskusije = (Diskusija)objava;
                var pratioci = await _context.Diskusije.Where(d => d.Id == objava.Id)
                    .Include(d => d.Pratioci)
                    .Select(d => d.Pratioci).FirstAsync();

                string sliceNaslova = odgovorDiskusije.Naslov
                                        .Substring(0, Math.Min(odgovorDiskusije.Naslov.Length, 10))
                                        + (odgovorDiskusije.Naslov.Length >= 10 ? "..." : "");


                foreach (Korisnik pratioc in pratioci)
                {
                    if (pratioc != objava.Autor)
                    {
                        _context.Obavestenja.Add(new Obavestenje
                        {
                            DatumIVreme = DateTime.Now,
                            Objava = komentar,
                            Korisnik = pratioc,
                            Sadrzaj = $"Korisnik {korisnik.Ime} je dodao komentar Vašoj diskusiji <b>{sliceNaslova}</b>"
                        });
                    }
                }
                if (korisnik != objava.Autor)
                {
                    _context.Obavestenja.Add(new Obavestenje
                    {
                        DatumIVreme = DateTime.Now,
                        Objava = komentar,
                        Korisnik = objava.Autor,
                        Sadrzaj = $"Korisnik {korisnik.Ime} je dodao komentar Vašoj diskusiji <b>{sliceNaslova}</b>"
                    });
                }
            }
            else if (objava is Komentar)
            {

                Komentar odgovorKomentaru = (Komentar)objava;

                if (objava.Autor != korisnik)
                    _context.Obavestenja.Add(new Obavestenje
                    {
                        DatumIVreme = DateTime.Now,
                        Objava = komentar,
                        Sadrzaj = $"Imate novi odgovor od korisnika {korisnik.Ime}",
                        Korisnik = objava.Autor
                    });
            }

            await _context.SaveChangesAsync();

            return Ok(komentar);
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }


    [Authorize]
    [HttpPut("izmeni-komentar")]
    public async Task<ActionResult> IzmenaKomentara([FromBody] KomentarModel komentarModel)
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

            var komentar = await _context.Komentari
                .Where(k => k.Id == komentarModel.Id)
                .Include(k => k.Autor)
                .Include(k => k.Objava)
                .ThenInclude(o => o.Autor)
                .Include(k => k.Dodaci)
                .FirstAsync();

            if (komentar != null)
            {
                if (korisnik == null)
                    return BadRequest(new { msg = "Nije pronadjen korisnik sa navedenim id-jem" });

                komentar.DatumIzmene = DateTime.Now;
                komentar.Sadrzaj = komentarModel.Sadrzaj;

                var dodaci = komentar.Dodaci.Where(d => !komentarModel.Dodaci.Any(n => n.Id == d.Id));
                dodaci.ToList().ForEach(d => _fileService.DeleteFileNoAwait(d.Sadrzaj));
                _context.Dodaci.RemoveRange(dodaci);

                komentarModel.Dodaci.Where(d => d.Id <= 0).ToList().ForEach(d =>
                {
                    _context.DodaciObjavi.Add(new DodatakObjavi
                    {
                        Objava = komentar,
                        Naziv = d.Naziv,
                        Sadrzaj = d.Sadrzaj
                    });
                });

                Obavestenje obavestenje = new Obavestenje
                {
                    DatumIVreme = DateTime.Now,
                    Objava = komentar
                };

                if (komentar.Objava is Diskusija)
                {
                    Diskusija odgovorDiskusije = (Diskusija)komentar.Objava;
                    var pratioci = await _context.Diskusije.Where(d => d.Id == komentar.Objava.Id)
                        .Include(d => d.Pratioci)
                        .Select(d => d.Pratioci).FirstAsync();

                    foreach (Korisnik pratioc in pratioci)
                    {
                        if (pratioc != komentar.Autor)
                        {
                            _context.Obavestenja.Add(new Obavestenje
                            {
                                DatumIVreme = DateTime.Now,
                                Objava = komentar.Objava,
                                Korisnik = pratioc,
                                Sadrzaj = $"Korisnik {korisnik.Ime} je izmenio komentar u objavi koju pratite {odgovorDiskusije.Naslov}"
                            });
                        }
                    }
                    if (korisnik != komentar.Objava.Autor)
                    {
                        _context.Obavestenja.Add(new Obavestenje
                        {
                            DatumIVreme = DateTime.Now,
                            Objava = komentar.Objava,
                            Korisnik = komentar.Objava.Autor,
                            Sadrzaj = $"Korisnik {korisnik.Ime} je izmenio komentar u Vašoj objavi {odgovorDiskusije.Naslov}"
                        });
                    }
                }
                else if (komentar.Objava is Komentar)
                {

                    Komentar odgovorKomentaru = (Komentar)komentar.Objava;

                    _context.Obavestenja.Add(new Obavestenje
                    {
                        DatumIVreme = DateTime.Now,
                        Objava = komentar.Objava,
                        Sadrzaj = $"Imate novi odgovor od korisnika {korisnik.Ime}",
                        Korisnik = komentar.Objava.Autor
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    komentar.Id,
                    komentar.Sadrzaj,
                    komentar.Dodaci,
                    komentar.DatumIzmene
                });

            }

            return BadRequest(new { msg = "Nije pronadjen trazeni komentar" });
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [HttpPut("/arhiviraj/{id}")]

    public async Task<ActionResult> ArhiviranjeKomentara(int id)
    {
        try
        {
            var komentar = await _context.Komentari.Where(k => k.Id == id).FirstAsync();

            if (komentar != null)
            {
                komentar.Arhivirana = true;

                await _context.SaveChangesAsync();

                return Ok(new { msg = "Uspešno arhiviran komentar" });
            }

            return BadRequest(new { msg = "Nije pronadjen trazeni komentar" });
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [Authorize]
    [HttpPut("potvrdjivanje-tacnosti")]
    public async Task<ActionResult> PotvrdjivanjeTacnosti([FromBody] int idKomentara)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Profesor profesor = await _context.Profesori.Where(k => k.Email == email).FirstAsync();

            if (profesor == null)
                return BadRequest(new { msg = "Korisnik nije profesor" });

            var komentar = await _context.Komentari.Where(k => k.Id == idKomentara)
                .Include(k => k.PotvrdilacResenja)
                .Include(k => k.Autor)
                .Include(k => k.Objava)
                .Include(k => k.Diskusija)
                .FirstOrDefaultAsync();

            if (komentar == null)
                return BadRequest(new { msg = "Nije pronadjen komentar" });

            if (komentar.PotvrdilacResenja == null)
            {
                komentar.PotvrdilacResenja = profesor;
                // komentar.Prihvacen = true; trebalo bi samo ako autor diskusije to uradi

                Diskusija diskusija = (Diskusija)komentar.Objava;
                string sliceNaslova = diskusija.Naslov
                                        .Substring(0, Math.Min(diskusija.Naslov.Length, 10))
                                        + (diskusija.Naslov.Length >= 10 ? "..." : "");

                Obavestenje obavestenje = new()
                {
                    DatumIVreme = DateTime.Now,
                    Objava = komentar,
                    Sadrzaj = $"<b>{sliceNaslova}</b>: profesor ${profesor.Ime} je potvrdio Vaše predloženo rešenje .\n",
                    Korisnik = komentar.Autor
                };

                _context.Obavestenja.Add(obavestenje);

                var pratioci = await _context.Diskusije.Where(d => d.Id == komentar.Objava.Id)
                    .Include(d => d.Pratioci)
                    .Select(d => d.Pratioci)
                    .FirstAsync();

                diskusija.Zavrsena = true;

                foreach (Korisnik pratioc in pratioci)
                {
                    _context.Obavestenja.Add(new Obavestenje
                    {
                        DatumIVreme = DateTime.Now,
                        Korisnik = pratioc,
                        Objava = komentar,
                        Sadrzaj = $"Diskusija <b>{sliceNaslova}</b> je dobila potvrđeno rešenje."
                    });
                }
            }
            else
            {
                komentar.PotvrdilacResenja = null;
                // komentar.Prihvacen = false; trebalo bi samo ako autor diskusije to uradi

                Obavestenje obavestenje = new()
                {
                    DatumIVreme = DateTime.Now,
                    Objava = komentar,
                    Sadrzaj = "Profesor " + profesor.Ime + " je uklonio oznaku tačnosti Vašeg predloženog rešenja.\n",
                    Korisnik = komentar.Autor
                };
                _context.Obavestenja.Add(obavestenje);
            }

            await _context.SaveChangesAsync();

            return Ok(new { msg = "Uspesno potvrdjeno resenje" });

        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [Authorize]
    [HttpPut("prihvati-odgovor-toggle")]
    public async Task<ActionResult> PrihvatiOdgovorToggle([FromBody] int idKomentara)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null)
                return BadRequest(new { msg = "Korisnik nije pronadjen" });

            var komentar = await _context.Komentari.Where(k => k.Id == idKomentara)
                .Include(k => k.Autor)
                .Include(k => k.Objava)
                .FirstOrDefaultAsync();

            if (komentar == null)
                return BadRequest(new { msg = "Nije pronadjen komentar" });

            komentar.Prihvacen = !komentar.Prihvacen;

            if (komentar.Prihvacen)
            {
                Obavestenje obavestenje = new()
                {
                    DatumIVreme = DateTime.Now,
                    Objava = komentar,
                    Sadrzaj = "Korisnik " + korisnik.Ime + " je prihvatio Vaše rešenje.\n",
                    Korisnik = komentar.Autor
                };

                _context.Obavestenja.Add(obavestenje);

                Diskusija diskusija = (Diskusija)komentar.Objava;
                var pratioci = await _context.Diskusije.Where(d => d.Id == komentar.Objava.Id)
                    .Include(d => d.Pratioci)
                    .Select(d => d.Pratioci)
                    .FirstAsync();

                foreach (Korisnik pratioc in pratioci)
                {
                    string sliceNaslova = diskusija.Naslov
                                            .Substring(0, Math.Min(diskusija.Naslov.Length, 10))
                                            + (diskusija.Naslov.Length >= 10 ? "..." : "");
                    _context.Obavestenja.Add(new Obavestenje
                    {
                        DatumIVreme = DateTime.Now,
                        Korisnik = pratioc,
                        Objava = komentar,
                        Sadrzaj = $"Diskusija <b>{sliceNaslova}</b> je dobila prihvaćeno rešenje."
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { msg = "Uspešno prihvaćeno rešenje." });

        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return BadRequest(new
            {
                msg = "Exception: " + e.Message
            });
        }
    }
}
