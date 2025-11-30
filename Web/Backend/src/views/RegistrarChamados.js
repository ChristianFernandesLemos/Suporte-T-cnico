// RegistrarChamados.js - Sistema de registro de chamados multi-etapas
// VERSÃO CONSOLIDADA COM ATUALIZAÇÕES DE PRIORIDADE/CONTESTAÇÃO (IA_atualizado.txt)
console.log('🚀 Sistema de Registro de Chamados Carregado');

// ========================================
// CONFIGURAÇÃO
// ========================================
const API_URL = 'http://localhost:3000/api/chamados';
const N8N_WEBHOOK_URL = 'https://n8n.srv993727.hstgr.cloud/webhook/ia'; // [cite: 1]

// ========================================
// STORAGE - Gerencia dados temporários
// ========================================
const chamadoStorage = {
  storageKey: 'chamado_temp_data',

  salvarEtapa(etapa, dados) {
    try {
      const dadosExistentes = this.obterTodos() || {};
      dadosExistentes[etapa] = {
        ...dados,
        timestamp: new Date().toISOString()
      };
      sessionStorage.setItem(this.storageKey, JSON.stringify(dadosExistentes));
      console.log(`✅ Dados da etapa ${etapa} salvos:`, dados);
      return true;
    } catch (error) {
      console.error('❌ Erro ao salvar dados:', error);
      return false;
    }
  },

  obterEtapa(etapa) {
    try {
      const dados = this.obterTodos();
      return dados ? dados[etapa] : null;
    } catch (error) {
      console.error('❌ Erro ao obter dados:', error);
      return null;
    }
  },

  obterTodos() {
    try {
      const dados = sessionStorage.getItem(this.storageKey);
      return dados ? JSON.parse(dados) : null;
    } catch (error) {
      console.error('❌ Erro ao obter dados:', error);
      return null;
    }
  },

  limpar() {
    try {
      sessionStorage.removeItem(this.storageKey);
      console.log('🗑️ Dados temporários limpos');
      return true;
    } catch (error) {
      console.error('❌ Erro ao limpar dados:', error);
      return false;
    }
  },

  buscarUsuarioPorEmail(email) {
    try {
      const dados = this.obterTodos();
      if (!dados || !dados.etapa1) return null;

      return dados.etapa1.email === email ? dados.etapa1 : null;
    } catch (error) {
      console.error('❌ Erro ao procurar Usuario:', error);
      return null;
    }
  }
};

// ========================================
// ✅ FUNÇÃO: Buscar ID do usuário na API
// ========================================
async function buscarUsuarioPorEmail(email) {
  try {
    console.log('🔍 Buscando usuário por email:', email);
    
    const token = sessionStorage.getItem('token');
    
    if (!token) {
      console.warn('⚠️ Token não encontrado, tentando sem autenticação...');
    }
    
    const response = await fetch(`http://localhost:3000/api/users/buscar-por-email?email=${encodeURIComponent(email)}`, {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      }
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('❌ Erro na resposta:', errorText);
      throw new Error(`Erro ao buscar usuário: ${response.status} - ${errorText}`);
    }
    
    const resultado = await response.json();
    console.log('✅ Resposta da API:', resultado);
    
    if (!resultado.success) {
      throw new Error(resultado.message || 'Usuário não encontrado');
    }
    
    console.log('✅ ID do usuário:', resultado.userId);
    return resultado.userId;
    
  } catch (error) {
    console.error('❌ Erro ao buscar usuário por email:', error);
    throw error;
  }
}

