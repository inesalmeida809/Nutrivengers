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

        var produtoDto = new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Marca = produto.Marca,
            ImageUrl = produto.ImageUrl,
            NutriScore = produto.NutriScore,
            Ingredientes = produto.Ingredientes,
            InformacaoNutricional = produto.InformacaoNutricional
        };

        return CreatedAtAction(
            nameof(ObterProduto),
            new { id = produto.Id },
            produtoDto
        );
    }

    [HttpGet]
    public async Task<IActionResult> ObterProdutos(string? nome)
    {
        var query = _context.Produtos.AsQueryable();

        // Filtrar por nome se pesquisado
        if(!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(p => p.Nome.ToLower().Contains(nome.ToLower()));
        }

        var produtos = await query
        .Select(p => new ProdutoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Marca = p.Marca,
            ImageUrl = p.ImageUrl,
            NutriScore = p.NutriScore,
            Ingredientes = p.Ingredientes,
            InformacaoNutricional = p.InformacaoNutricional
        })
        .ToListAsync();

    return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterProduto(int id)
    {
        var produto = await _context.Produtos.Where(p => p.Id == id).Select(p => new ProdutoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Marca = p.Marca,
            ImageUrl = p.ImageUrl,
            NutriScore = p.NutriScore,
            Ingredientes = p.Ingredientes,
            InformacaoNutricional = p.InformacaoNutricional
        }).FirstOrDefaultAsync();

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        return Ok(produto);
    }
}