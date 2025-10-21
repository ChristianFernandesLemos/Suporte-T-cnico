using System;
using System.Security.Cryptography;
using System.Text;

namespace SistemaChamados.Helpers
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Gera um hash SHA256 da senha fornecida
        /// </summary>
        /// <param name="senha">Senha em texto simples</param>
        /// <returns>Hash da senha em formato hexadecimal</returns>
        public static string GerarHash(string senha)
        {
            if (string.IsNullOrEmpty(senha))
                throw new ArgumentException("A senha não pode ser vazia", nameof(senha));

            using (SHA256 sha256 = SHA256.Create())
            {
                // Converte a senha para bytes
                byte[] senhaBytes = Encoding.UTF8.GetBytes(senha);
                
                // Calcula o hash
                byte[] hashBytes = sha256.ComputeHash(senhaBytes);
                
                // Converte o hash para string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifica se a senha fornecida corresponde ao hash armazenado
        /// </summary>
        /// <param name="senha">Senha em texto simples</param>
        /// <param name="hashArmazenado">Hash armazenado no banco de dados</param>
        /// <returns>True se a senha estiver correta, False caso contrário</returns>
        public static bool VerificarSenha(string senha, string hashArmazenado)
        {
            if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(hashArmazenado))
                return false;

            string hashSenha = GerarHash(senha);
            return hashSenha.Equals(hashArmazenado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