// ========================================
// ETAPA 1 - Informações Básicas
// ========================================
function inicializarEtapa1() {
  const form = document.querySelector('form');
  
  if (!form) return;

  console.log('📝 Etapa 1 inicializada');

  // Carrega dados salvos (se existirem)
  const dadosSalvos = chamadoStorage.obterEtapa('etapa1');
  if (dadosSalvos) {
    console.log('📂 Carregando dados salvos');
    document.getElementById('titulo').value = dadosSalvos.titulo || '';
    document.getElementById('nome').value = dadosSalvos.nome || '';
    document.getElementById('email').value = dadosSalvos.email || '';
    document.getElementById('categoria').value = dadosSalvos.categoria || '';
    document.getElementById('descricao').value = dadosSalvos.descricao || '';
  }

  // Evento de submit
  form.addEventListener('submit', function(e) {
    e.preventDefault();

    const dados = {
      titulo: document.getElementById('titulo').value.trim(),
      nome: document.getElementById('nome').value.trim(),
      email: document.getElementById('email').value.trim(),
      categoria: document.getElementById('categoria').value,
      descricao: document.getElementById('descricao').value.trim()
    };

    // Validação
    if (!dados.titulo || !dados.nome || !dados.email || !dados.categoria || !dados.descricao) {
      alert('⚠️ Por favor, preencha todos os campos.');
      return;
    }

    // Valida email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(dados.email)) {
      alert('⚠️ Por favor, insira um e-mail válido.');
      return;
    }

    // Salva e avança
    if (chamadoStorage.salvarEtapa('etapa1', dados)) {
      console.log('✅ Avançando para Etapa 2');
      window.location.href = '/registrar-chamado-p2';
    } else {
      alert('❌ Erro ao salvar. Tente novamente.');
    }
  });
}

// ========================================
// ETAPA 2 - Quem está sendo afetado
// ========================================
function inicializarEtapa2() {
  const form = document.querySelector('form');
  
  if (!form) return;

  console.log('📝 Etapa 2 inicializada');

  // Verifica dados da etapa 1
  const dadosEtapa1 = chamadoStorage.obterEtapa('etapa1');
  if (!dadosEtapa1) {
    alert('⚠️ Nenhum dado encontrado. Voltando para a primeira etapa.');
    window.location.href = '/registrar-chamado';
    return;
  }

  // Carrega dados salvos
  const dadosSalvos = chamadoStorage.obterEtapa('etapa2');
  if (dadosSalvos) {
    document.getElementById('afetado').value = dadosSalvos.afetado || '';
  }

  // Atualiza link do header
  const headerBackLink = document.querySelector('.back-link');
  if (headerBackLink) {
    headerBackLink.textContent = '← Voltar';
    headerBackLink.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado';
    });
  }

  // Evento de submit
  form.addEventListener('submit', function(e) {
    e.preventDefault();

    const dados = {
      afetado: document.getElementById('afetado').value
    };

    // Validação
    if (!dados.afetado) {
      alert('⚠️ Por favor, selecione quem está sendo afetado.');
      return;
    }

    // Salva e avança
    if (chamadoStorage.salvarEtapa('etapa2', dados)) {
      console.log('✅ Etapa 2 concluída');
      window.location.href = '/registrar-chamado-p3';
    } else {
      alert('❌ Erro ao salvar. Tente novamente.');
    }
  });

  // Botão voltar
  const btnVoltar = document.querySelector('.back-button');
  if (btnVoltar) {
    btnVoltar.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado';
    });
  }
}

