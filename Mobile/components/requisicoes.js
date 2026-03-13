import * as Crypto from 'expo-crypto';

const WEBHOOK_URL = "https://n8n.srv993727.hstgr.cloud/webhook/8e8b145a-52fd-4ec2-b174-81226b3eee81";

export async function sendToN8n(idUsuario, idChamado, tipo, login, senha) {
  try {
    // 1. Gera o Hash da senha (segurança)
    const hashsenha = await Crypto.digestStringAsync(
      Crypto.CryptoDigestAlgorithm.SHA256,
      senha || "" // Garante que não quebre se a senha for nula
    );

    const dataPayload = {
      tipo: tipo,
      idUsuario: idUsuario,
      idChamado: idChamado,
      login: login,
      hashsenha: hashsenha
    };

    console.log(`📤 Enviando (Tipo ${tipo}):`, JSON.stringify(dataPayload));

    const response = await fetch(WEBHOOK_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dataPayload)
    });

    if (!response.ok) {
      console.log("❌ Erro HTTP:", response.status);
      return tipo === 3 ? { success: false, erro: "Erro de conexão." } : [];
    }

    const rawJson = await response.json();
    console.log(`📥 Resposta (Tipo ${tipo}):`, JSON.stringify(rawJson));

    // ==========================================================
    // LÓGICA INTELIGENTE DE SEPARAÇÃO POR TIPO
    // ==========================================================

    // --- CENÁRIO A: LOGIN (TIPO 3) ---
    // Precisa retornar { success: true, usuario: ... }
    if (tipo === 3) {
        // Pega o primeiro item se for array (n8n padrão)
        const dadosUsuario = Array.isArray(rawJson) ? rawJson[0] : rawJson;

        // Verifica se encontrou o usuário (tem que ter ID)
        if (dadosUsuario && dadosUsuario.Id_usuario) {
            
            // Valida se está Ativo
            const isAtivo = dadosUsuario.Ativo === true || String(dadosUsuario.Ativo).toLowerCase() === 'true';

            if (isAtivo) {
                return { success: true, usuario: dadosUsuario };
            } else {
                return { success: false, erro: "Usuário inativo." };
            }
        } 
        
        return { success: false, erro: "E-mail ou senha incorretos." };
    }

    // --- CENÁRIO B: LISTA DE CHAMADOS (TIPO 1) ---
    // Precisa retornar um ARRAY [ {titulo: ...}, {titulo: ...} ]
    if (tipo === 1) {
        // Se o n8n retornou um array direto, ótimo.
        if (Array.isArray(rawJson)) {
            return rawJson;
        }
        
        // Se retornou { "data": [...] }
        if (rawJson.data && Array.isArray(rawJson.data)) {
            return rawJson.data;
        }
        
        // Se retornou { "chamados": [...] }
        if (rawJson.chamados && Array.isArray(rawJson.chamados)) {
            return rawJson.chamados;
        }

        // Se retornou apenas um objeto único (ex: 1 chamado só), coloca num array
        if (rawJson && typeof rawJson === 'object') {
            // Verifica se é erro antes
            if (rawJson.success === false) return [];
            return [rawJson];
        }

        return []; // Retorna lista vazia se não entendeu
    }

    // --- CENÁRIO C: OUTROS (CRIAR CHAMADO, ETC) ---
    // Retorna o objeto limpo para quem chamou tratar
    return Array.isArray(rawJson) ? rawJson[0] : rawJson;

  } catch (error) {
    console.error("❌ Erro Crítico:", error);
    // Retorna formato de erro compatível com Login se for tipo 3
    if (tipo === 3) return { success: false, erro: "Erro no App." };
    return null;
  }
}