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
        private Button btnFechar;
        private Button btnAlterar;
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
                Size = new Size(200, 20)
            };

            this.lblCategoria = new Label
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 40),
                Size = new Size(200, 20)
            };

            this.lblPrioridade = new Label
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 65),
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
                Location = new Point(15, 82),
                Size = new Size(630, 15)
            };

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
                Size = new Size(660, 220), // ← Era 180, ahora 220
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            // ⚠️ CONFIGURACIÓN CRÍTICA DEL TEXTBOX
            this.txtDescricao = new RichTextBox
            {
                Location = new Point(15, 25),
                Size = new Size(630, 180), // ← Era 140, ahora 180
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true, // ← IMPORTANTE: Wrap de palabras
                AcceptsTab = false,
                //AcceptsReturn = true 
            };

            this.gbDescricao.Controls.Add(this.txtDescricao);

            // GROUPBOX CONTESTAÇÕES
            this.gbContestacoes = new GroupBox
            {
                Text = "⚠️ CONTESTAÇÕES",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(12, 410), // ← Ajustado (era 370)
                Size = new Size(660, 140),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

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

            this.gbContestacoes.Controls.Add(this.txtContestacoes);

            // BOTÕES 
            this.btnAlterar = new Button
            {
                Text = "Alterar Status",
                Location = new Point(487, 565), // ← Ajustado (era 525)
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
                Location = new Point(582, 565), // ← Ajustado (era 525)
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
                    "DESCRICAO:",
                    "AFETADOS:",
                    "AFETADO:",
                    "IMPEDE TRABALHO:",
                    "IMPEDE:",
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
                // Informações básicas
                lblId.Text = $"Chamado #{_chamado.IdChamado}";
                lblCategoria.Text = $"Categoria: {_chamado.Categoria}";
                lblPrioridade.Text = $"Prioridade: {ObterTextoPrioridade(_chamado.Prioridade)}";
                lblPrioridade.ForeColor = ObterCorPrioridade(_chamado.Prioridade);
                lblData.Text = $"Aberto em: {_chamado.DataChamado:dd/MM/yyyy HH:mm}";

                lblStatus.Text = $"Status: {ObterTextoStatus((int)_chamado.Status)}";
                lblStatus.ForeColor = ObterCorStatus((int)_chamado.Status);
                lblSolicitante.Text = $"Solicitante: ID {_chamado.Afetado}";
                lblTecnico.Text = _chamado.TecnicoResponsavel.HasValue
                    ? $"Técnico: ID {_chamado.TecnicoResponsavel}"
                    : "Técnico: Não atribuído";

                // ⚠️ APLICAR FORMATAÇÃO
                string descricaoFormatada = FormatarDescricao(_chamado.Descricao);
                txtDescricao.Text = descricaoFormatada;

                // Contestações
                if (!string.IsNullOrEmpty(_chamado.Contestacoes))
                {
                    txtContestacoes.Text = _chamado.Contestacoes;
                    gbContestacoes.ForeColor = Color.FromArgb(220, 53, 69);
                }
                else
                {
                    txtContestacoes.Text = "Nenhuma contestação registrada.";
                    txtContestacoes.ForeColor = Color.Gray;
                    gbContestacoes.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao preencher dados: {ex.Message}\n\n{ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erro em PreencherDados: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
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
    }
}