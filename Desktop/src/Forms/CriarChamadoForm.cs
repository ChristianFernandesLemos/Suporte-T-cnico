using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Helpers;
using SistemaChamados.Services;
using System.Threading.Tasks;

namespace SistemaChamados.Forms
{
    public partial class CriarChamadoForm : Form
    {
        private ChamadosController _chamadosController;
        private Funcionarios _funcionarioLogado;

        // Controles comuns
        private Panel pnlHeader;
        private Label lblTitulo;
        private Label lblEtapa;
        private Panel pnlConteudo;
        private Panel pnlBotoes;
        private Button btnVoltar;
        private Button btnProximo;
        private Button btnCancelar;

        // Etapa 1 - Apresentação do Problema
        private Panel pnlEtapa1;
        private Label lblTituloProblema;
        private TextBox txtTitulo;
        private Label lblCategoriaEtapa1;
        private ComboBox cmbCategoria;
        private Label lblOutraCategoria;
        private TextBox txtOutraCategoria;
        private Label lblDescricaoEtapa1;
        private RichTextBox rtbDescricao;

        // Etapa 2 - Quem é Afetado
        private Panel pnlEtapa2;
        private Label lblPerguntaAfetado;
        private RadioButton rbApenasEu;
        private RadioButton rbMeuDepartamento;
        private RadioButton rbEmpresa;
        private Panel pnlRadioButtons;

        // Etapa 3 - Impede o Trabalho
        private Panel pnlEtapa3;
        private Label lblPerguntaImpede;
        private RadioButton rbImpedeSim;
        private RadioButton rbImpedeNao;
        private Panel pnlRadioImpede;

        // Etapa 4 - Revisão e Contestação
        private Panel pnlEtapa4;
        private Label lblRevisaoChamado;
        private Panel pnlResumo;
        private Label lblPrioridadeCalculada;
        private Label lblPerguntaContestacao;
        private RadioButton rbConcordoPrioridade;
        private RadioButton rbContestoPrioridade;
        private Panel pnlRadioContestacao;
        private Panel pnlContestacaoTexto;
        private Label lblJustificativaContestacao;
        private RichTextBox rtbJustificativaContestacao;

        // Dados do chamado
        private int etapaAtual = 1;
        private string tituloChamado;
        private string categoria;
        private string descricao;
        private string afetado;
        private bool impedeTrabalho;
        private bool contestaPrioridade = false;
        private string justificativaContestacao = "";
        private bool _criandoChamado = false;

        // IA Service
        private IAResponse analiseIA = null;
        private bool analisandoComIA = false;
        private Label lblAnalisandoIA;
        private Panel pnlResultadoIA;
        private Label lblPrioridadeIA;
        private Label lblJustificativaIA;


        public CriarChamadoForm(Funcionarios funcionario, ChamadosController chamadosController)
        {
            _funcionarioLogado = funcionario;
            _chamadosController = chamadosController;
            InitializeComponent();
            ConfigurarFormulario();
            this.FormClosing += CriarChamadoForm_FormClosing;

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Aumentada a altura para 650 para acomodar a etapa 4
            this.ClientSize = new Size(700, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Criar Novo Chamado - Sistema de Chamados";
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(0, 123, 255),
                Padding = new Padding(20)
            };

            lblTitulo = new Label
            {
                Text = "Criar Novo Chamado",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            lblEtapa = new Label
            {
                Text = "Etapa 1 de 4",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 60)
            };

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblEtapa);

            // Painel de Conteúdo com AutoScroll
            pnlConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.White,
                AutoScroll = true
            };

