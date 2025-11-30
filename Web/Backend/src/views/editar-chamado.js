// editar-chamado.js - Edição de chamados
// VERSÃO ATUALIZADA COM SELECTS E CAMPOS READONLY
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
const CONTESTACOES_URL = 'http://localhost:3000/api/contestacoes';

// ========================================
// FUNÇÕES AUXILIARES
// ========================================
function obterIdDaURL() {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get('id');
}

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
// BUSCAR CONTESTAÇÕES DO CHAMADO
// ========================================
async function buscarContestacoes(idChamado) {
  try {
    console.log(`📡 Buscando contestações do chamado #${idChamado}...`);
    
    const response = await fetch(`${CONTESTACOES_URL}/chamado/${idChamado}`);
    
    // 🚨 CORREÇÃO: Removemos a checagem específica por 404.
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status} ao buscar contestações.`);
    }

    const data = await response.json();
    console.log('📦 Contestações recebidas:', data);
    
    if (data.success && Array.isArray(data.contestacoes)) {
      console.log(`✅ ${data.contestacoes.length} contestação(ões) encontrada(s)`);
      return data.contestacoes;
    } else {
      return [];
    }
  } catch (error) {
    console.error('⚠️ Erro ao buscar contestações:', error);
    return [];
  }
}

// ========================================
// CONFIGURAR CAMPOS READONLY
// ========================================
function configurarCamposReadonly() {
  // Lista de campos que devem ser readonly
  const camposReadonly = ['titulo', 'nome', 'email', 'descricao', 'contestacao'];
  
  camposReadonly.forEach(id => {
    const campo = document.getElementById(id);
    if (campo) {
      campo.readOnly = true;
      campo.style.backgroundColor = '#f7fafc';
      campo.style.cursor = 'not-allowed';
      campo.style.color = '#4a5568';
    }
  });
  
  console.log('✅ Campos readonly configurados');
}

// ========================================
// PREENCHER FORMULÁRIO
// ========================================
async function preencherFormulario(chamado) {
  console.log('📝 Preenchendo formulário com dados:', chamado);
  
  // Atualiza título da página
  const titulo = document.querySelector('.form-title');
  if (titulo) {
    titulo.textContent = `Editar Chamado #${chamado.id}`;
  }

  // Preenche campos readonly (texto)
  const camposTexto = {
    'titulo': chamado.titulo || 'Sem título',
    'nome': chamado.usuarioNome || 'Não informado',
    'email': chamado.usuarioEmail || 'Não informado',
    'descricao': chamado.descricao || 'Sem descrição'
  };

  Object.keys(camposTexto).forEach(id => {
    const elemento = document.getElementById(id);
    if (elemento) {
      elemento.value = camposTexto[id];
      console.log(`✓ Campo ${id} preenchido`);
    }
  });

  // Preenche SELECT de Categoria
  const selectCategoria = document.getElementById('categoria');
  if (selectCategoria && chamado.categoria) {
    selectCategoria.value = chamado.categoria;
    console.log(`✓ Categoria selecionada: ${chamado.categoria}`);
  }

  // Preenche SELECT de Prioridade (valor numérico)
  const selectPrioridade = document.getElementById('prioridade');
  if (selectPrioridade && chamado.prioridade) {
    selectPrioridade.value = chamado.prioridade.toString();
    console.log(`✓ Prioridade selecionada: ${chamado.prioridade} (${PRIORIDADE[chamado.prioridade]})`);
  }

  // Preenche SELECT de Status (valor numérico)
  const selectStatus = document.getElementById('status');
  if (selectStatus && chamado.status) {
    selectStatus.value = chamado.status.toString();
    console.log(`✓ Status selecionado: ${chamado.status} (${STATUS[chamado.status]})`);
  }

  // Busca e renderiza contestações (readonly)
  const contestacoes = await buscarContestacoes(chamado.id);
  renderizarContestacoesReadonly(contestacoes);
  
  // Configura campos como readonly
  configurarCamposReadonly();
  
  console.log('✅ Formulário preenchido com sucesso');
}

