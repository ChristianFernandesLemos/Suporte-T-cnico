using System;

namespace SistemaChamados.Models
{
    // Enumeração para status do chamado
    public enum StatusChamado
    {
        Aberto = 1,
        EmAndamento = 2,
        Resolvido = 3,
        Fechado = 4,
        Cancelado = 5
    }

    // Enumeração para prioridade do chamado
    public enum PrioridadeChamado
    {
        Baixa = 1,
        Media = 2,
        Alta = 3,
        Critica = 4
    }

    // Classe Chamados
    public class Chamados
    {
        // Atributos
        public int IdChamado { get; set; }
        public string Categoria { get; set; }
        public string Contestacoes { get; set; }
        public int Prioridade { get; set; }
        public string Descricao { get; set; }
        public int Afetado { get; set; } // ID do funcionário afetado
        public DateTime DataChamado { get; set; }
        public StatusChamado Status { get; set; }
        public int? TecnicoResponsavel { get; set; } // ID do técnico responsável
        public DateTime? DataResolucao { get; set; }
        public string Solucao { get; set; }

        // Construtor
        public Chamados()
        {
            DataChamado = DateTime.Now;
            Status = StatusChamado.Aberto;
            Prioridade = (int)PrioridadeChamado.Media; // Prioridade padrão
        }

        // Construtor com parâmetros
        public Chamados(int idChamado, string categoria, string descricao, int afetado, int prioridade = (int)PrioridadeChamado.Media)
        {
            IdChamado = idChamado;
            Categoria = categoria;
            Descricao = descricao;
            Afetado = afetado;
            Prioridade = prioridade;
            DataChamado = DateTime.Now;
            Status = StatusChamado.Aberto;
        }

        // Método para criar chamado
        public int CriarChamado()
        {
            try
            {
                // Lógica para criar um novo chamado
                // Aqui seria feita a inserção no banco de dados
                this.DataChamado = DateTime.Now;
                this.Status = StatusChamado.Aberto;
                this.IdChamado = new Random().Next(1000, 9999);

                Console.WriteLine($"Chamado {IdChamado} criado com sucesso em {DataChamado}");
                return this.IdChamado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar chamado: {ex.Message}");
            }
        }

        // Método para alterar prioridade
        public void AlterarPrioridade(int novaPrioridade)
        {
            try
            {
                // Validar se a prioridade está dentro dos valores válidos
                if (novaPrioridade < 1 || novaPrioridade > 4)
                {
                    throw new ArgumentException("Prioridade deve estar entre 1 (Baixa) e 4 (Crítica)");
                }

                int prioridadeAnterior = this.Prioridade;
                this.Prioridade = novaPrioridade;
                
                Console.WriteLine($"Prioridade do chamado {IdChamado} alterada de {prioridadeAnterior} para {novaPrioridade}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar prioridade: {ex.Message}");
            }
        }

        // Método para alterar status
        public void AlterarStatus(StatusChamado novoStatus)
        {
            try
            {
                StatusChamado statusAnterior = this.Status;
                this.Status = novoStatus;

                // Se o status for alterado para Resolvido, definir data de resolução
                if (novoStatus == StatusChamado.Resolvido)
                {
                    this.DataResolucao = DateTime.Now;
                }

                Console.WriteLine($"Status do chamado {IdChamado} alterado de {statusAnterior} para {novoStatus}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar status: {ex.Message}");
            }
        }

        // Método para alterar status (sobrecarga com int)
        public void AlterarStatus(int novoStatus)
        {
            try
            {
                if (!Enum.IsDefined(typeof(StatusChamado), novoStatus))
                {
                    throw new ArgumentException("Status inválido");
                }

                AlterarStatus((StatusChamado)novoStatus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar status: {ex.Message}");
            }
        }

        // Método para atribuir técnico responsável
        public void AtribuirTecnico(int idTecnico)
        {
            try
            {
                this.TecnicoResponsavel = idTecnico;
                this.Status = StatusChamado.EmAndamento;
                
                Console.WriteLine($"Técnico {idTecnico} atribuído ao chamado {IdChamado}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atribuir técnico: {ex.Message}");
            }
        }

        // Método para adicionar contestação
        public void AdicionarContestacao(string contestacao)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Contestacoes))
                {
                    this.Contestacoes = contestacao;
                }
                else
                {
                    this.Contestacoes += $"\n--- {DateTime.Now} ---\n{contestacao}";
                }
                
                Console.WriteLine($"Contestação adicionada ao chamado {IdChamado}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar contestação: {ex.Message}");
            }
        }

        // Método para obter descrição da prioridade
        public string ObterDescricaoPrioridade()
        {
            return ((PrioridadeChamado)this.Prioridade).ToString();
        }

        // Método para obter descrição do status
        public string ObterDescricaoStatus()
        {
            return this.Status.ToString();
        }
    }
}