            // Painel de Botões
            pnlBotoes = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 20)
            };

            btnVoltar = new Button
            {
                Text = "← Voltar",
                Size = new Size(120, 40),
                Location = new Point(30, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.Click += BtnVoltar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(120, 40),
                Location = new Point(430, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            btnProximo = new Button
            {
                Text = "Próximo →",
                Size = new Size(120, 40),
                Location = new Point(550, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnProximo.FlatAppearance.BorderSize = 0;
            btnProximo.Click += BtnProximo_Click;

            pnlBotoes.Controls.Add(btnVoltar);
            pnlBotoes.Controls.Add(btnCancelar);
            pnlBotoes.Controls.Add(btnProximo);

            this.Controls.Add(pnlConteudo);
            this.Controls.Add(pnlBotoes);
            this.Controls.Add(pnlHeader);

            this.ResumeLayout(false);
        }

        private void ConfigurarFormulario()
        {
            CriarEtapa1();
            CriarEtapa2();
            CriarEtapa3();
            CriarEtapa4();
            txtTitulo.SetPlaceholder("Ex: Computador não liga");
            MostrarEtapa(1);
        }

        private void CriarEtapa1()
        {
            pnlEtapa1 = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            lblTituloProblema = new Label
            {
                Text = "Título do Problema:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtTitulo = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(600, 30),
                Font = new Font("Segoe UI", 11F)
            };

            lblCategoriaEtapa1 = new Label
            {
                Text = "Categoria:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 100),
                AutoSize = true
            };

            cmbCategoria = new ComboBox
            {
                Location = new Point(20, 130),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategoria.Items.AddRange(new string[] { "Hardware", "Software", "Rede", "Outros..." });
            cmbCategoria.SelectedIndex = 0;
            cmbCategoria.SelectedIndexChanged += CmbCategoria_SelectedIndexChanged;

            lblOutraCategoria = new Label
            {
                Text = "Especifique a categoria:",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                Location = new Point(340, 105),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 100, 100),
                Visible = false
            };

            txtOutraCategoria = new TextBox
            {
                Location = new Point(340, 130),
                Size = new Size(280, 30),
                Font = new Font("Segoe UI", 11F),
                Visible = false,
                MaxLength = 50
            };
            txtOutraCategoria.SetPlaceholder("Digite a categoria...");

            lblDescricaoEtapa1 = new Label
            {
                Text = "Descrição Detalhada do Problema:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 180),
                AutoSize = true
            };

            rtbDescricao = new RichTextBox
            {
                Location = new Point(20, 210),
                Size = new Size(600, 150),
                Font = new Font("Segoe UI", 10F),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            pnlEtapa1.Controls.Add(lblTituloProblema);
            pnlEtapa1.Controls.Add(txtTitulo);
            pnlEtapa1.Controls.Add(lblCategoriaEtapa1);
            pnlEtapa1.Controls.Add(cmbCategoria);
            pnlEtapa1.Controls.Add(lblOutraCategoria);
            pnlEtapa1.Controls.Add(txtOutraCategoria);
            pnlEtapa1.Controls.Add(lblDescricaoEtapa1);
            pnlEtapa1.Controls.Add(rtbDescricao);
        }

        private void CriarEtapa2()
        {
            pnlEtapa2 = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                Visible = false
            };

            lblPerguntaAfetado = new Label
            {
                Text = "Quem está sendo afetado por este problema?",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 40),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlRadioButtons = new Panel
            {
                Location = new Point(150, 120),
                Size = new Size(400, 200),
                BackColor = Color.White
            };

            rbApenasEu = new RadioButton
            {
                Text = "Apenas eu",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(50, 20),
                Size = new Size(300, 40),
                Checked = true,
                Cursor = Cursors.Hand
            };

            rbMeuDepartamento = new RadioButton
            {
                Text = "Meu departamento",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(50, 70),
                Size = new Size(300, 40),
                Cursor = Cursors.Hand
            };

            rbEmpresa = new RadioButton
            {
                Text = "A empresa toda",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(50, 120),
                Size = new Size(300, 40),
                Cursor = Cursors.Hand
            };

            pnlRadioButtons.Controls.Add(rbApenasEu);
            pnlRadioButtons.Controls.Add(rbMeuDepartamento);
            pnlRadioButtons.Controls.Add(rbEmpresa);

            pnlEtapa2.Controls.Add(lblPerguntaAfetado);
            pnlEtapa2.Controls.Add(pnlRadioButtons);
        }

        private void CriarEtapa3()
        {
            pnlEtapa3 = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                Visible = false
            };

            lblPerguntaImpede = new Label
            {
                Text = "Este problema impede o seu trabalho?",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 40),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlRadioImpede = new Panel
            {
                Location = new Point(200, 150),
                Size = new Size(300, 120),
                BackColor = Color.White
            };

            rbImpedeSim = new RadioButton
            {
                Text = "Sim, não consigo trabalhar",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(30, 20),
                Size = new Size(250, 40),
                Checked = false,
                Cursor = Cursors.Hand
            };

            rbImpedeNao = new RadioButton
            {
                Text = "Não, consigo trabalhar",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(30, 70),
                Size = new Size(250, 40),
                Checked = true,
                Cursor = Cursors.Hand
            };

            pnlRadioImpede.Controls.Add(rbImpedeSim);
            pnlRadioImpede.Controls.Add(rbImpedeNao);

            pnlEtapa3.Controls.Add(lblPerguntaImpede);
            pnlEtapa3.Controls.Add(pnlRadioImpede);
        }

        private void CriarEtapa4()
        {
            pnlEtapa4 = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                Visible = false,
                AutoScroll = true  // Importante para cuando el contenido sea grande
            };

            lblRevisaoChamado = new Label
            {
                Text = "Revisão do Chamado",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 10),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ====== PAINEL DE RESUMO (SEM PRIORIDADE CALCULADA) ======
            pnlResumo = new Panel
            {
                Location = new Point(50, 50),
                Size = new Size(600, 100),  // Reducido porque ya no hay prioridad calculada
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPrioridadeCalculada = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(580, 80),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft
            };

            pnlResumo.Controls.Add(lblPrioridadeCalculada);
            // ❌ NO agregar lblPrioridadeDestaque (eliminado)

            // ====== LABEL "ANALISANDO COM IA" ======
            lblAnalisandoIA = new Label
            {
                Location = new Point(50, 165),  // Ajustado
                Size = new Size(600, 40),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "🤖 Analisando com Inteligência Artificial...\nAguarde alguns segundos",
                Visible = false
            };

            // ====== PAINEL DE RESULTADO DA IA - AUMENTADO ======
            pnlResultadoIA = new Panel
            {
                Location = new Point(50, 165),  // Ajustado
                Size = new Size(600, 180),  // ✅ AUMENTADO de 100 para 180
                BackColor = Color.FromArgb(225, 245, 254),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                AutoScroll = false  
            };

            Label lblTituloIA = new Label
            {
                Text = "🤖 Análise da Inteligência Artificial",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(580, 25),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblPrioridadeIA = new Label
            {
                Location = new Point(10, 40),
                Size = new Size(580, 30),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "⚡ Prioridade Sugerida: Carregando...",
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblTituloJustificativa = new Label
            {
                Text = "📝 Justificativa:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(10, 75),
                Size = new Size(580, 20),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            RichTextBox rtbJustificativaIA = new RichTextBox
            {
                Name = "rtbJustificativaIA", 
                Location = new Point(10, 100),
                Size = new Size(580, 70),  
                Font = new Font("Segoe UI", 9F),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(225, 245, 254),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Text = "Aguardando análise..."
            };

            pnlResultadoIA.Controls.Add(lblTituloIA);
            pnlResultadoIA.Controls.Add(lblPrioridadeIA);
            pnlResultadoIA.Controls.Add(lblTituloJustificativa);
            pnlResultadoIA.Controls.Add(rtbJustificativaIA);

            // ====== PERGUNTA SOBRE CONTESTAÇÃO - AJUSTADA ======
            lblPerguntaContestacao = new Label
            {
                Text = "Você concorda com a prioridade sugerida pela IA?",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 360),  // Ajustado
                Size = new Size(600, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlRadioContestacao = new Panel
            {
                Location = new Point(150, 395),  // Ajustado
                Size = new Size(400, 80),
                BackColor = Color.White
            };

            rbConcordoPrioridade = new RadioButton
            {
                Text = "✅ Sim, concordo com a prioridade da IA",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 10),
                Size = new Size(320, 30),
                Checked = true,
                Cursor = Cursors.Hand
            };
            rbConcordoPrioridade.CheckedChanged += RbContestacao_CheckedChanged;

            rbContestoPrioridade = new RadioButton
            {
                Text = "⚠️ Não, desejo contestar a prioridade",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 45),
                Size = new Size(320, 30),
                Cursor = Cursors.Hand,
                ForeColor = Color.FromArgb(220, 53, 69)
            };
            rbContestoPrioridade.CheckedChanged += RbContestacao_CheckedChanged;

            pnlRadioContestacao.Controls.Add(rbConcordoPrioridade);
            pnlRadioContestacao.Controls.Add(rbContestoPrioridade);

            // ====== PAINEL DE CONTESTAÇÃO ======
            pnlContestacaoTexto = new Panel
            {
                Location = new Point(50, 490),
                Size = new Size(600, 150),  // ✅ Simplificado
                BackColor = Color.FromArgb(255, 243, 205),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            lblJustificativaContestacao = new Label
            {
                Text = "⚠️ Justifique por que você contesta a prioridade sugerida pela IA:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(580, 20),
                ForeColor = Color.FromArgb(133, 100, 4)
            };

            rtbJustificativaContestacao = new RichTextBox
            {
                Location = new Point(10, 35),
                Size = new Size(580, 105),  // ✅ Más grande sin el combo
                Font = new Font("Segoe UI", 9F),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            pnlContestacaoTexto.Controls.Add(lblJustificativaContestacao);
            pnlContestacaoTexto.Controls.Add(rtbJustificativaContestacao);


            // ====== AGREGAR TODOS LOS CONTROLES AL PANEL ======
            pnlEtapa4.Controls.Add(lblRevisaoChamado);
            pnlEtapa4.Controls.Add(pnlResumo);
            pnlEtapa4.Controls.Add(lblAnalisandoIA);
            pnlEtapa4.Controls.Add(pnlResultadoIA);
            pnlEtapa4.Controls.Add(lblPerguntaContestacao);
            pnlEtapa4.Controls.Add(pnlRadioContestacao);
            pnlEtapa4.Controls.Add(pnlContestacaoTexto);
        }


        private void RbContestacao_CheckedChanged(object sender, EventArgs e)
        {
            pnlContestacaoTexto.Visible = rbContestoPrioridade.Checked;

            if (rbContestoPrioridade.Checked)
            {
                rtbJustificativaContestacao.Focus();
            }
        }

        private void AtualizarRevisao()
        {
            lblPrioridadeCalculada.Text =
                $"📋 Título: {tituloChamado}\n" +
                $"📁 Categoria: {categoria}\n" +
                $"👥 Afetados: {ObterTextoAfetado()}\n" +
                $"🚨 Impede trabalho: {(impedeTrabalho ? "Sim" : "Não")}";
        }

        private Color ObterCorPrioridade(int prioridade)
        {
            switch (prioridade)
            {
                case 1: return Color.FromArgb(40, 167, 69);   // Verde
                case 2: return Color.FromArgb(0, 123, 255);   // Azul
                case 3: return Color.FromArgb(255, 193, 7);   // Amarelo
                case 4: return Color.FromArgb(220, 53, 69);   // Vermelho
                default: return Color.FromArgb(0, 123, 255);
            }
        }

        private void MostrarEtapa(int etapa)
        {
            etapaAtual = etapa;
            pnlConteudo.Controls.Clear();
            lblEtapa.Text = $"Etapa {etapa} de 4";

            switch (etapa)
            {
                case 1:
                    lblTitulo.Text = "Apresentação do Problema";
                    pnlConteudo.Controls.Add(pnlEtapa1);
                    btnVoltar.Visible = false;
                    btnProximo.Text = "Próximo →";
                    txtTitulo.Focus();
                    break;

                case 2:
                    lblTitulo.Text = "Quem é Afetado?";
                    pnlConteudo.Controls.Add(pnlEtapa2);
                    pnlEtapa2.Visible = true;
                    btnVoltar.Visible = true;
                    btnProximo.Text = "Próximo →";
                    rbApenasEu.Focus();
                    break;

                case 3:
                    lblTitulo.Text = "Impacto no Trabalho";
                    pnlConteudo.Controls.Add(pnlEtapa3);
                    pnlEtapa3.Visible = true;
                    btnVoltar.Visible = true;
                    btnProximo.Text = "Próximo →";
                    rbImpedeNao.Focus();
                    break;

                case 4:
                    lblTitulo.Text = "Revisão e Confirmação";
                    AtualizarRevisao();  // ✅ Sin calcular prioridad
                    pnlConteudo.Controls.Add(pnlEtapa4);
                    pnlEtapa4.Visible = true;
                    btnVoltar.Visible = true;
                    btnProximo.Text = "Concluir";

                    _ = AnalisarComIAAsync();

                    rbConcordoPrioridade.Focus();
                    break;
            }
        }

        private async Task AnalisarComIAAsync()
        {
            if (analisandoComIA) return;

            try
            {
                analisandoComIA = true;
                lblAnalisandoIA.Visible = true;
                pnlResultadoIA.Visible = false;
                btnProximo.Enabled = false;

                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine("🤖 Iniciando análise com IA...");
                Console.WriteLine($"Título: {tituloChamado}");
                Console.WriteLine($"Categoria: {categoria}");
                Console.WriteLine($"Afetado: {afetado}");
                Console.WriteLine($"Bloqueia: {impedeTrabalho}");
                Console.WriteLine("═══════════════════════════════════════");

                string pessoasAfetadasTexto = afetado;
                string bloqueiaTrabalhoTexto = impedeTrabalho ? "sim" : "não";
                string prioridadeUsuario = "";
                string justificativaPrioridade = "";

                // Chamar IA
                analiseIA = await IAService.SendToN8nToIa(
                    _funcionarioLogado.Id.ToString(),
                    tituloChamado,
                    _funcionarioLogado.Nome,
                    _funcionarioLogado.Email ?? "sem-email@empresa.com",
                    categoria,
                    descricao,
                    pessoasAfetadasTexto,
                    bloqueiaTrabalhoTexto,
                    prioridadeUsuario,
                    justificativaPrioridade,
                    1
                );

                lblAnalisandoIA.Visible = false;

                Console.WriteLine("═══════════════════════════════════════");
                if (analiseIA != null && analiseIA.Success)
                {
                    Console.WriteLine("✅ IA RESPONDEU COM SUCESSO");
                    Console.WriteLine($"Prioridade retornada: '{analiseIA.Prioridade}'");
                    Console.WriteLine($"Justificativa retornada: '{analiseIA.Justificativa?.Substring(0, Math.Min(100, analiseIA.Justificativa?.Length ?? 0))}'");
                    Console.WriteLine("═══════════════════════════════════════");

                    ExibirResultadoIA(analiseIA);
                }
                else
                {
                    Console.WriteLine("❌ IA NÃO RESPONDEU OU FALHOU");
                    Console.WriteLine($"analiseIA null? {analiseIA == null}");
                    if (analiseIA != null)
                    {
                        Console.WriteLine($"Success: {analiseIA.Success}");
                        Console.WriteLine($"Prioridade: '{analiseIA.Prioridade}'");
                        Console.WriteLine($"Justificativa: '{analiseIA.Justificativa}'");
                    }
                    Console.WriteLine("═══════════════════════════════════════");

                    MessageBox.Show(
                        "⚠️ Não foi possível obter análise da IA.\n\n" +
                        "Por favor, verifique sua conexão e tente novamente.\n" +
                        "Você pode clicar em 'Voltar' e depois 'Próximo' para tentar novamente.",
                        "Atenção",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine($"❌ EXCEÇÃO na análise IA: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                Console.WriteLine("═══════════════════════════════════════");

                lblAnalisandoIA.Visible = false;

                MessageBox.Show(
                    $"❌ Erro ao conectar com a IA:\n\n{ex.Message}\n\n" +
                    "Você pode voltar e tentar novamente.",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                analisandoComIA = false;
                btnProximo.Enabled = true;
            }
        }



        private void ExibirResultadoIA(IAResponse analise)
        {
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("🤖 EXIBINDO RESULTADO DA IA");
            Console.WriteLine($"   Prioridade recebida: '{analise.Prioridade}'");
            Console.WriteLine($"   Justificativa length: {analise.Justificativa?.Length ?? 0}");

            if (string.IsNullOrWhiteSpace(analise.Prioridade))
            {
                Console.WriteLine("⚠️ ATENÇÃO: N8N não retornou prioridade! Calculando localmente...");
                string prioridadeCalculada = CalcularPrioridadeLocal();
                analise.Prioridade = prioridadeCalculada;
                Console.WriteLine($"✅ Prioridade calculada localmente: {prioridadeCalculada}");
                lblPrioridadeIA.Text = $"⚡ Prioridade Sugerida: {prioridadeCalculada} (calculada automaticamente)";
            }
            else
            {
                analise.Prioridade = analise.Prioridade.Trim();
                Console.WriteLine($"✅ Prioridade da IA (limpa): '{analise.Prioridade}'");
                lblPrioridadeIA.Text = $"⚡ Prioridade Sugerida: {analise.Prioridade}";
            }

            RichTextBox rtbJustificativa = pnlResultadoIA.Controls["rtbJustificativaIA"] as RichTextBox;

            if (rtbJustificativa != null)
            {
                if (string.IsNullOrEmpty(analise.Justificativa))
                {
                    Console.WriteLine("⚠️ ATENÇÃO: IA não retornou justificativa!");
                    rtbJustificativa.Text = "Sem justificativa disponível";
                }
                else
                {
                    rtbJustificativa.Text = analise.Justificativa;
                }
            }

            Color corIA = ObterCorPorPrioridadeTexto(analise.Prioridade);
            lblPrioridadeIA.ForeColor = corIA;

            pnlResultadoIA.Visible = true;

            Console.WriteLine("═══════════════════════════════════════════════");
        }


        private string CalcularPrioridadeLocal()
        {
            Console.WriteLine("🔢 Calculando prioridade baseada em:");
            Console.WriteLine($"   Afetado: {afetado}");
            Console.WriteLine($"   Impede trabalho: {impedeTrabalho}");

            if (impedeTrabalho)
            {
                if (afetado == "empresa")
                {
                    Console.WriteLine("   → Crítica (bloqueia + empresa toda)");
                    return "Crítica";
                }
                else if (afetado == "departamento")
                {
                    Console.WriteLine("   → Alta (bloqueia + departamento)");
                    return "Alta";
                }
                else // "eu"
                {
                    Console.WriteLine("   → Média (bloqueia + só eu)");
                    return "Média";
                }
            }
            else
            {
                Console.WriteLine("   → Baixa (não bloqueia)");
                return "Baixa";
            }
        }

        private Color ObterCorPorPrioridadeTexto(string prioridade)
        {
            if (string.IsNullOrEmpty(prioridade)) return Color.FromArgb(0, 123, 255);

            string prioridadeLower = prioridade.ToLower();

            if (prioridadeLower.Contains("baixa") || prioridadeLower.Contains("low"))
                return Color.FromArgb(40, 167, 69);   // Verde
            else if (prioridadeLower.Contains("média") || prioridadeLower.Contains("media") || prioridadeLower.Contains("medium"))
                return Color.FromArgb(0, 123, 255);   // Azul
            else if (prioridadeLower.Contains("alta") || prioridadeLower.Contains("high"))
                return Color.FromArgb(255, 193, 7);   // Amarelo
            else if (prioridadeLower.Contains("crítica") || prioridadeLower.Contains("critica") || prioridadeLower.Contains("urgent"))
                return Color.FromArgb(220, 53, 69);   // Vermelho

            return Color.FromArgb(0, 123, 255);  // Padrão: Azul
        }

        private void MostrarConfirmacaoComIA()
        {
            string mensagem = $"Deseja concluir a criação do chamado?\n\n" +
                            $"📋 Título: {tituloChamado}\n" +
                            $"📁 Categoria: {categoria}\n" +
                            $"👥 Afetados: {ObterTextoAfetado()}\n" +
                            $"🚨 Impede trabalho: {(impedeTrabalho ? "Sim" : "Não")}\n" +
                            $"⚡ Prioridade (IA): {analiseIA.Prioridade}";

            if (contestaPrioridade)
            {
                mensagem += $"\n\n⚠️ CONTESTAÇÃO REGISTRADA\nUm técnico revisará sua solicitação.";
            }

            var result = MessageBox.Show(mensagem, "Confirmar Criação do Chamado",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CriarChamadoComIA();  // ✅ Novo método
            }
            else
            {
                MostrarEtapa(4);
            }
        }

        private void BtnProximo_Click(object sender, EventArgs e)
        {
            if (etapaAtual == 1)
            {
                if (!ValidarEtapa1()) return;
                SalvarDadosEtapa1();
                MostrarEtapa(2);
            }
            else if (etapaAtual == 2)
            {
                SalvarDadosEtapa2();
                MostrarEtapa(3);
            }
            else if (etapaAtual == 3)
            {
                SalvarDadosEtapa3();
                MostrarEtapa(4);
            }
            else if (etapaAtual == 4)
            {
                // Validar que la IA haya respondido
                if (analiseIA == null || !analiseIA.Success)
                {
                    MessageBox.Show(
                        "⚠️ A análise da IA não foi concluída.\n\n" +
                        "Por favor, aguarde a análise ou volte e tente novamente.",
                        "Análise Pendente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!ValidarEtapa4()) return;
                SalvarDadosEtapa4();
                MostrarConfirmacaoComIA();  // ✅ Novo método
            }
        }

        private void BtnVoltar_Click(object sender, EventArgs e)
        {
            if (etapaAtual > 1)
            {
                MostrarEtapa(etapaAtual - 1);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Deseja realmente cancelar a criação do chamado?",
                "Confirmar Cancelamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool ValidarEtapa1()
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("Por favor, informe o título do problema.",
                    "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return false;
            }

            if (txtTitulo.Text.Trim().Length < 5)
            {
                MessageBox.Show("O título deve ter pelo menos 5 caracteres.",
                    "Título Muito Curto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return false;
            }

            if (cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, selecione uma categoria.",
                    "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategoria.Focus();
                return false;
            }

            if (cmbCategoria.Text == "Outros...")
            {
                string outraCategoria = txtOutraCategoria.GetText();
                if (string.IsNullOrWhiteSpace(outraCategoria))
                {
                    MessageBox.Show("Por favor, especifique a categoria.",
                        "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOutraCategoria.Focus();
                    return false;
                }

                if (outraCategoria.Length < 3)
                {
                    MessageBox.Show("A categoria deve ter pelo menos 3 caracteres.",
                        "Categoria Muito Curta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOutraCategoria.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(rtbDescricao.Text))
            {
                MessageBox.Show("Por favor, descreva o problema.",
                    "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescricao.Focus();
                return false;
            }

            if (rtbDescricao.Text.Trim().Length < 20)
            {
                MessageBox.Show("A descrição deve ter pelo menos 20 caracteres.",
                    "Descrição Muito Curta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescricao.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarEtapa4()
        {
            if (rbContestoPrioridade.Checked)
            {
                if (string.IsNullOrWhiteSpace(rtbJustificativaContestacao.Text))
                {
                    MessageBox.Show("Por favor, justifique sua contestação da prioridade.",
                        "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbJustificativaContestacao.Focus();
                    return false;
                }

                if (rtbJustificativaContestacao.Text.Trim().Length < 20)
                {
                    MessageBox.Show("A justificativa deve ter pelo menos 20 caracteres.",
                        "Justificativa Muito Curta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbJustificativaContestacao.Focus();
                    return false;
                }
            }

            return true;
        }

        private void SalvarDadosEtapa1()
        {
            tituloChamado = txtTitulo.Text.Trim();

            if (cmbCategoria.Text == "Outros...")
            {
                categoria = txtOutraCategoria.GetText().Trim();
            }
            else
            {
                categoria = cmbCategoria.Text;
            }

            descricao = rtbDescricao.Text.Trim();
        }

        private void CmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isOutros = cmbCategoria.Text == "Outros...";
            lblOutraCategoria.Visible = isOutros;
            txtOutraCategoria.Visible = isOutros;

            if (isOutros)
            {
                txtOutraCategoria.Focus();
            }
        }

        private void SalvarDadosEtapa2()
        {
            if (rbApenasEu.Checked)
                afetado = "eu";
            else if (rbMeuDepartamento.Checked)
                afetado = "departamento";
            else
                afetado = "empresa";
        }

        private void SalvarDadosEtapa3()
        {
            impedeTrabalho = rbImpedeSim.Checked;
        }

        private void SalvarDadosEtapa4()
        {
            contestaPrioridade = rbContestoPrioridade.Checked;

            if (contestaPrioridade)
            {
                justificativaContestacao = rtbJustificativaContestacao.Text.Trim();
            }
        }


        private string ObterTextoPrioridade(int prioridade)
        {
            switch (prioridade)
            {
                case 1: return "Baixa";
                case 2: return "Média";
                case 3: return "Alta";
                case 4: return "Crítica";
                default: return "Média";
            }
        }

        private string ObterTextoAfetado()
        {
            switch (afetado)
            {
                case "eu": return "Apenas eu";
                case "departamento": return "Meu departamento";
                case "empresa": return "A empresa toda";
                default: return "Não especificado";
            }
        }

        private async void CriarChamadoComIA()
        {
            if (_criandoChamado)
            {
                Console.WriteLine("⚠️ Criação já em andamento");
                return;
            }

            try
            {
                _criandoChamado = true;

                btnProximo.Enabled = false;
                btnVoltar.Enabled = false;
                btnCancelar.Enabled = false;
                btnProximo.Text = "Salvando na nuvem...";

                Console.WriteLine("☁️ Salvando chamado na nuvem via IA...");

                string pessoasAfetadasTexto = afetado;
                string bloqueiaTrabalhoTexto = impedeTrabalho ? "sim" : "não";

                // ✅ LÓGICA CORRETA: SEMPRE enviar a prioridade da IA
                // A contestação é apenas um registro para o técnico revisar
                string prioridadeParaEnviar = analiseIA.Prioridade; // SEMPRE a prioridade da IA
                string userPriorityReason;

                if (contestaPrioridade)
                {
                    // Contestação: envia a justificativa do usuário
                    userPriorityReason = $"{justificativaContestacao}";
                    Console.WriteLine($"⚠️ Usuário CONTESTA a prioridade '{analiseIA.Prioridade}'");
                    Console.WriteLine($"   Justificativa: {justificativaContestacao}");
                    Console.WriteLine($"   ✅ Mantendo prioridade da IA para técnico revisar");
                }
                else
                {
                    // Aceita: envia a justificativa da IA
                    userPriorityReason = analiseIA.Justificativa ?? "";
                    Console.WriteLine($"✅ Usuário ACEITA prioridade da IA: {prioridadeParaEnviar}");
                }

                // Validação: garantir que prioridade não está vazia
                if (string.IsNullOrWhiteSpace(prioridadeParaEnviar))
                {
                    prioridadeParaEnviar = "Média";
                    Console.WriteLine($"⚠️ Prioridade vazia detectada, usando fallback: {prioridadeParaEnviar}");
                }

                Console.WriteLine($"📤 Enviando para N8N: Prioridade='{prioridadeParaEnviar}'");

                IAResponse resultadoSalvar = await IAService.SendToN8nToIa(
                    _funcionarioLogado.Id.ToString(),
                    tituloChamado,
                    _funcionarioLogado.Nome,
                    _funcionarioLogado.Email ?? "sem-email@empresa.com",
                    categoria,
                    descricao,
                    pessoasAfetadasTexto,
                    bloqueiaTrabalhoTexto,
                    prioridadeParaEnviar,   // ✅ SEMPRE a prioridade da IA
                    userPriorityReason,     // Justificativa (da IA ou contestação)
                    2
                );

                if (resultadoSalvar != null && resultadoSalvar.Success)
                {
                    Console.WriteLine("✅ Chamado salvo na nuvem com sucesso!");

                    string mensagemSucesso = $"✅ Chamado criado com sucesso!\n\n" +
                                            $"Título: {tituloChamado}\n" +
                                            $"Prioridade: {analiseIA.Prioridade}";

                    if (contestaPrioridade)
                    {
                        mensagemSucesso += $"\n\n⚠️ CONTESTAÇÃO REGISTRADA\n" +
                                          $"Sua contestação foi registrada no chamado.\n" +
                                          $"Um técnico revisará e ajustará a prioridade se necessário.";
                    }
                    else
                    {
                        mensagemSucesso += "\n\n🔧 Técnico atribuído automaticamente\n" +
                                          "📊 Chamado criado com a prioridade sugerida pela IA.";
                    }

                    MessageBox.Show(mensagemSucesso, "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.FormClosing -= CriarChamadoForm_FormClosing;
                    this.Close();
                    return;
                }

                // ======================================================================
                // FALLBACK: Salvar localmente
                // ======================================================================
                Console.WriteLine("⚠️ Salvamento na nuvem falhou, salvando localmente...");
                btnProximo.Text = "Criando localmente...";

                // ✅ Prioridade SEMPRE da IA (mesmo se contestou)
                int prioridadeFinal = ConverterPrioridadeTextoParaNumero(analiseIA.Prioridade);

                Console.WriteLine($"🔍 SALVAMENTO LOCAL - PRIORIDADE:");
                Console.WriteLine($"   Texto IA: '{analiseIA.Prioridade}'");
                Console.WriteLine($"   Número: {prioridadeFinal}");

                string descricaoCompleta = $"DESCRIÇÃO:\n{descricao}\n\n" +
                                          $"AFETADOS: {ObterTextoAfetado()}\n" +
                                          $"IMPEDE TRABALHO: {(impedeTrabalho ? "Sim" : "Não")}\n" +
                                          $"PRIORIDADE IA: {analiseIA.Prioridade}\n" +
                                          $"JUSTIFICATIVA IA: {analiseIA.Justificativa}";

                if (contestaPrioridade)
                {
                    descricaoCompleta += $"\n\n⚠️ USUÁRIO CONTESTOU A PRIORIDADE\n{justificativaContestacao}";
                }

                var chamado = new Chamados
                {
                    Titulo = tituloChamado,
                    Categoria = categoria,
                    Prioridade = prioridadeFinal, // ✅ Sempre da IA
                    Descricao = descricaoCompleta,
                    Afetado = _funcionarioLogado.Id,
                    DataChamado = DateTime.Now,
                    Status = StatusChamado.Aberto,
                    TecnicoResponsavel = null
                };

                // Registrar contestação no histórico (se houver)
                if (contestaPrioridade)
                {
                    string contestacao = $"[CONTESTAÇÃO DE PRIORIDADE - {DateTime.Now:dd/MM/yyyy HH:mm}]\n" +
                                       $"Funcionário: {_funcionarioLogado.Nome}\n" +
                                       $"Prioridade IA: {analiseIA.Prioridade}\n" +
                                       $"Justificativa da Contestação:\n{justificativaContestacao}\n" +
                                       $"Status: Aguardando revisão do técnico";

                    chamado.Contestacoes = contestacao;
                }

                int idChamado = _chamadosController.CriarChamado(chamado);

                if (idChamado > 0)
                {
                    Console.WriteLine($"✅ Chamado #{idChamado} criado localmente");
                    Console.WriteLine($"   Prioridade: {chamado.Prioridade} ({ObterTextoPrioridade(chamado.Prioridade)})");

                    if (contestaPrioridade)
                    {
                        Console.WriteLine($"   ⚠️ Contestação registrada para revisão técnica");
                    }

                    string msgLocal = $"✅ Chamado criado localmente!\n\n" +
                                    $"Número: #{idChamado}\n" +
                                    $"Prioridade: {ObterTextoPrioridade(chamado.Prioridade)}";

                    if (contestaPrioridade)
                    {
                        msgLocal += $"\n\n⚠️ Contestação registrada.\n" +
                                   $"O técnico revisará sua solicitação.";
                    }

                    msgLocal += "\n\n⚠️ Será sincronizado com a nuvem em breve.";

                    MessageBox.Show(msgLocal, "Chamado Criado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.FormClosing -= CriarChamadoForm_FormClosing;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erro ao criar o chamado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    _criandoChamado = false;
                    btnProximo.Enabled = true;
                    btnVoltar.Enabled = true;
                    btnCancelar.Enabled = true;
                    btnProximo.Text = "Concluir";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro crítico: {ex.Message}");
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _criandoChamado = false;
                btnProximo.Enabled = true;
                btnVoltar.Enabled = true;
                btnCancelar.Enabled = true;
                btnProximo.Text = "Concluir";
            }
        }



        private int ConverterPrioridadeTextoParaNumero(string prioridade)
        {
            if (string.IsNullOrEmpty(prioridade))
            {
                Console.WriteLine("⚠️ Prioridade vazia, usando padrão: Média (2)");
                return 2;
            }

            string prioridadeLower = prioridade.ToLower().Trim();

            // Remover acentos
            prioridadeLower = prioridadeLower
                .Replace("í", "i")
                .Replace("é", "e")
                .Replace("ê", "e")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ã", "a");

            Console.WriteLine($"🔄 Convertendo prioridade: '{prioridade}' -> '{prioridadeLower}'");

            if (prioridadeLower.Contains("baixa") || prioridadeLower.Contains("low"))
            {
                Console.WriteLine("   ✅ Resultado: 1 (Baixa)");
                return 1;
            }
            else if (prioridadeLower.Contains("media") || prioridadeLower.Contains("medium"))
            {
                Console.WriteLine("   ✅ Resultado: 2 (Média)");
                return 2;
            }
            else if (prioridadeLower.Contains("alta") || prioridadeLower.Contains("high"))
            {
                Console.WriteLine("   ✅ Resultado: 3 (Alta)");
                return 3;
            }
            else if (prioridadeLower.Contains("critica") || prioridadeLower.Contains("urgent") || prioridadeLower.Contains("critical"))
            {
                Console.WriteLine("   ✅ Resultado: 4 (Crítica)");
                return 4;
            }

            Console.WriteLine($"   ⚠️ Não reconhecido: '{prioridade}', usando padrão: 2 (Média)");
            return 2;
        }



        private void CriarChamadoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Se está criando um chamado, prevenir fechamento acidental
            if (_criandoChamado && e.CloseReason == CloseReason.UserClosing)
            {
                var resultado = MessageBox.Show(
                    "Um chamado está sendo criado. Deseja realmente cancelar?\n\n" +
                    "⚠️ Isso pode resultar em dados inconsistentes.",
                    "Criação em Andamento",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.No)
                {
                    e.Cancel = true; // Cancelar o fechamento
                }
            }
        }
    }
}