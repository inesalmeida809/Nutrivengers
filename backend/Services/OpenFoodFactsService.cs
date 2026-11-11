
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

    
public async Task<JsonElement> ObterCategoriasAsync(
    CancellationToken cancellationToken = default)
{
    var pedido = new
{
    sort_by = "code",
    page = 1,
    page_size = 1,
    langs = new[] { "pt", "en" },
    facets = new[] { "categories_tags" }
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
            $"Erro ao obter categorias: {(int)resposta.StatusCode}. " +
            detalhe,
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

public async Task<JsonElement> ObterTaxonomiaCategoriasAsync(
    CancellationToken cancellationToken = default)
{
    const string url =
        "https://world.openfoodfacts.org/api/v2/taxonomy" +
        "?tagtype=categories" +
        "&fields=name,parents" +
        "&include_root_entries=1" +
        "&lc=pt";

    using var resposta = await _httpClient.GetAsync(
        url,
        cancellationToken
    );

    if (!resposta.IsSuccessStatusCode)
    {
        var detalhe = await resposta.Content.ReadAsStringAsync(
            cancellationToken
        );

        throw new HttpRequestException(
            $"Erro ao obter a taxonomia: {(int)resposta.StatusCode}. " +
            detalhe,
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
public Dictionary<string, string> ObterNomesCategoriasPrincipais()
{
    return new Dictionary<string, string>
    {
        ["en:dairies"] = "Laticínios",
        ["en:fresh-foods"] = "Frescos",
        ["en:meats-and-their-products"] = "Carne",
        ["en:seafood"] = "Peixe",
        ["en:plant-based-foods-and-beverages"] = "Produtos vegetais",
        ["en:beverages-and-beverages-preparations"] = "Bebidas",
        ["en:cereals-and-potatoes"] = "Cereais e batatas",
        ["en:snacks"] = "Snacks",
        ["en:desserts"] = "Sobremesas",
        ["en:frozen-foods"] = "Congelados",
        ["en:meals"] = "Refeições",
        ["en:condiments"] = "Condimentos",
        ["en:eggs-and-their-products"] = "Á base de ovos",
        ["en:spreads"] = "Cremes para barrar",
        ["en:bread"] = "Padaria"
    };
}


}
