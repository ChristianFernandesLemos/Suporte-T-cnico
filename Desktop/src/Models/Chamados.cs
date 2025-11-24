using System;

namespace SistemaChamados.Models
{
    public enum StatusChamado
    {
        Aberto = 1,
        EmAndamento = 2,
        Resolvido = 3,
        Fechado = 4,
        Cancelado = 5
    }

    public class Chamados
    {
        // Propriedades principais
        public int IdChamado { get; set; }

        // ⭐ NOVO: Título separado
        public string Titulo { get; set; }

        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public int Prioridade { get; set; }
        public int Afetado { get; set; }
        public DateTime DataChamado { get; set; }
        public StatusChamado Status { get; set; }
        public string Solucao { get; set; }
        public string Contestacoes { get; set; }
        public int? TecnicoResponsavel { get; set; }
        public DateTime? DataResolucao { get; set; }

        // Construtor padrão
        public Chamados()
        {
            DataChamado = DateTime.Now;
            Status = StatusChamado.Aberto;
            Prioridade = 2; // Média por padrão
        }

        // Construtor com parâmetros (ATUALIZADO)
        public Chamados(string titulo, string descricao, string categoria, int prioridade, int afetado)
        {
            Titulo = titulo;
            Descricao = descricao;
            Categoria = categoria;
            Prioridade = prioridade;
            Afetado = afetado;
            DataChamado = DateTime.Now;
            Status = StatusChamado.Aberto;
        }

        // Métodos de negócio
        public void AlterarStatus(StatusChamado novoStatus)
        {
            Status = novoStatus;

            if (novoStatus == StatusChamado.Resolvido || novoStatus == StatusChamado.Fechado)
            {
                DataResolucao = DateTime.Now;
            }
        }

        public void AtribuirTecnico(int idTecnico)
        {
            TecnicoResponsavel = idTecnico;

            if (Status == StatusChamado.Aberto)
            {
                Status = StatusChamado.EmAndamento;
            }
        }

        public void AlterarPrioridade(int novaPrioridade)
        {
            if (novaPrioridade < 1 || novaPrioridade > 4)
                throw new ArgumentException("Prioridade deve estar entre 1 (Baixa) e 4 (Crítica)");

            Prioridade = novaPrioridade;
        }

        public void DefinirSolucao(string solucao)
        {
            Solucao = solucao;
            AlterarStatus(StatusChamado.Resolvido);
        }

        // Validações
        public bool ValidarChamado()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                throw new ArgumentException("Título é obrigatório");

            if (string.IsNullOrWhiteSpace(Descricao))
                throw new ArgumentException("Descrição é obrigatória");

            if (string.IsNullOrWhiteSpace(Categoria))
                throw new ArgumentException("Categoria é obrigatória");

            if (Prioridade < 1 || Prioridade > 4)
                throw new ArgumentException("Prioridade inválida");

            if (Afetado <= 0)
                throw new ArgumentException("Funcionário afetado é obrigatório");

            return true;
        }

        // Propriedades calculadas
        public string StatusTexto
        {
            get
            {
                return Status switch
                {
                    StatusChamado.Aberto => "Aberto",
                    StatusChamado.EmAndamento => "Em Andamento",
                    StatusChamado.Resolvido => "Resolvido",
                    StatusChamado.Fechado => "Fechado",
                    StatusChamado.Cancelado => "Cancelado",
                    _ => "Desconhecido"
                };
            }
        }

        public string PrioridadeTexto
        {
            get
            {
                return Prioridade switch
                {
                    1 => "Baixa",
                    2 => "Média",
                    3 => "Alta",
                    4 => "Crítica",
                    _ => "Desconhecida"
                };
            }
        }

        public TimeSpan TempoAberto
        {
            get
            {
                if (DataResolucao.HasValue)
                    return DataResolucao.Value - DataChamado;
                else
                    return DateTime.Now - DataChamado;
            }
        }

        public bool EstaAtrasado
        {
            get
            {
                if (Status == StatusChamado.Resolvido || Status == StatusChamado.Fechado)
                    return false;

                // Definir SLA baseado na prioridade
                var slaHoras = Prioridade switch
                {
                    4 => 4,   // Crítica: 4 horas
                    3 => 24,  // Alta: 24 horas
                    2 => 72,  // Média: 72 horas
                    1 => 168, // Baixa: 7 dias
                    _ => 72
                };

                return TempoAberto.TotalHours > slaHoras;
            }
        }

        // Override ToString para debug
        public override string ToString()
        {
            return $"#{IdChamado} - {Titulo} [{StatusTexto}] - {PrioridadeTexto}";
        }
    }
}