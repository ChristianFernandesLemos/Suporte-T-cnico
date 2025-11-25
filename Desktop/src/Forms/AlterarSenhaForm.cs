using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Helpers;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace SistemaChamados.Forms
{
    public partial class AlterarSenhaForm : Form
    {
        private Funcionarios _usuario;
        private Funcionarios _adminLogado;
        private FuncionariosController _funcionariosController;

        private TextBox txtNovaSenha;
        private TextBox txtConfirmarSenha;
        private CheckBox chkMostrarSenha;
        private Button btnSalvar;
        private Button btnCancelar;
        private Button btnGerarSenha;
        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblAviso;

        /// <summary>
        /// Constructor ÚNICO - Só o admin puede usar este formulário
        /// </summary>
        public AlterarSenhaForm(Funcionarios usuario, Funcionarios adminLogado, FuncionariosController funcionariosController)
        {
            // Validar que quien abre el formulario es admin
            if (adminLogado == null || adminLogado.NivelAcesso != 3)
            {
                throw new UnauthorizedAccessException("Apenas administradores podem redefinir senhas!");
            }

            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            _adminLogado = adminLogado;
            _funcionariosController = funcionariosController ?? throw new ArgumentNullException(nameof(funcionariosController));

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.lblUsuario = new Label();
            this.lblAviso = new Label();
            this.txtNovaSenha = new TextBox();
            this.txtConfirmarSenha = new TextBox();
            this.chkMostrarSenha = new CheckBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();
            this.btnGerarSenha = new Button();

            // lblTitulo
            this.lblTitulo.Text = "🔐 Redefinir Senha de Usuário";
            this.lblTitulo.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 15);
            this.lblTitulo.Size = new Size(360, 25);
            this.lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            // lblUsuario - Informações do usuário
            this.lblUsuario.Location = new Point(12, 50);
            this.lblUsuario.Size = new Size(360, 60);
            this.lblUsuario.BackColor = Color.FromArgb(230, 240, 250);
            this.lblUsuario.Padding = new Padding(10);
            this.lblUsuario.BorderStyle = BorderStyle.FixedSingle;
            this.lblUsuario.Text = $"👤 Usuário: {_usuario.Nome}\n" +
                                  $"📧 Email: {_usuario.Email}\n" +
                                  $"🆔 CPF: {_usuario.Cpf}";

            // lblAviso - Aviso importante
            this.lblAviso.Location = new Point(12, 120);
            this.lblAviso.Size = new Size(360, 45);
            this.lblAviso.BackColor = Color.FromArgb(255, 243, 205);
            this.lblAviso.Padding = new Padding(8);
            this.lblAviso.BorderStyle = BorderStyle.FixedSingle;
            this.lblAviso.ForeColor = Color.FromArgb(133, 100, 4);
            this.lblAviso.Text = "⚠️ A nova senha NÃO será enviada por email.\n" +
                                "Você deve informar ao usuário pessoalmente.";
            this.lblAviso.Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Regular);

            // Nova Senha
            var lblNova = new Label
            {
                Text = "Nova Senha:",
                Location = new Point(12, 180),
                Size = new Size(80, 15)
            };
            this.txtNovaSenha.Location = new Point(12, 200);
            this.txtNovaSenha.Size = new Size(250, 20);
            this.txtNovaSenha.PasswordChar = '*';

            // Botão Gerar Senha
            this.btnGerarSenha.Text = "🎲 Gerar";
            this.btnGerarSenha.Location = new Point(270, 198);
            this.btnGerarSenha.Size = new Size(100, 24);
            this.btnGerarSenha.BackColor = Color.FromArgb(108, 117, 125);
            this.btnGerarSenha.ForeColor = Color.White;
            this.btnGerarSenha.FlatStyle = FlatStyle.Flat;
            this.btnGerarSenha.Click += btnGerarSenha_Click;

            // Confirmar Senha
            var lblConfirmar = new Label
            {
                Text = "Confirmar Senha:",
                Location = new Point(12, 235),
                Size = new Size(120, 15)
            };
            this.txtConfirmarSenha.Location = new Point(12, 255);
            this.txtConfirmarSenha.Size = new Size(250, 20);
            this.txtConfirmarSenha.PasswordChar = '*';

            // Checkbox Mostrar Senha
            this.chkMostrarSenha.Text = "👁️ Mostrar senhas";
            this.chkMostrarSenha.Location = new Point(12, 290);
            this.chkMostrarSenha.Size = new Size(150, 20);
            this.chkMostrarSenha.CheckedChanged += chkMostrarSenha_CheckedChanged;

            // Botões
            this.btnSalvar.Text = "💾 Salvar";
            this.btnSalvar.Location = new Point(217, 330);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSalvar.ForeColor = Color.White;
            this.btnSalvar.FlatStyle = FlatStyle.Flat;
            this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(297, 330);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancelar.ForeColor = Color.White;
            this.btnCancelar.FlatStyle = FlatStyle.Flat;
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            // Form
            this.Text = "Redefinir Senha - Sistema InterFix";
            this.Size = new Size(400, 410);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Adicionar controles
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblUsuario);
            this.Controls.Add(lblAviso);
            this.Controls.Add(lblNova);
            this.Controls.Add(txtNovaSenha);
            this.Controls.Add(btnGerarSenha);
            this.Controls.Add(lblConfirmar);
            this.Controls.Add(txtConfirmarSenha);
            this.Controls.Add(chkMostrarSenha);
            this.Controls.Add(btnSalvar);
            this.Controls.Add(btnCancelar);
        }

        private void btnGerarSenha_Click(object sender, EventArgs e)
        {
            string senhaGerada = GerarSenhaAleatoria();
            txtNovaSenha.Text = senhaGerada;
            txtConfirmarSenha.Text = senhaGerada;

            // Mostrar senha gerada temporariamente
            chkMostrarSenha.Checked = true;

            MessageBox.Show(
                $"✅ Senha gerada com sucesso!\n\n" +
                $"Senha: {senhaGerada}\n\n" +
                $"⚠️ ANOTE esta senha antes de salvar!\n" +
                $"Você deverá informá-la ao usuário.",
                "Senha Gerada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private string GerarSenhaAleatoria()
        {
            // Gera senha de 10 caracteres (letras e números, sem caracteres confusos)
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var random = new Random();
            var senha = new char[10];

            for (int i = 0; i < senha.Length; i++)
            {
                senha[i] = chars[random.Next(chars.Length)];
            }

            return new string(senha);
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            char passwordChar = chkMostrarSenha.Checked ? '\0' : '*';
            txtNovaSenha.PasswordChar = passwordChar;
            txtConfirmarSenha.PasswordChar = passwordChar;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validações
                if (string.IsNullOrWhiteSpace(txtNovaSenha.Text))
                {
                    MessageBox.Show("Digite a nova senha.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNovaSenha.Focus();
                    return;
                }

                if (txtNovaSenha.Text != txtConfirmarSenha.Text)
                {
                    MessageBox.Show("As senhas não coincidem!", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtConfirmarSenha.Focus();
                    return;
                }

                if (txtNovaSenha.Text.Length < 6)
                {
                    MessageBox.Show(
                        "A senha deve ter pelo menos 6 caracteres.\n\n" +
                        "💡 Use o botão 'Gerar' para criar uma senha segura.",
                        "Senha Fraca",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtNovaSenha.Focus();
                    return;
                }

                // Confirmar ação
                var result = MessageBox.Show(
                    $"Confirma redefinição de senha para:\n\n" +
                    $"👤 {_usuario.Nome}\n" +
                    $"📧 {_usuario.Email}\n\n" +
                    $"⚠️ Você deverá informar a nova senha ao usuário!",
                    "Confirmar Redefinição",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";
                Application.DoEvents();

                // Gerar hash da nova senha
                string senhaTemporaria = txtNovaSenha.Text;
                string novaSenhaHash = PasswordHasher.GerarHash(senhaTemporaria);

                // Atualizar no banco
                bool sucesso = _funcionariosController.AlterarSenha(_usuario.Id, novaSenhaHash);

                if (sucesso)
                {
                    // Mostrar senha para o admin copiar
                    var formResultado = MessageBox.Show(
                        $"✅ Senha redefinida com sucesso!\n\n" +
                        $"━━━━━━━━━━━━━━━━━━━━━━\n" +
                        $"👤 Usuário: {_usuario.Nome}\n" +
                        $"📧 Email: {_usuario.Email}\n" +
                        $"🔑 Nova Senha: {senhaTemporaria}\n" +
                        $"━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                        $"⚠️ IMPORTANTE:\n" +
                        $"Anote esta senha e informe ao usuário!\n\n" +
                        $"Deseja copiar a senha para área de transferência?",
                        "Senha Redefinida",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    // Copiar senha para clipboard
                    if (formResultado == DialogResult.Yes)
                    {
                        Clipboard.SetText(senhaTemporaria);
                        MessageBox.Show("📋 Senha copiada para área de transferência!", "Copiado",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "❌ Erro ao alterar senha no banco de dados.\n\n" +
                        "Tente novamente ou contate o suporte técnico.",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Erro ao salvar:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                btnSalvar.Enabled = true;
                btnSalvar.Text = "💾 Salvar";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}