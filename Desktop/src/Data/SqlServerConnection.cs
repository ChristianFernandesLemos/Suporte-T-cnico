using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Controllers;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Data
{
    /// <summary>
    /// ✅ MIGRADO PARA: Suporte_Tecnico
    /// - Tabla Usuario (antes Funcionarios)
    /// - Tabla chamados (antes Chamados)
    /// - Usa VIEWs: vw_LoginUsuarios, vw_ChamadosCompletos
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

        #region CHAMADOS - BUSCAR/LISTAR
        public Chamados BuscarChamadoPorId(int idChamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
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
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
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

                            // Insertar contestación si existe
                            if (!string.IsNullOrEmpty(chamado.Contestacoes))
                            {
                                // Insertar en Contestacoes y obtener el ID
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

                                // Actualizar chamado con el código de la contestación
                                string sqlUpdateChamado = "UPDATE chamados SET Contestacoes_Codigo = @Codigo WHERE id_chamado = @IdChamado";
                                using (var cmd = new SqlCommand(sqlUpdateChamado, connection, transaction))
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
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
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

                            // Actualizar o crear contestación
                            if (!string.IsNullOrEmpty(chamado.Contestacoes))
                            {
                                // Verificar si ya tiene contestación
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
                                    // Actualizar contestación existente
                                    string sqlUpdateCont = @"
                                        UPDATE Contestacoes 
                                        SET Justificativa = @Justificativa, DataContestacao = @DataContestacao
                                        WHERE Codigo = @Codigo";

                                    using (var cmd = new SqlCommand(sqlUpdateCont, connection, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@Codigo", codigoExistente.Value);
                                        cmd.Parameters.AddWithValue("@Justificativa", chamado.Contestacoes);
                                        cmd.Parameters.AddWithValue("@DataContestacao", DateTime.Now);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Crear nueva contestación
                                    string sqlInsertCont = @"
                                        INSERT INTO Contestacoes (Justificativa, DataContestacao)
                                        VALUES (@Justificativa, @DataContestacao);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                                    int nuevoCodigo;
                                    using (var cmd = new SqlCommand(sqlInsertCont, connection, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@Justificativa", chamado.Contestacoes);
                                        cmd.Parameters.AddWithValue("@DataContestacao", DateTime.Now);
                                        nuevoCodigo = (int)cmd.ExecuteScalar();
                                    }

                                    // Actualizar chamado con el nuevo código
                                    string sqlUpdateChamado = "UPDATE chamados SET Contestacoes_Codigo = @Codigo WHERE id_chamado = @IdChamado";
                                    using (var cmd = new SqlCommand(sqlUpdateChamado, connection, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@Codigo", nuevoCodigo);
                                        cmd.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                                        cmd.ExecuteNonQuery();
                                    }
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
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Obtener código de contestación
                            string sqlGetCodigo = "SELECT Contestacoes_Codigo FROM chamados WHERE id_chamado = @IdChamado";
                            int? codigoCont = null;
                            
                            using (var cmd = new SqlCommand(sqlGetCodigo, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@IdChamado", idChamado);
                                var result = cmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    codigoCont = (int)result;
                            }

                            // Remover chamado
                            string sql = "DELETE FROM chamados WHERE id_chamado = @IdChamado";
                            using (var command = new SqlCommand(sql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@IdChamado", idChamado);
                                command.ExecuteNonQuery();
                            }

                            // Remover contestación si existe
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover chamado: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region FUNCIONÁRIOS - LOGIN/BUSCAR
        public bool ValidarLogin(string email, string senha)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                SELECT COUNT(*) 
                FROM vw_LoginUsuarios 
                WHERE Email = @Email AND Senha = @Senha AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Senha", senha);

                        int count = (int)command.ExecuteScalar();

                        // Log solo en Debug Output (no visible para usuarios)
                        System.Diagnostics.Debug.WriteLine($"[LOGIN] {email} - {(count > 0 ? "Sucesso" : "Falhou")}");

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN ERRO] {ex.Message}");
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
                       DataCadastro, Ativo
                FROM vw_LoginUsuarios
                WHERE Email = @Email AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var funcionario = CriarFuncionarioPorTipo(reader);

                                if (funcionario != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[USUARIO] {funcionario.Nome} ({funcionario.GetType().Name})");
                                }

                                return funcionario;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BUSCAR ERRO] {ex.Message}");
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
                            if (func != null)
                                funcionarios.Add(func);
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
                            var tecnico = (Tecnico)CriarFuncionarioPorTipo(reader);
                            if (tecnico != null)
                                tecnicos.Add(tecnico);
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
        #endregion

        #region FUNCIONÁRIOS - INSERIR/ATUALIZAR/EXCLUIR
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
                            // 1. Insertar en Usuario (SOLO 2 TABLAS)
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

                            // 2. Insertar en E_mail
                            string sqlEmail = "INSERT INTO E_mail (Id_usuario, E_mail) VALUES (@Id, @Email)";
                            using (var cmd = new SqlCommand(sqlEmail, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", novoId);
                                cmd.Parameters.AddWithValue("@Email", funcionario.Email);
                                cmd.ExecuteNonQuery();
                            }

                            // ⚠️ NO insertar en Tecnico/Funcionario porque NO EXISTEN
                            // El tipo se determina solo por Acess_codigo

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
                            // 1. Atualizar Usuario
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

                            // 2. Atualizar E_mail
                            string sqlEmail = "UPDATE E_mail SET E_mail = @Email WHERE Id_usuario = @Id";
                            using (var cmd = new SqlCommand(sqlEmail, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionario.Id);
                                cmd.Parameters.AddWithValue("@Email", funcionario.Email);
                                cmd.ExecuteNonQuery();
                            }

                            // ⚠️ NO actualizar Tecnico/Funcionario porque NO EXISTEN

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

                    // Verificar chamados ativos
                    string sqlCheck = @"
                SELECT COUNT(*) FROM chamados 
                WHERE (Afetado = @Id OR Tecnico_Atribuido = @Id) 
                AND Status NOT IN (3, 4, 5)"; // Resolvido=3, Fechado=4, Cancelado=5

                    using (var checkCommand = new SqlCommand(sqlCheck, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", funcionarioId);
                        int chamadosAtivos = (int)checkCommand.ExecuteScalar();

                        if (chamadosAtivos > 0)
                        {
                            throw new InvalidOperationException($"Funcionário possui {chamadosAtivos} chamado(s) ativo(s)");
                        }
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Eliminar en orden (solo 2 tablas)
                            string sqlEmail = "DELETE FROM E_mail WHERE Id_usuario = @Id";
                            using (var cmd = new SqlCommand(sqlEmail, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionarioId);
                                cmd.ExecuteNonQuery();
                            }

                            string sqlUsuario = "DELETE FROM Usuario WHERE Id_usuario = @Id";
                            using (var cmd = new SqlCommand(sqlUsuario, connection, transaction))
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

        #region RELATÓRIOS
        public DataTable ObterRelatorioGeral()
        {
            var dataTable = new DataTable();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT id_chamado, categoria, descricao, prioridade, Status,
                               Data_Registro, Data_Resolucao,
                               Solicitante_Nome as Solicitante, Tecnico_Nome as Tecnico
                        FROM vw_ChamadosCompletos
                        ORDER BY Data_Registro DESC";

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
                    string sql = @"
                        SELECT id_chamado, categoria, descricao, prioridade, Status,
                               Data_Registro, Data_Resolucao,
                               Solicitante_Nome as Solicitante, Tecnico_Nome as Tecnico
                        FROM vw_ChamadosCompletos
                        WHERE Data_Registro BETWEEN @DataInicio AND @DataFim
                        ORDER BY Data_Registro DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@DataInicio", dataInicio);
                        command.Parameters.AddWithValue("@DataFim", dataFim.AddDays(1));

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
            try
            {
                System.Diagnostics.Debug.WriteLine("--- CRIAR FUNCIONÁRIO POR TIPO ---");

                // Verificar se existe coluna NivelAcesso
                int colIndex = -1;
                try
                {
                    colIndex = reader.GetOrdinal("NivelAcesso");
                    System.Diagnostics.Debug.WriteLine($"✅ Coluna NivelAcesso encontrada no índice {colIndex}");
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("❌ Coluna NivelAcesso não encontrada!");

                    // Listar todas as colunas disponíveis
                    System.Diagnostics.Debug.WriteLine("Colunas disponíveis na VIEW:");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        System.Diagnostics.Debug.WriteLine($"   [{i}] {reader.GetName(i)} ({reader.GetFieldType(i).Name})");
                    }

                    throw new Exception("Coluna NivelAcesso não existe na VIEW!");
                }

                int nivelAcesso = (int)reader["NivelAcesso"];
                System.Diagnostics.Debug.WriteLine($"Nível de Acesso: {nivelAcesso}");

                Funcionarios funcionario;
                if (nivelAcesso == 1)
                {
                    funcionario = new Funcionario();  // Funcionário comum
                    System.Diagnostics.Debug.WriteLine("Criando: Funcionario (comum)");
                }
                else if (nivelAcesso == 2)
                {
                    funcionario = new Tecnico();      // Técnico
                    System.Diagnostics.Debug.WriteLine("Criando: Tecnico");
                }
                else if (nivelAcesso == 3)
                {
                    funcionario = new ADM();          // Administrador
                    System.Diagnostics.Debug.WriteLine("Criando: ADM (Administrador)");
                }
                else
                {
                    throw new ArgumentException($"Nível de acesso inválido: {nivelAcesso}");
                }

                // Propriedades básicas
                funcionario.Id = (int)reader["Id"];
                funcionario.Nome = reader["Nome"].ToString();
                funcionario.Cpf = reader["Cpf"].ToString();
                funcionario.Email = reader["Email"].ToString();
                funcionario.Senha = reader["Senha"].ToString();
                funcionario.NivelAcesso = nivelAcesso;
                funcionario.DataCadastro = (DateTime)reader["DataCadastro"];
                funcionario.Ativo = (bool)reader["Ativo"];

                System.Diagnostics.Debug.WriteLine($"Dados preenchidos:");
                System.Diagnostics.Debug.WriteLine($"   Id: {funcionario.Id}");
                System.Diagnostics.Debug.WriteLine($"   Nome: {funcionario.Nome}");
                System.Diagnostics.Debug.WriteLine($"   Email: {funcionario.Email}");
                System.Diagnostics.Debug.WriteLine($"   Cpf: {funcionario.Cpf}");
                System.Diagnostics.Debug.WriteLine($"   NivelAcesso: {funcionario.NivelAcesso}");
                System.Diagnostics.Debug.WriteLine($"   Ativo: {funcionario.Ativo}");

                // Campos opcionais que podem não existir na VIEW
                try
                {
                    int ordTipo = reader.GetOrdinal("TipoFuncionario");
                    if (!reader.IsDBNull(ordTipo))
                    {
                        funcionario.TipoFuncionario = reader["TipoFuncionario"].ToString();
                        System.Diagnostics.Debug.WriteLine($"   TipoFuncionario: {funcionario.TipoFuncionario}");
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("   TipoFuncionario: (coluna não existe na view)");
                }

                // Especialização para Técnico
                if (funcionario is Tecnico tecnico)
                {
                    try
                    {
                        int ordEspec = reader.GetOrdinal("Especializacao");
                        if (!reader.IsDBNull(ordEspec))
                        {
                            tecnico.Especializacao = reader["Especializacao"].ToString();
                            System.Diagnostics.Debug.WriteLine($"   Especializacao: {tecnico.Especializacao}");
                        }
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("   Especializacao: (coluna não existe)");
                    }
                }

                // Departamento e Cargo para Funcionário comum
                if (funcionario is Funcionario func)
                {
                    try
                    {
                        int ordDept = reader.GetOrdinal("Departamento");
                        if (!reader.IsDBNull(ordDept))
                        {
                            func.Departamento = reader["Departamento"].ToString();
                            System.Diagnostics.Debug.WriteLine($"   Departamento: {func.Departamento}");
                        }
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("   Departamento: (coluna não existe)");
                    }

                    try
                    {
                        int ordCargo = reader.GetOrdinal("Cargo");
                        if (!reader.IsDBNull(ordCargo))
                        {
                            func.Cargo = reader["Cargo"].ToString();
                            System.Diagnostics.Debug.WriteLine($"   Cargo: {func.Cargo}");
                        }
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("   Cargo: (coluna não existe)");
                    }
                }

                System.Diagnostics.Debug.WriteLine("✅ Funcionário criado com sucesso!");
                return funcionario;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO ao criar funcionário: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                Console.WriteLine($"Erro ao criar funcionário: {ex.Message}");
                return null;
            }
        }

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

            // Buscar contestación usando Contestacoes_Codigo
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT c.Justificativa 
                        FROM chamados ch
                        INNER JOIN Contestacoes c ON ch.Contestacoes_Codigo = c.Codigo
                        WHERE ch.id_chamado = @IdChamado";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            chamado.Contestacoes = result.ToString();
                        }
                    }
                }
            }
            catch
            {
                chamado.Contestacoes = null;
            }

            return chamado;
        }
        #endregion
    }
}


