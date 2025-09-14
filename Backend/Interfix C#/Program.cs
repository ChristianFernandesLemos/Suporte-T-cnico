using System;
using System.Windows.Forms;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Senha { get; set; }
    public string Tipo { get; set; }
}

public class Chamado
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public string Status { get; set; }
    public DateTime DataAbertura { get; set; }
    public int UsuarioId { get; set; }
}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TelaLogin());
    }
}