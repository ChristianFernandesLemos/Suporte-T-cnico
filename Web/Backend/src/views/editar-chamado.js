// editar-chamado.js - Edição de chamados
console.log('🚀 editar-chamado.js carregado');

// ========================================
// MAPEAMENTOS
// ========================================
const STATUS = {
  1: 'Aberto',
  2: 'Em Andamento',
  3: 'Resolvido',
  4: 'Fechado',
  5: 'Cancelado'
};

const PRIORIDADE = {
  1: 'Baixa',
  2: 'Média',
  3: 'Alta',
  4: 'Crítica'
};

// ========================================
// CONFIGURAÇÃO DA API
// ========================================
const API_URL = 'http://localhost:3000/api/chamados';

// ========================================
// FUNÇÕES AUXILIARES
// ========================================
function obterIdDaURL() {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get('id');
}

// ========================================
// BUSCAR DADOS DO CHAMADO
// ========================================
async function buscarChamado(id) {
  try {
    console.log(`📡 Buscando chamado #${id} para edição...`);
    
    const response = await fetch(`${API_URL}/${id}`);
    
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status}`);
    }

    const data = await response.json();
    console.log('📦 Dados recebidos:', data);
    
    if (data.success && data.chamado) {
      console.log('✅ Chamado encontrado:', data.chamado);
      return data.chamado;
    } else {
      throw new Error(data.message || 'Chamado não encontrado');
    }
  } catch (error) {
    console.error('❌ Erro ao buscar chamado:', error);
    throw error;
  }
}

// ========================================
// PREENCHER FORMULÁRIO
// ========================================
function preencherFormulario(chamado) {
  console.log('📝 Preenchendo formulário com dados:', chamado);
  
  // Atualiza título da página
  const titulo = document.querySelector('.form-title');
  if (titulo) {
    titulo.textContent = `Editar Chamado #${chamado.id}`;
  }

  // Preenche campos do formulário
  document.getElementById('titulo').value = chamado.titulo || '';
  document.getElementById('nome').value = chamado.usuarioNome || '';
  document.getElementById('email').value = chamado.usuarioEmail || '';
  document.getElementById('categoria').value = chamado.categoria || '';
  document.getElementById('impacto').value = chamado.impacto || '';
  document.getElementById('bloqueio').value = chamado.bloqueioTotal ? 'Sim' : 'Não';
  document.getElementById('prioridade').value = PRIORIDADE[chamado.prioridade] || '';
  document.getElementById('status').value = STATUS[chamado.status] || '';
  document.getElementById('descricao').value = chamado.descricao || '';
  
  console.log('✅ Formulário preenchido');
}

// ========================================
// SALVAR ALTERAÇÕES
// ========================================
async function salvarAlteracoes(event) {
  event.preventDefault();
  
  const chamadoId = obterIdDaURL();
  
  if (!chamadoId) {
    alert('❌ ID do chamado não encontrado!');
    return;
  }

  // Coleta dados do formulário
  const dadosAtualizados = {
    titulo: document.getElementById('titulo').value,
    usuarioNome: document.getElementById('nome').value,
    usuarioEmail: document.getElementById('email').value,
    categoria: document.getElementById('categoria').value,
    impacto: document.getElementById('impacto').value,
    bloqueioTotal: document.getElementById('bloqueio').value.toLowerCase() === 'sim',
    descricao: document.getElementById('descricao').value,
    // Converte prioridade de texto para número
    prioridade: Object.keys(PRIORIDADE).find(
      key => PRIORIDADE[key] === document.getElementById('prioridade').value
    ),
    // Converte status de texto para número
    status: Object.keys(STATUS).find(
      key => STATUS[key] === document.getElementById('status').value
    )
  };

  console.log('💾 Salvando alterações:', dadosAtualizados);

  try {
    const response = await fetch(`${API_URL}/${chamadoId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(dadosAtualizados)
    });

    const data = await response.json();

    if (response.ok && data.success) {
      console.log('✅ Chamado atualizado com sucesso!');
      alert('✅ Chamado atualizado com sucesso!');
      
      // Redireciona para detalhes ou lista
      window.location.href = `/detalhes-chamado?id=${chamadoId}`;
    } else {
      throw new Error(data.message || 'Erro ao atualizar chamado');
    }
  } catch (error) {
    console.error('❌ Erro ao salvar:', error);
    alert(`❌ Erro ao salvar alterações: ${error.message}`);
  }
}

// ========================================
// MOSTRAR ERRO
// ========================================
function mostrarErro(mensagem) {
  const mainContent = document.querySelector('.main-content');
  if (mainContent) {
    mainContent.innerHTML = `
      <section class="form-section" style="text-align: center; padding: 40px;">
        <h1 style="color: #e53e3e; margin-bottom: 20px;">❌ Erro</h1>
        <p style="margin-bottom: 20px;">${mensagem}</p>
        <button onclick="voltarParaLista()" style="padding: 12px 24px; background: #667eea; color: white; border: none; border-radius: 8px; cursor: pointer; font-size: 16px;">
          ← Voltar para Lista de Chamados
        </button>
      </section>
    `;
  }
}

// ========================================
// NAVEGAÇÃO
// ========================================
function voltarParaLista() {
  window.location.href = '/chamados';
}

function configurarBotaoVoltar() {
  const backLink = document.querySelector('.back-link');
  if (backLink) {
    backLink.addEventListener('click', (e) => {
      e.preventDefault();
      voltarParaLista('/chamados');
    });
  }
}

// ========================================
// INICIALIZAÇÃO
// ========================================
async function inicializar() {
  console.log('🚀 Inicializando página de edição');
  
  try {
    // Obtém ID da URL
    const chamadoId = obterIdDaURL();
    
    if (!chamadoId) {
      throw new Error('ID do chamado não fornecido na URL');
    }

    console.log(`🔍 ID do chamado: ${chamadoId}`);

    // Busca dados do chamado
    const chamado = await buscarChamado(chamadoId);
    
    // Preenche formulário
    preencherFormulario(chamado);
    
    // Configura envio do formulário
    const form = document.querySelector('.ticket-form');
    if (form) {
      form.addEventListener('submit', salvarAlteracoes);
    }
    
    console.log('✅ Página inicializada com sucesso');
  } catch (error) {
    console.error('❌ Erro ao inicializar:', error);
    mostrarErro(error.message || 'Erro ao carregar chamado para edição');
  }
}

// ========================================
// EXECUÇÃO
// ========================================
// Aguarda DOM carregar
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => {
    inicializar();
    configurarBotaoVoltar();
  });
} else {
  // DOM já carregado
  inicializar();
  configurarBotaoVoltar();
}

// Expõe função globalmente
window.voltarParaLista = voltarParaLista;