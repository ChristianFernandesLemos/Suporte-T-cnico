// Script para o Menu Principal

document.addEventListener('DOMContentLoaded', function() {
    // Verifica autenticação
    checkAuth();

    // Carrega informações do usuário
    loadUserInfo();

    // Configura menu de navegação
    setupNavigation();

    // Adiciona botão de logout
    addLogoutButton();

    // Adiciona estilos de animação se ainda não estiverem presentes
    addAnimationStyles();
});

// Verifica se usuário está autenticado
async function checkAuth() {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user'); // Adiciona verificação dos dados do usuário
    
    // Se não há token OU não há dados do usuário, redireciona imediatamente
    if (!token || !user) {
        redirectToLogin();
        return;
    }

    try {
        const response = await fetch('/api/auth/verify', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        // Se a resposta não for OK (ex: 401 Unauthorized), redireciona
        if (!response.ok) {
            console.warn('Verificação de token falhou no backend. Redirecionando...');
            redirectToLogin();
        }
    } catch (error) {
        // Erro de rede ou servidor
        console.error('Erro ao verificar autenticação (Falha de Rede/Servidor):', error);
        redirectToLogin();
    }
}

// Redireciona para login
function redirectToLogin() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
}

// Carrega informações do usuário
function loadUserInfo() {
    const userStr = localStorage.getItem('user');
    
    if (!userStr) {
        redirectToLogin();
        return;
    }

    const user = JSON.parse(userStr);

    // Atualiza nome do usuário no menu
    const userNameElements = document.querySelectorAll('.user-name');
    userNameElements.forEach(el => {
        el.textContent = user.nome;
    });

    // Atualiza saudação
    const welcomeElement = document.querySelector('.welcome-section h2');
    if (welcomeElement) {
        welcomeElement.textContent = `Bem-vindo, ${user.nome}!`;
    }

    // Mostra/esconde opções baseado no tipo de usuário
    if (user.tipo_usuario !== 'admin') {
        // Esconde opção de gerenciar acessos para não-admins
        const gerenciarAcessosLink = Array.from(document.querySelectorAll('.menu a'))
            .find(a => a.textContent.includes('Gerenciar Acessos'));
        
        if (gerenciarAcessosLink) {
            gerenciarAcessosLink.parentElement.style.display = 'none';
        }
    }
}

// Configura navegação do menu
function setupNavigation() {
    const menuLinks = document.querySelectorAll('.menu a');

    menuLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const text = this.textContent.trim();

            if (text === 'Documentação') {
                showDocumentation(); // Chama a função para exibir o manual na página
                return;
            }

            // Mapeia links para rotas
            const routes = {
                'Registrar Chamado': '/registrar-chamado',
                'Visualizar Chamados': '/chamados',
                // 'Ver Relatórios' foi removido
            };

            const route = routes[text];
            
            if (route) {
                window.location.href = route;
            } else if (text !== 'Ver Relatórios' && text !== 'Gerenciar Acessos') { 
                 showMessage('Página em desenvolvimento.', 'info');
            }
        });
    });
}

/**
 * Retorna o conteúdo estruturado do Manual do Usuário, adaptado para a versão web.
 */
