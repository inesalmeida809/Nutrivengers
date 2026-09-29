namespace backend.Models;

public class RelatorioReceita
{
    public int RelatorioId { get; set; }

    public int ReceitaId { get; set; }

    public Relatorio Relatorio { get; set; } = null!;

    public Receita Receita { get; set; } = null!;
}