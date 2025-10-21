using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SistemaChamados.Forms
{
    /// <summary>
    /// Formulário para alterar senha
    /// </summary>
    public partial class AlterarSenhaForm : Form
    {
        private Funcionarios _usuario;
        private FuncionariosController _funcionariosController;
        private TextBox txtNovaSenha;
        private TextBox txtConfirmarSenha;
        private Button btnSalvar;
        private Button btnCancelar;

        private void InitializeComponent()
        {
            this.txtNovaSenha = new TextBox();
            this.txtConfirmarSenha = new TextBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();

            var lblTitulo = new Label
            {
                Text = $"Alterar Senha - {_usuario.Email}",
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(300, 20)
            };

            var lblNova = new Label { Text = "Nova Senha:", Location = new Point(12, 50), Size = new Size(80, 15) };
            this.txtNovaSenha.Location = new Point(12, 70);
            this.txtNovaSenha.Size = new Size(250, 20);
            this.txtNovaSenha.PasswordChar = '*';

            var lblConfirmar = new Label { Text = "Confirmar Senha:", Location = new Point(12, 100), Size = new Size(100, 15) };
            this.txtConfirmarSenha.Location = new Point(12, 120);
            this.txtConfirmarSenha.Size = new Size(250, 20);
            this.txtConfirmarSenha.PasswordChar = '*';

            this.btnSalvar.Text = "Alterar";
            this.btnSalvar.Location = new Point(107, 160);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(187, 160);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            this.Text = "Alterar Senha";
            this.Size = new Size(290, 230);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[]
            {
                lblTitulo, lblNova, this.txtNovaSenha,
                lblConfirmar, this.txtConfirmarSenha,
                this.btnSalvar, this.btnCancelar
            });
        }

        public AlterarSenhaForm(Funcionarios usuario, FuncionariosController funcionariosController)
        {
            _usuario = usuario;
            _funcionariosController = funcionariosController;
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNovaSenha.Text))
                {
                    MessageBox.Show("Digite a nova senha.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNovaSenha.Text != txtConfirmarSenha.Text)
                {
                    MessageBox.Show("As senhas não coincidem.", "Senhas Diferentes",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // MODIFICADO: Gerar hash da nova senha
                string novaSenhaHash = PasswordHasher.GerarHash(txtNovaSenha.Text);
                _funcionariosController.AlterarSenha(_usuario.Id, novaSenhaHash);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar senha: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
