using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class AtribuirTecnicoForm : Form
    {
        private FuncionariosController _funcionariosController;
        private ComboBox cmbTecnicos;
        private Button btnAtribuir;
        private Button btnCancelar;
        public int TecnicoSelecionado { get; private set; }

        public AtribuirTecnicoForm(FuncionariosController funcionariosController)
        {
            _funcionariosController = funcionariosController;
            InitializeComponent();
            CarregarTecnicos();
        }

        private void InitializeComponent()
        {
            this.cmbTecnicos = new ComboBox();
            this.btnAtribuir = new Button();
            this.btnCancelar = new Button();

            var lblTitulo = new Label
            {
                Text = "Selecione o Técnico",
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(200, 20)
            };

            this.cmbTecnicos.Location = new Point(12, 45);
            this.cmbTecnicos.Size = new Size(260, 21);
            this.cmbTecnicos.DropDownStyle = ComboBoxStyle.DropDownList;

            this.btnAtribuir.Text = "Atribuir";
            this.btnAtribuir.Location = new Point(117, 80);
            this.btnAtribuir.Size = new Size(75, 30);
            this.btnAtribuir.Click += BtnAtribuir_Click;

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(197, 80);
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Text = "Atribuir Técnico";
            this.Size = new Size(300, 160);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[] { lblTitulo, this.cmbTecnicos, this.btnAtribuir, this.btnCancelar });
        }

        private void CarregarTecnicos()
        {
            var tecnicos = _funcionariosController.ListarTecnicos();
            foreach (var tecnico in tecnicos)
            {
                cmbTecnicos.Items.Add(new TecnicoItem { Id = tecnico.Id, Nome = tecnico.Nome.ToString() });
            }
            if (cmbTecnicos.Items.Count > 0)
                cmbTecnicos.SelectedIndex = 0;
        }

        private void BtnAtribuir_Click(object sender, EventArgs e)
        {
            if (cmbTecnicos.SelectedItem != null)
            {
                TecnicoSelecionado = ((TecnicoItem)cmbTecnicos.SelectedItem).Id;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
