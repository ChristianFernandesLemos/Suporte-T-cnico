using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SistemaChamados.Config;
using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Forms;
using SistemaChamados.Data;

namespace SistemaChamados.Forms
{
    public partial class MenuPrincipalForm : Form
    {
        private Funcionarios _usuarioLogado;
        private FuncionariosController _funcionariosController;
        private ChamadosController _chamadosController;

        // Componentes do Sidebar
        private Panel panelSidebar;
        private Panel panelHeader;
        private Panel panelMenu;
        private Panel panelFooter;
        private Label lblNomeUsuario;
        private Label lblTipoUsuario;
        private PictureBox picAvatar;

        // Botões do menu
        private Button btnDashboard;
        private Button btnNovoChamado;
        private Button btnVisualizarChamados;
        private Button btnGerenciarChamados;
        private Button btnNovoUsuario;
        private Button btnGerenciarUsuarios;
        private Button btnRelatorios;
        private Button btnConfiguracoes;
        private Button btnSair;
        private Button btnLogout;
        // Status bar
        private StatusStrip statusBar;
        private ToolStripStatusLabel lblDataHora;
        private ToolStripStatusLabel lblStatus;

        // Timer
        private System.Windows.Forms.Timer timerRelogio;

        // Panel principal
        private Panel panelPrincipal;

        public MenuPrincipalForm(Funcionarios usuarioLogado)
        {
            _usuarioLogado = usuarioLogado;
            var connectionString = DatabaseConfig.ConnectionString;
            var database = new SqlServerConnection(connectionString);
            _funcionariosController = new FuncionariosController(database);
            _chamadosController = new ChamadosController(database);

            InitializeComponent();
            ConfigurarFormulario();
            ConfigurarPermissoes();
            MostrarDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistema de Chamados - InterFix";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.White;

            // ========================================
            // SIDEBAR
            // ========================================
            this.panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(32, 33, 36)
            };

            // Header do Sidebar (Avatar e Info do Usuário)
            this.panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.FromArgb(41, 42, 45)
            };

            this.picAvatar = new PictureBox
            {
                Location = new Point(75, 20),
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle
            };
            // Crear avatar con iniciales
            this.picAvatar.Image = CriarAvatarComIniciais(_usuarioLogado.Nome);

            this.lblNomeUsuario = new Label
            {
                Text = _usuarioLogado.Nome,
                Location = new Point(10, 125),
                Size = new Size(230, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.panelHeader.Controls.Add(picAvatar);
            this.panelHeader.Controls.Add(lblNomeUsuario);

            // Menu do Sidebar
            this.panelMenu = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(32, 33, 36),
                AutoScroll = true
            };

            int yPos = 20;

            // Botão Dashboard
            this.btnDashboard = CriarBotaoMenu("🏠  Dashboard", yPos);
            this.btnDashboard.Click += (s, e) => MostrarDashboard();
            yPos += 55;

            // Seção: Chamados
            var lblChamados = CriarLabelSecao("CHAMADOS", yPos);
            this.panelMenu.Controls.Add(lblChamados);
            yPos += 30;

            this.btnNovoChamado = CriarBotaoMenu("➕  Novo Chamado", yPos);
            this.btnNovoChamado.Click += (s, e) => ItemNovoChamado_Click(s, e);
            yPos += 55;

            this.btnVisualizarChamados = CriarBotaoMenu("👁️  Visualizar Chamados", yPos);
            this.btnVisualizarChamados.Click += (s, e) => ItemVisualizarChamados_Click(s, e);
            yPos += 55;

            this.btnGerenciarChamados = CriarBotaoMenu("⚙️  Gerenciar Chamados", yPos);
            this.btnGerenciarChamados.Click += (s, e) => ItemGerenciarChamados_Click(s, e);
            yPos += 55;

            // Seção: Usuários
            var lblUsuarios = CriarLabelSecao("USUÁRIOS", yPos);
            this.panelMenu.Controls.Add(lblUsuarios);
            yPos += 30;

            this.btnNovoUsuario = CriarBotaoMenu("👤  Novo Usuário", yPos);
            this.btnNovoUsuario.Click += (s, e) => ItemNovoUsuario_Click(s, e);
            yPos += 55;

            this.btnGerenciarUsuarios = CriarBotaoMenu("👥  Gerenciar Usuários", yPos);
            this.btnGerenciarUsuarios.Click += (s, e) => ItemGerenciarUsuarios_Click(s, e);
            yPos += 55;

            // Seção: Relatórios
            var lblRelatorios = CriarLabelSecao("RELATÓRIOS", yPos);
            this.panelMenu.Controls.Add(lblRelatorios);
            yPos += 30;

            this.btnRelatorios = CriarBotaoMenu("📊  Relatórios", yPos);
            this.btnRelatorios.Click += (s, e) => ItemRelatorioChamados_Click(s, e);
            yPos += 55;

            // Seção: Sistema
            var lblSistema = CriarLabelSecao("SISTEMA", yPos);
            this.panelMenu.Controls.Add(lblSistema);
            yPos += 30;

            this.btnConfiguracoes = CriarBotaoMenu("🔧  Alterar Senha", yPos);
            this.btnConfiguracoes.Click += (s, e) => ItemAlterarSenha_Click(s, e);
            yPos += 55;

            // Botão Logout (separado de Sair)
            this.btnLogout = CriarBotaoMenu("🔓  Logout", yPos);
            this.btnLogout.Click += (s, e) => ItemLogout_Click(s, e);
            this.btnLogout.BackColor = Color.FromArgb(255, 193, 7);
            yPos += 55;

            this.btnSair = CriarBotaoMenu("🚪  Sair do Sistema", yPos);
            this.btnSair.Click += (s, e) => ItemSair_Click(s, e);
            this.btnSair.BackColor = Color.FromArgb(220, 53, 69);

            // Footer do Sidebar
            this.panelFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(41, 42, 45)
            };

