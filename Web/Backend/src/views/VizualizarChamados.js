// VizualizarChamados.js - Frontend para lista-chamados (1).html
console.log('🚀 VizualizarChamados.js carregado');

// ========================================
// MAPEAMENTOS
// ========================================
const STATUS = {
  ABERTO: 1,
  EM_ANDAMENTO: 2,
  RESOLVIDO: 3,
  FECHADO: 4,
  CANCELADO: 5
};

const PRIORIDADE = {
  BAIXA: 1,
  MEDIA: 2,
  ALTA: 3,
  CRITICA: 4
};

// ========================================
// CONFIGURAÇÃO DA API
// ========================================
const API_URL = 'http://localhost:3000/api/chamados';

// ========================================
// FUNÇÕES DE LABELS
// ========================================
function getPrioridadeLabel(prioridade) {
  const labels = {
    [PRIORIDADE.BAIXA]: 'Baixa',
    [PRIORIDADE.MEDIA]: 'Média',
    [PRIORIDADE.ALTA]: 'Alta',
    [PRIORIDADE.CRITICA]: 'Crítica'
  };
  return labels[prioridade] || 'Desconhecida';
}

function getPrioridadeClass(prioridade) {
  const classes = {
    [PRIORIDADE.BAIXA]: 'priority-low',
    [PRIORIDADE.MEDIA]: 'priority-medium',
    [PRIORIDADE.ALTA]: 'priority-high',
    [PRIORIDADE.CRITICA]: 'priority-critical'
  };
  return classes[prioridade] || 'priority-low';
}

function getStatusLabel(status) {
  const labels = {
    [STATUS.ABERTO]: 'Aberto',
    [STATUS.EM_ANDAMENTO]: 'Em Andamento',
    [STATUS.RESOLVIDO]: 'Resolvido',
    [STATUS.FECHADO]: 'Fechado',
    [STATUS.CANCELADO]: 'Cancelado'
  };
  return labels[status] || 'Desconhecido';
}

function formatarData(dataStr) {
  if (!dataStr) return 'N/A';
  
  const data = new Date(dataStr);
  
  // Formata como DD/MM/YYYY
  const dia = String(data.getDate()).padStart(2, '0');
  const mes = String(data.getMonth() + 1).padStart(2, '0');
  const ano = data.getFullYear();
  
  return `${dia}/${mes}/${ano}`;
}

// ========================================
// BUSCAR CHAMADOS DO BANCO
// ========================================
async function buscarChamados() {
  try {
    console.log('📡 Buscando chamados do banco de dados...', API_URL);
    
    const response = await fetch(API_URL);
    console.log('📊 Response status:', response.status);
    
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status}`);
    }

    const data = await response.json();
    console.log('📦 Dados recebidos:', data);
    
    if (data.success) {
      console.log(`✅ ${data.total} chamados encontrados:`, data.chamados);
      return data.chamados;
    } else {
      console.error('❌ Erro no formato da resposta:', data);
      throw new Error(data.message || 'Erro ao buscar chamados');
    }
  } catch (error) {
    console.error('❌ Erro completo ao buscar chamados:', error);
    mostrarErro(`Erro: ${error.message}`);
    return [];
  }
}

// ========================================
// RENDERIZAR TABELA
// ========================================
function renderizarTabela(chamados) {
  const tbody = document.querySelector('.tickets-table tbody');
  
  if (!tbody) {
    console.error('❌ Tbody não encontrado');
    return;
  }

  // Limpa tbody
  tbody.innerHTML = '';

  if (chamados.length === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="6" style="text-align: center; padding: 40px;">
          📭 Nenhum chamado encontrado
        </td>
      </tr>
    `;
    return;
  }

  // Renderiza cada chamado
  chamados.forEach(chamado => {
    const tr = document.createElement('tr');
    
    const prioridadeClass = getPrioridadeClass(chamado.prioridade);
    const prioridadeLabel = getPrioridadeLabel(chamado.prioridade);
    const statusLabel = getStatusLabel(chamado.status);
    const dataFormatada = formatarData(chamado.dataAbertura);
    
    // Cria a célula de categoria como "título" já que não temos titulo na tabela
    const titulo = chamado.titulo || 'Sem categoria';
    
    tr.innerHTML = `
      <td class="actions-cell">
        <button class="action-btn" aria-label="Visualizar chamado ${chamado.id}" onclick="verDetalhes(${chamado.id})">
          <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M10 4C5 4 1.73 7.11 1 10c.73 2.89 4 6 9 6s8.27-3.11 9-6c-.73-2.89-4-6-9-6z" stroke="currentColor" stroke-width="2"/>
            <circle cx="10" cy="10" r="3" stroke="currentColor" stroke-width="2"/>
          </svg>
        </button>
        <button class="action-btn" aria-label="Editar chamado ${chamado.id}" onclick="editarChamado(${chamado.id})">
          <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M14.5 2.5a2.121 2.121 0 113 3L6 17H3v-3L14.5 2.5z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
      </td>
      <td>${chamado.id}</td>
      <td>${titulo}</td>
      <td>${dataFormatada}</td>
      <td><span class="priority-badge ${prioridadeClass}">${prioridadeLabel}</span></td>
      <td>${statusLabel}</td>
    `;
    
    tbody.appendChild(tr);
  });
}