// ========================================
// ETAPA 3 - Bloqueio Total
// ========================================
function inicializarEtapa3() {
  const form = document.querySelector('form');
  
  if (!form) return;

  console.log('📝 Etapa 3 inicializada');

  // Verifica dados da etapa 2
  const dadosEtapa2 = chamadoStorage.obterEtapa('etapa2');
  if (!dadosEtapa2) {
    alert('⚠️ Nenhum dado encontrado. Voltando para a segunda etapa.');
    window.location.href = '/registrar-chamado-p2';
    return;
  }

  // Carrega dados salvos
  const dadosSalvos = chamadoStorage.obterEtapa('etapa3');
  if (dadosSalvos && dadosSalvos.bloqueioTotal) {
    const radioSelecionado = document.querySelector(`input[name="impacto"][value="${dadosSalvos.bloqueioTotal}"]`);
    if (radioSelecionado) {
      radioSelecionado.checked = true;
    }
  }

  // Atualiza link do header
  const headerBackLink = document.querySelector('.back-link');
  if (headerBackLink) {
    headerBackLink.textContent = '← Voltar';
    headerBackLink.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado-p2';
    });
  }

  // Evento de submit
  form.addEventListener('submit', function(e) {
    e.preventDefault();

    const impactoSelecionado = document.querySelector('input[name="impacto"]:checked');
    
    if (!impactoSelecionado) {
      alert('⚠️ Por favor, selecione se o problema bloqueia totalmente o trabalho.');
      return;
    }

    const dados = {
      bloqueioTotal: impactoSelecionado.value
    };

    console.log('📊 Dados da Etapa 3:', dados);

    // Salva e avança
    if (chamadoStorage.salvarEtapa('etapa3', dados)) {
      console.log('✅ Etapa 3 concluída');
      window.location.href = '/registrar-chamado-p4';
    } else {
      alert('❌ Erro ao salvar. Tente novamente.');
    }
  });

  // Botão voltar
  const btnVoltar = document.querySelector('.back-button');
  if (btnVoltar) {
    btnVoltar.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado-p3';
    });
  }
}

// ========================================
// ETAPA 4 - Confirmação e envio para IA
// ========================================
function inicializarEtapa4() {
  const form = document.querySelector('form');
  
  if (!form) return;

  console.log('📝 Etapa 4 inicializada');

  // Verifica etapa anterior
  const dadosEtapa3 = chamadoStorage.obterEtapa('etapa3');
  if (!dadosEtapa3) {
    alert('⚠️ Nenhum dado encontrado. Voltando para a terceira etapa.');
    window.location.href = '/registrar-chamado-p3';
    return;
  }

  // Atualiza header
  const headerBackLink = document.querySelector('.back-link');
  if (headerBackLink) {
    headerBackLink.textContent = '← Voltar';
    headerBackLink.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado-p3';
    });
  }

  // Evento de submit - Envia para IA
  form.addEventListener('submit', async function(e) {
    e.preventDefault();

    // Mostra loading
    const submitBtn = form.querySelector('button[type="submit"]');
    const textoOriginal = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = '⏳ Analisando com IA...';

    try {
      await enviarParaIA();
      console.log('✅ Etapa 4 concluída - Aguardando resposta da IA');
      window.location.href = '/prioridadeia';
    } catch (error) {
      console.error('❌ Erro:', error);
      alert('❌ Erro ao processar com IA. Tente novamente.');
      submitBtn.disabled = false;
      submitBtn.textContent = textoOriginal;
    }
  });

  // Botão voltar
  const btnVoltar = document.querySelector('.back-button');
  if (btnVoltar) {
    btnVoltar.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado-p3';
    });
  }
}

