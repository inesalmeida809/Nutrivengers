using backend.Data;
using backend.DTOs.Produto;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
//só os utilizadores autenticados podem aceder a este controller
[Authorize]
public class ProdutoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarProduto(CreateProdutoDto dto)
    {
        var produto = new Produto
        {
            Nome = dto.Nome,
            Marca = dto.Marca,
            ImageUrl = dto.ImageUrl,
            NutriScore = dto.NutriScore,
            Ingredientes = dto.Ingredientes,
            InformacaoNutricional = dto.InformacaoNutricional
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObterProduto),
            new { id = produto.Id },
            produto
        );
    }

    [HttpGet]
    public async Task<IActionResult> ObterProdutos()
    {
        var produtos = await _context.Produtos.ToListAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        return Ok(produto);
    }
}