            this.lblTipoUsuario = new Label
            {
                Text = $"🔑 {ObterTextoNivelAcesso(_usuarioLogado.NivelAcesso)}",
                Location = new Point(10, 10),
                Size = new Size(230, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 200, 255),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblVersao = new Label
            {
                Text = "v1.0.0 - InterFix",
                Location = new Point(10, 35),
                Size = new Size(230, 15),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.panelFooter.Controls.Add(lblTipoUsuario);
            this.panelFooter.Controls.Add(lblVersao);

            this.panelSidebar.Controls.Add(panelMenu);
            this.panelSidebar.Controls.Add(panelHeader);
            this.panelSidebar.Controls.Add(panelFooter);

            // ========================================
            // STATUS BAR
            // ========================================
            this.statusBar = new StatusStrip
            {
                BackColor = Color.FromArgb(240, 240, 240)
            };

            this.lblStatus = new ToolStripStatusLabel
            {
                Text = "Sistema operando normalmente",
                BorderSides = ToolStripStatusLabelBorderSides.Right
            };

            this.lblDataHora = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight
            };

            this.statusBar.Items.AddRange(new ToolStripItem[] {
                this.lblStatus,
                this.lblDataHora
            });

            // ========================================
            // PANEL PRINCIPAL (Conteúdo)
            // ========================================
            this.panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                AutoScroll = true
            };

            // Timer
            this.timerRelogio = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            this.timerRelogio.Tick += TimerRelogio_Tick;
            this.timerRelogio.Start();

            // Adicionar ao Form
            this.Controls.Add(panelPrincipal);
            this.Controls.Add(statusBar);
            this.Controls.Add(panelSidebar);

