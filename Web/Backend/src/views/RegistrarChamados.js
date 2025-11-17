// RegistrarChamados.js - Sistema de registro de chamados multi-etapas
console.log('🚀 Sistema de Registro de Chamados Carregado');

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
  }
};

// =========================================================================================================
// ETAPA 1 - Informações Básicas
// =========================================================================================================
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

// ====================================================================================================================
// ETAPA 2 - Quem está sendo afetado
// ====================================================================================================================
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
      //alert('✅ Chamado salvo com sucesso! (Etapa 3 ainda não implementada)');
      // Quando criar a etapa 3, descomente:
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

// ============================================================================================================
// ETAPA 3 - O problema impede o trabalho?
// ===========================================================================================================
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

  // Carrega dados salvos (para radio buttons)
  const dadosSalvos = chamadoStorage.obterEtapa('etapa3');
  if (dadosSalvos && dadosSalvos.impacto) {
    console.log('📂 Carregando dados salvos:', dadosSalvos.impacto);
    // Marca o radio button correto
    const radioSelecionado = document.querySelector(`input[name="impacto"][value="${dadosSalvos.impacto}"]`);
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

    // ✅ CORREÇÃO: Pega o valor do radio button selecionado
    const impactoSelecionado = document.querySelector('input[name="impacto"]:checked');
    
    if (!impactoSelecionado) {
      alert('⚠️ Por favor, selecione se o problema impede o trabalho.');
      return;
    }

    const dados = {
      impacto: impactoSelecionado.value  // 'sim' ou 'nao'
    };

    console.log('📊 Dados da Etapa 3:', dados);

    // Salva e avança
    if (chamadoStorage.salvarEtapa('etapa3', dados)) {
      console.log('✅ Etapa 3 concluída');
      //alert('✅ Chamado salvo com sucesso! (Etapa 4 ainda não implementada)');
      // Quando criar a etapa 4, descomente:
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


//========================================================================================================
// Etapa 4 - Confirmação de Conclusão de chamado
//=========================================================================================================

// Evento de submit
  
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

  // Evento de submit
  
  form.addEventListener('submit', function(e) {
    e.preventDefault();

    // Cria dados
    const dados = {
    finalizado: true,
    dataFinalizacao: new Date().toISOString()
};

     // Salva e avança
    if (chamadoStorage.salvarEtapa('etapa4', dados)) {
      console.log('✅ Etapa 4 concluída');
      //alert('✅ Chamado salvo com sucesso! (Etapa 4 ainda não implementada)');
      // Quando criar a etapa 4, descomente:
      window.location.href = '/PrioridadeIA';
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

//=====================================================================================================
// PRIORIDADE ATRIBUIDA PELA I.A
//=====================================================================================================

function iniciarPrioridadeIA(){
   const form=document.querySelector('form');
  
   if(!form) return;
   console.log('prioridade I.A mostrada')

   // verifica etapa anterior
   const dadosEtapa4 = chamadoStorage.obterEtapa('etapa4');
   if(!dadosEtapa4){
    alert('Nenhum dado encontrado. Voltando para a quarta etapa.');
    window.location.href = '/registrar-chamado-p3';
    return;
   }

   // atualizar header
   const headerBackLink = document.querySelector('.back-link');
  if (headerBackLink) {
    headerBackLink.textContent = '← Voltar';
    headerBackLink.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/registrar-chamado-p4';
    });
  }

    const dados = {
    finalizado: true,
     dataFinalizacao: new Date().toISOString()
     };

    // Concorda com a prioridade Salva e finaliza
    if (chamadoStorage.salvarEtapa('Prioridade', dados)) {
      console.log('✅ Prioridade concluída');
      //finalizarChamado(); // Função para enviar para API
    } else {
      alert('❌ Erro ao salvar. Tente novamente.');
    }

    // Não concorda com a prioridade
  const btnVoltar = document.querySelector('.back-button');
  if (btnVoltar) {
    btnVoltar.addEventListener('click', function(e) {
      e.preventDefault();
      window.location.href = '/Contestação';
    });
  }
}


//=======================================================
// CONTESTAÇÃO
//=======================================================

function iniciarContestacao(){
     const form = document.querySelector('form');
     if (!form) return;
     console.log('Contestação iniciada');




}







//===================================================================================
// INICIALIZAÇÃO - ATUALIZADA
// ==================================================================================
document.addEventListener('DOMContentLoaded', function() {
  const url = window.location.pathname;
  
  console.log('📍 URL atual:', url);

  if (url.includes('Contestação')){
    iniciarContestacao()
  }
  else if (url.includes('PrioridadeIA')){
    iniciarPrioridadeIA();
  }
  if(url.includes('registrar-chamado-p4')){
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