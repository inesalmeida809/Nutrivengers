import { useState } from "react";
import api from "../services/api";

function Login() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();

        try {
            const response = await api.post("/Auth/login", {
                username,
                password
            });

            localStorage.setItem("token", response.data.token);

            alert("Login efetuado com sucesso!");
        } catch (error) {
            alert("Username ou password incorretos.");
        }
    };

    return (
        <div>
            <h1>Login</h1>

            <form onSubmit={handleLogin}>
                <div>
                    <label>Username</label>
                    <input
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                </div>

                <div>
                    <label>Password</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <button type="submit">
                    Entrar
                </button>
            </form>
        </div>
    );
}

export default Login;