// ========================================
// RENDERIZAR CONTESTAÇÕES (SOMENTE LEITURA)
// ========================================
function renderizarContestacoesReadonly(contestacoes) {
  console.log('🎨 Renderizando contestações no formulário:', contestacoes);
  
  const contestacaoTextarea = document.getElementById('contestacao');
  
  if (!contestacaoTextarea) {
    console.warn('⚠️ Campo de contestação não encontrado no HTML');
    return;
  }

  // Ajusta altura do textarea baseado na quantidade de contestações
  // Mantemos o ajuste de altura, mas com um mínimo razoável.
  contestacaoTextarea.rows = contestacoes.length > 0 ? Math.min(contestacoes.length * 5 + 2, 20) : 3;
  
  if (contestacoes.length === 0) {
    contestacaoTextarea.value = '📋 Nenhuma contestação registrada para este chamado.';
    return;
  }

  // Formata contestações para exibir APENAS a justificativa
  let texto = '';
  
  contestacoes.forEach((cont, index) => {
    
    // Obtém a justificativa ou um texto padrão
    const justificativa = cont.Justificativa || 'Sem justificativa fornecida';
    
    // Adiciona a justificativa
    texto += justificativa + '\n\n';
  });
  
  contestacaoTextarea.value = texto.trim(); // .trim() para remover espaços extras no final
  console.log('✅ Contestações renderizadas no formulário (apenas justificativas)');
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

  // Mostra loading no botão
  const submitBtn = event.target.querySelector('button[type="submit"]');
  const textoOriginal = submitBtn ? submitBtn.textContent : '';
  if (submitBtn) {
    submitBtn.disabled = true;
    submitBtn.textContent = '⏳ Salvando...';
  }

  try {
    // Coleta APENAS os dados editáveis (selects)
    const categoria = document.getElementById('categoria')?.value;
    const prioridade = document.getElementById('prioridade')?.value;
    const status = document.getElementById('status')?.value;

    // Validação
    if (!categoria || !prioridade || !status) {
      throw new Error('Por favor, preencha todos os campos obrigatórios');
    }

    const dadosAtualizados = {
      categoria: categoria,
      prioridade: parseInt(prioridade),
      status: parseInt(status)
    };

    console.log('💾 Salvando alterações:', dadosAtualizados);

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
      
      // Redireciona para detalhes
      window.location.href = `/detalhes-chamado?id=${chamadoId}`;
    } else {
      throw new Error(data.message || 'Erro ao atualizar chamado');
    }
  } catch (error) {
    console.error('❌ Erro ao salvar:', error);
    alert(`❌ Erro ao salvar alterações: ${error.message}`);
    
    // Restaura botão
    if (submitBtn) {
      submitBtn.disabled = false;
      submitBtn.textContent = textoOriginal;
    }
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
  window.location.href = '/lista-chamados';
}

function configurarBotaoVoltar() {
  const backLink = document.querySelector('.back-link');
  if (backLink) {
    backLink.addEventListener('click', (e) => {
      e.preventDefault();
      
      // Verifica se houve alterações nos selects
      const categoria = document.getElementById('categoria');
      const prioridade = document.getElementById('prioridade');
      const status = document.getElementById('status');
      
      const houveAlteracao = categoria?.dataset.original !== categoria?.value ||
                            prioridade?.dataset.original !== prioridade?.value ||
                            status?.dataset.original !== status?.value;
      
      if (houveAlteracao) {
        if (confirm('Você fez alterações. Deseja sair sem salvar?')) {
          window.location.href ='/chamados';
        }
      } else {
          window.location.href ='/chamados'
      }
    });
  }
}

// ========================================
// GUARDAR VALORES ORIGINAIS
// ========================================
function guardarValoresOriginais() {
  const categoria = document.getElementById('categoria');
  const prioridade = document.getElementById('prioridade');
  const status = document.getElementById('status');
  
  if (categoria) categoria.dataset.original = categoria.value;
  if (prioridade) prioridade.dataset.original = prioridade.value;
  if (status) status.dataset.original = status.value;
  
  console.log('✅ Valores originais guardados para detecção de alterações');
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
    
    // Preenche formulário (incluindo contestações)
    await preencherFormulario(chamado);
    
    // Guarda valores originais para detectar alterações
    guardarValoresOriginais();
    
    // Configura envio do formulário
    const form = document.querySelector('.ticket-form');
    if (form) {
      form.addEventListener('submit', salvarAlteracoes);
      console.log('✓ Event listener de submit configurado');
    } else {
      console.error('❌ Formulário não encontrado');
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