/**
 * 📧 SERVIÇO DE E-MAIL
 * Gerencia envio de e-mails via SMTP (Gmail)
 * 
 * RECURSOS:
 * - Envio assíncrono de e-mails
 * - Templates HTML profissionais
 * - Redefinição de senha via e-mail
 * - Notificações automáticas de chamados
 */

const nodemailer = require('nodemailer');
require('dotenv').config();

class EmailService {
    constructor() {
        // Configurações do SMTP (Gmail)
        this.config = {
            smtpServer: process.env.SMTP_SERVER || 'smtp.gmail.com',
            smtpPort: parseInt(process.env.SMTP_PORT) || 587,
            enableSsl: process.env.SMTP_ENABLE_SSL === 'true',
            emailFrom: process.env.EMAIL_FROM || '',
            emailFromName: process.env.EMAIL_FROM_NAME || 'Sistema de Chamados',
            emailUsername: process.env.EMAIL_USERNAME || '',
            emailPassword: process.env.EMAIL_PASSWORD || '',
            emailEnabled: process.env.ENABLE_EMAIL_NOTIFICATIONS === 'true',
            emailAdministrador: process.env.EMAIL_ADMINISTRADOR || process.env.EMAIL_FROM
        };

        // Criar transporter do nodemailer
        this.transporter = this.criarTransporter();
    }

    // ============================================
    // ✅ MÉTODOS PÚBLICOS
    // ============================================

    /**
     * Envia e-mail genérico
     */
    async enviarEmail(destinatario, assunto, corpo, isHtml = true) {
        if (!this.config.emailEnabled) {
            console.warn('⚠️ E-mails desabilitados no .env');
            return false;
        }

        if (!this.config.emailUsername || !this.config.emailPassword) {
            throw new Error('Credenciais de e-mail não configuradas!');
        }

        try {
            const mailOptions = {
                from: `"${this.config.emailFromName}" <${this.config.emailFrom}>`,
                to: destinatario,
                subject: assunto,
                [isHtml ? 'html' : 'text']: corpo
            };

            await this.transporter.sendMail(mailOptions);
            console.log(`✅ E-mail enviado para ${destinatario}`);
            return true;
        } catch (error) {
            console.error(`❌ Erro ao enviar e-mail: ${error.message}`);
            return false;
        }
    }

    // ============================================
    // 📧 E-MAILS ESPECÍFICOS
    // ============================================

    /**
     * Envia solicitação de redefinição de senha para o administrador
     */
    async enviarSolicitacaoRedefinicaoSenha(nomeUsuario, emailUsuario, cpfUsuario) {
        const assunto = `🔐 Solicitação de Redefinição de Senha - ${nomeUsuario}`;
        const corpo = this.gerarTemplateRedefinicaoSenha(nomeUsuario, emailUsuario, cpfUsuario);
        return await this.enviarEmail(this.config.emailAdministrador, assunto, corpo, true);
    }

    /**
     * Envia confirmação de redefinição de senha ao usuário
     */
    async enviarConfirmacaoNovaSenha(destinatario, nomeUsuario, novaSenhaTemporaria) {
        const assunto = '🔑 Sua Senha Foi Redefinida';
        const corpo = this.gerarTemplateConfirmacaoSenha(nomeUsuario, novaSenhaTemporaria);
        return await this.enviarEmail(destinatario, assunto, corpo, true);
    }

    /**
     * Notifica criação de novo chamado
     */
    async notificarNovoChamado(emailTecnico, nomeTecnico, idChamado, categoria, descricao) {
        const assunto = `🆕 Novo Chamado #${idChamado} - ${categoria}`;
        const corpo = this.gerarTemplateNovoChamado(nomeTecnico, idChamado, categoria, descricao);
        return await this.enviarEmail(emailTecnico, assunto, corpo, true);
    }

