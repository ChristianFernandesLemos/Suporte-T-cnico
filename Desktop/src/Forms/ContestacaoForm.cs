using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class ContestacaoForm : Form
    {
        private Chamados _chamado;
        private ChamadosController _controller;
        private TextBox txtContestacao;
        private Button btnEnviar;
        private Button btnCancelar;
        private Label lblChamadoInfo;

        public ContestacaoForm(Chamados chamado, ChamadosController controller)
        {
            _chamado = chamado;
            _controller = controller;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtContestacao = new TextBox();
            this.btnEnviar = new Button();
            this.btnCancelar = new Button();
            this.lblChamadoInfo = new Label();

            var lblTitulo = new Label
            {
                Text = "Adicionar Contestação",
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(300, 20)
            };

            this.lblChamadoInfo.Text = $"Chamado #{_chamado.IdChamado}: {_chamado.Categoria}";
            this.lblChamadoInfo.Location = new Point(12, 40);
            this.lblChamadoInfo.Size = new Size(360, 20);

            var lblTexto = new Label
            {
                Text = "Digite sua contestação:",
                Location = new Point(12, 70),
                Size = new Size(150, 15)
            };

            this.txtContestacao.Location = new Point(12, 90);
            this.txtContestacao.Size = new Size(360, 120);
            this.txtContestacao.Multiline = true;
            this.txtContestacao.ScrollBars = ScrollBars.Vertical;

            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.Location = new Point(217, 220);
            this.btnEnviar.Size = new Size(75, 30);
            this.btnEnviar.BackColor = Color.FromArgb(40, 167, 69);
            this.btnEnviar.ForeColor = Color.White;
            this.btnEnviar.FlatStyle = FlatStyle.Flat;
            this.btnEnviar.Click += BtnEnviar_Click;

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(297, 220);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += BtnCancelar_Click;

            this.Text = "Contestação de Chamado";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[] {
                lblTitulo, this.lblChamadoInfo, lblTexto,
                this.txtContestacao, this.btnEnviar, this.btnCancelar
            });
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContestacao.Text))
            {
                MessageBox.Show("Digite uma contestação.", "Campo Obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string novaContestacao = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {txtContestacao.Text}\n---\n{_chamado.Contestacoes}";
                _chamado.Contestacoes = novaContestacao;
                _controller.AtualizarChamado(_chamado);

                MessageBox.Show("Contestação adicionada com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar contestação: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
