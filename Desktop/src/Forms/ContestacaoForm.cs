using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Data.Repositories;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class ContestacaoForm : Form
    {
        private Chamados _chamado;
        private Funcionarios _usuarioLogado;
        private ContestacoesRepository _repository;

        // Controles
        private ListView lstHistorial;
        private TextBox txtNovaContestacao;
        private Button btnAdicionar;
        private Button btnFechar;
        private Label lblChamadoInfo;
        private Label lblTotalContestacoes;

        public ContestacaoForm(Chamados chamado, Funcionarios usuarioLogado)
        {
            _chamado = chamado ?? throw new ArgumentNullException(nameof(chamado));
            _usuarioLogado = usuarioLogado ?? throw new ArgumentNullException(nameof(usuarioLogado));
            _repository = new ContestacoesRepository();

            InitializeComponent();
            CarregarHistorial();
        }

        private void InitializeComponent()
        {
            this.lstHistorial = new ListView();
            this.txtNovaContestacao = new TextBox();
            this.btnAdicionar = new Button();
            this.btnFechar = new Button();
            this.lblChamadoInfo = new Label();
            this.lblTotalContestacoes = new Label();

            // Configuração do formulário
            this.Text = "Historial de Contestações";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(700, 500);

            // ===========================================
            // SEÇÃO 1: INFORMAÇÕES DO CHAMADO
            // ===========================================
            var panelInfo = new Panel
            {
                Location = new Point(12, 12),
                Size = new Size(760, 70),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var lblTitulo = new Label
            {
                Text = "📋 Informações do Chamado",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, 8),
                Size = new Size(400, 20)
            };

            this.lblChamadoInfo.Text = $"Chamado #{_chamado.IdChamado} - {_chamado.Categoria}\n" +
                                       $"Descrição: {(_chamado.Descricao.Length > 100 ? _chamado.Descricao.Substring(0, 100) + "..." : _chamado.Descricao)}";
            this.lblChamadoInfo.Location = new Point(10, 30);
            this.lblChamadoInfo.Size = new Size(550, 32);
            this.lblChamadoInfo.AutoSize = false;

            this.lblTotalContestacoes.Text = "0 contestações";
            this.lblTotalContestacoes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblTotalContestacoes.ForeColor = Color.FromArgb(0, 123, 255);
            this.lblTotalContestacoes.Location = new Point(580, 35);
            this.lblTotalContestacoes.Size = new Size(150, 20);

            panelInfo.Controls.AddRange(new Control[] { lblTitulo, this.lblChamadoInfo, this.lblTotalContestacoes });

            // ===========================================
            // SEÇÃO 2: HISTORIAL (ListView)
            // ===========================================
            var lblHistorial = new Label
            {
                Text = "📜 Historial de Contestações:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(12, 90),
                Size = new Size(200, 20)
            };

            this.lstHistorial.Location = new Point(12, 115);
            this.lstHistorial.Size = new Size(760, 280);
            this.lstHistorial.View = View.Details;
            this.lstHistorial.FullRowSelect = true;
            this.lstHistorial.GridLines = true;
            this.lstHistorial.MultiSelect = false;
            this.lstHistorial.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Colunas do ListView
            this.lstHistorial.Columns.Clear();
            this.lstHistorial.Columns.Add("Data/Hora", 150);
            this.lstHistorial.Columns.Add("Usuário", 200);
            this.lstHistorial.Columns.Add("Justificativa", 390);

            // ===========================================
            // SEÇÃO 3: NOVA CONTESTAÇÃO
            // ===========================================
            var panelNova = new GroupBox
            {
                Text = "➕ Adicionar Nova Contestação",
                Location = new Point(12, 405),
                Size = new Size(760, 120),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var lblTexto = new Label
            {
                Text = "Justificativa:",
                Location = new Point(10, 25),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            this.txtNovaContestacao.Location = new Point(10, 50);
            this.txtNovaContestacao.Size = new Size(635, 60);  // ✅ Más ancho
            this.txtNovaContestacao.Multiline = true;
            this.txtNovaContestacao.ScrollBars = ScrollBars.Vertical;
            this.txtNovaContestacao.MaxLength = 1000;
            this.txtNovaContestacao.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            this.btnAdicionar.Text = "✓ Adicionar";
            this.btnAdicionar.Location = new Point(655, 50);  // ✅ Ajustado
            this.btnAdicionar.Size = new Size(90, 60);
            this.btnAdicionar.BackColor = Color.FromArgb(40, 167, 69);
            this.btnAdicionar.ForeColor = Color.White;
            this.btnAdicionar.FlatStyle = FlatStyle.Flat;
            this.btnAdicionar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnAdicionar.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnAdicionar.Click += BtnAdicionar_Click;

            panelNova.Controls.AddRange(new Control[] {
        lblTexto, this.txtNovaContestacao, this.btnAdicionar
    });

            var lblContador = new Label
            {
                Name = "lblContador",
                Text = "0/1000 caracteres",
                Location = new Point(275, 85),
                Size = new Size(150, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8F)
            };

            this.txtNovaContestacao.TextChanged += (s, e) => {
                int length = txtNovaContestacao.Text.Length;
                lblContador.Text = $"{length}/1000 caracteres";
                lblContador.ForeColor = length > 900 ? Color.Red : Color.Gray;
            };

            panelNova.Controls.AddRange(new Control[] {
                lblTexto, this.txtNovaContestacao,
                this.btnAdicionar, lblContador
            });

            // ===========================================
            // SEÇÃO 4: BOTÃO FECHAR
            // ===========================================
            this.btnFechar.Text = "✖ Fechar";
            this.btnFechar.Location = new Point(672, 535);
            this.btnFechar.Size = new Size(100, 30);
            this.btnFechar.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnFechar.Click += (s, e) => this.Close();

            // Adicionar todos ao formulário
            this.Controls.AddRange(new Control[] {
                panelInfo, lblHistorial, this.lstHistorial,
                panelNova, this.btnFechar
            });
        }

        /// <summary>
        /// Carrega o historial de contestações do banco de dados
        /// </summary>
        private void CarregarHistorial()
        {
            try
            {
                lstHistorial.Items.Clear();
                var contestacoes = _repository.ListarPorChamado(_chamado.IdChamado);

                foreach (var contestacao in contestacoes)
                {
                    var item = new ListViewItem(contestacao.DataContestacao.ToString("dd/MM/yyyy HH:mm"));
                    item.SubItems.Add(contestacao.NomeUsuario);
                    item.SubItems.Add(contestacao.Justificativa);  // ✅ SIN columna Tipo
                    item.Tag = contestacao;

                    // ✅ Color único para todas
                    item.BackColor = Color.FromArgb(255, 250, 240);

                    lstHistorial.Items.Add(item);
                }

                lblTotalContestacoes.Text = $"{contestacoes.Count} contestação{(contestacoes.Count == 1 ? "" : "ões")}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar historial:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adiciona uma nova contestação
        /// </summary>
        private void BtnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNovaContestacao.Text))
                {
                    MessageBox.Show("Digite a justificativa da contestação.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNovaContestacao.Focus();
                    return;
                }

                var novaContestacao = new Contestacao(
                    _chamado.IdChamado,
                    _usuarioLogado.Id,
                    txtNovaContestacao.Text.Trim()
                );

                int novoId = _repository.Inserir(novaContestacao);

                if (novoId > 0)
                {
                    MessageBox.Show("Contestação adicionada com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtNovaContestacao.Clear();
                    CarregarHistorial();
                }
                else
                {
                    throw new Exception("Falha ao inserir contestação.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar contestação:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}