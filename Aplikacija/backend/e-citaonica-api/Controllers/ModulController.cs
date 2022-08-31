using e_citaonica_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Controllers
{
    [ApiController]
    [Route("moduli")]
    public class ModulController : Controller
    {
        private ECitaonicaContext _context;

        public ModulController(ECitaonicaContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<List<Modul>>> Moduli()
        {
            try
            {
                return Ok(await _context.Moduli.Select(m => new
                {
                    Id = m.Id,
                    Naziv = m.Naziv

                }).OrderByDescending(m => m.Naziv.IndexOf("Opšti"))
                .ToListAsync());
            }
            catch(Exception e)
            {
                return BadRequest(new { msg = "Exception: " + e.Message });
            }
        }

    }
}
