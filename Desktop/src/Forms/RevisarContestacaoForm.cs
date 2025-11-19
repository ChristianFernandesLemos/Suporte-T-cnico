using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    /// <summary>
    /// Formulário para técnicos revisarem contestações de prioridade
    /// </summary>
    public partial class RevisarContestacaoForm : Form
    {
        private Chamados _chamado;
        private ChamadosController _controller;
        private int _prioridadeOriginal;

        // Controles
        private Panel pnlHeader;
        private Label lblTitulo;
        private Panel pnlConteudo;
        private GroupBox grpInfoChamado;
        private Label lblInfoChamado;
        private GroupBox grpContestacao;
        private RichTextBox rtbContestacao;
        private GroupBox grpNovaPrioridade;
        private Label lblPrioridadeAtual;
        private Label lblSelecionePrioridade;
        private RadioButton rbPrioridade1;
        private RadioButton rbPrioridade2;
        private RadioButton rbPrioridade3;
        private RadioButton rbPrioridade4;
        private Panel pnlRadioPrioridades;
        private Label lblRespostaTecnico;
        private RichTextBox rtbRespostaTecnico;
        private Panel pnlBotoes;
        private Button btnAprovar;
        private Button btnAlterarPrioridade;
        private Button btnCancelar;

        public RevisarContestacaoForm(Chamados chamado, ChamadosController controller)
        {
            _chamado = chamado ?? throw new ArgumentNullException(nameof(chamado));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _prioridadeOriginal = chamado.Prioridade;

            InitializeComponent();
            CarregarDados();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(850, 750);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Revisar Contestação de Prioridade";
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(255, 193, 7),
                Padding = new Padding(20)
            };

            lblTitulo = new Label
            {
                Text = "⚠️ Revisar Contestação de Prioridade",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Location = new Point(20, 25)
            };

            pnlHeader.Controls.Add(lblTitulo);

            // Painel de Conteúdo
            pnlConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White,
                AutoScroll = true
            };

            // Informações do Chamado
            grpInfoChamado = new GroupBox
            {
                Text = "Informações do Chamado",
                Location = new Point(20, 10),
                Size = new Size(740, 100),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            lblInfoChamado = new Label
            {
                Location = new Point(15, 25),
                Size = new Size(710, 60),
                Font = new Font("Segoe UI", 10F),
                AutoSize = false
            };

            grpInfoChamado.Controls.Add(lblInfoChamado);

            // Contestação
            grpContestacao = new GroupBox
            {
                Text = "Contestação do Funcionário",
                Location = new Point(20, 120),
                Size = new Size(740, 150),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            rtbContestacao = new RichTextBox
            {
                Location = new Point(15, 25),
                Size = new Size(710, 110),
                Font = new Font("Segoe UI", 10F),
                ReadOnly = true,
                BackColor = Color.FromArgb(255, 243, 205),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            grpContestacao.Controls.Add(rtbContestacao);

            // Nova Prioridade
            grpNovaPrioridade = new GroupBox
            {
                Text = "Ação do Técnico",
                Location = new Point(20, 280),
                Size = new Size(740, 280),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            lblPrioridadeAtual = new Label
            {
                Location = new Point(15, 25),
                Size = new Size(710, 25),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            lblSelecionePrioridade = new Label
            {
                Text = "Selecione a nova prioridade (se desejar alterar):",
                Location = new Point(15, 55),
                Size = new Size(710, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            pnlRadioPrioridades = new Panel
            {
                Location = new Point(15, 80),
                Size = new Size(710, 100),
                BackColor = Color.White
            };

            rbPrioridade1 = new RadioButton
            {
                Text = "🟢 Baixa (1) - Pode ser resolvido sem urgência",
                Location = new Point(20, 5),
                Size = new Size(650, 20),
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };

            rbPrioridade2 = new RadioButton
            {
                Text = "🔵 Média (2) - Necessita atenção moderada",
                Location = new Point(20, 30),
                Size = new Size(650, 20),
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };

            rbPrioridade3 = new RadioButton
            {
                Text = "🟡 Alta (3) - Requer atenção prioritária",
                Location = new Point(20, 55),
                Size = new Size(650, 20),
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };

            rbPrioridade4 = new RadioButton
            {
                Text = "🔴 Crítica (4) - Urgente! Impacto severo",
                Location = new Point(20, 80),
                Size = new Size(650, 20),
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };

            pnlRadioPrioridades.Controls.Add(rbPrioridade1);
            pnlRadioPrioridades.Controls.Add(rbPrioridade2);
            pnlRadioPrioridades.Controls.Add(rbPrioridade3);
            pnlRadioPrioridades.Controls.Add(rbPrioridade4);

            lblRespostaTecnico = new Label
            {
                Text = "Mensagem para o funcionário (opcional):",
                Location = new Point(15, 185),
                Size = new Size(710, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            rtbRespostaTecnico = new RichTextBox
            {
                Location = new Point(15, 210),
                Size = new Size(710, 60),
                Font = new Font("Segoe UI", 9F),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            grpNovaPrioridade.Controls.Add(lblPrioridadeAtual);
            grpNovaPrioridade.Controls.Add(lblSelecionePrioridade);
            grpNovaPrioridade.Controls.Add(pnlRadioPrioridades);
            grpNovaPrioridade.Controls.Add(lblRespostaTecnico);
            grpNovaPrioridade.Controls.Add(rtbRespostaTecnico);

            pnlConteudo.Controls.Add(grpInfoChamado);
            pnlConteudo.Controls.Add(grpContestacao);
            pnlConteudo.Controls.Add(grpNovaPrioridade);

            // Painel de Botões
            pnlBotoes = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(120, 40),
                Location = new Point(380, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            btnAprovar = new Button
            {
                Text = "✓ Manter Prioridade",
                Size = new Size(160, 40),
                Location = new Point(510, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAprovar.FlatAppearance.BorderSize = 0;
            btnAprovar.Click += BtnAprovar_Click;

            btnAlterarPrioridade = new Button
            {
                Text = "✎ Alterar Prioridade",
                Size = new Size(160, 40),
                Location = new Point(510, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnAlterarPrioridade.FlatAppearance.BorderSize = 0;
            btnAlterarPrioridade.Click += BtnAlterarPrioridade_Click;

            pnlBotoes.Controls.Add(btnCancelar);
            pnlBotoes.Controls.Add(btnAprovar);
            pnlBotoes.Controls.Add(btnAlterarPrioridade);

            // Eventos para controlar visibilidade dos botões
            rbPrioridade1.CheckedChanged += RadioPrioridade_CheckedChanged;
            rbPrioridade2.CheckedChanged += RadioPrioridade_CheckedChanged;
            rbPrioridade3.CheckedChanged += RadioPrioridade_CheckedChanged;
            rbPrioridade4.CheckedChanged += RadioPrioridade_CheckedChanged;

            this.Controls.Add(pnlConteudo);
            this.Controls.Add(pnlBotoes);
            this.Controls.Add(pnlHeader);

            this.ResumeLayout(false);
        }

        private void CarregarDados()
        {
            // Informações do chamado
            lblInfoChamado.Text =
                $"Chamado: #{_chamado.IdChamado}\n" +
                $"Categoria: {_chamado.Categoria}\n" +
                $"Status: {_chamado.Status}\n" +
                $"Data: {_chamado.DataChamado:dd/MM/yyyy HH:mm}";

            // Contestação
            rtbContestacao.Text = _chamado.Contestacoes ?? "Sem contestação registrada.";

            // Prioridade atual
            string textoPrioridade = ObterTextoPrioridade(_prioridadeOriginal);
            lblPrioridadeAtual.Text = $"Prioridade Atual: {textoPrioridade} ({_prioridadeOriginal})";

            // Marcar a prioridade atual como selecionada por padrão
            SelecionarPrioridade(_prioridadeOriginal);
        }

        private void SelecionarPrioridade(int prioridade)
        {
            switch (prioridade)
            {
                case 1: rbPrioridade1.Checked = true; break;
                case 2: rbPrioridade2.Checked = true; break;
                case 3: rbPrioridade3.Checked = true; break;
                case 4: rbPrioridade4.Checked = true; break;
            }
        }

        private int ObterPrioridadeSelecionada()
        {
            if (rbPrioridade1.Checked) return 1;
            if (rbPrioridade2.Checked) return 2;
            if (rbPrioridade3.Checked) return 3;
            if (rbPrioridade4.Checked) return 4;
            return _prioridadeOriginal;
        }

        private void RadioPrioridade_CheckedChanged(object sender, EventArgs e)
        {
            int novaPrioridade = ObterPrioridadeSelecionada();
            bool prioridadeMudou = novaPrioridade != _prioridadeOriginal;

            // Mostrar botão apropriado
            btnAprovar.Visible = !prioridadeMudou;
            btnAlterarPrioridade.Visible = prioridadeMudou;
        }

        private void BtnAprovar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Você manterá a prioridade atual do chamado.\n\n" +
                "A contestação será marcada como revisada e negada.\n\n" +
                "Deseja continuar?",
                "Confirmar Ação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                FinalizarRevisao(_prioridadeOriginal, false);
            }
        }

        private void BtnAlterarPrioridade_Click(object sender, EventArgs e)
        {
            int novaPrioridade = ObterPrioridadeSelecionada();
            string textoPrioridadeNova = ObterTextoPrioridade(novaPrioridade);
            string textoPrioridadeAntiga = ObterTextoPrioridade(_prioridadeOriginal);

            var result = MessageBox.Show(
                $"Você alterará a prioridade do chamado:\n\n" +
                $"De: {textoPrioridadeAntiga} ({_prioridadeOriginal})\n" +
                $"Para: {textoPrioridadeNova} ({novaPrioridade})\n\n" +
                "A contestação será marcada como revisada e aprovada.\n\n" +
                "Deseja continuar?",
                "Confirmar Alteração",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                FinalizarRevisao(novaPrioridade, true);
            }
        }

        private void FinalizarRevisao(int novaPrioridade, bool aprovada)
        {
            try
            {
                // Atualizar a prioridade se foi alterada
                if (novaPrioridade != _prioridadeOriginal)
                {
                    _controller.AlterarPrioridade(_chamado.IdChamado, novaPrioridade);
                }

                // Adicionar resposta do técnico à contestação
                string respostaTecnico = rtbRespostaTecnico.Text.Trim();
                string statusContestacao = aprovada ? "APROVADA" : "NEGADA";
                string novaContestacao = $"{_chamado.Contestacoes}\n\n" +
                                        $"[RESPOSTA DO TÉCNICO - {DateTime.Now:dd/MM/yyyy HH:mm}]\n" +
                                        $"Status: {statusContestacao}\n" +
                                        $"Prioridade Original: {ObterTextoPrioridade(_prioridadeOriginal)}\n" +
                                        $"Prioridade Final: {ObterTextoPrioridade(novaPrioridade)}\n";

                if (!string.IsNullOrEmpty(respostaTecnico))
                {
                    novaContestacao += $"Mensagem: {respostaTecnico}\n";
                }

                novaContestacao += "---\nContestação revisada e finalizada.";

                // Atualizar contestação
                _controller.AdicionarContestacao(_chamado.IdChamado, novaContestacao);

                MessageBox.Show(
                    "Contestação revisada com sucesso!\n\n" +
                    $"Status: {statusContestacao}\n" +
                    $"Prioridade: {ObterTextoPrioridade(novaPrioridade)}",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao processar contestação: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Deseja realmente cancelar a revisão da contestação?\n\n" +
                "Nenhuma alteração será salva.",
                "Confirmar Cancelamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
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
                default: return "Desconhecida";
            }
        }
    }
}