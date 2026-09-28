using backend.Data;
using backend.DTOs.Receita;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceitaController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReceitaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarReceita(CreateReceitaDto dto)
    {
        var receita = new Receita
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Instrucoes = dto.Instrucoes,
            ImageUrl = dto.ImageUrl
        };

        _context.Receitas.Add(receita);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObterReceita),
            new { id = receita.Id },
            receita
        );
    }

    [HttpGet]
    public async Task<IActionResult> ObterReceitas()
    {
        var receitas = await _context.Receitas.ToListAsync();

        return Ok(receitas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterReceita(int id)
    {
        var receita = await _context.Receitas.FindAsync(id);

        if (receita == null)
        {
            return NotFound("Receita não encontrada.");
        }

        return Ok(receita);
    }
}