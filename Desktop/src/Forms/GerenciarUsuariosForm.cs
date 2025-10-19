using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;
using SistemaChamados.Helpers;

namespace SistemaChamados.Forms
{
    public partial class GerenciarUsuariosForm : Form
    {
        private FuncionariosController _funcionariosController;
        private Funcionarios _funcionarioLogado;
        private DataGridView dgvUsuarios;
        private ComboBox cmbFiltroNivel;
        private ComboBox cmbFiltroStatus;
        private TextBox txtPesquisa;
        private Button btnPesquisar;
        private Button btnLimparFiltros;
        private Button btnAtualizar;
        private Button btnNovoUsuario;
        private Button btnEditarUsuario;
        private Button btnAlterarSenha;
        private Button btnAlterarNivel;
        private Button btnAtivarDesativar;
        private Button btnExcluir;
        private GroupBox gbFiltros;
        private GroupBox gbAcoes;
        private Label lblTotalUsuarios;
        private List<Funcionarios> _usuariosCarregados;

        public GerenciarUsuariosForm(Funcionarios funcionario, FuncionariosController funcionariosController)
        {
            _funcionarioLogado = funcionario;
            _funcionariosController = funcionariosController;
            _usuariosCarregados = new List<Funcionarios>();
            InitializeComponent();
            ConfigurarFormulario();
            CarregarUsuarios();
        }

