// VizualizarChamados.js - Frontend para lista-chamados.html
console.log('🚀 VizualizarChamados.js (Versão Completa com Filtros e Categoria Corrigida) carregado');

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

// Mapeamento de Categorias (ID -> Nome)
const CATEGORIA = {
  1: 'Hardware',
  2: 'Software',
  3: 'Rede',
  4: 'Financeiro',
  5: 'Outros'
};

// 🌟 NOVO: Mapeamento Reverso (Nome -> ID) para Filtragem
const CATEGORIA_NOME_PARA_ID = {};
// Preenche o mapeamento reverso
for (const [id, nome] of Object.entries(CATEGORIA)) {
    // Garante que o nome esteja em maiúsculas e sem acentos para comparação robusta
    CATEGORIA_NOME_PARA_ID[nome.toUpperCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "")] = id;
}
// Exemplo: CATEGORIA_NOME_PARA_ID['HARDWARE'] = '1'

// ========================================
// CONFIGURAÇÃO DA API
// ========================================
const API_URL = 'http://localhost:3000/api/chamados';

// ========================================
// VARIÁVEIS GLOBAIS
// ========================================
let chamadosCarregados = []; 
let chamadosFiltrados = []; 

// Variáveis de Paginação
let currentPage = 1;
const pageSize = 10; 

// Elementos DOM (Incluindo Filtros)
const chamadosBody = document.getElementById('chamados-body');
const filtroStatus = document.getElementById('filtroStatus');
const filtroPrioridade = document.getElementById('filtroPrioridade');
const filtroCategoria = document.getElementById('filtroCategoria');
const paginationList = document.getElementById('pagination-list');


// ========================================
// FUNÇÕES DE AUTENTICAÇÃO
// ========================================
function obterUsuarioLogado() {
  try {
    const userString = localStorage.getItem('user');
    if (!userString) return null;
    return JSON.parse(userString);
  } catch (error) {
    console.error('❌ Erro ao obter usuário logado:', error);
    return null;
  }
}

// ========================================
// FUNÇÕES AUXILIARES
// ========================================

/**
 * Converte data ISO para formato brasileiro.
 */
function formatarData(dataStr) {
    if (!dataStr) return 'N/A';
    
    const data = new Date(dataStr.split('.')[0]); 
    const dia = String(data.getDate()).padStart(2, '0');
    const mes = String(data.getMonth() + 1).padStart(2, '0');
    const ano = data.getFullYear();
    const hora = String(data.getHours()).padStart(2, '0');
    const minuto = String(data.getMinutes()).padStart(2, '0');
    
    return `${dia}/${mes}/${ano} ${hora}:${minuto}`;
}

/**
 * Cria o HTML para os botões de ação na tabela.
 */
function criarBotoesAcao(idChamado) {
    return `
        <button class="action-button view-button" onclick="window.location.href='/detalhes?id=${idChamado}'" title="Ver Detalhes">
            <i class="fa-solid fa-eye"></i>
        </button>
        <button class="action-button edit-button" onclick="window.location.href='/editar?id=${idChamado}'" title="Editar Chamado">
            <i class="fa-solid fa-pen-to-square"></i>
        </button>
    `;
}

/**
 * Renderiza uma linha da tabela para um chamado, incluindo a Categoria.
 */
function renderizarLinha(chamado) {
    const prioridadeClasse = `priority-${chamado.prioridade}`;
    const statusClasse = `status-${chamado.status}`;
    
    // Obtém o valor da categoria. Se a API retornar o ID, ele usa CATEGORIA[ID]. 
    // Se a API retornar o Nome (string), ele usa o nome diretamente.
    let categoriaValor;
    if (CATEGORIA[chamado.categoria]) {
        // Se a API retornou o ID numérico (ex: 1)
        categoriaValor = CATEGORIA[chamado.categoria];
    } else {
        // Se a API retornou o Nome (string) (ex: 'Hardware')
        categoriaValor = chamado.categoria || 'N/A';
    }

    return `
        <tr>
            <td class="action-cell">${criarBotoesAcao(chamado.id)}</td>
            <td>${chamado.id}</td>
            <td>${chamado.titulo}</td>
            <td>${formatarData(chamado.dataAbertura)}</td>
            <td class="${prioridadeClasse}">${PRIORIDADE[chamado.prioridade] || 'N/A'}</td>
            <td>${categoriaValor}</td> <td class="${statusClasse}">${STATUS[chamado.status] || 'N/A'}</td>
        </tr>
    `;
}


// ========================================
// FUNÇÕES DE PAGINAÇÃO
// ========================================

/**
 * Renderiza os links de paginação baseado no total de chamados filtrados.
 */
function renderizarPaginacao() {
    if (!paginationList) return;

    const totalPaginas = Math.ceil(chamadosFiltrados.length / pageSize);
    let html = '';

    for (let i = 1; i <= totalPaginas; i++) {
        const activeClass = i === currentPage ? 'active' : '';
        html += `
            <li>
                <a href="#" class="pagination-link ${activeClass}" 
                   data-page="${i}" onclick="mudarPagina(${i}); return false;">
                   ${i}
                </a>
            </li>
        `;
    }
    
    paginationList.innerHTML = html;
}

/**
 * Altera a página atual e re-renderiza a tabela.
 */
function mudarPagina(page) {
    const totalPaginas = Math.ceil(chamadosFiltrados.length / pageSize);
    
    if (page < 1 || page > totalPaginas) return;

    currentPage = page;
    renderizarChamados(); 
    renderizarPaginacao();
    window.scrollTo(0, 0); 
}

