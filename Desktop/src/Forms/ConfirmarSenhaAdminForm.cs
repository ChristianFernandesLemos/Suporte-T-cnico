using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Models;
using SistemaChamados.Helpers;

namespace SistemaChamados.Forms
{
    public partial class ConfirmarSenhaAdminForm : Form
    {
        private Funcionarios _administrador;
        private Label lblTitulo;
        private Label lblInstrucao;
        private TextBox txtSenha;
        private Button btnConfirmar;
        private Button btnCancelar;
        private Panel pnlHeader;

        public bool SenhaConfirmada { get; private set; }

        public ConfirmarSenhaAdminForm(Funcionarios administrador)
        {
            _administrador = administrador;
            SenhaConfirmada = false;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(450, 280);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Confirmação de Segurança";
            this.BackColor = Color.White;

            // Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(220, 53, 69)
            };

            lblTitulo = new Label
            {
                Text = "🔒 Confirmação de Segurança",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(450, 35),
                Location = new Point(0, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlHeader.Controls.Add(lblTitulo);

            // Instrução
            lblInstrucao = new Label
            {
                Text = "Para realizar alterações nos usuários,\npor favor confirme sua senha de administrador:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(70, 70, 70),
                AutoSize = false,
                Size = new Size(400, 45),
                Location = new Point(25, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // TextBox Senha
            txtSenha = new TextBox
            {
                Location = new Point(25, 155),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 11F),
                PasswordChar = '●',
                MaxLength = 50
            };
            txtSenha.KeyPress += TxtSenha_KeyPress;

            // Botão Confirmar
            btnConfirmar = new Button
            {
                Text = "Confirmar",
                Size = new Size(120, 40),
                Location = new Point(305, 210),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            // Botão Cancelar
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(120, 40),
                Location = new Point(175, 210),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblInstrucao);
            this.Controls.Add(txtSenha);
            this.Controls.Add(btnConfirmar);
            this.Controls.Add(btnCancelar);

            this.ResumeLayout(false);
        }

        private void TxtSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnConfirmar_Click(sender, e);
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show(
                    "Por favor, digite sua senha.",
                    "Campo Obrigatório",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtSenha.Focus();
                return;
            }

            // Verifica a senha usando hash
            if (PasswordHasher.VerificarSenha(txtSenha.Text, _administrador.Senha))
            {
                SenhaConfirmada = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Senha incorreta! Tente novamente.",
                    "Erro de Autenticação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                txtSenha.Clear();
                txtSenha.Focus();
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            SenhaConfirmada = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
