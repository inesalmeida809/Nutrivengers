
using System.Net.Http;
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
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        // Preparar os parâmetros da pesquisa
        string pesquisaCodificada = Uri.EscapeDataString(pesquisa);

        string url =
            $"https://search.openfoodfacts.org/search" +
            $"?q={pesquisaCodificada}" +
            $"&page={pagina}" +
            $"&page_size=20" +
            $"&langs=pt,en";

        // Enviar pedido GET
        using var resposta = await _httpClient.GetAsync(
            url,
            cancellationToken
        );

        // Verificar se o pedido foi bem-sucedido
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

        // Ler a resposta
        await using var stream = await resposta.Content.ReadAsStreamAsync(
            cancellationToken
        );

        using var documento = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken
        );

        // Devolver o resultado
        return documento.RootElement.Clone();
    }
}
