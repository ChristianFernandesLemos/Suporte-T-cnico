using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaChamados.Config
{
    /// <summary>
    /// Gerenciador de conexão com o banco de dados
    /// Centraliza a leitura da connection string e testes de conexão
    /// </summary>
    public static class DatabaseConnectionManager
    {
        private static string _connectionString;

        /// <summary>
        /// Obtém a connection string do App.config
        /// </summary>
        public static string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;

            try
            {
                // MÉTODO 1: Tentar ler de connectionStrings
                var connString = ConfigurationManager.ConnectionStrings["Suporte_Tecnico"];
                if (connString != null && !string.IsNullOrEmpty(connString.ConnectionString))
                {
                    _connectionString = connString.ConnectionString;
                    System.Diagnostics.Debug.WriteLine($"✅ Connection String obtida de connectionStrings: {_connectionString}");
                    return _connectionString;
                }

                // MÉTODO 2: Tentar ler de applicationSettings
                _connectionString = Properties.Settings.Default.ConnectionString;
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Connection String obtida de Settings: {_connectionString}");
                    return _connectionString;
                }

                // MÉTODO 3: Fallback - Connection string padrão
                _connectionString = "Server=localhost\\SQLEXPRESS;Database=Suporte_Tecnico;Integrated Security=true;Connection Timeout=30;Encrypt=false;TrustServerCertificate=true;";
                System.Diagnostics.Debug.WriteLine($"⚠️ Usando connection string padrão (fallback): {_connectionString}");
                return _connectionString;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao obter connection string: {ex.Message}");

                // Fallback final
                _connectionString = "Server=localhost\\SQLEXPRESS;Database=Suporte_Tecnico;Integrated Security=true;Connection Timeout=30;Encrypt=false;TrustServerCertificate=true;";
                return _connectionString;
            }
        }

        #region ➕ NOVOS MÉTODOS PARA REPOSITÓRIOS

        /// <summary>
        /// Cria e retorna uma nova conexão SQL (NÃO abre automaticamente)
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            string connectionString = GetConnectionString();
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Cria, abre e retorna uma conexão SQL (pronta para usar)
        /// ⚠️ LEMBRE-SE de fazer Dispose() ou usar using()
        /// </summary>
        public static SqlConnection OpenConnection()
        {
            var connection = CreateConnection();
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Executa uma ação com uma conexão aberta (gerencia Dispose automaticamente)
        /// Exemplo: DatabaseConnectionManager.ExecuteWithConnection(conn => { ... });
        /// </summary>
        public static void ExecuteWithConnection(Action<SqlConnection> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using (var connection = OpenConnection())
            {
                action(connection);
            }
        }

        /// <summary>
        /// Executa uma função com uma conexão aberta e retorna resultado
        /// Exemplo: var result = DatabaseConnectionManager.ExecuteWithConnection(conn => { return ...; });
        /// </summary>
        public static T ExecuteWithConnection<T>(Func<SqlConnection, T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            using (var connection = OpenConnection())
            {
                return func(connection);
            }
        }

        #endregion

        /// <summary>
        /// Testa a conexão com o banco de dados
        /// </summary>
        public static bool TestarConexao(bool mostrarMensagem = true)
        {
            string connectionString = GetConnectionString();

            System.Diagnostics.Debug.WriteLine("=".PadRight(60, '='));
            System.Diagnostics.Debug.WriteLine("TESTE DE CONEXÃO");
            System.Diagnostics.Debug.WriteLine($"Connection String: {connectionString}");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("Tentando abrir conexão...");
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine($"Estado da conexão: {connection.State}");
                    System.Diagnostics.Debug.WriteLine($"Servidor: {connection.DataSource}");
                    System.Diagnostics.Debug.WriteLine($"Database: {connection.Database}");
                    System.Diagnostics.Debug.WriteLine($"Versão do SQL Server: {connection.ServerVersion}");

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        System.Diagnostics.Debug.WriteLine("✅ CONEXÃO BEM-SUCEDIDA!");
                        System.Diagnostics.Debug.WriteLine("=".PadRight(60, '='));

                        if (mostrarMensagem)
                        {
                            MessageBox.Show(
                                $"✅ Conexão estabelecida com sucesso!\n\n" +
                                $"Servidor: {connection.DataSource}\n" +
                                $"Database: {connection.Database}\n" +
                                $"Versão: {connection.ServerVersion}",
                                "Teste de Conexão",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }

                        return true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO SQL: {sqlEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Número do erro: {sqlEx.Number}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {sqlEx.StackTrace}");
                System.Diagnostics.Debug.WriteLine("=".PadRight(60, '='));

                if (mostrarMensagem)
                {
                    string mensagemErro = $"❌ Erro ao conectar ao SQL Server:\n\n{sqlEx.Message}\n\n";

                    switch (sqlEx.Number)
                    {
                        case 2:
                        case 53:
                            mensagemErro += "🔧 Possíveis soluções:\n" +
                                          "1. Verifique se o SQL Server está rodando\n" +
                                          "2. Verifique o nome da instância (\\SQLEXPRESS)\n" +
                                          "3. Habilite TCP/IP no SQL Server Configuration Manager";
                            break;
                        case 4060:
                            mensagemErro += "🔧 Solução:\n" +
                                          "O banco de dados 'Suporte_Tecnico' não existe.\n" +
                                          "Execute o script de criação do banco.";
                            break;
                        case 18456:
                            mensagemErro += "🔧 Solução:\n" +
                                          "Erro de autenticação. Verifique as credenciais.";
                            break;
                    }

                    MessageBox.Show(mensagemErro, "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO GERAL: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("=".PadRight(60, '='));

                if (mostrarMensagem)
                {
                    MessageBox.Show(
                        $"❌ Erro ao testar conexão:\n\n{ex.Message}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Verifica se o banco de dados existe
        /// </summary>
        public static bool VerificarBancoDados()
        {
            try
            {
                string connectionString = GetConnectionString()
                    .Replace("Database=Suporte_Tecnico", "Database=master");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = 'Suporte_Tecnico'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar banco de dados: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtém informações sobre o servidor SQL
        /// </summary>
        public static string ObterInfoServidor()
        {
            try
            {
                string connectionString = GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return $"Servidor: {connection.DataSource}\n" +
                           $"Database: {connection.Database}\n" +
                           $"Versão: {connection.ServerVersion}\n" +
                           $"Estado: {connection.State}";
                }
            }
            catch (Exception ex)
            {
                return $"Erro ao obter informações: {ex.Message}";
            }
        }
    }
}