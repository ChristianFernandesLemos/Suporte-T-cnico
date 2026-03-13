using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class VisualizarChamadosForm : Form
    {
        private ChamadosController _chamadosController;
        private FuncionariosController _funcionariosController; // ⭐ AGREGADO
        private Funcionarios _funcionarioLogado;
        private DataGridView dgvChamados;
        private ComboBox cmbFiltroStatus;
        private ComboBox cmbFiltroPrioridade;
        private ComboBox cmbFiltroCategoria;
        private TextBox txtPesquisa;
        private Button btnPesquisar;
        private Button btnLimparFiltros;
        private Button btnAtualizar;
        private Button btnDetalhes;
        private Button btnNovoContestacao;
        private Label lblTotalChamados;
        private GroupBox gbFiltros;
        private List<Chamados> _chamadosCarregados;

        // ⭐ MODIFICADO: Ahora recibe FuncionariosController también
        public VisualizarChamadosForm(Funcionarios funcionario, ChamadosController chamadosController, FuncionariosController funcionariosController)
        {
            _funcionarioLogado = funcionario;
            _chamadosController = chamadosController;
            _funcionariosController = funcionariosController; // ⭐ AGREGADO
            _chamadosCarregados = new List<Chamados>();
            InitializeComponent();
            this.Text = GetTituloFormulario();
            ConfigurarFormulario();
            CarregarChamados();
        }

        private void InitializeComponent()
        {
            this.dgvChamados = new DataGridView();
            this.cmbFiltroStatus = new ComboBox();
            this.cmbFiltroPrioridade = new ComboBox();
            this.cmbFiltroCategoria = new ComboBox();
            this.txtPesquisa = new TextBox();
            this.btnPesquisar = new Button();
            this.btnLimparFiltros = new Button();
            this.btnAtualizar = new Button();
            this.btnDetalhes = new Button();
            this.btnNovoContestacao = new Button();
            this.lblTotalChamados = new Label();
            this.gbFiltros = new GroupBox();

            this.SuspendLayout();

            // gbFiltros
            this.gbFiltros.Text = "Filtros";
            this.gbFiltros.Location = new Point(12, 12);
            this.gbFiltros.Size = new Size(1000, 80);
            this.gbFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // txtPesquisa
            this.txtPesquisa.Location = new Point(15, 25);
            this.txtPesquisa.Size = new Size(200, 20);
            this.txtPesquisa.ForeColor = Color.Gray;
            this.txtPesquisa.Text = "Pesquisar por título ou descrição...";
            this.txtPesquisa.GotFocus += TxtPesquisa_GotFocus;
            this.txtPesquisa.LostFocus += TxtPesquisa_LostFocus;

            // cmbFiltroStatus
            this.cmbFiltroStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroStatus.Location = new Point(225, 25);
            this.cmbFiltroStatus.Size = new Size(120, 21);

            // cmbFiltroPrioridade
            this.cmbFiltroPrioridade.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroPrioridade.Location = new Point(355, 25);
            this.cmbFiltroPrioridade.Size = new Size(120, 21);

            // cmbFiltroCategoria
            this.cmbFiltroCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroCategoria.Location = new Point(485, 25);
            this.cmbFiltroCategoria.Size = new Size(120, 21);

            // btnPesquisar
            this.btnPesquisar.Text = "Pesquisar";
            this.btnPesquisar.Location = new Point(620, 23);
            this.btnPesquisar.Size = new Size(80, 25);
            this.btnPesquisar.Click += new EventHandler(this.btnPesquisar_Click);

            // btnLimparFiltros
            this.btnLimparFiltros.Text = "Limpar";
            this.btnLimparFiltros.Location = new Point(710, 23);
            this.btnLimparFiltros.Size = new Size(70, 25);
            this.btnLimparFiltros.Click += new EventHandler(this.btnLimparFiltros_Click);

            // btnAtualizar
            this.btnAtualizar.Text = "Atualizar";
            this.btnAtualizar.Location = new Point(790, 23);
            this.btnAtualizar.Size = new Size(70, 25);
            this.btnAtualizar.Click += new EventHandler(this.btnAtualizar_Click);

            this.gbFiltros.Controls.AddRange(new Control[]
            {
                this.txtPesquisa, this.cmbFiltroStatus, this.cmbFiltroPrioridade,
                this.cmbFiltroCategoria, this.btnPesquisar, this.btnLimparFiltros,
                this.btnAtualizar
            });

            // dgvChamados
            this.dgvChamados.Location = new Point(12, 110);
            this.dgvChamados.Size = new Size(1000, 400);
            this.dgvChamados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvChamados.AllowUserToAddRows = false;
            this.dgvChamados.AllowUserToDeleteRows = false;
            this.dgvChamados.ReadOnly = true;
            this.dgvChamados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvChamados.MultiSelect = false;
            this.dgvChamados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChamados.RowHeadersVisible = false;
            this.dgvChamados.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvChamados_CellDoubleClick);

            // lblTotalChamados
            this.lblTotalChamados.Location = new Point(12, 520);
            this.lblTotalChamados.Size = new Size(200, 15);
            this.lblTotalChamados.Text = "Total: 0 chamados";

            // btnDetalhes
            this.btnDetalhes = new Button();
            this.btnDetalhes.Text = "Ver Detalhes";
            this.btnDetalhes.Location = new Point(535, 20);
            this.btnDetalhes.Size = new Size(90, 30);
            this.btnDetalhes.BackColor = Color.FromArgb(23, 162, 184);
            this.btnDetalhes.ForeColor = Color.White;
            this.btnDetalhes.FlatStyle = FlatStyle.Flat;
            this.btnDetalhes.Click += new EventHandler(this.btnDetalhes_Click);

            // btnNovoContestacao
            this.btnNovoContestacao.Text = "Contestar";
            this.btnNovoContestacao.Location = new Point(820, 520);
            this.btnNovoContestacao.Size = new Size(90, 30);
            this.btnNovoContestacao.Click += new EventHandler(this.btnNovoContestacao_Click);

            // VisualizarChamadosForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1024, 568);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.dgvChamados);
            this.Controls.Add(this.lblTotalChamados);
            this.Controls.Add(this.btnDetalhes);
            this.Controls.Add(this.btnNovoContestacao);
            this.Name = "VisualizarChamadosForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Visualizar Chamados - Sistema de Chamados";
            this.WindowState = FormWindowState.Maximized;
            this.ResumeLayout(false);
        }

        private void TxtPesquisa_GotFocus(object sender, EventArgs e)
        {
            if (txtPesquisa.Text == "Pesquisar por nome ou email...")
            {
                txtPesquisa.Text = "";
                txtPesquisa.ForeColor = Color.Black;
            }
        }

        private void TxtPesquisa_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisa.Text))
            {
                txtPesquisa.Text = "Pesquisar por nome ou email...";
                txtPesquisa.ForeColor = Color.Gray;
            }
        }

        private void ConfigurarFormulario()
        {
            try
            {
                cmbFiltroStatus.Items.Add("Todos");
                cmbFiltroStatus.Items.Add("Aberto");
                cmbFiltroStatus.Items.Add("Em Andamento");
                cmbFiltroStatus.Items.Add("Resolvido");
                cmbFiltroStatus.Items.Add("Fechado");
                cmbFiltroStatus.Items.Add("Cancelado");
                cmbFiltroStatus.SelectedIndex = 0;

                cmbFiltroPrioridade.Items.Add("Todas");
                cmbFiltroPrioridade.Items.Add("Baixa");
                cmbFiltroPrioridade.Items.Add("Média");
                cmbFiltroPrioridade.Items.Add("Alta");
                cmbFiltroPrioridade.Items.Add("Crítica");
                cmbFiltroPrioridade.SelectedIndex = 0;

                cmbFiltroCategoria.Items.Add("Todas");
                cmbFiltroCategoria.Items.AddRange(new string[]
                {
                    "Hardware", "Software", "Rede", "Outro"
                });
                cmbFiltroCategoria.SelectedIndex = 0;

                ConfigurarDataGridView();
                ConfigurarPermissoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao configurar formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDataGridView()
        {
            dgvChamados.Columns.Clear();

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IdChamado",
                HeaderText = "ID",
                Width = 60,
                ReadOnly = true
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataChamado",
                HeaderText = "Data",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Titulo",
                HeaderText = "Título",
                Width = 180,
                ReadOnly = true
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoria",
                Width = 100,
                ReadOnly = true
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Descricao",
                HeaderText = "Descrição",
                Width = 220,
                ReadOnly = true
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prioridade",
                HeaderText = "Prioridade",
                Width = 80,
                ReadOnly = true
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 100,
                ReadOnly = true
            });

            if (_funcionarioLogado.NivelAcesso >= 2)
            {
                dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Afetado",
                    HeaderText = "Solicitante",
                    Width = 150,
                    ReadOnly = true
                });
            }

            if (_funcionarioLogado.NivelAcesso >= 3)
            {
                dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TecnicoResponsavel",
                    HeaderText = "Técnico",
                    Width = 150,
                    ReadOnly = true
                });
            }

            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvChamados.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            dgvChamados.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void ConfigurarPermissoes()
        {
            if (_funcionarioLogado.NivelAcesso == 1)
            {
                btnNovoContestacao.Visible = true;
            }
            else
            {
                btnNovoContestacao.Visible = false;
            }
        }

        private string GetTituloFormulario()
        {
            switch (_funcionarioLogado.NivelAcesso)
            {
                case 1:
                    return "Meus Chamados - Sistema de Chamados";
                case 2:
                    return "Chamados Atribuídos - Sistema de Chamados";
                case 3:
                    return "Todos os Chamados - Sistema de Chamados";
                default:
                    return "Chamados - Sistema de Chamados";
            }
        }

        private void CarregarChamados()
        {
            try
            {
                dgvChamados.DataSource = null;
                dgvChamados.Rows.Clear();

                switch (_funcionarioLogado.NivelAcesso)
                {
                    case 1:
                        _chamadosCarregados = _chamadosController.ListarChamadosPorFuncionario(_funcionarioLogado.Id);
                        break;
                    case 2:
                        _chamadosCarregados = _chamadosController.ListarChamadosPorTecnico(_funcionarioLogado.Id);
                        break;
                    case 3:
                        _chamadosCarregados = _chamadosController.ListarTodosChamados();
                        break;
                }

                PreencherDataGridView(_chamadosCarregados);
                AtualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreencherDataGridView(List<Chamados> chamados)
        {
            dgvChamados.Rows.Clear();

            foreach (var chamado in chamados)
            {
                var row = new DataGridViewRow();

                row.Cells.Add(new DataGridViewTextBoxCell { Value = chamado.IdChamado });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = chamado.DataChamado });

                string tituloMostrar = !string.IsNullOrWhiteSpace(chamado.Titulo)
                    ? chamado.Titulo
                    : "Sem título";
                row.Cells.Add(new DataGridViewTextBoxCell { Value = tituloMostrar });

                row.Cells.Add(new DataGridViewTextBoxCell { Value = chamado.Categoria });

                string descricaoResumida = chamado.Descricao.Length > 40
                    ? chamado.Descricao.Substring(0, 40) + "..."
                    : chamado.Descricao;
                row.Cells.Add(new DataGridViewTextBoxCell { Value = descricaoResumida });

                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterTextoPrioridade(chamado.Prioridade) });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterTextoStatus((int)chamado.Status) });

                // ⭐ CORREGIDO: Mostrar nombres en lugar de IDs
                if (_funcionarioLogado.NivelAcesso >= 2)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterNomeAfetado(chamado.Afetado) });
                }

                if (_funcionarioLogado.NivelAcesso >= 3)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterNomeTecnico(chamado.TecnicoResponsavel) });
                }

                ColorirLinhaPorPrioridade(row, chamado.Prioridade);

                dgvChamados.Rows.Add(row);
            }
        }

        private void ColorirLinhaPorPrioridade(DataGridViewRow row, int prioridade)
        {
            switch (prioridade)
            {
                case 1:
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    break;
                case 2:
                    break;
                case 3:
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    break;
                case 4:
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    break;
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

        // ⭐ CORREGIDO: Ahora usa FuncionariosController
        private string ObterNomeAfetado(int idFuncionario)
        {
            var funcionario = _funcionariosController.BuscarFuncionarioPorId(idFuncionario);
            return funcionario != null ? funcionario.Nome.ToString() : $"ID:{idFuncionario}";
        }

        // ⭐ CORREGIDO: Ahora usa FuncionariosController
        private string ObterNomeTecnico(int? idTecnico)
        {
            if (!idTecnico.HasValue)
                return "Não atribuído";

            var tecnico = _funcionariosController.BuscarFuncionarioPorId(idTecnico.Value);
            return tecnico != null ? tecnico.Nome.ToString() : $"ID:{idTecnico}";
        }

        private void AtualizarContador()
        {
            lblTotalChamados.Text = $"Total: {dgvChamados.Rows.Count} chamados";
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            FiltrarChamados();
        }

        private void txtPesquisa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                FiltrarChamados();
            }
        }

        private void FiltrarChamados()
        {
            try
            {
                var chamadosFiltrados = _chamadosCarregados.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(txtPesquisa.Text))
                {
                    string termo = txtPesquisa.Text.ToLower();
                    chamadosFiltrados = chamadosFiltrados.Where(c =>
                        c.Descricao.ToLower().Contains(termo) ||
                        c.Categoria.ToLower().Contains(termo) ||
                        (!string.IsNullOrWhiteSpace(c.Titulo) && c.Titulo.ToLower().Contains(termo)));
                }

                if (cmbFiltroStatus.SelectedIndex > 0)
                {
                    int statusFiltro = cmbFiltroStatus.SelectedIndex;
                    chamadosFiltrados = chamadosFiltrados.Where(c => (int)c.Status == statusFiltro);
                }

                if (cmbFiltroPrioridade.SelectedIndex > 0)
                {
                    int prioridadeFiltro = cmbFiltroPrioridade.SelectedIndex;
                    chamadosFiltrados = chamadosFiltrados.Where(c => c.Prioridade == prioridadeFiltro);
                }

                if (cmbFiltroCategoria.SelectedIndex > 0)
                {
                    string categoriaFiltro = cmbFiltroCategoria.Text;
                    chamadosFiltrados = chamadosFiltrados.Where(c => c.Categoria == categoriaFiltro);
                }

                PreencherDataGridView(chamadosFiltrados.ToList());
                AtualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimparFiltros_Click(object sender, EventArgs e)
        {
            txtPesquisa.Clear();
            cmbFiltroStatus.SelectedIndex = 0;
            cmbFiltroPrioridade.SelectedIndex = 0;
            cmbFiltroCategoria.SelectedIndex = 0;
            PreencherDataGridView(_chamadosCarregados);
            AtualizarContador();
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            CarregarChamados();
        }

        private void dgvChamados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnDetalhes_Click(sender, e);
            }
        }

        private void btnDetalhes_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvChamados.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Selecione um chamado para ver os detalhes.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int idChamado = Convert.ToInt32(dgvChamados.SelectedRows[0].Cells["IdChamado"].Value);
                var chamado = _chamadosCarregados.FirstOrDefault(c => c.IdChamado == idChamado);

                if (chamado != null)
                {
                    var formDetalhes = new DetalhesChamadoForm(chamado, _funcionarioLogado, _chamadosController, permiteEdicao: false);
                    formDetalhes.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir detalhes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNovoContestacao_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvChamados.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Selecione um chamado para contestar.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int idChamado = Convert.ToInt32(dgvChamados.SelectedRows[0].Cells["IdChamado"].Value);
                var chamado = _chamadosCarregados.FirstOrDefault(c => c.IdChamado == idChamado);

                if (chamado != null && chamado.Afetado == _funcionarioLogado.Id)
                {
                    var formContestacao = new ContestacaoForm(chamado, _funcionarioLogado);
                    if (formContestacao.ShowDialog() == DialogResult.OK)
                    {
                        CarregarChamados();
                    }
                }
                else
                {
                    MessageBox.Show("Você só pode contestar seus próprios chamados.", "Acesso Negado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir contestação: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}