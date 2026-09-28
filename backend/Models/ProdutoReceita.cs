namespace backend.Models;

public class ProdutoReceita
{
    public int ProdutoId { get; set; }

    public int ReceitaId { get; set; }

    public Produto Produto { get; set; } = null!;

    public Receita Receita { get; set; } = null!;
}