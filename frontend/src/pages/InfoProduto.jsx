import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";

function InfoProduto() {
    const { id } = useParams();

    const [produto, setProduto] = useState(null);
    const [loading, setLoading] = useState(true);
    const [erro, setErro] = useState("");

    useEffect(() => {
        async function carregarProduto() {
            try {
                setLoading(true);

                const resposta = await fetch(
                    `https://world.openfoodfacts.org/api/v2/product/${encodeURIComponent(id)}.json`
                );

                const dados = await resposta.json();

                if (dados.status !== 1) {
                    throw new Error("Produto não encontrado.");
                }

                setProduto(dados.product);
            } catch (error) {
                setErro(error.message);
            } finally {
                setLoading(false);
            }
        }

        carregarProduto();
    }, [id]);

    if (loading) return <p>A carregar produto...</p>;
    if (erro) return <p>{erro}</p>;

    return (
        <div className="info-produto-container">
            <h1>{produto.product_name || "Produto sem nome"}</h1>

            {produto.image_front_url && (
                <img
                    src={produto.image_front_url}
                    alt={produto.product_name}
                    style={{ width: "250px", objectFit: "contain" }}
                />
            )}

            <p><strong>Marca:</strong> {produto.brands || "Não disponível"}</p>
            <p><strong>Quantidade:</strong> {produto.quantity || "Não disponível"}</p>
            <p><strong>Nutri-Score:</strong> {produto.nutriscore_grade?.toUpperCase() || "Não disponível"}</p>
            <p><strong>Ingredientes:</strong> {produto.ingredients_text || "Não disponíveis"}</p>
        </div>
    );
}

export default InfoProduto;