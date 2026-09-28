namespace backend.Models;

public class Supermercado
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string LogoUrl { get; set; } = string.Empty;

    public ICollection<ProdutoSupermercado> ProdutosSupermercados { get; set; } = new List<ProdutoSupermercado>();
}