        private void InitializeComponent()
        {
            this.dgvUsuarios = new DataGridView();
            this.cmbFiltroNivel = new ComboBox();
            this.cmbFiltroStatus = new ComboBox();
            this.txtPesquisa = new TextBox();
            this.btnPesquisar = new Button();
            this.btnLimparFiltros = new Button();
            this.btnAtualizar = new Button();
            this.btnNovoUsuario = new Button();
            this.btnEditarUsuario = new Button();
            this.btnAlterarSenha = new Button();
            this.btnAlterarNivel = new Button();
            this.btnAtivarDesativar = new Button();
            this.btnExcluir = new Button();
            this.gbFiltros = new GroupBox();
            this.gbAcoes = new GroupBox();
            this.lblTotalUsuarios = new Label();

            this.SuspendLayout();

            // 
            // gbFiltros
            // 
            this.gbFiltros.Text = "Filtros de Busca";
            this.gbFiltros.Location = new Point(12, 12);
            this.gbFiltros.Size = new Size(900, 80);
            this.gbFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // txtPesquisa
            // 
            this.txtPesquisa.Location = new Point(15, 25);
            this.txtPesquisa.Size = new Size(250, 20);
            this.txtPesquisa.Text = "Pesquisar por nome ou email...";
            this.txtPesquisa.ForeColor = Color.Gray;
            // Adicionar eventos para simular placeholder
            this.txtPesquisa.GotFocus += TxtPesquisa_GotFocus;
            this.txtPesquisa.LostFocus += TxtPesquisa_LostFocus;
           

            // 
            // cmbFiltroNivel
            // 
            this.cmbFiltroNivel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroNivel.Location = new Point(275, 25);
            this.cmbFiltroNivel.Size = new Size(150, 21);

            // 
            // cmbFiltroStatus
            // 
            this.cmbFiltroStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFiltroStatus.Location = new Point(435, 25);
            this.cmbFiltroStatus.Size = new Size(120, 21);

            // 
            // btnPesquisar
            // 
            this.btnPesquisar.Text = "Pesquisar";
            this.btnPesquisar.Location = new Point(565, 23);
            this.btnPesquisar.Size = new Size(80, 25);
            this.btnPesquisar.Click += new EventHandler(this.btnPesquisar_Click);

            // 
            // btnLimparFiltros
            // 
            this.btnLimparFiltros.Text = "Limpar";
            this.btnLimparFiltros.Location = new Point(655, 23);
            this.btnLimparFiltros.Size = new Size(70, 25);
            this.btnLimparFiltros.Click += new EventHandler(this.btnLimparFiltros_Click);

            // 
            // btnAtualizar
            // 
            this.btnAtualizar.Text = "Atualizar";
            this.btnAtualizar.Location = new Point(735, 23);
            this.btnAtualizar.Size = new Size(70, 25);
            this.btnAtualizar.Click += new EventHandler(this.btnAtualizar_Click);

            this.gbFiltros.Controls.AddRange(new Control[]
            {
                this.txtPesquisa, this.cmbFiltroNivel, this.cmbFiltroStatus,
                this.btnPesquisar, this.btnLimparFiltros, this.btnAtualizar
            });

            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.Location = new Point(12, 110);
            this.dgvUsuarios.Size = new Size(900, 350);
            this.dgvUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.RowHeadersVisible = false;

            // 
            // gbAcoes
            // 
            this.gbAcoes.Text = "Ações";
            this.gbAcoes.Location = new Point(12, 470);
            this.gbAcoes.Size = new Size(900, 80);
            this.gbAcoes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // btnNovoUsuario
            // 
            this.btnNovoUsuario.Text = "Novo Usuário";
            this.btnNovoUsuario.Location = new Point(15, 25);
            this.btnNovoUsuario.Size = new Size(100, 30);
            this.btnNovoUsuario.BackColor = Color.FromArgb(40, 167, 69);
            this.btnNovoUsuario.ForeColor = Color.White;
            this.btnNovoUsuario.FlatStyle = FlatStyle.Flat;
            this.btnNovoUsuario.Click += new EventHandler(this.btnNovoUsuario_Click);

            // 
            // btnEditarUsuario
            // 
            this.btnEditarUsuario.Text = "Editar";
            this.btnEditarUsuario.Location = new Point(125, 25);
            this.btnEditarUsuario.Size = new Size(80, 30);
            this.btnEditarUsuario.BackColor = Color.FromArgb(0, 123, 255);
            this.btnEditarUsuario.ForeColor = Color.White;
            this.btnEditarUsuario.FlatStyle = FlatStyle.Flat;
            this.btnEditarUsuario.Click += new EventHandler(this.btnEditarUsuario_Click);

            // 
            // btnAlterarSenha
            // 
            this.btnAlterarSenha.Text = "Alterar Senha";
            this.btnAlterarSenha.Location = new Point(215, 25);
            this.btnAlterarSenha.Size = new Size(100, 30);
            this.btnAlterarSenha.BackColor = Color.FromArgb(255, 193, 7);
            this.btnAlterarSenha.ForeColor = Color.Black;
            this.btnAlterarSenha.FlatStyle = FlatStyle.Flat;
            this.btnAlterarSenha.Click += new EventHandler(this.btnAlterarSenha_Click);

            // 
            // btnAlterarNivel
            // 
            this.btnAlterarNivel.Text = "Alterar Nível";
            this.btnAlterarNivel.Location = new Point(325, 25);
            this.btnAlterarNivel.Size = new Size(100, 30);
            this.btnAlterarNivel.BackColor = Color.FromArgb(23, 162, 184);
            this.btnAlterarNivel.ForeColor = Color.White;
            this.btnAlterarNivel.FlatStyle = FlatStyle.Flat;
            this.btnAlterarNivel.Click += new EventHandler(this.btnAlterarNivel_Click);

            // 
            // btnAtivarDesativar
            // 
            this.btnAtivarDesativar.Text = "Ativar/Desativar";
            this.btnAtivarDesativar.Location = new Point(435, 25);
            this.btnAtivarDesativar.Size = new Size(110, 30);
            this.btnAtivarDesativar.BackColor = Color.FromArgb(108, 117, 125);
            this.btnAtivarDesativar.ForeColor = Color.White;
            this.btnAtivarDesativar.FlatStyle = FlatStyle.Flat;
            this.btnAtivarDesativar.Click += new EventHandler(this.btnAtivarDesativar_Click);

            // 
            // btnExcluir
            // 
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.Location = new Point(555, 25);
            this.btnExcluir.Size = new Size(80, 30);
            this.btnExcluir.BackColor = Color.FromArgb(220, 53, 69);
            this.btnExcluir.ForeColor = Color.White;
            this.btnExcluir.FlatStyle = FlatStyle.Flat;
            this.btnExcluir.Click += new EventHandler(this.btnExcluir_Click);

            this.gbAcoes.Controls.AddRange(new Control[]
            {
                this.btnNovoUsuario, this.btnEditarUsuario, this.btnAlterarSenha,
                this.btnAlterarNivel, this.btnAtivarDesativar, this.btnExcluir
            });

            // 
            // lblTotalUsuarios
            // 
            this.lblTotalUsuarios.Location = new Point(12, 560);
            this.lblTotalUsuarios.Size = new Size(200, 15);
            this.lblTotalUsuarios.Text = "Total: 0 usuários";

            // 
            // GerenciarUsuariosForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(924, 588);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.dgvUsuarios);
            this.Controls.Add(this.gbAcoes);
            this.Controls.Add(this.lblTotalUsuarios);
            this.Name = "GerenciarUsuariosForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Gerenciar Usuários - Sistema de Chamados";
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
            // Filtro por nível de acesso
            cmbFiltroNivel.Items.Add("Todos os Níveis");
            cmbFiltroNivel.Items.Add("Funcionário (Nível 1)");
            cmbFiltroNivel.Items.Add("Técnico (Nível 2)");
            cmbFiltroNivel.Items.Add("Administrador (Nível 3)");
            cmbFiltroNivel.SelectedIndex = 0;

            // Filtro por status
            cmbFiltroStatus.Items.Add("Todos");
            cmbFiltroStatus.Items.Add("Ativo");
            cmbFiltroStatus.Items.Add("Inativo");
            cmbFiltroStatus.SelectedIndex = 1; // Mostrar apenas ativos por padrão
        }

