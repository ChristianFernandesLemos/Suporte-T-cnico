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
                        u.Id_usuario,
                        u.nome,
                        u.Cpf,
                        e.E_mail,
                        u.senha,
                        u.Acess_codigo,
                        n.Nivel_acesso
                    FROM dbo.Usuario u
                    INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
                    INNER JOIN dbo.Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                    WHERE e.E_mail = @email AND u.Ativo = 1
                `);
            
            if (result.recordset.length === 0) {
                return null;
            }

            const userData = result.recordset[0];
            const usuario = new Usuarios();
            usuario.id_Usuario = userData.Id_usuario;
            usuario.Nome = userData.nome;
            usuario.Cpf = userData.Cpf;
            usuario.E_mail = userData.E_mail;
            usuario.Senha = userData.senha;
            usuario.Nivel_Acesso = userData.Acess_codigo;
            
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
                        u.Id_usuario,
                        u.nome,
                        u.Cpf,
                        e.E_mail,
                        u.senha,
                        u.Acess_codigo,
                        n.Nivel_acesso
                    FROM dbo.Usuario u
                    INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
                    INNER JOIN dbo.Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                    WHERE u.Id_usuario = @id AND u.Ativo = 1
                `);
            
            if (result.recordset.length === 0) {
                return null;
            }

            const userData = result.recordset[0];
            const usuario = new Usuarios();
            usuario.id_Usuario = userData.Id_usuario;
            usuario.Nome = userData.nome;
            usuario.Cpf = userData.Cpf;
            usuario.E_mail = userData.E_mail;
            usuario.Senha = userData.senha;
            usuario.Nivel_Acesso = userData.Acess_codigo;
            
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
                        u.Id_usuario,
                        u.nome,
                        u.Cpf,
                        e.E_mail,
                        u.Acess_codigo,
                        n.Nivel_acesso,
                        u.DataCadastro
                    FROM dbo.Usuario u
                    INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
                    INNER JOIN dbo.Nivel_de_acesso n ON u.Acess_codigo = n.codigo
                    WHERE u.Ativo = 1
                    ORDER BY u.nome
                `);
            
            return result.recordset.map(userData => {
                const usuario = new Usuarios();
                usuario.id_Usuario = userData.Id_usuario;
                usuario.Nome = userData.nome;
                usuario.Cpf = userData.Cpf;
                usuario.E_mail = userData.E_mail;
                usuario.Nivel_Acesso = userData.Acess_codigo;
                return usuario;
            });
        } catch (error) {
            console.error('Erro ao listar usuários:', error);
            throw error;
        }
    }

    // Salva usuário no banco (INSERT)
    async salvar() {
        const pool = await getConnection();
        const transaction = pool.transaction();
        
        try {
            await transaction.begin();
            
            // Insere o usuário
            const resultUsuario = await transaction.request()
                .input('nome', this.Nome)
                .input('cpf', this.Cpf)
                .input('senha', this.Senha)
                .input('acess_codigo', this.Nivel_Acesso)
                .query(`
                    INSERT INTO dbo.Usuario (nome, Cpf, senha, Acess_codigo, Ativo, DataCadastro)
                    OUTPUT INSERTED.Id_usuario
                    VALUES (@nome, @cpf, @senha, @acess_codigo, 1, GETDATE())
                `);
            
            this.id_Usuario = resultUsuario.recordset[0].Id_usuario;
            
            // Insere o email na tabela de emails
            await transaction.request()
                .input('email', this.E_mail)
                .input('id_usuario', this.id_Usuario)
                .query(`
                    INSERT INTO dbo.E_mail (E_mail, Id_usuario)
                    VALUES (@email, @id_usuario)
                `);
            
            await transaction.commit();
            return this;
        } catch (error) {
            await transaction.rollback();
            console.error('Erro ao salvar usuário:', error);
            throw error;
        }
    }

    // Atualiza usuário no banco (UPDATE)
    async atualizar() {
        const pool = await getConnection();
        const transaction = pool.transaction();
        
        try {
            await transaction.begin();
            
            // Atualiza dados do usuário
            await transaction.request()
                .input('id', this.id_Usuario)
                .input('nome', this.Nome)
                .input('cpf', this.Cpf)
                .input('acess_codigo', this.Nivel_Acesso)
                .query(`
                    UPDATE dbo.Usuario
                    SET nome = @nome,
                        Cpf = @cpf,
                        Acess_codigo = @acess_codigo
                    WHERE Id_usuario = @id
                `);
            
            // Atualiza email
            await transaction.request()
                .input('id', this.id_Usuario)
                .input('email', this.E_mail)
                .query(`
                    UPDATE dbo.E_mail
                    SET E_mail = @email
                    WHERE Id_usuario = @id
                `);
            
            await transaction.commit();
            return this;
        } catch (error) {
            await transaction.rollback();
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
                    UPDATE dbo.Usuario
                    SET senha = @senha
                    WHERE Id_usuario = @id
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
                    UPDATE dbo.Usuario
                    SET Ativo = 0
                    WHERE Id_usuario = @id
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
                    UPDATE dbo.Usuario
                    SET Ativo = 1
                    WHERE Id_usuario = @id
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