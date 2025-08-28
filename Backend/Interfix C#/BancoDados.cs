using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

public class BancoDados : IDisposable
{
    private SqlConnection conexao;
    private string connectionString;

    public BancoDados()
    {
        // Cadena de conexao para SQL Server com autenticación integrada de Windows
        connectionString = @"Server=localhost\SQLEXPRESS;Database=ChamadosDB;Integrated Security=true;";
        
        InicializarBanco();
    }

    private void InicializarBanco()
    {
        try
        {
            conexao = new SqlConnection(connectionString);
            conexao.Open();
            
            // Verifica se a base de dados existe, senao, cria ela
            VerificarECriarDatabase();
            CriarTabelas();
            InserirUsuariosPadrao();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al conectar con SQL Server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void VerificarECriarDatabase()
    {
        // Verificacao da base de dados
        string checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = 'ChamadosDB'";
        
        using (var tempConn = new SqlConnection(@"Server=localhost\SQLEXPRESS;Integrated Security=true;"))
        {
            tempConn.Open();
            using (var cmd = new SqlCommand(checkDbQuery, tempConn))
            {
                int exists = (int)cmd.ExecuteScalar();
                if (exists == 0)
                {
                    // Criacao (caso nao exista)
                    using (var createCmd = new SqlCommand("CREATE DATABASE ChamadosDB", tempConn))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    private void CriarTabelas()
    {
        using (var cmd = new SqlCommand(conexao))
        {
            // Tabela de usuarios
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='usuarios' AND xtype='U')
                CREATE TABLE usuarios (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    nome NVARCHAR(100) NOT NULL,
                    senha NVARCHAR(100) NOT NULL,
                    tipo NVARCHAR(20) NOT NULL DEFAULT 'user'
                )";
            cmd.ExecuteNonQuery();

            // Tabela de chamados
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='chamados' AND xtype='U')
                CREATE TABLE chamados (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    titulo NVARCHAR(200) NOT NULL,
                    descricao NVARCHAR(MAX) NOT NULL,
                    status NVARCHAR(20) NOT NULL DEFAULT 'aberto',
                    data_abertura DATETIME NOT NULL,
                    usuario_id INT,
                    CONSTRAINT FK_Chamados_Usuarios FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
                )";
            cmd.ExecuteNonQuery();
        }
    }

    private void InserirUsuariosPadrao()
    {
        try
        {
            // Verifica se os usuarios padrao ja existem
            string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE nome IN ('admin', 'user', 'tecnico')";
            using (var cmd = new SqlCommand(checkQuery, conexao))
            {
                int count = (int)cmd.ExecuteScalar();
                if (count == 0)
                {
                    InserirUsuario("admin", "admin123", "admin");
                    InserirUsuario("user", "user123", "user");
                    InserirUsuario("tecnico", "tecnico123", "tecnico");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al insertar usuarios: {ex.Message}");
        }
    }

    public int InserirUsuario(string nome, string senha, string tipo = "user")
    {
        using (var cmd = new SqlCommand(conexao))
        {
            cmd.CommandText = "INSERT INTO usuarios (nome, senha, tipo) OUTPUT INSERTED.id VALUES (@nome, @senha, @tipo)";
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@senha", senha);
            cmd.Parameters.AddWithValue("@tipo", tipo);
            
            return (int)cmd.ExecuteScalar();
        }
    }

    public (int, string) AutenticarUsuario(string nome, string senha)
    {
        using (var cmd = new SqlCommand(conexao))
        {
            cmd.CommandText = "SELECT id, tipo FROM usuarios WHERE nome = @nome AND senha = @senha";
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@senha", senha);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (reader.GetInt32(0), reader.GetString(1));
                }
            }
        }
        return (-1, null);
    }

    public void InserirChamado(string titulo, string descricao, int usuarioId)
    {
        using (var cmd = new SqlCommand(conexao))
        {
            cmd.CommandText = @"
                INSERT INTO chamados (titulo, descricao, status, data_abertura, usuario_id) 
                VALUES (@titulo, @descricao, 'aberto', @data, @usuarioId)";
            
            cmd.Parameters.AddWithValue("@titulo", titulo);
            cmd.Parameters.AddWithValue("@descricao", descricao);
            cmd.Parameters.AddWithValue("@data", DateTime.Now);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
            
            cmd.ExecuteNonQuery();
        }
    }

    public List<Chamado> BuscarChamados(string status = null)
    {
        var chamados = new List<Chamado>();
        string query = "SELECT * FROM chamados";

        if (!string.IsNullOrEmpty(status) && status != "meus")
        {
            query += $" WHERE status = @status";
        }

        using (var cmd = new SqlCommand(query, conexao))
        {
            if (!string.IsNullOrEmpty(status) && status != "meus")
            {
                cmd.Parameters.AddWithValue("@status", status);
            }

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    chamados.Add(new Chamado
                    {
                        Id = reader.GetInt32(0),
                        Titulo = reader.GetString(1),
                        Descricao = reader.GetString(2),
                        Status = reader.GetString(3),
                        DataAbertura = reader.GetDateTime(4),
                        UsuarioId = reader.GetInt32(5)
                    });
                }
            }
        }
        return chamados;
    }

    public List<Chamado> BuscarMeusChamados(int usuarioId)
    {
        var chamados = new List<Chamado>();
        
        using (var cmd = new SqlCommand(conexao))
        {
            cmd.CommandText = "SELECT * FROM chamados WHERE usuario_id = @usuarioId";
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    chamados.Add(new Chamado
                    {
                        Id = reader.GetInt32(0),
                        Titulo = reader.GetString(1),
                        Descricao = reader.GetString(2),
                        Status = reader.GetString(3),
                        DataAbertura = reader.GetDateTime(4),
                        UsuarioId = reader.GetInt32(5)
                    });
                }
            }
        }
        return chamados;
    }

    public void AtualizarChamado(int chamadoId, string status)
    {
        using (var cmd = new SqlCommand(conexao))
        {
            cmd.CommandText = "UPDATE chamados SET status = @status WHERE id = @id";
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", chamadoId);
            cmd.ExecuteNonQuery();
        }
    }

    public void Dispose()
    {
        conexao?.Close();
        conexao?.Dispose();
    }
}