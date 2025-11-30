// detalhes-chamado.js - Visualização de detalhes do chamado
// VERSÃO V2 - CONTESTAÇÕES OTIMIZADAS E VISUAL MELHORADO
console.log('🚀 detalhes-chamado.js V2 carregado');

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
const CONTESTACOES_URL = 'http://localhost:3000/api/contestacoes';

// ========================================
// FUNÇÕES AUXILIARES
// ========================================
function formatarData(dataStr) {
  if (!dataStr) return 'N/A';
  
  const data = new Date(dataStr);
  const dia = String(data.getDate()).padStart(2, '0');
  const mes = String(data.getMonth() + 1).padStart(2, '0');
  const ano = data.getFullYear();
  const hora = String(data.getHours()).padStart(2, '0');
  const minuto = String(data.getMinutes()).padStart(2, '0');
  
  return `${dia}/${mes}/${ano} às ${hora}:${minuto}`;
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
// BUSCAR CONTESTAÇÕES DO CHAMADO
// ========================================
async function buscarContestacoes(idChamado) {
  try {
    console.log(`📡 Buscando contestações do chamado #${idChamado}...`);
    
    const response = await fetch(`${CONTESTACOES_URL}/chamado/${idChamado}`);
    
    // 🚨 CORREÇÃO: Removemos a checagem específica por 404. 
    // Se a busca falhar (4xx ou 5xx), lançamos um erro.
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status} ao buscar contestações.`);
    }

    const data = await response.json();
    console.log('📦 Contestações recebidas:', data);
    
    if (data.success && Array.isArray(data.contestacoes)) {
      console.log(`✅ ${data.contestacoes.length} contestação(ões) encontrada(s)`);
      return data.contestacoes;
    } else {
      console.log('ℹ️ Nenhuma contestação no resultado ou formato inválido do Backend.');
      return [];
    }
  } catch (error) {
    console.error('⚠️ Erro ao buscar contestações:', error);
    // 🚨 AQUI, AGORA, LOGAREMOS O ERRO NO CONSOLE (ex: Erro HTTP: 500)
    // O retorno [] garante que a renderização não quebre.
    return [];
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
  atualizarCampo('Título', chamado.titulo || 'Sem título');
  atualizarCampo('Nome', chamado.usuarioNome || 'Não informado');
  atualizarCampo('Email', chamado.usuarioEmail || 'Não informado');
  atualizarCampo('Categoria', chamado.categoria || 'Não categorizado');
  atualizarCampo('Criado em', formatarData(chamado.dataAbertura));
  atualizarCampo('Prioridade', PRIORIDADE[chamado.prioridade] || 'Não definida');
  atualizarCampo('Status', STATUS[chamado.status] || 'Desconhecido');
  atualizarCampo('Descrição', chamado.descricao || 'Sem descrição');
}

// ========================================
// RENDERIZAR CONTESTAÇÕES - VERSÃO MELHORADA
// ========================================
function renderizarContestacoes(contestacoes) {
  console.log('🎨 Renderizando contestações:', contestacoes);
  
  const items = Array.from(document.querySelectorAll('.detail-item'));
  const contestacaoDiv = items.find(item => {
    const label = item.querySelector('.detail-label');
    return label && label.textContent.includes('Contestação');
  });

  if (!contestacaoDiv) {
    console.warn('⚠️ Elemento de contestação não encontrado no HTML');
    return;
  }

  const valueElement = contestacaoDiv.querySelector('.detail-value');
  
  if (contestacoes.length === 0) {
    valueElement.innerHTML = `
      <div style="color: #718096; padding: 10px; background: #f7fafc; border-radius: 6px; font-style: italic;">
        ✅ Nenhuma contestação registrada para este chamado.
      </div>
    `;
    return;
  }

  // Cria HTML estilizado para as contestações
  let html = `
    <div style="
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 12px 16px;
      border-radius: 8px 8px 0 0;
      margin-bottom: 0;
      font-weight: 600;
      display: flex;
      justify-content: space-between;
      align-items: center;
    ">
      <span>📋 Histórico de Contestações</span>
      <span style="
        background: rgba(255,255,255,0.2);
        padding: 4px 12px;
        border-radius: 12px;
        font-size: 0.875rem;
      ">${contestacoes.length} registro(s)</span>
    </div>
    <div class="contestacoes-list" style="
      display: flex;
      flex-direction: column;
      gap: 16px;
      background: #f7fafc;
      padding: 16px;
      border-radius: 0 0 8px 8px;
    ">
  `;
  
  contestacoes.forEach((cont, index) => {
    const tipo = cont.Tipo || 'Não especificado';
    const tipoIcon = tipo === 'Discordo da Prioridade' ? '⚠️' : 'ℹ️';
    const tipoColor = tipo === 'Discordo da Prioridade' ? '#f59e0b' : '#667eea';
    const data = formatarData(cont.DataContestacao);
    const usuario = cont.usuarioNome || 'Usuário não identificado';
    const email = cont.usuarioEmail || '';
    const justificativa = cont.Justificativa || 'Sem justificativa fornecida';
    
    html += `
      <div class="contestacao-item" style="
        background: white;
        padding: 16px;
        border-radius: 8px;
        border-left: 4px solid ${tipoColor};
        box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        transition: transform 0.2s, box-shadow 0.2s;
      ">
        <!-- Header da contestação -->
        <div style="
          display: flex;
          justify-content: space-between;
          align-items: start;
          margin-bottom: 12px;
          padding-bottom: 12px;
          border-bottom: 1px solid #e2e8f0;
        ">
          <div>
            <div style="
              display: flex;
              align-items: center;
              gap: 8px;
              margin-bottom: 4px;
            ">
              <span style="font-size: 1.2rem;">${tipoIcon}</span>
              <strong style="color: ${tipoColor}; font-size: 1rem;">
                ${tipo}
              </strong>
            </div>
            <div style="
              color: #64748b;
              font-size: 0.875rem;
              margin-left: 28px;
            ">
              Contestação #${(index + 1).toString().padStart(2, '0')}
            </div>
          </div>
          <div style="
            text-align: right;
            color: #64748b;
            font-size: 0.875rem;
          ">
            <div style="font-weight: 500; color: #475569;">
              📅 ${data}
            </div>
          </div>
        </div>

        <!-- Justificativa -->
        <div style="
          background: #f8fafc;
          padding: 12px;
          border-radius: 6px;
          margin-bottom: 12px;
          border-left: 3px solid #e2e8f0;
        ">
          <div style="
            font-size: 0.75rem;
            text-transform: uppercase;
            color: #94a3b8;
            font-weight: 600;
            margin-bottom: 6px;
            letter-spacing: 0.5px;
          ">
            Justificativa
          </div>
          <p style="
            margin: 0;
            color: #334155;
            line-height: 1.6;
            font-size: 0.95rem;
          ">${justificativa}</p>
        </div>

        <!-- Footer com info do usuário -->
        <div style="
          display: flex;
          align-items: center;
          gap: 8px;
          padding-top: 8px;
          border-top: 1px solid #f1f5f9;
        ">
          <div style="
            width: 32px;
            height: 32px;
            border-radius: 50%;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-weight: 600;
            font-size: 0.875rem;
          ">
            ${usuario.charAt(0).toUpperCase()}
          </div>
          <div style="flex: 1;">
            <div style="
              font-weight: 500;
              color: #334155;
              font-size: 0.875rem;
            ">${usuario}</div>
            ${email ? `
              <div style="
                color: #64748b;
                font-size: 0.75rem;
              ">${email}</div>
            ` : ''}
          </div>
        </div>
      </div>
    `;
  });
  
  html += '</div>';
  
  valueElement.innerHTML = html;
  console.log('✅ Contestações renderizadas com design aprimorado');
}

function atualizarCampo(label, valor) {
  const items = document.querySelectorAll('.detail-item');
  
  items.forEach(item => {
    const labelElement = item.querySelector('.detail-label');
    if (labelElement && labelElement.textContent.includes(label)) {
      const valueElement = item.querySelector('.detail-value');
      if (valueElement) {
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
        <button onclick="window.location.href='/chamados'" style="padding: 12px 24px; background: #667eea; color: white; border: none; border-radius: 8px; cursor: pointer; font-size: 16px;">
          ← Voltar para Lista de Chamados
        </button>
      </article>
    `;
  }
}

