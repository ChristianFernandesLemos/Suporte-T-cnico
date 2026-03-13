// components/ia.js
const WEBHOOK_URL = "https://n8n.srv993727.hstgr.cloud/webhook/ia";

export async function sendToN8nToIa(
    id_usuario, 
    titulo, 
    nome_usuario, 
    email, 
    categoria,
    descricao, 
    pessoas_afetadas, 
    bloqueia_trabalho, 
    porque_prioridade_usuario, 
    pedaco // 1 = Analisar, 2 = Salvar
) {

  // Montagem do payload conforme esperado pelo seu n8n
  const dataPayload = {
    id_usuario: id_usuario,
    title: titulo,
    employeeName: nome_usuario,
    email: email,
    category: categoria,
    description: descricao,
    affectedPeople: pessoas_afetadas,
    blocksWork: bloqueia_trabalho,
    userPriorityReason: porque_prioridade_usuario,
    piece: pedaco 
  };

  console.log(`📤 Enviando IA (Piece ${pedaco}):`, JSON.stringify(dataPayload));

  try {
    const response = await fetch(WEBHOOK_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(dataPayload)
    });

    // Se der erro 500 ou 400, o response.ok será false
    if (!response.ok) {
        const errorText = await response.text();
        console.log(`❌ Erro HTTP ${response.status}:`, errorText);
        return null;
    }

    const rawJson = await response.json();
    console.log(`📥 Resposta IA (Piece ${pedaco}):`, JSON.stringify(rawJson));

    // 1. Normalização: O n8n pode retornar Array ou Objeto. 
    // Pegamos sempre o objeto limpo.
    const result = Array.isArray(rawJson) ? rawJson[0] : rawJson;

    // 2. Verificação de ERRO explícito do n8n (conforme seu print de erro)
    //
    if (result && result.status === "Deu algum erro") {
        console.log("⛔ O n8n retornou erro de processamento.");
        return null;
    }

    // 3. Verificação de SUCESSO do Piece 2 (Salvar)
    //
    if (pedaco === 2) {
        if (result && result.status === "registrou") {
            // Retorna um objeto truthy para o NewTicketScreen saber que deu certo
            return { success: true };
        } else {
            // Se piece é 2 mas não veio "registrou", algo falhou
            return null;
        }
    }

    // 4. Retorno do Piece 1 (Análise)
    // Retorna o objeto completo contendo "prioridade", "Justificativa", etc.
    return result;

  } catch (error) {
    console.error("❌ Erro CRÍTICO de conexão IA:", error);
    return null;
  }
}