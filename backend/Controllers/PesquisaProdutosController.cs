
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
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(pesquisa))
        {
            return BadRequest(new
            {
                mensagem = "Indica um produto para pesquisar."
            });
        }

        
try
{
    var resultado = await _service.PesquisarProdutosAsync(
        pesquisa,
        cancellationToken);

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
}
