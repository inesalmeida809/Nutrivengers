
import api from "./api";

export async function pesquisarProdutos(pesquisa) {
  const resposta = await api.get("/pesquisa-produtos", {
    params: { pesquisa },
  });

  return resposta.data;
}
