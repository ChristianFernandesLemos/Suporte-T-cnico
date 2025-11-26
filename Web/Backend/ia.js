// components/requisicoes.js
const WEBHOOK_URL = "https://n8n.srv993727.hstgr.cloud/webhook/ia";

export async function sendToN8nToIa(id_usuario, titulo, nome_usuario, email, categoria,
     descricao, pessoas_afetadas, bloqueia_trabalho, prioridade_para_usuario, porque_prioridade_usuario, pedaco) {

  const dataPayload = {
    id_usuario: id_usuario,
    title: titulo,
    employeeName: nome_usuario,
    email: email,
    category: categoria,
    description: descricao,
    affectedPeople: pessoas_afetadas,
    blocksWork: bloqueia_trabalho,
    userPriority: prioridade_para_usuario,
    porqueprioridade: porque_prioridade_usuario,
    piece: pedaco
  };

  try {
    const response = await fetch(WEBHOOK_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(dataPayload)
    });

    if (response.ok) {
      const jsonResponse = await response.json();
      // Retorna os dados para quem chamou a função
      return jsonResponse;
    } else {
      console.log("Erro na requisição:", await response.text());
      return null;
    }

  } catch (error) {
    console.error("Erro de conexão:", error);
    return null;
  }
}