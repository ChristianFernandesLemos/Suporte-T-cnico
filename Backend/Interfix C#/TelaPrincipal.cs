using System;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class TelaPrincipal : Form
{
    private BancoDados bd;
    private int usuarioId;
    private string tipoUsuario;
    private DataGridView dataGridView;

    public TelaPrincipal(BancoDados bancoDados, int usuarioId, string tipoUsuario)
    {
        this.bd = bancoDados;
        this.usuarioId = usuarioId;
        this.tipoUsuario = tipoUsuario;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Sistema de Chamados";
        this.Size = new System.Drawing.Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Menu
        var menuStrip = new MenuStrip();
        var menuChamados = new ToolStripMenuItem("Chamados");
        
        menuChamados.DropDownItems.Add("Novo Chamado", null, (s, e) => NovoChamado());
        menuChamados.DropDownItems.Add("Todos Chamados", null, (s, e) => MostrarChamados());
        menuChamados.DropDownItems.Add("Meus Chamados", null, (s, e) => MostrarChamados("meus"));

        menuStrip.Items.Add(menuChamados);

        if (tipoUsuario == "admin")
        {
            var menuAdmin = new ToolStripMenuItem("Admin");
            menuAdmin.Click += (s, e) => MessageBox.Show("Área administrativa (em desenvolvimento)");
            menuStrip.Items.Add(menuAdmin);
        }
        else if (tipoUsuario == "tecnico")
        {
            var menuTecnico = new ToolStripMenuItem("Técnico");
            menuTecnico.Click += (s, e) => MessageBox.Show("Área técnica (em desenvolvimento)");
            menuStrip.Items.Add(menuTecnico);
        }

        var menuSair = new ToolStripMenuItem("Sair");
        menuSair.Click += (s, e) => Sair();
        menuStrip.Items.Add(menuSair);

        this.Controls.Add(menuStrip);
        this.MainMenuStrip = menuStrip;

        // DataGridView
        dataGridView = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        dataGridView.Columns.Add("Id", "ID");
        dataGridView.Columns.Add("Titulo", "Título");
        dataGridView.Columns.Add("Status", "Status");
        dataGridView.Columns.Add("Data", "Data Abertura");

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        panel.Controls.Add(dataGridView);
        this.Controls.Add(panel);

        MostrarChamados();
    }

    private void NovoChamado()
    {
        var form = new Form
        {
            Text = "Novo Chamado",
            Size = new System.Drawing.Size(400, 300),
            StartPosition = FormStartPosition.CenterParent
        };

        var lblTitulo = new Label { Text = "Título:", Location = new System.Drawing.Point(20, 20) };
        var txtTitulo = new TextBox { Location = new System.Drawing.Point(100, 20), Width = 250 };

        var lblDescricao = new Label { Text = "Descrição:", Location = new System.Drawing.Point(20, 60) };
        var txtDescricao = new TextBox { Location = new System.Drawing.Point(100, 60), Width = 250, Height = 100, Multiline = true };

        var btnSalvar = new Button { Text = "Salvar", Location = new System.Drawing.Point(150, 180), Width = 80 };

        form.Controls.AddRange(new Control[] { lblTitulo, txtTitulo, lblDescricao, txtDescricao, btnSalvar });

        btnSalvar.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(txtTitulo.Text) && !string.IsNullOrEmpty(txtDescricao.Text))
            {
                bd.InserirChamado(txtTitulo.Text, txtDescricao.Text, usuarioId);
                form.Close();
                MostrarChamados();
                MessageBox.Show("Chamado criado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Preencha todos os campos!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        form.ShowDialog();
    }

    private void MostrarChamados(string filtro = null)
{
    dataGridView.Rows.Clear();
    List<Chamado> chamados;

    if (filtro == "meus")
    {
        chamados = bd.BuscarMeusChamados(usuarioId);
    }
    else if (!string.IsNullOrEmpty(filtro))
    {
        chamados = bd.BuscarChamados(filtro);
    }
    else
    {
        chamados = bd.BuscarChamados();
    }

    foreach (var chamado in chamados)
    {
        dataGridView.Rows.Add(chamado.Id, chamado.Titulo, chamado.Status, chamado.DataAbertura);
    }
}

    private void Sair()
    {
        if (MessageBox.Show("Deseja sair do sistema?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            bd.FecharConexao();
            this.Close();
        }
    }
}