// ========================================
// 1. FUNÇÃO DE ANÁLISE (Busca Prioridade - Piece 1)
// **SUBSTITUÍDA PELA VERSÃO DO IA_ATUALIZADO.TXT**
// ========================================
async function enviarParaIA() {
  try {
    console.log('🤖 Consultando IA (Modo Análise - Piece 1)...');
    // Coleta dados do storage
    const todosOsDados = chamadoStorage.obterTodos(); // [cite: 2]
    // Busca ID do usuário (Segurança)
    let userId = null; // [cite: 3]
    try {
      userId = await buscarUsuarioPorEmail(todosOsDados.etapa1.email); // [cite: 4]
    } catch (error) {
      console.warn('⚠️ ID não encontrado, enviando sem ID:', error); // [cite: 5]
    }

    // Payload para análise (piece: 1)
    const payload = {
      id_usuario: userId,
      title: todosOsDados.etapa1.titulo,
      employeeName: todosOsDados.etapa1.nome,
      email: todosOsDados.etapa1.email,
      category: todosOsDados.etapa1.categoria,
      description: todosOsDados.etapa1.descricao,
      affectedPeople: todosOsDados.etapa2.afetado,
      blocksWork: todosOsDados.etapa3.bloqueioTotal === 'sim' ?
      'Sim' : 'Não', // [cite: 7]
      userPriorityReason: '', 
      
      piece: 1 // 1 = Analisar, NÃO Salvar [cite: 7]
    };
    const response = await fetch(N8N_WEBHOOK_URL, { // [cite: 8]
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });
    if (!response.ok) throw new Error(`Erro HTTP: ${response.status}`); // [cite: 9]

    const textoResposta = await response.text();
    let resultadoRaw;
    try {
        resultadoRaw = JSON.parse(textoResposta); // [cite: 10]
    } catch (e) {
        throw new Error("Resposta da IA não é um JSON válido"); // [cite: 11, 12]
    }

    // Normaliza resposta (Array ou Objeto)
    const resultado = Array.isArray(resultadoRaw) ?
    resultadoRaw[0] : resultadoRaw; // [cite: 13]

    console.log('✅ Análise Recebida:', resultado);
    
    // Salva a sugestão da IA no storage local
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: resultado.prioridade || 'Média',
      justificativa: resultado.justificativa || resultado.userPriorityReason || 'Análise automática',
      timestamp: new Date().toISOString(),
      contestado: false // Inicializa como false (não contestado) [cite: 14]
    });
    return resultado; // [cite: 14]

  } catch (error) {
    console.error('❌ Erro na análise IA:', error); // [cite: 15]
    // Fallback de segurança
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: 'Média',
      justificativa: 'Sistema indisponível temporariamente',
      erro: true
    }); // [cite: 15]
    return { prioridade: 'Média' }; // [cite: 16]
  }
}

