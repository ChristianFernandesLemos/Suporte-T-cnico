using System;
using System.Data;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Controllers;

namespace SistemaChamados.Data.Repositories
{
    /// <summary>
    /// 📊 REPOSITÓRIO: Geração de Relatórios
    /// - Relatórios gerais
    /// - Relatórios por período
    /// - Estatísticas
    /// </summary>
    public class RelatoriosRepository
    {
        #region 📊 RELATÓRIOS GERAIS

        /// <summary>
        /// Obtém relatório geral de todos os chamados
        /// </summary>
        public DataTable ObterRelatorioGeral()
        {
            var dataTable = new DataTable();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            c.id_chamado AS ID,
                            c.categoria AS Categoria,
                            c.descricao AS Descrição,
                            CASE c.prioridade
                                WHEN 1 THEN 'Baixa'
                                WHEN 2 THEN 'Média'
                                WHEN 3 THEN 'Alta'
                                WHEN 4 THEN 'Crítica'
                            END AS Prioridade,
                            CASE c.Status
                                WHEN 1 THEN 'Aberto'
                                WHEN 2 THEN 'Em Andamento'
                                WHEN 3 THEN 'Resolvido'
                                WHEN 4 THEN 'Fechado'
                                WHEN 5 THEN 'Cancelado'
                            END AS Status,
                            c.Data_Registro AS [Data Registro],
                            c.Data_Resolucao AS [Data Resolução],
                            u1.nome AS Solicitante,
                            u2.nome AS Técnico
                        FROM chamados c
                        INNER JOIN Usuario u1 ON c.Afetado = u1.Id_usuario
                        LEFT JOIN Usuario u2 ON c.Tecnico_Atribuido = u2.Id_usuario
                        ORDER BY c.Data_Registro DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.Fill(dataTable);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório geral: {ex.Message}");
            }
            return dataTable;
        }

