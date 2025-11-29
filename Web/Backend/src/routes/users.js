const express = require('express');
const router = express.Router();
const UserController = require('../controllers/UserController');
const { requireAuth, requireAdmin } = require('../middleware/auth');
const { getConnection, sql } = require('../../db');

// ========================================
// ⭐ ROTA PÚBLICA - BUSCAR USUÁRIO POR EMAIL
// ========================================
// IMPORTANTE: Esta rota DEVE ficar ANTES do router.use(requireAuth)
router.get('/buscar-por-email', async (req, res) => {
  try {
    const { email } = req.query;
    
    if (!email) {
      return res.status(400).json({
        success: false,
        message: 'Email não fornecido'
      });
    }

    console.log(`📧 Buscando usuário por email: ${email}`);
    
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('email', sql.VarChar(100), email)
      .query(`
        SELECT 
          u.Id_usuario as userId,
          u.nome as nome,
          e.E_mail as email,
          u.Ativo as ativo
        FROM dbo.Usuario u
        INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE e.E_mail = @email
      `);

    if (result.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Usuário não encontrado'
      });
    }

    const usuario = result.recordset[0];
    
    console.log('✅ Usuário encontrado:', usuario);

    res.json({
      success: true,
      userId: usuario.userId,
      nome: usuario.nome,
      email: usuario.email,
      ativo: usuario.ativo
    });

  } catch (error) {
    console.error('❌ Erro ao buscar usuário por email:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar usuário',
      error: error.message
    });
  }
});

// ========================================
// ⬇️ A partir daqui, TODAS as rotas exigem autenticação
// ========================================
router.use(requireAuth);

// Rotas que requerem permissão de admin
router.get('/', requireAdmin, UserController.index);
router.get('/:id', requireAdmin, UserController.show);
router.post('/', requireAdmin, UserController.create);
router.put('/:id', requireAdmin, UserController.update);
router.patch('/:id/deactivate', requireAdmin, UserController.deactivate);
router.patch('/:id/activate', requireAdmin, UserController.activate);

// ========================================
// BUSCAR USUÁRIO POR ID (PROTEGIDO)
// ========================================
router.get('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    
    console.log(`🔍 Buscando usuário ID: ${id}`);
    
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('id', sql.Int, id)
      .query(`
        SELECT 
          u.Id_usuario as userId,
          u.nome as nome,
          u.senha as senha,
          u.Cpf as cpf,
          u.Acess_codigo as accessCodigo,
          u.DataCadastro as dataCadastro,
          u.Ativo as ativo,
          e.E_mail as email
        FROM dbo.Usuario u
        LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE u.Id_usuario = @id
      `);

    if (result.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Usuário não encontrado'
      });
    }

    const usuario = result.recordset[0];
    
    // Remove senha antes de retornar
    delete usuario.senha;
    
    console.log('✅ Usuário encontrado:', usuario);

    res.json({
      success: true,
      usuario
    });

  } catch (error) {
    console.error('❌ Erro ao buscar usuário:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar usuário',
      error: error.message
    });
  }
});

// ========================================
// LISTAR TODOS OS USUÁRIOS (PROTEGIDO)
// ========================================
router.get('/', async (req, res) => {
  try {
    console.log('📡 Buscando todos os usuários...');
    
    const pool = await getConnection();
    
    const result = await pool.request().query(`
      SELECT 
        u.Id_usuario as userId,
        u.nome as nome,
        u.Cpf as cpf,
        u.Acess_codigo as accessCodigo,
        u.DataCadastro as dataCadastro,
        u.Ativo as ativo,
        e.E_mail as email
      FROM dbo.Usuario u
      LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
      ORDER BY u.DataCadastro DESC
    `);

    console.log(`✅ ${result.recordset.length} usuários encontrados`);

    res.json({
      success: true,
      total: result.recordset.length,
      usuarios: result.recordset
    });

  } catch (error) {
    console.error('❌ Erro ao listar usuários:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao listar usuários',
      error: error.message
    });
  }
});

// ========================================
// CRIAR NOVO USUÁRIO (PROTEGIDO)
// ========================================
router.post('/', async (req, res) => {
  try {
    const {
      nome,
      email,
      senha,
      cpf,
      accessCodigo
    } = req.body;

    // Validação básica
    if (!nome || !email || !senha) {
      return res.status(400).json({
        success: false,
        message: 'Nome, email e senha são obrigatórios'
      });
    }

    const pool = await getConnection();
    
    // Inicia transação
    const transaction = pool.transaction();
    await transaction.begin();

    try {
      // Insere usuário
      const resultUsuario = await transaction.request()
        .input('nome', sql.VarChar(100), nome)
        .input('senha', sql.VarChar(100), senha)
        .input('cpf', sql.Char(11), cpf || null)
        .input('accessCodigo', sql.Decimal(18, 0), accessCodigo || null)
        .query(`
          INSERT INTO dbo.Usuario (nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo)
          OUTPUT INSERTED.Id_usuario
          VALUES (@nome, @senha, @cpf, @accessCodigo, GETDATE(), 1)
        `);

      const novoUsuarioId = resultUsuario.recordset[0].Id_usuario;

      // Insere email
      await transaction.request()
        .input('email', sql.VarChar(100), email)
        .input('usuarioId', sql.Int, novoUsuarioId)
        .query(`
          INSERT INTO dbo.E_mail (E_mail, Id_usuario)
          VALUES (@email, @usuarioId)
        `);

      await transaction.commit();

      console.log(`✅ Usuário ${novoUsuarioId} criado com sucesso`);

      res.status(201).json({
        success: true,
        message: 'Usuário criado com sucesso',
        userId: novoUsuarioId
      });

    } catch (error) {
      await transaction.rollback();
      throw error;
    }

  } catch (error) {
    console.error('❌ Erro ao criar usuário:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao criar usuário',
      error: error.message
    });
  }
});

// ========================================
// ATUALIZAR USUÁRIO (PROTEGIDO)
// ========================================
router.put('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    const { nome, email, cpf, ativo } = req.body;

    const pool = await getConnection();
    
    // Atualiza dados do usuário
    let updates = [];
    const request = pool.request().input('id', sql.Int, id);

    if (nome !== undefined) {
      updates.push('nome = @nome');
      request.input('nome', sql.VarChar(100), nome);
    }

    if (cpf !== undefined) {
      updates.push('Cpf = @cpf');
      request.input('cpf', sql.Char(11), cpf);
    }

    if (ativo !== undefined) {
      updates.push('Ativo = @ativo');
      request.input('ativo', sql.Bit, ativo);
    }

    if (updates.length > 0) {
      const query = `UPDATE dbo.Usuario SET ${updates.join(', ')} WHERE Id_usuario = @id`;
      await request.query(query);
    }

    // Atualiza email se fornecido
    if (email !== undefined) {
      await pool.request()
        .input('email', sql.VarChar(100), email)
        .input('id', sql.Int, id)
        .query(`
          UPDATE dbo.E_mail 
          SET E_mail = @email 
          WHERE Id_usuario = @id
        `);
    }

    console.log(`✅ Usuário ${id} atualizado`);

    res.json({
      success: true,
      message: 'Usuário atualizado com sucesso'
    });

  } catch (error) {
    console.error('❌ Erro ao atualizar usuário:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao atualizar usuário',
      error: error.message
    });
  }
});

module.exports = router;