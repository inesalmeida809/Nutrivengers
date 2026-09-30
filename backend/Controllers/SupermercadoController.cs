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

        var supermercadoDto = new SupermercadoDto
        {
            Id = supermercado.Id,
            Nome = supermercado.Nome,
            LogoUrl = supermercado.LogoUrl
        };

        return CreatedAtAction(
            nameof(ObterSupermercado),
            new { id = supermercado.Id },
            supermercadoDto
        );
    }

    [HttpGet]
    public async Task<IActionResult> ObterSupermercados()
    {
        var supermercados = await _context.Supermercados.Select(s => new SupermercadoDto
        {
            Id = s.Id,
            Nome = s.Nome,
            LogoUrl = s.LogoUrl
        }).ToListAsync();

        return Ok(supermercados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterSupermercado(int id)
    {
        var supermercado = await _context.Supermercados.Where(s => s.Id == id).Select(s => new SupermercadoDto
        {
            Id = s.Id,
            Nome = s.Nome,
            LogoUrl = s.LogoUrl
        }).FirstOrDefaultAsync();

        if (supermercado == null)
        {
            return NotFound("Supermercado não encontrado.");
        }

        return Ok(supermercado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarSupermercado(int id, CreateSupermercadoDto dto)
    {
        var supermercado = await _context.Supermercados.FindAsync(id);

        if(supermercado == null)
        {
            return NotFound("Supermercado não encontrado.");
        }

        supermercado.Nome = dto.Nome;
        supermercado.LogoUrl = dto.LogoUrl;

        await _context.SaveChangesAsync();

        var supermercadoDto = new SupermercadoDto
        {
            Id = supermercado.Id,
            Nome = supermercado.Nome,
            LogoUrl = supermercado.LogoUrl
        };

        return Ok(supermercadoDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ApagarSupermercado(int id)
    {
        var supermercado = await _context.Supermercados.FindAsync(id);

        if(supermercado == null)
        {
            return NotFound("Supermercado não encontrado.");
        }

        _context.Supermercados.Remove(supermercado);
        await _context.SaveChangesAsync();


        return Ok("Supermercado apagado com sucesso.");
    }
    
}