function getManualContent(userType) {
    const isAdmin = userType === 'admin';
    const isTechOrAdmin = userType === 'tecnico' || userType === 'admin';
    const isFuncOrAdmin = userType === 'funcionario' || userType === 'admin';

    const nivelAcessoTexto = {
        'funcionario': 'Funcionário',
        'tecnico': 'Técnico',
        'admin': 'Administrador'
    }[userType] || 'Desconhecido';

    const funcionalidadesDisponiveis = (() => {
        if (userType === 'funcionario') {
            return "• Criar novos chamados\n• Visualizar seus chamados\n• Adicionar contestações";
        } else if (userType === 'tecnico') {
            return "• Visualizar todos os chamados\n• Gerenciar chamados (Atribuir, Alterar Prioridade, Resolver)";
        } else if (userType === 'admin') {
            return "• Criar novos chamados\n• Visualizar todos os chamados\n• Gerenciar chamados\n• Acesso completo às funcionalidades web";
        }
        return "Nenhuma funcionalidade disponível";
    })();

    const tabs = [];

    // 1. Aba: Primeiros Passos (TODOS)
    tabs.push({
        title: "🚀 Início",
        content: `BEM-VINDO AO SISTEMA INTERFIX!

Este é o Sistema de Gerenciamento de Chamados da InterFix (Versão Web).

📋 O QUE É O SISTEMA?
O sistema permite que você registre e acompanhe problemas técnicos, solicitações de suporte e manutenções.

👤 SEU NÍVEL DE ACESSO: ${nivelAcessoTexto}

🎯 FUNCIONALIDADES DISPONÍVEIS:
${funcionalidadesDisponiveis}

💡 DICA:
Use o menu à esquerda para navegar entre as diferentes seções do sistema. O acesso ao manual se dá pela opção **Documentação**.`
    });

    // 2. Aba: Como Criar Chamado (Funcionário e Admin)
    if (isFuncOrAdmin) {
        tabs.push({
            title: "➕ Criar Chamado",
            content: `COMO CRIAR UM NOVO CHAMADO

📝 PASSO A PASSO:

1. ACESSAR CRIAÇÃO
   • Clique em 'Registrar Chamado' no menu lateral.

2. PREENCHIMENTO DO FORMULÁRIO
   • **Título**: Digite um título claro (ex: "Impressora não funciona").
   • **Categoria**: Selecione Hardware, Software, Rede ou Outros.
   • **Descrição**: Descreva detalhadamente o problema (mínimo 20 caracteres).

3. IMPACTO E CONFIRMAÇÃO
   • Preencha as informações sobre **Quem é Afetado** e **Impacto no Trabalho**.
   • Revise as informações e clique em 'Concluir'.
   • Anote o número do chamado gerado.

📊 PRIORIDADES (calculadas automaticamente):
• Baixa: Não impede trabalho, afeta só você
• Média: Não impede, mas afeta departamento
• Alta: Impede trabalho do departamento
• Crítica: Impede trabalho da empresa toda

✅ DEPOIS DE CRIAR:
• Você receberá um número de protocolo.
• Pode acompanhar o status em 'Visualizar Chamados'.`
        });
    }

    // 3. Aba: Gerenciar Chamados (Técnico e Admin)
    if (isTechOrAdmin) {
        tabs.push({
            title: "⚙️ Gerenciar",
            content: `GERENCIAMENTO DE CHAMADOS (TÉCNICO/ADMIN)

⚙️ FUNCIONALIDADES:

1. VISUALIZAR E FILTRAR CHAMADOS
   • Acesse 'Visualizar Chamados' no menu.
   • Use os filtros para buscar por status, prioridade ou técnico (se for Admin).
   • Busque por palavras-chave na lista de chamados.

2. ATRIBUIR, ALTERAR PRIORIDADE E RESOLVER
   • **Atribuir Técnico**: Selecione o chamado, escolha o técnico (para Admin/Gerente) e o status muda para 'Em Andamento'.
   • **Alterar Prioridade**: Altere a prioridade (Baixa, Média, Alta ou Crítica) conforme a necessidade do negócio.
   • **Resolver Chamado**: Adicione a solução aplicada e marque o status como 'Resolvido'.

3. FECHAR CHAMADO
   • Após a solução e confirmação do solicitante, clique em 'Fechar Chamado'.
   • Status muda para 'Fechado'.

📋 STATUS DOS CHAMADOS:
• Aberto: Aguardando atribuição
• Em Andamento: Técnico está trabalhando
• Resolvido: Problema foi solucionado
• Fechado: Chamado finalizado
• Cancelado: Chamado foi cancelado

🎯 BOAS PRÁTICAS:
• Sempre adicione comentários ao realizar ações.
• Mantenha os chamados atualizados e priorize os críticos.
• Comunique-se com o solicitante.`
        });
    }

    // 4. Aba: FAQ (TODOS)
    tabs.push({
        title: "❓ Perguntas Frequentes",
        content: `PERGUNTAS FREQUENTES (FAQ)

❓ COMO FAÇO LOGIN?
Use seu e-mail corporativo e a senha fornecida pelo administrador.

❓ ESQUECI MINHA SENHA
Entre em contato com o administrador do sistema.

❓ QUANTO TEMPO LEVA PARA RESOLVER UM CHAMADO?
Depende da prioridade:
• Crítica: Até 4 horas
• Alta: Até 1 dia útil
• Média: Até 3 dias úteis
• Baixa: Até 1 semana

❓ POSSO CANCELAR UM CHAMADO?
Sim, entre em contato com o técnico responsável ou administrador.

❓ COMO ACOMPANHO MEU CHAMADO?
Acesse 'Visualizar Chamados' na barra lateral.

❓ POSSO CRIAR CHAMADO PARA OUTRA PESSOA?
Não, cada usuário deve criar seus próprios chamados.

❓ O QUE FAZER SE O PROBLEMA PERSISTIR?
Adicione uma contestação ao chamado ou crie um novo chamado relacionado.

❓ POSSO VER CHAMADOS DE OUTRAS PESSOAS?
• Funcionário: Não, apenas seus próprios
• Técnico/Admin: Sim, todos os chamados.

📞 SUPORTE TÉCNICO:
• E-mail: interfix87@gmail.com
• Telefone: (12) 99164-1425
• Horário: Segunda a Sexta, 8h às 18h`
    });

    return tabs;
}

