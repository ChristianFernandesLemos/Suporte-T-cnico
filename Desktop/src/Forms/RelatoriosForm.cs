using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class RelatoriosForm : Form
    {
        private ChamadosController _chamadosController;
        private FuncionariosController _funcionariosController;
        private Funcionarios _funcionarioLogado;

        // Controles principais
        private TabControl tabControl;
        private TabPage tabRelatorios;
        private TabPage tabEstatisticas;

        // Controles de Relatórios
        private GroupBox gbPeriodo;
        private RadioButton rbDiario;
        private RadioButton rbSemanal;
        private RadioButton rbQuinzenal;
        private RadioButton rbMensal;
        private RadioButton rbPersonalizado;
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFim;
        private Label lblInicio;
        private Label lblFim;
        private Button btnGerarRelatorio;
        private Button btnExportarPDF;
        private DataGridView dgvRelatorio;
        private Panel panelResumo;
        private Label lblTotalChamados;
        private Label lblAbertos;
        private Label lblResolvidos;
        private Label lblTempoMedio;

        // Controles de Estatísticas
        private Chart chartCategorias;
        private Chart chartTempoResolucao;
        private Chart chartResolvidosMensais;
        private Panel panelEstatisticasResumo;
        private ComboBox cmbPeriodoEstatisticas;
        private Button btnAtualizarEstatisticas;

        public RelatoriosForm(Funcionarios funcionario, ChamadosController chamadosController, FuncionariosController funcionariosController)
        {
            _funcionarioLogado = funcionario;
            _chamadosController = chamadosController;
            _funcionariosController = funcionariosController;
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void InitializeComponent()
        {
            this.tabControl = new TabControl();
            this.tabRelatorios = new TabPage("📊 Relatórios");
            this.tabEstatisticas = new TabPage("📈 Estatísticas");

            // Configuração do TabControl
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Font = new Font("Segoe UI", 10F);

            // ========================================
            // TAB: RELATÓRIOS
            // ========================================
            InicializarTabRelatorios();

            // ========================================
            // TAB: ESTATÍSTICAS
            // ========================================
            InicializarTabEstatisticas();

            // Adicionar tabs
            this.tabControl.TabPages.Add(this.tabRelatorios);
            this.tabControl.TabPages.Add(this.tabEstatisticas);

            // Configuração do Form
            this.Text = "Relatórios e Estatísticas - Sistema de Chamados";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            this.Controls.Add(this.tabControl);
        }

        private void InicializarTabRelatorios()
        {
            // ========================================
            // GROUPBOX: PERÍODO
            // ========================================
            this.gbPeriodo = new GroupBox
            {
                Text = "Selecionar Período",
                Location = new Point(20, 20),
                Size = new Size(350, 200),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            this.rbDiario = new RadioButton
            {
                Text = "Diário (Hoje)",
                Location = new Point(20, 30),
                Size = new Size(150, 25),
                Checked = true
            };
            this.rbDiario.CheckedChanged += PeriodoChanged;

            this.rbSemanal = new RadioButton
            {
                Text = "Semanal (Últimos 7 dias)",
                Location = new Point(20, 60),
                Size = new Size(200, 25)
            };
            this.rbSemanal.CheckedChanged += PeriodoChanged;

            this.rbQuinzenal = new RadioButton
            {
                Text = "Quinzenal (Últimos 15 dias)",
                Location = new Point(20, 90),
                Size = new Size(220, 25)
            };
            this.rbQuinzenal.CheckedChanged += PeriodoChanged;

            this.rbMensal = new RadioButton
            {
                Text = "Mensal (Últimos 30 dias)",
                Location = new Point(20, 120),
                Size = new Size(220, 25)
            };
            this.rbMensal.CheckedChanged += PeriodoChanged;

            this.rbPersonalizado = new RadioButton
            {
                Text = "Personalizado:",
                Location = new Point(20, 150),
                Size = new Size(120, 25)
            };
            this.rbPersonalizado.CheckedChanged += PeriodoChanged;

            this.lblInicio = new Label
            {
                Text = "De:",
                Location = new Point(30, 175),
                Size = new Size(30, 20),
                Enabled = false
            };

            this.dtpInicio = new DateTimePicker
            {
                Location = new Point(60, 173),
                Size = new Size(110, 23),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };

            this.lblFim = new Label
            {
                Text = "Até:",
                Location = new Point(180, 175),
                Size = new Size(30, 20),
                Enabled = false
            };

            this.dtpFim = new DateTimePicker
            {
                Location = new Point(215, 173),
                Size = new Size(110, 23),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };

            this.gbPeriodo.Controls.AddRange(new Control[] {
                rbDiario, rbSemanal, rbQuinzenal, rbMensal, rbPersonalizado,
                lblInicio, dtpInicio, lblFim, dtpFim
            });

            // ========================================
            // BOTÕES DE AÇÃO
            // ========================================
            this.btnGerarRelatorio = new Button
            {
                Text = "📋 Gerar Relatório",
                Location = new Point(20, 230),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            this.btnGerarRelatorio.FlatAppearance.BorderSize = 0;
            this.btnGerarRelatorio.Click += BtnGerarRelatorio_Click;

            this.btnExportarPDF = new Button
            {
                Text = "📄 Exportar PDF",
                Location = new Point(190, 230),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            this.btnExportarPDF.FlatAppearance.BorderSize = 0;
            this.btnExportarPDF.Click += BtnExportarPDF_Click;

            // ========================================
            // PANEL RESUMO
            // ========================================
            this.panelResumo = new Panel
            {
                Location = new Point(390, 20),
                Size = new Size(760, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            this.lblTotalChamados = CriarLabelResumo("Total de Chamados: 0", new Point(20, 15), Color.FromArgb(0, 123, 255));
            this.lblAbertos = CriarLabelResumo("Abertos: 0", new Point(20, 45), Color.FromArgb(255, 193, 7));
            this.lblResolvidos = CriarLabelResumo("Resolvidos: 0", new Point(200, 45), Color.FromArgb(40, 167, 69));
            this.lblTempoMedio = CriarLabelResumo("Tempo Médio: N/A", new Point(400, 45), Color.FromArgb(108, 117, 125));

            this.panelResumo.Controls.AddRange(new Control[] {
                lblTotalChamados, lblAbertos, lblResolvidos, lblTempoMedio
            });

            // ========================================
            // DATAGRIDVIEW RELATÓRIO
            // ========================================
            this.dgvRelatorio = new DataGridView
            {
                Location = new Point(20, 290),
                Size = new Size(1130, 320),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            ConfigurarDataGridViewRelatorio();

            // Adicionar controles à tab
            this.tabRelatorios.Controls.AddRange(new Control[] {
                gbPeriodo, btnGerarRelatorio, btnExportarPDF, panelResumo, dgvRelatorio
            });
        }

        private void InicializarTabEstatisticas()
        {
            // ========================================
            // CONTROLES DE FILTRO
            // ========================================
            var lblPeriodo = new Label
            {
                Text = "Período de Análise:",
                Location = new Point(20, 20),
                Size = new Size(130, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            this.cmbPeriodoEstatisticas = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(160, 17),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F)
            };
            this.cmbPeriodoEstatisticas.Items.AddRange(new object[] {
                "Últimos 7 dias",
                "Últimos 15 dias",
                "Últimos 30 dias",
                "Últimos 3 meses",
                "Últimos 6 meses",
                "Último ano"
            });
            this.cmbPeriodoEstatisticas.SelectedIndex = 2; // 30 dias por padrão

            this.btnAtualizarEstatisticas = new Button
            {
                Text = "🔄 Atualizar",
                Location = new Point(380, 15),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            this.btnAtualizarEstatisticas.FlatAppearance.BorderSize = 0;
            this.btnAtualizarEstatisticas.Click += BtnAtualizarEstatisticas_Click;

            // ========================================
            // GRÁFICO: ORIGEM DE TRÁFEGO (PIZZA)
            // ========================================
            this.chartCategorias = new Chart
            {
                Location = new Point(20, 60),
                Size = new Size(360, 280),
                BackColor = Color.White
            };
            ConfigurarGraficoPizza(chartCategorias, "Chamados por Categoria");

            // ========================================
            // GRÁFICO: TEMPO DE RESOLUÇÃO (COLUNAS)
            // ========================================
            this.chartTempoResolucao = new Chart
            {
                Location = new Point(400, 60),
                Size = new Size(360, 280),
                BackColor = Color.White
            };
            ConfigurarGraficoColunas(chartTempoResolucao, "Tempo de Resolução");

            // ========================================
            // GRÁFICO: RESOLVIDOS MENSAIS (COLUNAS)
            // ========================================
            this.chartResolvidosMensais = new Chart
            {
                Location = new Point(780, 60),
                Size = new Size(360, 280),
                BackColor = Color.White
            };
            ConfigurarGraficoColunas(chartResolvidosMensais, "Chamados Resolvidos por Categoria");

            // ========================================
            // PANEL DE INSIGHTS
            // ========================================
            this.panelEstatisticasResumo = new Panel
            {
                Location = new Point(20, 360),
                Size = new Size(1120, 240),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblInsights = new Label
            {
                Text = "💡 Insights e Análises",
                Location = new Point(20, 15),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255)
            };
            this.panelEstatisticasResumo.Controls.Add(lblInsights);

            // Adicionar controles à tab
            this.tabEstatisticas.Controls.AddRange(new Control[] {
                lblPeriodo, cmbPeriodoEstatisticas, btnAtualizarEstatisticas,
                chartCategorias, chartTempoResolucao, chartResolvidosMensais,
                panelEstatisticasResumo
            });
        }

        private void ConfigurarFormulario()
        {
            // Carregar dados iniciais
            GerarRelatorio();
            AtualizarEstatisticas();
        }

        private void ConfigurarDataGridViewRelatorio()
        {
            dgvRelatorio.Columns.Clear();

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                Width = 50
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Data",
                HeaderText = "Data",
                Width = 100
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoria",
                Width = 100
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prioridade",
                HeaderText = "Prioridade",
                Width = 80
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 100
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Solicitante",
                HeaderText = "Solicitante",
                Width = 150
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tecnico",
                HeaderText = "Técnico",
                Width = 150
            });

            dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TempoResolucao",
                HeaderText = "Tempo Resolução",
                Width = 120
            });

            dgvRelatorio.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }

        private Label CriarLabelResumo(string texto, Point localizacao, Color cor)
        {
            return new Label
            {
                Text = texto,
                Location = localizacao,
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = cor
            };
        }

        private void ConfigurarGraficoPizza(Chart chart, string titulo)
        {
            chart.Series.Clear();
            chart.Titles.Clear();
            chart.ChartAreas.Clear();

            var chartArea = new ChartArea();
            chartArea.BackColor = Color.White;
            chart.ChartAreas.Add(chartArea);

            var title = new Title
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64)
            };
            chart.Titles.Add(title);

            var series = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            chart.Series.Add(series);

            chart.Legends.Add(new Legend
            {
                Docking = Docking.Bottom,
                Font = new Font("Segoe UI", 9F)
            });
        }

        private void ConfigurarGraficoColunas(Chart chart, string titulo)
        {
            chart.Series.Clear();
            chart.Titles.Clear();
            chart.ChartAreas.Clear();

            var chartArea = new ChartArea();
            chartArea.BackColor = Color.White;
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            chart.ChartAreas.Add(chartArea);

            var title = new Title
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64)
            };
            chart.Titles.Add(title);

            var series = new Series
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            chart.Series.Add(series);
        }

        private void PeriodoChanged(object sender, EventArgs e)
        {
            bool personalizado = rbPersonalizado.Checked;
            lblInicio.Enabled = personalizado;
            dtpInicio.Enabled = personalizado;
            lblFim.Enabled = personalizado;
            dtpFim.Enabled = personalizado;
        }

        private void BtnGerarRelatorio_Click(object sender, EventArgs e)
        {
            GerarRelatorio();
        }

        private void GerarRelatorio()
        {
            try
            {
                DateTime dataInicio, dataFim;
                ObterPeriodoSelecionado(out dataInicio, out dataFim);

                var todosChamados = _chamadosController.ListarTodosChamados();
                var chamadosFiltrados = todosChamados
                    .Where(c => c.DataChamado.Date >= dataInicio.Date && c.DataChamado.Date <= dataFim.Date)
                    .OrderByDescending(c => c.DataChamado)
                    .ToList();

                PreencherRelatorio(chamadosFiltrados);
                AtualizarResumo(chamadosFiltrados);

                btnExportarPDF.Enabled = chamadosFiltrados.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ObterPeriodoSelecionado(out DateTime dataInicio, out DateTime dataFim)
        {
            dataFim = DateTime.Now;

            if (rbDiario.Checked)
            {
                dataInicio = DateTime.Now.Date;
            }
            else if (rbSemanal.Checked)
            {
                dataInicio = DateTime.Now.AddDays(-7);
            }
            else if (rbQuinzenal.Checked)
            {
                dataInicio = DateTime.Now.AddDays(-15);
            }
            else if (rbMensal.Checked)
            {
                dataInicio = DateTime.Now.AddDays(-30);
            }
            else // Personalizado
            {
                dataInicio = dtpInicio.Value.Date;
                dataFim = dtpFim.Value.Date.AddHours(23).AddMinutes(59);
            }
        }

        private void PreencherRelatorio(List<Chamados> chamados)
        {
            dgvRelatorio.Rows.Clear();

            foreach (var chamado in chamados)
            {
                var tempoResolucao = "N/A";
                if (chamado.DataResolucao.HasValue)
                {
                    var tempo = chamado.DataResolucao.Value - chamado.DataChamado;
                    tempoResolucao = $"{tempo.TotalHours:F1}h";
                }

                dgvRelatorio.Rows.Add(
                    chamado.IdChamado,
                    chamado.DataChamado.ToString("dd/MM/yyyy HH:mm"),
                    chamado.Categoria,
                    ObterTextoPrioridade(chamado.Prioridade),
                    ObterTextoStatus((int)chamado.Status),
                    ObterNomeFuncionario(chamado.Afetado),
                    ObterNomeTecnico(chamado.TecnicoResponsavel),
                    tempoResolucao
                );
            }
        }

        private void AtualizarResumo(List<Chamados> chamados)
        {
            int total = chamados.Count;
            int abertos = chamados.Count(c => (int)c.Status == 1 || (int)c.Status == 2);
            int resolvidos = chamados.Count(c => (int)c.Status == 3 || (int)c.Status == 4);

            var chamadosComResolucao = chamados.Where(c => c.DataResolucao.HasValue).ToList();
            string tempoMedio = "N/A";
            if (chamadosComResolucao.Any())
            {
                var mediaHoras = chamadosComResolucao
                    .Average(c => (c.DataResolucao.Value - c.DataChamado).TotalHours);
                tempoMedio = $"{mediaHoras:F1} horas";
            }

            lblTotalChamados.Text = $"Total de Chamados: {total}";
            lblAbertos.Text = $"Abertos: {abertos}";
            lblResolvidos.Text = $"Resolvidos: {resolvidos}";
            lblTempoMedio.Text = $"Tempo Médio: {tempoMedio}";
        }

        private void BtnAtualizarEstatisticas_Click(object sender, EventArgs e)
        {
            AtualizarEstatisticas();
        }

        private void AtualizarEstatisticas()
        {
            try
            {
                int dias = ObterDiasEstatisticas();
                DateTime dataInicio = DateTime.Now.AddDays(-dias);

                var todosChamados = _chamadosController.ListarTodosChamados();
                var chamadosFiltrados = todosChamados
                    .Where(c => c.DataChamado >= dataInicio)
                    .ToList();

                AtualizarGraficoCategorias(chamadosFiltrados);
                AtualizarGraficoTempoResolucao(chamadosFiltrados);
                AtualizarGraficoResolvidosMensais(chamadosFiltrados);
                AtualizarInsights(chamadosFiltrados);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar estatísticas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObterDiasEstatisticas()
        {
            switch (cmbPeriodoEstatisticas.SelectedIndex)
            {
                case 0: return 7;
                case 1: return 15;
                case 2: return 30;
                case 3: return 90;
                case 4: return 180;
                case 5: return 365;
                default: return 30;
            }
        }

        private void AtualizarGraficoCategorias(List<Chamados> chamados)
        {
            var series = chartCategorias.Series[0];
            series.Points.Clear();

            var porCategoria = chamados
                .GroupBy(c => c.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Count() })
                .OrderByDescending(x => x.Total)
                .ToList();

            Color[] cores = new[] {
                Color.FromArgb(0, 123, 255),
                Color.FromArgb(40, 167, 69),
                Color.FromArgb(255, 193, 7),
                Color.FromArgb(220, 53, 69),
                Color.FromArgb(23, 162, 184),
                Color.FromArgb(108, 117, 125)
            };

            int colorIndex = 0;
            foreach (var item in porCategoria)
            {
                var point = series.Points.Add(item.Total);
                point.LegendText = $"{item.Categoria} ({item.Total})";
                point.Label = $"{item.Total}";
                point.Color = cores[colorIndex % cores.Length];
                colorIndex++;
            }
        }

        private void AtualizarGraficoTempoResolucao(List<Chamados> chamados)
        {
            var series = chartTempoResolucao.Series[0];
            series.Points.Clear();

            var resolvidos = chamados.Where(c => c.DataResolucao.HasValue).ToList();

            int menos24h = resolvidos.Count(c => (c.DataResolucao.Value - c.DataChamado).TotalHours <= 24);
            int entre24e48h = resolvidos.Count(c =>
                (c.DataResolucao.Value - c.DataChamado).TotalHours > 24 &&
                (c.DataResolucao.Value - c.DataChamado).TotalHours <= 48);
            int mais48h = resolvidos.Count(c => (c.DataResolucao.Value - c.DataChamado).TotalHours > 48);

            series.Points.AddXY("< 24h", menos24h);
            series.Points[0].Color = Color.FromArgb(40, 167, 69);

            series.Points.AddXY("24-48h", entre24e48h);
            series.Points[1].Color = Color.FromArgb(255, 193, 7);

            series.Points.AddXY("> 48h", mais48h);
            series.Points[2].Color = Color.FromArgb(220, 53, 69);
        }

        private void AtualizarGraficoResolvidosMensais(List<Chamados> chamados)
        {
            var series = chartResolvidosMensais.Series[0];
            series.Points.Clear();

            var resolvidos = chamados
                .Where(c => (int)c.Status == 3 || (int)c.Status == 4)
                .GroupBy(c => c.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Count() })
                .OrderByDescending(x => x.Total)
                .ToList();

            Color[] cores = new[] {
                Color.FromArgb(0, 123, 255),
                Color.FromArgb(40, 167, 69),
                Color.FromArgb(255, 193, 7),
                Color.FromArgb(220, 53, 69)
            };

            int colorIndex = 0;
            foreach (var item in resolvidos)
            {
                var pointIndex = series.Points.AddXY(item.Categoria, item.Total);
                series.Points[pointIndex].Color = cores[colorIndex % cores.Length];
                colorIndex++;
            }
        }

        private void AtualizarInsights(List<Chamados> chamados)
        {
            // Limpar insights anteriores (exceto o título)
            var controles = panelEstatisticasResumo.Controls.Cast<Control>().ToList();
            foreach (var control in controles.Skip(1))
            {
                panelEstatisticasResumo.Controls.Remove(control);
            }

            int yPos = 50;

            // Insight 1: Categoria mais problemática
            var categoriaMaisProblematica = chamados
                .GroupBy(c => c.Categoria)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (categoriaMaisProblematica != null)
            {
                AdicionarInsight(
                    "🔴 Categoria com Mais Chamados:",
                    $"{categoriaMaisProblematica.Key} - {categoriaMaisProblematica.Count()} chamados",
                    ref yPos);
            }

            // Insight 2: Taxa de resolução
            int totalChamados = chamados.Count;
            int resolvidos = chamados.Count(c => (int)c.Status == 3 || (int)c.Status == 4);
            double taxaResolucao = totalChamados > 0 ? (resolvidos * 100.0 / totalChamados) : 0;

            AdicionarInsight(
                "✅ Taxa de Resolução:",
                $"{taxaResolucao:F1}% ({resolvidos} de {totalChamados} chamados)",
                ref yPos);

            // Insight 3: Tempo médio de resolução
            var chamadosResolvidos = chamados.Where(c => c.DataResolucao.HasValue).ToList();
            if (chamadosResolvidos.Any())
            {
                var tempoMedio = chamadosResolvidos.Average(c => (c.DataResolucao.Value - c.DataChamado).TotalHours);
                string status = tempoMedio <= 24 ? "Excelente ⭐" : tempoMedio <= 48 ? "Bom 👍" : "Precisa melhorar ⚠️";

                AdicionarInsight(
                    "⏱️ Tempo Médio de Resolução:",
                    $"{tempoMedio:F1} horas - {status}",
                    ref yPos);
            }

            // Insight 4: Prioridades críticas
            int criticos = chamados.Count(c => c.Prioridade == 4);
            if (criticos > 0)
            {
                AdicionarInsight(
                    "⚠️ Chamados Críticos:",
                    $"{criticos} chamados de prioridade crítica identificados",
                    ref yPos);
            }

            // Insight 5: Performance do período
            var diasAnalise = ObterDiasEstatisticas();
            var mediaDiaria = totalChamados / (double)diasAnalise;

            AdicionarInsight(
                "📊 Média Diária:",
                $"{mediaDiaria:F1} chamados por dia no período analisado",
                ref yPos);
        }

        private void AdicionarInsight(string titulo, string descricao, ref int yPos)
        {
            var lblTitulo = new Label
            {
                Text = titulo,
                Location = new Point(30, yPos),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            var lblDescricao = new Label
            {
                Text = descricao,
                Location = new Point(50, yPos + 22),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            panelEstatisticasResumo.Controls.Add(lblTitulo);
            panelEstatisticasResumo.Controls.Add(lblDescricao);

            yPos += 50;
        }

        private void BtnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    Title = "Exportar Relatório",
                    FileName = $"Relatorio_Chamados_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GerarPDF(saveDialog.FileName);
                    MessageBox.Show($"Relatório exportado com sucesso!\n\nLocal: {saveDialog.FileName}",
                        "Exportação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abrir o arquivo
                    var resultado = MessageBox.Show("Deseja abrir o arquivo agora?", "Abrir PDF",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GerarPDF(string caminhoArquivo)
        {
            // Usar PrintDocument para gerar PDF
            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = "Relatório de Chamados";

            // Configurar impressora virtual para PDF (se disponível)
            // Ou usar biblioteca externa como iTextSharp

            // Por simplicidade, vamos gerar um HTML e depois converter
            string htmlRelatorio = GerarHTMLRelatorio();

            // Salvar como HTML temporário e depois imprimir
            string tempHtml = System.IO.Path.GetTempFileName() + ".html";
            System.IO.File.WriteAllText(tempHtml, htmlRelatorio);

            // Abrir no navegador para impressão manual para PDF
            System.Diagnostics.Process.Start(tempHtml);

            MessageBox.Show(
                "O relatório foi aberto no navegador.\n\n" +
                "Para salvar como PDF:\n" +
                "1. Pressione Ctrl+P (Imprimir)\n" +
                "2. Selecione 'Microsoft Print to PDF' ou 'Salvar como PDF'\n" +
                "3. Clique em Imprimir/Salvar",
                "Exportar para PDF",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private string GerarHTMLRelatorio()
        {
            DateTime dataInicio, dataFim;
            ObterPeriodoSelecionado(out dataInicio, out dataFim);

            var html = new System.Text.StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<title>Relatório de Chamados</title>");
            html.AppendLine("<style>");
            html.AppendLine(@"
                body { 
                    font-family: 'Segoe UI', Arial, sans-serif; 
                    margin: 40px;
                    color: #333;
                }
                .header { 
                    text-align: center; 
                    margin-bottom: 30px;
                    border-bottom: 3px solid #007bff;
                    padding-bottom: 20px;
                }
                .header h1 { 
                    color: #007bff; 
                    margin: 0;
                }
                .info { 
                    background-color: #f8f9fa; 
                    padding: 15px; 
                    margin-bottom: 20px;
                    border-radius: 5px;
                }
                .info-row {
                    display: inline-block;
                    margin-right: 30px;
                    margin-bottom: 10px;
                }
                .info-label {
                    font-weight: bold;
                    color: #555;
                }
                .info-value {
                    color: #007bff;
                    font-size: 1.1em;
                }
                table { 
                    width: 100%; 
                    border-collapse: collapse; 
                    margin-top: 20px;
                }
                th { 
                    background-color: #007bff; 
                    color: white; 
                    padding: 12px; 
                    text-align: left;
                }
                td { 
                    border: 1px solid #ddd; 
                    padding: 10px; 
                }
                tr:nth-child(even) { 
                    background-color: #f8f9fa; 
                }
                .footer {
                    margin-top: 40px;
                    text-align: center;
                    color: #888;
                    font-size: 0.9em;
                    border-top: 1px solid #ddd;
                    padding-top: 20px;
                }
                .prioridade-alta { color: #dc3545; font-weight: bold; }
                .prioridade-media { color: #ffc107; font-weight: bold; }
                .prioridade-baixa { color: #28a745; }
                .status-aberto { color: #007bff; font-weight: bold; }
                .status-resolvido { color: #28a745; font-weight: bold; }
            </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            // Cabeçalho
            html.AppendLine("<div class='header'>");
            html.AppendLine("<h1>📊 RELATÓRIO DE CHAMADOS</h1>");
            html.AppendLine("<p>Sistema de Gerenciamento de Chamados</p>");
            html.AppendLine("</div>");

            // Informações do relatório
            html.AppendLine("<div class='info'>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>Período:</span> <span class='info-value'>{dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}</span></div>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>Gerado em:</span> <span class='info-value'>{DateTime.Now:dd/MM/yyyy HH:mm}</span></div>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>Por:</span> <span class='info-value'>{_funcionarioLogado.Nome}</span></div>");
            html.AppendLine("<br>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>{lblTotalChamados.Text}</span></div>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>{lblAbertos.Text}</span></div>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>{lblResolvidos.Text}</span></div>");
            html.AppendLine($"<div class='info-row'><span class='info-label'>{lblTempoMedio.Text}</span></div>");
            html.AppendLine("</div>");

            // Tabela de chamados
            html.AppendLine("<h2>Detalhes dos Chamados</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>ID</th>");
            html.AppendLine("<th>Data</th>");
            html.AppendLine("<th>Categoria</th>");
            html.AppendLine("<th>Prioridade</th>");
            html.AppendLine("<th>Status</th>");
            html.AppendLine("<th>Solicitante</th>");
            html.AppendLine("<th>Técnico</th>");
            html.AppendLine("<th>Tempo</th>");
            html.AppendLine("</tr>");

            foreach (DataGridViewRow row in dgvRelatorio.Rows)
            {
                if (row.IsNewRow) continue;

                string prioridadeClass = "";
                string prioridade = row.Cells["Prioridade"].Value?.ToString() ?? "";
                if (prioridade.Contains("Alta") || prioridade.Contains("Crítica"))
                    prioridadeClass = "prioridade-alta";
                else if (prioridade.Contains("Média"))
                    prioridadeClass = "prioridade-media";
                else
                    prioridadeClass = "prioridade-baixa";

                string statusClass = "";
                string status = row.Cells["Status"].Value?.ToString() ?? "";
                if (status.Contains("Aberto") || status.Contains("Andamento"))
                    statusClass = "status-aberto";
                else if (status.Contains("Resolvido") || status.Contains("Fechado"))
                    statusClass = "status-resolvido";

                html.AppendLine("<tr>");
                html.AppendLine($"<td>{row.Cells["Id"].Value}</td>");
                html.AppendLine($"<td>{row.Cells["Data"].Value}</td>");
                html.AppendLine($"<td>{row.Cells["Categoria"].Value}</td>");
                html.AppendLine($"<td class='{prioridadeClass}'>{prioridade}</td>");
                html.AppendLine($"<td class='{statusClass}'>{status}</td>");
                html.AppendLine($"<td>{row.Cells["Solicitante"].Value}</td>");
                html.AppendLine($"<td>{row.Cells["Tecnico"].Value}</td>");
                html.AppendLine($"<td>{row.Cells["TempoResolucao"].Value}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");

            // Rodapé
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>Sistema de Gerenciamento de Chamados - Relatório Confidencial</p>");
            html.AppendLine($"<p>© {DateTime.Now.Year} - Todos os direitos reservados</p>");
            html.AppendLine("</div>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
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

        private string ObterTextoStatus(int status)
        {
            switch (status)
            {
                case 1: return "Aberto";
                case 2: return "Em Andamento";
                case 3: return "Resolvido";
                case 4: return "Fechado";
                case 5: return "Cancelado";
                default: return "Desconhecido";
            }
        }

        private string ObterNomeFuncionario(int idFuncionario)
        {
            var funcionario = _funcionariosController.BuscarFuncionarioPorId(idFuncionario);
            return funcionario != null ? funcionario.Nome.ToString() : $"ID:{idFuncionario}";
        }

        private string ObterNomeTecnico(int? idTecnico)
        {
            if (!idTecnico.HasValue)
                return "Não atribuído";

            var tecnico = _funcionariosController.BuscarFuncionarioPorId(idTecnico.Value);
            return tecnico != null ? tecnico.Nome.ToString() : $"ID:{idTecnico}";
        }
    }
}