namespace backend.DTOs.ProdutoSupermercado;

//cria a associação
public class CreateProdutoSupermercadoDto
{
    public int ProdutoId { get; set; }

    public int SupermercadoId { get; set; }

    public string Disponibilidade { get; set; } = string.Empty;

    public string ProductUrl { get; set; } = string.Empty;
}