        /// <summary>
        /// Obtém relatório de chamados por período
        /// </summary>
        public DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var dataTable = new DataTable();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            c.id_chamado AS ID,
                            c.categoria AS Categoria,
                            c.descricao AS Descrição,
                            CASE c.prioridade
                                WHEN 1 THEN 'Baixa'
                                WHEN 2 THEN 'Média'
                                WHEN 3 THEN 'Alta'
                                WHEN 4 THEN 'Crítica'
                            END AS Prioridade,
                            CASE c.Status
                                WHEN 1 THEN 'Aberto'
                                WHEN 2 THEN 'Em Andamento'
                                WHEN 3 THEN 'Resolvido'
                                WHEN 4 THEN 'Fechado'
                                WHEN 5 THEN 'Cancelado'
                            END AS Status,
                            c.Data_Registro AS [Data Registro],
                            c.Data_Resolucao AS [Data Resolução],
                            u1.nome AS Solicitante,
                            u2.nome AS Técnico
                        FROM chamados c
                        INNER JOIN Usuario u1 ON c.Afetado = u1.Id_usuario
                        LEFT JOIN Usuario u2 ON c.Tecnico_Atribuido = u2.Id_usuario
                        WHERE c.Data_Registro BETWEEN @DataInicio AND @DataFim
                        ORDER BY c.Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@DataInicio", dataInicio);
                        command.Parameters.AddWithValue("@DataFim", dataFim.AddDays(1).AddSeconds(-1));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório por período: {ex.Message}");
            }
            return dataTable;
        }

        /// <summary>
        /// Obtém relatório de chamados por técnico
        /// </summary>
        public DataTable ObterRelatorioPorTecnico(int tecnicoId)
        {
            var dataTable = new DataTable();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            c.id_chamado AS ID,
                            c.categoria AS Categoria,
                            c.descricao AS Descrição,
                            CASE c.prioridade
                                WHEN 1 THEN 'Baixa'
                                WHEN 2 THEN 'Média'
                                WHEN 3 THEN 'Alta'
                                WHEN 4 THEN 'Crítica'
                            END AS Prioridade,
                            CASE c.Status
                                WHEN 1 THEN 'Aberto'
                                WHEN 2 THEN 'Em Andamento'
                                WHEN 3 THEN 'Resolvido'
                                WHEN 4 THEN 'Fechado'
                                WHEN 5 THEN 'Cancelado'
                            END AS Status,
                            c.Data_Registro AS [Data Registro],
                            c.Data_Resolucao AS [Data Resolução],
                            u1.nome AS Solicitante
                        FROM chamados c
                        INNER JOIN Usuario u1 ON c.Afetado = u1.Id_usuario
                        WHERE c.Tecnico_Atribuido = @TecnicoId
                        ORDER BY c.Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TecnicoId", tecnicoId);

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório por técnico: {ex.Message}");
            }
            return dataTable;
        }

        /// <summary>
        /// Obtém relatório de chamados por status
        /// </summary>
        public DataTable ObterRelatorioPorStatus(int status)
        {
            var dataTable = new DataTable();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            c.id_chamado AS ID,
                            c.categoria AS Categoria,
                            c.descricao AS Descrição,
                            CASE c.prioridade
                                WHEN 1 THEN 'Baixa'
                                WHEN 2 THEN 'Média'
                                WHEN 3 THEN 'Alta'
                                WHEN 4 THEN 'Crítica'
                            END AS Prioridade,
                            c.Data_Registro AS [Data Registro],
                            c.Data_Resolucao AS [Data Resolução],
                            u1.nome AS Solicitante,
                            u2.nome AS Técnico
                        FROM chamados c
                        INNER JOIN Usuario u1 ON c.Afetado = u1.Id_usuario
                        LEFT JOIN Usuario u2 ON c.Tecnico_Atribuido = u2.Id_usuario
                        WHERE c.Status = @Status
                        ORDER BY c.Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório por status: {ex.Message}");
            }
            return dataTable;
        }

        #endregion

        #region 📈 ESTATÍSTICAS

        /// <summary>
        /// Obtém estatísticas de funcionários
        /// </summary>
        public EstatisticasFuncionarios ObterEstatisticasFuncionarios()
        {
            var stats = new EstatisticasFuncionarios();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            COUNT(*) as Total,
                            SUM(CASE WHEN Acess_codigo = 3 THEN 1 ELSE 0 END) as Administradores,
                            SUM(CASE WHEN Acess_codigo = 2 THEN 1 ELSE 0 END) as Tecnicos,
                            SUM(CASE WHEN Acess_codigo = 1 THEN 1 ELSE 0 END) as Funcionarios,
                            SUM(CASE WHEN Ativo = 1 THEN 1 ELSE 0 END) as Ativos
                        FROM Usuario";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalFuncionarios = (int)reader["Total"];
                            stats.TotalAdministradores = (int)reader["Administradores"];
                            stats.TotalTecnicos = (int)reader["Tecnicos"];
                            stats.TotalFuncionariosComuns = (int)reader["Funcionarios"];
                            stats.FuncionariosAtivos = (int)reader["Ativos"];
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas de funcionários: {ex.Message}");
            }
            return stats;
        }

        /// <summary>
        /// Obtém estatísticas de chamados
        /// </summary>
        public EstatisticasChamados ObterEstatisticasChamados()
        {
            var stats = new EstatisticasChamados();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            COUNT(*) as Total,
                            SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) as Abertos,
                            SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) as EmAndamento,
                            SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) as Resolvidos,
                            SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END) as Fechados,
                            SUM(CASE WHEN Status = 5 THEN 1 ELSE 0 END) as Cancelados,
                            SUM(CASE WHEN prioridade = 1 THEN 1 ELSE 0 END) as PrioridadeBaixa,
                            SUM(CASE WHEN prioridade = 2 THEN 1 ELSE 0 END) as PrioridadeMedia,
                            SUM(CASE WHEN prioridade = 3 THEN 1 ELSE 0 END) as PrioridadeAlta,
                            SUM(CASE WHEN prioridade = 4 THEN 1 ELSE 0 END) as PrioridadeCritica
                        FROM chamados";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.Total = (int)reader["Total"];
                            stats.Abertos = (int)reader["Abertos"];
                            stats.EmAndamento = (int)reader["EmAndamento"];
                            stats.Resolvidos = (int)reader["Resolvidos"];
                            stats.Fechados = (int)reader["Fechados"];
                            stats.Cancelados = (int)reader["Cancelados"];
                            stats.PrioridadeBaixa = (int)reader["PrioridadeBaixa"];
                            stats.PrioridadeMedia = (int)reader["PrioridadeMedia"];
                            stats.PrioridadeAlta = (int)reader["PrioridadeAlta"];
                            stats.PrioridadeCritica = (int)reader["PrioridadeCritica"];
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas de chamados: {ex.Message}");
            }
            return stats;
        }

        /// <summary>
        /// Obtém tempo médio de resolução (em horas)
        /// </summary>
        public double ObterTempoMedioResolucao()
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT AVG(DATEDIFF(HOUR, Data_Registro, Data_Resolucao)) 
                        FROM chamados 
                        WHERE Data_Resolucao IS NOT NULL";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao calcular tempo médio: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtém chamados por categoria
        /// </summary>
        public DataTable ObterChamadosPorCategoria()
        {
            var dataTable = new DataTable();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            categoria AS Categoria,
                            COUNT(*) AS [Total de Chamados],
                            SUM(CASE WHEN Status IN (3, 4) THEN 1 ELSE 0 END) AS Resolvidos,
                            SUM(CASE WHEN Status IN (1, 2) THEN 1 ELSE 0 END) AS [Em Aberto]
                        FROM chamados
                        GROUP BY categoria
                        ORDER BY COUNT(*) DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.Fill(dataTable);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter chamados por categoria: {ex.Message}");
            }
            return dataTable;
        }

        #endregion
    }

    #region 📊 CLASSES DE ESTATÍSTICAS

    /// <summary>
    /// Estatísticas de chamados
    /// </summary>
    public class EstatisticasChamados
    {
        public int Total { get; set; }
        public int Abertos { get; set; }
        public int EmAndamento { get; set; }
        public int Resolvidos { get; set; }
        public int Fechados { get; set; }
        public int Cancelados { get; set; }
        public int PrioridadeBaixa { get; set; }
        public int PrioridadeMedia { get; set; }
        public int PrioridadeAlta { get; set; }
        public int PrioridadeCritica { get; set; }
    }

    #endregion
}