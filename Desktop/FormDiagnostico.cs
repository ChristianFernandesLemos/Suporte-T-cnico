using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace SistemaChamados
{
    public partial class FormDiagnostico : Form
    {
        private TextBox txtResultado;
        private Button btnTestar1;
        private Button btnTestar2;
        private Button btnTestar3;
        private Button btnTestar4;
        private Button btnListarServicos;
        private ComboBox cmbInstancia;

        public FormDiagnostico()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtResultado = new TextBox();
            this.btnTestar1 = new Button();
            this.btnTestar2 = new Button();
            this.btnTestar3 = new Button();
            this.btnTestar4 = new Button();
            this.btnListarServicos = new Button();
            this.cmbInstancia = new ComboBox();

            this.SuspendLayout();

            // txtResultado
            this.txtResultado.Location = new System.Drawing.Point(12, 150);
            this.txtResultado.Multiline = true;
            this.txtResultado.Name = "txtResultado";
            this.txtResultado.ScrollBars = ScrollBars.Both;
            this.txtResultado.Size = new System.Drawing.Size(760, 400);
            this.txtResultado.TabIndex = 0;
            this.txtResultado.Font = new System.Drawing.Font("Consolas", 9F);

            // cmbInstancia
            this.cmbInstancia.Location = new System.Drawing.Point(12, 12);
            this.cmbInstancia.Name = "cmbInstancia";
            this.cmbInstancia.Size = new System.Drawing.Size(300, 21);
            this.cmbInstancia.TabIndex = 1;
            this.cmbInstancia.Items.AddRange(new object[] {
                "localhost\\SQLEXPRESS",
                ".\\SQLEXPRESS",
                "(localdb)\\MSSQLLocalDB",
                "localhost",
                ".",
                "(local)"
            });
            this.cmbInstancia.SelectedIndex = 0;

            // btnTestar1
            this.btnTestar1.Location = new System.Drawing.Point(12, 50);
            this.btnTestar1.Name = "btnTestar1";
            this.btnTestar1.Size = new System.Drawing.Size(180, 30);
            this.btnTestar1.TabIndex = 2;
            this.btnTestar1.Text = "1. Testar App.config";
            this.btnTestar1.Click += new EventHandler(this.btnTestar1_Click);

            // btnTestar2
            this.btnTestar2.Location = new System.Drawing.Point(198, 50);
            this.btnTestar2.Name = "btnTestar2";
            this.btnTestar2.Size = new System.Drawing.Size(180, 30);
            this.btnTestar2.TabIndex = 3;
            this.btnTestar2.Text = "2. Testar Master DB";
            this.btnTestar2.Click += new EventHandler(this.btnTestar2_Click);

            // btnTestar3
            this.btnTestar3.Location = new System.Drawing.Point(384, 50);
            this.btnTestar3.Name = "btnTestar3";
            this.btnTestar3.Size = new System.Drawing.Size(180, 30);
            this.btnTestar3.TabIndex = 4;
            this.btnTestar3.Text = "3. Testar Instância Selecionada";
            this.btnTestar3.Click += new EventHandler(this.btnTestar3_Click);

            // btnTestar4
            this.btnTestar4.Location = new System.Drawing.Point(570, 50);
            this.btnTestar4.Name = "btnTestar4";
            this.btnTestar4.Size = new System.Drawing.Size(200, 30);
            this.btnTestar4.TabIndex = 5;
            this.btnTestar4.Text = "4. Verificar Banco Existe";
            this.btnTestar4.Click += new EventHandler(this.btnTestar4_Click);

            // btnListarServicos
            this.btnListarServicos.Location = new System.Drawing.Point(12, 90);
            this.btnListarServicos.Name = "btnListarServicos";
            this.btnListarServicos.Size = new System.Drawing.Size(200, 30);
            this.btnListarServicos.TabIndex = 6;
            this.btnListarServicos.Text = "Listar Serviços SQL Server";
            this.btnListarServicos.Click += new EventHandler(this.btnListarServicos_Click);

            // FormDiagnostico
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnListarServicos);
            this.Controls.Add(this.btnTestar4);
            this.Controls.Add(this.btnTestar3);
            this.Controls.Add(this.btnTestar2);
            this.Controls.Add(this.btnTestar1);
            this.Controls.Add(this.cmbInstancia);
            this.Controls.Add(this.txtResultado);
            this.Name = "FormDiagnostico";
            this.Text = "🔧 Diagnóstico de Conexão SQL Server";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Log(string mensagem)
        {
            txtResultado.AppendText(mensagem + Environment.NewLine);
            Debug.WriteLine(mensagem);
        }

        private void LogHeader(string titulo)
        {
            string linha = new string('=', 80);
            Log(linha);
            Log(titulo);
            Log(linha);
        }
        

    // ============================================
    // TESTE 1: LER APP.CONFIG
    // ============================================
    private void btnTestar1_Click(object sender, EventArgs e)
        {
            txtResultado.Clear();
            LogHeader("🔍 TESTE 1: LEITURA DO APP.CONFIG");

            try
            {
                // Método 1: ConnectionStrings
                Log("\n📋 Método 1: ConfigurationManager.ConnectionStrings");
                var connStrings = ConfigurationManager.ConnectionStrings;

                if (connStrings.Count > 0)
                {
                    Log($"   ✅ Encontradas {connStrings.Count} connection strings:");
                    foreach (ConnectionStringSettings conn in connStrings)
                    {
                        Log($"   • Nome: {conn.Name}");
                        Log($"     String: {conn.ConnectionString}");
                        Log($"     Provider: {conn.ProviderName}");
                        Log("");
                    }
                }
                else
                {
                    Log("   ❌ Nenhuma connection string encontrada!");
                }

                // Método 2: AppSettings
                Log("\n📋 Método 2: ConfigurationManager.AppSettings");
                var appSettings = ConfigurationManager.AppSettings;
                Log($"   Total de AppSettings: {appSettings.Count}");

                // Método 3: Properties.Settings
                Log("\n📋 Método 3: Properties.Settings.Default");
                try
                {
                    var settingsConn = Properties.Settings.Default.ConnectionString;
                    Log($"   ConnectionString: {settingsConn}");
                }
                catch (Exception ex)
                {
                    Log($"   ❌ Erro ao ler Settings: {ex.Message}");
                }

                // Teste específico
                Log("\n📋 Tentando ler 'Suporte_Tecnico':");
                var suporteConn = ConfigurationManager.ConnectionStrings["Suporte_Tecnico"];
                if (suporteConn != null)
                {
                    Log($"   ✅ ENCONTRADA!");
                    Log($"   String: {suporteConn.ConnectionString}");
                }
                else
                {
                    Log("   ❌ NÃO ENCONTRADA!");
                }
            }
            catch (Exception ex)
            {
                Log($"\n❌ ERRO: {ex.Message}");
                Log($"Stack: {ex.StackTrace}");
            }
        }

        // ============================================
        // TESTE 2: CONECTAR NO MASTER
        // ============================================
        private void btnTestar2_Click(object sender, EventArgs e)
        {
            txtResultado.Clear();
            LogHeader("🔍 TESTE 2: CONEXÃO COM DATABASE MASTER");

            string instancia = cmbInstancia.SelectedItem?.ToString() ?? "localhost\\SQLEXPRESS";

            string[] connectionStrings = new string[]
            {
                $"Server={instancia};Database=master;Integrated Security=true;Connection Timeout=5;",
                $"Server={instancia};Database=master;Integrated Security=true;TrustServerCertificate=true;Connection Timeout=5;",
                $"Server={instancia};Database=master;Integrated Security=SSPI;Connection Timeout=5;",
                $"Data Source={instancia};Initial Catalog=master;Integrated Security=true;Connection Timeout=5;"
            };

            for (int i = 0; i < connectionStrings.Length; i++)
            {
                Log($"\n🔄 Tentativa {i + 1}:");
                Log($"Connection String: {connectionStrings[i]}");

                try
                {
                    using (var conn = new SqlConnection(connectionStrings[i]))
                    {
                        Log("   Abrindo conexão...");
                        conn.Open();

                        Log($"   ✅ SUCESSO!");
                        Log($"   • Estado: {conn.State}");
                        Log($"   • Servidor: {conn.DataSource}");
                        Log($"   • Database: {conn.Database}");
                        Log($"   • Versão: {conn.ServerVersion}");

                        // Listar databases
                        var cmd = new SqlCommand("SELECT name FROM sys.databases ORDER BY name", conn);
                        var reader = cmd.ExecuteReader();

                        Log("\n   📁 Databases encontradas:");
                        while (reader.Read())
                        {
                            string dbName = reader["name"].ToString();
                            Log($"   • {dbName}");
                        }

                        Log("\n   ✅ ESTA CONNECTION STRING FUNCIONA!");
                        Log($"   👉 USE: {connectionStrings[i].Replace("master", "Suporte_Tecnico")}");
                        return; // Parar no primeiro sucesso
                    }
                }
                catch (SqlException sqlEx)
                {
                    Log($"   ❌ Erro SQL #{sqlEx.Number}: {sqlEx.Message}");

                    switch (sqlEx.Number)
                    {
                        case 2:
                        case 53:
                            Log("   💡 O servidor não foi encontrado ou não está acessível");
                            Log("   🔧 Verifique se o serviço SQL Server está rodando");
                            break;
                        case 26:
                            Log("   💡 Erro ao localizar o servidor/instância especificada");
                            Log("   🔧 Verifique o nome da instância");
                            break;
                        case 18456:
                            Log("   💡 Falha na autenticação");
                            Log("   🔧 Verifique as credenciais ou use Integrated Security");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log($"   ❌ Erro: {ex.Message}");
                }
            }

            Log("\n❌ NENHUMA CONNECTION STRING FUNCIONOU!");
        }

        // ============================================
        // TESTE 3: TESTAR INSTÂNCIA SELECIONADA
        // ============================================
        private void btnTestar3_Click(object sender, EventArgs e)
        {
            txtResultado.Clear();
            LogHeader("🔍 TESTE 3: TESTAR INSTÂNCIA SELECIONADA");

            string instancia = cmbInstancia.SelectedItem?.ToString() ?? "localhost\\SQLEXPRESS";
            Log($"Instância selecionada: {instancia}\n");

            string connString = $"Server={instancia};Database=Suporte_Tecnico;Integrated Security=true;TrustServerCertificate=true;Connection Timeout=5;";
            Log($"Connection String: {connString}\n");

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    Log("Tentando conectar...");
                    conn.Open();

                    Log("✅ CONEXÃO ESTABELECIDA!");
                    Log($"• Servidor: {conn.DataSource}");
                    Log($"• Database: {conn.Database}");
                    Log($"• Versão: {conn.ServerVersion}");
                    Log($"• Estado: {conn.State}");

                    // Testar query
                    Log("\n🔍 Testando query SELECT:");
                    var cmd = new SqlCommand("SELECT COUNT(*) FROM Usuario", conn);
                    int count = (int)cmd.ExecuteScalar();
                    Log($"✅ Total de usuários: {count}");

                    Log("\n✅ TUDO FUNCIONANDO!");
                    Log($"\n👉 USE ESTA CONNECTION STRING NO SEU APP.CONFIG:");
                    Log(connString);
                }
            }
            catch (SqlException sqlEx)
            {
                Log($"❌ Erro SQL #{sqlEx.Number}: {sqlEx.Message}");
                Log($"\nDetalhes:");
                Log($"• Source: {sqlEx.Source}");
                Log($"• Procedure: {sqlEx.Procedure}");
                Log($"• Server: {sqlEx.Server}");

                if (sqlEx.Number == 4060)
                {
                    Log("\n💡 O banco 'Suporte_Tecnico' não existe!");
                    Log("🔧 Execute o script SQL para criar o banco");
                }
            }
            catch (Exception ex)
            {
                Log($"❌ Erro: {ex.Message}");
                Log($"Stack: {ex.StackTrace}");
            }
        }

        // ============================================
        // TESTE 4: VERIFICAR SE BANCO EXISTE
        // ============================================
        private void btnTestar4_Click(object sender, EventArgs e)
        {
            string resultado = "🎯 TESTE FINAL - MSSQLSERVER (Instância Padrão)\n";
            resultado += new string('=', 80) + "\n\n";

            // Connection string CORRETA para sua instalação
            string connectionString = "Server=localhost;Database=Suporte_Tecnico;Integrated Security=true;TrustServerCertificate=true;Connection Timeout=10;";

            resultado += "📋 Connection String que será testada:\n";
            resultado += connectionString + "\n\n";
            resultado += new string('-', 80) + "\n\n";

            try
            {
                resultado += "🔄 Passo 1: Conectando ao servidor...\n";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    resultado += "✅ CONEXÃO ESTABELECIDA COM SUCESSO!\n\n";
                    resultado += "📊 Informações do Servidor:\n";
                    resultado += $"   • Data Source: {connection.DataSource}\n";
                    resultado += $"   • Database: {connection.Database}\n";
                    resultado += $"   • Server Version: {connection.ServerVersion}\n";
                    resultado += $"   • State: {connection.State}\n";
                    resultado += $"   • Timeout: {connection.ConnectionTimeout}s\n\n";

                    resultado += new string('-', 80) + "\n\n";

                    // Verificar se o banco existe
                    resultado += "🔄 Passo 2: Verificando se banco 'Suporte_Tecnico' existe...\n";

                    connection.ChangeDatabase("master");
                    var cmdVerificar = new SqlCommand(
                        "SELECT COUNT(*) FROM sys.databases WHERE name = 'Suporte_Tecnico'",
                        connection);

                    int bancoExiste = (int)cmdVerificar.ExecuteScalar();

                    if (bancoExiste > 0)
                    {
                        resultado += "✅ Banco 'Suporte_Tecnico' EXISTE!\n\n";

                        // Conectar no banco e verificar tabelas
                        resultado += new string('-', 80) + "\n\n";
                        resultado += "🔄 Passo 3: Verificando tabelas no banco...\n";

                        connection.ChangeDatabase("Suporte_Tecnico");

                        var cmdTabelas = new SqlCommand(@"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE = 'BASE TABLE'
                    ORDER BY TABLE_NAME",
                            connection);

                        var reader = cmdTabelas.ExecuteReader();
                        int countTabelas = 0;

                        resultado += "\n📁 Tabelas encontradas:\n";
                        while (reader.Read())
                        {
                            resultado += $"   ✓ {reader["TABLE_NAME"]}\n";
                            countTabelas++;
                        }
                        reader.Close();

                        resultado += $"\n✅ Total: {countTabelas} tabelas\n\n";

                        if (countTabelas > 0)
                        {
                            // Verificar dados nas tabelas principais
                            resultado += new string('-', 80) + "\n\n";
                            resultado += "🔄 Passo 4: Verificando dados...\n\n";

                            // Verificar usuários
                            var cmdUsuarios = new SqlCommand("SELECT COUNT(*) FROM Usuario", connection);
                            int totalUsuarios = (int)cmdUsuarios.ExecuteScalar();
                            resultado += $"   • Usuários: {totalUsuarios} registros\n";

                            // Verificar emails
                            var cmdEmails = new SqlCommand("SELECT COUNT(*) FROM E_mail", connection);
                            int totalEmails = (int)cmdEmails.ExecuteScalar();
                            resultado += $"   • Emails: {totalEmails} registros\n";

                            // Verificar chamados
                            var cmdChamados = new SqlCommand("SELECT COUNT(*) FROM chamados", connection);
                            int totalChamados = (int)cmdChamados.ExecuteScalar();
                            resultado += $"   • Chamados: {totalChamados} registros\n\n";

                            if (totalUsuarios > 0)
                            {
                                resultado += new string('-', 80) + "\n\n";
                                resultado += "🔄 Passo 5: Testando login do usuário admin...\n\n";

                                // Testar VIEW de login
                                var cmdLogin = new SqlCommand(@"
                            SELECT Id, Nome, Email, Senha, NivelAcesso
                            FROM vw_LoginUsuarios
                            WHERE Email = 'chriscamplopes@gmail.com'
                            AND Senha = 'MinhaSenha'
                            AND Ativo = 1",
                                    connection);

                                var readerLogin = cmdLogin.ExecuteReader();

                                if (readerLogin.Read())
                                {
                                    resultado += "✅ LOGIN FUNCIONANDO!\n\n";
                                    resultado += "👤 Dados do usuário:\n";
                                    resultado += $"   • ID: {readerLogin["Id"]}\n";
                                    resultado += $"   • Nome: {readerLogin["Nome"]}\n";
                                    resultado += $"   • Email: {readerLogin["Email"]}\n";
                                    resultado += $"   • Nível: {readerLogin["NivelAcesso"]}\n";
                                }
                                else
                                {
                                    resultado += "⚠️ Login não encontrado ou senha incorreta\n";
                                    resultado += "Mas a conexão está funcionando!\n";
                                }
                                readerLogin.Close();
                            }

                            resultado += "\n" + new string('=', 80) + "\n";
                            resultado += "🎉 SUCESSO TOTAL!\n";
                            resultado += new string('=', 80) + "\n\n";

                            resultado += "✅ TUDO FUNCIONANDO PERFEITAMENTE!\n\n";
                            resultado += "👉 USE ESTA CONNECTION STRING NO SEU App.config:\n\n";
                            resultado += connectionString + "\n\n";

                            resultado += "📝 Seu App.config deve ter:\n\n";
                            resultado += "<connectionStrings>\n";
                            resultado += "  <add name=\"Suporte_Tecnico\"\n";
                            resultado += $"       connectionString=\"{connectionString}\"\n";
                            resultado += "       providerName=\"System.Data.SqlClient\" />\n";
                            resultado += "</connectionStrings>\n\n";

                            resultado += "⚠️ IMPORTANTE: Note que NÃO tem \\SQLEXPRESS!\n";
                            resultado += "Você tem a instância PADRÃO (MSSQLSERVER)\n";
                        }
                    }
                    else
                    {
                        resultado += "❌ Banco 'Suporte_Tecnico' NÃO EXISTE!\n\n";
                        resultado += "🔧 SOLUÇÃO:\n";
                        resultado += "1. Abra SQL Server Management Studio (SSMS)\n";
                        resultado += "2. Conecte em: localhost (sem \\SQLEXPRESS)\n";
                        resultado += "3. Execute o script 'Suporte_Tecnico.sql' completo\n";
                        resultado += "4. Execute este teste novamente\n";
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                resultado += $"❌ ERRO SQL #{sqlEx.Number}: {sqlEx.Message}\n\n";
                resultado += "Detalhes:\n";
                resultado += $"Source: {sqlEx.Source}\n";
                resultado += $"Server: {sqlEx.Server}\n\n";

                switch (sqlEx.Number)
                {
                    case -1:
                    case 2:
                    case 53:
                        resultado += "💡 PROBLEMA: Servidor não encontrado\n";
                        resultado += "🔧 SOLUÇÃO: Isso é estranho, pois o serviço está rodando...\n";
                        resultado += "Tente usar '.' ao invés de 'localhost':\n";
                        resultado += "Server=.;Database=Suporte_Tecnico;Integrated Security=true;\n";
                        break;
                    case 4060:
                        resultado += "💡 PROBLEMA: Banco não existe\n";
                        resultado += "🔧 SOLUÇÃO: Execute o script SQL de criação\n";
                        break;
                    case 18456:
                        resultado += "💡 PROBLEMA: Autenticação falhou\n";
                        resultado += "🔧 SOLUÇÃO: Verifique as permissões do Windows\n";
                        break;
                }
            }
            catch (Exception ex)
            {
                resultado += $"❌ ERRO GERAL: {ex.Message}\n\n";
                resultado += $"Stack Trace:\n{ex.StackTrace}\n";
            }

            // Mostrar resultado
            Form formResultado = new Form();
            formResultado.Text = "Resultado do Teste Final";
            formResultado.Size = new System.Drawing.Size(900, 700);
            formResultado.StartPosition = FormStartPosition.CenterScreen;

            TextBox txtResultado = new TextBox();
            txtResultado.Multiline = true;
            txtResultado.ScrollBars = ScrollBars.Both;
            txtResultado.Dock = DockStyle.Fill;
            txtResultado.Font = new System.Drawing.Font("Consolas", 9);
            txtResultado.Text = resultado;
            txtResultado.ReadOnly = true;

            Button btnCopiar = new Button();
            btnCopiar.Text = "📋 Copiar Resultado";
            btnCopiar.Dock = DockStyle.Bottom;
            btnCopiar.Height = 40;
            btnCopiar.Click += (s, ev) => {
                Clipboard.SetText(resultado);
                MessageBox.Show("✅ Resultado copiado para clipboard!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            Button btnFechar = new Button();
            btnFechar.Text = "✅ Fechar";
            btnFechar.Dock = DockStyle.Bottom;
            btnFechar.Height = 40;
            btnFechar.Click += (s, ev) => formResultado.Close();

            formResultado.Controls.Add(txtResultado);
            formResultado.Controls.Add(btnCopiar);
            formResultado.Controls.Add(btnFechar);
            formResultado.ShowDialog();

            // Também mostrar em MessageBox
            if (resultado.Contains("SUCESSO TOTAL"))
            {
                MessageBox.Show(
                    "🎉 CONEXÃO FUNCIONANDO!\n\n" +
                    "Seu banco de dados está configurado corretamente.\n" +
                    "Verifique o resultado completo na janela de texto.",
                    "Teste Bem-Sucedido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        // ============================================
        // LISTAR SERVIÇOS SQL SERVER
        // ============================================
        private void btnListarServicos_Click(object sender, EventArgs e)
        {
            txtResultado.Clear();
            LogHeader("🔍 SERVIÇOS SQL SERVER NO SISTEMA");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c sc query type= service state= all | findstr /i \"SQL\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    Log("Serviços SQL encontrados:");
                    Log(output);
                }
                else
                {
                    Log("❌ Nenhum serviço SQL Server encontrado!");
                    Log("\n💡 Possíveis causas:");
                    Log("• SQL Server não está instalado");
                    Log("• SQL Server está instalado mas com outro nome");
                }

                // Tentar também com PowerShell
                Log("\n" + new string('-', 80));
                Log("Tentando com PowerShell:");

                psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Get-Service | Where-Object {$_.Name -like '*SQL*'} | Format-Table -AutoSize\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process = Process.Start(psi);
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    Log(output);
                }
            }
            catch (Exception ex)
            {
                Log($"❌ Erro ao listar serviços: {ex.Message}");
            }
        }
    }
}