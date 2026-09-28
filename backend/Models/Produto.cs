namespace backend.Models;

public class Produto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string NutriScore { get; set; } = string.Empty;

    public string Ingredientes { get; set; } = string.Empty;

    public string InformacaoNutricional { get; set; } = string.Empty;

    public ICollection<ProdutoSupermercado> ProdutosSupermercados { get; set; } = new List<ProdutoSupermercado>();

    public ICollection<ProdutoReceita> ProdutosReceitas { get; set; } = new List<ProdutoReceita>();
}