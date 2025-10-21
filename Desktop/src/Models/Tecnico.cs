using System;

namespace SistemaChamados.Models
{
    // Classe filha Técnico
    public class Tecnico : Funcionarios
    {
        public Tecnico() : base() { }

        // Construtor
        public Tecnico(int id, string nome, string cpf, string email, string senha, int nivelAcesso)
            : base(id, nome, cpf, email, senha, nivelAcesso)
        {
        }
        public override string TipoFuncionario => "Técnico";
        public override int NivelAcesso => 2;
        public string Especializacao { get; set; }

        // Método para marcar chamado como resolvido
        public void MarcarChamadoResolvido(int idChamado)
        {
            try
            {
                // Lógica para marcar chamado como resolvido
                // Aqui seria feita a atualização no banco de dados
                Console.WriteLine($"Chamado {idChamado} marcado como resolvido pelo técnico {Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao marcar chamado como resolvido: {ex.Message}");
            }
        }

        // Método para trocar prioridade do chamado
        public void TrocarPrioridadeDoChamado(int idChamado, int novaPrioridade)
        {
            try
            {
                // Lógica para alterar prioridade do chamado
                // Aqui seria feita a atualização no banco de dados
                Console.WriteLine($"Prioridade do chamado {idChamado} alterada para {novaPrioridade} pelo técnico {Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar prioridade do chamado: {ex.Message}");
            }
        }

        // Override do método VisualizarChamados para técnicos
        public override void VisualizarChamados()
        {
            // Implementação específica para técnicos
            // Técnicos podem ver chamados atribuídos a eles
            Console.WriteLine($"Visualizando chamados do técnico {Id}");
        }
    }
}
