import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import ProdutoCard from "../components/ProdutoCard";
import { pesquisarProdutos } from "../services/produtosService";
import "./PesquisaProdutos.css";

function PesquisaProdutos() {
  const [searchParams] = useSearchParams();
  const pesquisa = searchParams.get("termo") || "";

  const [produtos, setProdutos] = useState([]);
  const [aPesquisar, setAPesquisar] = useState(false);
  const [erro, setErro] = useState("");

  useEffect(() => {
    if (!pesquisa.trim()) {
      setProdutos([]);
      return;
    }

    const carregarProdutos = async () => {
      try {
        setAPesquisar(true);
        setErro("");

        const dados = await pesquisarProdutos(pesquisa);

        console.log("Produtos recebidos:", dados.hits?.length);
        console.log("Resposta completa:", dados);

        setProdutos(dados.hits || []);
      } catch (error) {
        console.error("Erro ao carregar produtos:", error);
        setErro("Não foi possível carregar os produtos.");
        setProdutos([]);
      } finally {
        setAPesquisar(false);
      }
    };

    carregarProdutos();
  }, [pesquisa]);

  return (
    <div className="pesquisa-produtos-container">
      <h1>Resultados da pesquisa</h1>
      <p>
        Resultados para: <strong>{pesquisa}</strong>
      </p>

      {aPesquisar && <p>A procurar produtos...</p>}

      {erro && <p>{erro}</p>}

      {!aPesquisar && !erro && produtos.length === 0 && (
        <p>Não foram encontrados produtos.</p>
      )}

      <div className="produtos-grid">
        {produtos.map((produto) => (
          <ProdutoCard
            key={produto.code}
            produto={produto}
            onClick={(produto) => console.log("Produto selecionado:", produto)}
          />
        ))}
      </div>
    </div>
  );
}

export default PesquisaProdutos;