// ========================================
// FUNÇÕES DE RENDERIZAÇÃO E FILTRO
// ========================================

/**
 * Renderiza a lista de chamados na tabela (função principal de visualização).
 */
function renderizarChamados() {
    if (!chamadosBody) return;

    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    
    const chamadosDaPagina = chamadosFiltrados.slice(start, end);

    if (chamadosDaPagina.length === 0) {
        // COLSPAN é 7
        chamadosBody.innerHTML = `<tr><td colspan="7" style="text-align: center; padding: 30px;">Nenhum chamado encontrado.</td></tr>`;
        return;
    }

    const html = chamadosDaPagina.map(renderizarLinha).join('');
    chamadosBody.innerHTML = html;
}

/**
 * Aplica os filtros de Status, Prioridade e Categoria (REVISADO).
 */
function aplicarFiltros() {
    console.log('🔄 Aplicando filtros...');
    
    const statusSelecionado = filtroStatus ? filtroStatus.value : '';
    const prioridadeSelecionada = filtroPrioridade ? filtroPrioridade.value : '';
    const categoriaSelecionada = filtroCategoria ? filtroCategoria.value : ''; // ID numérico do filtro (ex: '1')
    
    const tempFiltrados = chamadosCarregados.filter(chamado => {
        
        const chamadoStatus = String(chamado.status || '');
        const chamadoPrioridade = String(chamado.prioridade || '');
        
        // 🌟 CORREÇÃO DE CATEGORIA: Tenta converter o valor do chamado para ID numérico
        let idCategoriaDoChamado = '';
        const valorChamadoCategoria = chamado.categoria ? String(chamado.categoria) : '';

        if (!valorChamadoCategoria) {
            // Categoria vazia, id é ''
            idCategoriaDoChamado = '';
        } else if (CATEGORIA[valorChamadoCategoria]) {
            // Caso 1: A API retornou o ID numérico (ex: '1')
            idCategoriaDoChamado = valorChamadoCategoria;
        } else {
            // Caso 2: A API retornou o NOME (ex: 'Hardware'). Faz o reverse lookup.
            const nomeFormatado = valorChamadoCategoria.toUpperCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
            idCategoriaDoChamado = CATEGORIA_NOME_PARA_ID[nomeFormatado] || '';
        }

        // Verifica se o chamado passa pelo filtro de Status
        const filtroStatusPassa = !statusSelecionado || chamadoStatus === statusSelecionado;
        
        // Verifica se o chamado passa pelo filtro de Prioridade
        const filtroPrioridadePassa = !prioridadeSelecionada || chamadoPrioridade === prioridadeSelecionada;

        // Verifica se o chamado passa pelo filtro de Categoria
        const filtroCategoriaPassa = !categoriaSelecionada || idCategoriaDoChamado === categoriaSelecionada; 
        
        return filtroStatusPassa && filtroPrioridadePassa && filtroCategoriaPassa; 
    });

    chamadosFiltrados = tempFiltrados;
    currentPage = 1; 
    
    renderizarChamados();
    renderizarPaginacao();
}

/**
 * Busca a lista completa de chamados na API.
 */
async function carregarChamados() {
    try {
        console.log('📡 Buscando todos os chamados na API...');
        
        const response = await fetch(API_URL);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const data = await response.json();
        
        if (data.success && data.chamados) {
            chamadosCarregados = data.chamados.sort((a, b) => b.id - a.id); 
            console.log(`✅ ${chamadosCarregados.length} chamados carregados.`);
            
            aplicarFiltros(); 

        } else {
            throw new Error(data.mensagem || 'Resposta da API inválida.');
        }

    } catch (error) {
        console.error('❌ Erro ao carregar chamados:', error);
        if (chamadosBody) {
            // COLSPAN é 7
            chamadosBody.innerHTML = `<tr><td colspan="7" style="text-align: center; padding: 30px; color: red;">
                Erro ao carregar chamados: ${error.message}
            </td></tr>`;
        }
    }
}

// ========================================
// CONFIGURAÇÃO
// ========================================

/**
 * Configura os event listeners para os campos de filtro.
 */
function configurarEventListeners() {
    if (filtroStatus) {
        filtroStatus.addEventListener('change', aplicarFiltros);
    }
    if (filtroPrioridade) {
        filtroPrioridade.addEventListener('change', aplicarFiltros);
    }
    if (filtroCategoria) { 
        filtroCategoria.addEventListener('change', aplicarFiltros);
    }
}

/**
 * Configura o botão de voltar.
 */
function configurarBotaoVoltar() {
  const backLink = document.querySelector('.back-link');
  if (backLink) {
    backLink.addEventListener('click', (e) => {
      e.preventDefault();
      window.location.href = '/menu';
    });
  }
}

/**
 * Inicializa a aplicação.
 */
async function inicializar() {
  console.log('🚀 Inicializando lista de chamados');
  
  const usuario = obterUsuarioLogado();
  if (!usuario) {
    console.warn('⚠️ Usuário não está logado');
    alert('Você precisa estar logado para acessar esta página.');
    window.location.href = '/login';
    return;
  }
  
  console.log(`👤 Usuário logado: ${usuario.nome} (${usuario.tipo_usuario})`);
  
  configurarEventListeners();
  configurarBotaoVoltar();
  
  await carregarChamados();
  
  console.log('✅ Sistema inicializado com sucesso');
}

// ========================================
// EXECUÇÃO
// ========================================

if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', inicializar);
} else {
  inicializar();
}

window.mudarPagina = mudarPagina;