import { useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

function Dashboard() {
    const { logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <div>
            <h1>Nutrivengers</h1>
            <h2>Dashboard</h2>
            <p>Bem-vindo ao Nutrivengers.</p>

            <button onClick={handleLogout}>
                Sair
            </button>
        </div>
    );
}

export default Dashboard;