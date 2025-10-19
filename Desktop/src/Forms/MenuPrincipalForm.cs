using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SistemaChamados.Config;
using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Forms;
using SistemaChamados.Data;
using SistemaChamados.Properties;


namespace SistemaChamados.Forms
{
    /// <summary>
    /// Formulário principal do sistema com menu
    /// </summary>
    public partial class MenuPrincipalForm : Form
    {
        // Propriedade para o usuário logado
        private Funcionarios _usuarioLogado;
        private FuncionariosController _funcionariosController;
        private ChamadosController _chamadosController;
        
        // Componentes do menu
        private MenuStrip menuPrincipal;
        private ToolStripMenuItem menuArquivo;
        private ToolStripMenuItem menuChamados;
        private ToolStripMenuItem menuUsuarios;
        private ToolStripMenuItem menuRelatorios;
        private ToolStripMenuItem menuAjuda;
        
        // Status bar
        private StatusStrip statusBar;
        private ToolStripStatusLabel lblUsuarioLogado;
        private ToolStripStatusLabel lblDataHora;
        private ToolStripStatusLabel lblNivelAcesso;
        
        // Timer para atualizar hora
        private System.Windows.Forms.Timer timerRelogio;
        
        // Panel principal
        private Panel panelPrincipal;
        private Label lblBemVindo;
        private PictureBox pictureLogo;
        private Funcionarios _funcionarioLogado;

        public MenuPrincipalForm(Funcionarios usuarioLogado)
        {
            _usuarioLogado = usuarioLogado;
            // Connection string do banco de dados
            var connectionString = DatabaseConfig.ConnectionString;
            var database = new SqlServerConnection(connectionString);
            _funcionariosController = new FuncionariosController(database);
            _chamadosController = new ChamadosController(database);

            InitializeComponent();            
            ConfigurarFormulario();
            ConfigurarPermissoes();
            
        }

