// RegistrarChamados.js - Sistema de registro de chamados multi-etapas
// VERSÃO COMPLETA - 100% COMPATÍVEL COM N8N - CORRIGIDA
console.log('🚀 Sistema de Registro de Chamados Carregado');

// ========================================
// CONFIGURAÇÃO
// ========================================
const API_URL = 'http://localhost:3000/api/chamados';
const N8N_WEBHOOK_URL = 'https://n8n.srv993727.hstgr.cloud/webhook/ia';

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
      if (!dados || !dados.etapa1) return afetadoId;

      return dados.etapa1.email === email ? dados.etapa1 : null;
    } catch (error) {
      console.error('❌ Erro ao procurar Usuario:', error);
      return null;
    }
  }
};

// ========================================
// ✅ NOVA FUNÇÃO: Buscar ID do usuário na API
// ========================================
async function buscarUsuarioPorEmail(email) {
  try {
    console.log('🔍 Buscando usuário por email:', email);
    
    // ✅ Pega o token de autenticação do sessionStorage
    const token = sessionStorage.getItem('token');
    
    if (!token) {
      console.warn('⚠️ Token não encontrado, tentando sem autenticação...');
    }
    
    // ✅ CORREÇÃO: Rota correta com query parameter e autenticação
    const response = await fetch(`http://localhost:3000/api/users/buscar-por-email?email=${encodeURIComponent(email)}`, {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }) // Adiciona token se existir
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

  // Botão voltar
  const btnVoltar = document.querySelector('.back-link');
  if (btnVoltar) {
    btnVoltar.addEventListener('click', function(e) {
      e.preventDefault();
      if (confirm('Deseja voltar? Os dados não salvos serão perdidos.')) {
        chamadoStorage.limpar();
        window.location.href = '/menu';
      }
    });
  }
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
      bloqueioTotal: impactoSelecionado.value // 'sim' ou 'nao'
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
      window.location.href = '/registrar-chamado-p2';
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
      // Envia para N8N (IA) para análise de prioridade
      await enviarParaIA();
      
      console.log('✅ Etapa 4 concluída - Aguardando resposta da IA');
      window.location.href = '/PrioridadeIA';
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
// CONFIGURAÇÕES
// ========================================

// ========================================
// 1. FUNÇÃO DE ANÁLISE (Busca Prioridade)
// ========================================
async function enviarParaIA() {
  try {
    console.log('🤖 Consultando IA (Modo Análise - Piece 1)...');
    
    // Coleta dados
    const todosOsDados = chamadoStorage.obterTodos();
    
    // Busca ID do usuário (Etapa de segurança)
    let userId = null;
    try {
      // Assumindo que essa função já existe no seu escopo global
      userId = await buscarUsuarioPorEmail(todosOsDados.etapa1.email);
    } catch (error) {
      console.warn('⚠️ ID não encontrado, enviando sem ID:', error);
    }

    // Payload corrigido para o padrão do Mobile
    const payload = {
      id_usuario: userId,
      title: todosOsDados.etapa1.titulo,
      employeeName: todosOsDados.etapa1.nome,
      email: todosOsDados.etapa1.email,
      category: todosOsDados.etapa1.categoria,
      description: todosOsDados.etapa1.descricao,
      affectedPeople: todosOsDados.etapa2.afetado,
      blocksWork: todosOsDados.etapa3.bloqueioTotal === 'sim' ? 'Sim' : 'Não',
      
      // Campos vazios pois é apenas análise
      userPriority: '', 
      userPriorityReason: '', // Nome corrigido (era porqueprioridade)
      
      piece: 1 // ✅ CORREÇÃO: 1 = Analisar, NÃO Salvar
    };

    const response = await fetch(N8N_WEBHOOK_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error(`Erro HTTP: ${response.status}`);

    // Tratamento robusto do JSON (Array vs Objeto)
    const textoResposta = await response.text();
    let resultadoRaw;
    try {
        resultadoRaw = JSON.parse(textoResposta);
    } catch (e) {
        throw new Error("Resposta da IA não é um JSON válido");
    }

    // Normaliza: Se vier array [{}], pega o primeiro item. Se vier objeto {}, usa ele.
    const resultado = Array.isArray(resultadoRaw) ? resultadoRaw[0] : resultadoRaw;

    console.log('✅ Análise Recebida:', resultado);

    // Salva a sugestão da IA no storage local
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: resultado.prioridade || resultado.userPriority || 'Média',
      justificativa: resultado.justificativa || resultado.userPriorityReason || 'Análise automática',
      timestamp: new Date().toISOString(),
      contestado: false // Inicializa como false
    });

    return resultado;

  } catch (error) {
    console.error('❌ Erro na análise IA:', error);
    // Fallback em caso de erro para não travar o usuário
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: 'Média',
      justificativa: 'Sistema indisponível temporariamente',
      erro: true
    });
    return { prioridade: 'Média' };
  }
}

