import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import "./Login.css";
import cadeado from "../assets/cadeado.png";
import avatar from "../assets/avatar.png";
import toque from "../assets/toque.png";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await api.post("/Auth/login", {
        username,
        password,
      });

      login(response.data.token);
      navigate("/dashboard");
    } catch (error) {
      alert("Username ou password incorretos.");
    }
  };

  return (
    <div className="login-container">
      <div className="login-content">
        <h1>
          <span className="titulo1">MERCADO</span>
          <br />
          <span className="titulo2">NUTRI</span>
        </h1>

        <div className="login-form">
          <form onSubmit={handleLogin}>
            <div className="input-username">
              <img src={avatar} alt="" />
              <input
                type="text"
                placeholder="NOME UTILIZADOR"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>

            <div className="input-password">
              <img src={cadeado} alt="" />
              <input
                type="password"
                placeholder="PALAVRA-PASSE"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <button type="submit">
              <img src={toque} alt="Login" />
              LOGIN
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Login;
