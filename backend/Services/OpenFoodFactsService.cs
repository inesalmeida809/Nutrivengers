
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace backend.Services;

public class OpenFoodFactsService
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Nutrivengers/1.0 (contact: ines06almeida@gmail.com)"
        );
    }

    public async Task<JsonElement> PesquisarProdutosAsync(
        string pesquisa,
        CancellationToken cancellationToken = default)
    {
        var pedido = new
        {
            q = pesquisa,
            langs = new[] { "pt", "en" },
            page = 1,
            size = 20
        };

        using var resposta = await _httpClient.PostAsJsonAsync(
            "https://search.openfoodfacts.org/search",
            pedido,
            cancellationToken
        );

        if (!resposta.IsSuccessStatusCode)
        {
            var detalhe = await resposta.Content.ReadAsStringAsync(
                cancellationToken
            );

            throw new HttpRequestException(
                $"Open Food Facts respondeu {(int)resposta.StatusCode} " +
                $"({resposta.StatusCode}). Detalhe: {detalhe}",
                null,
                resposta.StatusCode
            );
        }

        await using var stream = await resposta.Content.ReadAsStreamAsync(
            cancellationToken
        );

        using var documento = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken
        );

        return documento.RootElement.Clone();
    }
}
