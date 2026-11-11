
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/pesquisa-produtos")]
public class PesquisaProdutosController : ControllerBase
{
    private readonly OpenFoodFactsService _service;

    public PesquisaProdutosController(OpenFoodFactsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Pesquisar(
        [FromQuery] string pesquisa,
        [FromQuery] int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pesquisa))
        {
            return BadRequest(new
            {
                mensagem = "Indica um produto para pesquisar."
            });
        }

        if (pagina < 1)
        {
            return BadRequest(new
            {
                mensagem = "O número da página tem de ser superior a zero."
            });
        }

        try
        {
            var resultado = await _service.PesquisarProdutosAsync(
                pesquisa,
                pagina,
                cancellationToken
            );

            return Ok(resultado);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new
            {
                mensagem = "Não foi possível obter os produtos da fonte externa.",
                detalhe = ex.Message,
                codigoHttp = ex.StatusCode?.ToString()
            });
        }
    }

    
[HttpGet("categorias")]
public async Task<IActionResult> ObterCategorias(
    CancellationToken cancellationToken = default)
{
    try
    {
        var categorias = await _service.ObterCategoriasAsync(
            cancellationToken
        );

        return Ok(categorias);
    }
    catch (HttpRequestException ex)
    {
        return StatusCode(502, new
        {
            mensagem = "Não foi possível obter as categorias.",
            detalhe = ex.Message
        });
    }
}


[HttpGet("taxonomia-categorias")]
public async Task<IActionResult> ObterTaxonomiaCategorias(
    CancellationToken cancellationToken = default)
{
    try
    {
        var taxonomia = await _service.ObterTaxonomiaCategoriasAsync(
            cancellationToken
        );

        return Ok(taxonomia);
    }
    catch (HttpRequestException ex)
    {
        return StatusCode(502, new
        {
            mensagem = "Não foi possível obter a taxonomia das categorias.",
            detalhe = ex.Message
        });
    }
}
[HttpGet("categorias-principais")]
public IActionResult ObterCategoriasPrincipais()
{
    var categorias = _service.ObterNomesCategoriasPrincipais();

    return Ok(categorias.Select(c => new
    {
        id = c.Key,
        nome = c.Value
    }));
}


}
