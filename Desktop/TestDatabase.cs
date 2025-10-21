using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SistemaChamados.Config;
using SistemaChamados.Data;

namespace SistemaChamados
{
    public class TestDatabase
    {
        public static void TestConnection()
        {
            try
            {
                Console.WriteLine("=== DIAGNÓSTICO DE CONEXÃO ===");

                // 1. Testar string de conexão
                string connectionString = DatabaseConfig.ConnectionString;
                Console.WriteLine($"String de conexão: {connectionString}");

                // 2. Testar conexão direta com SqlConnection
                Console.WriteLine("Testando conexão direta...");
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("✅ Conexão direta OK!");

                    // 3. Testar se as tabelas existem
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Funcionarios'", conn))
                    {
                        int tableCount = (int)cmd.ExecuteScalar();
                        if (tableCount > 0)
                        {
                            Console.WriteLine("✅ Tabela Funcionarios existe!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Tabela Funcionarios não existe!");
                            return;
                        }
                    }

                    // 4. Testar se existe usuário admin
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Funcionarios WHERE Email = 'admin@sistema.com'", conn))
                    {
                        int userCount = (int)cmd.ExecuteScalar();
                        if (userCount > 0)
                        {
                            Console.WriteLine("✅ Usuário admin existe!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Usuário admin não existe!");
                        }
                    }
                }

                // 5. Testar nossa classe SqlServerConnection
                Console.WriteLine("Testando SqlServerConnection...");
                var database = new SqlServerConnection(connectionString);

                if (database.TestarConexao())
                {
                    Console.WriteLine("✅ SqlServerConnection OK!");

                    // 6. Testar login
                    Console.WriteLine("Testando login...");
                    bool loginOK = database.ValidarLogin("admin@sistema.com", "admin123");

                    if (loginOK)
                    {
                        Console.WriteLine("✅ Login funcionando!");

                        // 7. Testar busca de funcionário
                        Console.WriteLine("Testando busca de funcionário...");
                        var funcionario = database.BuscarFuncionarioPorEmail("admin@sistema.com");

                        if (funcionario != null)
                        {
                            Console.WriteLine($"✅ Funcionário encontrado: {funcionario.Nome} - {funcionario.Email}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Erro ao buscar funcionário!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Erro no login!");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Erro no SqlServerConnection!");
                }

                database.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}