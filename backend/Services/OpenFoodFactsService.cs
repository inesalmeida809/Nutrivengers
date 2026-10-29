
using System.Net.Http;
using System.Text.Json;

namespace backend.Services;

public class OpenFoodFactsService
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonElement> PesquisarProdutosAsync(
        string pesquisa,
        CancellationToken cancellationToken = default)
    {
        
    var url =
        $"cgi/search.pl?search_terms={Uri.EscapeDataString(pesquisa)}" +
        "&search_simple=1&action=process&json=1" +
        "&page_size=20" +
        "&fields=code,product_name,brands,image_url,nutriscore_grade," +
        "ingredients_text,nutriments";


        using var resposta = await _httpClient.GetAsync(
            url,
            cancellationToken);

        if (!resposta.IsSuccessStatusCode)
{
    var detalhe = await resposta.Content.ReadAsStringAsync(
        cancellationToken);

    throw new HttpRequestException(
        $"Open Food Facts respondeu {(int)resposta.StatusCode} " +
        $"({resposta.StatusCode}). Detalhe: {detalhe}",
        null,
        resposta.StatusCode);
}

        await using var stream = await resposta.Content.ReadAsStreamAsync(
            cancellationToken);

        using var documento = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken);

        return documento.RootElement.Clone();
    }
}
