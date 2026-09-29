namespace backend.DTOs.Relatorio;

public class CreateRelatorioDto
{
    public int UtilizadorId { get; set; }

    // Nome do paciente
    public string Nome { get; set; } = string.Empty;

    public string Nota { get; set; } = string.Empty;

    // Produtos escolhidos para o relatório
    public List<int> ProdutoIds { get; set; } = new();

    // Receitas escolhidas para o relatório
    public List<int> ReceitaIds { get; set; } = new();
}