        private void InitializeComponent()
        {
            // Configuração do Form
            this.Text = "Sistema de Chamados - Menu Principal";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.Icon = SystemIcons.Application;
            
            // Menu Principal
            this.menuPrincipal = new MenuStrip();
            this.menuPrincipal.BackColor = Color.FromArgb(240, 240, 240);
            this.menuPrincipal.Font = new Font("Segoe UI", 9F);
            
            // Menu Arquivo
            this.menuArquivo = new ToolStripMenuItem("&Arquivo");
            var itemAlterarSenha = new ToolStripMenuItem("Alterar Senha", null, ItemAlterarSenha_Click);
            itemAlterarSenha.ShortcutKeys = Keys.Control | Keys.P;
            var itemLogout = new ToolStripMenuItem("Logout", null, ItemLogout_Click);
            itemLogout.ShortcutKeys = Keys.Control | Keys.L;
            var itemSair = new ToolStripMenuItem("Sair", null, ItemSair_Click);
            itemSair.ShortcutKeys = Keys.Alt | Keys.F4;
            
            this.menuArquivo.DropDownItems.AddRange(new ToolStripItem[] {
                itemAlterarSenha,
                new ToolStripSeparator(),
                itemLogout,
                new ToolStripSeparator(),
                itemSair
            });
            
            // Menu Chamados
            this.menuChamados = new ToolStripMenuItem("&Chamados");
            var itemNovoChamado = new ToolStripMenuItem("Novo Chamado", null, ItemNovoChamado_Click);
            itemNovoChamado.ShortcutKeys = Keys.Control | Keys.N;
            var itemVisualizarChamados = new ToolStripMenuItem("Visualizar Chamados", null, ItemVisualizarChamados_Click);
            itemVisualizarChamados.ShortcutKeys = Keys.Control | Keys.V;
            var itemGerenciarChamados = new ToolStripMenuItem("Gerenciar Chamados", null, ItemGerenciarChamados_Click);
            itemGerenciarChamados.ShortcutKeys = Keys.Control | Keys.G;
            
            this.menuChamados.DropDownItems.AddRange(new ToolStripItem[] {
                itemNovoChamado,
                new ToolStripSeparator(),
                itemVisualizarChamados,
                itemGerenciarChamados
            });
            
            // Menu Usuários (apenas para administradores)
            this.menuUsuarios = new ToolStripMenuItem("&Usuários");
            var itemNovoUsuario = new ToolStripMenuItem("Novo Usuário", null, ItemNovoUsuario_Click);
            var itemGerenciarUsuarios = new ToolStripMenuItem("Gerenciar Usuários", null, ItemGerenciarUsuarios_Click);
            
            this.menuUsuarios.DropDownItems.AddRange(new ToolStripItem[] {
                itemNovoUsuario,
                new ToolStripSeparator(),
                itemGerenciarUsuarios
            });
            
            // Menu Relatórios
            this.menuRelatorios = new ToolStripMenuItem("&Relatórios");
            var itemRelatorioChamados = new ToolStripMenuItem("Relatório de Chamados", null, ItemRelatorioChamados_Click);
            var itemEstatisticas = new ToolStripMenuItem("Estatísticas", null, ItemEstatisticas_Click);
            
            this.menuRelatorios.DropDownItems.AddRange(new ToolStripItem[] {
                itemRelatorioChamados,
                itemEstatisticas
            });
            
            // Menu Ajuda
            this.menuAjuda = new ToolStripMenuItem("&Ajuda");
            var itemManual = new ToolStripMenuItem("Manual do Usuário", null, ItemManual_Click);
            itemManual.ShortcutKeys = Keys.F1;
            var itemSobre = new ToolStripMenuItem("Sobre", null, ItemSobre_Click);
            
            this.menuAjuda.DropDownItems.AddRange(new ToolStripItem[] {
                itemManual,
                new ToolStripSeparator(),
                itemSobre
            });
            
            // Adicionar menus ao MenuStrip
            this.menuPrincipal.Items.AddRange(new ToolStripItem[] {
                this.menuArquivo,
                this.menuChamados,
                this.menuUsuarios,
                this.menuRelatorios,
                this.menuAjuda
            });
            
            // Status Bar
            this.statusBar = new StatusStrip();
            this.statusBar.BackColor = Color.FromArgb(240, 240, 240);

            this.lblUsuarioLogado = new ToolStripStatusLabel
            {
                Text = $"Usuário: {_usuarioLogado.Nome} ({_usuarioLogado.Email})",
                BorderSides = ToolStripStatusLabelBorderSides.Right
            };

            this.lblNivelAcesso = new ToolStripStatusLabel();
            this.lblNivelAcesso.Text = $"Nível: {ObterTextoNivelAcesso(_usuarioLogado.NivelAcesso)}";
            this.lblNivelAcesso.BorderSides = ToolStripStatusLabelBorderSides.Right;
            
            this.lblDataHora = new ToolStripStatusLabel();
            this.lblDataHora.Spring = true;
            this.lblDataHora.TextAlign = ContentAlignment.MiddleRight;
            
            this.statusBar.Items.AddRange(new ToolStripItem[] {
                this.lblUsuarioLogado,
                this.lblNivelAcesso,
                this.lblDataHora
            });
            
            // Panel Principal con Dashboard
            this.panelPrincipal = new Panel();
            this.panelPrincipal.Dock = DockStyle.Fill;
            this.panelPrincipal.BackColor = Color.White;
            this.panelPrincipal.AutoScroll = true;
            
            // Título do Dashboard
            var lblTituloDashboard = new Label();
            lblTituloDashboard.Text = "Dashboard - Visão Geral";
            lblTituloDashboard.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTituloDashboard.Location = new Point(50, 30);
            lblTituloDashboard.AutoSize = true;
            lblTituloDashboard.ForeColor = Color.FromArgb(52, 58, 64);
            
            // Card de Boas-vindas
            var panelBoasVindas = CriarCardInfo(
                "Bem-vindo ao Sistema",
                $"Olá, {_usuarioLogado.Nome}!\n" +
                $"Nível: {ObterTextoNivelAcesso(_usuarioLogado.NivelAcesso)}\n" +
                $"Email: {_usuarioLogado.Email}",
                Color.FromArgb(0, 123, 255),
                new Point(50, 80)
            );
            
            // Card de Atalhos Rápidos
            var panelAtalhos = CriarCardAtalhos(new Point(400, 80));
            
            // Card de Estatísticas (se for Admin ou Técnico)
            Panel panelEstatisticas = null;
            if (_usuarioLogado.NivelAcesso >= 2)
            {
                panelEstatisticas = CriarCardEstatisticas(new Point(50, 250));
            }
            
            // Card de Informações do Sistema
            var panelInfoSistema = CriarCardInfo(
                "Informações do Sistema",
                $"Versão: 1.0.0\n" +
                $"Data: {DateTime.Now:dd/MM/yyyy}\n" +
                $"Hora: {DateTime.Now:HH:mm:ss}",
                Color.FromArgb(40, 167, 69),
                new Point(400, 250)
            );
            
            // Adicionar todos os cards ao panel
            this.panelPrincipal.Controls.Add(lblTituloDashboard);
            this.panelPrincipal.Controls.Add(panelBoasVindas);
            this.panelPrincipal.Controls.Add(panelAtalhos);
            if (panelEstatisticas != null)
                this.panelPrincipal.Controls.Add(panelEstatisticas);
            this.panelPrincipal.Controls.Add(panelInfoSistema);
            
            // Timer para atualizar relógio
            this.timerRelogio = new System.Windows.Forms.Timer();
            this.timerRelogio.Interval = 1000;
            this.timerRelogio.Tick += TimerRelogio_Tick;
            this.timerRelogio.Start();
            
            // Adicionar controles ao Form
            this.Controls.Add(this.panelPrincipal);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuPrincipal);
            this.MainMenuStrip = this.menuPrincipal;
            
