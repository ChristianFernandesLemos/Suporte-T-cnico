using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Helpers;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace SistemaChamados.Forms
{
    /// <summary>
    /// Formulário para ADMIN redefinir senha de usuário
    /// Usuários comuns NÃO podem alterar própria senha
    /// </summary>
    public partial class AlterarSenhaForm : Form
    {
        private Funcionarios _usuario;
        private FuncionariosController _funcionariosController;

        private TextBox txtNovaSenha;
        private TextBox txtConfirmarSenha;
        private CheckBox chkMostrarSenha;
        private Button btnSalvar;
        private Button btnCancelar;
        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblAviso;

        /// <summary>
        /// Constructor - Solo puede ser llamado por Administrador
        /// </summary>
        public AlterarSenhaForm(Funcionarios usuario, FuncionariosController funcionariosController)
        {
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
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

            // lblTitulo
            this.lblTitulo.Text = "🔐 Redefinir Senha de Usuário";
            this.lblTitulo.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 15);
            this.lblTitulo.Size = new Size(360, 20);

            // lblUsuario
            this.lblUsuario.Location = new Point(12, 45);
            this.lblUsuario.Size = new Size(360, 60);
            this.lblUsuario.Text = $"Usuário: {_usuario.Nome}\n" +
                                  $"Email: {_usuario.Email}\n" +
                                  $"CPF: {_usuario.Cpf}";

            // lblAviso
            this.lblAviso.Location = new Point(12, 110);
            this.lblAviso.Size = new Size(360, 40);
            this.lblAviso.ForeColor = Color.DarkOrange;
            this.lblAviso.Text = "⚠️ Digite a nova senha manualmente.\n" +
                                "Você deverá informar ao usuário pessoalmente.";

            // Nova Senha
            var lblNova = new Label
            {
                Text = "Nova Senha:",
                Location = new Point(12, 160),
                Size = new Size(100, 15)
            };
            this.txtNovaSenha.Location = new Point(12, 180);
            this.txtNovaSenha.Size = new Size(360, 20);
            this.txtNovaSenha.PasswordChar = '*';

            // Confirmar Senha
            var lblConfirmar = new Label
            {
                Text = "Confirmar Senha:",
                Location = new Point(12, 215),
                Size = new Size(120, 15)
            };
            this.txtConfirmarSenha.Location = new Point(12, 235);
            this.txtConfirmarSenha.Size = new Size(360, 20);
            this.txtConfirmarSenha.PasswordChar = '*';

            // Checkbox Mostrar Senha
            this.chkMostrarSenha.Text = "Mostrar senhas";
            this.chkMostrarSenha.Location = new Point(12, 270);
            this.chkMostrarSenha.Size = new Size(120, 20);
            this.chkMostrarSenha.CheckedChanged += chkMostrarSenha_CheckedChanged;

            // Botões
            this.btnSalvar.Text = "💾 Salvar";
            this.btnSalvar.Location = new Point(217, 310);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSalvar.ForeColor = Color.White;
            this.btnSalvar.FlatStyle = FlatStyle.Flat;
            this.btnSalvar.Click += btnSalvar_Click;

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(297, 310);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += btnCancelar_Click;

            // Form
            this.Text = "Redefinir Senha - InterFix";
            this.Size = new Size(400, 390);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Adicionar controles
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblUsuario);
            this.Controls.Add(lblAviso);
            this.Controls.Add(lblNova);
            this.Controls.Add(txtNovaSenha);
            this.Controls.Add(lblConfirmar);
            this.Controls.Add(txtConfirmarSenha);
            this.Controls.Add(chkMostrarSenha);
            this.Controls.Add(btnSalvar);
            this.Controls.Add(btnCancelar);
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
                    MessageBox.Show("A senha deve ter pelo menos 6 caracteres.", "Senha Fraca",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNovaSenha.Focus();
                    return;
                }

                // Confirmar ação
                var result = MessageBox.Show(
                    $"Confirma redefinição de senha para:\n\n" +
                    $"{_usuario.Nome}\n" +
                    $"{_usuario.Email}\n\n" +
                    $"⚠️ Você deverá informar manualmente\n" +
                    $"a nova senha ao usuário!",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";
                Application.DoEvents();

                // Gerar hash da nova senha
                string novaSenhaTexto = txtNovaSenha.Text;
                string novaSenhaHash = PasswordHasher.GerarHash(novaSenhaTexto);

                // Atualizar no banco
                bool sucesso = _funcionariosController.AlterarSenha(_usuario.Id, novaSenhaHash);

                if (sucesso)
                {
                    MessageBox.Show(
                        $"✅ Senha redefinida com sucesso!\n\n" +
                        $"📋 Nova senha: {novaSenhaTexto}\n\n" +
                        $"⚠️ IMPORTANTE:\n" +
                        $"Informe esta senha ao usuário via:\n" +
                        $"• Email: {_usuario.Email}\n" +
                        $"• Telefone\n" +
                        $"• Pessoalmente",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erro ao alterar senha no banco de dados.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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