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
});

// Verifica se usuário está autenticado
async function checkAuth() {
    const token = localStorage.getItem('token');

    if (!token) {
        redirectToLogin();
        return;
    }

    try {
        const response = await fetch('/api/auth/verify', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            redirectToLogin();
        }
    } catch (error) {
        console.error('Erro ao verificar autenticação:', error);
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

            // Mapeia links para rotas
            const routes = {
                'Registrar Chamado': '/registrar-chamado',
                'Visualizar Chamados': '/chamados',
                'Gerenciar Acessos': '/adicionar-usuario',
                'Ver Relatórios': '/relatorios',
                'Documentação': '/documentacao'
            };

            const route = routes[text];
            
            if (route) {
                window.location.href = route;
            } else {
                showMessage('Página em desenvolvimento.', 'info');
            }
        });
    });
}

// Adiciona botão de logout
function addLogoutButton() {
    const userInfo = document.querySelector('.user-info');
    
    if (!userInfo) return;

    // Cria botão de logout
    const logoutBtn = document.createElement('button');
    logoutBtn.textContent = 'Sair';
    logoutBtn.className = 'logout-btn';
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

    logoutBtn.addEventListener('mouseenter', function() {
        this.style.backgroundColor = '#c82333';
    });

    logoutBtn.addEventListener('mouseleave', function() {
        this.style.backgroundColor = '#dc3545';
    });

    logoutBtn.addEventListener('click', handleLogout);

    userInfo.appendChild(logoutBtn);
}

// Função de logout
async function handleLogout() {
    const token = localStorage.getItem('token');

    try {
        await fetch('/api/auth/logout', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
    } catch (error) {
        console.error('Erro no logout:', error);
    } finally {
        // Limpa dados locais
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        
        // Redireciona para login
        window.location.href = '/login';
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

// Adiciona estilos de animação
const style = document.createElement('style');
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
