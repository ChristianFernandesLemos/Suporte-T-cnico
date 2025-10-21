using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaChamados.Forms
{
    public partial class AlterarStatusForm : Form
    {
        private ComboBox cmbStatus;
        private Button btnSalvar;
        private Button btnCancelar;
        public int StatusSelecionado { get; private set; }

        public AlterarStatusForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.cmbStatus = new ComboBox();
            this.btnSalvar = new Button();
            this.btnCancelar = new Button();

            var lblTitulo = new Label
            {
                Text = "Selecione o Novo Status",
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(200, 20)
            };

            this.cmbStatus.Location = new Point(12, 45);
            this.cmbStatus.Size = new Size(200, 21);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] {
                new ComboBoxItem("Aberto", 1),
                new ComboBoxItem("Em Andamento", 2),
                new ComboBoxItem("Resolvido", 3),
                new ComboBoxItem("Fechado", 4),
                new ComboBoxItem("Cancelado", 5)
            });
            this.cmbStatus.SelectedIndex = 0;

            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new Point(57, 80);
            this.btnSalvar.Size = new Size(75, 30);
            this.btnSalvar.Click += BtnSalvar_Click;

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(137, 80);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += BtnCancelar_Click;

            this.Text = "Alterar Status";
            this.Size = new Size(240, 160);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[] { lblTitulo, this.cmbStatus, this.btnSalvar, this.btnCancelar });
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (cmbStatus.SelectedItem != null)
            {
                StatusSelecionado = ((ComboBoxItem)cmbStatus.SelectedItem).Value;
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
