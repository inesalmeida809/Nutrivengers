namespace backend.DTOs.ProdutoSupermercado;

//devolve informacao
public class ProdutoSupermercadoDto
{
    public int Id { get; set; }

    public string Produto { get; set; } = string.Empty;

    public string Supermercado { get; set; } = string.Empty;

    public string Disponibilidade { get; set; } = string.Empty;

    public string ProductUrl { get; set; } = string.Empty;

    public DateTime LastChecked { get; set; }
}