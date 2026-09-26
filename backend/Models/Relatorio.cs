namespace backend.Models;

public class Relatorio
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Nota { get; set; } = string.Empty;
}