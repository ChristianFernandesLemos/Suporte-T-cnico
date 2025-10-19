using System;

namespace SistemaChamados.Models
{
    public abstract class Funcionarios
    {
        public int Id { get; set; }
        public string Nome { get; set; } 
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        
        // Propriedades virtuais para permitir override nas classes filhas
        public virtual int NivelAcesso { get; set; }
        public virtual string TipoFuncionario { get; set; }
        
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        protected Funcionarios()
        {
            DataCadastro = DateTime.Now;
            Ativo = true;
        }

        protected Funcionarios(int id, string nome, string cpf, string email, string senha, int nivelAcesso)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Email = email;
            Senha = senha;
            NivelAcesso = nivelAcesso;
            DataCadastro = DateTime.Now;
            Ativo = true;
        }

        // Métodos virtuais para serem sobrescritos
        public virtual void VisualizarChamados()
        {
            Console.WriteLine($"{Nome} está visualizando chamados...");
        }

        public virtual void AlterarSenhaPropria(string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("Nova senha não pode ser vazia");

            Senha = novaSenha;
            Console.WriteLine("Senha alterada com sucesso!");
        }
    }
}