        private void ConfigurarDataGridView()
        {
            dgvUsuarios.Columns.Clear();

            // Configurar colunas
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                Width = 50,
                ReadOnly = true
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                HeaderText = "Nome",
                Width = 200,
                ReadOnly = true
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "E-mail",
                Width = 250,
                ReadOnly = true
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cpf",
                HeaderText = "CPF",
                Width = 120,
                ReadOnly = true
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NivelAcesso",
                HeaderText = "Nível de Acesso",
                Width = 130,
                ReadOnly = true
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataCriacao",
                HeaderText = "Data de Cadastro",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 80,
                ReadOnly = true
            });

            // Configurar aparência
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvUsuarios.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            dgvUsuarios.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void ConfigurarPermissoes()
        {
            // Apenas administradores podem gerenciar usuários
            if (_funcionarioLogado.NivelAcesso < 3)
            {
                MessageBox.Show("Acesso negado! Apenas administradores podem gerenciar usuários.",
                    "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }
        }

        private void CarregarUsuarios()
        {
            try
            {
                dgvUsuarios.Rows.Clear();
                _usuariosCarregados = _funcionariosController.ListarTodosFuncionarios();
                PreencherDataGridView(_usuariosCarregados);
                AtualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreencherDataGridView(List<Funcionarios> usuarios)
        {
            dgvUsuarios.Rows.Clear();

            foreach (var usuario in usuarios)
            {
                var row = new DataGridViewRow();

                row.Cells.Add(new DataGridViewTextBoxCell { Value = usuario.Id });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = usuario.Nome });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = usuario.Email });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = FormatarCpf(usuario.Cpf) });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = ObterTextoNivelAcesso(usuario.NivelAcesso) });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = DateTime.Now }); // Placeholder para DataCriacao
                row.Cells.Add(new DataGridViewTextBoxCell { Value = "Ativo" }); // Placeholder para Status

                // Colorir linha baseado no nível de acesso
                ColorirLinhaPorNivel(row, usuario.NivelAcesso);

                // Desabilitar linha se for o próprio usuário logado (evitar auto-exclusão)
                if (usuario.Id == _funcionarioLogado.Id)
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    row.DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
                }

                dgvUsuarios.Rows.Add(row);
            }
        }

        private void ColorirLinhaPorNivel(DataGridViewRow row, int nivelAcesso)
        {
            switch (nivelAcesso)
            {
                case 1: // Funcionário - Verde claro
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    break;
                case 2: // Técnico - Azul claro
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    break;
                case 3: // Administrador - Rosa claro
                    row.DefaultCellStyle.BackColor = Color.LightPink;
                    break;
            }
        }

        private string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
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

        private void AtualizarContador()
        {
            lblTotalUsuarios.Text = $"Total: {dgvUsuarios.Rows.Count} usuários";
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            FiltrarUsuarios();
        }

        private void FiltrarUsuarios()
        {
            try
            {
                var usuariosFiltrados = _usuariosCarregados.AsEnumerable();

                // Filtro por texto (ignorar se for o placeholder)
                if (!string.IsNullOrWhiteSpace(txtPesquisa.Text) && txtPesquisa.Text != "Pesquisar por nome ou email...")
                {
                    string termo = txtPesquisa.Text.ToLower();
                    usuariosFiltrados = usuariosFiltrados.Where(u =>
                        u.Nome.ToString().ToLower().Contains(termo) ||
                        u.Email.ToLower().Contains(termo) ||
                        u.Cpf.Contains(termo.Replace(".", "").Replace("-", "")));
                }

                // Filtro por nível de acesso
                if (cmbFiltroNivel.SelectedIndex > 0)
                {
                    int nivelFiltro = cmbFiltroNivel.SelectedIndex;
                    usuariosFiltrados = usuariosFiltrados.Where(u => u.NivelAcesso == nivelFiltro);
                }

                // Filtro por status (placeholder - implementar quando tiver campo status)
                if (cmbFiltroStatus.SelectedIndex == 1) // Ativo
                {
                    // usuariosFiltrados = usuariosFiltrados.Where(u => u.Ativo);
                }
                else if (cmbFiltroStatus.SelectedIndex == 2) // Inativo
                {
                    // usuariosFiltrados = usuariosFiltrados.Where(u => !u.Ativo);
                }

                PreencherDataGridView(usuariosFiltrados.ToList());
                AtualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar usuários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimparFiltros_Click(object sender, EventArgs e)
        {
            txtPesquisa.Text = "Pesquisar por nome ou email...";
            txtPesquisa.ForeColor = Color.Gray;
            cmbFiltroNivel.SelectedIndex = 0;
            cmbFiltroStatus.SelectedIndex = 1;
            PreencherDataGridView(_usuariosCarregados);
            AtualizarContador();
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            CarregarUsuarios();
        }

        // Confirmação de seleção
        private bool ConfirmarSenhaAdmin()
        {
            var formConfirmar = new ConfirmarSenhaAdminForm(_funcionarioLogado);
            return formConfirmar.ShowDialog() == DialogResult.OK;
        }

        private void btnNovoUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                // Solicitar confirmaçao de senha
                if (!ConfirmarSenhaAdmin())
                {
                    MessageBox.Show("Operação cancelada.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formNovoUsuario = new NovoUsuarioForm(_funcionariosController);
                if (formNovoUsuario.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Usuário criado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar usuário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                var usuarioSelecionado = ObterUsuarioSelecionado();
                if (usuarioSelecionado == null) return;

                // Solicitar confirmaçao de senha
                if (!ConfirmarSenhaAdmin())
                {
                    MessageBox.Show("Operação cancelada.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formEditarUsuario = new EditarUsuarioForm(usuarioSelecionado, _funcionariosController);
                if (formEditarUsuario.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Usuário atualizado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar usuário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAlterarSenha_Click(object sender, EventArgs e)
        {
            try
            {
                var usuarioSelecionado = ObterUsuarioSelecionado();
                if (usuarioSelecionado == null) return;

                // Solicitar confirmaçao de senha
                if (!ConfirmarSenhaAdmin())
                {
                    MessageBox.Show("Operação cancelada.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formAlterarSenha = new AlterarSenhaForm(usuarioSelecionado, _funcionariosController);
                if (formAlterarSenha.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Senha alterada com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar senha: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAlterarNivel_Click(object sender, EventArgs e)
        {
            try
            {
                var usuarioSelecionado = ObterUsuarioSelecionado();
                if (usuarioSelecionado == null) return;

                if (usuarioSelecionado.Id == _funcionarioLogado.Id)
                {
                    MessageBox.Show("Você não pode alterar seu próprio nível de acesso!", "Operação Inválida",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Solicitar confirmaçao de senha
                if (!ConfirmarSenhaAdmin())
                {
                    MessageBox.Show("Operação cancelada.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var formAlterarNivel = new AlterarNivelAcessoForm(usuarioSelecionado);
                if (formAlterarNivel.ShowDialog() == DialogResult.OK)
                {
                    int novoNivel = formAlterarNivel.NovoNivel;
                    _funcionariosController.AlterarNivelAcesso(usuarioSelecionado.Id, novoNivel);

                    MessageBox.Show($"Nível de acesso alterado para {ObterTextoNivelAcesso(novoNivel)}!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar nível de acesso: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAtivarDesativar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuarioSelecionado = ObterUsuarioSelecionado();
                if (usuarioSelecionado == null) return;

                if (usuarioSelecionado.Id == _funcionarioLogado.Id)
                {
                    MessageBox.Show("Você não pode desativar sua própria conta!", "Operação Inválida",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Solicitar confirmação de senha
                if (!ConfirmarSenhaAdmin())
                {
                    MessageBox.Show("Operação cancelada.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Placeholder para funcionalidade de ativar/desativar
                MessageBox.Show("Funcionalidade de ativar/desativar usuário será implementada.", "Em Desenvolvimento",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ativar/desativar usuário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                var usuarioSelecionado = ObterUsuarioSelecionado();
                if (usuarioSelecionado == null) return;

                if (usuarioSelecionado.Id == _funcionarioLogado.Id)
                {
                    MessageBox.Show("Você não pode excluir sua própria conta!", "Operação Inválida",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var resultado = MessageBox.Show(
                    $"Tem certeza que deseja excluir o usuário '{usuarioSelecionado.Nome}'?\n\n" +
                    "ATENÇÃO: Esta ação é irreversível e todos os chamados relacionados serão afetados!",
                    "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (resultado == DialogResult.Yes)
                {
                    // Solicitar confirmação de senha
                    if (!ConfirmarSenhaAdmin())
                    {
                        MessageBox.Show("Operação cancelada.", "Cancelado",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    _funcionariosController.ExcluirFuncionario(usuarioSelecionado.Id);
                    MessageBox.Show("Usuário excluído com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir usuário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Funcionarios ObterUsuarioSelecionado()
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um usuário primeiro.", "Nenhum Usuário Selecionado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            int idUsuario = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
            return _usuariosCarregados.FirstOrDefault(u => u.Id == idUsuario);
        }
    }

    /// <summary>
    /// Formulário para alterar nível de acesso
    /// </summary>
    public partial class AlterarNivelAcessoForm : Form
    {
        private Funcionarios _usuario;
        private ComboBox cmbNovoNivel;
        private Label lblUsuario;
        private Label lblNivelAtual;
        private Label lblNovoNivel;
        private Button btnSalvar;
        private Button btnCancelar;

        public int NovoNivel { get; private set; }

        public AlterarNivelAcessoForm(Funcionarios usuario)
        {
            _usuario = usuario;
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void InitializeComponent()
        {
            this.lblUsuario = new Label();
            this.lblNivelAtual = new Label();
            this.lblNovoNivel = new Label();
            this.cmbNovoNivel = new ComboBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();

            // lblUsuario
            this.lblUsuario.Location = new Point(20, 20);
            this.lblUsuario.Size = new Size(350, 20);
            this.lblUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);

            // lblNivelAtual
            this.lblNivelAtual.Location = new Point(20, 50);
            this.lblNivelAtual.Size = new Size(350, 20);

            // lblNovoNivel
            this.lblNovoNivel.Text = "Novo Nível de Acesso:";
            this.lblNovoNivel.Location = new Point(20, 80);
            this.lblNovoNivel.Size = new Size(150, 20);

            // cmbNovoNivel
            this.cmbNovoNivel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbNovoNivel.Location = new Point(20, 100);
            this.cmbNovoNivel.Size = new Size(200, 21);

            // btnSalvar
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new Point(145, 140);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

            // btnCancelar
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(230, 140);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            // Form
            this.Text = "Alterar Nível de Acesso";
            this.Size = new Size(400, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[]
            {
                this.lblUsuario, this.lblNivelAtual, this.lblNovoNivel,
                this.cmbNovoNivel, this.btnSalvar, this.btnCancelar
            });
        }

        private void ConfigurarFormulario()
        {
            lblUsuario.Text = $"Usuário: {_usuario.Nome} ({_usuario.Email})";
            lblNivelAtual.Text = $"Nível Atual: {ObterTextoNivelAcesso(_usuario.NivelAcesso)}";

            cmbNovoNivel.Items.Add(new ComboBoxItem { Text = "Funcionário", Value = 1 });
            cmbNovoNivel.Items.Add(new ComboBoxItem { Text = "Técnico", Value = 2 });
            cmbNovoNivel.Items.Add(new ComboBoxItem { Text = "Administrador", Value = 3 });

            // Selecionar nível atual
            cmbNovoNivel.SelectedIndex = _usuario.NivelAcesso - 1;
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

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            var item = (ComboBoxItem)cmbNovoNivel.SelectedItem;
            NovoNivel = item.Value;

            if (NovoNivel == _usuario.NivelAcesso)
            {
                MessageBox.Show("Selecione um nível diferente do atual.", "Mesmo Nível",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}