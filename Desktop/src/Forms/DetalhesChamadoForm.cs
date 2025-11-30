using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class DetalhesChamadoForm : Form
    {
        private Chamados _chamado;
        private Funcionarios _funcionarioLogado;
        private ChamadosController _controller;
        private bool _permiteEdicao;
        private RichTextBox txtDescricao;
        private RichTextBox txtContestacoes;
        private Label lblId, lblCategoria, lblStatus, lblPrioridade, lblData, lblSolicitante, lblTecnico;
        private Label lblTituloChamado; 
        private Button btnFechar;
        private Button btnAlterar;
        private Button btnVerHistorico;
        private Panel panelHeader;
        private Panel panelInfo;
        private GroupBox gbDescricao;
        private GroupBox gbContestacoes;

        public DetalhesChamadoForm(Chamados chamado, Funcionarios funcionarioLogado, ChamadosController controller, bool permiteEdicao = false)
        {
            _chamado = chamado;
            _funcionarioLogado = funcionarioLogado;
            _controller = controller;
            _permiteEdicao = permiteEdicao;
            System.Diagnostics.Debug.WriteLine("=== CONSTRUCTOR DetalhesChamadoForm ===");
            System.Diagnostics.Debug.WriteLine($"ID: {chamado?.IdChamado}");
            System.Diagnostics.Debug.WriteLine($"Título: '{chamado?.Titulo ?? "(null)"}'");
            System.Diagnostics.Debug.WriteLine($"Descrição: {chamado?.Descricao?.Length ?? 0} chars");
            System.Diagnostics.Debug.WriteLine($"Primeira linha desc: {chamado?.Descricao?.Substring(0, Math.Min(50, chamado.Descricao?.Length ?? 0))}");
            InitializeComponent();
            PreencherDados();
        }

        private void InitializeComponent()
        {
            // PAINEL DE CABEÇALHO
            this.panelHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(700, 60),
                BackColor = Color.FromArgb(0, 123, 255),
                Dock = DockStyle.Top
            };


            var lblTitulo = new Label
            {
                Text = "DETALHES DO CHAMADO",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            this.panelHeader.Controls.Add(lblTitulo);

            // PAINEL DE INFORMAÇÕES BÁSICAS
            this.panelInfo = new Panel
            {
                Location = new Point(12, 70),
                Size = new Size(660, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            // Coluna 1
            this.lblId = new Label
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(15, 15),
                Size = new Size(620, 20),
            };

            this.lblTituloChamado = new Label
            {
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(15, 40),
                Size = new Size(620, 25),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            this.lblCategoria = new Label
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 70), 
                Size = new Size(200, 20)
            };


            this.lblPrioridade = new Label
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 70),
                Size = new Size(200, 20)
            };

            // Coluna 2
            this.lblStatus = new Label
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(340, 15),
                Size = new Size(300, 20)
            };

            this.lblSolicitante = new Label
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(340, 40),
                Size = new Size(300, 20)
            };

            this.lblTecnico = new Label
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(340, 65),
                Size = new Size(300, 20)
            };

            this.lblData = new Label
            {
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(340, 50),
                Size = new Size(300, 20)
            };

            this.panelInfo.Controls.Add(this.lblTituloChamado);
            this.panelInfo.Controls.AddRange(new Control[] {
                this.lblId, this.lblCategoria, this.lblPrioridade,
                this.lblStatus, this.lblSolicitante, this.lblTecnico, this.lblData
            });



            // GROUPBOX DESCRIÇÃO 
            this.gbDescricao = new GroupBox
            {
                Text = "📋 DESCRIÇÃO DO PROBLEMA",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(12, 180),
                Size = new Size(660, 220), 
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            this.txtDescricao = new RichTextBox
            {
                Location = new Point(15, 25),
                Size = new Size(630, 180), 
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true, 
                AcceptsTab = false,
                //AcceptsReturn = true 
            };

            this.gbDescricao.Controls.Add(this.txtDescricao);

            // GROUPBOX CONTESTAÇÕES
            this.gbContestacoes = new GroupBox
            {
                Text = "⚠️ CONTESTAÇÕES",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(12, 410), 
                Size = new Size(660, 150),
                ForeColor = Color.FromArgb(220, 53, 69)
            };
            this.gbContestacoes.Controls.Add(this.btnVerHistorico);
            this.gbContestacoes.Controls.Add(this.txtContestacoes);
            this.gbContestacoes.Size = new Size(660, 150);


            this.txtContestacoes = new RichTextBox
            {
                Location = new Point(15, 25),
                Size = new Size(630, 100),
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true,
                AcceptsTab = false,
                //AcceptsReturn = true
            };


            // BOTÕES
            this.btnVerHistorico = new Button
            {
                Text = "Ver Histórico Completo",
                Location = new Point(332, 570),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(23, 162, 184), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            this.btnVerHistorico.FlatAppearance.BorderSize = 0;
            this.btnVerHistorico.Click += BtnVerHistorico_Click;

            this.btnAlterar = new Button
            {
                Text = "Alterar Status",
                Location = new Point(487, 565), 
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Visible = _permiteEdicao && _funcionarioLogado.NivelAcesso >= 2
            };
            this.btnAlterar.FlatAppearance.BorderSize = 0;
            this.btnAlterar.Click += BtnAlterar_Click;

            this.btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(582, 565), 
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.Click += BtnFechar_Click;

            // CONFIGURAÇÃO DO FORM 
            this.Text = _permiteEdicao ? "Detalhes do Chamado - Gerenciamento" : "Detalhes do Chamado - Visualização";
            this.ClientSize = new Size(684, 620); 
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.Controls.AddRange(new Control[] {
                this.panelHeader,
                this.panelInfo,
                this.gbDescricao,
                this.gbContestacoes,
                this.btnAlterar,
                this.btnVerHistorico,
                this.btnFechar
            });
        }

        /// <summary>
        /// Formata a descrição para melhor legibilidade
        /// </summary>
        private string FormatarDescricao(string descricao)
        {
            if (string.IsNullOrEmpty(descricao))
                return descricao;

            try
            {
                // Debug: Ver texto original
                System.Diagnostics.Debug.WriteLine("=== TEXTO ORIGINAL ===");
                System.Diagnostics.Debug.WriteLine(descricao);
                System.Diagnostics.Debug.WriteLine("======================");

                // Lista de palabras chave SIN espacios
                string[] chavesComuns = new[] {
                    "TÍTULO:",
                    "TITULO:",
                    "DESCRIÇÃO:",
                    "DESCRICAO: ",
                    "AFETADOS: ",
                    "AFETADO: ",
                    "IMPEDE TRABALHO: ",
                    "IMPEDE: ",
                    "PRIORIDADE:",
                    "URGENTE:",
                    "OBSERVAÇÃO:",
                    "OBSERVACAO:",
                    "DETALHES:",
                    "PROBLEMA:",
                    "SOLICITAÇÃO:",
                    "SOLICITACAO:"
                };

                string resultado = descricao;

                // Procesar cada palabra clave
                foreach (var chave in chavesComuns)
                {
                    // Buscar todas las ocurrencias
                    int index = 0;
                    while ((index = resultado.IndexOf(chave, index, StringComparison.OrdinalIgnoreCase)) >= 0)
                    {
                        // Agregar salto ANTES (si no está al inicio)
                        if (index > 0 && resultado[index - 1] != '\n')
                        {
                            resultado = resultado.Insert(index, "\r\n\r\n");
                            index += 4; // 4 caracteres: \r\n\r\n
                        }

                        // Buscar fin de la clave
                        int finChave = index + chave.Length;

                        // Si hay texto inmediato después (sin espacio)
                        if (finChave < resultado.Length)
                        {
                            char siguienteChar = resultado[finChave];

                            // Si no hay espacio ni salto, agregar salto
                            if (siguienteChar != ' ' && siguienteChar != '\r' && siguienteChar != '\n')
                            {
                                resultado = resultado.Insert(finChave, "\r\n");
                                finChave += 2;
                            }
                            // Si hay espacio, convertir a salto
                            else if (siguienteChar == ' ')
                            {
                                resultado = resultado.Remove(finChave, 1);
                                resultado = resultado.Insert(finChave, "\r\n");
                                finChave += 2;
                            }
                        }

                        index = finChave;
                    }
                }

                // Limpiar múltiples saltos
                while (resultado.Contains("\r\n\r\n\r\n"))
                {
                    resultado = resultado.Replace("\r\n\r\n\r\n", "\r\n\r\n");
                }

                // Debug: Ver texto formateado
                System.Diagnostics.Debug.WriteLine("=== TEXTO FORMATEADO ===");
                System.Diagnostics.Debug.WriteLine(resultado);
                System.Diagnostics.Debug.WriteLine("========================");

                return resultado.Trim();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao formatar: {ex.Message}");
                return descricao;
            }
        }

        private void PreencherDados()
        {
            try
            {
                // ========================================
                // INFORMAÇÕES BÁSICAS
                // ========================================
                lblId.Text = $"Chamado #{_chamado.IdChamado}";

                // ⭐ TÍTULO (Mostrar "Sem título" si está vacío)
                if (!string.IsNullOrWhiteSpace(_chamado.Titulo))
                {
                    lblTituloChamado.Text = $"📋 {_chamado.Titulo}";
                    lblTituloChamado.Visible = true;
                }
                else
                {
                    lblTituloChamado.Text = "📋 Sem título";
                    lblTituloChamado.Visible = true;
                }

                // CATEGORIA E PRIORIDADE
                lblCategoria.Text = $"Categoria: {_chamado.Categoria}";
                lblPrioridade.Text = $"Prioridade: {ObterTextoPrioridade(_chamado.Prioridade)}";
                lblPrioridade.ForeColor = ObterCorPrioridade(_chamado.Prioridade);

                // STATUS
                lblStatus.Text = $"Status: {ObterTextoStatus((int)_chamado.Status)}";
                lblStatus.ForeColor = ObterCorStatus((int)_chamado.Status);

                // SOLICITANTE
                lblSolicitante.Text = $"Solicitante: ID {_chamado.Afetado}";

                // TÉCNICO RESPONSÁVEL
                if (_chamado.TecnicoResponsavel.HasValue)
                {
                    lblTecnico.Text = $"Técnico: ID {_chamado.TecnicoResponsavel.Value}";
                }
                else
                {
                    lblTecnico.Text = "Técnico: Não atribuído";
                    lblTecnico.ForeColor = Color.Gray;
                }

                // DATA
                lblData.Text = $"Registrado em: {_chamado.DataChamado:dd/MM/yyyy HH:mm}";
                if (_chamado.DataResolucao.HasValue)
                {
                    lblData.Text += $" | Resolvido em: {_chamado.DataResolucao:dd/MM/yyyy HH:mm}";
                }

                // ========================================
                // DESCRIÇÃO (COM FORMATAÇÃO)
                // ========================================
                if (!string.IsNullOrWhiteSpace(_chamado.Descricao))
                {
                    string descricaoFormatada = FormatarDescricao(_chamado.Descricao);
                    txtDescricao.Text = descricaoFormatada;

                    // Debug
                    System.Diagnostics.Debug.WriteLine($"=== DESCRIÇÃO CARREGADA ===");
                    System.Diagnostics.Debug.WriteLine($"Original: {_chamado.Descricao.Substring(0, Math.Min(100, _chamado.Descricao.Length))}");
                    System.Diagnostics.Debug.WriteLine($"Formatada: {descricaoFormatada.Substring(0, Math.Min(100, descricaoFormatada.Length))}");
                }
                else
                {
                    txtDescricao.Text = "Sem descrição disponível.";
                    System.Diagnostics.Debug.WriteLine("⚠️ DESCRIÇÃO VAZIA!");
                }

                // ========================================
                // CONTESTAÇÕES
                // ========================================
                if (!string.IsNullOrWhiteSpace(_chamado.Contestacoes))
                {
                    txtContestacoes.Text = _chamado.Contestacoes;
                    gbContestacoes.Visible = true;

                    System.Diagnostics.Debug.WriteLine($"=== CONTESTAÇÕES CARREGADAS ===");
                    System.Diagnostics.Debug.WriteLine(_chamado.Contestacoes);
                }
                else
                {
                    txtContestacoes.Text = "Sem contestações registradas.";
                    gbContestacoes.Visible = true;
                    txtContestacoes.ForeColor = Color.Gray;

                    System.Diagnostics.Debug.WriteLine("ℹ️ Sem contestações");
                }

                // ========================================
                // LOGS DE DEBUG
                // ========================================
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("=== RESUMO DO CHAMADO ===");
                System.Diagnostics.Debug.WriteLine($"ID: {_chamado.IdChamado}");
                System.Diagnostics.Debug.WriteLine($"Título: {_chamado.Titulo ?? "(vazio)"}");
                System.Diagnostics.Debug.WriteLine($"Descrição length: {_chamado.Descricao?.Length ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Categoria: {_chamado.Categoria}");
                System.Diagnostics.Debug.WriteLine($"Status: {_chamado.Status}");
                System.Diagnostics.Debug.WriteLine($"Prioridade: {_chamado.Prioridade}");
                System.Diagnostics.Debug.WriteLine("========================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO em PreencherDados: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                MessageBox.Show(
                    $"Erro ao preencher dados do chamado:\n\n{ex.Message}\n\n" +
                    $"ID do Chamado: {_chamado?.IdChamado}\n" +
                    $"Título: {_chamado?.Titulo ?? "(null)"}\n" +
                    $"Descrição length: {_chamado?.Descricao?.Length ?? 0}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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

        private Color ObterCorStatus(int status)
        {
            switch (status)
            {
                case 1: return Color.FromArgb(0, 123, 255);
                case 2: return Color.FromArgb(255, 193, 7);
                case 3: return Color.FromArgb(40, 167, 69);
                case 4: return Color.FromArgb(108, 117, 125);
                case 5: return Color.FromArgb(220, 53, 69);
                default: return Color.Black;
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

        private Color ObterCorPrioridade(int prioridade)
        {
            switch (prioridade)
            {
                case 1: return Color.FromArgb(40, 167, 69);
                case 2: return Color.FromArgb(0, 123, 255);
                case 3: return Color.FromArgb(255, 193, 7);
                case 4: return Color.FromArgb(220, 53, 69);
                default: return Color.Black;
            }
        }

        private void BtnAlterar_Click(object sender, EventArgs e)
        {
            var formStatus = new AlterarStatusForm();
            if (formStatus.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _controller.AlterarStatus(_chamado.IdChamado, formStatus.StatusSelecionado);
                    MessageBox.Show("Status alterado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao alterar status: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnVerHistorico_Click(object sender, EventArgs e)
        {
            var formHistorico = new ContestacaoForm(_chamado, _funcionarioLogado);
            formHistorico.ShowDialog();

            // _chamado = _controller.BuscarPorId(_chamado.IdChamado);
            // PreencherDados();
        }
    }
}