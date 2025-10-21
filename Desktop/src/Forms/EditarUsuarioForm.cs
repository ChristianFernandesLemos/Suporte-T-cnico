using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Forms;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    /// <summary>
    /// Formulário para editar usuário existente
    /// </summary>
    public partial class EditarUsuarioForm : Form
    {
        private Funcionarios _usuario;
        private FuncionariosController _funcionariosController;
        private TextBox txtNome;
        private TextBox txtCpf;
        private TextBox txtEmail;
        private Button btnSalvar;
        private Button btnCancelar;

        private void InitializeComponent()
        {
            // Similar ao NovoUsuarioForm, mas sem senha e nível de acesso
            this.txtNome = new TextBox();
            this.txtCpf = new TextBox();
            this.txtEmail = new TextBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();

            var lblTitulo = new Label
            {
                Text = "Editar Usuário",
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(200, 20)
            };

            var lblNome = new Label { Text = "Nome:", Location = new Point(12, 50), Size = new Size(80, 15) };
            this.txtNome.Location = new Point(12, 70);
            this.txtNome.Size = new Size(360, 20);

            var lblCpf = new Label { Text = "CPF:", Location = new Point(12, 100), Size = new Size(80, 15) };
            this.txtCpf.Location = new Point(12, 120);
            this.txtCpf.Size = new Size(150, 20);

            var lblEmail = new Label { Text = "E-mail:", Location = new Point(12, 150), Size = new Size(80, 15) };
            this.txtEmail.Location = new Point(12, 170);
            this.txtEmail.Size = new Size(360, 20);

            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new Point(217, 220);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(297, 220);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            this.Text = "Editar Usuário";
            this.Size = new Size(400, 290);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[]
            {
                lblTitulo, lblNome, this.txtNome, lblCpf, this.txtCpf,
                lblEmail, this.txtEmail, this.btnSalvar, this.btnCancelar
            });
        }

        public EditarUsuarioForm(Funcionarios usuario, FuncionariosController funcionariosController)
        {
            _usuario = usuario;
            _funcionariosController = funcionariosController;
            InitializeComponent();
            CarregarDados();
        }

        private void CarregarDados()
        {
            txtNome.Text = _usuario.Nome.ToString();
            txtCpf.Text = _usuario.Cpf;
            txtEmail.Text = _usuario.Email;
        }

        private TextBox GetTxtNome()
        {
            return txtNome;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                _usuario.Nome = txtNome.Text; // Assuming Nome is string, if not, adjust accordingly
                _usuario.Cpf = txtCpf.Text;
                _usuario.Email = txtEmail.Text;

                _funcionariosController.AtualizarFuncionario(_usuario);

                MessageBox.Show("Usuário atualizado com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar usuário: {ex.Message}", "Erro",
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
