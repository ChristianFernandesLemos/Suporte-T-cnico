// src/routes/contestacoes.js - Rotas para gerenciamento de contestações
const express = require('express');
const router = express.Router();
const { getConnection, sql } = require('../../db');

// ========================================
// LISTAR CONTESTAÇÕES DE UM CHAMADO
// ========================================
router.get('/chamado/:idChamado', async (req, res) => {
  try {
    const { idChamado } = req.params;
    console.log(`📡 Buscando contestações do chamado #${idChamado}...`);
    
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('idChamado', sql.Int, idChamado)
      .query(`
        SELECT 
          hc.Id as id,
          hc.id_chamado as idChamado,
          hc.Id_usuario as idUsuario,
          hc.Justificativa,
          hc.DataContestacao,
          -- hc.Tipo, <--- REMOVIDO
          u.nome as usuarioNome,
          e.E_mail as usuarioEmail
        FROM dbo.Historial_Contestacoes hc
        LEFT JOIN dbo.Usuario u ON hc.Id_usuario = u.Id_usuario
        LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE hc.id_chamado = @idChamado
        ORDER BY hc.DataContestacao DESC
      `);

    if (result.recordset.length === 0) {
      console.log(`ℹ️ Nenhuma contestação encontrada para o chamado #${idChamado}`);
      
      return res.status(200).json({ 
        success: true, 
        message: 'Nenhuma contestação encontrada',
        contestacoes: []
      });
    }

    console.log(`✅ ${result.recordset.length} contestação(ões) encontrada(s)`);

    res.json({
      success: true,
      total: result.recordset.length,
      contestacoes: result.recordset
    });

  } catch (error) {
    console.error('❌ Erro ao buscar contestações:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar contestações',
      error: error.message
    });
  }
});

// ========================================
// BUSCAR CONTESTAÇÃO POR ID
// ========================================
router.get('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    console.log(`📡 Buscando contestação ID: ${id}`);
    
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('id', sql.Int, id)
      .query(`
        SELECT 
          hc.Id as id,
          hc.id_chamado as idChamado,
          hc.Id_usuario as idUsuario,
          hc.Justificativa,
          hc.DataContestacao,
          -- hc.Tipo, <--- REMOVIDO
          u.nome as usuarioNome,
          e.E_mail as usuarioEmail
        FROM dbo.Historial_Contestacoes hc
        LEFT JOIN dbo.Usuario u ON hc.Id_usuario = u.Id_usuario
        LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE hc.Id = @id
      `);

    if (result.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Contestação não encontrada'
      });
    }

    console.log('✅ Contestação encontrada:', result.recordset[0]);

    res.json({
      success: true,
      contestacao: result.recordset[0]
    });

  } catch (error) {
    console.error('❌ Erro ao buscar contestação:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar contestação',
      error: error.message
    });
  }
});

// ========================================
// CRIAR NOVA CONTESTAÇÃO
// ========================================
router.post('/', async (req, res) => {
  try {
    // Campo 'tipo' removido da desestruturação
    const {
      idChamado,
      idUsuario,
      justificativa
      // tipo removido
    } = req.body;

    // Validação básica corrigida (removido 'tipo')
    if (!idChamado || !idUsuario || !justificativa) {
      return res.status(400).json({
        success: false,
        message: 'idChamado, idUsuario e justificativa são obrigatórios'
      });
    }

    // ❌ Validação de tipo removida, pois a coluna não existe.

    const pool = await getConnection();
    
    // Verifica se o chamado existe
    const chamadoExiste = await pool.request()
      .input('idChamado', sql.Int, idChamado)
      .query('SELECT id_chamado FROM dbo.chamados WHERE id_chamado = @idChamado');

    if (chamadoExiste.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Chamado não encontrado'
      });
    }

    // Verifica se o usuário existe
    const usuarioExiste = await pool.request()
      .input('idUsuario', sql.Int, idUsuario)
      .query('SELECT Id_usuario FROM dbo.Usuario WHERE Id_usuario = @idUsuario');

    if (usuarioExiste.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Usuário não encontrado'
      });
    }

    // Insere a contestação (campo 'Tipo' removido do INSERT)
    const result = await pool.request()
      .input('idChamado', sql.Int, idChamado)
      .input('idUsuario', sql.Int, idUsuario)
      .input('justificativa', sql.NVarChar(1000), justificativa)
      // .input('tipo', sql.NVarChar(20), tipo) <-- INPUT REMOVIDO
      .query(`
        INSERT INTO dbo.Historial_Contestacoes (
          id_chamado, Id_usuario, Justificativa, DataContestacao
          -- Tipo REMOVIDO
        )
        OUTPUT INSERTED.Id
        VALUES (
          @idChamado, @idUsuario, @justificativa, GETDATE()
          -- @tipo REMOVIDO
        )
      `);

    const novaContestacaoId = result.recordset[0].Id;

    console.log(`✅ Contestação ${novaContestacaoId} criada com sucesso`);

    res.status(201).json({
      success: true,
      message: 'Contestação criada com sucesso',
      id: novaContestacaoId
    });

  } catch (error) {
    console.error('❌ Erro ao criar contestação:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao criar contestação',
      error: error.message
    });
  }
});

