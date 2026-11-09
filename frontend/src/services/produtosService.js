
import api from "./api";

export async function pesquisarProdutos(pesquisa, pagina = 1) {
  const resposta = await api.get("/pesquisa-produtos", {
    params: {
      pesquisa,
      pagina
    },
  });

  return resposta.data;
}
