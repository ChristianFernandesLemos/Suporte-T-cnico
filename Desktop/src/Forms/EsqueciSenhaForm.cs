using System;
using System.Windows.Forms;
using SistemaChamados.Helpers;
using SistemaChamados.Controllers;
using SistemaChamados.Data;
using SistemaChamados.Config;
using SistemaChamados.Services;
using SistemaChamados.Models;

namespace SistemaChamados.src.Forms
{
    public partial class EsqueciSenhaForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtCpf;
        private Button btnEnviar;
        private Button btnCancelar;
        private Label lblTitulo;
        private Label lblInstrucao;
        private Label lblEmail;
        private Label lblCpf;

        private FuncionariosController _funcionariosController;
        private EmailService _emailService;

        public EsqueciSenhaForm()
        {
            InitializeComponent();
            InicializarServicos();
            ConfigurarPlaceholders();
        }

        private void InicializarServicos()
        {
            try
            {
                var connectionString = DatabaseConfig.ConnectionString;
                var database = new SqlServerConnection(connectionString);
                _funcionariosController = new FuncionariosController(database);
                _emailService = new EmailService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar serviços: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarPlaceholders()
        {
            txtEmail.SetPlaceholder("Digite seu email cadastrado");
            txtCpf.SetPlaceholder("Digite seu CPF (11 dígitos)");
        }

        private void InitializeComponent()
        {
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtCpf = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblInstrucao = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblCpf = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(90, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(210, 24);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🔐 Recuperar Senha";

            // lblInstrucao
            this.lblInstrucao.Location = new System.Drawing.Point(30, 60);
            this.lblInstrucao.Name = "lblInstrucao";
            this.lblInstrucao.Size = new System.Drawing.Size(340, 50);
            this.lblInstrucao.TabIndex = 1;
            this.lblInstrucao.Text = "Informe seu email e CPF cadastrados.\n" +
                "O administrador receberá uma solicitação para redefinir sua senha.";
            this.lblInstrucao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(30, 125);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(35, 13);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "Email:";

            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(30, 145);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(340, 20);
            this.txtEmail.TabIndex = 3;

            // lblCpf
            this.lblCpf.AutoSize = true;
            this.lblCpf.Location = new System.Drawing.Point(30, 180);
            this.lblCpf.Name = "lblCpf";
            this.lblCpf.Size = new System.Drawing.Size(30, 13);
            this.lblCpf.TabIndex = 4;
            this.lblCpf.Text = "CPF:";

            // txtCpf
            this.txtCpf.Location = new System.Drawing.Point(30, 200);
            this.txtCpf.Name = "txtCpf";
            this.txtCpf.Size = new System.Drawing.Size(200, 20);
            this.txtCpf.TabIndex = 5;
            this.txtCpf.MaxLength = 11;

            // btnEnviar
            this.btnEnviar.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnEnviar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviar.ForeColor = System.Drawing.Color.White;
            this.btnEnviar.Location = new System.Drawing.Point(30, 245);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(160, 35);
            this.btnEnviar.TabIndex = 6;
            this.btnEnviar.Text = "📧 Enviar Solicitação";
            this.btnEnviar.UseVisualStyleBackColor = false;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

            // btnCancelar
            this.btnCancelar.BackColor = System.Drawing.Color.Gray;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(210, 245);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(160, 35);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // EsqueciSenhaForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 310);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtCpf);
            this.Controls.Add(this.lblCpf);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblInstrucao);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EsqueciSenhaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recuperar Senha - InterFix";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validar campos
                string email = txtEmail.GetText().Trim();
                string cpf = txtCpf.GetText().Trim().Replace(".", "").Replace("-", "");

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Por favor, informe o email.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(cpf))
                {
                    MessageBox.Show("Por favor, informe o CPF.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCpf.Focus();
                    return;
                }

                if (cpf.Length != 11)
                {
                    MessageBox.Show("CPF deve conter 11 dígitos.", "CPF Inválido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCpf.Focus();
                    return;
                }

                // Desabilitar botão
                btnEnviar.Enabled = false;
                btnEnviar.Text = "⏳ Enviando...";
                Application.DoEvents();

                // 2. Verificar se email está configurado
                if (!_emailService.EstaConfigurado())
                {
                    MessageBox.Show(
                        "⚠️ Serviço de email não está configurado!\n\n" +
                        "Configure o Gmail no App.config:\n" +
                        "- EmailFrom\n" +
                        "- EmailUsername\n" +
                        "- EmailPassword (senha de app)\n" +
                        "- EmailAdministrador",
                        "Email Não Configurado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 3. Buscar usuário no banco
                var usuario = _funcionariosController.ListarTodosFuncionarios()
                    .Find(f => f.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                            && f.Cpf == cpf);

                if (usuario == null)
                {
                    // Por segurança, não informar se usuário existe ou não
                    MessageBox.Show(
                        "✅ Se os dados informados estiverem corretos, " +
                        "o administrador receberá sua solicitação em breve.",
                        "Solicitação Enviada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    System.Diagnostics.Debug.WriteLine($"❌ Usuário não encontrado: {email} / CPF: {cpf}");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // 4. Enviar email para o administrador
                bool emailEnviado = await _emailService.EnviarSolicitacaoRedefinicaoSenhaAsync(
                    usuario.Nome,
                    usuario.Email,
                    usuario.Cpf
                );

                if (emailEnviado)
                {
                    MessageBox.Show(
                        "✅ Solicitação enviada com sucesso!\n\n" +
                        $"Olá {usuario.Nome},\n\n" +
                        "O administrador foi notificado e entrará em contato " +
                        "para redefinir sua senha.\n\n" +
                        "Aguarde o email de confirmação.",
                        "Solicitação Enviada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    System.Diagnostics.Debug.WriteLine($"✅ Email enviado para admin sobre: {usuario.Nome}");

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "❌ Erro ao enviar email!\n\n" +
                        "Possíveis causas:\n" +
                        "- Configuração incorreta do Gmail\n" +
                        "- Senha de app inválida\n" +
                        "- Conexão com internet\n\n" +
                        "Contate o administrador diretamente.",
                        "Erro no Envio",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar solicitação: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                btnEnviar.Enabled = true;
                btnEnviar.Text = "📧 Enviar Solicitação";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}