// ========================================
// NAVEGAÇÃO
// ========================================
function configurarBotaoVoltar() {
  const backLink = document.querySelector('.back-link');
  if (backLink) {
    backLink.addEventListener('click', (e) => {
      e.preventDefault();
      window.location.href = '/chamados';
    });
  }
}

// ========================================
// INICIALIZAÇÃO
// ========================================
async function inicializar() {
  console.log('🚀 Inicializando página de detalhes');
  
  try {
    const chamadoId = obterIdDaURL();
    
    if (!chamadoId) {
      throw new Error('ID do chamado não fornecido na URL');
    }

    console.log(`🔍 ID do chamado: ${chamadoId}`);

    const chamado = await buscarDetalhes(chamadoId);
    renderizarDetalhes(chamado);
    
    const contestacoes = await buscarContestacoes(chamadoId);
    renderizarContestacoes(contestacoes);
    
    console.log('✅ Página inicializada com sucesso');
  } catch (error) {
    console.error('❌ Erro ao inicializar:', error);
    mostrarErro(error.message || 'Erro ao carregar detalhes do chamado');
  }
}

// ========================================
// EXECUÇÃO
// ========================================
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => {
    inicializar();
    configurarBotaoVoltar();
  });
} else {
  inicializar();
  configurarBotaoVoltar();
}