    /**
     * Notifica resolução de chamado
     */
    async notificarChamadoResolvido(emailUsuario, nomeUsuario, idChamado, categoria) {
        const assunto = `✅ Chamado #${idChamado} Resolvido`;
        const corpo = this.gerarTemplateChamadoResolvido(nomeUsuario, idChamado, categoria);
        return await this.enviarEmail(emailUsuario, assunto, corpo, true);
    }

    /**
     * Notifica mudança de status do chamado
     */
    async notificarMudancaStatus(emailUsuario, nomeUsuario, idChamado, novoStatus) {
        const assunto = `📊 Atualização do Chamado #${idChamado}`;
        const corpo = this.gerarTemplateMudancaStatus(nomeUsuario, idChamado, novoStatus);
        return await this.enviarEmail(emailUsuario, assunto, corpo, true);
    }

    // ============================================
    // 🛠️ MÉTODOS PRIVADOS
    // ============================================

    criarTransporter() {
        return nodemailer.createTransport({
            host: this.config.smtpServer,
            port: this.config.smtpPort,
            secure: this.config.smtpPort === 465, // true para porta 465, false para outras
            auth: {
                user: this.config.emailUsername,
                pass: this.config.emailPassword
            },
            tls: {
                rejectUnauthorized: false
            }
        });
    }

    // ============================================
    // 📄 TEMPLATES HTML
    // ============================================

    gerarTemplateRedefinicaoSenha(nomeUsuario, emailUsuario, cpfUsuario) {
        const agora = new Date().toLocaleString('pt-BR');
        const ano = new Date().getFullYear();

        return `
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }
        .header { background: #007bff; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }
        .content { background: white; padding: 30px; border-radius: 0 0 5px 5px; }
        .info-box { background: #e7f3ff; padding: 15px; margin: 20px 0; border-left: 4px solid #007bff; }
        .footer { text-align: center; padding: 20px; color: #666; font-size: 12px; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Solicitação de Redefinição de Senha</h1>
        </div>
        <div class='content'>
            <h2>Olá, Administrador!</h2>
            <p>Um usuário solicitou a redefinição de senha no sistema.</p>
            
            <div class='info-box'>
                <strong>📋 Informações do Usuário:</strong><br>
                <strong>Nome:</strong> ${nomeUsuario}<br>
                <strong>E-mail:</strong> ${emailUsuario}<br>
                <strong>CPF:</strong> ${cpfUsuario}<br>
                <strong>Data/Hora:</strong> ${agora}
            </div>

            <p><strong>Próximos Passos:</strong></p>
            <ol>
                <li>Verifique a identidade do usuário</li>
                <li>Acesse o sistema como Administrador</li>
                <li>Vá em <strong>Gerenciar Usuários</strong></li>
                <li>Redefina a senha do usuário</li>
            </ol>

            <p style='color: #dc3545; font-weight: bold;'>⚠️ Se você não reconhece esta solicitação, ignore este e-mail.</p>
        </div>
        <div class='footer'>
            <p>Sistema de Chamados InterFix &copy; ${ano}</p>
            <p>Este é um e-mail automático, não responda.</p>
        </div>
    </div>
</body>
</html>`;
    }

    gerarTemplateConfirmacaoSenha(nomeUsuario, novaSenhaTemporaria) {
        return `
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }
        .header { background: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }
        .content { background: white; padding: 30px; border-radius: 0 0 5px 5px; }
        .senha-box { background: #fff3cd; padding: 20px; margin: 20px 0; border: 2px dashed #ffc107; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 2px; }
        .warning { background: #f8d7da; padding: 15px; margin: 20px 0; border-left: 4px solid #dc3545; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔑 Sua Senha Foi Redefinida</h1>
        </div>
        <div class='content'>
            <h2>Olá, ${nomeUsuario}!</h2>
            <p>Sua senha foi redefinida com sucesso pelo administrador.</p>
            
            <p><strong>Sua senha temporária é:</strong></p>
            <div class='senha-box'>
                ${novaSenhaTemporaria}
            </div>

            <div class='warning'>
                <strong>⚠️ IMPORTANTE:</strong>
                <ul style='margin: 10px 0;'>
                    <li>Esta é uma senha <strong>temporária</strong></li>
                    <li>Faça login com esta senha</li>
                    <li><strong>Altere imediatamente</strong> para uma senha segura</li>
                </ul>
            </div>
        </div>
    </div>
</body>
</html>`;
    }

