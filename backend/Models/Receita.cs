namespace backend.Models;

public class Receita
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Instrucoes { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}