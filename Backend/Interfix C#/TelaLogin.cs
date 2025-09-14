using System;
using System.Windows.Forms;

public partial class TelaLogin : Form
{
    private BancoDados bd;

    public TelaLogin()
    {
        InitializeComponent();
        bd = new BancoDados();
    }

    private void btnLogin_Click(object sender, EventArgs e)
{
    using (var bd = new BancoDados())
    {
        var (usuarioId, tipoUsuario) = bd.AutenticarUsuario(txtUsuario.Text, txtSenha.Text);
        if (usuarioId != -1)
        {
            this.Hide();
            var telaPrincipal = new TelaPrincipal(bd, usuarioId, tipoUsuario);
            telaPrincipal.ShowDialog();
            this.Close();
        }
        else
        {
            MessageBox.Show("Usuário ou senha incorretos!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

    private void InitializeComponent()
    {
        this.Text = "Login - Sistema Chamados";
        this.Size = new System.Drawing.Size(350, 200);
        this.StartPosition = FormStartPosition.CenterScreen;

        var panel = new Panel { Dock = DockStyle.Fill };
        this.Controls.Add(panel);

        var lblUsuario = new Label { Text = "Usuário:", Location = new System.Drawing.Point(20, 20), Width = 60 };
        var txtUsuario = new TextBox { Location = new System.Drawing.Point(90, 20), Width = 150 };

        var lblSenha = new Label { Text = "Senha:", Location = new System.Drawing.Point(20, 50), Width = 60 };
        var txtSenha = new TextBox { Location = new System.Drawing.Point(90, 50), Width = 150, PasswordChar = '*' };

        var btnLogin = new Button { Text = "Login", Location = new System.Drawing.Point(90, 80), Width = 80 };

        panel.Controls.AddRange(new Control[] { lblUsuario, txtUsuario, lblSenha, txtSenha, btnLogin });

        btnLogin.Click += (sender, e) =>
        {
            var (usuarioId, tipoUsuario) = bd.AutenticarUsuario(txtUsuario.Text, txtSenha.Text);
            if (usuarioId != -1)
            {
                this.Hide();
                var telaPrincipal = new TelaPrincipal(bd, usuarioId, tipoUsuario);
                telaPrincipal.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorretos!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
    }
}