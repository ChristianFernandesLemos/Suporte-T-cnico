// Script para a página de login

document.addEventListener('DOMContentLoaded', function() {
    const form = document.querySelector('.login-box form') || document.querySelector('form');
    const emailInput = document.querySelector('input[type="email"]');
    const passwordInput = document.querySelector('input[type="password"]');
    const submitButton = document.querySelector('button[type="submit"]') || document.querySelector('button');

    // Previne envio padrão do formulário
    if (form) {
        form.addEventListener('submit', async function(e) {
            e.preventDefault();
            await handleLogin();
        });
    }

    // Função de login
    async function handleLogin() {
        const email = emailInput.value.trim();
        const senha = passwordInput.value;

        // Validações básicas
        if (!email) {
            showMessage('Por favor, digite seu e-mail.', 'error');
            emailInput.focus();
            return;
        }

        if (!senha) {
            showMessage('Por favor, digite sua senha.', 'error');
            passwordInput.focus();
            return;
        }

        // Desabilita botão durante requisição
        submitButton.disabled = true;
        submitButton.textContent = 'Entrando...';

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, senha })
            });

            const data = await response.json();

            if (data.success) {
                // Salva token e dados do usuário
                localStorage.setItem('token', data.token);
                localStorage.setItem('user', JSON.stringify(data.user));

                showMessage('Login realizado com sucesso!', 'success');

                // Redireciona para o menu principal
                setTimeout(() => {
                    window.location.href = '/menu';
                }, 1000);
            } else {
                showMessage(data.message || 'Erro ao realizar login.', 'error');
                submitButton.disabled = false;
                submitButton.textContent = 'Entrar';
            }
        } catch (error) {
            console.error('Erro no login:', error);
            showMessage('Erro ao conectar com o servidor. Tente novamente.', 'error');
            submitButton.disabled = false;
            submitButton.textContent = 'Entrar';
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

        // Estilos inline
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
            ${type === 'success' ? 'background-color: #4CAF50;' : 'background-color: #f44336;'}
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

    // Esqueci a senha
    const forgotLink = document.querySelector('.forgot');
    if (forgotLink) {
        forgotLink.addEventListener('click', async function(e) {
            e.preventDefault();
            
            const email = emailInput.value.trim();
            
            if (!email) {
                showMessage('Digite seu e-mail para recuperar a senha.', 'error');
                emailInput.focus();
                return;
            }

            try {
                const response = await fetch('/api/auth/forgot-password', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ email })
                });

                const data = await response.json();
                showMessage(data.message, 'success');
            } catch (error) {
                console.error('Erro:', error);
                showMessage('Erro ao processar solicitação.', 'error');
            }
        });
    }
});