            this.FormClosing += MenuPrincipalForm_FormClosing;
        }

        private Button CriarBotaoMenu(string texto, int yPos)
        {
            var btn = new Button
            {
                Text = texto,
                Location = new Point(10, yPos),
                Size = new Size(230, 45),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                Padding = new Padding(15, 0, 0, 0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 52, 55);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 62, 65);

            this.panelMenu.Controls.Add(btn);
            return btn;
        }

        private Label CriarLabelSecao(string texto, int yPos)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(15, yPos),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Image CriarAvatarComIniciais(string nome)
        {
            var bitmap = new Bitmap(100, 100);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Fundo
                graphics.Clear(Color.FromArgb(0, 123, 255));

                // Iniciais
                string iniciais = ObterIniciais(nome);
                using (var font = new Font("Segoe UI", 32F, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString(iniciais, font, brush, new RectangleF(0, 0, 100, 100), sf);
                }
            }
            return bitmap;
        }

        private string ObterIniciais(string nome)
        {
            var partes = nome.Split(' ');
            if (partes.Length >= 2)
                return $"{partes[0][0]}{partes[1][0]}".ToUpper();
            return nome.Length >= 2 ? nome.Substring(0, 2).ToUpper() : nome.ToUpper();
        }

        private void ConfigurarFormulario()
        {
            TimerRelogio_Tick(null, null);
        }

        private void ConfigurarPermissoes()
        {
            switch (_usuarioLogado.NivelAcesso)
            {
                case 1: // Funcionário
                    btnNovoUsuario.Visible = false;
                    btnGerenciarUsuarios.Visible = false;
                    btnRelatorios.Visible = false;
                    btnGerenciarChamados.Visible = false;
                    btnConfiguracoes.Visible = false;
                    break;

                case 2: // Técnico
                    btnNovoUsuario.Visible = false;
                    btnGerenciarUsuarios.Visible = false;
                    btnNovoChamado.Visible = false;
                    btnConfiguracoes.Visible = false;
                    break;

                case 3: // Administrador
                    // Acesso completo
                    break;
            }
        }

        private void MostrarDashboard()
        {
            panelPrincipal.Controls.Clear();

            // Título
            var lblTitulo = new Label
            {
                Text = $"Bem-vindo, {_usuarioLogado.Nome}!",
                Location = new Point(40, 30),
                Size = new Size(600, 35),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 33, 36)
            };
            panelPrincipal.Controls.Add(lblTitulo);

            var lblSubtitulo = new Label
            {
                Text = $"Nível de acesso: {ObterTextoNivelAcesso(_usuarioLogado.NivelAcesso)}",
                Location = new Point(40, 75),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.Gray
            };
            panelPrincipal.Controls.Add(lblSubtitulo);

            // Cards de estatísticas
            int xPos = 40;
            int yPos = 130;

            try
            {
                var chamados = _chamadosController.ListarTodosChamados();

                // Card 1: Total
                var cardTotal = CriarCardDashboard(
                    "Total de Chamados",
                    chamados.Count.ToString(),
                    Color.FromArgb(0, 123, 255),
                    new Point(xPos, yPos)
                );
                panelPrincipal.Controls.Add(cardTotal);
                xPos += 260;

                // Card 2: Abertos
                int abertos = chamados.Count(c => (int)c.Status == 1);
                var cardAbertos = CriarCardDashboard(
                    "Chamados Abertos",
                    abertos.ToString(),
                    Color.FromArgb(255, 193, 7),
                    new Point(xPos, yPos)
                );
                panelPrincipal.Controls.Add(cardAbertos);
                xPos += 260;

                // Card 3: Em Andamento
                int emAndamento = chamados.Count(c => (int)c.Status == 2);
                var cardAndamento = CriarCardDashboard(
                    "Em Andamento",
                    emAndamento.ToString(),
                    Color.FromArgb(23, 162, 184),
                    new Point(xPos, yPos)
                );
                panelPrincipal.Controls.Add(cardAndamento);
                xPos += 260;

                // Card 4: Resolvidos
                int resolvidos = chamados.Count(c => (int)c.Status == 3 || (int)c.Status == 4);
                var cardResolvidos = CriarCardDashboard(
                    "Resolvidos",
                    resolvidos.ToString(),
                    Color.FromArgb(40, 167, 69),
                    new Point(xPos, yPos)
                );
                panelPrincipal.Controls.Add(cardResolvidos);
            }
            catch
            {
                var lblErro = new Label
                {
                    Text = "Erro ao carregar estatísticas",
                    Location = new Point(40, yPos),
                    Size = new Size(300, 30),
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.Red
                };
                panelPrincipal.Controls.Add(lblErro);
            }

            // Atalhos rápidos
            yPos += 180;
            var lblAtalhos = new Label
            {
                Text = "⚡ Atalhos Rápidos",
                Location = new Point(40, yPos),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 33, 36)
            };
            panelPrincipal.Controls.Add(lblAtalhos);
            yPos += 50;

            // Botões de atalho
            if (_usuarioLogado.NivelAcesso == 1 || _usuarioLogado.NivelAcesso == 3)
            {
                var btnAtalhoNovo = CriarBotaoAtalho("➕ Novo Chamado", new Point(40, yPos));
                btnAtalhoNovo.Click += (s, e) => ItemNovoChamado_Click(s, e);
                panelPrincipal.Controls.Add(btnAtalhoNovo);
            }

            var btnAtalhoVisualizar = CriarBotaoAtalho("👁️ Ver Meus Chamados", new Point(280, yPos));
            btnAtalhoVisualizar.Click += (s, e) => ItemVisualizarChamados_Click(s, e);
            panelPrincipal.Controls.Add(btnAtalhoVisualizar);

            if (_usuarioLogado.NivelAcesso >= 2)
            {
                var btnAtalhoGerenciar = CriarBotaoAtalho("⚙️ Gerenciar", new Point(520, yPos));
                btnAtalhoGerenciar.Click += (s, e) => ItemGerenciarChamados_Click(s, e);
                panelPrincipal.Controls.Add(btnAtalhoGerenciar);
            }

            lblStatus.Text = "Dashboard carregado";
        }

        private Panel CriarCardDashboard(string titulo, string valor, Color cor, Point localizacao)
        {
            var panel = new Panel
            {
                Location = localizacao,
                Size = new Size(240, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Barra colorida no topo
            var barraTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = cor
            };
            panel.Controls.Add(barraTop);

            var lblTitulo = new Label
            {
                Text = titulo,
                Location = new Point(15, 20),
                Size = new Size(210, 25),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray
            };
            panel.Controls.Add(lblTitulo);

            var lblValor = new Label
            {
                Text = valor,
                Location = new Point(15, 50),
                Size = new Size(210, 45),
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = cor
            };
            panel.Controls.Add(lblValor);

            return panel;
        }

        private Button CriarBotaoAtalho(string texto, Point localizacao)
        {
            var btn = new Button
            {
                Text = texto,
                Location = localizacao,
                Size = new Size(220, 50),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
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

        private void TimerRelogio_Tick(object sender, EventArgs e)
        {
            lblDataHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        // Eventos dos botões
        private void ItemNovoChamado_Click(object sender, EventArgs e)
        {
            try
            {
                var formCriarChamado = new CriarChamadoForm(_usuarioLogado, _chamadosController);
                formCriarChamado.ShowDialog(this);
                MostrarDashboard(); // Atualizar dashboard
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
                MostrarDashboard();
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
                    MostrarDashboard();
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

        private void ItemNovoUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioLogado.NivelAcesso >= 3)
                {
                    if (!ConfirmarSenhaAdmin())
                    {
                        MessageBox.Show("Operação cancelada.", "Cancelado",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ItemRelatorioChamados_Click(object sender, EventArgs e)
        {
            try
            {
                var formRelatorios = new RelatoriosForm(_usuarioLogado, _chamadosController, _funcionariosController);
                formRelatorios.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir relatórios: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemAlterarSenha_Click(object sender, EventArgs e)
        {
            try
            {

                if (!ConfirmarSenhaAdmin())
                {
                    MessageBox.Show("Operação cancelada.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

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
                // Parar o timer
                if (timerRelogio != null)
                {
                    timerRelogio.Stop();
                    timerRelogio.Dispose();
                }

                // Criar novo formulário de login
                var loginForm = new LoginForm();
                
                // Quando o login for fechado, liberar recursos deste formulário
                loginForm.FormClosed += (s, args) => this.Dispose();
                
                // Esconder este formulário e mostrar o login
                this.Hide();
                loginForm.Show();
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

        private bool ConfirmarSenhaAdmin()
        {
            var formConfirmar = new ConfirmarSenhaAdminForm(_usuarioLogado);
            return formConfirmar.ShowDialog() == DialogResult.OK;
        }
    }
}