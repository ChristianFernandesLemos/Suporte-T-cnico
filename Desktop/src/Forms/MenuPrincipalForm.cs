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

        // Status bar
        private StatusStrip statusBar;
        private ToolStripStatusLabel lblDataHora;
        private ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Timer timerRelogio;
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
                AutoScroll = true,
                Padding = new Padding(0, 10, 0, 10)
            };

            // 🎯 CONSTRUIR MENU DINÁMICO POR NIVEL DE ACCESO
            ConstruirMenuSidebarPorNivel();

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

        /// <summary>
        /// 🎯 Construir sidebar dinámicamente según nivel de acceso
        /// </summary>
        private void ConstruirMenuSidebarPorNivel()
        {
            panelMenu.Controls.Clear();
            int yPos = 10;

            // ========================================
            // BOTÓN DASHBOARD - TODOS
            // ========================================
            var btnDashboard = CriarBotaoMenu("🏠  Dashboard", yPos);
            btnDashboard.Click += (s, e) => MostrarDashboard();
            yPos += 55;

            // ========================================
            // MENÚ SEGÚN NIVEL DE ACCESO
            // ========================================
            switch (_usuarioLogado.NivelAcesso)
            {
                case 1: // 👤 FUNCIONÁRIO
                    yPos = ConstruirMenuFuncionario(yPos);
                    break;

                case 2: // 🔧 TÉCNICO
                    yPos = ConstruirMenuTecnico(yPos);
                    break;

                case 3: // 👑 ADMINISTRADOR
                    yPos = ConstruirMenuAdministrador(yPos);
                    break;
            }

            // ========================================
            // SEÇÃO AJUDA - TODOS
            // ========================================
            var lblAjuda = CriarLabelSecao("AJUDA", yPos);
            yPos += 30;

            var btnManual = CriarBotaoMenu("📖  Manual do Usuário", yPos);
            btnManual.Click += (s, e) => ItemManual_Click(s, e);
            yPos += 55;

            // ========================================
            // SEÇÃO SISTEMA - TODOS
            // ========================================
            var lblSistema = CriarLabelSecao("SISTEMA", yPos);
            yPos += 30;

            // Alterar Senha - SOLO ADMIN
            if (_usuarioLogado.NivelAcesso == 3)
            {
                var btnConfiguracoes = CriarBotaoMenu("🔧  Alterar Senha", yPos);
                btnConfiguracoes.Click += (s, e) => ItemAlterarSenha_Click(s, e);
                yPos += 55;
            }

            // Logout - TODOS
            var btnLogout = CriarBotaoMenu("🔓  Logout", yPos);
            btnLogout.Click += (s, e) => ItemLogout_Click(s, e);
            btnLogout.BackColor = Color.FromArgb(255, 193, 7);
            btnLogout.ForeColor = Color.Black;
            yPos += 55;

            // Sair - TODOS
            var btnSair = CriarBotaoMenu("🚪  Sair do Sistema", yPos);
            btnSair.Click += (s, e) => ItemSair_Click(s, e);
            btnSair.BackColor = Color.FromArgb(220, 53, 69);
        }

        /// <summary>
        /// Menu específico para FUNCIONÁRIO
        /// </summary>
        private int ConstruirMenuFuncionario(int yPos)
        {
            var lblChamados = CriarLabelSecao("CHAMADOS", yPos);
            yPos += 30;

            var btnNovo = CriarBotaoMenu("➕  Novo Chamado", yPos);
            btnNovo.Click += (s, e) => ItemNovoChamado_Click(s, e);
            yPos += 55;

            var btnVisualizar = CriarBotaoMenu("👁️  Meus Chamados", yPos);
            btnVisualizar.Click += (s, e) => ItemVisualizarChamados_Click(s, e);
            yPos += 55;

            return yPos;
        }

        /// <summary>
        /// Menu específico para TÉCNICO
        /// </summary>
        private int ConstruirMenuTecnico(int yPos)
        {
            var lblChamados = CriarLabelSecao("CHAMADOS", yPos);
            yPos += 30;

            var btnVisualizar = CriarBotaoMenu("👁️  Visualizar Chamados", yPos);
            btnVisualizar.Click += (s, e) => ItemVisualizarChamados_Click(s, e);
            yPos += 55;

            var btnGerenciar = CriarBotaoMenu("⚙️  Gerenciar Chamados", yPos);
            btnGerenciar.Click += (s, e) => ItemGerenciarChamados_Click(s, e);
            yPos += 55;

            // Seção Relatórios
            var lblRelatorios = CriarLabelSecao("RELATÓRIOS", yPos);
            yPos += 30;

            var btnRelatorios = CriarBotaoMenu("📊  Relatórios", yPos);
            btnRelatorios.Click += (s, e) => ItemRelatorioChamados_Click(s, e);
            yPos += 55;

            return yPos;
        }

        /// <summary>
        /// Menu específico para ADMINISTRADOR
        /// </summary>
        private int ConstruirMenuAdministrador(int yPos)
        {
            // Seção Chamados
            var lblChamados = CriarLabelSecao("CHAMADOS", yPos);
            yPos += 30;

            var btnNovo = CriarBotaoMenu("➕  Novo Chamado", yPos);
            btnNovo.Click += (s, e) => ItemNovoChamado_Click(s, e);
            yPos += 55;

            var btnVisualizar = CriarBotaoMenu("👁️  Visualizar Chamados", yPos);
            btnVisualizar.Click += (s, e) => ItemVisualizarChamados_Click(s, e);
            yPos += 55;

            var btnGerenciar = CriarBotaoMenu("⚙️  Gerenciar Chamados", yPos);
            btnGerenciar.Click += (s, e) => ItemGerenciarChamados_Click(s, e);
            yPos += 55;

            // Seção Usuários
            var lblUsuarios = CriarLabelSecao("USUÁRIOS", yPos);
            yPos += 30;

            var btnNovoUsuario = CriarBotaoMenu("👤  Novo Usuário", yPos);
            btnNovoUsuario.Click += (s, e) => ItemNovoUsuario_Click(s, e);
            yPos += 55;

            var btnGerenciarUsuarios = CriarBotaoMenu("👥  Gerenciar Usuários", yPos);
            btnGerenciarUsuarios.Click += (s, e) => ItemGerenciarUsuarios_Click(s, e);
            yPos += 55;

            // Seção Relatórios
            var lblRelatorios = CriarLabelSecao("RELATÓRIOS", yPos);
            yPos += 30;

            var btnRelatorios = CriarBotaoMenu("📊  Relatórios", yPos);
            btnRelatorios.Click += (s, e) => ItemRelatorioChamados_Click(s, e);
            yPos += 55;

            return yPos;
        }

        #region Métodos Auxiliares de UI

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
            var lbl = new Label
            {
                Text = texto,
                Location = new Point(15, yPos),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.panelMenu.Controls.Add(lbl);
            return lbl;
        }

        private Image CriarAvatarComIniciais(string nome)
        {
            var bitmap = new Bitmap(100, 100);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.FromArgb(0, 123, 255));

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

        #endregion

        #region Dashboard

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

            // Botões de atalho según nivel
            int xPosAtalho = 40;

            if (_usuarioLogado.NivelAcesso == 1 || _usuarioLogado.NivelAcesso == 3)
            {
                var btnAtalhoNovo = CriarBotaoAtalho("➕ Novo Chamado", new Point(xPosAtalho, yPos));
                btnAtalhoNovo.Click += (s, e) => ItemNovoChamado_Click(s, e);
                panelPrincipal.Controls.Add(btnAtalhoNovo);
                xPosAtalho += 240;
            }

            var btnAtalhoVisualizar = CriarBotaoAtalho("👁️ Ver Chamados", new Point(xPosAtalho, yPos));
            btnAtalhoVisualizar.Click += (s, e) => ItemVisualizarChamados_Click(s, e);
            panelPrincipal.Controls.Add(btnAtalhoVisualizar);
            xPosAtalho += 240;

            if (_usuarioLogado.NivelAcesso >= 2)
            {
                var btnAtalhoGerenciar = CriarBotaoAtalho("⚙️ Gerenciar", new Point(xPosAtalho, yPos));
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

        #endregion

        #region Eventos del Menú

        private void ItemNovoChamado_Click(object sender, EventArgs e)
        {
            try
            {
                var formCriarChamado = new CriarChamadoForm(_usuarioLogado, _chamadosController);
                formCriarChamado.ShowDialog(this);
                MostrarDashboard();
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

        private void ItemManual_Click(object sender, EventArgs e)
        {
            try
            {
                string manualPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Manual_Usuario.pdf");

                if (System.IO.File.Exists(manualPath))
                {
                    // Abrir el PDF com el programa predeterminado
                    System.Diagnostics.Process.Start(manualPath);
                }
                else
                {
                    // Mostrar manual de ayuda integrado
                    var formManual = new ManualUsuarioForm(_usuarioLogado.NivelAcesso);
                    formManual.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Manual do usuário em breve.\n\n" +
                    "Para dúvidas, entre em contato com o suporte técnico.\n\n" +
                    $"Erro: {ex.Message}",
                    "Manual do Usuário",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
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
                if (timerRelogio != null)
                {
                    timerRelogio.Stop();
                    timerRelogio.Dispose();
                }

                var loginForm = new LoginForm();
                loginForm.FormClosed += (s, args) => this.Dispose();
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

        #endregion

        #region Métodos de Suporte

        private void ConfigurarFormulario()
        {
            TimerRelogio_Tick(null, null);
        }

        private string ObterTextoNivelAcesso(int nivel)
        {
            if (nivel == 1)
                return "Funcionário";
            else if (nivel == 2)
                return "Técnico";
            else if (nivel == 3)
                return "Administrador";
            else
                return "Desconhecido";
        }

        private void TimerRelogio_Tick(object sender, EventArgs e)
        {
            lblDataHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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

        #endregion
    }

    #region Formulario Manual de Usuario

    /// <summary>
    /// Formulário integrado para mostrar manual do usuário
    /// </summary>
    public class ManualUsuarioForm : Form
    {
        private int _nivelAcesso;
        private TabControl tabControl;
        private Button btnFechar;

        public ManualUsuarioForm(int nivelAcesso)
        {
            _nivelAcesso = nivelAcesso;
            InitializeComponent();
            CarregarConteudo();
        }

        private void InitializeComponent()
        {
            this.Text = "Manual do Usuário - InterFix";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Header
            var lblTitulo = new Label
            {
                Text = "📖 Manual do Usuário",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(400, 35),
                ForeColor = Color.FromArgb(0, 123, 255)
            };
            this.Controls.Add(lblTitulo);

            // TabControl
            tabControl = new TabControl
            {
                Location = new Point(20, 70),
                Size = new Size(840, 480),
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(tabControl);

            // Botão Fechar
            btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(760, 560),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.Click += (s, e) => this.Close();
            this.Controls.Add(btnFechar);
        }

        private void CarregarConteudo()
        {
            // Aba: Primeiros Passos
            var tabInicio = new TabPage("🚀 Início");
            tabInicio.BackColor = Color.White;
            tabInicio.Controls.Add(CriarPanelConteudo(ObterConteudoInicio()));
            tabControl.TabPages.Add(tabInicio);

            // Aba: Como Criar Chamado (Funcionário e Admin)
            if (_nivelAcesso == 1 || _nivelAcesso == 3)
            {
                var tabCriar = new TabPage("➕ Criar Chamado");
                tabCriar.BackColor = Color.White;
                tabCriar.Controls.Add(CriarPanelConteudo(ObterConteudoCriarChamado()));
                tabControl.TabPages.Add(tabCriar);
            }

            // Aba: Gerenciar Chamados (Técnico e Admin)
            if (_nivelAcesso >= 2)
            {
                var tabGerenciar = new TabPage("⚙️ Gerenciar");
                tabGerenciar.BackColor = Color.White;
                tabGerenciar.Controls.Add(CriarPanelConteudo(ObterConteudoGerenciar()));
                tabControl.TabPages.Add(tabGerenciar);
            }

            // Aba: Usuários (Solo Admin)
            if (_nivelAcesso == 3)
            {
                var tabUsuarios = new TabPage("👥 Usuários");
                tabUsuarios.BackColor = Color.White;
                tabUsuarios.Controls.Add(CriarPanelConteudo(ObterConteudoUsuarios()));
                tabControl.TabPages.Add(tabUsuarios);
            }

            // Aba: FAQ
            var tabFAQ = new TabPage("❓ Perguntas Frequentes");
            tabFAQ.BackColor = Color.White;
            tabFAQ.Controls.Add(CriarPanelConteudo(ObterConteudoFAQ()));
            tabControl.TabPages.Add(tabFAQ);
        }

        private Panel CriarPanelConteudo(string conteudo)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };

            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Text = conteudo
            };

            panel.Controls.Add(richTextBox);
            return panel;
        }

        private string ObterConteudoInicio()
        {
            return @"BEM-VINDO AO SISTEMA INTERFIX!

Este é o Sistema de Gerenciamento de Chamados da InterFix.

📋 O QUE É O SISTEMA?
O sistema permite que você registre e acompanhe problemas técnicos, solicitações de suporte e manutenções.

👤 SEU NÍVEL DE ACESSO: " + ObterTextoNivel() + @"

🎯 FUNCIONALIDADES DISPONÍVEIS:
" + ObterFuncionalidadesDisponiveis() + @"

⌨️ ATALHOS DE TECLADO:
• Dashboard: Clique no botão 🏠 na barra lateral
• F1: Abrir este manual
• Alt+F4: Fechar o sistema

💡 DICA:
Use a barra lateral à esquerda para navegar entre as diferentes seções do sistema.

Para mais informações sobre cada funcionalidade, navegue pelas abas acima.";
        }

        private string ObterConteudoCriarChamado()
        {
            return @"COMO CRIAR UM NOVO CHAMADO

📝 PASSO A PASSO:

1. ACESSAR CRIAÇÃO
   • Clique em '➕ Novo Chamado' na barra lateral
   • Ou use o atalho rápido no Dashboard

2. ETAPA 1 - DESCRIÇÃO DO PROBLEMA
   • Digite um título claro (ex: ""Impressora não funciona"")
   • Selecione a categoria: Hardware, Software, Rede ou Outros
   • Descreva detalhadamente o problema (mínimo 20 caracteres)

3. ETAPA 2 - QUEM É AFETADO
   • Apenas eu: Problema afeta só você
   • Meu departamento: Afeta seu setor
   • Empresa toda: Problema generalizado

4. ETAPA 3 - IMPACTO NO TRABALHO
   • Sim, não consigo trabalhar: Alta prioridade
   • Não, consigo trabalhar: Prioridade normal

5. CONFIRMAÇÃO
   • Revise as informações
   • Clique em 'Concluir'
   • Anote o número do chamado gerado

📊 PRIORIDADES (calculadas automaticamente):
• Baixa: Não impede trabalho, afeta só você
• Média: Não impede, mas afeta departamento
• Alta: Impede trabalho do departamento
• Crítica: Impede trabalho da empresa toda

✅ DEPOIS DE CRIAR:
• Você receberá um número de protocolo
• Pode acompanhar o status em 'Ver Meus Chamados'
• Receberá notificações de atualizações";
        }

        private string ObterConteudoGerenciar()
        {
            return @"GERENCIAMENTO DE CHAMADOS (TÉCNICO/ADMIN)

⚙️ FUNCIONALIDADES:

1. VISUALIZAR CHAMADOS
   • Ver todos os chamados do sistema
   • Filtrar por status, prioridade, técnico
   • Buscar por palavra-chave

2. ATRIBUIR CHAMADO
   • Selecione o chamado
   • Clique em 'Atribuir Técnico'
   • Escolha o técnico responsável
   • Status muda para 'Em Andamento'

3. ALTERAR PRIORIDADE
   • Selecione o chamado
   • Clique em 'Alterar Prioridade'
   • Escolha: Baixa, Média, Alta ou Crítica

4. RESOLVER CHAMADO
   • Selecione o chamado
   • Clique em 'Marcar como Resolvido'
   • Adicione a solução aplicada
   • Status muda para 'Resolvido'

5. FECHAR CHAMADO
   • Após resolver, aguarde confirmação
   • Clique em 'Fechar Chamado'
   • Status muda para 'Fechado'

📋 STATUS DOS CHAMADOS:
• Aberto: Aguardando atribuição
• Em Andamento: Técnico está trabalhando
• Resolvido: Problema foi solucionado
• Fechado: Chamado finalizado
• Cancelado: Chamado foi cancelado

🎯 BOAS PRÁTICAS:
• Sempre adicione comentários ao resolver
• Mantenha os chamados atualizados
• Priorize chamados críticos
• Comunique-se com o solicitante";
        }

        private string ObterConteudoUsuarios()
        {
            return @"GERENCIAMENTO DE USUÁRIOS (ADMINISTRADOR)

👥 FUNCIONALIDADES ADMINISTRATIVAS:

1. CRIAR NOVO USUÁRIO
   • Clique em '👤 Novo Usuário'
   • Confirme sua senha de administrador
   • Preencha os dados do novo usuário
   • Defina o nível de acesso:
     - Funcionário: Pode criar e ver seus chamados
     - Técnico: Pode gerenciar chamados
     - Administrador: Acesso completo

2. EDITAR USUÁRIO
   • Em 'Gerenciar Usuários', selecione o usuário
   • Clique em 'Editar'
   • Confirme sua senha
   • Altere os dados necessários

3. ALTERAR NÍVEL DE ACESSO
   • Selecione o usuário
   • Clique em 'Alterar Nível'
   • Confirme sua senha
   • Escolha o novo nível

4. ALTERAR SENHA DE USUÁRIO
   • Selecione o usuário
   • Clique em 'Alterar Senha'
   • Confirme sua senha de admin
   • Digite a nova senha do usuário

5. ATIVAR/DESATIVAR USUÁRIO
   • Selecione o usuário
   • Clique em 'Ativar/Desativar'
   • Confirme a ação

6. EXCLUIR USUÁRIO
   • Selecione o usuário
   • Clique em 'Excluir'
   • ATENÇÃO: Ação irreversível!
   • Confirme sua senha

⚠️ RESTRIÇÕES DE SEGURANÇA:
• Não pode alterar seu próprio nível
• Não pode desativar sua própria conta
• Não pode excluir sua própria conta
• Todas as ações críticas requerem confirmação de senha

🔒 NÍVEIS DE ACESSO:
• Funcionário (Nível 1): Criar e visualizar próprios chamados
• Técnico (Nível 2): Gerenciar todos os chamados
• Administrador (Nível 3): Controle total do sistema";
        }

        private string ObterConteudoFAQ()
        {
            return @"PERGUNTAS FREQUENTES (FAQ)

❓ COMO FAÇO LOGIN?
Use seu e-mail corporativo e a senha fornecida pelo administrador.

❓ ESQUECI MINHA SENHA
Entre em contato com o administrador do sistema.

❓ QUANTO TEMPO LEVA PARA RESOLVER UM CHAMADO?
Depende da prioridade:
• Crítica: Até 4 horas
• Alta: Até 1 dia útil
• Média: Até 3 dias úteis
• Baixa: Até 1 semana

❓ POSSO CANCELAR UM CHAMADO?
Sim, entre em contato com o técnico responsável ou administrador.

❓ COMO ACOMPANHO MEU CHAMADO?
Acesse 'Ver Meus Chamados' na barra lateral.

❓ POSSO CRIAR CHAMADO PARA OUTRA PESSOA?
Não, cada usuário deve criar seus próprios chamados.

❓ O QUE FAZER SE O PROBLEMA PERSISTIR?
Adicione uma contestação ao chamado ou crie um novo chamado relacionado.

❓ COMO ALTERO MINHA SENHA?
Apenas administradores podem alterar senhas no menu 'Alterar Senha'.

❓ POSSO VER CHAMADOS DE OUTRAS PESSOAS?
• Funcionário: Não, apenas seus próprios
• Técnico: Sim, todos os chamados
• Admin: Sim, todos os chamados

❓ PRECISO DE TREINAMENTO?
Este manual contém todas as informações necessárias. Para dúvidas específicas, contate o suporte.

📞 SUPORTE TÉCNICO:
• E-mail: suporte@interfix.com
• Telefone: (12) 3456-7890
• Horário: Segunda a Sexta, 8h às 18h";
        }

        private string ObterTextoNivel()
        {
            switch (_nivelAcesso)
            {
                case 1:
                    return "Funcionário";
                case 2:
                    return "Técnico";
                case 3:
                    return "Administrador";
                default:
                    return "Desconhecido";
            }
        }

        private string ObterFuncionalidadesDisponiveis()
        {
            if (_nivelAcesso == 1)
                return "• Criar novos chamados\n• Visualizar seus chamados\n• Adicionar contestações";
            else if (_nivelAcesso == 2)
                return "• Visualizar todos os chamados\n• Gerenciar chamados\n• Atribuir técnicos\n• Resolver chamados\n• Gerar relatórios";
            else if (_nivelAcesso == 3)
                return "• Todas as funcionalidades de Técnico\n• Criar e gerenciar usuários\n• Alterar senhas\n• Configurações do sistema";
            else
                return "Nenhuma funcionalidade disponível";
        }
    }

    #endregion
}