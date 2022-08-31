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

namespace e_citaonica_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("zahvalnica")]
    public class ZahvalnicaController : Controller
    {
        private ECitaonicaContext _context;
        private static readonly object _lock = new object();
        public ZahvalnicaController(ECitaonicaContext context)
        {
            _context = context;
        }


        [HttpPut("zahvali")]
        public async Task<ActionResult> DodavanjeZahvalnice([FromBody] int objavaId)
        {
            try
            {
                var email = HttpContext.User.FindFirstValue("preferred_username");

                Console.WriteLine(email);

                if (email == null)
                    return BadRequest(new { msg = "Korisnik nije autorizovan" });

                Korisnik korisnik = await _context.Korisnici.Where(k => k.Email == email).SingleAsync();

                if (korisnik == null)
                    return BadRequest(new { msg = "Korisnik ne postoji" });

                var objava = _context.Objave.Where(o => o.Id == objavaId)
                    .Include(o => o.Zahvalnice)
                    .ThenInclude(z => z.Korisnik)
                    .Include(o => o.Autor)
                    .FirstOrDefault();

                if (objava == null) return BadRequest(new { msg = "Doslo je do greske" });

                var zahvalnica = objava.Zahvalnice.Where(z => z.Korisnik.Id == korisnik.Id).FirstOrDefault();

                if (zahvalnica != null)
                {
                    _context.Zahvalnice.Remove(zahvalnica);
                    objava.BrZahvalnica--;
                    objava.Autor.BrZahvalnica--;
                }
                else
                {
                    objava.Zahvalnice.Add(new Zahvalnica
                    {
                        Korisnik = korisnik,
                        Vrednost = 1
                    });
                    objava.BrZahvalnica++;
                    objava.Autor.BrZahvalnica++;
                }
                await _context.SaveChangesAsync();

                return Ok(new { msg = "" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest(new { msg = "Exception: " + e.Message });
            }
        }

    }
}
