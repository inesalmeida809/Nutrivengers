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

function Dashboard() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const [show, setShow] = useState(false);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const handleShow = () => setShow(true);
  const handleClose = () => setShow(false);

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

          <a className="navbar-brand" href="#">
            <Link to="/carrinho">
              <img src={carrinho} alt="Carrinho" className="carrinho" />
            </Link>
            <Link to="/perfil">
              <img src={avatar3} alt="Avatar" className="avatar3" />
            </Link>
          </a>
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
        />
        <button type="button" aria-label="Seta">
          <img src={seta} alt="" className="seta" />
        </button>
      </div>

      <div className="category-boxes">
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box"></button>
        <button type="button" className="category-box">
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
    </div>
  );
}

export default Dashboard;
