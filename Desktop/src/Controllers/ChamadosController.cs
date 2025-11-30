using System;
using System.Collections.Generic;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;
using SistemaChamados.Data.Repositories;

namespace SistemaChamados.Controllers
{
    public class ChamadosController
    {
        private readonly IDatabaseConnection _database;
        private readonly ContestacoesRepository _contestacoesRepository;

        public ChamadosController(IDatabaseConnection database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _contestacoesRepository = new ContestacoesRepository();
        }

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

                // Guardar contestação temporariamente
                string contestacaoTexto = chamado.Contestacoes;

                // Limpar contestação do objeto antes de inserir
                // (O repositório não toca mais na tabela Contestacoes)
                chamado.Contestacoes = null;

                // Inserir no banco e retornar ID
                int idChamado = _database.InserirChamado(chamado);

                // Se havia contestação, inserir no Historial_Contestacoes
                if (idChamado > 0 && !string.IsNullOrEmpty(contestacaoTexto))
                {
                    try
                    {
                        var contestacao = new Contestacao(
                            idChamado,
                            chamado.Afetado,
                            contestacaoTexto
                        );

                        int contestacaoId = _contestacoesRepository.Inserir(contestacao);

                        if (contestacaoId > 0)
                        {
                            Console.WriteLine($"✅ Contestação {contestacaoId} adicionada ao chamado {idChamado}");
                        }
                    }
                    catch (Exception exCont)
                    {
                        Console.WriteLine($"⚠️ Erro ao adicionar contestação: {exCont.Message}");
                    }
                }

                return idChamado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar chamado: {ex.Message}");
                throw;
            }
        }

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

        /// <summary>
        /// Adiciona uma contestação usando o novo sistema de Historial
        /// </summary>
        public bool AdicionarContestacao(int idChamado, string contestacao)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contestacao))
                    throw new ArgumentException("Contestação não pode ser vazia");

                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                    throw new Exception("Chamado não encontrado");

                var novaContestacao = new Contestacao(
                    idChamado,
                    chamado.TecnicoResponsavel ?? chamado.Afetado,
                    contestacao
                );

                int contestacaoId = _contestacoesRepository.Inserir(novaContestacao);
                return contestacaoId > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar contestação: {ex.Message}");
                return false;
            }
        }

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