            // Evento de fechamento
            this.FormClosing += MenuPrincipalForm_FormClosing;
        }

        private void ConfigurarFormulario()
        {
            // Atualizar data/hora inicial
            TimerRelogio_Tick(null, null);
            
            // Configurar atalhos de teclado globais
            this.KeyPreview = true;
            this.KeyDown += MenuPrincipalForm_KeyDown;
        }

        private void ConfigurarPermissoes()
        {
            // Configurar visibilidade dos menus baseado no nível de acesso
            switch (_usuarioLogado.NivelAcesso)
            {
                case 1: // Funcionário
                    this.menuUsuarios.Visible = false;
                    this.menuRelatorios.Visible = false;
                    
                    // Limitar opções de chamados - CORRIGIDO: verificar tipo antes do cast
                    foreach (ToolStripItem item in this.menuChamados.DropDownItems)
                    {
                        if (item is ToolStripMenuItem menuItem && menuItem.Text == "Gerenciar Chamados")
                            menuItem.Visible = false;
                    }
                    
                    // Ocultar "Alterar Senha" - apenas admin pode alterar senhas
                    foreach (ToolStripItem item in this.menuArquivo.DropDownItems)
                    {
                        if (item is ToolStripMenuItem menuItem && menuItem.Text == "Alterar Senha")
                            menuItem.Visible = false;
                    }
                    break;
                    
                case 2: // Técnico - NÃO pode criar chamados
                    this.menuUsuarios.Visible = false;
                    
                    // Ocultar "Novo Chamado" para técnicos - CORRIGIDO: verificar tipo antes do cast
                    foreach (ToolStripItem item in this.menuChamados.DropDownItems)
                    {
                        if (item is ToolStripMenuItem menuItem && menuItem.Text == "Novo Chamado")
                            menuItem.Visible = false;
                    }
                    
                    // Ocultar "Alterar Senha" - apenas admin pode alterar senhas
                    foreach (ToolStripItem item in this.menuArquivo.DropDownItems)
                    {
                        if (item is ToolStripMenuItem menuItem && menuItem.Text == "Alterar Senha")
                            menuItem.Visible = false;
                    }
                    break;
                    
                case 3: // Administrador
                    // Tem acesso a tudo
                    break;
            }
        }

        private string ObterTextoNivelAcesso(int nivel)
        {
            switch (nivel)
            {
                case 1: return "Funcionário";
                case 2: return "Técnico";
                case 3: return "Administrador";
                default: return "Desconhecido";
            }
        }

