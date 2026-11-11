import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

import Button from "react-bootstrap/Button";
import Offcanvas from "react-bootstrap/Offcanvas";

import ProdutoCard from "../components/ProdutoCard";
import { pesquisarProdutos } from "../services/produtosService";

import avatar3 from "../assets/avatar3.png";
import carrinho from "../assets/carrinho.png";

import "./PesquisaProdutos.css";

function PesquisaProdutos() {
  const { logout } = useAuth();

  const [show, setShow] = useState(false);

  const handleShow = () => setShow(true);
  const handleClose = () => setShow(false);

  const [searchParams] = useSearchParams();
  const pesquisa = searchParams.get("termo") || "";

  const handleLogout = () => {
    logout();
    window.location.href = "/login";
  };

  const [produtos, setProdutos] = useState([]);
  const [aPesquisar, setAPesquisar] = useState(false);
  const [erro, setErro] = useState("");

  // Paginação
  const [pagina, setPagina] = useState(1);
  const [totalPaginas, setTotalPaginas] = useState(1);

  // Quando a pesquisa muda, voltar à primeira página
  useEffect(() => {
    setPagina(1);
  }, [pesquisa]);

  // Carregar produtos da página atual
  useEffect(() => {
    if (!pesquisa.trim()) {
      setProdutos([]);
      setTotalPaginas(1);
      return;
    }

    const carregarProdutos = async () => {
      try {
        setAPesquisar(true);
        setErro("");

        const dados = await pesquisarProdutos(pesquisa, pagina);

        console.log("PÁGINA PEDIDA:", pagina);
        console.log("PÁGINA DEVOLVIDA:", dados.page);
        console.log("TOTAL DE PÁGINAS:", dados.page_count);
        console.log("TOTAL DE PRODUTOS:", dados.count);
        console.log("PRODUTOS RECEBIDOS:", dados.hits?.length);
        console.log("PRIMEIRO PRODUTO:", dados.hits?.[0]?.product_name);

        console.log("Produtos recebidos:", dados.hits?.length);
        console.log("Resposta completa:", dados);

        setProdutos(dados.hits || []);
        setTotalPaginas(dados.page_count || 1);
      } catch (error) {
        console.error("Erro ao carregar produtos:", error);
        setErro("Não foi possível carregar os produtos.");
        setProdutos([]);
        setTotalPaginas(1);
      } finally {
        setAPesquisar(false);
      }
    };

    carregarProdutos();
  }, [pesquisa, pagina]);
  console.log("TOTAL DE PRODUTOS:", produtos.length);

  return (
    <div className="pesquisa-produtos-container">
      {/* NAVBAR */}
      <nav className="navbar navbar-expand-lg">
        <div className="container-fluid">
          <Button className="menu-toggle" onClick={handleShow}>
            <span className="hamburger-lines">
              <span></span>
              <span></span>
              <span></span>
            </span>
          </Button>

          <div className="navbar-brand">
            <Link to="/carrinho">
              <img src={carrinho} alt="Carrinho" className="carrinho" />
            </Link>

            <Link to="/perfil">
              <img src={avatar3} alt="Avatar" className="avatar3" />
            </Link>
          </div>
        </div>
      </nav>

      {/* MENU LATERAL */}
      <Offcanvas show={show} onHide={handleClose} placement="start">
        <Offcanvas.Header closeButton>
          <Offcanvas.Title>
            <span className="sidebar-mercado">MERCADO</span>
            <br />
            <span className="sidebar-nutri">NUTRI</span>
          </Offcanvas.Title>
        </Offcanvas.Header>

        <Offcanvas.Body>
          <nav className="side-menu">
            <p>
              <Link to="/dashboard">HOME</Link>
            </p>

            <p>
              <Link to="/produtos">PRODUTOS</Link>
            </p>

            <p>
              <Link to="/receitas">RECEITAS</Link>
            </p>

            <p>
              <Link to="/relatorios">RELATORIOS</Link>
            </p>

            <button onClick={handleLogout}>TERMINAR SESSAO</button>
          </nav>
        </Offcanvas.Body>
      </Offcanvas>

      {/* RESULTADOS DA PESQUISA */}
      {aPesquisar && <p>A procurar produtos...</p>}

      {erro && <p>{erro}</p>}

      {!aPesquisar && !erro && produtos.length === 0 && (
        <p>Não foram encontrados produtos.</p>
      )}

      {/* GRELHA DE PRODUTOS */}
      <div className="produtos-grid">
        {produtos.map((produto) => (
          <ProdutoCard
            key={produto.code}
            produto={produto}
            onClick={(produto) => console.log("Produto selecionado:", produto)}
          />
        ))}
      </div>

      {/* PAGINAÇÃO */}
      {!erro && produtos.length > 0 && (
        <div className="paginacao">
          <button
            onClick={() => setPagina((paginaAtual) => paginaAtual - 1)}
            disabled={pagina === 1 || aPesquisar}
          >
            Anterior
          </button>

          <span>
            {pagina} de {totalPaginas}
          </span>

          <button
            onClick={() => setPagina((paginaAtual) => paginaAtual + 1)}
            disabled={pagina >= totalPaginas || aPesquisar}
          >
            Seguinte
          </button>
        </div>
      )}
    </div>
  );
}

export default PesquisaProdutos;
