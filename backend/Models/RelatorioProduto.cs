namespace backend.Models;

public class RelatorioProduto
{
    public int RelatorioId { get; set; }

    public int ProdutoId { get; set; }

    public Relatorio Relatorio { get; set; } = null!;

    public Produto Produto { get; set; } = null!;
}