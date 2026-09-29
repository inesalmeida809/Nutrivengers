namespace backend.DTOs.Relatorio;

public class RelatorioDto
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Nota { get; set; } = string.Empty;

    public List<ProdutoRelatorioDto> Produtos { get; set; } = new();

    public List<ReceitaRelatorioDto> Receitas { get; set; } = new();
}

public class ProdutoRelatorioDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
}

public class ReceitaRelatorioDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
}