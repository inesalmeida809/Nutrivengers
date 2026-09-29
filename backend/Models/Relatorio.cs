namespace backend.Models;

public class Relatorio
{
    public int Id { get; set; }

    //Nutricionista responsável pelo relatório
    public int UtilizadorId { get; set; }

    //Nome paciente
    public string Nome { get; set; } = string.Empty;

    public string Nota { get; set; } = string.Empty;

    public Utilizador Utilizador { get; set; } = null!;

    public ICollection<RelatorioProduto> RelatoriosProdutos { get; set; }
    = new List<RelatorioProduto>();

    public ICollection<RelatorioReceita> RelatoriosReceitas { get; set; }
    = new List<RelatorioReceita>();
}