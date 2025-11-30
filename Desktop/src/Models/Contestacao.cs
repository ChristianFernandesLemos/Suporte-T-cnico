using System;

namespace SistemaChamados.Models
{
    /// <summary>
    /// Modelo para Contestações (Historial)
    /// Representa uma contestação individual no historial
    /// </summary>
    public class Contestacao
    {
        // Propriedades básicas
        public int Id { get; set; }
        public int IdChamado { get; set; }
        public int IdUsuario { get; set; }
        public string Justificativa { get; set; }
        public DateTime DataContestacao { get; set; }

        // Propriedades de navegação (carregadas do banco)
        public string NomeUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string TipoUsuario { get; set; }

        // Construtor padrão
        public Contestacao()
        {
            DataContestacao = DateTime.Now;
        }

        // Construtor com parâmetros
        public Contestacao(int idChamado, int idUsuario, string justificativa)
        {
            IdChamado = idChamado;
            IdUsuario = idUsuario;
            Justificativa = justificativa;
            DataContestacao = DateTime.Now;
        }

        // Método para validar
        public bool Validar()
        {
            if (IdChamado <= 0)
                throw new ArgumentException("ID do chamado inválido");

            if (IdUsuario <= 0)
                throw new ArgumentException("ID do usuário inválido");

            if (string.IsNullOrWhiteSpace(Justificativa))
                throw new ArgumentException("Justificativa é obrigatória");

            if (Justificativa.Length > 1000)
                throw new ArgumentException("Justificativa não pode ter mais de 1000 caracteres");

            return true;
        }


        // Override ToString para debug
        public override string ToString()
        {
            return $"[{DataContestacao:dd/MM/yyyy HH:mm}] {NomeUsuario ?? $"Usuário {IdUsuario}"}: {Justificativa}";
        }
    }
}
