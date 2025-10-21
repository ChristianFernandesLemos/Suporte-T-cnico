using SistemaChamados.Controllers;
using SistemaChamados.Forms;
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
    /// Formulário para criar novo usuário
    /// </summary>
    public partial class NovoUsuarioForm : Form
    {
        private FuncionariosController _funcionariosController;
        private TextBox txtNome;
        private TextBox txtCpf;
        private TextBox txtEmail;
        private TextBox txtSenha;
        private ComboBox cmbNivelAcesso;
        private Button btnSalvar;
        private Button btnCancelar;
        private Label lblTitulo;

        
        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.txtNome = new TextBox();
            this.txtCpf = new TextBox();
            this.txtEmail = new TextBox();
            this.txtSenha = new TextBox();
            this.cmbNivelAcesso = new ComboBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();

            // lblTitulo
            this.lblTitulo.Text = "Cadastrar Novo Usuário";
            this.lblTitulo.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 15);
            this.lblTitulo.Size = new Size(200, 20);

            // Campos do formulário
            var lblNome = new Label { Text = "Nome:", Location = new Point(12, 50), Size = new Size(80, 15) };
            this.txtNome.Location = new Point(12, 70);
            this.txtNome.Size = new Size(360, 20);

            var lblCpf = new Label { Text = "CPF:", Location = new Point(12, 100), Size = new Size(80, 15) };
            this.txtCpf.Location = new Point(12, 120);
            this.txtCpf.Size = new Size(150, 20);

            var lblEmail = new Label { Text = "E-mail:", Location = new Point(12, 150), Size = new Size(80, 15) };
            this.txtEmail.Location = new Point(12, 170);
            this.txtEmail.Size = new Size(360, 20);

            var lblSenha = new Label { Text = "Senha:", Location = new Point(12, 200), Size = new Size(80, 15) };
            this.txtSenha.Location = new Point(12, 220);
            this.txtSenha.Size = new Size(200, 20);
            this.txtSenha.PasswordChar = '*';

            var lblNivel = new Label { Text = "Nível de Acesso:", Location = new Point(12, 250), Size = new Size(100, 15) };
            this.cmbNivelAcesso.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbNivelAcesso.Location = new Point(12, 270);
            this.cmbNivelAcesso.Size = new Size(200, 21);

            // Configurar combo
            this.cmbNivelAcesso.Items.Add(new ComboBoxItem { Text = "Funcionário", Value = 1 });
            this.cmbNivelAcesso.Items.Add(new ComboBoxItem { Text = "Técnico", Value = 2 });
            this.cmbNivelAcesso.Items.Add(new ComboBoxItem { Text = "Administrador", Value = 3 });
            this.cmbNivelAcesso.SelectedIndex = 0;

            // btnSalvar
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new Point(217, 310);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSalvar.ForeColor = Color.White;
            this.btnSalvar.FlatStyle = FlatStyle.Flat;
            this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

            // btnCancelar
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(297, 310);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            // Form
            this.Text = "Novo Usuário - Sistema de Chamados";
            this.Size = new Size(400, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[]
            {
                this.lblTitulo, lblNome, this.txtNome, lblCpf, this.txtCpf,
                lblEmail, this.txtEmail, lblSenha, this.txtSenha,
                lblNivel, this.cmbNivelAcesso, this.btnSalvar, this.btnCancelar
            });
        }

        public NovoUsuarioForm(FuncionariosController funcionariosController)
        {
            _funcionariosController = funcionariosController;
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos()) return;

                var novoFuncionario = CriarFuncionario();
                int id = _funcionariosController.AdicionarFuncionario(novoFuncionario);

                if (id > 0)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erro ao criar usuário.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar usuário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Nome é obrigatório.", "Campo Obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCpf.Text))
            {
                MessageBox.Show("CPF é obrigatório.", "Campo Obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCpf.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("E-mail é obrigatório.", "Campo Obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show("Senha é obrigatória.", "Campo Obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSenha.Focus();
                return false;
            }

            return true;
        }

        private Funcionarios CriarFuncionario()
        {
            var item = (ComboBoxItem)cmbNivelAcesso.SelectedItem;
            int nivelAcesso = item.Value;

            // MODIFICADO: Gerar hash da senha antes de criar o funcionário
            string senhaHash = PasswordHasher.GerarHash(txtSenha.Text);

            // Use 0 for Id and nivelAcesso from ComboBox
            switch (nivelAcesso)
            {
                case 1:
                    return new Funcionario(0, txtNome.Text, txtCpf.Text, txtEmail.Text, senhaHash, nivelAcesso);
                case 2:
                    return new Tecnico(0, txtNome.Text, txtCpf.Text, txtEmail.Text, senhaHash, nivelAcesso);
                case 3:
                    return new ADM(0, txtNome.Text, txtCpf.Text, txtEmail.Text, senhaHash, nivelAcesso);
                default:
                    throw new ArgumentException("Nível de acesso inválido");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
