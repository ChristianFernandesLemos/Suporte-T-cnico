
using System;
using System.Collections.Generic;
using SistemaChamados.Config;
using SistemaChamados.Data;
using SistemaChamados.Models;

namespace SistemaChamados.Controllers
{
    public class ChamadosController
    {
        private readonly SqlServerConnection _database;

        // Constructor con parámetro
        public ChamadosController(SqlServerConnection database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        // Constructor sin parámetros para compatibilidad
        public ChamadosController()
        {
            string connectionString = DatabaseConfig.ConnectionString;
            _database = new SqlServerConnection(connectionString);
        }

        // CORRIGIDO: Criar chamado unificado
        public int CriarChamado(Chamados chamado)
        {
            try
            {
                if (chamado == null)
                    throw new ArgumentNullException(nameof(chamado));

                // Validações básicas
                if (string.IsNullOrWhiteSpace(chamado.Descricao))
                    throw new ArgumentException("Descrição é obrigatória");

                if (string.IsNullOrWhiteSpace(chamado.Categoria))
                    throw new ArgumentException("Categoria é obrigatória");

                // Configurar dados padrão
                chamado.DataChamado = DateTime.Now;
                chamado.Status = StatusChamado.Aberto;

                // Inserir no banco e retornar ID
                return _database.InserirChamado(chamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar chamado: {ex.Message}");
                throw;
            }
        }

        // CORRIGIDO: Listar chamados por funcionário
        public List<Chamados> ListarChamadosPorFuncionario(int funcionarioId)
        {
            try
            {
                return _database.ListarChamadosPorFuncionario(funcionarioId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por funcionário: {ex.Message}");
                return new List<Chamados>();
            }
        }

        // CORRIGIDO: Listar chamados por técnico
        public List<Chamados> ListarChamadosPorTecnico(int tecnicoId)
        {
            try
            {
                return _database.ListarChamadosPorTecnico(tecnicoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por técnico: {ex.Message}");
                return new List<Chamados>();
            }
        }

        // CORRIGIDO: Listar todos os chamados
        public List<Chamados> ListarTodosChamados()
        {
            try
            {
                return _database.ListarTodosChamados();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar todos os chamados: {ex.Message}");
                return new List<Chamados>();
            }
        }

        // CORRIGIDO: Alterar status implementado
        public bool AlterarStatus(int idChamado, int novoStatus)
        {
            try
            {
                if (!Enum.IsDefined(typeof(StatusChamado), novoStatus))
                    throw new ArgumentException("Status inválido");

                return AlterarStatus(idChamado, (StatusChamado)novoStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar status: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para enum
        public bool AlterarStatus(int idChamado, StatusChamado novoStatus)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                    throw new Exception("Chamado não encontrado");

                chamado.AlterarStatus(novoStatus);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar status: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Alterar prioridade implementado
        public bool AlterarPrioridade(int idChamado, int novaPrioridade)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                    throw new Exception("Chamado não encontrado");

                chamado.AlterarPrioridade(novaPrioridade);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar prioridade: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Atribuir técnico implementado
        public bool AtribuirTecnico(int idChamado, int idTecnico)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                    throw new Exception("Chamado não encontrado");

                chamado.AtribuirTecnico(idTecnico);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atribuir técnico: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Marcar como resolvido implementado
        public bool MarcarComoResolvido(int idChamado)
        {
            try
            {
                return AlterarStatus(idChamado, StatusChamado.Resolvido);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao marcar como resolvido: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Fechar chamado implementado
        public bool FecharChamado(int idChamado)
        {
            try
            {
                return AlterarStatus(idChamado, StatusChamado.Fechado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fechar chamado: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Reabrir chamado implementado
        public bool ReabrirChamado(int idChamado)
        {
            try
            {
                return AlterarStatus(idChamado, StatusChamado.Aberto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao reabrir chamado: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Adicionar contestação implementado
        public bool AdicionarContestacao(int idChamado, string contestacao)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contestacao))
                    throw new ArgumentException("Contestação não pode ser vazia");

                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                    throw new Exception("Chamado não encontrado");

                chamado.AdicionarContestacao(contestacao);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar contestação: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Atualizar chamado completo
        public bool AtualizarChamado(Chamados chamado)
        {
            try
            {
                if (chamado == null)
                    throw new ArgumentNullException(nameof(chamado));

                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar chamado: {ex.Message}");
                return false;
            }
        }

        // Buscar chamado por ID
        public Chamados BuscarChamadoPorId(int idChamado)
        {
            try
            {
                return _database.BuscarChamadoPorId(idChamado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar chamado: {ex.Message}");
                return null;
            }
        }

        // Estatísticas
        public Dictionary<string, int> ObterEstatisticas()
        {
            try
            {
                var estatisticas = new Dictionary<string, int>();
                var todosChamados = _database.ListarTodosChamados();

                estatisticas["Total"] = todosChamados.Count;
                estatisticas["Abertos"] = todosChamados.FindAll(c => c.Status == StatusChamado.Aberto).Count;
                estatisticas["EmAndamento"] = todosChamados.FindAll(c => c.Status == StatusChamado.EmAndamento).Count;
                estatisticas["Resolvidos"] = todosChamados.FindAll(c => c.Status == StatusChamado.Resolvido).Count;
                estatisticas["Fechados"] = todosChamados.FindAll(c => c.Status == StatusChamado.Fechado).Count;
                estatisticas["PrioridadeAlta"] = todosChamados.FindAll(c => c.Prioridade >= 3).Count;
                estatisticas["PrioridadeCritica"] = todosChamados.FindAll(c => c.Prioridade == 4).Count;

                return estatisticas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }
    }
}