function showDocumentation() {
    const userStr = localStorage.getItem('user');
    const user = userStr ? JSON.parse(userStr) : { tipo_usuario: 'funcionario' }; // Default para 'funcionario' se não logado
    const manualTabs = getManualContent(user.tipo_usuario);
    const mainContent = document.querySelector('.main-content');
    
    // Limpa conteúdo principal
    mainContent.innerHTML = '';

    // Cria a estrutura do manual
    const docContainer = document.createElement('div');
    docContainer.className = 'documentation-container';
    docContainer.innerHTML = `
        <h2 style="color: #007bff; margin-bottom: 20px;">📖 Manual do Usuário - Versão Web</h2>
        <div class="tabs-control"></div>
        <div class="tabs-content"></div>
    `;

    const tabsControl = docContainer.querySelector('.tabs-control');
    const tabsContent = docContainer.querySelector('.tabs-content');

    manualTabs.forEach((tab, index) => {
        // Cria botão/link da aba
        const tabLink = document.createElement('button');
        tabLink.textContent = tab.title;
        tabLink.className = 'tab-link';
        tabLink.dataset.tab = `tab-${index}`;
        if (index === 0) tabLink.classList.add('active'); // Ativa a primeira aba

        // Cria o painel de conteúdo da aba
        const tabPanel = document.createElement('div');
        tabPanel.className = 'tab-panel';
        tabPanel.id = `tab-${index}`;
        tabPanel.style.display = index === 0 ? 'block' : 'none';
        
        // Substitui quebras de linha '\n' por <br> e formata para exibição
        const formattedContent = tab.content.replace(/\n/g, '<br>').replace(/•/g, '<strong>•</strong>').replace(/❓/g, '<strong>❓</strong>').replace(/📋/g, '<strong>📋</strong>');

        tabPanel.innerHTML = `<p style="white-space: pre-wrap; font-size: 14px; line-height: 1.6;">${formattedContent}</p>`;


        tabsControl.appendChild(tabLink);
        tabsContent.appendChild(tabPanel);
    });

    // Adiciona o container ao conteúdo principal
    mainContent.appendChild(docContainer);

    // Adiciona a lógica de troca de abas
    tabsControl.addEventListener('click', function(e) {
        if (e.target.classList.contains('tab-link')) {
            const tabId = e.target.dataset.tab;

            // Remove 'active' de todos os links e esconde todos os painéis
            docContainer.querySelectorAll('.tab-link').forEach(link => link.classList.remove('active'));
            docContainer.querySelectorAll('.tab-panel').forEach(panel => panel.style.display = 'none');

            // Ativa o link e mostra o painel correspondente
            e.target.classList.add('active');
            document.getElementById(tabId).style.display = 'block';
        }
    });

    // Adiciona estilos básicos para o manual (pode ser movido para style.css)
    const style = document.createElement('style');
    style.textContent = `
        .documentation-container {
            padding: 40px;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }
        .tabs-control {
            border-bottom: 1px solid #ccc;
            margin-bottom: 20px;
        }
        .tab-link {
            background: #f4f4f4;
            border: 1px solid #ccc;
            border-bottom: none;
            padding: 10px 15px;
            cursor: pointer;
            margin-right: 5px;
            border-radius: 5px 5px 0 0;
            font-weight: 600;
            color: #333;
            transition: all 0.2s;
        }
        .tab-link.active {
            background: #ffffff;
            border-color: #007bff;
            border-top: 3px solid #007bff;
            color: #007bff;
            margin-top: -3px;
        }
        .tab-panel {
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 0 5px 5px 5px;
            min-height: 400px;
        }
    `;
    mainContent.appendChild(style);
}

