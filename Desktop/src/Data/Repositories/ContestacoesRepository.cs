using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Models;

namespace SistemaChamados.Data.Repositories
{
    public class ContestacoesRepository
    {
        #region CONSULTAR / BUSCAR

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
                            hc.DataContestacao
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
                            hc.DataContestacao
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
                            hc.DataContestacao
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
                            hc.DataContestacao
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

        #region INSERIR

        public int Inserir(Contestacao contestacao)
        {
            try
            {
                contestacao.Validar();

                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        INSERT INTO Historial_Contestacoes (id_chamado, Id_usuario, Justificativa, DataContestacao)
                        VALUES (@IdChamado, @IdUsuario, @Justificativa, @DataContestacao);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", contestacao.IdChamado);
                        command.Parameters.AddWithValue("@IdUsuario", contestacao.IdUsuario);
                        command.Parameters.AddWithValue("@Justificativa", contestacao.Justificativa);
                        command.Parameters.AddWithValue("@DataContestacao", contestacao.DataContestacao);

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

        #endregion

        #region REMOVER

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

        #region MÉTODOS AUXILIARES

        // ✅✅✅ MÉTODO CORREGIDO SIN TipoContestacao ✅✅✅
        private Contestacao CriarContestacaoFromReader(SqlDataReader reader)
        {
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
                DataContestacao = (DateTime)reader["DataContestacao"]
            };
        }

        #endregion
    }
}