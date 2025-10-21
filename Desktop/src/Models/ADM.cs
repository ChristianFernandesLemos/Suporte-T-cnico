using SistemaChamados.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SistemaChamados.Models
{
    // Classe filha ADM (Administrador)
    public class ADM : Funcionarios
    {
        public ADM() : base() { }

        // Construtor
        public ADM(int id, string nome, string cpf, string email, string senha, int nivelAcesso)
            : base(id, nome, cpf, email, senha, nivelAcesso)
        {
        }

        public override string TipoFuncionario => "Administrador";
        public override int NivelAcesso => 3;

        // Método para adicionar usuários
        public void AdicionarUsuarios(Funcionarios novoFuncionario)
        {
            try
            {
                // Lógica para adicionar novo usuário ao sistema
                // Aqui seria feita a inserção no banco de dados
                Console.WriteLine($"Usuário {novoFuncionario.Email} adicionado pelo administrador {Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar usuário: {ex.Message}");
            }
        }

        // Sobrecarga do método AdicionarUsuarios com parâmetros individuais
        public void AdicionarUsuarios(int id, string nome, string cpf, string email, string senha, int nivelAcesso, string tipoFuncionario)
        {
            try
            {
                Funcionarios novoFuncionario;
                
                // Criar instância baseada no tipo de funcionário
                switch (tipoFuncionario.ToLower())
                {
                    case "tecnico":
                        novoFuncionario = new Tecnico(id, nome, cpf, email, senha, nivelAcesso);
                        break;
                    case "adm":
                        novoFuncionario = new ADM(id, nome, cpf, email, senha, nivelAcesso);
                        break;
                    case "funcionario":
                        novoFuncionario = new Funcionario(id, nome, cpf, email, senha, nivelAcesso);
                        break;
                    default:
                        throw new ArgumentException("Tipo de funcionário inválido");
                }

                AdicionarUsuarios(novoFuncionario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar e adicionar usuário: {ex.Message}");
            }
        }

        // Método para alterar senha
        public void AlterarSenha(int idFuncionario, string novaSenha)
        {
            try
            {
                // Lógica para alterar senha de um funcionário
                // Aqui seria feita a atualização no banco de dados
                Console.WriteLine($"Senha do funcionário {idFuncionario} alterada pelo administrador {Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar senha: {ex.Message}");
            }
        }

        
        // Método para alterar própria senha
        public void AlterarSenha(string novaSenha)
        {
            try
            {
                // Lógica para alterar própria senha
                this.Senha = novaSenha;
                Console.WriteLine($"Senha do administrador {Id} alterada com sucesso");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar própria senha: {ex.Message}");
            }
        }

        // Override do método VisualizarChamados para administradores
        public override void VisualizarChamados()
        {
            // Implementação específica para administradores
            // Administradores podem ver todos os chamados do sistema
            Console.WriteLine($"Visualizando todos os chamados do sistema - Administrador {Id}");
        }

        // Método adicional para remover usuários (funcionalidade administrativa)
        public void RemoverUsuario(int idFuncionario)
        {
            try
            {
                // Lógica para remover usuário do sistema
                Console.WriteLine($"Usuário {idFuncionario} removido pelo administrador {Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao remover usuário: {ex.Message}");
            }
        }
    }
}
