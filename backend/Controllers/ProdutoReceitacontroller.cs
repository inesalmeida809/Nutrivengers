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
        var produto = await _context.Produtos.FindAsync(dto.ProdutoId);

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        var receita = await _context.Receitas.FindAsync(dto.ReceitaId);

        if (receita == null)
        {
            return NotFound("Receita não encontrada.");
        }

        var associacaoExiste = await _context.ProdutosReceitas
            .AnyAsync(pr =>
                pr.ProdutoId == dto.ProdutoId &&
                pr.ReceitaId == dto.ReceitaId);

        if (associacaoExiste)
        {
            return BadRequest("Este produto já está associado a esta receita.");
        }

        var produtoReceita = new ProdutoReceita
        {
            ProdutoId = dto.ProdutoId,
            ReceitaId = dto.ReceitaId
        };

        _context.ProdutosReceitas.Add(produtoReceita);
        await _context.SaveChangesAsync();

        var produtoReceitaDto = new ProdutoReceitaDto
        {
            ProdutoId = produto.Id,
            Produto = produto.Nome,
            ReceitaId = receita.Id,
            Receita = receita.Nome
        };

        return Ok(produtoReceitaDto);
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