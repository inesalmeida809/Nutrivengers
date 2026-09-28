using backend.Data;
using backend.DTOs.Supermercado;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupermercadoController : ControllerBase
{
    private readonly AppDbContext _context;

    public SupermercadoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarSupermercado(CreateSupermercadoDto dto)
    {
        var supermercado = new Supermercado
        {
            Nome = dto.Nome,
            LogoUrl = dto.LogoUrl,
        };

        _context.Supermercados.Add(supermercado);
        await _context.SaveChangesAsync();

        return Ok(supermercado);
    }

    [HttpGet]
    public async Task<IActionResult> ObterSupermercados()
    {
        var supermercados = await _context.Supermercados.ToListAsync();

        return Ok(supermercados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterSupermercado(int id)
    {
        var supermercado = await _context.Supermercados.FindAsync(id);

        if (supermercado == null)
        {
            return NotFound("Supermercado não encontrado.");
        }

        return Ok(supermercado);
    }
}