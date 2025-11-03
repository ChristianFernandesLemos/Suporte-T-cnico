using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Models;

namespace SistemaChamados.Data.Repositories
{
    /// <summary>
    /// 📦 REPOSITÓRIO: Operações CRUD para Funcionários
    /// - Login e autenticação
    /// - Gerenciamento de usuários
    /// </summary>
    public class FuncionariosRepository
    {
        #region 🔐 LOGIN E AUTENTICAÇÃO

        /// <summary>
        /// Valida login com email e senha (hash)
        /// </summary>
        public bool ValidarLogin(string email, string senhaHash)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = @"
                        SELECT COUNT(*) 
                        FROM vw_LoginUsuarios 
                        WHERE Email = @Email AND Senha = @Senha AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Senha", senhaHash);

                        int count = (int)command.ExecuteScalar();

                        System.Diagnostics.Debug.WriteLine($"[LOGIN] {email} - {(count > 0 ? "Sucesso" : "Falhou")}");

                        return count > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN ERRO] {ex.Message}");
                Console.WriteLine($"Erro ao validar login: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Busca funcionário por email (após login bem-sucedido)
        /// </summary>
        public Funcionarios BuscarPorEmail(string email)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
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
                    return null;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BUSCAR ERRO] {ex.Message}");
                Console.WriteLine($"Erro ao buscar funcionário por email: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region 🔍 CONSULTAR / BUSCAR

        /// <summary>
        /// Busca funcionário por ID
        /// </summary>
        public Funcionarios BuscarPorId(int id)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
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
                    return null;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário por ID: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lista todos os funcionários
        /// </summary>
        public List<Funcionarios> ListarTodos()
        {
            var funcionarios = new List<Funcionarios>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
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
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar funcionários: {ex.Message}");
            }
            return funcionarios;
        }

        /// <summary>
        /// Lista apenas técnicos
        /// </summary>
        public List<Tecnico> ListarTecnicos()
        {
            var tecnicos = new List<Tecnico>();
            try
            {
                DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
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
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar técnicos: {ex.Message}");
            }
            return tecnicos;
        }

        #endregion

        #region ➕ INSERIR

        /// <summary>
        /// Insere novo funcionário
        /// </summary>
        public int Inserir(Funcionarios funcionario)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Inserir em Usuario
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

