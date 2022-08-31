using e_citaonica_api.Models;
using e_citaonica_api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace e_citaonica_api.Controllers;

[ApiController]
[Authorize]
[Route("korisnici")]
public class UserController : ControllerBase
{
    private readonly ECitaonicaContext _context;
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly IDeleteService _deleteService;
    private readonly IConfiguration _configuration;
    public UserController(
        ECitaonicaContext context,
        IUserService userHelper,
        IFileService FileService,
        IDeleteService deleteService,
        IConfiguration configuration)
    {
        this._context = context;
        this._userService = userHelper;
        this._fileService = FileService;
        this._deleteService = deleteService;
        this._configuration = configuration;
    }

    // ako nije preko SSO
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegister user)
    {
        try
        {
            Korisnik? korisnik = await _context.Korisnici
            .Where(u => u.Email == user.Email)
            .SingleOrDefaultAsync();

            if (korisnik is not null)
            {
                if (korisnik.PotvrdjenEmail)
                {
                    return BadRequest(new { msg = "Već postoji nalog sa zadatim email-om." });
                }
                else
                {
                    _context.Korisnici.Remove(korisnik); // bice dodat ponovo
                }
            }

            Korisnik noviKorisnik;

            if (!user.JeProfesor)
            {
                Modul? modul = await _context.Moduli.FindAsync(user.ModulId);

                if (modul is null) return BadRequest(new { msg = "Nije pronadjen modul." });

                noviKorisnik = new Student
                {
                    Ime = user.Ime.Trim() + " " + user.Prezime.Trim(),
                    Email = user.Email.Trim(),
                    Indeks = user.Indeks,
                    Modul = modul,
                    Godina = user.Godina,
                    SlikaURL = string.Empty,
                    LozinkaHash = _userService.CreatePasswordHash(user.Lozinka),
                };

                noviKorisnik.PraceniPredmeti = await _context.Predmeti
                .Include(p => p.Moduli)
                .Where(p => p.Godina >= user.Godina && p.Moduli.Contains(modul)).ToListAsync();
            }
            else
            {
                noviKorisnik = new Profesor
                {
                    Ime = user.Ime.Trim() + " " + user.Prezime.Trim(),
                    Email = user.Email.Trim(),
                    SlikaURL = string.Empty,
                    LozinkaHash = _userService.CreatePasswordHash(user.Lozinka)
                };
            }

            await _context.Korisnici.AddAsync(noviKorisnik);
            await _context.SaveChangesAsync();

            const string subject = "E-Čitaonica verifikacioni email";
            StringBuilder sb = new();
            sb.AppendLine("Dobrodošli u e-čitaonicu.");
            sb.AppendLine("Potvrdite vaš nalog na sledećoj adresi:");

            string token = _userService.CreateToken(noviKorisnik);

            sb.AppendLine($"{_configuration["Client:URL"]}/confirm-email?id={noviKorisnik.Id}&token={token}");
            string body = sb.ToString();

            await _userService.SendEmailAsync(user.Email, subject, body);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest(new { msg = "Doslo je do greske" });
        }

    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ObirisiKorisnika(int id)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return Unauthorized(new { msg = "Nije pronadjen odgovarajuci token claim" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik.Tip != "administrator" && korisnik.Id != id)
            {
                return Unauthorized(new { msg = "Niste autorizovani" });
            }

            await _deleteService.ObrisiKorisnika(id);
            await _context.SaveChangesAsync();

            return Ok(new { msg = "Korisnik obrisan" });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("sso-register")]
    [Authorize]
    public async Task<IActionResult> SsoRegister([FromBody] SsoRegister user)
    {
        ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
        {
            return BadRequest(new { msg = "No token claims" });
        }

        IEnumerable<Claim> claims = identity.Claims;
        var email = claims.First(c => c.Type == "preferred_username").Value;
        var ime = claims.First(c => c.Type == "name").Value;
        var role = claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

        Korisnik noviKorisnik;
        try
        {
            if (_context.Korisnici.Any(u => u.Email == email))
                return BadRequest(new { msg = "Vec postoji nalog sa zadatim email-om" });

            var pwd = new PasswordGenerator.Password(
                includeLowercase: true,
                includeUppercase: true,
                passwordLength: 30,
                includeSpecial: false,
                includeNumeric: true);

            if (role == "student")
            {
                Modul? modul = await _context.Moduli.Include(m => m.Predmeti).Where(m => m.Id == user.ModulId).FirstAsync();
                if (modul is null)
                    return BadRequest(new { msg = "Ne postoji modul" });

                noviKorisnik = new Student
                {
                    Ime = ime,
                    Email = email,
                    Indeks = user.Indeks,
                    Modul = modul,
                    Godina = user.Godina,
                    SlikaURL = string.Empty,
                    LozinkaHash = _userService.CreatePasswordHash(pwd.Next())
                };

                noviKorisnik.PraceniPredmeti = new();
                noviKorisnik.PraceniPredmeti.AddRange(modul.Predmeti.Where(p => p.Godina == user.Godina));
            }
            else
            {
                noviKorisnik = new Profesor
                {
                    Ime = ime,
                    Email = email,
                    SlikaURL = string.Empty,
                    LozinkaHash = _userService.CreatePasswordHash(pwd.Next())
                };
            }

            noviKorisnik.PotvrdjenEmail = true;

            await _context.Korisnici.AddAsync(noviKorisnik);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok(new
        {
            noviKorisnik.Id,
            noviKorisnik.Ime,
            noviKorisnik.Tip,
            noviKorisnik.SlikaURL,
            noviKorisnik.BrZahvalnica
        });
    }

    // ako nije preko SSO
    [AllowAnonymous]
    [HttpPost("confirm-register")]
    public async Task<IActionResult> ConfirmRegister([FromBody] ConfirmRegister confirmRegister)
    {
        try
        {
            Korisnik? k = await _context.Korisnici.FindAsync(confirmRegister.Id);
            if (k is null) return BadRequest(new { msg = "Ne postoji korisnik" });

            if (k.PotvrdjenEmail)
                return BadRequest(new { msg = "Email vec potvrdjen" });

            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(confirmRegister.Token);

            if (k.Email != token.Claims.First(claim => claim.Type == "preferred_username").Value)
                return BadRequest(new { msg = "Nije validan token" });

            k.PotvrdjenEmail = true;

            await _context.SaveChangesAsync();

            return Ok(new { msg = "Potvrdjen email" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.StackTrace);
        }
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin user)
    {
        Korisnik? k = _context.Korisnici.Where(k => k.Email == user.Email).FirstOrDefault();
        if (k is null)
            return BadRequest(new { msg = "Korisnik nije pronadjen" });

        if (!k.PotvrdjenEmail)
            return BadRequest(new { msg = "Email nije potvrdjen." });

        if (!_userService.VerifyPassword(user.Password, k.LozinkaHash))
        {
            return BadRequest(new { msg = "Pogrešan email ili lozinka." });
        }

        string token = _userService.CreateToken(k);

        return Ok(new
        {
            user = new
            {
                k.Id,
                k.Ime,
                k.Tip,
                k.SlikaURL,
                k.BrZahvalnica
            },
            token
        });
    }

    [Authorize]
    [HttpGet("sso-login/{email}")]
    public async Task<IActionResult> SsoLogin(string email)
    {
        Korisnik? k = await _context.Korisnici.Where(k => k.Email == email).FirstOrDefaultAsync();

        if (k != null)
        {
            return Ok(
            new
            {
                k.Id,
                k.Ime,
                k.Tip,
                k.SlikaURL,
                k.BrZahvalnica
            });
        }
        else
        {
            return NotFound(new { msg = "Korisnik sa datom email adresom ne postoji" });
        }
    }

    [HttpPut("promena-lozinke")]
    [Authorize]
    public async Task<IActionResult> PromenaLozinke([FromBody] PromenaLozinke model)
    {
        try
        {
            string email = HttpContext.User.FindFirstValue("preferred_username");

            Korisnik k = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (!_userService.VerifyPassword(model.oldPassword, k.LozinkaHash))
            {
                return BadRequest(new { msg = "Uneta lozinka se ne poklapa sa starom lozinkom." });
            }

            k.LozinkaHash = _userService.CreatePasswordHash(model.newPassword);

            await _context.SaveChangesAsync();

            return Ok(new { msg = "Uspešno promenjena lozinka." });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return BadRequest(new { msg = e.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Korisnik>> GetKorisnik(int id)
    {
        try
        {
            Korisnik? k = await _context.Korisnici.Where(k => k.Id == id)
                .Include(k => k.Objave.Where(o => o is Diskusija))
                .FirstOrDefaultAsync();

            if (k is null) return NotFound(new { msg = "Korisnik nije pronadjen" });

            if (k is Student)
            {
                Student s = await _context.Studenti.Where(s => s.Id == id)
                    .Include(s => s.PraceniPredmeti)
                    .Include(s => s.Modul)
                    .ThenInclude(m => m.Predmeti)
                    .FirstAsync();

                return Ok(new
                {
                    s.Id,
                    s.Email,
                    s.Ime,
                    BrZahvalnica = await _context.Zahvalnice.Where(z => z.Objava.Autor.Id == id).CountAsync(),
                    s.SlikaURL,
                    s.Tip,
                    s.Indeks,
                    Modul = s.Modul.Naziv,
                    s.Godina,
                    s.Objave,
                    BrResenja = await _context.Komentari.Where(k => k.Autor == s && k.PotvrdilacResenja != null).CountAsync(),
                    BrDiskusija = await _context.Diskusije.Where(d => d.Autor == s).CountAsync()
                });
            }
            else if (k is Profesor)
            {
                Profesor p = await _context.Profesori.Where(p => p.Id == id)
                    .Include(p => p.Predmeti)
                    .FirstAsync();
                return Ok(new
                {
                    p.Id,
                    p.Email,
                    p.Ime,
                    BrZahvalnica = await _context.Zahvalnice.Where(z => z.Objava.Autor.Id == id).CountAsync(),
                    p.SlikaURL,
                    p.Tip,
                    p.Objave,
                    Predmeti = p.Predmeti.Select(p => new
                    {
                        p.Id,
                        p.Naziv
                    }),
                    BrResenja = await _context.Komentari.Where(k => k.Autor == p && k.PotvrdilacResenja != null).CountAsync(),
                    BrDiskusija = await _context.Diskusije.Where(d => d.Autor == p).CountAsync()

                });
            }
            else
            {
                return Ok(new
                {
                    k.Id,
                    k.Email,
                    k.Ime,
                    BrZahvalnica = await _context.Zahvalnice.Where(z => z.Objava.Autor.Id == id).CountAsync(),
                    k.SlikaURL,
                    k.Tip,
                    k.Objave,
                    BrResenja = await _context.Komentari.Where(kom => kom.Autor == k && kom.PotvrdilacResenja != null).CountAsync(),
                    BrDiskusija = await _context.Diskusije.Where(d => d.Autor == k).CountAsync()
                });
            }

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = e.StackTrace + "\n" + e.Message });
        }
    }

    [HttpPut("obrisi-sliku")]
    public async Task<ActionResult> ObrisiSliku([FromBody] int id)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");
            if (email == null) return Unauthorized(new { msg = "Greska u autentifikaciji" });
            var korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();
            if (korisnik == null) return Unauthorized(new { msg = "Korisnik nije pronadjen" });

            string rez = "";
            if (korisnik.SlikaURL != "")
               rez = _fileService.DeleteFile(korisnik.SlikaURL);
            Console.WriteLine(rez);

            korisnik.SlikaURL = "";

            await _context.SaveChangesAsync();

            return Ok(new { msg = "Slika obrisana" });

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = e.StackTrace });
        }
    }

    [HttpPost("postavi-sliku/{id}")]
    public async Task<ActionResult> PostaviSliku(IFormFile file, int id)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");
            if (email == null) return Unauthorized(new { msg = "Greska u autentifikaciji" });
            var korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();
            if (korisnik == null) return Unauthorized(new { msg = "Korisnik nije pronadjen" });

            //var korisnik = await _context.Korisnici.FindAsync(id);

            //if (korisnik is null)
            //    return BadRequest(new { msg = "Nije pronadjen korinsik" });

            string rez = "";
            if (korisnik.SlikaURL != "")
                rez = _fileService.DeleteFile(korisnik.SlikaURL);
            Console.WriteLine(rez);

            string newFilePath = await _fileService.SaveImage(file);

            korisnik.SlikaURL = newFilePath;

            await _context.SaveChangesAsync();

            return Ok(new { path = newFilePath });

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = e.StackTrace });
        }
    }

    [Authorize]
    [HttpGet("obavestenja")]
    public async Task<IActionResult> GetObavestenja()
    {
        var email = HttpContext.User.FindFirstValue("preferred_username");

        if (email == null) return Unauthorized(new { msg = "Greska u autentifikaciji" });

        var user = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

        if (user == null) return Unauthorized(new { msg = "Korisnik nije pronadjen" });

        var obavestenjaDisksuije = _context.Obavestenja.Where(o => o.Korisnik == user)
               .Include(o => o.Objava)
               .Where(o => o.Objava is Diskusija)
               .Select(o => new
               {
                   Id = o.Id,
                   o.Sadrzaj,
                   o.DatumIVreme,
                   PredmetId = (o.Objava as Diskusija).Predmet.Id,
                   ObjavaId = o.Objava.Id,
                   DiskusijaId = o.Objava.Id
               });

        var obavestenjaKomentari = _context.Obavestenja.Where(o => o.Korisnik == user)
            .Include(o => o.Objava)
            .Where(o => o.Objava is Komentar)
            .Select(o => new
            {
                Id = o.Id,
                o.Sadrzaj,
                o.DatumIVreme,
                PredmetId = (o.Objava as Komentar).Diskusija.Predmet.Id,
                ObjavaId = o.Objava.Id,
                DiskusijaId = (o.Objava as Komentar).Diskusija.Id
            });

        return Ok(await obavestenjaKomentari
                            .Concat(obavestenjaDisksuije)
                            .OrderByDescending(o => o.DatumIVreme)
                            .ToListAsync());
    }

    [HttpGet("profesori")]
    public async Task<ActionResult<List<Profesor>>> GetProfesori()
    {
        try
        {
            return Ok(await _context.Profesori.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }


    [HttpGet("profesori-koji-nisu-sa-predmeta/{predmetId}")]
    public async Task<ActionResult<List<Profesor>>> ProfesoriKojiNisuSaPredmeta(int predmetId)
    {
        try
        {
            var profesori = await _context.Profesori.Where(p => !p.Predmeti.Select(p => p.Id).Contains(predmetId))
                .OrderBy(p => p.Ime)
                .ToListAsync();

            return Ok(profesori);
        }
        catch (Exception e)
        {
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    #region Brisanje obavestenja
    [Authorize]
    [HttpDelete("obavestenja/{id}")]
    public async Task<ActionResult> ObrisiObavestenje(int id)
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

            var obavestenje = await _context.Obavestenja.Where(o => o.Id == id).FirstAsync();
            if (obavestenje != null)
            {

                korisnik.Obavestenja.Remove(obavestenje);
                _context.Obavestenja.Remove(obavestenje);
                await _context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno obrisano obavestenje" });


            }

            return BadRequest(new { msg = "Nije pronadjeno obavestenje" });

        }
        catch (Exception e)
        {

            return BadRequest(new { msg = "Exception: " + e.Message });
        }

    }
    #endregion


    [Authorize]
    [HttpGet("diskusije/{korisnikId}")]
    public async Task<ActionResult<List<Diskusija>>> DiskusijeKorisnika(int korisnikId)
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
                .Where(d => d.Autor.Id == korisnikId)
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Oblasti)
                .ThenInclude(o => o.Predmet)
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
                    d.DatumIzmene,
                    d.Dodaci,
                    d.DatumKreiranja,
                    d.Arhivirana,
                    d.Sadrzaj,
                    d.Naslov,
                    BrZahvalnica = _context.Zahvalnice.Where(z => z.Objava.Id == d.Id).Count(),
                    Predmet = d.Oblasti != null && d.Oblasti.Count() > 0 ? d.Oblasti[0].Predmet : null,
                    Tip = "oblast",
                    BrKomentara = d.Komentari.Count,
                    Zahvaljena = d.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    Pracena = d.Pratioci.Any(k => k == korisnik)
                })
                .ToListAsync();

            var diskusijeZ = await _context.DiskusijeZaZadatke
                .Where(d => d.Autor.Id == korisnikId)
                .Include(d => d.Autor)
                .Include(d => d.Dodaci)
                .Include(d => d.Zadatak)
                .ThenInclude(z => z.Oblasti)
                .ThenInclude(o => o.Predmet)
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
                    BrZahvalnica = _context.Zahvalnice.Where(z => z.Objava.Id == d.Id).Count(),
                    d.DatumKreiranja,
                    d.DatumIzmene,
                    d.Naslov,
                    d.Arhivirana,
                    Predmet = d.Zadatak.Oblasti != null && d.Zadatak.Oblasti.Count() > 0 ? d.Zadatak.Oblasti[0].Predmet : null,
                    Tip = "zadatak",
                    BrKomentara = d.Komentari.Count,
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
            return BadRequest(new { msg = "Exception: " + e.Message });
        }
    }

    [HttpGet("objave/{id}")]
    [Authorize]
    public async Task<ActionResult> ObjaveKorisnikaSplit(int id, int? disskip, int? komskip, DateTime? disod, DateTime? komod)
    {
        try
        {
            var email = HttpContext.User.FindFirstValue("preferred_username");

            Console.WriteLine(email);

            if (email == null)
                return BadRequest(new { msg = "Korisnik nije autorizovan" });

            Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).FirstAsync();

            if (korisnik == null) return BadRequest(new { msg = "Korisnik nije autorizovan" });

            var diskusije = _context.Diskusije
                .Where(d => d.Autor.Id == id && (disod == null || d.DatumKreiranja < disod))
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
                    BrKomentara = d.KomentariDiskusije.Count(),
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
                }).OrderByDescending(d => d.DatumKreiranja).Skip(disskip != null ? disskip.Value : 0).Take(10);

            var komentari = _context.Komentari
                .Where(k => k.Autor.Id == id)
                .Where(k => (komod == null || k.DatumKreiranja < komod))
                .Include(k => k.PotvrdilacResenja)
                .Include(k => k.Autor)
                .Include(k => k.Dodaci)
                .Include(k => k.Diskusija)
                .ThenInclude(k => k.Predmet)
                .Select(k => new
                {
                    k.Id,
                    k.DatumIzmene,
                    k.DatumKreiranja,
                    k.Sadrzaj,
                    k.Dodaci,
                    k.PredlogResenja,
                    BrZahvalnica = k.Zahvalnice.Count(),
                    Zahvaljena = k.Zahvalnice.Any(z => z.Korisnik == korisnik),
                    k.PotvrdilacResenja,
                    Autor = new
                    {
                        k.Autor.Id,
                        k.Autor.Ime,
                        k.Autor.BrZahvalnica,
                        k.Autor.SlikaURL,
                        k.Autor.Tip
                    },
                    Diskusija = new
                    {
                        k.Diskusija.Id,
                        k.Diskusija.Naslov,
                        Predmet = new
                        {
                            k.Diskusija.Predmet.Id,
                            k.Diskusija.Predmet.Naziv
                        }
                    }
                }).OrderByDescending(k => k.DatumKreiranja).Skip(komskip != null ? komskip.Value : 0).Take(10);

            var objave = ((IEnumerable<dynamic>)diskusije).Concat(komentari)
                .OrderByDescending(d => ((dynamic)d).DatumKreiranja).Take(10);

            return Ok(objave);

        }
        catch (Exception e)
        {
            return BadRequest(new { msg = e.Message });
        }
    }
}
