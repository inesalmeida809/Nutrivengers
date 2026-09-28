using backend.Data;
using backend.DTOs.ProdutoSupermercado;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutoSupermercadoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutoSupermercadoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarProdutoSupermercado(
        CreateProdutoSupermercadoDto dto)
    {
        var produto = await _context.Produtos.FindAsync(dto.ProdutoId);

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        var supermercado = await _context.Supermercados.FindAsync(dto.SupermercadoId);

        if (supermercado == null)
        {
            return NotFound("Supermercado não encontrado.");
        }

        var produtoSupermercado = new ProdutoSupermercado
        {
            ProdutoId = dto.ProdutoId,
            SupermercadoId = dto.SupermercadoId,
            Disponibilidade = dto.Disponibilidade,
            ProductUrl = dto.ProductUrl,
            LastChecked = DateTime.UtcNow
        };

        _context.ProdutosSupermercados.Add(produtoSupermercado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObterProdutoSupermercado),
            new { id = produtoSupermercado.Id },
            produtoSupermercado
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterProdutoSupermercado(int id)
    {
        var produtoSupermercado =
            await _context.ProdutosSupermercados.FindAsync(id);

        if (produtoSupermercado == null)
        {
            return NotFound("Associação não encontrada.");
        }

        return Ok(produtoSupermercado);
    }
}