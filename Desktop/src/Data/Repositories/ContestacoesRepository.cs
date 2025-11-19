using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Models;

namespace SistemaChamados.Data.Repositories
{
    /// <summary>
    /// 📦 REPOSITÓRIO: Operações para Contestações (Historial)
    /// </summary>
    public class ContestacoesRepository
    {
        #region 🔍 CONSULTAR / BUSCAR

        /// <summary>
        /// Lista todas as contestações de um chamado (ordenadas por data DESC)
        /// </summary>
        public List<Contestacao> ListarPorChamado(int idChamado)
        {
            var contestacoes = new List<Contestacao>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            hc.Id,
                            hc.id_chamado AS IdChamado,
                            hc.Id_usuario AS IdUsuario,
                            u.nome AS NomeUsuario,
                            e.E_mail AS EmailUsuario,
                            n.Nivel_acesso AS TipoUsuario,
                            hc.Justificativa,
                            hc.DataContestacao,
                            hc.Tipo
                        FROM Historial_Contestacoes hc
                        INNER JOIN Usuario u ON hc.Id_usuario = u.Id_usuario
                        LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
                        LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                        WHERE hc.id_chamado = @IdChamado
                        ORDER BY hc.DataContestacao DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                contestacoes.Add(CriarContestacaoFromReader(reader));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar contestações: {ex.Message}");
            }
            return contestacoes;
        }

        /// <summary>
        /// Busca uma contestação específica por ID
        /// </summary>
        public Contestacao BuscarPorId(int id)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            hc.Id,
                            hc.id_chamado AS IdChamado,
                            hc.Id_usuario AS IdUsuario,
                            u.nome AS NomeUsuario,
                            e.E_mail AS EmailUsuario,
                            n.Nivel_acesso AS TipoUsuario,
                            hc.Justificativa,
                            hc.DataContestacao,
                            hc.Tipo
                        FROM Historial_Contestacoes hc
                        INNER JOIN Usuario u ON hc.Id_usuario = u.Id_usuario
                        LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
                        LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                        WHERE hc.Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return CriarContestacaoFromReader(reader);
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar contestação: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtém a última contestação de um chamado
        /// </summary>
        public Contestacao ObterUltima(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT TOP 1
                            hc.Id,
                            hc.id_chamado AS IdChamado,
                            hc.Id_usuario AS IdUsuario,
                            u.nome AS NomeUsuario,
                            e.E_mail AS EmailUsuario,
                            n.Nivel_acesso AS TipoUsuario,
                            hc.Justificativa,
                            hc.DataContestacao,
                            hc.Tipo
                        FROM Historial_Contestacoes hc
                        INNER JOIN Usuario u ON hc.Id_usuario = u.Id_usuario
                        LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
                        LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                        WHERE hc.id_chamado = @IdChamado
                        ORDER BY hc.DataContestacao DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return CriarContestacaoFromReader(reader);
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter última contestação: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Conta quantas contestações tem um chamado
        /// </summary>
        public int ContarPorChamado(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = "SELECT COUNT(*) FROM Historial_Contestacoes WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        return (int)command.ExecuteScalar();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao contar contestações: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Lista contestações por usuário
        /// </summary>
        public List<Contestacao> ListarPorUsuario(int idUsuario)
        {
            var contestacoes = new List<Contestacao>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT 
                            hc.Id,
                            hc.id_chamado AS IdChamado,
                            hc.Id_usuario AS IdUsuario,
                            u.nome AS NomeUsuario,
                            e.E_mail AS EmailUsuario,
                            n.Nivel_acesso AS TipoUsuario,
                            hc.Justificativa,
                            hc.DataContestacao,
                            hc.Tipo
                        FROM Historial_Contestacoes hc
                        INNER JOIN Usuario u ON hc.Id_usuario = u.Id_usuario
                        LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
                        LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                        WHERE hc.Id_usuario = @IdUsuario
                        ORDER BY hc.DataContestacao DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                contestacoes.Add(CriarContestacaoFromReader(reader));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar contestações por usuário: {ex.Message}");
            }
            return contestacoes;
        }

        #endregion

        #region ➕ INSERIR

        /// <summary>
        /// Adiciona uma nova contestação
        /// </summary>
        public int Inserir(Contestacao contestacao)
        {
            try
            {
                // Validar antes de inserir
                contestacao.Validar();

                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        INSERT INTO Historial_Contestacoes (id_chamado, Id_usuario, Justificativa, DataContestacao, Tipo)
                        VALUES (@IdChamado, @IdUsuario, @Justificativa, @DataContestacao, @Tipo);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", contestacao.IdChamado);
                        command.Parameters.AddWithValue("@IdUsuario", contestacao.IdUsuario);
                        command.Parameters.AddWithValue("@Justificativa", contestacao.Justificativa);
                        command.Parameters.AddWithValue("@DataContestacao", contestacao.DataContestacao);
                        command.Parameters.AddWithValue("@Tipo", contestacao.Tipo.ToString());

                        int novoId = (int)command.ExecuteScalar();
                        contestacao.Id = novoId;
                        return novoId;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir contestação: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adiciona uma contestação usando stored procedure
        /// </summary>
        public int InserirUsandoSP(int idChamado, int idUsuario, string justificativa, TipoContestacao tipo = TipoContestacao.Contestacao)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    using (var command = new SqlCommand("sp_AdicionarContestacao", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        command.Parameters.AddWithValue("@Justificativa", justificativa);
                        command.Parameters.AddWithValue("@Tipo", tipo.ToString());

                        return (int)command.ExecuteScalar();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir contestação (SP): {ex.Message}");
                throw;
            }
        }

        #endregion

        #region ❌ REMOVER

        /// <summary>
        /// Remove uma contestação (usar com cuidado!)
        /// </summary>
        public bool Remover(int id)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = "DELETE FROM Historial_Contestacoes WHERE Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover contestação: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remove todas as contestações de um chamado
        /// </summary>
        public bool RemoverTodosDoChamado(int idChamado)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = "DELETE FROM Historial_Contestacoes WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        return command.ExecuteNonQuery() > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover contestações do chamado: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 🛠️ MÉTODOS AUXILIARES

        /// <summary>
        /// Cria objeto Contestacao a partir do SqlDataReader
        /// </summary>
        private Contestacao CriarContestacaoFromReader(SqlDataReader reader)
        {
            // Parse do tipo (string -> enum)
            TipoContestacao tipo = TipoContestacao.Contestacao;
            string tipoStr = reader["Tipo"].ToString();
            if (Enum.TryParse(tipoStr, out TipoContestacao tipoEnum))
                tipo = tipoEnum;

            return new Contestacao
            {
                Id = (int)reader["Id"],
                IdChamado = (int)reader["IdChamado"],
                IdUsuario = (int)reader["IdUsuario"],
                NomeUsuario = reader["NomeUsuario"].ToString(),
                EmailUsuario = reader.IsDBNull(reader.GetOrdinal("EmailUsuario")) ? 
                    null : reader["EmailUsuario"].ToString(),
                TipoUsuario = reader.IsDBNull(reader.GetOrdinal("TipoUsuario")) ? 
                    null : reader["TipoUsuario"].ToString(),
                Justificativa = reader["Justificativa"].ToString(),
                DataContestacao = (DateTime)reader["DataContestacao"],
                Tipo = tipo
            };
        }

        #endregion
    }
}
