using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Models;

namespace SistemaChamados.Data.Repositories
{
    /// <summary>
    /// 📦 REPOSITÓRIO: Operações CRUD para Chamados
    /// ATUALIZADO: Sem usar tabela Contestacoes (agora usa Historial_Contestacoes)
    /// </summary>
    public class ChamadosRepository
    {
        #region 🔍 CONSULTAR / BUSCAR

        public Chamados BuscarPorId(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT id_chamado, categoria, prioridade, descricao, Afetado,
                               Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
                        FROM chamados 
                        WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return CriarChamadoFromReader(reader);
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar chamado: {ex.Message}");
                return null;
            }
        }

        public List<Chamados> ListarTodos()
        {
            var chamados = new List<Chamados>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT id_chamado, categoria, prioridade, descricao, Afetado,
                               Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
                        FROM chamados 
                        ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            chamados.Add(CriarChamadoFromReader(reader));
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarPorFuncionario(int funcionarioId)
        {
            var chamados = new List<Chamados>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT id_chamado, categoria, prioridade, descricao, Afetado,
                               Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
                        FROM chamados 
                        WHERE Afetado = @FuncionarioId
                        ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por funcionário: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarPorTecnico(int tecnicoId)
        {
            var chamados = new List<Chamados>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT id_chamado, categoria, prioridade, descricao, Afetado,
                               Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
                        FROM chamados 
                        WHERE Tecnico_Atribuido = @TecnicoId
                        ORDER BY prioridade DESC, Data_Registro ASC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TecnicoId", tecnicoId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por técnico: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarPorStatus(StatusChamado status)
        {
            var chamados = new List<Chamados>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT id_chamado, categoria, prioridade, descricao, Afetado,
                               Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
                        FROM chamados 
                        WHERE Status = @Status
                        ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Status", (int)status);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por status: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarPorPrioridade(int prioridade)
        {
            var chamados = new List<Chamados>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT id_chamado, categoria, prioridade, descricao, Afetado,
                               Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
                        FROM chamados 
                        WHERE prioridade = @Prioridade
                        ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Prioridade", prioridade);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por prioridade: {ex.Message}");
            }
            return chamados;
        }

        #endregion

        #region ➕ INSERIR

        /// <summary>
        /// Insere um novo chamado
        /// ATUALIZADO: Não usa mais a tabela Contestacoes
        /// Contestações agora vão direto para Historial_Contestacoes (via ContestacoesRepository)
        /// </summary>
        public int Inserir(Chamados chamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    // Inserir apenas o chamado - SEM contestações na mesma transação
                    string sql = @"
                        INSERT INTO chamados (categoria, descricao, prioridade, Afetado, 
                                            Data_Registro, Status, Tecnico_Atribuido)
                        VALUES (@Categoria, @Descricao, @Prioridade, @Afetado, 
                                @DataRegistro, @Status, @TecnicoAtribuido);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int novoId;
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                        command.Parameters.AddWithValue("@Afetado", chamado.Afetado);
                        command.Parameters.AddWithValue("@DataRegistro", chamado.DataChamado);
                        command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                        command.Parameters.AddWithValue("@TecnicoAtribuido",
                            chamado.TecnicoResponsavel.HasValue ? (object)chamado.TecnicoResponsavel.Value : DBNull.Value);

                        novoId = (int)command.ExecuteScalar();
                    }

                    // Se tem contestação, inserir no Historial_Contestacoes usando o repository apropriado
                    // NOTA: Isso será feito separadamente pelo ChamadosController
                    // Apenas armazenamos no campo Contestacoes como texto temporário

                    chamado.IdChamado = novoId;
                    Console.WriteLine($"✅ Chamado {novoId} inserido com sucesso");

                    return novoId;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir chamado: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return 0;
            }
        }

        #endregion

        #region 🔄 ATUALIZAR

        /// <summary>
        /// Atualiza um chamado existente
        /// ATUALIZADO: Não toca mais na tabela Contestacoes
        /// </summary>
        public bool Atualizar(Chamados chamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    // Atualizar apenas o chamado
                    string sql = @"
                        UPDATE chamados 
                        SET categoria = @Categoria, descricao = @Descricao, 
                            prioridade = @Prioridade, Status = @Status, 
                            Tecnico_Atribuido = @TecnicoAtribuido, 
                            Data_Resolucao = @DataResolucao
                        WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                        command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                        command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                        command.Parameters.AddWithValue("@TecnicoAtribuido",
                            chamado.TecnicoResponsavel.HasValue ? (object)chamado.TecnicoResponsavel.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@DataResolucao",
                            chamado.DataResolucao.HasValue ? (object)chamado.DataResolucao.Value : DBNull.Value);

                        int rows = command.ExecuteNonQuery();

                        Console.WriteLine($"✅ Chamado {chamado.IdChamado} atualizado ({rows} row(s) affected)");

                        return rows > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar chamado: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region ❌ REMOVER

        /// <summary>
        /// Remove um chamado
        /// ATUALIZADO: Não precisa mais remover de Contestacoes (CASCADE cuida do Historial)
        /// </summary>
        public bool Remover(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    // Remover chamado (CASCADE remove do Historial_Contestacoes automaticamente)
                    string sql = "DELETE FROM chamados WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        int rows = command.ExecuteNonQuery();

                        Console.WriteLine($"✅ Chamado {idChamado} removido ({rows} row(s) affected)");

                        return rows > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover chamado: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 🛠️ MÉTODOS AUXILIARES

        /// <summary>
        /// Cria objeto Chamados a partir do SqlDataReader
        /// </summary>
        private Chamados CriarChamadoFromReader(SqlDataReader reader)
        {
            var chamado = new Chamados
            {
                IdChamado = (int)reader["id_chamado"],
                Categoria = reader["categoria"].ToString(),
                Prioridade = (int)reader["prioridade"],
                Descricao = reader["descricao"].ToString(),
                Afetado = (int)reader["Afetado"],
                DataChamado = (DateTime)reader["Data_Registro"],
                Status = (StatusChamado)(int)reader["Status"],
                TecnicoResponsavel = reader.IsDBNull(reader.GetOrdinal("Tecnico_Atribuido")) ?
                    (int?)null : (int)reader["Tecnico_Atribuido"],
                DataResolucao = reader.IsDBNull(reader.GetOrdinal("Data_Resolucao")) ?
                    (DateTime?)null : (DateTime)reader["Data_Resolucao"]
            };

            // Buscar contestações do Historial_Contestacoes
            chamado.Contestacoes = BuscarContestacaoTexto(chamado.IdChamado);

            return chamado;
        }

        /// <summary>
        /// Busca contestações do Historial_Contestacoes como texto concatenado
        /// </summary>
        private string BuscarContestacaoTexto(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT TOP 1 Justificativa 
                        FROM Historial_Contestacoes
                        WHERE id_chamado = @IdChamado
                        ORDER BY DataContestacao DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        var result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value ? result.ToString() : null;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar contestações: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}