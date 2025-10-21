const { getConnection } = require('../../db');

class Usuarios {
    constructor() {
        this.id_Usuario = 0;
        this.Nome = '';
        this.Cpf = '';
        this.E_mail = '';
        this.Senha = '';
        this.Nivel_Acesso = 0; // 1 = Funcionario, 2 = Tecnico, 3 = Adm
    }

    // Busca usuário por email
    static async findByEmail(email) {
        try {
            const pool = await getConnection();
            const result = await pool.request()
                .input('email', email)
                .query(`
                    SELECT 
                        id_Usuario,
                        Nome,
                        Cpf,
                        E_mail,
                        Senha,
                        Nivel_Acesso
                    FROM usuarios
                    WHERE E_mail = @email AND ativo = 1
                `);
            
            if (result.recordset.length === 0) {
                return null;
            }

            const userData = result.recordset[0];
            const usuario = new Usuarios();
            usuario.id_Usuario = userData.id_Usuario;
            usuario.Nome = userData.Nome;
            usuario.Cpf = userData.Cpf;
            usuario.E_mail = userData.E_mail;
            usuario.Senha = userData.Senha;
            usuario.Nivel_Acesso = userData.Nivel_Acesso;
            
            return usuario;
        } catch (error) {
            console.error('Erro ao buscar usuário:', error);
            throw error;
        }
    }

    // Busca usuário por ID
    static async findById(id) {
        try {
            const pool = await getConnection();
            const result = await pool.request()
                .input('id', id)
                .query(`
                    SELECT 
                        id_Usuario,
                        Nome,
                        Cpf,
                        E_mail,
                        Senha,
                        Nivel_Acesso
                    FROM usuarios
                    WHERE id_Usuario = @id AND ativo = 1
                `);
            
            if (result.recordset.length === 0) {
                return null;
            }

            const userData = result.recordset[0];
            const usuario = new Usuarios();
            usuario.id_Usuario = userData.id_Usuario;
            usuario.Nome = userData.Nome;
            usuario.Cpf = userData.Cpf;
            usuario.E_mail = userData.E_mail;
            usuario.Senha = userData.Senha;
            usuario.Nivel_Acesso = userData.Nivel_Acesso;
            
            return usuario;
        } catch (error) {
            console.error('Erro ao buscar usuário por ID:', error);
            throw error;
        }
    }

    // Lista todos os usuários
    static async findAll() {
        try {
            const pool = await getConnection();
            const result = await pool.request()
                .query(`
                    SELECT 
                        id_Usuario,
                        Nome,
                        Cpf,
                        E_mail,
                        Nivel_Acesso,
                        data_criacao
                    FROM usuarios
                    WHERE ativo = 1
                    ORDER BY Nome
                `);
            
            return result.recordset.map(userData => {
                const usuario = new Usuarios();
                usuario.id_Usuario = userData.id_Usuario;
                usuario.Nome = userData.Nome;
                usuario.Cpf = userData.Cpf;
                usuario.E_mail = userData.E_mail;
                usuario.Nivel_Acesso = userData.Nivel_Acesso;
                return usuario;
            });
        } catch (error) {
            console.error('Erro ao listar usuários:', error);
            throw error;
        }
    }

    // Salva usuário no banco (INSERT)
    async salvar() {
        try {
            const pool = await getConnection();
            const result = await pool.request()
                .input('nome', this.Nome)
                .input('cpf', this.Cpf)
                .input('email', this.E_mail)
                .input('senha', this.Senha)
                .input('nivel_acesso', this.Nivel_Acesso)
                .query(`
                    INSERT INTO usuarios (Nome, Cpf, E_mail, Senha, Nivel_Acesso, ativo, data_criacao)
                    OUTPUT INSERTED.id_Usuario
                    VALUES (@nome, @cpf, @email, @senha, @nivel_acesso, 1, GETDATE())
                `);
            
            this.id_Usuario = result.recordset[0].id_Usuario;
            return this;
        } catch (error) {
            console.error('Erro ao salvar usuário:', error);
            throw error;
        }
    }

    // Atualiza usuário no banco (UPDATE)
    async atualizar() {
        try {
            const pool = await getConnection();
            await pool.request()
                .input('id', this.id_Usuario)
                .input('nome', this.Nome)
                .input('cpf', this.Cpf)
                .input('email', this.E_mail)
                .input('nivel_acesso', this.Nivel_Acesso)
                .query(`
                    UPDATE usuarios
                    SET Nome = @nome,
                        Cpf = @cpf,
                        E_mail = @email,
                        Nivel_Acesso = @nivel_acesso,
                        data_atualizacao = GETDATE()
                    WHERE id_Usuario = @id
                `);
            
            return this;
        } catch (error) {
            console.error('Erro ao atualizar usuário:', error);
            throw error;
        }
    }

    // Altera senha
    async alterarSenha(novaSenha) {
        try {
            const pool = await getConnection();
            await pool.request()
                .input('id', this.id_Usuario)
                .input('senha', novaSenha)
                .query(`
                    UPDATE usuarios
                    SET Senha = @senha,
                        data_atualizacao = GETDATE()
                    WHERE id_Usuario = @id
                `);
            
            this.Senha = novaSenha;
            return true;
        } catch (error) {
            console.error('Erro ao alterar senha:', error);
            throw error;
        }
    }

    // Desativa usuário (soft delete)
    async desativar() {
        try {
            const pool = await getConnection();
            await pool.request()
                .input('id', this.id_Usuario)
                .query(`
                    UPDATE usuarios
                    SET ativo = 0,
                        data_atualizacao = GETDATE()
                    WHERE id_Usuario = @id
                `);
            
            return true;
        } catch (error) {
            console.error('Erro ao desativar usuário:', error);
            throw error;
        }
    }

    // Reativa usuário
    async reativar() {
        try {
            const pool = await getConnection();
            await pool.request()
                .input('id', this.id_Usuario)
                .query(`
                    UPDATE usuarios
                    SET ativo = 1,
                        data_atualizacao = GETDATE()
                    WHERE id_Usuario = @id
                `);
            
            return true;
        } catch (error) {
            console.error('Erro ao reativar usuário:', error);
            throw error;
        }
    }

    // Retorna tipo de usuário como string
    getTipoUsuario() {
        switch (this.Nivel_Acesso) {
            case 1:
                return 'funcionario';
            case 2:
                return 'tecnico';
            case 3:
                return 'admin';
            default:
                return 'desconhecido';
        }
    }

    // Converte para objeto simples (sem senha)
    toJSON() {
        return {
            id_Usuario: this.id_Usuario,
            Nome: this.Nome,
            Cpf: this.Cpf,
            E_mail: this.E_mail,
            Nivel_Acesso: this.Nivel_Acesso,
            tipo_usuario: this.getTipoUsuario()
        };
    }
}

module.exports = Usuarios;
