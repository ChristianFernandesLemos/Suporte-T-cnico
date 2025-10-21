using SistemaChamados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaChamados.Models
{
    
    public class Funcionario : Funcionarios
    {

        public Funcionario() : base() {
           
        }
        public Funcionario(int id, string nome, string cpf, string email, string senha, int nivelAcesso)
            : base(id, nome, cpf, email, senha, nivelAcesso)
        {
        }

        public override string TipoFuncionario => "Funcionário";
        public override int NivelAcesso => 1;
        public string Departamento { get; set; }
        public string Cargo { get; set; }
        

        /// <summary>
        /// Cria um novo chamado para este funcionário
        /// </summary>
        /// <param name="categoria">Categoria do chamado</param>
        /// <param name="descricao">Descrição do problema</param>
        /// <param name="prioridade">Prioridade inicial (padrão: 2 - Média)</param>
        /// <returns>ID do chamado criado</returns>
        public int CriarChamado(string categoria, string descricao, int prioridade = 2)
        {
            try
            {
                var chamado = new Chamados
                {
                    Categoria = categoria,
                    Descricao = descricao,
                    Prioridade = prioridade,
                    Afetado = this.Id,
                    DataChamado = DateTime.Now,
                    Status = StatusChamado.Aberto
                };

                // Aqui você implementaria a lógica para salvar no banco
                // Por enquanto, retorna um ID fictício
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar chamado: {ex.Message}");
            }
        }

        /// <summary>
        /// Lista os chamados criados por este funcionário
        /// </summary>
        /// <returns>Lista de chamados do funcionário</returns>
        public List<Chamados> ListarMeusChamados()
        {
            try
            {
                // Implementar lógica para buscar chamados no banco
                // Filtrar apenas os chamados onde Afetado = this.Id
                return new List<Chamados>(); // Placeholder
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar chamados: {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona uma contestação a um chamado próprio
        /// </summary>
        /// <param name="idChamado">ID do chamado</param>
        /// <param name="contestacao">Texto da contestação</param>
        public void AdicionarContestacao(int idChamado, string contestacao)
        {
            try
            {
                // Verificar se o chamado pertence a este funcionário
                // Adicionar contestação ao chamado
                var chamado = new Chamados(); // Buscar do banco
                if (chamado.Afetado == this.Id)
                {
                    chamado.AdicionarContestacao(contestacao);
                }
                else
                {
                    throw new UnauthorizedAccessException("Você só pode contestar seus próprios chamados.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar contestação: {ex.Message}");
            }
        }

        /// <summary>
        /// Altera sua própria senha
        /// </summary>
        /// <param name="senhaAtual">Senha atual</param>
        /// <param name="novaSenha">Nova senha</param>
        public void AlterarMinhaSenha(string senhaAtual, string novaSenha)
        {
            try
            {
                if (this.Senha == senhaAtual)
                {
                    this.Senha = novaSenha;
                    // Implementar lógica para atualizar no banco
                }
                else
                {
                    throw new UnauthorizedAccessException("Senha atual incorreta.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar senha: {ex.Message}");
            }
        }


        /// <summary>
        /// Verifica se pode realizar determinada ação
        /// </summary>
        /// <param name="acao">Ação a ser verificada</param>
        /// <returns>True se pode realizar a ação</returns>
        public bool PodeRealizarAcao(AcaoSistema acao)
        {
            switch (acao)
            {
                case AcaoSistema.CriarChamado:
                case AcaoSistema.VisualizarProprioChamado:
                case AcaoSistema.Contestarchamado:
                case AcaoSistema.AlterarPropriaSenha:
                    return true;

                case AcaoSistema.GerenciarUsuarios:
                case AcaoSistema.AlterarSenhaOutroUsuario:
                case AcaoSistema.VisualizarTodosChamados:
                case AcaoSistema.AtribuirTecnico:
                case AcaoSistema.MarcarComoResolvido:
                    return false;

                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Enumeração das ações possíveis no sistema
    /// </summary>
    
    public enum AcaoSistema
    {
        CriarChamado,
        VisualizarProprioChamado,
        VisualizarTodosChamados,
        Contestarchamado,
        MarcarComoResolvido,
        AtribuirTecnico,
        GerenciarUsuarios,
        AlterarPropriaSenha,
        AlterarSenhaOutroUsuario,
        GerarRelatorio
    }
    

    /// <summary>
    /// Classe para estatísticas de funcionário
    /// </summary>
    public class EstatisticasFuncionario
    {
        public int TotalChamados { get; set; }
        public int ChamadosAbertos { get; set; }
        public int ChamadosEmAndamento { get; set; }
        public int ChamadosResolvidos { get; set; }
        public int ChamadosFechados { get; set; }
        public TimeSpan TempoMedioResolucao { get; set; }
        public string CategoriaComMaisChamados { get; set; }
    }
}
