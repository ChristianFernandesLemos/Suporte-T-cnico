// detalhes-chamado.js - Visualização de detalhes do chamado
console.log('🚀 detalhes-chamado.js carregado');

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
function formatarData(dataStr) {
  if (!dataStr) return 'N/A';
  
  const data = new Date(dataStr);
  const dia = String(data.getDate()).padStart(2, '0');
  const mes = String(data.getMonth() + 1).padStart(2, '0');
  const ano = data.getFullYear();
  
  return `${dia}/${mes}/${ano}`;
}

function obterIdDaURL() {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get('id');
}

// ========================================
// BUSCAR DETALHES DO CHAMADO
// ========================================
async function buscarDetalhes(id) {
  try {
    console.log(`📡 Buscando detalhes do chamado #${id}...`);
    
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
    console.error('❌ Erro ao buscar detalhes:', error);
    throw error;
  }
}

// ========================================
// RENDERIZAR DETALHES
// ========================================
function renderizarDetalhes(chamado) {
  console.log('🎨 Renderizando detalhes do chamado:', chamado);
  
  // Atualiza título
  const titulo = document.querySelector('.ticket-title');
  if (titulo) {
    titulo.textContent = `Detalhes do Chamado #${chamado.id}`;
  }

  // Atualiza os campos
  atualizarCampo('Cadastrador', chamado.cadastradorNome || 'Não informado');
  atualizarCampo('Título', chamado.categoria || 'Sem título');
  atualizarCampo('Nome', chamado.usuarioNome || 'Não informado');
  atualizarCampo('Email', chamado.usuarioEmail || 'Não informado');
  atualizarCampo('Categoria', chamado.categoria || 'Não categorizado');
  atualizarCampo('Impacto', chamado.impacto || 'Não informado');
  atualizarCampo('Bloqueio Total', chamado.bloqueioTotal ? 'Sim' : 'Não');
  atualizarCampo('Criado em', formatarData(chamado.dataAbertura));
  atualizarCampo('Prioridade', PRIORIDADE[chamado.prioridade] || 'Não definida');
  atualizarCampo('Status', STATUS[chamado.status] || 'Desconhecido');
  atualizarCampo('Descrição', chamado.descricao || 'Sem descrição');
}

function atualizarCampo(label, valor) {
  const items = document.querySelectorAll('.detail-item');
  
  items.forEach(item => {
    const labelElement = item.querySelector('.detail-label');
    if (labelElement && labelElement.textContent.includes(label)) {
      const valueElement = item.querySelector('.detail-value');
      if (valueElement) {
        // Preserva tags HTML se for data (time)
        if (label === 'Criado em') {
          valueElement.innerHTML = `<time>${valor}</time>`;
        } else {
          valueElement.textContent = valor;
        }
      }
    }
  });
}

// ========================================
// MOSTRAR ERRO
// ========================================
function mostrarErro(mensagem) {
  const mainContent = document.querySelector('.main-content');
  if (mainContent) {
    mainContent.innerHTML = `
      <article class="ticket-details" style="text-align: center; padding: 40px;">
        <h1 style="color: #e53e3e; margin-bottom: 20px;">❌ Erro</h1>
        <p style="margin-bottom: 20px;">${mensagem}</p>
        <button onclick="voltarParaLista()" style="padding: 12px 24px; background: #667eea; color: white; border: none; border-radius: 8px; cursor: pointer; font-size: 16px;">
          ← Voltar para Lista de Chamados
        </button>
      </article>
    `;
  }
}

// ========================================
// NAVEGAÇÃO
// ========================================
function voltarParaLista() {
  window.location.href = '/lista-chamados';
}

function configurarBotaoVoltar() {
  const backLink = document.querySelector('.back-link');
  if (backLink) {
    backLink.addEventListener('click', (e) => {
      e.preventDefault();
      voltarParaLista();
    });
  }
}

// ========================================
// INICIALIZAÇÃO
// ========================================
async function inicializar() {
  console.log('🚀 Inicializando página de detalhes');
  
  try {
    // Obtém ID da URL
    const chamadoId = obterIdDaURL();
    
    if (!chamadoId) {
      throw new Error('ID do chamado não fornecido na URL');
    }

    console.log(`🔍 ID do chamado: ${chamadoId}`);

    // Busca detalhes
    const chamado = await buscarDetalhes(chamadoId);
    
    // Renderiza na página
    renderizarDetalhes(chamado);
    
    console.log('✅ Página inicializada com sucesso');
  } catch (error) {
    console.error('❌ Erro ao inicializar:', error);
    mostrarErro(error.message || 'Erro ao carregar detalhes do chamado');
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