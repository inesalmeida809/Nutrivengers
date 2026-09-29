using backend.Data;
using backend.DTOs.Relatorio;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class RelatorioController : ControllerBase
{
    private readonly AppDbContext _context;

    public RelatorioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarRelatorio(CreateRelatorioDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var utilizador = await _context.Utilizadores
                .FindAsync(dto.UtilizadorId);

            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            foreach (var produtoId in dto.ProdutoIds)
            {
                var produto = await _context.Produtos.FindAsync(produtoId);

                if (produto == null)
                {
                    return NotFound($"Produto com ID {produtoId} não encontrado.");
                }
            }

            foreach (var receitaId in dto.ReceitaIds)
            {
                var receita = await _context.Receitas.FindAsync(receitaId);

                if (receita == null)
                {
                    return NotFound($"Receita com ID {receitaId} não encontrada.");
                }
            }

            var relatorio = new Relatorio
            {
                Nome = dto.Nome,
                Nota = dto.Nota,
                UtilizadorId = dto.UtilizadorId
            };

            _context.Relatorios.Add(relatorio);
            await _context.SaveChangesAsync();

            foreach (var produtoId in dto.ProdutoIds)
            {
                var relatorioProduto = new RelatorioProduto
                {
                    RelatorioId = relatorio.Id,
                    ProdutoId = produtoId
                };

                _context.RelatoriosProdutos.Add(relatorioProduto);
            }

            foreach (var receitaId in dto.ReceitaIds)
            {
                var relatorioReceita = new RelatorioReceita
                {
                    RelatorioId = relatorio.Id,
                    ReceitaId = receitaId
                };

                _context.RelatoriosReceitas.Add(relatorioReceita);
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var relatorioDto = new RelatorioDto
            {
                Id = relatorio.Id,
                UtilizadorId = relatorio.UtilizadorId,
                Nome = relatorio.Nome,
                Nota = relatorio.Nota,

                Produtos = await _context.RelatoriosProdutos
                    .Where(rp => rp.RelatorioId == relatorio.Id)
                    .Select(rp => new ProdutoRelatorioDto
                    {
                        Id = rp.Produto.Id,
                        Nome = rp.Produto.Nome
                    })
                    .ToListAsync(),

                Receitas = await _context.RelatoriosReceitas
                    .Where(rr => rr.RelatorioId == relatorio.Id)
                    .Select(rr => new ReceitaRelatorioDto
                    {
                        Id = rr.Receita.Id,
                        Nome = rr.Receita.Nome
                    })
                    .ToListAsync()
            };

            return CreatedAtAction(
                nameof(ObterRelatorio),
                new { id = relatorio.Id },
                relatorioDto
            );
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterRelatorios()
    {
        var relatorios = await _context.Relatorios
        .Select(r => new RelatorioDto
        {
            Id = r.Id,
            UtilizadorId = r.UtilizadorId,
            Nome = r.Nome,
            Nota = r.Nota,

            Produtos = r.RelatoriosProdutos
                .Select(rp => new ProdutoRelatorioDto
                {
                    Id = rp.Produto.Id,
                    Nome = rp.Produto.Nome
                })
                .ToList(),

            Receitas = r.RelatoriosReceitas
                .Select(rr => new ReceitaRelatorioDto
                {
                    Id = rr.Receita.Id,
                    Nome = rr.Receita.Nome
                })
                .ToList()
        })
        .ToListAsync();

        return Ok(relatorios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterRelatorio(int id)
    {
        var relatorio = await _context.Relatorios
            .Where(r => r.Id == id)
            .Select(r => new RelatorioDto
            {
                Id = r.Id,
                UtilizadorId = r.UtilizadorId,
                Nome = r.Nome,
                Nota = r.Nota,

                Produtos = r.RelatoriosProdutos
                    .Select(rp => new ProdutoRelatorioDto
                    {
                        Id = rp.Produto.Id,
                        Nome = rp.Produto.Nome
                    })
                    .ToList(),

                Receitas = r.RelatoriosReceitas
                    .Select(rr => new ReceitaRelatorioDto
                    {
                        Id = rr.Receita.Id,
                        Nome = rr.Receita.Nome
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (relatorio == null)
        {
            return NotFound("Relatório não encontrado.");
        }

        return Ok(relatorio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarRelatorio(
        int id,
        CreateRelatorioDto dto)
    {
        var relatorio = await _context.Relatorios.FindAsync(id);

        if (relatorio == null)
        {
            return NotFound("Relatório não encontrado.");
        }

        var utilizador = await _context.Utilizadores.FindAsync(dto.UtilizadorId);

        if (utilizador == null)
        {
            return NotFound("Utilizador não encontrado.");
        }

        foreach (var produtoId in dto.ProdutoIds)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);

            if (produto == null)
            {
                return NotFound($"Produto com ID {produtoId} não encontrado.");
            }
        }

        foreach (var receitaId in dto.ReceitaIds)
        {
            var receita = await _context.Receitas.FindAsync(receitaId);

            if (receita == null)
            {
                return NotFound($"Receita com ID {receitaId} não encontrada.");
            }
        }

        relatorio.Nome = dto.Nome;
        relatorio.Nota = dto.Nota;
        relatorio.UtilizadorId = dto.UtilizadorId;

        var produtosAtuais = await _context.RelatoriosProdutos
            .Where(rp => rp.RelatorioId == id)
            .ToListAsync();

        var receitasAtuais = await _context.RelatoriosReceitas
            .Where(rr => rr.RelatorioId == id)
            .ToListAsync();

        _context.RelatoriosProdutos.RemoveRange(produtosAtuais);
        _context.RelatoriosReceitas.RemoveRange(receitasAtuais);

        foreach (var produtoId in dto.ProdutoIds)
        {
            _context.RelatoriosProdutos.Add(new RelatorioProduto
            {
                RelatorioId = id,
                ProdutoId = produtoId
            });
        }

        foreach (var receitaId in dto.ReceitaIds)
        {
            _context.RelatoriosReceitas.Add(new RelatorioReceita
            {
                RelatorioId = id,
                ReceitaId = receitaId
            });
        }

        await _context.SaveChangesAsync();

        return Ok("Relatório atualizado com sucesso.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ApagarRelatorio(int id)
    {
        var relatorio = await _context.Relatorios.FindAsync(id);

        if (relatorio == null)
        {
            return NotFound("Relatório não encontrado.");
        }

        var produtos = await _context.RelatoriosProdutos
            .Where(rp => rp.RelatorioId == id)
            .ToListAsync();

        var receitas = await _context.RelatoriosReceitas
            .Where(rr => rr.RelatorioId == id)
            .ToListAsync();

        _context.RelatoriosProdutos.RemoveRange(produtos);
        _context.RelatoriosReceitas.RemoveRange(receitas);

        _context.Relatorios.Remove(relatorio);

        await _context.SaveChangesAsync();

        return Ok("Relatório apagado com sucesso.");
    }
}