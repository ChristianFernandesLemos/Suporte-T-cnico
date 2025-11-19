using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SistemaChamados.Services
{
    /// <summary>
    /// 📧 SERVIÇO DE E-MAIL
    /// Gerencia envio de e-mails via SMTP (Gmail)
    /// 
    /// RECURSOS:
    /// - Envio assíncrono de e-mails
    /// - Templates HTML profissionais
    /// - Redefinição de senha via e-mail
    /// - Notificações automáticas de chamados
    /// - Teste de conexão SMTP
    /// </summary>
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string _emailFrom;
        private readonly string _emailFromName;
        private readonly string _emailUsername;
        private readonly string _emailPassword;
        private readonly bool _emailEnabled;

        public EmailService()
        {
            // Carregar configurações do App.config
            _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _enableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");
            _emailFrom = ConfigurationManager.AppSettings["EmailFrom"] ?? "";
            _emailFromName = ConfigurationManager.AppSettings["EmailFromName"] ?? "Sistema de Chamados";
            _emailUsername = ConfigurationManager.AppSettings["EmailUsername"] ?? "";
            _emailPassword = ConfigurationManager.AppSettings["EmailPassword"] ?? "";
            _emailEnabled = bool.Parse(ConfigurationManager.AppSettings["EnableEmailNotifications"] ?? "false");
        }

        #region ✅ MÉTODOS PÚBLICOS

        /// <summary>
        /// Envia e-mail genérico
        /// </summary>
        public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string corpo, bool isHtml = true)
        {
            if (!_emailEnabled)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ E-mails desabilitados no App.config");
                return false;
            }

            if (string.IsNullOrEmpty(_emailUsername) || string.IsNullOrEmpty(_emailPassword))
            {
                throw new InvalidOperationException("Credenciais de e-mail não configuradas!");
            }

            try
            {
                using (var client = CriarSmtpClient())
                using (var message = CriarMensagem(destinatario, assunto, corpo, isHtml))
                {
                    await client.SendMailAsync(message);
                    System.Diagnostics.Debug.WriteLine($"✅ E-mail enviado para {destinatario}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao enviar e-mail: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Envia e-mail síncrono (para compatibilidade)
        /// </summary>
        public bool EnviarEmail(string destinatario, string assunto, string corpo, bool isHtml = true)
        {
            return EnviarEmailAsync(destinatario, assunto, corpo, isHtml).Result;
        }

        #endregion

        #region 📧 E-MAILS ESPECÍFICOS

        /// <summary>
        /// Envia solicitação de redefinição de senha para o administrador
        /// </summary>
        public async Task<bool> EnviarSolicitacaoRedefinicaoSenhaAsync(string nomeUsuario, string emailUsuario, string cpfUsuario)
        {
            string emailAdmin = ConfigurationManager.AppSettings["EmailAdministrador"] ?? _emailFrom;
            string assunto = $"🔐 Solicitação de Redefinição de Senha - {nomeUsuario}";
            string corpo = GerarTemplateRedefinicaoSenha(nomeUsuario, emailUsuario, cpfUsuario);
            return await EnviarEmailAsync(emailAdmin, assunto, corpo, true);
        }

        /// <summary>
        /// Envia confirmação de redefinição de senha ao usuário
        /// </summary>
        public async Task<bool> EnviarConfirmacaoNovaSenhaAsync(string destinatario, string nomeUsuario, string novaSenhaTemporaria)
        {
            string assunto = "🔑 Sua Senha Foi Redefinida";
            string corpo = GerarTemplateConfirmacaoSenha(nomeUsuario, novaSenhaTemporaria);
            return await EnviarEmailAsync(destinatario, assunto, corpo, true);
        }

        /// <summary>
        /// Notifica criação de novo chamado
        /// </summary>
        public async Task<bool> NotificarNovoChamadoAsync(string emailTecnico, string nomeTecnico, int idChamado, string categoria, string descricao)
        {
            string assunto = $"🆕 Novo Chamado #{idChamado} - {categoria}";
            string corpo = GerarTemplateNovoChamado(nomeTecnico, idChamado, categoria, descricao);
            return await EnviarEmailAsync(emailTecnico, assunto, corpo, true);
        }

        /// <summary>
        /// Notifica resolução de chamado
        /// </summary>
        public async Task<bool> NotificarChamadoResolvidoAsync(string emailUsuario, string nomeUsuario, int idChamado, string categoria)
        {
            string assunto = $"✅ Chamado #{idChamado} Resolvido";
            string corpo = GerarTemplateChamadoResolvido(nomeUsuario, idChamado, categoria);
            return await EnviarEmailAsync(emailUsuario, assunto, corpo, true);
        }

        /// <summary>
        /// Notifica mudança de status do chamado
        /// </summary>
        public async Task<bool> NotificarMudancaStatusAsync(string emailUsuario, string nomeUsuario, int idChamado, string novoStatus)
        {
            string assunto = $"📊 Atualização do Chamado #{idChamado}";
            string corpo = GerarTemplateMudancaStatus(nomeUsuario, idChamado, novoStatus);
            return await EnviarEmailAsync(emailUsuario, assunto, corpo, true);
        }

        #endregion

        #region 🛠️ MÉTODOS PRIVADOS

        private SmtpClient CriarSmtpClient()
        {
            return new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_emailUsername, _emailPassword),
                EnableSsl = _enableSsl,
                Timeout = 10000
            };
        }

        private MailMessage CriarMensagem(string destinatario, string assunto, string corpo, bool isHtml)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailFrom, _emailFromName),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = isHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            message.To.Add(destinatario);
            return message;
        }

        #endregion

        #region 📄 TEMPLATES HTML

        private string GerarTemplateRedefinicaoSenha(string nomeUsuario, string emailUsuario, string cpfUsuario)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }}
        .header {{ background: #007bff; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: white; padding: 30px; border-radius: 0 0 5px 5px; }}
        .info-box {{ background: #e7f3ff; padding: 15px; margin: 20px 0; border-left: 4px solid #007bff; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
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
                <strong>Nome:</strong> {nomeUsuario}<br>
                <strong>E-mail:</strong> {emailUsuario}<br>
                <strong>CPF:</strong> {cpfUsuario}<br>
                <strong>Data/Hora:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}
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
            <p>Sistema de Chamados InterFix &copy; {DateTime.Now.Year}</p>
            <p>Este é um e-mail automático, não responda.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GerarTemplateConfirmacaoSenha(string nomeUsuario, string novaSenhaTemporaria)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }}
        .header {{ background: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: white; padding: 30px; border-radius: 0 0 5px 5px; }}
        .senha-box {{ background: #fff3cd; padding: 20px; margin: 20px 0; border: 2px dashed #ffc107; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 2px; }}
        .warning {{ background: #f8d7da; padding: 15px; margin: 20px 0; border-left: 4px solid #dc3545; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔑 Sua Senha Foi Redefinida</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nomeUsuario}!</h2>
            <p>Sua senha foi redefinida com sucesso pelo administrador.</p>
            
            <p><strong>Sua senha temporária é:</strong></p>
            <div class='senha-box'>
                {novaSenhaTemporaria}
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
</html>";
        }

        private string GerarTemplateNovoChamado(string nomeTecnico, int idChamado, string categoria, string descricao)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }}
        .header {{ background: #17a2b8; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: white; padding: 30px; border-radius: 0 0 5px 5px; }}
        .chamado-box {{ background: #d1ecf1; padding: 20px; margin: 20px 0; border-left: 4px solid #17a2b8; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🆕 Novo Chamado Atribuído</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nomeTecnico}!</h2>
            <p>Um novo chamado foi atribuído a você.</p>
            
            <div class='chamado-box'>
                <strong>📋 Detalhes:</strong><br><br>
                <strong>ID:</strong> #{idChamado}<br>
                <strong>Categoria:</strong> {categoria}<br>
                <strong>Data:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}<br><br>
                <strong>Descrição:</strong><br>{descricao}
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string GerarTemplateChamadoResolvido(string nomeUsuario, int idChamado, string categoria)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }}
        .header {{ background: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: white; padding: 30px; border-radius: 0 0 5px 5px; }}
        .success-box {{ background: #d4edda; padding: 20px; margin: 20px 0; border-left: 4px solid #28a745; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Chamado Resolvido</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nomeUsuario}!</h2>
            
            <div class='success-box'>
                <h3 style='margin: 0; color: #28a745;'>🎉 Seu chamado foi resolvido!</h3>
            </div>

            <p><strong>Informações:</strong></p>
            <ul>
                <li><strong>Chamado:</strong> #{idChamado}</li>
                <li><strong>Categoria:</strong> {categoria}</li>
                <li><strong>Resolvido em:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
            </ul>
        </div>
    </div>
</body>
</html>";
        }

        private string GerarTemplateMudancaStatus(string nomeUsuario, int idChamado, string novoStatus)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background: #f4f4f4; }}
        .header {{ background: #ffc107; color: #333; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: white; padding: 30px; border-radius: 0 0 5px 5px; }}
        .status-box {{ background: #fff3cd; padding: 15px; margin: 20px 0; border-left: 4px solid #ffc107; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 Atualização de Chamado</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nomeUsuario}!</h2>
            <p>O status do seu chamado foi atualizado.</p>
            
            <div class='status-box'>
                <strong>📋 Chamado #{idChamado}</strong><br>
                <strong>Novo Status:</strong> {novoStatus}<br>
                <strong>Atualizado em:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}
            </div>
        </div>
    </div>
</body>
</html>";
        }

        #endregion

        #region 🔍 VERIFICAÇÃO

        public bool EstaConfigurado()
        {
            return _emailEnabled &&
                   !string.IsNullOrEmpty(_emailUsername) &&
                   !string.IsNullOrEmpty(_emailPassword) &&
                   !string.IsNullOrEmpty(_emailFrom);
        }

        public async Task<(bool Sucesso, string Mensagem)> TestarConexaoAsync()
        {
            if (!EstaConfigurado())
                return (false, "E-mails não configurados no App.config");

            try
            {
                using (var client = CriarSmtpClient())
                {
                    var testEmail = new MailMessage
                    {
                        From = new MailAddress(_emailFrom, _emailFromName),
                        Subject = "Teste de Conexão SMTP",
                        Body = "Este é um e-mail de teste.",
                        IsBodyHtml = false
                    };
                    testEmail.To.Add(_emailFrom);
                    await client.SendMailAsync(testEmail);
                    return (true, "Conexão SMTP OK! E-mail de teste enviado.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao testar SMTP: {ex.Message}");
            }
        }

        #endregion
    }
}