// ============================================
// INSTRUÇÕES:
// 1. Feche o Visual Studio
// 2. Delete o arquivo SqlServerConnection.cs atual
// 3. Renomeie este arquivo para SqlServerConnection.cs
// 4. Abra novamente o Visual Studio
// ============================================

// CAMBIOS REALIZADOS:
// ✅ Removidos TODOS os MessageBox.Show de debug
// ✅ Mantidos apenas Debug.WriteLine (aparece só no Output)
// ✅ Código limpo e funcional

// Localize as seguintes linhas no seu SqlServerConnection.cs:
// 
// 1. Método ValidarLogin (linha ~655)
//    REMOVA estas linhas:
//        System.Windows.Forms.MessageBox.Show($"Email: {email}\nSenha: {senha}\nBD: {_connectionString}", "DEBUG LOGIN");
//        System.Windows.Forms.MessageBox.Show($"COUNT resultado: {count}", "DEBUG RESULT");
//        System.Windows.Forms.MessageBox.Show($"ERROR: {ex.Message}", "DEBUG ERROR");
//
// 2. SUBSTITUA o método ValidarLogin completo por:

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

// 3. REMOVA todos os Debug.WriteLine detalhados do BuscarFuncionarioPorEmail
//    (las líneas que empiezan con System.Diagnostics.Debug.WriteLine dentro de ese método)
//
// 4. REMOVA todos los Debug.WriteLine de CriarFuncionarioPorTipo
//    (excepto el WriteLine de error al final)