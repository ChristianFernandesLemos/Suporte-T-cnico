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
    /// - Separado da lógica de conexão
    /// - Responsável APENAS por operações de dados
    /// </summary>
    public class ChamadosRepository
    {
        #region 🔍 CONSULTAR / BUSCAR

        /// <summary>
        /// Busca um chamado por ID
        /// </summary>
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

        /// <summary>
        /// Lista todos os chamados
        /// </summary>
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

        /// <summary>
        /// Lista chamados de um funcionário específico
        /// </summary>
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

        /// <summary>
        /// Lista chamados atribuídos a um técnico
        /// </summary>
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

        /// <summary>
        /// Lista chamados por status
        /// </summary>
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

        /// <summary>
        /// Lista chamados por prioridade
        /// </summary>
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
        /// </summary>
        public int Inserir(Chamados chamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Inserir chamado
                            string sql = @"
                                INSERT INTO chamados (categoria, descricao, prioridade, Afetado, 
                                                    Data_Registro, Status, Tecnico_Atribuido)
                                VALUES (@Categoria, @Descricao, @Prioridade, @Afetado, 
                                        @DataRegistro, @Status, @TecnicoAtribuido);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int novoId;
                            using (var command = new SqlCommand(sql, connection, transaction))
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

                            // 2. Inserir contestação se existe
                            if (!string.IsNullOrEmpty(chamado.Contestacoes))
                            {
                                string sqlCont = @"
                                    INSERT INTO Contestacoes (Justificativa, DataContestacao)
                                    VALUES (@Justificativa, @DataContestacao);
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                                int codigoContestacao;
                                using (var cmd = new SqlCommand(sqlCont, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Justificativa", chamado.Contestacoes);
                                    cmd.Parameters.AddWithValue("@DataContestacao", DateTime.Now);
                                    codigoContestacao = (int)cmd.ExecuteScalar();
                                }

                                // Atualizar chamado com o código da contestação
                                string sqlUpdate = "UPDATE chamados SET Contestacoes_Codigo = @Codigo WHERE id_chamado = @IdChamado";
                                using (var cmd = new SqlCommand(sqlUpdate, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Codigo", codigoContestacao);
                                    cmd.Parameters.AddWithValue("@IdChamado", novoId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            chamado.IdChamado = novoId;
                            return novoId;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir chamado: {ex.Message}");
                return 0;
            }
        }

        #endregion

        #region 🔄 ATUALIZAR

        /// <summary>
        /// Atualiza um chamado existente
        /// </summary>
        public bool Atualizar(Chamados chamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Atualizar chamado
                            string sql = @"
                                UPDATE chamados 
                                SET categoria = @Categoria, descricao = @Descricao, 
                                    prioridade = @Prioridade, Status = @Status, 
                                    Tecnico_Atribuido = @TecnicoAtribuido, 
                                    Data_Resolucao = @DataResolucao
                                WHERE id_chamado = @IdChamado";

                            using (var command = new SqlCommand(sql, connection, transaction))
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

                                command.ExecuteNonQuery();
                            }

                            // 2. Atualizar contestação si existe
                            if (!string.IsNullOrEmpty(chamado.Contestacoes))
                            {
                                AtualizarContestacao(connection, transaction, chamado);
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
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
        /// </summary>
        public bool Remover(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Obter código da contestação
                            int? codigoCont = null;
                            string sqlGetCodigo = "SELECT Contestacoes_Codigo FROM chamados WHERE id_chamado = @IdChamado";

                            using (var cmd = new SqlCommand(sqlGetCodigo, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@IdChamado", idChamado);
                                var result = cmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    codigoCont = (int)result;
                            }

                            // 2. Remover chamado
                            string sql = "DELETE FROM chamados WHERE id_chamado = @IdChamado";
                            using (var command = new SqlCommand(sql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@IdChamado", idChamado);
                                command.ExecuteNonQuery();
                            }

                            // 3. Remover contestação se existe
                            if (codigoCont.HasValue)
                            {
                                string sqlDelCont = "DELETE FROM Contestacoes WHERE Codigo = @Codigo";
                                using (var cmd = new SqlCommand(sqlDelCont, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Codigo", codigoCont.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
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

            // Buscar contestação
            chamado.Contestacoes = BuscarContestacao(chamado.IdChamado);

            return chamado;
        }

        /// <summary>
        /// Busca contestação de um chamado
        /// </summary>
        private string BuscarContestacao(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT c.Justificativa 
                        FROM chamados ch
                        INNER JOIN Contestacoes c ON ch.Contestacoes_Codigo = c.Codigo
                        WHERE ch.id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        var result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value ? result.ToString() : null;
                    }
                });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Atualiza ou cria contestação
        /// </summary>
        private void AtualizarContestacao(SqlConnection connection, SqlTransaction transaction, Chamados chamado)
        {
            // Verificar se já tem contestação
            string sqlGetCodigo = "SELECT Contestacoes_Codigo FROM chamados WHERE id_chamado = @IdChamado";
            int? codigoExistente = null;

            using (var cmd = new SqlCommand(sqlGetCodigo, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    codigoExistente = (int)result;
            }

            if (codigoExistente.HasValue)
            {
                // Atualizar contestação existente
                string sqlUpdate = @"
                    UPDATE Contestacoes 
                    SET Justificativa = @Justificativa, DataContestacao = @DataContestacao
                    WHERE Codigo = @Codigo";

                using (var cmd = new SqlCommand(sqlUpdate, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigoExistente.Value);
                    cmd.Parameters.AddWithValue("@Justificativa", chamado.Contestacoes);
                    cmd.Parameters.AddWithValue("@DataContestacao", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // Criar nova contestação
                string sqlInsert = @"
                    INSERT INTO Contestacoes (Justificativa, DataContestacao)
                    VALUES (@Justificativa, @DataContestacao);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int nuevoCodigo;
                using (var cmd = new SqlCommand(sqlInsert, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Justificativa", chamado.Contestacoes);
                    cmd.Parameters.AddWithValue("@DataContestacao", DateTime.Now);
                    nuevoCodigo = (int)cmd.ExecuteScalar();
                }

                // Vincular ao chamado
                string sqlUpdateChamado = "UPDATE chamados SET Contestacoes_Codigo = @Codigo WHERE id_chamado = @IdChamado";
                using (var cmd = new SqlCommand(sqlUpdateChamado, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Codigo", nuevoCodigo);
                    cmd.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}