    gerarTemplateNovoChamado(nomeTecnico, idChamado, categoria, descricao) {
        const agora = new Date().toLocaleString('pt-BR');

        return `
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }
        .header { background: #17a2b8; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }
        .content { background: white; padding: 30px; border-radius: 0 0 5px 5px; }
        .chamado-box { background: #d1ecf1; padding: 20px; margin: 20px 0; border-left: 4px solid #17a2b8; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🆕 Novo Chamado Atribuído</h1>
        </div>
        <div class='content'>
            <h2>Olá, ${nomeTecnico}!</h2>
            <p>Um novo chamado foi atribuído a você.</p>
            
            <div class='chamado-box'>
                <strong>📋 Detalhes:</strong><br><br>
                <strong>ID:</strong> #${idChamado}<br>
                <strong>Categoria:</strong> ${categoria}<br>
                <strong>Data:</strong> ${agora}<br><br>
                <strong>Descrição:</strong><br>${descricao}
            </div>
        </div>
    </div>
</body>
</html>`;
    }

    gerarTemplateChamadoResolvido(nomeUsuario, idChamado, categoria) {
        const agora = new Date().toLocaleString('pt-BR');

        return `
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }
        .header { background: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }
        .content { background: white; padding: 30px; border-radius: 0 0 5px 5px; }
        .success-box { background: #d4edda; padding: 20px; margin: 20px 0; border-left: 4px solid #28a745; text-align: center; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Chamado Resolvido</h1>
        </div>
        <div class='content'>
            <h2>Olá, ${nomeUsuario}!</h2>
            
            <div class='success-box'>
                <h3 style='margin: 0; color: #28a745;'>🎉 Seu chamado foi resolvido!</h3>
            </div>

            <p><strong>Informações:</strong></p>
            <ul>
                <li><strong>Chamado:</strong> #${idChamado}</li>
                <li><strong>Categoria:</strong> ${categoria}</li>
                <li><strong>Resolvido em:</strong> ${agora}</li>
            </ul>
        </div>
    </div>
</body>
</html>`;
    }

    gerarTemplateMudancaStatus(nomeUsuario, idChamado, novoStatus) {
        const agora = new Date().toLocaleString('pt-BR');

        return `
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }
        .header { background: #ffc107; color: #333; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }
        .content { background: white; padding: 30px; border-radius: 0 0 5px 5px; }
        .status-box { background: #fff3cd; padding: 15px; margin: 20px 0; border-left: 4px solid #ffc107; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 Atualização de Chamado</h1>
        </div>
        <div class='content'>
            <h2>Olá, ${nomeUsuario}!</h2>
            <p>O status do seu chamado foi atualizado.</p>
            
            <div class='status-box'>
                <strong>📋 Chamado #${idChamado}</strong><br>
                <strong>Novo Status:</strong> ${novoStatus}<br>
                <strong>Atualizado em:</strong> ${agora}
            </div>
        </div>
    </div>
</body>
</html>`;
    }

    // ============================================
    // 🔍 VERIFICAÇÃO
    // ============================================

    estaConfigurado() {
        return this.config.emailEnabled &&
               this.config.emailUsername &&
               this.config.emailPassword &&
               this.config.emailFrom;
    }

    async testarConexao() {
        if (!this.estaConfigurado()) {
            return { sucesso: false, mensagem: 'E-mails não configurados no .env' };
        }

        try {
            await this.transporter.verify();
            return { sucesso: true, mensagem: 'Conexão SMTP OK!' };
        } catch (error) {
            return { sucesso: false, mensagem: `Erro ao testar SMTP: ${error.message}` };
        }
    }
}

module.exports = new EmailService();