// Adiciona botão de logout
function addLogoutButton() {
    const userInfo = document.querySelector('.user-info');
    
    if (!userInfo) return;

    // Cria botão de logout
    const logoutBtn = document.createElement('button');
    logoutBtn.textContent = 'Sair';
    logoutBtn.className = 'logout-btn';
    
    // Estilos do botão de logout
    logoutBtn.style.cssText = `
        width: 100%;
        padding: 10px;
        margin-top: 15px;
        background-color: #dc3545;
        color: white;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        font-weight: bold;
        transition: background-color 0.3s;
    `;

    // Adiciona os eventos de mouse (hover)
    logoutBtn.addEventListener('mouseenter', function() {
        this.style.backgroundColor = '#c82333';
    });

    logoutBtn.addEventListener('mouseleave', function() {
        this.style.backgroundColor = '#dc3545';
    });


    logoutBtn.addEventListener('click', handleLogout);

    userInfo.appendChild(logoutBtn);
}

// Função de logout simplificada
async function handleLogout() {
    // Confirmação antes de sair
    if (!confirm('Deseja realmente sair?')) {
        return; 
    }

    // Tenta fazer o logout no servidor
    const token = localStorage.getItem('token');
    try {
        await fetch('/api/auth/logout', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
    } catch (error) {
        console.error('Erro no logout (apenas informativo):', error);
    } finally {
        // Redireciona de forma direta, limpando os dados locais
        redirectToLogin();
    }
}

// Função para exibir mensagens
function showMessage(message, type) {
    // Remove mensagem anterior se existir
    const oldMessage = document.querySelector('.message-box');
    if (oldMessage) {
        oldMessage.remove();
    }

    // Cria elemento de mensagem
    const messageBox = document.createElement('div');
    messageBox.className = `message-box message-${type}`;
    messageBox.textContent = message;

    // Estilos baseados no tipo
    const colors = {
        success: '#4CAF50',
        error: '#f44336',
        info: '#2196F3',
        warning: '#ff9800'
    };

    messageBox.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 5px;
        color: white;
        font-weight: bold;
        z-index: 9999;
        animation: slideIn 0.3s ease-out;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        background-color: ${colors[type] || colors.info};
    `;

    document.body.appendChild(messageBox);

    // Remove mensagem após 3 segundos
    setTimeout(() => {
        messageBox.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => messageBox.remove(), 300);
    }, 3000);
}

// Adiciona estilos de animação (garante que showMessage funcione)
function addAnimationStyles() {
    if (!document.querySelector('style[data-animation="interfix"]')) {
        const style = document.createElement('style');
        style.setAttribute('data-animation', 'interfix');
        style.textContent = `
            @keyframes slideIn {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
            @keyframes slideOut {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(100%);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
}