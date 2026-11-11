
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { useEffect, useState } from "react";
import Button from "react-bootstrap/Button";
import Offcanvas from "react-bootstrap/Offcanvas";
import Modal from "react-bootstrap/Modal";
import api from "../services/api";

import "./Dashboard.css";
import avatar3 from "../assets/avatar3.png";
import carrinho from "../assets/carrinho.png";
import lupa from "../assets/lupa.png";
import seta from "../assets/seta.png";

function Dashboard() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const [show, setShow] = useState(false);
  const [showCategorias, setShowCategorias] = useState(false);

  const [pesquisa, setPesquisa] = useState("");
  const [erroPesquisa, setErroPesquisa] = useState("");

  const [categorias, setCategorias] = useState([]);
  const [aCarregarCategorias, setACarregarCategorias] = useState(true);
  const [erroCategorias, setErroCategorias] = useState("");

  const categoriasPrincipais = categorias.slice(0, 4);
  const categoriasRestantes = categorias.slice(4);

  const handleShow = () => setShow(true);
  const handleClose = () => setShow(false);

  const handleShowCategorias = () => setShowCategorias(true);
  const handleCloseCategorias = () => setShowCategorias(false);

  useEffect(() => {
    const carregarCategorias = async () => {
      try {
        setACarregarCategorias(true);
        setErroCategorias("");

        const resposta = await api.get(
          "/pesquisa-produtos/categorias-principais"
        );

        setCategorias(resposta.data || []);
      } catch (error) {
        console.error("Erro ao carregar categorias:", error);
        setErroCategorias("Não foi possível carregar as categorias.");
      } finally {
        setACarregarCategorias(false);
      }
    };

    carregarCategorias();
  }, []);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const handlePesquisar = () => {
    if (!pesquisa.trim()) {
      setErroPesquisa("Escreve o nome de um alimento.");
      return;
    }

    setErroPesquisa("");

    navigate(
      `/pesquisa?termo=${encodeURIComponent(pesquisa.trim())}`
    );
  };

  const handleSelecionarCategoria = (categoria) => {
    handleCloseCategorias();

    navigate(
      `/pesquisa?categoria=${encodeURIComponent(categoria.id)}`
    );
  };

  return (
    <div className="dashboard-container">
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

      <div className="dashboard-content">
        <h1>
          <span className="titulo1">BEM VINDA,</span>
          <br />
          <span className="titulo2">CECILIA</span>
        </h1>
      </div>

      <div className="dashboard-search">
        <img src={lupa} alt="Lupa" className="lupa" />

        <input
          type="search"
          placeholder="Procurar alimentos..."
          aria-label="Pesquisar produtos"
          value={pesquisa}
          onChange={(e) => setPesquisa(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              handlePesquisar();
            }
          }}
        />

        <button
          type="button"
          aria-label="Pesquisar"
          onClick={handlePesquisar}
        >
          <img src={seta} alt="" className="seta" />
        </button>
      </div>

      {erroPesquisa && (
        <p className="erro-pesquisa">{erroPesquisa}</p>
      )}

      <div className="category-boxes">
        {aCarregarCategorias ? (
          <p>A carregar categorias...</p>
        ) : erroCategorias ? (
          <p>{erroCategorias}</p>
        ) : (
          <>
            {categoriasPrincipais.map((categoria) => (
              <button
                key={categoria.id}
                type="button"
                className="category-box"
                onClick={() => handleSelecionarCategoria(categoria)}
              >
                {categoria.nome}
              </button>
            ))}

            {categoriasRestantes.length > 0 && (
              <button
                type="button"
                onClick={handleShowCategorias}
                className="category-box"
              >
                Mais
              </button>
            )}
          </>
        )}
      </div>

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

            <button onClick={handleLogout}>
              TERMINAR SESSAO
            </button>
          </nav>
        </Offcanvas.Body>
      </Offcanvas>

      <Modal
        show={showCategorias}
        onHide={handleCloseCategorias}
        centered
        dialogClassName="categorias-modal-dialog"
      >
        <Modal.Header>
          <Modal.Title>ESCOLHA A CATEGORIA</Modal.Title>

          <button
            type="button"
            className="fechar-modal"
            onClick={handleCloseCategorias}
            aria-label="Fechar"
          >
            ×
          </button>
        </Modal.Header>

        <Modal.Body>
          <div className="categorias-lista">
            {categoriasRestantes.map((categoria) => (
              <button
                key={categoria.id}
                type="button"
                className="categoria-item"
                onClick={() => handleSelecionarCategoria(categoria)}
              >
                <span>{categoria.nome}</span>
                <span className="categoria-seta">›</span>
              </button>
            ))}
          </div>
        </Modal.Body>
      </Modal>
    </div>
  );
}

export default Dashboard;
