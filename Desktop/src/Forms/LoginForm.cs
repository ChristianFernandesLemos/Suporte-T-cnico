using SistemaChamados.Controllers;
using SistemaChamados.Data;
using SistemaChamados.Models;
using SistemaChamados.Config;
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
    public partial class LoginForm : Form
    {
        private FuncionariosController _funcionariosController;
        private TextBox txtEmail;
        private TextBox txtSenha;
        private Button btnLogin;
        private Label lblEmail;
        private Label lblSenha;
        private Label lblTitulo;
        private CheckBox chkMostrarSenha;
        private LinkLabel lnkEsqueciSenha;

        public LoginForm()
        {
            InitializeComponent();
            InicializarControladores();
            ConfigurarPlaceholders();
        }

        private void InicializarControladores()
        {
            try
            {
                var connectionString = DatabaseConfig.ConnectionString;
                var database = new SqlServerConnection(connectionString);
                _funcionariosController = new FuncionariosController(database);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com o banco de dados: {ex.Message}",
                    "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarPlaceholders()
        {
            // Configurar placeholders usando a classe helper
            txtEmail.SetPlaceholder("Digite seu email");
            txtSenha.SetPlaceholder("Digite sua senha");
        }


        private void InitializeComponent()
        {
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblSenha = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.chkMostrarSenha = new System.Windows.Forms.CheckBox();
            this.lnkEsqueciSenha = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(50, 100);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(300, 20);
            this.txtEmail.TabIndex = 2;
            // 
            // txtSenha
            // 
            this.txtSenha.Location = new System.Drawing.Point(50, 160);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '*';
            this.txtSenha.Size = new System.Drawing.Size(300, 20);
            this.txtSenha.TabIndex = 4;
            this.txtSenha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSenha_KeyPress);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(50, 240);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(300, 35);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Entrar";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(50, 80);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(35, 13);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "Email:";
            // 
            // lblSenha
            // 
            this.lblSenha.AutoSize = true;
            this.lblSenha.Location = new System.Drawing.Point(50, 140);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(41, 13);
            this.lblSenha.TabIndex = 3;
            this.lblSenha.Text = "Senha:";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(117, 38);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(157, 26);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "InterFix Login";
            // 
            // chkMostrarSenha
            // 
            this.chkMostrarSenha.AutoSize = true;
            this.chkMostrarSenha.Location = new System.Drawing.Point(50, 190);
            this.chkMostrarSenha.Name = "chkMostrarSenha";
            this.chkMostrarSenha.Size = new System.Drawing.Size(93, 17);
            this.chkMostrarSenha.TabIndex = 5;
            this.chkMostrarSenha.Text = "Mostrar senha";
            this.chkMostrarSenha.UseVisualStyleBackColor = true;
            this.chkMostrarSenha.CheckedChanged += new System.EventHandler(this.chkMostrarSenha_CheckedChanged);
            // 
            // lnkEsqueciSenha
            // 
            this.lnkEsqueciSenha.AutoSize = true;
            this.lnkEsqueciSenha.Location = new System.Drawing.Point(50, 215);
            this.lnkEsqueciSenha.Name = "lnkEsqueciSenha";
            this.lnkEsqueciSenha.Size = new System.Drawing.Size(105, 13);
            this.lnkEsqueciSenha.TabIndex = 7;
            this.lnkEsqueciSenha.TabStop = true;
            this.lnkEsqueciSenha.Text = "Esqueceu sua senha?";
            this.lnkEsqueciSenha.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkEsqueciSenha_LinkClicked);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 320);
            this.Controls.Add(this.lnkEsqueciSenha);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.chkMostrarSenha);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.lblSenha);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login - Sistema de Chamados";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Usar GetText() para obter o valor real (sem placeholder)
                string email = txtEmail.GetText();
                string senha = txtSenha.GetText();            

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Por favor, informe o email.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(senha))
                {
                    MessageBox.Show("Por favor, informe a senha.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSenha.Focus();
                    return;
                }

                btnLogin.Enabled = false;
                btnLogin.Text = "Entrando...";

                // Gerar hash da senha antes de enviar
                string senhaHash = PasswordHasher.GerarHash(senha);
                var funcionario = _funcionariosController.RealizarLogin(email.Trim(), senhaHash);

                if (funcionario != null)
                {
                    // Criar o menu principal
                    var menuPrincipal = new MenuPrincipalForm(funcionario);
                    menuPrincipal.FormClosed += (s, args) => this.Close();
                    // Esconder login e mostrar menu
                    this.Hide();
                    menuPrincipal.Show();
                }
                else
                {
                    MessageBox.Show("Email ou senha incorretos.", "Erro de Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSenha.Clear();
                    txtSenha.SetPlaceholder("Digite sua senha");
                    txtSenha.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao realizar login: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Entrar";
            }
        }

        private void txtSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.PasswordChar = chkMostrarSenha.Checked ? '\0' : '*';
        }

        private void lnkEsqueciSenha_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var esqueciSenhaForm = new src.Forms.EsqueciSenhaForm();
            esqueciSenhaForm.ShowDialog();
        }
    }
}
