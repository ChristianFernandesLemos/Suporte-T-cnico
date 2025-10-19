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
    public partial class GerenciarChamadosForm : Form
    {
        private ChamadosController _chamadosController;
        private FuncionariosController _funcionariosController;
        private Funcionarios _funcionarioLogado;
        private DataGridView dgvChamados;
        private ComboBox cmbFiltroStatus;
        private ComboBox cmbFiltroPrioridade;
        private ComboBox cmbFiltroTecnico;
        private TextBox txtPesquisa;
        private Button btnPesquisar;
        private Button btnLimparFiltros;
        private Button btnAtualizar;
        private Button btnAtribuir;
        private Button btnAlterarStatus;
        private Button btnAlterarPrioridade;
        private Button btnDetalhes;
        private Button btnFechar;
        private Button btnReabrir;
        private GroupBox gbFiltros;
        private GroupBox gbAcoes;
        private Label lblTotalChamados;
        private List<Chamados> _chamadosCarregados;

        public GerenciarChamadosForm(Funcionarios funcionario, ChamadosController chamadosController, FuncionariosController funcionariosController)
        {
            _funcionarioLogado = funcionario;
            _chamadosController = chamadosController;
            _funcionariosController = funcionariosController;
            _chamadosCarregados = new List<Chamados>();
            InitializeComponent();
            ConfigurarFormulario();
            CarregarChamados();
        }

        private void InitializeComponent()
        {
            this.dgvChamados = new DataGridView();
            this.cmbFiltroStatus = new ComboBox();
            this.cmbFiltroPrioridade = new ComboBox();
            this.cmbFiltroTecnico = new ComboBox();
            this.txtPesquisa = new TextBox();
            this.btnPesquisar = new Button();
            this.btnLimparFiltros = new Button();
            this.btnAtualizar = new Button();
            this.btnAtribuir = new Button();
            this.btnAlterarStatus = new Button();
            this.btnAlterarPrioridade = new Button();
            this.btnFechar = new Button();
            this.btnDetalhes = new Button();
            this.btnReabrir = new Button();
            this.gbFiltros = new GroupBox();
            this.gbAcoes = new GroupBox();
            this.lblTotalChamados = new Label();

            this.SuspendLayout();

            // 
            // gbFiltros
            // 
            this.gbFiltros.Text = "Filtros";
            this.gbFiltros.Location = new Point(12, 12);
            this.gbFiltros.Size = new Size(1000, 80);
            this.gbFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // txtPesquisa
            // 
            this.txtPesquisa.Location = new Point(15, 25);
            this.txtPesquisa.Size = new Size(200, 20);
            this.txtPesquisa.Text = "Pesquisar...";
            this.txtPesquisa.ForeColor = Color.Gray;
            this.txtPesquisa.GotFocus += TxtPesquisa_GotFocus;
            this.txtPesquisa.LostFocus += TxtPesquisa_LostFocus;
           

            // 
            // cmbFiltroStatus
            // 
            this.cmbFiltroStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroStatus.Location = new Point(225, 25);
            this.cmbFiltroStatus.Size = new Size(120, 21);

            // 
            // cmbFiltroPrioridade
            // 
            this.cmbFiltroPrioridade.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroPrioridade.Location = new Point(355, 25);
            this.cmbFiltroPrioridade.Size = new Size(120, 21);

            // 
            // cmbFiltroTecnico
            // 
            this.cmbFiltroTecnico.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroTecnico.Location = new Point(485, 25);
            this.cmbFiltroTecnico.Size = new Size(150, 21);


            //
            // btnDetalhes
            //
            this.btnDetalhes = new Button();
            this.btnDetalhes.Text = "Ver Detalhes";
            this.btnDetalhes.Location = new Point(535, 20);
            this.btnDetalhes.Size = new Size(90, 30);
            this.btnDetalhes.BackColor = Color.FromArgb(23, 162, 184);
            this.btnDetalhes.ForeColor = Color.White;
            this.btnDetalhes.FlatStyle = FlatStyle.Flat;
            this.btnDetalhes.Click += new EventHandler(this.btnDetalhes_Click);

            // 
            // btnPesquisar
            // 
            this.btnPesquisar.Text = "Pesquisar";
            this.btnPesquisar.Location = new Point(650, 23);
            this.btnPesquisar.Size = new Size(80, 25);
            this.btnPesquisar.Click += new EventHandler(this.btnPesquisar_Click);

            // 
            // btnLimparFiltros
            // 
            this.btnLimparFiltros.Text = "Limpar";
            this.btnLimparFiltros.Location = new Point(740, 23);
            this.btnLimparFiltros.Size = new Size(70, 25);
            this.btnLimparFiltros.Click += new EventHandler(this.btnLimparFiltros_Click);

            // 
            // btnAtualizar
            // 
            this.btnAtualizar.Text = "Atualizar";
            this.btnAtualizar.Location = new Point(820, 23);
            this.btnAtualizar.Size = new Size(70, 25);
            this.btnAtualizar.Click += new EventHandler(this.btnAtualizar_Click);

            this.gbFiltros.Controls.AddRange(new Control[]
            {
                this.txtPesquisa, this.cmbFiltroStatus, this.cmbFiltroPrioridade,
                this.cmbFiltroTecnico, this.btnPesquisar, this.btnLimparFiltros, this.btnAtualizar
            });

            // 
            // dgvChamados
            // 
            this.dgvChamados.Location = new Point(12, 110);
            this.dgvChamados.Size = new Size(1000, 350);
            this.dgvChamados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvChamados.AllowUserToAddRows = false;
            this.dgvChamados.AllowUserToDeleteRows = false;
            this.dgvChamados.ReadOnly = true;
            this.dgvChamados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvChamados.MultiSelect = true;
            this.dgvChamados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChamados.RowHeadersVisible = false;

            // 
            // gbAcoes
            // 
            this.gbAcoes.Text = "Ações";
            this.gbAcoes.Location = new Point(12, 470);
            this.gbAcoes.Size = new Size(1000, 60);
            this.gbAcoes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // btnAtribuir
            // 
            this.btnAtribuir.Text = "Atribuir Técnico";
            this.btnAtribuir.Location = new Point(15, 20);
            this.btnAtribuir.Size = new Size(100, 30);
            this.btnAtribuir.BackColor = Color.FromArgb(0, 123, 255);
            this.btnAtribuir.ForeColor = Color.White;
            this.btnAtribuir.FlatStyle = FlatStyle.Flat;
            this.btnAtribuir.Click += new EventHandler(this.btnAtribuir_Click);

            // 
            // btnAlterarStatus
            // 
            this.btnAlterarStatus.Text = "Alterar Status";
            this.btnAlterarStatus.Location = new Point(125, 20);
            this.btnAlterarStatus.Size = new Size(100, 30);
            this.btnAlterarStatus.BackColor = Color.FromArgb(40, 167, 69);
            this.btnAlterarStatus.ForeColor = Color.White;
            this.btnAlterarStatus.FlatStyle = FlatStyle.Flat;
            this.btnAlterarStatus.Click += new EventHandler(this.btnAlterarStatus_Click);

            // 
            // btnAlterarPrioridade
            // 
            this.btnAlterarPrioridade.Text = "Alterar Prioridade";
            this.btnAlterarPrioridade.Location = new Point(235, 20);
            this.btnAlterarPrioridade.Size = new Size(110, 30);
            this.btnAlterarPrioridade.BackColor = Color.FromArgb(255, 193, 7);
            this.btnAlterarPrioridade.ForeColor = Color.Black;
            this.btnAlterarPrioridade.FlatStyle = FlatStyle.Flat;
            this.btnAlterarPrioridade.Click += new EventHandler(this.btnAlterarPrioridade_Click);

            // 
            // btnFechar
            // 
            this.btnFechar.Text = "Fechar";
            this.btnFechar.Location = new Point(355, 20);
            this.btnFechar.Size = new Size(80, 30);
            this.btnFechar.BackColor = Color.FromArgb(108, 117, 125);
            this.btnFechar.ForeColor = Color.White;
            this.btnFechar.FlatStyle = FlatStyle.Flat;
            this.btnFechar.Click += new EventHandler(this.btnFechar_Click);

            // 
            // btnReabrir
            // 
            this.btnReabrir.Text = "Reabrir";
            this.btnReabrir.Location = new Point(445, 20);
            this.btnReabrir.Size = new Size(80, 30);
            this.btnReabrir.BackColor = Color.FromArgb(220, 53, 69);
            this.btnReabrir.ForeColor = Color.White;
            this.btnReabrir.FlatStyle = FlatStyle.Flat;
            this.btnReabrir.Click += new EventHandler(this.btnReabrir_Click);

            this.gbAcoes.Controls.AddRange(new Control[]
            {
                this.btnAtribuir, this.btnAlterarStatus, this.btnAlterarPrioridade,
                this.btnFechar, this.btnReabrir, this.btnDetalhes
            });

            // 
            // lblTotalChamados
            // 
            this.lblTotalChamados.Location = new Point(12, 540);
            this.lblTotalChamados.Size = new Size(200, 15);
            this.lblTotalChamados.Text = "Total: 0 chamados";

            // 
            // GerenciarChamadosForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1024, 568);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.dgvChamados);
            this.Controls.Add(this.gbAcoes);
            this.Controls.Add(this.lblTotalChamados);
            this.Name = "GerenciarChamadosForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Gerenciar Chamados - Sistema de Chamados";
            this.WindowState = FormWindowState.Maximized;
            this.ResumeLayout(false);
        }

        private void TxtPesquisa_GotFocus(object sender, EventArgs e)
        {
            if (txtPesquisa.Text == "Pesquisar...")
            {
                txtPesquisa.Text = "";
                txtPesquisa.ForeColor = Color.Black;
            }
        }

        private void TxtPesquisa_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisa.Text))
            {
                txtPesquisa.Text = "Pesquisar...";
                txtPesquisa.ForeColor = Color.Gray;
            }
        }


        private void ConfigurarFormulario()
        {
            try
            {
                ConfigurarCombos();
                ConfigurarDataGridView();
                ConfigurarPermissoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao configurar formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarCombos()
        {
            // Status
            cmbFiltroStatus.Items.Add("Todos");
            cmbFiltroStatus.Items.Add("Aberto");
            cmbFiltroStatus.Items.Add("Em Andamento");
            cmbFiltroStatus.Items.Add("Resolvido");
            cmbFiltroStatus.Items.Add("Fechado");
            cmbFiltroStatus.SelectedIndex = 0;

            // Prioridades
            cmbFiltroPrioridade.Items.Add("Todas");
            cmbFiltroPrioridade.Items.Add("Baixa");
            cmbFiltroPrioridade.Items.Add("Média");
            cmbFiltroPrioridade.Items.Add("Alta");
            cmbFiltroPrioridade.Items.Add("Crítica");
            cmbFiltroPrioridade.SelectedIndex = 0;

            // Técnicos
            cmbFiltroTecnico.Items.Add("Todos");
            cmbFiltroTecnico.Items.Add("Não Atribuído");

            // Carregar lista de técnicos
            var tecnicos = _funcionariosController.ListarTecnicos();
            foreach (var tecnico in tecnicos)
            {
                cmbFiltroTecnico.Items.Add($"{tecnico.Nome} ({tecnico.Email})");
            }
            cmbFiltroTecnico.SelectedIndex = 0;
        }

        private void ConfigurarDataGridView()
        {
            dgvChamados.Columns.Clear();

            // Configurar colunas
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
                HeaderText = "Data/Hora",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Solicitante",
                HeaderText = "Solicitante",
                Width = 150,
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
                Width = 250,
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

            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TecnicoResponsavel",
                HeaderText = "Técnico Responsável",
                Width = 150,
                ReadOnly = true
            });

            // Configurar aparência
            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvChamados.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            dgvChamados.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void ConfigurarPermissoes()
        {
            // Técnicos podem alterar status e prioridade apenas dos seus chamados
            // Administradores podem fazer tudo
            bool isAdmin = _funcionarioLogado.NivelAcesso >= 3;

            btnAtribuir.Visible = isAdmin;
            btnFechar.Visible = isAdmin;
            btnReabrir.Visible = isAdmin;

            if (!isAdmin)
            {
                // Técnicos têm acesso limitado
                this.Text = "Meus Chamados Atribuídos - Sistema de Chamados";
            }
        }

        private void CarregarChamados()
        {
            try
            {
                dgvChamados.Rows.Clear();

                // Carregar chamados baseado no nível de acesso
                if (_funcionarioLogado.NivelAcesso >= 3) // Admin
                {
                    _chamadosCarregados = _chamadosController.ListarTodosChamados();
                }
                else if (_funcionarioLogado.NivelAcesso == 2) // Técnico
                {
                    _chamadosCarregados = _chamadosController.ListarChamadosPorTecnico(_funcionarioLogado.Id);
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

                // ← REMOVIDO: Checkbox cell
                row.Cells.Add(new DataGridViewTextBoxCell { Value = chamado.IdChamado });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = chamado.DataChamado });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterNomeSolicitante(chamado.Afetado) });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = chamado.Categoria });

                // Descrição resumida
                string descricaoResumida = chamado.Descricao.Length > 60
                    ? chamado.Descricao.Substring(0, 60) + "..."
                    : chamado.Descricao;
                row.Cells.Add(new DataGridViewTextBoxCell { Value = descricaoResumida });

                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterTextoPrioridade(chamado.Prioridade) });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterTextoStatus((int)chamado.Status) });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterNomeTecnico(chamado.TecnicoResponsavel) });

                // Colorir linha baseado na prioridade e status
                ColorirLinha(row, chamado.Prioridade, (int)chamado.Status);

                dgvChamados.Rows.Add(row);
            }
        }

        private void ColorirLinha(DataGridViewRow row, int prioridade, int status)
        {
            // Cor baseada no status
            switch (status)
            {
                case 1: // Aberto - Sem cor especial
                    break;
                case 2: // Em Andamento - Azul claro
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    break;
                case 3: // Resolvido - Verde claro
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    break;
                case 4: // Fechado - Cinza
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.DarkGray;
                    break;
                case 5: // Cancelado - Vermelho claro
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    break;
            }

            // Destacar prioridade crítica com borda vermelha
            if (prioridade == 4 && status != 4) // Crítica e não fechado
            {
                row.DefaultCellStyle.BackColor = Color.LightCoral;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
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

        private string ObterNomeSolicitante(int idFuncionario)
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

        private void AtualizarContador()
        {
            int total = dgvChamados.Rows.Count;
            int selecionados = dgvChamados.SelectedRows.Count; // ← MODIFICADO

            if (selecionados > 0)
                lblTotalChamados.Text = $"Total: {total} chamados | Selecionados: {selecionados}";
            else
                lblTotalChamados.Text = $"Total: {total} chamados";
        }

        private List<int> ObterChamadosSelecionados()
        {
            var selecionados = new List<int>();

            // ← MODIFICADO: Solo usar filas seleccionadas (sin checkboxes)
            if (dgvChamados.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvChamados.SelectedRows)
                {
                    selecionados.Add(Convert.ToInt32(row.Cells["IdChamado"].Value));
                }
            }

            return selecionados;
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            FiltrarChamados();
        }

        private void FiltrarChamados()
        {
            try
            {
                var chamadosFiltrados = _chamadosCarregados.AsEnumerable();

                // Filtro por texto
                if (!string.IsNullOrWhiteSpace(txtPesquisa.Text))
                {
                    string termo = txtPesquisa.Text.ToLower();
                    chamadosFiltrados = chamadosFiltrados.Where(c =>
                        c.Descricao.ToLower().Contains(termo) ||
                        c.Categoria.ToLower().Contains(termo));
                }

                // Filtro por status
                if (cmbFiltroStatus.SelectedIndex > 0)
                {
                    int statusFiltro = cmbFiltroStatus.SelectedIndex;
                    chamadosFiltrados = chamadosFiltrados.Where(c => (int)c.Status == statusFiltro);
                }

                // Filtro por prioridade
                if (cmbFiltroPrioridade.SelectedIndex > 0)
                {
                    int prioridadeFiltro = cmbFiltroPrioridade.SelectedIndex;
                    chamadosFiltrados = chamadosFiltrados.Where(c => c.Prioridade == prioridadeFiltro);
                }

                // Filtro por técnico
                if (cmbFiltroTecnico.SelectedIndex > 1) // Maior que "Todos" e "Não Atribuído"
                {
                    // Implementar lógica de filtro por técnico
                }
                else if (cmbFiltroTecnico.SelectedIndex == 1) // Não Atribuído
                {
                    chamadosFiltrados = chamadosFiltrados.Where(c => !c.TecnicoResponsavel.HasValue);
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
                    // ← MODIFICADO: permiteEdicao = true
                    var formDetalhes = new DetalhesChamadoForm(chamado, _funcionarioLogado, _chamadosController, permiteEdicao: true);
                    if (formDetalhes.ShowDialog() == DialogResult.OK)
                    {
                        // Recarregar lista se houve alterações
                        CarregarChamados();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir detalhes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimparFiltros_Click(object sender, EventArgs e)
        {
            txtPesquisa.Clear();
            cmbFiltroStatus.SelectedIndex = 0;
            cmbFiltroPrioridade.SelectedIndex = 0;
            cmbFiltroTecnico.SelectedIndex = 0;
            PreencherDataGridView(_chamadosCarregados);
            AtualizarContador();
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            CarregarChamados();
        }

        private void btnAtribuir_Click(object sender, EventArgs e)
        {
            try
            {
                var selecionados = ObterChamadosSelecionados();

                if (selecionados.Count == 0)
                {
                    MessageBox.Show("Selecione pelo menos um chamado para atribuir.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formAtribuir = new AtribuirTecnicoForm(_funcionariosController);
                if (formAtribuir.ShowDialog() == DialogResult.OK)
                {
                    int idTecnico = formAtribuir.TecnicoSelecionado;

                    foreach (int idChamado in selecionados)
                    {
                        _chamadosController.AtribuirTecnico(idChamado, idTecnico);
                    }

                    MessageBox.Show($"{selecionados.Count} chamado(s) atribuído(s) com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CarregarChamados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atribuir técnico: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAlterarStatus_Click(object sender, EventArgs e)
        {
            try
            {
                var selecionados = ObterChamadosSelecionados();

                if (selecionados.Count == 0)
                {
                    MessageBox.Show("Selecione pelo menos um chamado para alterar o status.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formStatus = new AlterarStatusForm();
                if (formStatus.ShowDialog() == DialogResult.OK)
                {
                    int novoStatus = formStatus.StatusSelecionado;

                    foreach (int idChamado in selecionados)
                    {
                        _chamadosController.AlterarStatus(idChamado, novoStatus);
                    }

                    MessageBox.Show($"Status alterado para {selecionados.Count} chamado(s)!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CarregarChamados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar status: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAlterarPrioridade_Click(object sender, EventArgs e)
        {
            try
            {
                var selecionados = ObterChamadosSelecionados();

                if (selecionados.Count == 0)
                {
                    MessageBox.Show("Selecione pelo menos um chamado para alterar a prioridade.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formPrioridade = new AlterarPrioridadeForm();
                if (formPrioridade.ShowDialog() == DialogResult.OK)
                {
                    int novaPrioridade = formPrioridade.PrioridadeSelecionada;

                    foreach (int idChamado in selecionados)
                    {
                        _chamadosController.AlterarPrioridade(idChamado, novaPrioridade);
                    }

                    MessageBox.Show($"Prioridade alterada para {selecionados.Count} chamado(s)!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CarregarChamados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar prioridade: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            try
            {
                var selecionados = ObterChamadosSelecionados();

                if (selecionados.Count == 0)
                {
                    MessageBox.Show("Selecione pelo menos um chamado para fechar.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resultado = MessageBox.Show($"Deseja realmente fechar {selecionados.Count} chamado(s)?",
                    "Confirmar Fechamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    foreach (int idChamado in selecionados)
                    {
                        _chamadosController.FecharChamado(idChamado);
                    }

                    MessageBox.Show($"{selecionados.Count} chamado(s) fechado(s) com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CarregarChamados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao fechar chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReabrir_Click(object sender, EventArgs e)
        {
            try
            {
                var selecionados = ObterChamadosSelecionados();

                if (selecionados.Count == 0)
                {
                    MessageBox.Show("Selecione pelo menos um chamado para reabrir.", "Nenhum Chamado Selecionado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resultado = MessageBox.Show($"Deseja realmente reabrir {selecionados.Count} chamado(s)?",
                    "Confirmar Reabertura", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    foreach (int idChamado in selecionados)
                    {
                        _chamadosController.ReabrirChamado(idChamado);
                    }

                    MessageBox.Show($"{selecionados.Count} chamado(s) reaberto(s) com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CarregarChamados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao reabrir chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