// ========================================
// ATUALIZAR CONTESTAÇÃO
// ========================================
router.put('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    // O campo 'tipo' foi mantido apenas para checar se ele está sendo enviado, mas a lógica de atualização foi removida.
    const { justificativa, tipo } = req.body; 

    // Validação corrigida (checando apenas justificativa, se for o único campo editável)
    if (!justificativa) {
      // Aviso: Se a coluna 'Tipo' foi removida, a variável 'tipo' deve ser removida da desestruturação do req.body acima.
      return res.status(400).json({
        success: false,
        message: 'Nenhum campo válido para atualizar (Apenas Justificativa permitida)'
      });
    }

    const pool = await getConnection();
    
    let updates = [];
    const request = pool.request().input('id', sql.Int, id);

    if (justificativa) {
      updates.push('Justificativa = @justificativa');
      request.input('justificativa', sql.NVarChar(1000), justificativa);
    }
    
    // ❌ LÓGICA DE ATUALIZAÇÃO DO 'TIPO' REMOVIDA
    /*
    if (tipo) {
      updates.push('Tipo = @tipo');
      request.input('tipo', sql.NVarChar(20), tipo);
    }
    */
    
    // Se não há atualizações válidas, retorna erro
    if (updates.length === 0) {
      return res.status(400).json({
        success: false,
        message: 'Nenhum campo válido para atualizar.'
      });
    }


    const query = `
      UPDATE dbo.Historial_Contestacoes 
      SET ${updates.join(', ')}
      WHERE Id = @id
    `;
    
    const result = await request.query(query);

    if (result.rowsAffected[0] === 0) {
      return res.status(404).json({
        success: false,
        message: 'Contestação não encontrada'
      });
    }

    console.log(`✅ Contestação ${id} atualizada com sucesso`);

    res.json({
      success: true,
      message: 'Contestação atualizada com sucesso'
    });

  } catch (error) {
    console.error('❌ Erro ao atualizar contestação:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao atualizar contestação',
      error: error.message
    });
  }
});

// ========================================
// DELETAR CONTESTAÇÃO (NÃO ALTERADO)
// ========================================
router.delete('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    console.log(`🗑️ Deletando contestação ID: ${id}`);
    
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('id', sql.Int, id)
      .query('DELETE FROM dbo.Historial_Contestacoes WHERE Id = @id');

    if (result.rowsAffected[0] === 0) {
      return res.status(404).json({
        success: false,
        message: 'Contestação não encontrada'
      });
    }

    console.log(`✅ Contestação ${id} deletada com sucesso`);

    res.json({
      success: true,
      message: 'Contestação deletada com sucesso'
    });

  } catch (error) {
    console.error('❌ Erro ao deletar contestação:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao deletar contestação',
      error: error.message
    });
  }
});

// ========================================
// LISTAR TODAS AS CONTESTAÇÕES (ADMIN)
// ========================================
router.get('/', async (req, res) => {
  try {
    console.log('📡 Buscando todas as contestações...');
    
    const pool = await getConnection();
    
    const result = await pool.request().query(`
      SELECT 
        hc.Id as id,
        hc.id_chamado as idChamado,
        hc.Id_usuario as idUsuario,
        hc.Justificativa,
        hc.DataContestacao,
        -- hc.Tipo, <--- REMOVIDO
        u.nome as usuarioNome,
        e.E_mail as usuarioEmail,
        c.titulo as chamadoTitulo,
        c.categoria as chamadoCategoria
      FROM dbo.Historial_Contestacoes hc
      LEFT JOIN dbo.Usuario u ON hc.Id_usuario = u.Id_usuario
      LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
      LEFT JOIN dbo.chamados c ON hc.id_chamado = c.id_chamado
      ORDER BY hc.DataContestacao DESC
    `);

    console.log(`✅ ${result.recordset.length} contestação(ões) encontrada(s)`);

    res.json({
      success: true,
      total: result.recordset.length,
      contestacoes: result.recordset
    });

  } catch (error) {
    console.error('❌ Erro ao buscar contestações:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar contestações',
      error: error.message
    });
  }
});

module.exports = router;