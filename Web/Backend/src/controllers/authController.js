const { getConnection, sql } = require('../config/database');

class AuthController {
  async login(req, res) {
    try {
      const { email, senha } = req.body;
      
      if (!email || !senha) {
        return res.status(400).json({ 
          error: 'Email e senha são obrigatórios' 
        });
      }

      const pool = await getConnection();
      const result = await pool.request()
        .input('email', sql.VarChar, email)
        .input('senha', sql.VarChar, senha)
        .query(`
          SELECT id, nome, email, tipo_usuario 
          FROM usuarios 
          WHERE email = @email AND senha = @senha
        `);

      if (result.recordset.length > 0) {
        const user = result.recordset[0];
        res.json({
          success: true,
          message: 'Login realizado com sucesso!',
          user: user
        });
      } else {
        res.status(401).json({
          success: false,
          message: 'Email ou senha incorretos'
        });
      }
    } catch (error) {
      console.error('Erro no login:', error);
      res.status(500).json({ 
        error: 'Erro interno do servidor' 
      });
    }
  }

  async register(req, res) {
    try {
      const { nome, email, senha, tipo_usuario } = req.body;
      
      const pool = await getConnection();
      const result = await pool.request()
        .input('nome', sql.VarChar, nome)
        .input('email', sql.VarChar, email)
        .input('senha', sql.VarChar, senha)
        .input('tipo_usuario', sql.VarChar, tipo_usuario || 'usuario')
        .query(`
          INSERT INTO usuarios (nome, email, senha, tipo_usuario)
          OUTPUT INSERTED.id, INSERTED.nome, INSERTED.email, INSERTED.tipo_usuario
          VALUES (@nome, @email, @senha, @tipo_usuario)
        `);

      res.status(201).json({
        success: true,
        message: 'Usuário criado com sucesso!',
        user: result.recordset[0]
      });
    } catch (error) {
      console.error('Erro no registro:', error);
      res.status(500).json({ 
        error: 'Erro ao criar usuário' 
      });
    }
  }
}

module.exports = new AuthController();