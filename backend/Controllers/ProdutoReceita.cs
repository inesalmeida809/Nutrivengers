using backend.Data;
using backend.DTOs.ProdutoReceita;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutoReceitaController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutoReceitaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarProdutoReceita(
        CreateProdutoReceitaDto dto)
    {
        var produtoReceita = new ProdutoReceita
        {
            ProdutoId = dto.ProdutoId,
            ReceitaId = dto.ReceitaId
        };

        _context.ProdutosReceitas.Add(produtoReceita);
        await _context.SaveChangesAsync();

        return Ok(produtoReceita);
    }

    [HttpGet]
    public async Task<IActionResult> ObterProdutosReceitas()
    {
        var produtosReceitas = await _context.ProdutosReceitas
            .Select(pr => new ProdutoReceitaDto
            {
                ProdutoId = pr.ProdutoId,
                Produto = pr.Produto.Nome,
                ReceitaId = pr.ReceitaId,
                Receita = pr.Receita.Nome
            })
            .ToListAsync();

        return Ok(produtosReceitas);
    }
}