        // Eventos do Timer
        private void TimerRelogio_Tick(object sender, EventArgs e)
        {
            this.lblDataHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        // Eventos do Menu Arquivo
        private void ItemAlterarSenha_Click(object sender, EventArgs e)
        {
            try
            {
                var formAlterarSenha = new AlterarSenhaForm(_usuarioLogado, _funcionariosController);
                if (formAlterarSenha.ShowDialog(this) == DialogResult.OK)
                {
                    MessageBox.Show("Senha alterada com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemLogout_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("Deseja realmente fazer logout?", "Confirmar Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (resultado == DialogResult.Yes)
            {
                this.Hide();
                var loginForm = new LoginForm();
                loginForm.ShowDialog();
                this.Close();
            }
        }

        private void ItemSair_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("Deseja realmente sair do sistema?", "Confirmar Saída",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (resultado == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Eventos do Menu Chamados
        private void ItemNovoChamado_Click(object sender, EventArgs e)
        {
            try
            {
                var formCriarChamado = new CriarChamadoForm(_usuarioLogado, _chamadosController);
                formCriarChamado.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemVisualizarChamados_Click(object sender, EventArgs e)
        {
            try
            {
                var formVisualizarChamados = new VisualizarChamadosForm(_usuarioLogado, _chamadosController);
                formVisualizarChamados.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemGerenciarChamados_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioLogado.NivelAcesso >= 2)
                {
                    var formGerenciarChamados = new GerenciarChamadosForm(_usuarioLogado, _chamadosController, _funcionariosController);
                    formGerenciarChamados.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("Acesso negado. Apenas técnicos e administradores podem gerenciar chamados.",
                        "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Eventos do Menu Usuários
        private void ItemNovoUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioLogado.NivelAcesso >= 3)
                {
                    // ← AGREGADO: Solicitar confirmação de senha do admin
                    if (!ConfirmarSenhaAdmin())
                    {
                        MessageBox.Show("Operação cancelada. Senha incorreta ou operação cancelada.",
                            "Cancelado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    var formNovoUsuario = new NovoUsuarioForm(_funcionariosController);
                    if (formNovoUsuario.ShowDialog(this) == DialogResult.OK)
                    {
                        MessageBox.Show("Usuário criado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Acesso negado. Apenas administradores podem criar usuários.",
                        "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemGerenciarUsuarios_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioLogado.NivelAcesso >= 3)
                {
                    var formGerenciarUsuarios = new GerenciarUsuariosForm(_usuarioLogado, _funcionariosController);
                    formGerenciarUsuarios.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("Acesso negado. Apenas administradores podem gerenciar usuários.",
                        "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Eventos do Menu Relatórios
        private void ItemRelatorioChamados_Click(object sender, EventArgs e)
        {
            try
            {
                var formRelatorios = new RelatoriosForm(_funcionarioLogado, _chamadosController, _funcionariosController);
                formRelatorios.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir relatórios: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemEstatisticas_Click(object sender, EventArgs e)
        {
            try
            {
                var formRelatorios = new RelatoriosForm(_funcionarioLogado, _chamadosController, _funcionariosController);
                formRelatorios.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir estatísticas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Eventos do Menu Ajuda
        private void ItemManual_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Manual do usuário será disponibilizado em breve.\n\n" +
                "Para dúvidas, entre em contato com o suporte técnico.",
                "Manual do Usuário", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ItemSobre_Click(object sender, EventArgs e)
        {
            string sobre = "Sistema de Chamados v1.0.0\n\n" +
                          "Sistema de gerenciamento de chamados de InterFix.\n" +
                          "com integração de IA\n\n" +
                          "Desenvolvido em C# com Windows Forms\n" +
                          "Banco de dados: SQL Server\n\n" +
                          "© 2025 - Todos os direitos reservados";
                          
            MessageBox.Show(sobre, "Sobre o Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Eventos do Form
        private void MenuPrincipalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var resultado = MessageBox.Show("Deseja realmente sair do sistema?", "Confirmar Saída",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                if (resultado == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void MenuPrincipalForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Atalhos globais
            if (e.Control && e.KeyCode == Keys.Q)
            {
                ItemSair_Click(sender, e);
            }
        }

        // Método para abrir formulário MDI Child (opcional para futuro)
        private void AbrirFormulario(Form formulario)
        {
            formulario.MdiParent = this;
            formulario.Show();
        }

        // Método para fechar todos os formulários filhos (opcional)
        private void FecharTodosFormularios()
        {
            foreach (Form form in this.MdiChildren)
            {
                form.Close();
            }
        }

        // Métodos para criar cards do Dashboard
        private Panel CriarCardInfo(string titulo, string conteudo, Color corTitulo, Point localizacao)
        {
            var panel = new Panel();
            panel.Size = new Size(320, 140);
            panel.Location = localizacao;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.White;

            var lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.ForeColor = corTitulo;
            lblTitulo.Location = new Point(15, 15);
            lblTitulo.AutoSize = true;

            var lblConteudo = new Label();
            lblConteudo.Text = conteudo;
            lblConteudo.Font = new Font("Segoe UI", 10F);
            lblConteudo.Location = new Point(15, 45);
            lblConteudo.Size = new Size(290, 80);
            lblConteudo.ForeColor = Color.FromArgb(73, 80, 87);

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblConteudo);

            return panel;
        }

        private Panel CriarCardAtalhos(Point localizacao)
        {
            var panel = new Panel();
            panel.Size = new Size(320, 140);
            panel.Location = localizacao;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.White;

            var lblTitulo = new Label();
            lblTitulo.Text = "Atalhos Rápidos";
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(220, 53, 69);
            lblTitulo.Location = new Point(15, 15);
            lblTitulo.AutoSize = true;

            var lblAtalhos = new Label();
            string atalhos = "Ctrl+N - Novo Chamado\n" +
                           "Ctrl+V - Visualizar Chamados\n" +
                           "Ctrl+G - Gerenciar Chamados\n" +
                           "Ctrl+L - Logout\n" +
                           "F1 - Ajuda";
            lblAtalhos.Text = atalhos;
            lblAtalhos.Font = new Font("Segoe UI", 9F);
            lblAtalhos.Location = new Point(15, 45);
            lblAtalhos.Size = new Size(290, 80);
            lblAtalhos.ForeColor = Color.FromArgb(73, 80, 87);

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblAtalhos);

            return panel;
        }

        private Panel CriarCardEstatisticas(Point localizacao)
        {
            var panel = new Panel();
            panel.Size = new Size(320, 140);
            panel.Location = localizacao;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.White;

            var lblTitulo = new Label();
            lblTitulo.Text = "Estatísticas Rápidas";
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(255, 193, 7);
            lblTitulo.Location = new Point(15, 15);
            lblTitulo.AutoSize = true;

            // Obter estatísticas
            try
            {
                var chamados = _chamadosController.ListarTodosChamados();
                int totalChamados = chamados.Count;
                int abertos = chamados.Count(c => (int)c.Status == 1);
                int emAndamento = chamados.Count(c => (int)c.Status == 2);
                int resolvidos = chamados.Count(c => (int)c.Status == 3);

                var lblStats = new Label();
                lblStats.Text = $"Total de Chamados: {totalChamados}\n" +
                              $"Abertos: {abertos}\n" +
                              $"Em Andamento: {emAndamento}\n" +
                              $"Resolvidos: {resolvidos}";
                lblStats.Font = new Font("Segoe UI", 10F);
                lblStats.Location = new Point(15, 45);
                lblStats.Size = new Size(290, 80);
                lblStats.ForeColor = Color.FromArgb(73, 80, 87);

                panel.Controls.Add(lblStats);
            }
            catch
            {
                var lblErro = new Label();
                lblErro.Text = "Não foi possível\ncarregar estatísticas";
                lblErro.Font = new Font("Segoe UI", 10F);
                lblErro.Location = new Point(15, 45);
                lblErro.Size = new Size(290, 80);
                lblErro.ForeColor = Color.Gray;
                panel.Controls.Add(lblErro);
            }

            panel.Controls.Add(lblTitulo);

            return panel;
        }

        private bool ConfirmarSenhaAdmin()
        {
            var formConfirmar = new ConfirmarSenhaAdminForm(_funcionarioLogado);
            return formConfirmar.ShowDialog() == DialogResult.OK;
        }
    }
}
