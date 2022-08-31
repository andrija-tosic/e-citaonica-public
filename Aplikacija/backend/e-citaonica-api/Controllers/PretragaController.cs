using e_citaonica_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("pretraga")]
    public class PretragaController : Controller
    {
        private ECitaonicaContext _context;
        public PretragaController(ECitaonicaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pretraga([FromBody] string query)
        {
            try
            {
                if (query == string.Empty)
                    return Ok();

                query = query.Trim();


                var korisnici = (await _context.Korisnici.Where(k =>
                k.Ime.Contains(query))
                    .ToListAsync())
                    .Take(5)
                    .OrderBy(s => s.Ime.IndexOf(query));

                var predmeti = (await _context.Predmeti.Where(p =>
                p.Naziv.Contains(query))
                    .ToListAsync())
                    .Take(5)
                    .OrderBy(s => s.Naziv.IndexOf(query));

                return Ok(new
                {
                    Korisnici = korisnici,
                    Predmeti = predmeti
                });

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Exception: " + e.Message
                });
            }
        }
    }
}
