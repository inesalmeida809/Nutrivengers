import { useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { useState } from "react";
import Button from "react-bootstrap/Button";
import Offcanvas from "react-bootstrap/Offcanvas";
import "./Dashboard.css";
import avatar3 from "../assets/avatar3.png";
import carrinho from "../assets/carrinho.png";
import { Link } from "react-router-dom";
import lupa from "../assets/lupa.png";
import seta from "../assets/seta.png";
import Modal from "react-bootstrap/Modal";
import { pesquisarProdutos } from "../services/produtosService";

function Dashboard() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const [show, setShow] = useState(false);
  const [showCategorias, setShowCategorias] = useState(false);
  const handleShowCategorias = () => setShowCategorias(true);
  const handleCloseCategorias = () => setShowCategorias(false);
  const [pesquisa, setPesquisa] = useState("");
  const [produtos, setProdutos] = useState([]);
  const [aPesquisar, setAPesquisar] = useState(false);
  const [erroPesquisa, setErroPesquisa] = useState("");

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const handleShow = () => setShow(true);
  const handleClose = () => setShow(false);

  const handlePesquisar = async () => {
    console.log("Cliqiuei no botão de pesquisar com a pesquisa:", pesquisa);
    if (!pesquisa.trim()) {
      setProdutos([]);
      setErroPesquisa("Escreve o nome de um alimento.");
      return;
    }

    try {
      setAPesquisar(true);
      setErroPesquisa("");

      const dados = await pesquisarProdutos(pesquisa.trim());
      setProdutos(dados.products ?? []);
    } catch (erro) {
      console.error("Erro ao pesquisar produtos:", erro);
      setErroPesquisa("Não foi possível carregar os produtos.");
      setProdutos([]);
    } finally {
      setAPesquisar(false);
    }
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
            if (e.key === "Enter") handlePesquisar();
          }}
        />

        <button
          type="button"
          aria-label="Pesquisar"
          onClick={handlePesquisar}
          disabled={aPesquisar}
        >
          <img src={seta} alt="" className="seta" />
        </button>
      </div>

      <div className="category-boxes">
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box"></button>
        <button
          type="button"
          onClick={handleShowCategorias}
          className="category-box"
        >
          {" "}
          Mais
        </button>
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
              <a href="/dashboard">HOME</a>
            </p>
            <p>
              <a href="/produtos">PRODUTOS</a>
            </p>
            <p>
              <a href="/receitas">RECEITAS</a>
            </p>
            <p>
              <a href="/relatorios">RELATORIOS</a>
            </p>

            <button onClick={handleLogout}>TERMINAR SESSAO</button>
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
            {[
              "Laticínios",
              "Frutas",
              "Legumes",
              "Carnes",
              "Cereais",
              "Mercearia",
              "Congelados",
            ].map((categoria) => (
              <button
                key={categoria}
                type="button"
                className="categoria-item"
                onClick={() => {
                  console.log("Categoria selecionada:", categoria);
                  handleCloseCategorias();
                }}
              >
                <span>{categoria}</span>
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