// ========================================
// 2. FUNÇÃO DE REGISTRO (Salva no Banco)
// ========================================
async function finalizarChamado() {
    console.log('💾 Iniciando gravação final (Piece 2)...');
    
    const todosOsDados = chamadoStorage.obterTodos();
    const dadosIA = chamadoStorage.obterEtapa('ia_response');
    const dadosContestacao = chamadoStorage.obterEtapa('contestacao'); // Caso tenha havido contestação

    // Determina a prioridade final (Do usuário se contestou, ou da IA se aceitou)
    let prioridadeFinal = dadosIA.prioridade;
    let justificativaFinal = dadosIA.justificativa;

    if (dadosIA.contestado && dadosContestacao) {
        prioridadeFinal = dadosContestacao.prioridadeUsuario;
        justificativaFinal = dadosContestacao.justificativa;
    }

    // Busca ID novamente para garantir
    let userId = null;
    try {
        userId = await buscarUsuarioPorEmail(todosOsDados.etapa1.email);
    } catch (e) {}

    const payload = {
      id_usuario: userId,
      title: todosOsDados.etapa1.titulo,
      employeeName: todosOsDados.etapa1.nome,
      email: todosOsDados.etapa1.email,
      category: todosOsDados.etapa1.categoria,
      description: todosOsDados.etapa1.descricao,
      affectedPeople: todosOsDados.etapa2.afetado,
      blocksWork: todosOsDados.etapa3.bloqueioTotal === 'sim' ? 'Sim' : 'Não',
      
      // ✅ AQUI VAI A DECISÃO FINAL
      userPriority: prioridadeFinal,
      userPriorityReason: justificativaFinal,
      
      piece: 2 // ✅ CORREÇÃO: 2 = Salvar definitivamente
    };

    const response = await fetch(N8N_WEBHOOK_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error('Falha ao registrar chamado no servidor');
    
    // Verifica se o backend retornou sucesso
    const respostaRaw = await response.json();
    const resposta = Array.isArray(respostaRaw) ? respostaRaw[0] : respostaRaw;

    if (resposta.status === 'Deu algum erro') {
        throw new Error('O servidor recusou o registro do chamado.');
    }

    return true;
}

// ========================================
// 3. LOGICA DA TELA DE PRIORIDADE (UI)
// ========================================
function iniciarPrioridadeIA() {
  const form = document.querySelector('form');
  if (!form) return;

  const dadosIA = chamadoStorage.obterEtapa('ia_response');
  
  // Se não tem dados da IA, tenta buscar agora (caso o usuário tenha recarregado a página)
  if (!dadosIA) {
      enviarParaIA().then(() => {
          window.location.reload();
      });
      return; 
  }

  // Preenche HTML
  const prioridadeElement = document.querySelector('.prioridade');
  const paragrafosCard = document.querySelectorAll('.card p');

  if (prioridadeElement) {
    // Aplica cor baseada na prioridade
    let cor = '#f1c40f'; // Média (Amarelo)
    if(dadosIA.prioridade === 'Alta' || dadosIA.prioridade === 'Urgente') cor = '#e74c3c';
    if(dadosIA.prioridade === 'Baixa') cor = '#2ecc71';
    
    prioridadeElement.innerHTML = `<strong style="color:${cor}">${dadosIA.prioridade}</strong>`;
  }

  // Insere Justificativa
  if (paragrafosCard.length >= 2) {
      // Procura onde inserir ou cria um elemento novo se necessário
      let containerJustificativa = document.getElementById('ia-justificativa');
      if (!containerJustificativa) {
          containerJustificativa = document.createElement('div');
          containerJustificativa.id = 'ia-justificativa';
          containerJustificativa.style.marginTop = '15px';
          containerJustificativa.style.padding = '10px';
          containerJustificativa.style.backgroundColor = '#f8f9fa';
          containerJustificativa.style.borderRadius = '5px';
          document.querySelector('.card').appendChild(containerJustificativa);
      }
      containerJustificativa.innerHTML = `<p style="font-size:0.9em; margin:0;"><strong>Motivo da IA:</strong> ${dadosIA.justificativa}</p>`;
  }

  // BOTÃO CONCORDAR
  form.addEventListener('submit', async function(e) {
    e.preventDefault();
    const btn = form.querySelector('button[type="submit"]');
    const textoOriginal = btn.textContent;
    
    try {
        btn.disabled = true;
        btn.textContent = '💾 Salvando...';
        
        await finalizarChamado(); // Chama a função que envia piece: 2
        
        alert('✅ Chamado registrado com sucesso!');
        chamadoStorage.limpar();
        window.location.href = '/menu'; // Ou sua página de sucesso
        
    } catch (erro) {
        console.error(erro);
        alert('Erro ao salvar: ' + erro.message);
        btn.disabled = false;
        btn.textContent = textoOriginal;
    }
  });

  // BOTÃO CONTESTAR
  const btnContestar = document.querySelector('.back-button'); // Ou o seletor correto do seu botão "Não Concordo"
  if (btnContestar) {
      btnContestar.onclick = (e) => {
          e.preventDefault();
          window.location.href = '/contestacao';
      };
  }
}

// ========================================
// 4. LÓGICA DA TELA DE CONTESTAÇÃO
// ========================================
function iniciarContestacao() {
    const form = document.querySelector('form');
    if (!form) return;

    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const novaPrioridade = document.getElementById('prioridade-usuario').value;
        const novaJustificativa = document.getElementById('justificativa').value;

        if(!novaPrioridade || !novaJustificativa) {
            alert("Preencha todos os campos");
            return;
        }

        // Salva a contestação
        chamadoStorage.salvarEtapa('contestacao', {
            prioridadeUsuario: novaPrioridade,
            justificativa: novaJustificativa
        });

        // Marca que houve contestação na flag da IA também
        const dadosIA = chamadoStorage.obterEtapa('ia_response') || {};
        chamadoStorage.salvarEtapa('ia_response', {
            ...dadosIA,
            contestado: true
        });

        const btn = form.querySelector('button[type="submit"]');
        const textoOriginal = btn.textContent;

        try {
            btn.disabled = true;
            btn.textContent = '💾 Salvando contestação...';

            await finalizarChamado(); // Chama a função que envia piece: 2

            alert('✅ Chamado registrado com sua prioridade!');
            chamadoStorage.limpar();
            window.location.href = '/menu';

        } catch (erro) {
            alert('Erro ao salvar: ' + erro.message);
            btn.disabled = false;
            btn.textContent = textoOriginal;
        }
    });
}
/*
// ========================================
// ✅ CORRIGIDO: FINALIZAR CHAMADO
// ========================================
async function finalizarChamado() {
  try {
    console.log('💾 Finalizando e salvando chamado no banco...');
    
    // Coleta todos os dados
    const todosOsDados = chamadoStorage.obterTodos();
    const iaResponse = todosOsDados.ia_response;

    // Mapeia prioridade para número
    const prioridadeMap = {
      'Baixa': 1,
      'Média': 2,
      'Alta': 3,
      'Crítica': 4
    };

    // ✅ CORREÇÃO: Busca ID do usuário pela API
    const userId = await buscarUsuarioPorEmail(todosOsDados.etapa1.email);

    // Monta payload para API
    const chamadoData = {
      titulo: todosOsDados.etapa1.titulo,
      categoria: todosOsDados.etapa1.categoria,
      descricao: todosOsDados.etapa1.descricao,
      prioridade: prioridadeMap[iaResponse.prioridade] || 2,
      afetadoId: userId, // ✅ AGORA USA O ID CORRETO DO BANCO
      usuarioNome: todosOsDados.etapa1.nome,
      usuarioEmail: todosOsDados.etapa1.email,
      impacto: todosOsDados.etapa2.afetado,
      bloqueioTotal: todosOsDados.etapa3.bloqueioTotal === 'sim'
    };

    console.log('📤 Enviando para API:', chamadoData);

    // Envia para API
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(chamadoData)
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Erro HTTP ${response.status}: ${errorText}`);
    }

    const resultado = await response.json();
    console.log('✅ Chamado salvo no banco:', resultado);

    return resultado;
  } catch (error) {
    console.error('❌ Erro ao finalizar chamado:', error);
    throw error;
  }
} */

// ========================================
// INICIALIZAÇÃO
// ========================================
document.addEventListener('DOMContentLoaded', function() {
  const url = window.location.pathname;
  
  console.log('📍 URL atual:', url);

  if (url.includes('Contestação')) {
    iniciarContestacao();
  }
  else if (url.includes('PrioridadeIA')) {
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
  else if (url.includes('registrar-chamado') || url.includes('Registrar-Chamados')) {
    inicializarEtapa1();
  }

  // Funções globais para debug
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