// ========================================
// AÇÕES - REDIRECIONAMENTO PARA PÁGINAS
// ========================================
function verDetalhes(id) {
  console.log('👁️ Redirecionando para detalhes do chamado:', id);
  window.location.href = `/detalhes-chamado?id=${id}`;
}

function editarChamado(id) {
  console.log('✏️ Redirecionando para edição do chamado:', id);
  window.location.href = `/editar-chamado?id=${id}`;
}

// ========================================
// MOSTRAR ERRO
// ========================================
function mostrarErro(mensagem) {
  const tbody = document.querySelector('.tickets-table tbody');
  if (tbody) {
    tbody.innerHTML = `
      <tr>
        <td colspan="6" style="text-align: center; padding: 40px; color: #e53e3e;">
          ❌ ${mensagem}
          <br><br>
          <button onclick="location.reload()" style="padding: 10px 20px; background: #667eea; color: white; border: none; border-radius: 8px; cursor: pointer;">
            🔄 Tentar Novamente
          </button>
        </td>
      </tr>
    `;
  }
}

// ========================================
// INICIALIZAÇÃO
// ========================================
async function inicializar() {
  console.log('🚀 Inicializando lista de chamados');
  
  // Mostra loading
  const tbody = document.querySelector('.tickets-table tbody');
  if (tbody) {
    tbody.innerHTML = `
      <tr>
        <td colspan="6" style="text-align: center; padding: 40px;">
          ⏳ Carregando chamados do banco de dados...
        </td>
      </tr>
    `;
  }
  
  try {
    // Busca chamados do banco
    const chamados = await buscarChamados();
    
    // Renderiza tabela
    renderizarTabela(chamados);
    
    console.log('✅ Sistema inicializado com sucesso');
  } catch (error) {
    console.error('❌ Erro ao inicializar:', error);
    mostrarErro('Erro ao carregar chamados. Tente novamente.');
  }
}

// ========================================
// CONFIGURA BOTÃO VOLTAR
// ========================================
function configurarBotaoVoltar() {
  const backLink = document.querySelector('.back-link');
  if (backLink) {
    backLink.addEventListener('click', (e) => {
      e.preventDefault();
      window.location.href = '/menu';
    });
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

// Atualiza a cada 30 segundos
setInterval(async () => {
  console.log('🔄 Atualizando chamados...');
  const chamados = await buscarChamados();
  renderizarTabela(chamados);
}, 30000);

// Expõe funções globalmente para os botões HTML
window.verDetalhes = verDetalhes;
window.editarChamado = editarChamado;