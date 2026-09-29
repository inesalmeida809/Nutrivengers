namespace backend.DTOs.Receita;

public class ReceitaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Instrucoes { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public List<ProdutoReceitaSimplesDto> Produtos { get; set; } = new();
}

public class ProdutoReceitaSimplesDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}