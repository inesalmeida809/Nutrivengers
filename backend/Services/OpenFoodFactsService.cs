
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace backend.Services;

public class OpenFoodFactsService
{
    private readonly HttpClient _httpClient;

    private const int ProdutosPorPagina = 20;
    private static readonly TimeSpan CacheDuracao = TimeSpan.FromMinutes(5);

    private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new();
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Travas = new();

    private sealed class CacheEntry
    {
        public string Dados { get; set; } = "";
        public DateTime ExpiraEm { get; set; }
    }

    public OpenFoodFactsService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Nutrivengers/1.0 (contact: ines06almeida@gmail.com)"
            );
        }
    }

    public async Task<JsonElement> PesquisarProdutosAsync(
        string pesquisa,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        string chave = pesquisa.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(chave))
            throw new ArgumentException("A pesquisa não pode estar vazia.");

        string dadosCacheados = await ObterResultadosOrdenadosAsync(
            chave,
            cancellationToken
        );

        using var documento = JsonDocument.Parse(dadosCacheados);

        var resultado = JsonNode.Parse(
            documento.RootElement.GetRawText()
        )!.AsObject();

        var todosProdutos = resultado["hits"] as JsonArray
            ?? new JsonArray();

        int totalProdutos = todosProdutos.Count;

        int totalPaginas = Math.Max(
            1,
            (int)Math.Ceiling(totalProdutos / (double)ProdutosPorPagina)
        );

        int paginaValida = Math.Clamp(pagina, 1, totalPaginas);

        var produtosPagina = new JsonArray();

        foreach (var produto in todosProdutos
            .Skip((paginaValida - 1) * ProdutosPorPagina)
            .Take(ProdutosPorPagina))
        {
            produtosPagina.Add(produto?.DeepClone());
        }

        resultado["hits"] = produtosPagina;
        resultado["page"] = paginaValida;
        resultado["page_size"] = ProdutosPorPagina;
        resultado["page_count"] = totalPaginas;
        resultado["count"] = totalProdutos;

        return JsonSerializer.SerializeToElement(resultado);
    }

    private async Task<string> ObterResultadosOrdenadosAsync(
        string pesquisa,
        CancellationToken cancellationToken)
    {
        if (Cache.TryGetValue(pesquisa, out var entrada) &&
            entrada.ExpiraEm > DateTime.UtcNow)
        {
            return entrada.Dados;
        }

        var trava = Travas.GetOrAdd(
            pesquisa,
            _ => new SemaphoreSlim(1, 1)
        );

        await trava.WaitAsync(cancellationToken);

        try
        {
            if (Cache.TryGetValue(pesquisa, out entrada) &&
                entrada.ExpiraEm > DateTime.UtcNow)
            {
                return entrada.Dados;
            }

            var primeiraPagina = await ObterPaginaExternaAsync(
                pesquisa,
                1,
                cancellationToken
            );

            var primeiroResultado = JsonNode.Parse(
                primeiraPagina.GetRawText()
            )!.AsObject();

            int totalPaginasOriginais =
                primeiroResultado["page_count"]?.GetValue<int>() ?? 1;

            var produtos = new List<(JsonNode Produto, int Ordem)>();

            int ordem = 0;

            if (primeiroResultado["hits"] is JsonArray primeiros)
            {
                foreach (var produto in primeiros)
                {
                    if (produto != null)
                    {
                        produtos.Add((produto.DeepClone(), ordem++));
                    }
                }
            }

            // Recolher todas as páginas da pesquisa
            for (int pagina = 2; pagina <= totalPaginasOriginais; pagina++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var resposta = await ObterPaginaExternaAsync(
                    pesquisa,
                    pagina,
                    cancellationToken
                );

                if (resposta.TryGetProperty("hits", out var hits) &&
                    hits.ValueKind == JsonValueKind.Array)
                {
                    foreach (var produto in hits.EnumerateArray())
                    {
                        var produtoNode = JsonNode.Parse(
                            produto.GetRawText()
                        );

                        if (produtoNode != null)
                        {
                            produtos.Add((produtoNode, ordem++));
                        }
                    }
                }
            }

            // Ordenação global dos produtos
            var produtosOrdenados = produtos
                .OrderByDescending(p =>
                    PontuacaoPortugal(p.Produto)
                )
                .ThenByDescending(p =>
                    PontuacaoRelevancia(p.Produto, pesquisa)
                )
                .ThenByDescending(p =>
                    PontuacaoCompletude(p.Produto)
                )
                .ThenBy(p => p.Ordem)
                .Select(p => p.Produto.DeepClone());

            var todosOsProdutos = new JsonArray();

            foreach (var produto in produtosOrdenados)
            {
                todosOsProdutos.Add(produto);
            }

            primeiroResultado["hits"] = todosOsProdutos;
            primeiroResultado["page"] = 1;
            primeiroResultado["page_size"] = ProdutosPorPagina;

            primeiroResultado["page_count"] = Math.Max(
                1,
                (int)Math.Ceiling(
                    produtos.Count / (double)ProdutosPorPagina
                )
            );

            primeiroResultado["count"] = produtos.Count;

            string dados = primeiroResultado.ToJsonString();

            Cache[pesquisa] = new CacheEntry
            {
                Dados = dados,
                ExpiraEm = DateTime.UtcNow.Add(CacheDuracao)
            };

            return dados;
        }
        finally
        {
            trava.Release();
        }
    }

    private async Task<JsonElement> ObterPaginaExternaAsync(
        string pesquisa,
        int pagina,
        CancellationToken cancellationToken)
    {
        string pesquisaCodificada = Uri.EscapeDataString(pesquisa);

        string url =
            "https://search.openfoodfacts.org/search" +
            $"?q={pesquisaCodificada}" +
            $"&page={pagina}" +
            $"&page_size={ProdutosPorPagina}" +
            $"&langs=pt,en" +
            "&fields=code,product_name,product_name_pt,brands,quantity,categories_tags,countries_tags,image_front_url,image_url,image_small_url";

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

    // Prioridade geográfica: produtos comercializados em Portugal
    private static int PontuacaoPortugal(JsonNode produto)
    {
        if (produto["countries_tags"] is not JsonArray paises)
            return 0;

        bool comercializadoEmPortugal = paises.Any(p =>
            string.Equals(
                p?.GetValue<string>(),
                "en:portugal",
                StringComparison.OrdinalIgnoreCase
            )
        );

        return comercializadoEmPortugal ? 8 : 0;
    }

    // Relevância do nome do produto face à pesquisa
    private static int PontuacaoRelevancia(
        JsonNode produto,
        string pesquisa)
    {
        string nome =
            ObterTexto(produto["product_name_pt"]) ??
            ObterTexto(produto["product_name"]) ??
            "";

        string nomeNormalizado = nome.Trim().ToLowerInvariant();
        string pesquisaNormalizada = pesquisa.Trim().ToLowerInvariant();

        if (nomeNormalizado == pesquisaNormalizada)
            return 5;

        if (nomeNormalizado.StartsWith(pesquisaNormalizada))
            return 4;

        if (nomeNormalizado.Contains(pesquisaNormalizada))
            return 3;

        return 0;
    }

    // Completude da informação do produto
    private static int PontuacaoCompletude(JsonNode produto)
    {
        int pontuacao = 0;

        // Fotografia
        if (TemTexto(produto["image_front_url"]) ||
            TemTexto(produto["image_url"]) ||
            TemTexto(produto["image_small_url"]))
        {
            pontuacao += 4;
        }

        // Categoria
        if (produto["categories_tags"] is JsonArray categorias &&
            categorias.Count > 0)
        {
            pontuacao += 3;
        }

        // Marca
        if (produto["brands"] is JsonArray marcas &&
            marcas.Count > 0)
        {
            pontuacao += 2;
        }

        // Quantidade
        if (TemTexto(produto["quantity"]))
        {
            pontuacao += 1;
        }

        return pontuacao;
    }

    private static string? ObterTexto(JsonNode? campo)
    {
        if (campo is JsonValue valor &&
            valor.TryGetValue<string>(out var texto))
        {
            return texto;
        }

        return null;
    }

    private static bool TemTexto(JsonNode? campo)
    {
        return !string.IsNullOrWhiteSpace(ObterTexto(campo));
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
            ["en:eggs-and-their-products"] = "À base de ovos",
            ["en:spreads"] = "Cremes para barrar",
            ["en:bread"] = "Padaria"
        };
    }
}
