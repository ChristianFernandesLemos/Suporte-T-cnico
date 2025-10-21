CardEstatisticas(Point localizacao)
        {
            var panel = new Panel();
            panel.Size = new Size(320, 140);
            panel.Location = localizacao;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.None;
            
            // Sombra simulada
            panel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle,
                    Color.FromArgb(222, 226, 230), ButtonBorderStyle.Solid);
            };

            var lblTitulo = new Label();
            lblTitulo.Text = "Estatísticas Rápidas";
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(255, 193, 7);
            lblTitulo.Location = new Point(15, 15);
            lblTitulo.AutoSize = true;

            // Obter estatísticas
            try
            {
                var chamados = _chamadosController.ListarTodosChamados();
                int totalChamados = chamados.Count;
                int abertos = chamados.Count(c => (int)c.Status == 1);
                int emAndamento = chamados.Count(c => (int)c.Status == 2);
                int resolvidos = chamados.Count(c => (int)c.Status == 3);

                var lblStats = new Label();
                lblStats.Text = $"Total de Chamados: {totalChamados}\n" +
                              $"Abertos: {abertos}\n" +
                              $"Em Andamento: {emAndamento}\n" +
                              $"Resolvidos: {resolvidos}";
                lblStats.Font = new Font("Segoe UI", 10F);
                lblStats.Location = new Point(15, 45);
                lblStats.Size = new Size(290, 80);
                lblStats.ForeColor = Color.FromArgb(73, 80, 87);

                panel.Controls.Add(lblStats);
            }
            catch
            {
                var lblErro = new Label();
                lblErro.Text = "Não foi possível\ncarregar estatísticas";
                lblErro.Font = new Font("Segoe UI", 10F);
                lblErro.Location = new Point(15, 45);
                lblErro.Size = new Size(290, 80);
                lblErro.ForeColor = Color.Gray;
                panel.Controls.Add(lblErro);
            }

            panel.Controls.Add(lblTitulo);

            return panel;
        }
    }
}
