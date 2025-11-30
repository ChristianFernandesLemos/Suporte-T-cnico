// No arquivo: recuperarController.js

const { getConnection } = require('../../db');
// Fetch já está disponível globalmente no Node.js 18+

/**
 * Solicita recuperação de senha e envia e-mail ao administrador via n8n
 * @param {string} cpf - CPF do usuário (pode conter formatação)
 * @param {string} email - E-mail do usuário
 * @returns {Object} - { sucesso: boolean, mensagem: string }
 */
async function solicitarRecuperacaoSenha(cpf, email) {
    try {
        console.log('🔐 Iniciando processo de recuperação de senha...');
        
        // 1. Validar entrada
        if (!cpf || !email) {
            console.log('❌ Validação falhou: dados ausentes');
            return {
                sucesso: false,
                mensagem: 'CPF e e-mail são obrigatórios!'
            };
        }

        // 2. Limpar CPF (remover pontos e traços)
        const cpfLimpo = cpf.replace(/[.\-]/g, '');
        console.log('✅ CPF limpo:', cpfLimpo);

        // 3. Validar formato do CPF
        if (cpfLimpo.length !== 11 || !/^\d+$/.test(cpfLimpo)) {
            console.log('❌ CPF inválido no Controller:', cpfLimpo);
            return {
                sucesso: false,
                mensagem: 'CPF inválido!'
            };
        }

        // 4. Buscar usuário no banco de dados com JOIN
        console.log('🔍 Buscando usuário no banco (com JOIN e dbo.)...');
        const pool = await getConnection();
        
        // 🚨 CORREÇÃO SQL APLICADA: 
        // Usa nomes de tabela corretos (dbo.Usuario e dbo.E_mail) e o JOIN.
        const result = await pool.request()
            .input('cpf', cpfLimpo)
            .query(`
                SELECT 
                    u.Id_usuario AS id, 
                    u.nome, 
                    u.Cpf AS cpf,
                    e.E_mail AS email
                FROM 
                    dbo.Usuario u  
                INNER JOIN 
                    dbo.E_mail e ON u.Id_usuario = e.Id_usuario
                WHERE 
                    u.Cpf = @cpf
            `);
        // 🚨 FIM DA CORREÇÃO

        if (result.recordset.length === 0) {
            console.log('❌ Usuário não encontrado com CPF:', cpfLimpo);
            return {
                sucesso: false,
                mensagem: 'CPF não encontrado no sistema!'
            };
        }

        const usuario = result.recordset[0];
        console.log('✅ Usuário encontrado:', { id: usuario.id, nome: usuario.nome, email: usuario.email });

        // 5. Verificar se o e-mail corresponde
        if (usuario.email.toLowerCase() !== email.toLowerCase()) {
            console.log('❌ E-mail não corresponde. Esperado:', usuario.email, 'Recebido:', email);
            return {
                sucesso: false,
                mensagem: 'E-mail não corresponde ao cadastrado!'
            };
        }

        console.log('✅ E-mail validado com sucesso');

        // 6. Preparar dados para envio ao n8n
        const webhookUrl = 'https://n8n.srv993727.hstgr.cloud/webhook/emailsenharecuperar';
        const dadosEnvio = {
            email: usuario.email,
            cpf: cpfLimpo,
            nome: usuario.nome,
            dataSolicitacao: new Date().toISOString()
        };

        console.log('📨 Enviando para n8n webhook:', webhookUrl);

        // 7. Enviar para n8n
        const response = await fetch(webhookUrl, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(dadosEnvio)
        });

        console.log('📡 Status da resposta n8n:', response.status, response.statusText);

        // 8. Processar resposta
        const respostaN8N = await response.text();
        console.log('📬 Resposta do n8n:', respostaN8N);

        // 9. Verificar se foi enviado com sucesso
        // Simplificado para checar apenas o status HTTP
        if (response.ok) { 
            console.log('✅ E-mail enviado com sucesso via n8n');

            // 10. Registrar solicitação no banco (opcional)
            try {
                // Adicionando 'dbo.' à tabela de registro por segurança
                await pool.request()
                    .input('usuarioId', usuario.id)
                    .input('dataHora', new Date())
                    .query(`
                        INSERT INTO dbo.solicitacoes_recuperacao (usuario_id, data_solicitacao)
                        VALUES (@usuarioId, @dataHora)
                    `);
                console.log('✅ Solicitação registrada no banco');
            } catch (dbError) {
                console.log('⚠️ Aviso: Não foi possível registrar no banco:', dbError.message);
            }

            return {
                sucesso: true,
                mensagem: 'Solicitação enviada! O administrador receberá sua solicitação em breve.'
            };
        } else {
            console.error('❌ Falha no envio do e-mail via n8n');
            return {
                sucesso: false,
                mensagem: 'Erro ao enviar e-mail. Tente novamente mais tarde.'
            };
        }

    } catch (error) {
        console.error('❌ ERRO CRÍTICO ao solicitar recuperação de senha:', error);
        console.error('Stack:', error.stack);
        
        return {
            sucesso: false,
            mensagem: 'Erro interno do servidor. Tente novamente mais tarde.'
        };
    }
}

module.exports = {
    solicitarRecuperacaoSenha
};