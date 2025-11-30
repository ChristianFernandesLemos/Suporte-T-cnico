using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Controllers; // Asegúrate de tener los Enums aquí (StatusChamado)
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Data
{
    /// <summary>
    /// ✅ Sincronizado con: ChamadosRepository, FuncionariosRepository, RelatoriosRepository y ContestacoesRepository.
    /// - Eliminada lógica obsoleta de tabla 'Contestacoes'.
    /// - Implementada lógica de 'Historial_Contestacoes'.
    /// - Unificados criterios de limpieza de texto y conversión de tipos.
    /// </summary>
    public class SqlServerConnection : IDatabaseConnection, IDisposable
    {
        private readonly string _connectionString;

        public SqlServerConnection(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        #region CONEXIÓN
        public bool TestarConexao()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao testar conexão: {ex.Message}");
                return false;
            }
        }

        public bool AbrirConexao() => TestarConexao();
        public void FecharConexao() { }
        public void Dispose() => GC.SuppressFinalize(this);
        #endregion

        #region CHAMADOS - BUSCAR/LISTAR (Sincronizado con ChamadosRepository)

        // SQL Base compartido para evitar duplicidad, igual que en el Repository
        private const string SQL_SELECT_CHAMADOS = @"
            SELECT id_chamado, titulo, categoria, prioridade, descricao, Afetado,
                   Data_Registro, Status, Tecnico_Atribuido, Data_Resolucao
            FROM chamados ";

        public Chamados BuscarChamadoPorId(int idChamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = SQL_SELECT_CHAMADOS + " WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return CriarChamadoFromReader(reader, connection); // Pasamos conexión para buscar contestación
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar chamado: {ex.Message}");
            }
            return null;
        }

        public List<Chamados> ListarTodosChamados()
        {
            var chamados = new List<Chamados>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = SQL_SELECT_CHAMADOS + " ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            chamados.Add(CriarChamadoFromReader(reader, connection)); // Pasamos conexión
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarChamadosPorFuncionario(int funcionarioId)
        {
            var chamados = new List<Chamados>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = SQL_SELECT_CHAMADOS + " WHERE Afetado = @FuncionarioId ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader, connection));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por funcionário: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarChamadosPorTecnico(int tecnicoId)
        {
            var chamados = new List<Chamados>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = SQL_SELECT_CHAMADOS + " WHERE Tecnico_Atribuido = @TecnicoId ORDER BY prioridade DESC, Data_Registro ASC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TecnicoId", tecnicoId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader, connection));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por técnico: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarChamadosPorStatus(StatusChamado status)
        {
            var chamados = new List<Chamados>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = SQL_SELECT_CHAMADOS + " WHERE Status = @Status ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Status", (int)status);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader, connection));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por status: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarChamadosPorPrioridade(int prioridade)
        {
            var chamados = new List<Chamados>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = SQL_SELECT_CHAMADOS + " WHERE prioridade = @Prioridade ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Prioridade", prioridade);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                chamados.Add(CriarChamadoFromReader(reader, connection));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por prioridade: {ex.Message}");
            }
            return chamados;
        }
        #endregion

        #region CHAMADOS - INSERIR/ATUALIZAR/REMOVER

        public int InserirChamado(Chamados chamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Implementación idéntica a ChamadosRepository
                    string sql = @"
                        INSERT INTO chamados (titulo, categoria, descricao, prioridade, Afetado, 
                                            Data_Registro, Status, Tecnico_Atribuido)
                        VALUES (@Titulo, @Categoria, @Descricao, @Prioridade, @Afetado, 
                                @DataRegistro, @Status, @TecnicoAtribuido);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Titulo", string.IsNullOrWhiteSpace(chamado.Titulo) ? "Sem título" : chamado.Titulo);
                        command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                        command.Parameters.AddWithValue("@Afetado", chamado.Afetado);
                        command.Parameters.AddWithValue("@DataRegistro", chamado.DataChamado);
                        command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                        command.Parameters.AddWithValue("@TecnicoAtribuido", chamado.TecnicoResponsavel.HasValue ? (object)chamado.TecnicoResponsavel.Value : DBNull.Value);

                        int novoId = (int)command.ExecuteScalar();
                        chamado.IdChamado = novoId;
                        return novoId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir chamado: {ex.Message}");
                return 0;
            }
        }

        public bool AtualizarChamado(Chamados chamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Implementación idéntica a ChamadosRepository
                    string sql = @"
                        UPDATE chamados 
                        SET titulo = @Titulo, categoria = @Categoria, descricao = @Descricao, 
                            prioridade = @Prioridade, Status = @Status, 
                            Tecnico_Atribuido = @TecnicoAtribuido, 
                            Data_Resolucao = @DataResolucao
                        WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                        command.Parameters.AddWithValue("@Titulo", string.IsNullOrWhiteSpace(chamado.Titulo) ? "Sem título" : chamado.Titulo);
                        command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                        command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                        command.Parameters.AddWithValue("@TecnicoAtribuido", chamado.TecnicoResponsavel.HasValue ? (object)chamado.TecnicoResponsavel.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@DataResolucao", chamado.DataResolucao.HasValue ? (object)chamado.DataResolucao.Value : DBNull.Value);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar chamado: {ex.Message}");
                return false;
            }
        }

        public bool RemoverChamado(int idChamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // ⚠️ ACTUALIZADO: Ya no busca ni elimina de la tabla antigua 'Contestacoes'
                    // El borrado en Historial_Contestacoes debe manejarse por CASCADE en la BD
                    // o lógica externa, pero aquí replicamos ChamadosRepository.Remover
                    string sql = "DELETE FROM chamados WHERE id_chamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover chamado: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region FUNCIONÁRIOS (Sincronizado con FuncionariosRepository)
        // La lógica aquí ya estaba mayormente correcta, solo aseguro el uso de vw_LoginUsuarios y la misma validación
        public bool ValidarLogin(string email, string senha)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM vw_LoginUsuarios WHERE Email = @Email AND Senha = @Senha AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Senha", senha);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao validar login: {ex.Message}");
                return false;
            }
        }

        public Funcionarios BuscarFuncionarioPorEmail(string email)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso,
                               DataCadastro, Ativo, Especializacao, Departamento, Cargo
                        FROM vw_LoginUsuarios
                        WHERE Email = @Email AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return CriarFuncionarioPorTipo(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário por email: {ex.Message}");
            }
            return null;
        }

        public Funcionarios BuscarFuncionarioPorId(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso,
                               DataCadastro, Ativo, Especializacao, Departamento, Cargo
                        FROM vw_LoginUsuarios
                        WHERE Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return CriarFuncionarioPorTipo(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário por ID: {ex.Message}");
            }
            return null;
        }

        public List<Funcionarios> ListarTodosFuncionarios()
        {
            var funcionarios = new List<Funcionarios>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso,
                               DataCadastro, Ativo, Especializacao, Departamento, Cargo
                        FROM vw_LoginUsuarios
                        ORDER BY Nome";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var func = CriarFuncionarioPorTipo(reader);
                            if (func != null) funcionarios.Add(func);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar funcionários: {ex.Message}");
            }
            return funcionarios;
        }

        public List<Tecnico> ListarTecnicos()
        {
            var tecnicos = new List<Tecnico>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso,
                               DataCadastro, Ativo, Especializacao
                        FROM vw_LoginUsuarios
                        WHERE NivelAcesso = 2 AND Ativo = 1
                        ORDER BY Nome";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tecnico = CriarFuncionarioPorTipo(reader) as Tecnico;
                            if (tecnico != null) tecnicos.Add(tecnico);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar técnicos: {ex.Message}");
            }
            return tecnicos;
        }

        public int InserirFuncionario(Funcionarios funcionario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlUsuario = @"
                                INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
                                VALUES (@Nome, @Cpf, @Senha, @Acess_codigo, @DataCadastro, @Ativo);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int novoId;
                            using (var cmd = new SqlCommand(sqlUsuario, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
                                cmd.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
                                cmd.Parameters.AddWithValue("@Senha", funcionario.Senha);
                                cmd.Parameters.AddWithValue("@Acess_codigo", funcionario.NivelAcesso);
                                cmd.Parameters.AddWithValue("@DataCadastro", funcionario.DataCadastro);
                                cmd.Parameters.AddWithValue("@Ativo", funcionario.Ativo ? 1 : 0);
                                novoId = (int)cmd.ExecuteScalar();
                            }

                            string sqlEmail = "INSERT INTO E_mail (Id_usuario, E_mail) VALUES (@Id, @Email)";
                            using (var cmd = new SqlCommand(sqlEmail, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", novoId);
                                cmd.Parameters.AddWithValue("@Email", funcionario.Email);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return novoId;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir funcionário: {ex.Message}");
                return 0;
            }
        }

        public bool AtualizarFuncionario(Funcionarios funcionario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlUsuario = @"
                                UPDATE Usuario 
                                SET nome = @Nome, Cpf = @Cpf, Acess_codigo = @Acess, Ativo = @Ativo
                                WHERE Id_usuario = @Id";

                            using (var cmd = new SqlCommand(sqlUsuario, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionario.Id);
                                cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
                                cmd.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
                                cmd.Parameters.AddWithValue("@Acess", funcionario.NivelAcesso);
                                cmd.Parameters.AddWithValue("@Ativo", funcionario.Ativo ? 1 : 0);
                                cmd.ExecuteNonQuery();
                            }

                            string sqlEmail = "UPDATE E_mail SET E_mail = @Email WHERE Id_usuario = @Id";
                            using (var cmd = new SqlCommand(sqlEmail, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionario.Id);
                                cmd.Parameters.AddWithValue("@Email", funcionario.Email);
                                cmd.ExecuteNonQuery();
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar funcionário: {ex.Message}");
                return false;
            }
        }

        public bool AlterarSenha(int funcionarioId, string novaSenha)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Usuario SET senha = @Senha WHERE Id_usuario = @Id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Senha", novaSenha);
                        command.Parameters.AddWithValue("@Id", funcionarioId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar senha: {ex.Message}");
                return false;
            }
        }

        public bool AlterarNivelAcesso(int funcionarioId, int novoNivel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Usuario SET Acess_codigo = @Nivel WHERE Id_usuario = @Id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nivel", novoNivel);
                        command.Parameters.AddWithValue("@Id", funcionarioId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar nível de acesso: {ex.Message}");
                return false;
            }
        }

        public bool RemoverFuncionario(int id) => ExcluirFuncionario(id);

        public bool ExcluirFuncionario(int funcionarioId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Validación igual a FuncionariosRepository
                    string sqlCheck = @"
                        SELECT COUNT(*) FROM chamados 
                        WHERE (Afetado = @Id OR Tecnico_Atribuido = @Id) 
                        AND Status NOT IN (3, 4, 5)";

                    using (var checkCommand = new SqlCommand(sqlCheck, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", funcionarioId);
                        int chamadosAtivos = (int)checkCommand.ExecuteScalar();
                        if (chamadosAtivos > 0)
                            throw new InvalidOperationException($"Funcionário possui {chamadosAtivos} chamado(s) ativo(s)");
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new SqlCommand("DELETE FROM E_mail WHERE Id_usuario = @Id", connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionarioId);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("DELETE FROM Usuario WHERE Id_usuario = @Id", connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionarioId);
                                cmd.ExecuteNonQuery();
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir funcionário: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region RELATÓRIOS (Sincronizado con RelatoriosRepository)
        // ⚠️ ACTUALIZADO: Antes usabas consultas simples. Ahora usamos las consultas detalladas de RelatoriosRepository.

        public DataTable ObterRelatorioGeral()
        {
            var dataTable = new DataTable();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Consulta idéntica a RelatoriosRepository.ObterRelatorioGeral
                    string sql = @"
                        SELECT 
                            c.id_chamado AS ID,
                            c.categoria AS Categoria,
                            c.descricao AS Descrição,
                            CASE c.prioridade
                                WHEN 1 THEN 'Baixa' WHEN 2 THEN 'Média' WHEN 3 THEN 'Alta' WHEN 4 THEN 'Crítica'
                            END AS Prioridade,
                            CASE c.Status
                                WHEN 1 THEN 'Aberto' WHEN 2 THEN 'Em Andamento' WHEN 3 THEN 'Resolvido' WHEN 4 THEN 'Fechado' WHEN 5 THEN 'Cancelado'
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório geral: {ex.Message}");
            }
            return dataTable;
        }

        public DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var dataTable = new DataTable();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Consulta idéntica a RelatoriosRepository.ObterRelatorioPorPeriodo
                    string sql = @"
                        SELECT 
                            c.id_chamado AS ID,
                            c.categoria AS Categoria,
                            c.descricao AS Descrição,
                            CASE c.prioridade
                                WHEN 1 THEN 'Baixa' WHEN 2 THEN 'Média' WHEN 3 THEN 'Alta' WHEN 4 THEN 'Crítica'
                            END AS Prioridade,
                            CASE c.Status
                                WHEN 1 THEN 'Aberto' WHEN 2 THEN 'Em Andamento' WHEN 3 THEN 'Resolvido' WHEN 4 THEN 'Fechado' WHEN 5 THEN 'Cancelado'
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório por período: {ex.Message}");
            }
            return dataTable;
        }

        public EstatisticasFuncionarios ObterEstatisticasFuncionarios()
        {
            var stats = new EstatisticasFuncionarios();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Consulta idéntica a RelatoriosRepository.ObterEstatisticasFuncionarios
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas de funcionários: {ex.Message}");
            }
            return stats;
        }
        #endregion

        #region MÉTODOS AUXILIARES

        private Funcionarios CriarFuncionarioPorTipo(SqlDataReader reader)
        {
            // Implementación exacta de FuncionariosRepository
            try
            {
                int nivelAcesso;
                var valorNivel = reader["NivelAcesso"];

                if (valorNivel is decimal decimalValue) nivelAcesso = Convert.ToInt32(decimalValue);
                else if (valorNivel is int intValue) nivelAcesso = intValue;
                else nivelAcesso = Convert.ToInt32(valorNivel);

                Funcionarios funcionario;
                if (nivelAcesso == 1) funcionario = new Funcionario();
                else if (nivelAcesso == 2) funcionario = new Tecnico();
                else if (nivelAcesso == 3) funcionario = new ADM();
                else throw new ArgumentException($"Nível de acesso inválido: {nivelAcesso}");

                funcionario.Id = Convert.ToInt32(reader["Id"]);
                funcionario.Nome = reader["Nome"].ToString();
                funcionario.Cpf = reader["Cpf"].ToString();
                funcionario.Email = reader["Email"].ToString();
                funcionario.Senha = reader["Senha"].ToString();
                funcionario.NivelAcesso = nivelAcesso;
                funcionario.DataCadastro = (DateTime)reader["DataCadastro"];
                funcionario.Ativo = (bool)reader["Ativo"];

                // Campos opcionales seguros
                try { if (!reader.IsDBNull(reader.GetOrdinal("TipoFuncionario"))) funcionario.TipoFuncionario = reader["TipoFuncionario"].ToString(); } catch { }

                if (funcionario is Tecnico tecnico)
                {
                    try { if (!reader.IsDBNull(reader.GetOrdinal("Especializacao"))) tecnico.Especializacao = reader["Especializacao"].ToString(); } catch { }
                }

                if (funcionario is Funcionario func)
                {
                    try { if (!reader.IsDBNull(reader.GetOrdinal("Departamento"))) func.Departamento = reader["Departamento"].ToString(); } catch { }
                    try { if (!reader.IsDBNull(reader.GetOrdinal("Cargo"))) func.Cargo = reader["Cargo"].ToString(); } catch { }
                }

                return funcionario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar funcionário: {ex.Message}");
                return null;
            }
        }

        private Chamados CriarChamadoFromReader(SqlDataReader reader, SqlConnection connection)
        {
            try
            {
                string tituloRaw = reader["titulo"] != DBNull.Value ? reader["titulo"].ToString().Trim() : "";
                string titulo = LimparTexto(tituloRaw);
                if (string.IsNullOrWhiteSpace(titulo)) titulo = "Sem título";

                string descricaoRaw = reader["descricao"] != DBNull.Value ? reader["descricao"].ToString().Trim() : "";
                string descricao = LimparTexto(descricaoRaw);

                var chamado = new Chamados
                {
                    IdChamado = (int)reader["id_chamado"],
                    Titulo = titulo,
                    Categoria = reader["categoria"].ToString(),
                    Prioridade = (int)reader["prioridade"],
                    Descricao = descricao,
                    Afetado = (int)reader["Afetado"],
                    DataChamado = (DateTime)reader["Data_Registro"],
                    Status = (StatusChamado)(int)reader["Status"],
                    TecnicoResponsavel = reader.IsDBNull(reader.GetOrdinal("Tecnico_Atribuido")) ? (int?)null : (int)reader["Tecnico_Atribuido"],
                    DataResolucao = reader.IsDBNull(reader.GetOrdinal("Data_Resolucao")) ? (DateTime?)null : (DateTime)reader["Data_Resolucao"]
                };

                chamado.Contestacoes = BuscarContestacaoTexto(chamado.IdChamado, connection);

                return chamado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em CriarChamadoFromReader: {ex.Message}");
                return null;
            }
        }

        private string LimparTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";
            string[] prefixos = { "TÍTULO:", "TITULO:", "TÍTULO :", "TITULO :", "DESCRIÇÃO:", "DESCRICAO:", "DESCRIÇÃO :", "DESCRICAO :" };
            string limpo = texto.Trim();
            foreach (var prefixo in prefixos)
            {
                if (limpo.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase))
                {
                    limpo = limpo.Substring(prefixo.Length).Trim();
                    break;
                }
            }
            return limpo.TrimStart('\r', '\n', ' ', '\t');
        }

        private string BuscarContestacaoTexto(int idChamado, SqlConnection connection)
        {
            try
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
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}