using SistemaChamados.Config;
using SistemaChamados.Data;
using SistemaChamados.Forms;
using System;
using System.Windows.Forms;


namespace SistemaChamados
{
    // Classe principal do programa
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para a aplicação.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                var connectionString = DatabaseConfig.ConnectionString;
                var database = new SqlServerConnection(connectionString);

                if (database.TestarConexao())
                {
                    Console.WriteLine("Conexão OK!");

                    // Testar login
                    var funcionario = database.BuscarFuncionarioPorEmail("admin@sistema.com");
                    if (funcionario != null)
                    {
                        Console.WriteLine($"Usuário encontrado: {funcionario.Nome} - {funcionario.TipoFuncionario}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }



            try
            {
                // Configurar aplicação Windows Forms
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
      

                // Verificar conexão com banco de dados
                if (!VerificarConexaoBancoDados())
                {
                    MessageBox.Show(
                        "Não foi possível conectar ao banco de dados.\n\n" +
                        "Verifique se:\n" +
                        "1. O SQL Server está rodando\n" +
                        "2. O banco 'SistemaChamados' existe\n" +
                        "3. As credenciais de acesso estão corretas\n" +
                        "4. A string de conexão no App.config está configurada corretamente",
                        "Erro de Conexão",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Exibir splash screen (opcional)
                ExibirSplashScreen();

                // Iniciar aplicação com tela de login
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                // Log do erro
                LogError(ex);

                // Exibir erro para o usuário
                MessageBox.Show(
                    $"Erro fatal na aplicação:\n\n{ex.Message}\n\nA aplicação será encerrada.",
                    "Erro Fatal",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
             
                );
            }
        }

        /// <summary>
        /// Verifica se é possível conectar ao banco de dados
        /// </summary>
        /// <returns>True se a conexão for bem-sucedida</returns>
        private static bool VerificarConexaoBancoDados()
        {
            try
            {
                return DatabaseConfig.TestarConexao();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Exibe uma tela de splash (opcional)
        /// </summary>
        private static void ExibirSplashScreen()
        {
            try
            {
                // Aqui você pode criar uma tela de splash personalizada
                // Por enquanto, apenas um delay simulado
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Registra erros em log
        /// </summary>
        /// <param name="ex">Exceção a ser registrada</param>
        private static void LogError(Exception ex)
        {
            try
            {
                string logPath = DatabaseConfig.SystemSettings.LogPath;
                
                // Criar diretório se não existir
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }

                string logFile = System.IO.Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}\nStackTrace: {ex.StackTrace}\n\n";

                System.IO.File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                // Se não conseguir escrever no log, apenas ignore
                // para evitar loops de erro
            }
        }

        /// <summary>
        /// Registra informações em log
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        public static void LogInfo(string message)
        {
            try
            {
                string logPath = DatabaseConfig.SystemSettings.LogPath;
                
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }

                string logFile = System.IO.Path.Combine(logPath, $"info_{DateTime.Now:yyyyMMdd}.log");
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}\n";

                System.IO.File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                // Ignorar erros de log
            }
        }
    }

    internal class MainAppContext : Form
    {
    }

    /// <summary>
    /// Classe para informações da aplicação
    /// </summary>
    public static class ApplicationInfo
    {
        public static string Name => DatabaseConfig.SystemSettings.ApplicationName;
        public static string Version => DatabaseConfig.SystemSettings.Version;
        public static string CompanyName => "Sua Empresa";
        public static string Copyright => $"© {DateTime.Now.Year} {CompanyName}. Todos os direitos reservados.";
        public static string Description => "Sistema de gerenciamento de chamados técnicos";
        
        /// <summary>
        /// Obtém informações completas da aplicação
        /// </summary>
        /// <returns>String com informações da aplicação</returns>
        public static string GetFullInfo()
        {
            return $"{Name} v{Version}\n{Description}\n{Copyright}";
        }
    }

    /// <summary>
    /// Classe para constantes do sistema
    /// </summary>
    public static class SystemConstants
    {
        // Níveis de acesso
        public const int NIVEL_ADMINISTRADOR = 1;
        public const int NIVEL_TECNICO = 2;
        public const int NIVEL_USUARIO = 3;

        // Status de chamados
        public const int STATUS_ABERTO = 1;
        public const int STATUS_EM_ANDAMENTO = 2;
        public const int STATUS_RESOLVIDO = 3;
        public const int STATUS_FECHADO = 4;
        public const int STATUS_CANCELADO = 5;

        // Prioridades
        public const int PRIORIDADE_BAIXA = 1;
        public const int PRIORIDADE_MEDIA = 2;
        public const int PRIORIDADE_ALTA = 3;
        public const int PRIORIDADE_CRITICA = 4;

        // Mensagens do sistema
        public const string MSG_LOGIN_SUCESSO = "Login realizado com sucesso!";
        public const string MSG_LOGIN_ERRO = "Email ou senha incorretos.";
        public const string MSG_ACESSO_NEGADO = "Acesso negado. Você não tem permissão para esta operação.";
        public const string MSG_OPERACAO_SUCESSO = "Operação realizada com sucesso!";
        public const string MSG_ERRO_GENERICO = "Ocorreu um erro inesperado. Tente novamente.";

        // Configurações de interface
        public const int GRID_PAGE_SIZE = 50;
        public const int SEARCH_MIN_CHARS = 3;
        public const int AUTO_REFRESH_SECONDS = 30;

    }
}



