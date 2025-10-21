// IDatabaseConnection.cs
using System;
using System.Collections.Generic;
using System.Data;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Interfaces
{
    public interface IDatabaseConnection : IDisposable
    {
        // Métodos de Funcionários
        bool ValidarLogin(string email, string senha);
        Funcionarios BuscarFuncionarioPorEmail(string email);
        Funcionarios BuscarFuncionarioPorId(int id);
        List<Funcionarios> ListarTodosFuncionarios();
        List<Tecnico> ListarTecnicos();
        int InserirFuncionario(Funcionarios funcionario);
        bool AtualizarFuncionario(Funcionarios funcionario);
        bool AlterarSenha(int funcionarioId, string novaSenha);
        bool AlterarNivelAcesso(int funcionarioId, int novoNivel);
        bool ExcluirFuncionario(int funcionarioId);
        EstatisticasFuncionarios ObterEstatisticasFuncionarios();

        // Métodos de Chamados
        int InserirChamado(Chamados chamado);
        bool AtualizarChamado(Chamados chamado);
        Chamados BuscarChamadoPorId(int id);
        List<Chamados> ListarTodosChamados();
        List<Chamados> ListarChamadosPorFuncionario(int funcionarioId);
        List<Chamados> ListarChamadosPorTecnico(int tecnicoId);
        List<Chamados> ListarChamadosPorStatus(StatusChamado status);
        List<Chamados> ListarChamadosPorPrioridade(int prioridade);

        // Relatórios
        DataTable ObterRelatorioGeral();
        DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim);

        // Conexão
        bool TestarConexao();
    }
}
