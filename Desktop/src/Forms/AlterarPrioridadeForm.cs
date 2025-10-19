using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaChamados.Forms
{
    public partial class AlterarPrioridadeForm : Form
    {
        private ComboBox cmbPrioridade;
        private Button btnSalvar;
        private Button btnCancelar;
        public int PrioridadeSelecionada { get; private set; }

        public AlterarPrioridadeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.cmbPrioridade = new ComboBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();

            var lblTitulo = new Label
            {
                Text = "Selecione a Nova Prioridade",
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(200, 20)
            };

            this.cmbPrioridade.Location = new Point(12, 45);
            this.cmbPrioridade.Size = new Size(200, 21);
            this.cmbPrioridade.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPrioridade.Items.AddRange(new object[] {
                new ComboBoxItem("Baixa", 1),
                new ComboBoxItem("Média", 2),
                new ComboBoxItem("Alta", 3),
                new ComboBoxItem("Crítica", 4)
            });
            this.cmbPrioridade.SelectedIndex = 1; // Média por padrão

            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new Point(57, 80);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.Click += BtnSalvar_Click;

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(137, 80);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += BtnCancelar_Click;

            this.Text = "Alterar Prioridade";
            this.Size = new Size(240, 160);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[] { lblTitulo, this.cmbPrioridade, this.btnSalvar, this.btnCancelar });
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (cmbPrioridade.SelectedItem != null)
            {
                PrioridadeSelecionada = ((ComboBoxItem)cmbPrioridade.SelectedItem).Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
