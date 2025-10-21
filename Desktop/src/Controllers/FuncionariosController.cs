using System;
using System.Collections.Generic;
using SistemaChamados.Data;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Controllers
{
    public class FuncionariosController
    {
        private readonly SqlServerConnection _database;

        public FuncionariosController(SqlServerConnection database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        

        public Funcionarios RealizarLogin(string email, string senha)
        {
            try
            {
                if (_database.ValidarLogin(email, senha))
                {
                    return _database.BuscarFuncionarioPorEmail(email);
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro no login: {ex.Message}");
                return null;
            }
        }

        // CORRIGIDO: Método implementado
        public Funcionarios BuscarFuncionarioPorId(int idFuncionario)
        {
            try
            {
                return _database.BuscarFuncionarioPorId(idFuncionario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário: {ex.Message}");
                return null;
            }
        }

        // CORRIGIDO: Método implementado
        public List<Funcionarios> ListarTodosFuncionarios()
        {
            try
            {
                return _database.ListarTodosFuncionarios();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar funcionários: {ex.Message}");
                return new List<Funcionarios>();
            }
        }

        // CORRIGIDO: Método implementado
        public List<Tecnico> ListarTecnicos()
        {
            try
            {
                return _database.ListarTecnicos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar técnicos: {ex.Message}");
                return new List<Tecnico>();
            }
        }

        // CORRIGIDO: Adicionar funcionário com polimorfismo
        public int AdicionarFuncionario(Funcionarios funcionario)
        {
            try
            {
                return _database.InserirFuncionario(funcionario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar funcionário: {ex.Message}");
                return 0;
            }
        }

        // CORRIGIDO: Atualizar funcionário
        public bool AtualizarFuncionario(Funcionarios funcionario)
        {
            try
            {
                return _database.AtualizarFuncionario(funcionario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar funcionário: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Alterar senha implementado
        public bool AlterarSenha(int funcionarioId, string novaSenha)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(novaSenha))
                    throw new ArgumentException("Nova senha não pode ser vazia");

                return _database.AlterarSenha(funcionarioId, novaSenha);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar senha: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Alterar nível de acesso
        public bool AlterarNivelAcesso(int funcionarioId, int novoNivel)
        {
            try
            {
                if (novoNivel < 1 || novoNivel > 3)
                    throw new ArgumentException("Nível de acesso inválido");

                return _database.AlterarNivelAcesso(funcionarioId, novoNivel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar nível de acesso: {ex.Message}");
                return false;
            }
        }

        // CORRIGIDO: Excluir funcionário
        public bool ExcluirFuncionario(int funcionarioId)
        {
            try
            {
                return _database.ExcluirFuncionario(funcionarioId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir funcionário: {ex.Message}");
                return false;
            }
        }

        // Estatísticas
        public EstatisticasFuncionarios ObterEstatisticasFuncionarios(Funcionarios solicitante)
        {
            try
            {
                if (solicitante.NivelAcesso < 3)
                    return null;

                return _database.ObterEstatisticasFuncionarios();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas: {ex.Message}");
                return null;
            }
        }
    }

    public class EstatisticasFuncionarios
    {
        public int TotalFuncionarios { get; set; }
        public int TotalAdministradores { get; set; }
        public int TotalTecnicos { get; set; }
        public int TotalFuncionariosComuns { get; set; }
        public int FuncionariosAtivos { get; set; }
    }
}