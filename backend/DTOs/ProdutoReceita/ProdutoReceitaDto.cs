namespace backend.DTOs.ProdutoReceita;

public class ProdutoReceitaDto
{
    public int ProdutoId { get; set; }

    public string Produto { get; set; } = string.Empty;

    public int ReceitaId { get; set; }

    public string Receita { get; set; } = string.Empty;
}