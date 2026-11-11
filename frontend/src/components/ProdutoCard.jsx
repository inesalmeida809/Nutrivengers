
import "./ProdutoCard.css";

import laticinios from "../assets/produtos/laticineos.png";
import generico from "../assets/produtos/generico.png";
import frescos from "../assets/produtos/frescos.png";
import carne from "../assets/produtos/carne.png";
import peixe from "../assets/produtos/peixe.png";
import produtos from "../assets/produtos/produtos.png";
import bebidas from "../assets/produtos/bebidas.png";
import cereais from "../assets/produtos/cereais.png";
import snacks from "../assets/produtos/snacks.png";
import sobremesas from "../assets/produtos/sobremesas.png";
import congelados from "../assets/produtos/congelados.png";
import refeicoes from "../assets/produtos/refeicoes.png";
import condimentos from "../assets/produtos/condimentos.png";
import ovos from "../assets/produtos/ovos.png";
import pastas from "../assets/produtos/pastas.png";
import paes from "../assets/produtos/paes.png";

const imagensCategorias = {
  "en:dairies": laticinios,
  "en:fresh-foods": frescos,
  "en:meats-and-their-products": carne,
  "en:seafood": peixe,
  "en:plant-based-foods-and-beverages": produtos,
  "en:beverages-and-beverages-preparations": bebidas,
  "en:cereals-and-potatoes": cereais,
  "en:snacks": snacks,
  "en:desserts": sobremesas,
  "en:frozen-foods": congelados,
  "en:meals": refeicoes,
  "en:condiments": condimentos,
  "en:eggs-and-their-products": ovos,
  "en:spreads": pastas,
  "en:bread": paes
};

const nomesCategorias = {
  "en:dairies": "Laticínios",
  "en:fresh-foods": "Alimentos frescos",
  "en:meats-and-their-products": "Carne",
  "en:seafood": "Peixe e marisco",
  "en:plant-based-foods-and-beverages": "Produtos de origem vegetal",
  "en:beverages-and-beverages-preparations": "Bebidas",
  "en:cereals-and-potatoes": "Cereais e batatas",
  "en:snacks": "Snacks",
  "en:desserts": "Sobremesas",
  "en:frozen-foods": "Congelados",
  "en:meals": "Refeições",
  "en:condiments": "Condimentos",
  "en:eggs-and-their-products": "Ovos",
  "en:spreads": "Cremes para barrar",
  "en:bread": "Padaria"
};

function obterImagemCategoria(produto) {
  const categorias = produto.categories_tags || [];

  for (const categoria of categorias) {
    if (imagensCategorias[categoria]) {
      return imagensCategorias[categoria];
    }
  }

  return generico;
}

function obterNomeCategoria(produto) {
  const categorias = produto.categories_tags || [];

  for (const categoria of categorias) {
    if (nomesCategorias[categoria]) {
      return nomesCategorias[categoria];
    }
  }

  return "Outros produtos";
}

function ProdutoCard({ produto, onClick }) {
  const imagem =
    produto.image_front_url ||
    produto.image_url ||
    produto.image_small_url;

  const imagemFinal = imagem || obterImagemCategoria(produto);

  const nome =
    produto.product_name_pt ||
    produto.product_name ||
    produto.generic_name_pt ||
    "Produto sem nome";

   if (nome.toLowerCase().includes("manteiga")) {
  console.log("PRODUTO:", nome);
  console.log("CATEGORIAS:", produto.categories_tags);
  console.log("IMAGEM:", {
    front: produto.image_front_url,
    normal: produto.image_url,
    pequena: produto.image_small_url
  });
}
    

  const marca = Array.isArray(produto.brands)
    ? produto.brands.join(", ")
    : produto.brands || "Marca desconhecida";

  const categoria = obterNomeCategoria(produto);

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
        <img src={imagemFinal} alt={nome} />
      </div>

      <div className="produto-card-info">
        <span className="produto-card-marca">{marca}</span>

        <span className="produto-card-categoria">
          {categoria}
        </span>

        <h3>{nome}</h3>

        <p>{produto.quantity || "Quantidade não indicada"}</p>
      </div>
    </article>
  );
}

export default ProdutoCard;
