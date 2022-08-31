using e_citaonica_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_citaonica_api.Controllers;

[ApiController]
[Route("rokovi")]
public class IspitniRokController : Controller
{
	private ECitaonicaContext _context;

	public IspitniRokController(ECitaonicaContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<List<IspitniRok>>> Rokovi()
	{
		try
		{
			return Ok(await _context.IspitniRokovi.OrderByDescending(ir => ir.Naziv.IndexOf("Kolokvijum I")).ToListAsync());
		}
		catch (Exception e)
		{
			return BadRequest(new { msg = "Exception: " + e.Message });
		}

	}

}