                            // 2. Inserir em E_mail
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
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir funcionário: {ex.Message}");
                return 0;
            }
        }

        #endregion

        #region 🔄 ATUALIZAR

        /// <summary>
        /// Atualiza dados do funcionário
        /// </summary>
        public bool Atualizar(Funcionarios funcionario)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
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
                Console.WriteLine($"Erro ao atualizar funcionário: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Altera senha do funcionário (já deve vir em hash)
        /// </summary>
        public bool AlterarSenha(int funcionarioId, string novaSenhaHash)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = "UPDATE Usuario SET senha = @Senha WHERE Id_usuario = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Senha", novaSenhaHash);
                        command.Parameters.AddWithValue("@Id", funcionarioId);
                        return command.ExecuteNonQuery() > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar senha: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Altera nível de acesso (1=Funcionário, 2=Técnico, 3=Admin)
        /// </summary>
        public bool AlterarNivelAcesso(int funcionarioId, int novoNivel)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
                    string sql = "UPDATE Usuario SET Acess_codigo = @Nivel WHERE Id_usuario = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nivel", novoNivel);
                        command.Parameters.AddWithValue("@Id", funcionarioId);
                        return command.ExecuteNonQuery() > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar nível de acesso: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region ❌ EXCLUIR

        /// <summary>
        /// Exclui funcionário (com validação de chamados ativos)
        /// </summary>
        public bool Excluir(int funcionarioId)
        {
            try
            {
                return DatabaseConnectionManager.ExecuteWithConnection(connection =>
                {
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
                            // Eliminar email
                            string sqlEmail = "DELETE FROM E_mail WHERE Id_usuario = @Id";
                            using (var cmd = new SqlCommand(sqlEmail, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", funcionarioId);
                                cmd.ExecuteNonQuery();
                            }

                            // Eliminar usuario
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
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir funcionário: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 🛠️ MÉTODOS AUXILIARES

        /// <summary>
        /// Cria instância correta (Funcionario, Tecnico ou ADM) baseado no NivelAcesso
        /// </summary>
        private Funcionarios CriarFuncionarioPorTipo(SqlDataReader reader)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("--- CRIAR FUNCIONÁRIO POR TIPO ---");

                // ✅ CORREÇÃO: Converter DECIMAL para INT corretamente
                int nivelAcesso;

                // Verificar o tipo real da coluna
                var tipoColuna = reader.GetFieldType(reader.GetOrdinal("NivelAcesso"));
                System.Diagnostics.Debug.WriteLine($"Tipo da coluna NivelAcesso: {tipoColuna.Name}");

                // Fazer conversão segura
                var valorNivel = reader["NivelAcesso"];
                if (valorNivel is decimal decimalValue)
                {
                    nivelAcesso = Convert.ToInt32(decimalValue);
                }
                else if (valorNivel is int intValue)
                {
                    nivelAcesso = intValue;
                }
                else
                {
                    // Tentar conversão genérica
                    nivelAcesso = Convert.ToInt32(valorNivel);
                }

                System.Diagnostics.Debug.WriteLine($"Nível de Acesso convertido: {nivelAcesso}");

                // Criar instância baseada no nível
                Funcionarios funcionario;
                if (nivelAcesso == 1)
                {
                    funcionario = new Funcionario();
                    System.Diagnostics.Debug.WriteLine("Criando: Funcionario (comum)");
                }
                else if (nivelAcesso == 2)
                {
                    funcionario = new Tecnico();
                    System.Diagnostics.Debug.WriteLine("Criando: Tecnico");
                }
                else if (nivelAcesso == 3)
                {
                    funcionario = new ADM();
                    System.Diagnostics.Debug.WriteLine("Criando: ADM (Administrador)");
                }
                else
                {
                    throw new ArgumentException($"Nível de acesso inválido: {nivelAcesso}");
                }

                // Propriedades básicas
                funcionario.Id = Convert.ToInt32(reader["Id"]);
                funcionario.Nome = reader["Nome"].ToString();
                funcionario.Cpf = reader["Cpf"].ToString();
                funcionario.Email = reader["Email"].ToString();
                funcionario.Senha = reader["Senha"].ToString();
                funcionario.NivelAcesso = nivelAcesso;
                funcionario.DataCadastro = (DateTime)reader["DataCadastro"];
                funcionario.Ativo = (bool)reader["Ativo"];

                System.Diagnostics.Debug.WriteLine($"✅ Funcionário criado:");
                System.Diagnostics.Debug.WriteLine($"   Id: {funcionario.Id}");
                System.Diagnostics.Debug.WriteLine($"   Nome: {funcionario.Nome}");
                System.Diagnostics.Debug.WriteLine($"   Email: {funcionario.Email}");
                System.Diagnostics.Debug.WriteLine($"   NivelAcesso: {funcionario.NivelAcesso}");

                // Campos opcionais
                try
                {
                    int ordTipo = reader.GetOrdinal("TipoFuncionario");
                    if (!reader.IsDBNull(ordTipo))
                    {
                        funcionario.TipoFuncionario = reader["TipoFuncionario"].ToString();
                    }
                }
                catch { }

                // Especialização para Técnico
                if (funcionario is Tecnico tecnico)
                {
                    try
                    {
                        int ordEspec = reader.GetOrdinal("Especializacao");
                        if (!reader.IsDBNull(ordEspec))
                        {
                            tecnico.Especializacao = reader["Especializacao"].ToString();
                        }
                    }
                    catch { }
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
                        }
                    }
                    catch { }

                    try
                    {
                        int ordCargo = reader.GetOrdinal("Cargo");
                        if (!reader.IsDBNull(ordCargo))
                        {
                            func.Cargo = reader["Cargo"].ToString();
                        }
                    }
                    catch { }
                }

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

        #endregion
    }
}