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

    const emailService = require('./emailService');
     const db = require('./database'); // Seu módulo de banco de dados

    
    // 1. Seleciona o link
    const linkEsqueciSenha = document.querySelector('.forgot');
    linkEsqueciSenha.addEventListener('click', function(event) {
    event.preventDefault(window.location.assign("/esquecisenha"));
     });
  

 /**
  * 🔐 RECUPERAÇÃO DE SENHA
  * Implementa funcionalidade de recuperação de senha via e-mail
  */

 // ============================================
 // SOLICITAÇÃO DE RECUPERAÇÃO DE SENHA
 // ============================================

 async function solicitarRecuperacaoSenha(cpf, email) {
    try {
        // 1. Validar entrada (Mantido do original)
        if (!cpf || !email) {
            return {
                sucesso: false,
                mensagem: 'CPF e e-mail são obrigatórios!' 
            }; 
        }

        // 2. Buscar usuário no banco de dados (Mantido do original)
        const usuario = await db.buscarUsuarioPorCpf(cpf);
        if (!usuario) {
            return {
                sucesso: false,
                mensagem: 'CPF não encontrado no sistema!'
            }; 
        }

        // 3. Verificar se o e-mail corresponde (Mantido do original)
        if (usuario.email.toLowerCase() !== email.toLowerCase()) {
            return {
                sucesso: false,
                mensagem: 'E-mail não corresponde ao cadastrado!'
            }; 
        }

        // ======================================================
        // 4. NOVA LÓGICA: Integração com n8n
        // ======================================================
        
        const webhookUrl = 'https://n8n.srv993727.hstgr.cloud/webhook/emailsenharecuperar';

        // Envia os dados para o n8n via POST
        const response = await fetch(webhookUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: usuario.email,
                cpf: cpf,
                nome: usuario.nome // Enviando o nome também para personalizar o e-mail no n8n
            })
        });

        // O n8n retorna texto puro: "Enviado" ou "Não enviado"
        const respostaN8N = await response.text();

        // 5. Verificar resposta do n8n
        if (respostaN8N.trim() === 'Enviado') {
            
            // 6. Registrar solicitação no banco (Opcional - mantido do original)
            await db.registrarSolicitacaoRecuperacao(usuario.id); // 

            return {
                sucesso: true,
                mensagem: 'Solicitação enviada! O administrador receberá sua solicitação.' 
            };
        } else {
            // Caso o n8n retorne "Não enviado" ou outra coisa
            console.error('Erro no n8n:', respostaN8N);
            return {
                sucesso: false,
                mensagem: 'Erro ao enviar e-mail via sistema externo. Tente novamente.'
            };
        }

    } catch (error) {
        console.error('Erro ao solicitar recuperação de senha:', error);
        return {
            sucesso: false,
            mensagem: 'Erro interno do servidor. Tente novamente mais tarde.'
        };
    }
}

// ============================================
// GERADOR DE SENHA TEMPORÁRIA
// ============================================

function gerarSenhaTemporaria(tamanho = 8) {
    const caracteres = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let senha = '';
    
    for (let i = 0; i < tamanho; i++) {
        const indice = Math.floor(Math.random() * caracteres.length);
        senha += caracteres.charAt(indice);
    }
    
    return senha;
}

// ============================================
// EXPORTAR FUNÇÕES
// ============================================

module.exports = {
    solicitarRecuperacaoSenha,
    redefinirSenhaUsuario,
    gerarSenhaTemporaria
};
});
