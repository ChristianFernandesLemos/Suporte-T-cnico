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
      if (!dados || !dados.etapa1) return null;

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
// ✅ CORRIGIDO: ENVIAR PARA N8N (IA)
// ========================================
async function enviarParaIA() {
  try {
    console.log('🤖 Enviando dados para IA (N8N)...');
    
    // Coleta todos os dados
    const todosOsDados = chamadoStorage.obterTodos();
    
    // Payload para N8N
    const payload = {
      id_usuario: null, // Será preenchido depois
      title: todosOsDados.etapa1.titulo,
      employeeName: todosOsDados.etapa1.nome,
      email: todosOsDados.etapa1.email,
      category: todosOsDados.etapa1.categoria,
      description: todosOsDados.etapa1.descricao,
      affectedPeople: todosOsDados.etapa2.afetado,
      blocksWork: todosOsDados.etapa3.bloqueioTotal === 'sim' ? 'Sim' : 'Não',
      userPriority: '', 
      porqueprioridade: '', 
      piece: 2,
    };

    console.log('📤 Payload para N8N:', payload);

    // Envia para N8N
    const response = await fetch(N8N_WEBHOOK_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    });

    console.log('📊 Status da resposta:', response.status);
    console.log('📊 Headers:', response.headers);

    // ✅ CORREÇÃO CRÍTICA: Primeiro pega o texto, depois tenta parsear
    const textoResposta = await response.text();
    console.log('📄 Resposta RAW do N8N:', textoResposta);
    
    let resultado;
    
    // Tenta fazer parse do JSON
    try {
      resultado = JSON.parse(textoResposta);
      console.log('✅ JSON parseado com sucesso:', resultado);
    } catch (parseError) {
      console.error('⚠️ Erro ao parsear JSON:', parseError);
      console.log('📄 Texto recebido:', textoResposta);
      
      // Se não for JSON válido, usa valores padrão
      resultado = {
        userPriority: 'Média',
        porqueprioridade: `Resposta padrão - N8N retornou: ${textoResposta.substring(0, 100)}`
      };
    }

    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status} - ${textoResposta}`);
    }

    console.log('✅ Resposta processada da IA:', resultado);

    // Salva resposta da IA
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: resultado.userPriority || resultado.prioridade || 'Média',
      justificativa: resultado.porqueprioridade || resultado.justificativa || 'Análise automática',
      timestamp: new Date().toISOString()
    });

    return resultado;
  } catch (error) {
    console.error('❌ Erro ao enviar para IA:', error);
    
    // Em caso de erro, salva resposta padrão para não bloquear o fluxo
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: 'Média',
      justificativa: 'Erro ao contactar IA - Prioridade definida automaticamente',
      timestamp: new Date().toISOString(),
      erro: true
    });
    
    // Não lança erro, permite continuar
    /*return {
      userPriority: 'Média',
      porqueprioridade: 'Erro ao contactar IA'
    };*/
  }
}

// ========================================
// PRIORIDADE ATRIBUÍDA PELA IA
// ========================================
function iniciarPrioridadeIA() {
  const form = document.querySelector('form');
  
  if (!form) return;
  console.log('📊 Prioridade IA inicializada');

  // Verifica resposta da IA
  const dadosIA = chamadoStorage.obterEtapa('ia_response');
  console.log('📦 Dados da IA recuperados:', dadosIA);
  
  if (!dadosIA) {
    console.error('❌ Nenhuma resposta da IA encontrada!');
    alert('❌ Nenhuma resposta da IA encontrada. Voltando...');
    window.location.href = '/registrar-chamado-p4';
    return;
  }

  // ✅ CORREÇÃO: Seleciona os elementos corretos do HTML
  const prioridadeElement = document.querySelector('.prioridade');
  const paragrafosCard = document.querySelectorAll('.card p');
  
  console.log('🔍 Elementos encontrados:', {
    prioridadeElement,
    totalParagrafos: paragrafosCard.length
  });
  
  const prioridadeTexto = dadosIA.prioridade || 'Não definida';
  const justificativaTexto = dadosIA.justificativa || 'Sem justificativa';
  
  console.log('📝 Valores a exibir:', {
    prioridade: prioridadeTexto,
    justificativa: justificativaTexto
  });
  
  // Atualiza o texto da prioridade
  if (prioridadeElement) {
    prioridadeElement.innerHTML = `<strong>Prioridade: ${prioridadeTexto}</strong>`;
    console.log('✅ Prioridade atualizada no elemento');
  }
  
  // Adiciona a justificativa no parágrafo vazio que vem depois
  if (paragrafosCard.length >= 3) {
    const paragrafoJustificativa = paragrafosCard[2]; // Terceiro <p>
    paragrafoJustificativa.innerHTML = `<em>${justificativaTexto}</em>`;
    paragrafoJustificativa.style.fontSize = '0.9em';
    paragrafoJustificativa.style.color = '#666';
    paragrafoJustificativa.style.marginTop = '10px';
    console.log('✅ Justificativa adicionada');
  }

  // Atualizar header
  const headerBackLink = document.querySelector('.back-link');
  if (headerBackLink) {
    headerBackLink.textContent = '← Voltar';
    headerBackLink.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado-p4';
    });
  }

  // Botão Concordar - Salva no banco
  form.addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const submitBtn = form.querySelector('button[type="submit"]');
    const textoOriginal = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = '⏳ Salvando chamado...';

    try {
      await finalizarChamado();
      alert('✅ Chamado registrado com sucesso!');
      chamadoStorage.limpar();
      window.location.href = '/menu';
    } catch (error) {
      console.error('❌ Erro:', error);
      alert('❌ Erro ao salvar chamado. Tente novamente.');
      submitBtn.disabled = false;
      submitBtn.textContent = textoOriginal;
    }
  });

  // Botão Não Concordar - Vai para contestação
  const btnContestar = document.querySelector('.back-button');
  if (btnContestar) {
    btnContestar.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/contestacao';
    });
  }
}

// ========================================
// CONTESTAÇÃO
// ========================================
function iniciarContestacao() {
  const form = document.querySelector('form');
  if (!form) return;
  
  console.log('⚖️ Contestação iniciada');

  // Carrega dados salvos
  const dadosSalvos = chamadoStorage.obterEtapa('contestacao');
  if (dadosSalvos) {
    const prioridadeUsuario = document.getElementById('prioridade-usuario');
    const justificativa = document.getElementById('justificativa');
    
    if (prioridadeUsuario) prioridadeUsuario.value = dadosSalvos.prioridadeUsuario || '';
    if (justificativa) justificativa.value = dadosSalvos.justificativa || '';
  }

  // Atualiza header
  const headerBackLink = document.querySelector('.back-link');
  if (headerBackLink) {
    headerBackLink.textContent = '← Voltar';
    headerBackLink.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/prioridadeia';
    });
  }

  // Submit da contestação
  form.addEventListener('submit', async function(e) {
    e.preventDefault();

    const prioridadeUsuario = document.getElementById('prioridade-usuario').value;
    const justificativa = document.getElementById('justificativa').value.trim();

    if (!prioridadeUsuario || !justificativa) {
      alert('⚠️ Por favor, selecione uma prioridade e justifique.');
      return;
    }

    // Salva contestação
    chamadoStorage.salvarEtapa('contestacao', {
      prioridadeUsuario,
      justificativa
    });

    // Sobrescreve resposta da IA com escolha do usuário
    chamadoStorage.salvarEtapa('ia_response', {
      prioridade: prioridadeUsuario,
      justificativa: `CONTESTADO PELO USUÁRIO: ${justificativa}`,
      contestado: true
    });

    const submitBtn = form.querySelector('button[type="submit"]');
    const textoOriginal = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = '⏳ Salvando com sua prioridade...';

    try {
      await finalizarChamado();
      alert('✅ Chamado registrado com sua prioridade!');
      chamadoStorage.limpar();
      window.location.href = '/menu';
    } catch (error) {
      console.error('❌ Erro:', error);
      alert('❌ Erro ao salvar. Tente novamente.');
      submitBtn.disabled = false;
      submitBtn.textContent = textoOriginal;
    }
  });
}

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
}

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