// ========================================
// FUNÇÃO AUXILIAR: Salvar contestação no banco
// ========================================
async function salvarContestacaoNoBanco(idChamado, idUsuario, justificativa) {
  try {
    console.log(`📝 Salvando contestação: Chamado ${idChamado}, Usuário ${idUsuario}`);
    
    const token = sessionStorage.getItem('token');
    
    const contestacaoPayload = {
      idChamado: idChamado,
      idUsuario: idUsuario,
      justificativa: justificativa,
      tipo: 'Discordo da Prioridade'
    };

    const response = await fetch('http://localhost:3000/api/contestacoes', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      },
      body: JSON.stringify(contestacaoPayload)
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Erro HTTP ${response.status}: ${errorText}`);
    }

    const resultado = await response.json();
    console.log('✅ Contestação salva com sucesso:', resultado);
    return resultado;

  } catch (error) {
    console.error('❌ Erro ao salvar contestação:', error);
    throw error;
  }
}

// ========================================
// FUNÇÃO AUXILIAR: Buscar último chamado do usuário
// ========================================
async function buscarUltimoChamadoDoUsuario(userId) {
  try {
    console.log(`🔍 Buscando último chamado do usuário ${userId}...`);
    
    const token = sessionStorage.getItem('token');
    
    const response = await fetch('http://localhost:3000/api/chamados', {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      }
    });

    if (!response.ok) {
      throw new Error(`Erro ao buscar chamados: ${response.status}`);
    }

    const data = await response.json();
    
    if (!data.success || !data.chamados || data.chamados.length === 0) {
      throw new Error('Nenhum chamado encontrado');
    }

    // Filtra chamados do usuário específico e pega o mais recente
    const chamadosDoUsuario = data.chamados.filter(c => c.afetadoId === userId);
    
    if (chamadosDoUsuario.length === 0) {
      throw new Error('Nenhum chamado encontrado para este usuário');
    }

    const ultimoChamado = chamadosDoUsuario[0]; // Já vem ordenado por data DESC
    console.log(`✅ Último chamado encontrado: ID ${ultimoChamado.id}`);
    
    return ultimoChamado.id;

  } catch (error) {
    console.error('❌ Erro ao buscar último chamado:', error);
    throw error;
  }
}

// ========================================
// 2. FUNÇÃO DE REGISTRO (Salva no Banco) - V2 ROBUSTA (MANTIDA)
// ========================================
async function finalizarChamado() {
  console.log('💾 Iniciando gravação final (Piece 2)...');
  
  const todosOsDados = chamadoStorage.obterTodos(); // [cite: 17]
  const dadosIA = chamadoStorage.obterEtapa('ia_response'); // [cite: 17]
  const dadosContestacao = chamadoStorage.obterEtapa('contestacao'); // [cite: 18]

  // Verifica se teve contestação
  const houveContestacao = dadosIA.contestado && dadosContestacao; // [cite: 20]
  
  // Prepara justificativas
  let prioridadeFinal = dadosIA.prioridade;
  let justificativaIA = dadosIA.justificativa;
  let justificativaUsuario = houveContestacao ? dadosContestacao.justificativa : '';
  let justificativaFinal = justificativaIA; // [cite: 19]

  if (houveContestacao) {
    justificativaFinal = justificativaUsuario;
  }

  // Busca user ID
  let userId = null;
  try {
    userId = await buscarUsuarioPorEmail(todosOsDados.etapa1.email); // [cite: 21]
    console.log('✅ User ID encontrado:', userId);
  } catch (e) {
    console.error('❌ Erro ao buscar userId:', e);
    throw new Error('Não foi possível identificar o usuário. Verifique o email.');
  }

  // 1️⃣ ENVIA PARA N8N (Cria o chamado)
  const payload = {
    id_usuario: userId,
    title: todosOsDados.etapa1.titulo,
    employeeName: todosOsDados.etapa1.nome,
    email: todosOsDados.etapa1.email,
    category: todosOsDados.etapa1.categoria,
    description: todosOsDados.etapa1.descricao,
    affectedPeople: todosOsDados.etapa2.afetado,
    blocksWork: todosOsDados.etapa3.bloqueioTotal === 'sim' ? 'Sim' : 'Não', // [cite: 23]
    userPriority: prioridadeFinal,
    userPriorityReason: justificativaFinal,
    piece: 2 // [cite: 24]
  };

  console.log('📤 Enviando para N8N:', payload);

  const response = await fetch(N8N_WEBHOOK_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    throw new Error(`Falha ao registrar chamado no N8N: ${response.status}`);
  }
  
  const respostaRaw = await response.json();
  const resposta = Array.isArray(respostaRaw) ? respostaRaw[0] : respostaRaw; // [cite: 26]

  if (resposta.status === 'Deu algum erro') {
    throw new Error('O servidor N8N recusou o registro do chamado.'); // [cite: 27]
  }

  console.log('✅ Chamado criado via N8N:', resposta);

  // 2️⃣ SE HOUVER CONTESTAÇÃO, SALVA NA TABELA Historial_Contestacoes
  if (houveContestacao) {
    console.log('📝 Tentando registrar contestação no banco de dados...');
    
    // Aguarda 1.5 segundos para garantir que o chamado foi criado
    await new Promise(resolve => setTimeout(resolve, 1500));
    
    try {
      // ABORDAGEM 1: Verificar se N8N retornou o ID do chamado
      let chamadoId = resposta.chamadoId || resposta.id || resposta.id_chamado;
      
      // ABORDAGEM 2: Se não veio ID, buscar último chamado do usuário
      if (!chamadoId) {
        console.log('⚠️ N8N não retornou ID, buscando último chamado...');
        chamadoId = await buscarUltimoChamadoDoUsuario(userId);
      }

      if (chamadoId) {
        console.log(`🎯 ID do chamado identificado: ${chamadoId}`);
        
        // Salva a contestação
        await salvarContestacaoNoBanco(
          chamadoId, 
          userId, 
          justificativaUsuario
        );
        
        console.log('✅ Contestação registrada com sucesso!');
      } else {
        throw new Error('Não foi possível identificar o ID do chamado');
      }

    } catch (contestacaoError) {
      console.error('❌ ERRO ao salvar contestação:', contestacaoError);
      
      // Mostra aviso ao usuário mas não bloqueia o fluxo
      console.warn('⚠️ Chamado criado, mas contestação não foi registrada no banco.');
      console.warn('💡 Entre em contato com o suporte técnico informando:');
      console.warn(`   - Seu email: ${todosOsDados.etapa1.email}`);
      console.warn(`   - Título do chamado: ${todosOsDados.etapa1.titulo}`);
      console.warn(`   - Contestação: ${justificativaUsuario}`);
    }
  }

  return true;
}

// ========================================
// 3. LÓGICA DA TELA DE PRIORIDADE (Visual)
// **SUBSTITUÍDA PELA VERSÃO DO IA_ATUALIZADO.TXT**
// ========================================
function iniciarPrioridadeIA() {
  const form = document.querySelector('form');
  // Se não tiver form ou não tiver o elemento visual de prioridade, sai.
  if (!document.querySelector('.prioridade') || !form) return; // [cite: 28]

  const dadosIA = chamadoStorage.obterEtapa('ia_response'); // [cite: 29]
  
  // Se não tem dados da IA, busca agora e recarrega
  if (!dadosIA) {
      enviarParaIA().then(() => {
          window.location.reload();
      }); // [cite: 30]
      return; 
  }

  // Preenche HTML Visual da Prioridade (Cores)
  const prioridadeElement = document.querySelector('.prioridade');
  if (prioridadeElement) { // [cite: 31]
    let cor = '#f1c40f'; // Média
    if(dadosIA.prioridade === 'Alta' || dadosIA.prioridade === 'Urgente') cor = '#e74c3c'; // [cite: 32]
    if(dadosIA.prioridade === 'Baixa') cor = '#2ecc71'; // [cite: 33]
    
    prioridadeElement.innerHTML = `<strong style="color:${cor}">${dadosIA.prioridade}</strong>`; // [cite: 34]
  }

  // Insere Justificativa da IA na tela
  const paragrafosCard = document.querySelectorAll('.card p');
  if (paragrafosCard.length >= 2) { // [cite: 35]
      let containerJustificativa = document.getElementById('ia-justificativa'); // [cite: 36]
      if (!containerJustificativa) {
          containerJustificativa = document.createElement('div');
          containerJustificativa.id = 'ia-justificativa';
          containerJustificativa.style.marginTop = '15px'; // [cite: 37]
          containerJustificativa.style.padding = '10px';
          containerJustificativa.style.backgroundColor = '#f8f9fa';
          containerJustificativa.style.borderRadius = '5px';
          document.querySelector('.card').appendChild(containerJustificativa); // [cite: 38]
      }
      containerJustificativa.innerHTML = `<p style="font-size:0.9em; margin:0;"><strong>Motivo da IA:</strong> ${dadosIA.justificativa}</p>`; // [cite: 39]
  }

  // BOTÃO CONCORDAR (Aceita a IA e Salva)
  const btnFinalizar = form.querySelector('button[type="submit"]'); // [cite: 40]
  if (btnFinalizar) {
      btnFinalizar.addEventListener('click', async function(e) {
        e.preventDefault();
        const textoOriginal = btnFinalizar.textContent;
        
        try {
            btnFinalizar.disabled = true;
            btnFinalizar.textContent = '💾 Salvando...';
            
            
            await finalizarChamado(); // Chama o Piece 2 (Envia dados da IA pois não houve contestação) [cite: 41]
            
            alert('✅ Chamado registrado com sucesso!');
            chamadoStorage.limpar();
            window.location.href = '/menu';
            
        } catch (erro) {
         
            console.error(erro); // [cite: 42]
            alert('Erro ao salvar: ' + erro.message);
            btnFinalizar.disabled = false;
            btnFinalizar.textContent = textoOriginal;
        }
      }); // [cite: 43]
  }

  // BOTÃO CONTESTAR (Redireciona para tela de contestação)
  const btnContestar = document.querySelector('.back-button'); // [cite: 44]
  if (btnContestar) {
      btnContestar.onclick = (e) => {
          e.preventDefault();
          window.location.href = '/contestacao'; // [cite: 45]
      };
  }
}

// ========================================
// 4. LÓGICA DA TELA DE CONTESTAÇÃO (Visual)
// **SUBSTITUÍDA PELA VERSÃO DO IA_ATUALIZADO.TXT**
// ========================================
function iniciarContestacao() {
  const form = document.querySelector('form');
  if (!form) return;

  console.log('📝 Tela de contestação inicializada'); // [cite: 46]

  // Botão Voltar/Cancelar
  const btnCancelar = document.querySelector('.back-button'); // [cite: 47]
  if (btnCancelar) {
      btnCancelar.addEventListener('click', function(e) {
          e.preventDefault();
          console.log('↩️ Cancelando contestação, voltando para tela de prioridade');
          window.location.href = '/prioridadeia'; // Volta para a tela da IA [cite: 48]
      });
  }

  // Botão Finalizar Contestação
  const btnFinalizar = document.querySelector('.submit-button') || document.querySelector('form button[type="submit"]'); // [cite: 49]
  if (btnFinalizar) {
      btnFinalizar.addEventListener('click', async function(e) {
          e.preventDefault();

          const campoJustificativa = document.getElementById('descricao') || document.getElementById('justificativa');
          const novaJustificativa = campoJustificativa ? campoJustificativa.value.trim() : '';

          // Validação
          if (!novaJustificativa) {
          
              alert("⚠️ Por favor, explique o motivo da contestação."); // [cite: 50]
              return;
          }

          console.log('💾 Salvando contestação:', novaJustificativa);

          // A. Salva contestação no storage
          chamadoStorage.salvarEtapa('contestacao', {
              justificativa: novaJustificativa,
              timestamp: new Date().toISOString() // [cite: 51]
          }); 

          // B. Marca flag 'contestado' na IA para true
          const dadosIA = chamadoStorage.obterEtapa('ia_response') || {};
          chamadoStorage.salvarEtapa('ia_response', {
              ...dadosIA,
              contestado: true // IMPORTANTE: Isso ativa o IF na função finalizarChamado [cite: 52]
          });
          // C. Envia para o servidor (Piece 2) [cite: 53]
          const textoOriginal = btnFinalizar.textContent;
          try { // [cite: 54]
              btnFinalizar.disabled = true; // [cite: 55]
              btnFinalizar.textContent = '💾 Salvando contestação...';

              // Ao chamar finalizarChamado agora, ele verá que 'contestado' é true
              // e usará os dados que acabamos de salvar.
              await finalizarChamado(); // [cite: 56]

              alert('✅ Contestação registrada! O chamado foi criado e será revisado por um supervisor.');
              chamadoStorage.limpar();
              window.location.href = '/menu';
          } catch (erro) { // [cite: 57]
              console.error('❌ Erro ao salvar contestação:', erro); // [cite: 58]
              alert('❌ Erro ao salvar: ' + erro.message);
              btnFinalizar.disabled = false;
              btnFinalizar.textContent = textoOriginal; // [cite: 59]
          }
      });
  }
}

// ========================================
// INICIALIZAÇÃO (MANTIDA DO ARQUIVO PRINCIPAL)
// ========================================
document.addEventListener('DOMContentLoaded', function() {
  const url = window.location.pathname.toLowerCase();
  
  console.log('📍 URL atual:', url);

  if (url.includes('contestacao') || url.includes('contestação')) {
    iniciarContestacao();
  }
  else if (url.includes('prioridadeia')) {
    iniciarPrioridadeIA();
  }
  else if (url.includes('registrar-chamado-p4')) {
    inicializarEtapa4();
  }
  else if (url.includes('registrar-chamado-p3')) {
    inicializarEtapa3();
  } 
  else if (url.includes('registrar-chamado-p2')) {
    inicializarEtapa2();
  } 
  else if (url.includes('registrar-chamado')) {
    inicializarEtapa1();
  }

  window.exibirResumo = function() {
    const dados = chamadoStorage.obterTodos();
    if (dados) {
      console.log('📊 Resumo dos dados:');
      console.log(JSON.stringify(dados, null, 2));
    } else {
      console.log('❌ Nenhum dado salvo');
    }
  };
  
  window.chamadoStorage = chamadoStorage;
});