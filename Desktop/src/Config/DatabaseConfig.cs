using System;
using System.Configuration;

namespace SistemaChamados.Config
{
    // Classe para gerenciar configurações do banco de dados
    public static class DatabaseConfig
    {


        // String de conexão padrão
        private static string _connectionString;

        // Propriedade para obter a string de conexão
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = GetConnectionString();
                }
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        // Método para obter string de conexão do app.config ou usar padrão
        private static string GetConnectionString()
        {
            try
            {
                // Tentar obter do app.config primeiro
                string configConnectionString = ConfigurationManager.ConnectionStrings["Suporte_Tecnico"]?.ConnectionString;
                
                if (!string.IsNullOrEmpty(configConnectionString))
                {
                    return configConnectionString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler configuração: {ex.Message}");
            }

            // Retornar string de conexão padrão se não encontrar no config
            return GetDefaultConnectionString();
        }

        // String de conexão padrão para desenvolvimento
        private static string GetDefaultConnectionString()
        {
            return "Server=localhost\\SQLEXPRESS;Database=Suporte_Tecnico;Integrated Security=true;Connection Timeout=30;TrustServerCertificate=True;";
        }

        // Método para configurar conexão com SQL Server usando autenticação do Windows
        public static string GetWindowsAuthConnectionString(string server, string database)
        {
            return $"Server={server};Database={database};Integrated Security=true;Connection Timeout=30;";
        }

        // Método para configurar conexão com SQL Server usando usuário e senha
        public static string GetSqlAuthConnectionString(string server, string database, string username, string password)
        {
            return $"Server={server};Database={database};User Id={username};Password={password};Connection Timeout=30;";
        }

        // Método para testar conexão
        public static bool TestarConexao(string connectionString = null)
        {
            try
            {
                string connStr = connectionString ?? ConnectionString;
                var database = new SistemaChamados.Data.SqlServerConnection(connStr);
                return database.TestarConexao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao testar conexão: {ex.Message}");
                return false;
            }
        }

        // Configurações do sistema
        public static class SystemSettings
        {
            public static string ApplicationName => "Sistema de Chamados";
            public static string Version => "1.0.0";
            public static int SessionTimeoutMinutes => 30;
            public static int MaxLoginAttempts => 3;
            public static bool EnableAuditLog => true;
            public static string LogPath => @"C:\Logs\SistemaChamados\";
        }

        // Configurações de email (se necessário para notificações)
        public static class EmailSettings
        {
            public static string SmtpServer => ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
            public static int SmtpPort => int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            public static string EmailFrom => ConfigurationManager.AppSettings["EmailFrom"] ?? "sistema@empresa.com";
            public static string EmailPassword => ConfigurationManager.AppSettings["EmailPassword"] ?? "";
            public static bool EnableSsl => bool.Parse(ConfigurationManager.AppSettings["EnableSsl"] ?? "true");
        }
    }
}
