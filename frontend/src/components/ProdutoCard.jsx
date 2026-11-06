
import "./ProdutoCard.css";

function ProdutoCard({ produto, onClick }) {
  const imagem =
    produto.image_front_url ||
    produto.image_url ||
    produto.image_small_url;

  const nome =
    produto.product_name_pt ||
    produto.product_name ||
    produto.generic_name_pt ||
    "Produto sem nome";

  const marca = Array.isArray(produto.brands)
    ? produto.brands.join(", ")
    : produto.brands || "Marca desconhecida";

  return (
    <article
      className="produto-card"
      onClick={() => onClick?.(produto)}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === "Enter") onClick?.(produto);
      }}
    >
      <div className="produto-card-imagem">
        {imagem ? (
          <img src={imagem} alt={nome} />
        ) : (
          <span>Sem imagem</span>
        )}
      </div>

      <div className="produto-card-info">
        <span className="produto-card-marca">{marca}</span>

        <h3>{nome}</h3>

        <p>{produto.quantity || "Quantidade não indicada"}</p>
      </div>
    </article>
  );
}

export default ProdutoCard;
