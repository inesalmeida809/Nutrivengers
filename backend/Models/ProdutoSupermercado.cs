namespace backend.Models;

public class ProdutoSupermercado
{
    public int Id { get; set; }

    public int ProdutoId { get; set; }

    public int SupermercadoId { get; set; }

    public string Disponibilidade { get; set; } = string.Empty;

    public string ProductUrl { get; set; } = string.Empty;

    public DateTime LastChecked { get; set; }

    public Produto Produto { get; set; } = null!;

    public Supermercado Supermercado { get; set; } = null!;
}