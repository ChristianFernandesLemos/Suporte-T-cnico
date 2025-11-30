// src/routes/chamados.js - Rotas para gerenciamento de chamados
// CORRIGIDO para buscar nome e email do usuário através do campo Afetado
const express = require('express');
const router = express.Router();
const { getConnection, sql } = require('../../db');

// ========================================
// MAPEAMENTOS DE STATUS E PRIORIDADE
// ========================================
const STATUS = {
  ABERTO: 1,
  EM_ANDAMENTO: 2,
  RESOLVIDO: 3,
  FECHADO: 4,
  CANCELADO: 5
};

const PRIORIDADE = {
  BAIXA: 1,
  MEDIA: 2,
  ALTA: 3,
  CRITICA: 4
};

// ========================================
// LISTAR TODOS OS CHAMADOS
// ========================================
router.get('/', async (req, res) => {
  try {
    console.log('📡 Buscando todos os chamados do banco...');
    
    const pool = await getConnection();
    
    // ⭐ QUERY CORRIGIDA - Agora busca nome e email do usuário afetado
    const result = await pool.request().query(`
      SELECT 
        c.id_chamado as id,
        c.categoria,
        c.prioridade,
        c.descricao,
        c.Status as status,
        c.Solucao as solucao,
        c.Data_Registro as dataAbertura,
        c.Data_Resolucao as dataResolucao,
        c.Afetado as afetadoId,
        c.Tecnico_Atribuido as tecnicoId,
        c.titulo,
        u.nome as usuarioNome,
        e.E_mail as usuarioEmail
      FROM dbo.chamados c
      LEFT JOIN dbo.Usuario u ON c.Afetado = u.Id_usuario
      LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
      ORDER BY c.Data_Registro DESC
    `);

    console.log(`✅ ${result.recordset.length} chamados encontrados no banco`);
    
    // Log dos dados para debug
    if (result.recordset.length > 0) {
      console.log('📊 Exemplo do primeiro chamado:', result.recordset[0]);
    }

    res.json({
      success: true,
      total: result.recordset.length,
      chamados: result.recordset
    });

  } catch (error) {
    console.error('❌ Erro ao buscar chamados:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar chamados',
      error: error.message
    });
  }
});

// ========================================
// BUSCAR CHAMADO POR ID
// ========================================
router.get('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    console.log(`📡 Buscando chamado ID: ${id}`);
    
    const pool = await getConnection();
    
    // ⭐ QUERY CORRIGIDA - Busca nome e email através do Afetado
    const result = await pool.request()
      .input('id', sql.Int, id)
      .query(`
        SELECT 
          c.id_chamado as id,
          c.categoria,
          c.prioridade,
          c.descricao,
          c.Status as status,
          c.Solucao as solucao,
          c.Data_Registro as dataAbertura,
          c.Data_Resolucao as dataResolucao,
          c.Afetado as afetadoId,
          c.Tecnico_Atribuido as tecnicoId,
          c.titulo,
          u.nome as usuarioNome,
          e.E_mail as usuarioEmail
        FROM dbo.chamados c
        LEFT JOIN dbo.Usuario u ON c.Afetado = u.Id_usuario
        LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE c.id_chamado = @id
      `);

    if (result.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Chamado não encontrado'
      });
    }

    console.log('✅ Chamado encontrado:', result.recordset[0]);

    res.json({
      success: true,
      chamado: result.recordset[0]
    });

  } catch (error) {
    console.error('❌ Erro ao buscar chamado:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar chamado',
      error: error.message
    });
  }
});

// ========================================
// ✅ CRIAR NOVO CHAMADO - CORRIGIDO
// ========================================
router.post('/', async (req, res) => {
  try {
    const {
      categoria,
      prioridade,
      descricao,
      afetadoId,
      usuarioId,
      titulo
    } = req.body;

    // Validação básica
    if (!categoria || !descricao || !afetadoId) {
      return res.status(400).json({
        success: false,
        message: 'Categoria, descrição e afetado são obrigatórios'
      });
    }

    // Define prioridade padrão se não fornecida
    const prioridadeFinal = prioridade || PRIORIDADE.MEDIA;

    const pool = await getConnection();
    
    // Inicia transação
    const transaction = pool.transaction();
    await transaction.begin();

    try {
      // ✅ CORREÇÃO: Remove OUTPUT INSERTED para evitar conflito com triggers
      await transaction.request()
        .input('categoria', sql.NVarChar(20), categoria)
        .input('prioridade', sql.Int, prioridadeFinal)
        .input('descricao', sql.NVarChar(1000), descricao)
        .input('status', sql.Int, STATUS.ABERTO)
        .input('afetadoId', sql.Int, afetadoId)
        .input('titulo', sql.NVarChar(200), titulo)
        .query(`
          INSERT INTO dbo.chamados (
            categoria, prioridade, descricao, Status, Afetado, Data_Registro, titulo
          )
          VALUES (
            @categoria, @prioridade, @descricao, @status, @afetadoId, GETDATE(), @titulo
          )
        `);

      // ✅ CORREÇÃO: Busca o ID inserido usando SCOPE_IDENTITY()
      const resultId = await transaction.request().query(`
        SELECT CAST(SCOPE_IDENTITY() AS INT) as id_chamado
      `);

      const novoChamadoId = resultId.recordset[0].id_chamado;

      // Se usuarioId foi fornecido e é diferente de afetadoId, insere na tabela registra
      if (usuarioId && usuarioId !== afetadoId) {
        await transaction.request()
          .input('usuarioId', sql.Int, usuarioId)
          .input('chamadoId', sql.Int, novoChamadoId)
          .query(`
            INSERT INTO dbo.registra (id_usuario, id_chamado, DataRegistro)
            VALUES (@usuarioId, @chamadoId, GETDATE())
          `);
      }

      await transaction.commit();

      console.log(`✅ Chamado ${novoChamadoId} criado com sucesso`);

      res.status(201).json({
        success: true,
        message: 'Chamado criado com sucesso',
        id: novoChamadoId
      });

    } catch (error) {
      await transaction.rollback();
      throw error;
    }

  } catch (error) {
    console.error('❌ Erro ao criar chamado:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao criar chamado',
      error: error.message
    });
  }
});

// ========================================
// ATUALIZAR CHAMADO
// ========================================
router.put('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    const {
      titulo,
      categoria,
      prioridade,
      status,
      descricao,
      solucao,
      usuarioNome,
      usuarioEmail
    } = req.body;

    console.log(`📝 Atualizando chamado ${id}:`, req.body);

    const pool = await getConnection();
    
    // Monta query dinamicamente
    let updates = [];
    const request = pool.request().input('id', sql.Int, id);

    if (titulo !== undefined) {
      updates.push('titulo = @titulo');
      request.input('titulo', sql.NVarChar(200), titulo);
    }

    if (categoria !== undefined) {
      updates.push('categoria = @categoria');
      request.input('categoria', sql.NVarChar(20), categoria);
    }

    if (prioridade !== undefined) {
      updates.push('prioridade = @prioridade');
      request.input('prioridade', sql.Int, prioridade);
    }

    if (status !== undefined) {
      updates.push('Status = @status');
      request.input('status', sql.Int, status);
      
      // Se está sendo resolvido ou fechado, atualiza data de resolução
      if (status === STATUS.RESOLVIDO || status === STATUS.FECHADO) {
        updates.push('Data_Resolucao = GETDATE()');
      }
    }

    if (descricao !== undefined) {
      updates.push('descricao = @descricao');
      request.input('descricao', sql.NVarChar(1000), descricao);
    }

    if (solucao !== undefined) {
      updates.push('Solucao = @solucao');
      request.input('solucao', sql.NVarChar(1000), solucao);
    }

    if (updates.length === 0) {
      return res.status(400).json({
        success: false,
        message: 'Nenhum campo para atualizar'
      });
    }

    const query = `UPDATE dbo.chamados SET ${updates.join(', ')} WHERE id_chamado = @id`;
    
    await request.query(query);

    console.log(`✅ Chamado ${id} atualizado com sucesso`);

    res.json({
      success: true,
      message: 'Chamado atualizado com sucesso'
    });

  } catch (error) {
    console.error('❌ Erro ao atualizar chamado:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao atualizar chamado',
      error: error.message
    });
  }
});

// ========================================
// ATUALIZAR STATUS DO CHAMADO
// ========================================
router.patch('/:id/status', async (req, res) => {
  try {
    const { id } = req.params;
    const { status, solucao, tecnicoId } = req.body;

    if (!status) {
      return res.status(400).json({
        success: false,
        message: 'Status não fornecido'
      });
    }

    const pool = await getConnection();
    
    let query = `UPDATE dbo.chamados SET Status = @status`;
    const request = pool.request()
      .input('id', sql.Int, id)
      .input('status', sql.Int, status);

    if (solucao) {
      query += `, Solucao = @solucao`;
      request.input('solucao', sql.NVarChar(1000), solucao);
    }

    if (tecnicoId) {
      query += `, Tecnico_Atribuido = @tecnicoId`;
      request.input('tecnicoId', sql.Int, tecnicoId);
    }

    // Se o status é RESOLVIDO ou FECHADO, atualiza Data_Resolucao
    if (status === STATUS.RESOLVIDO || status === STATUS.FECHADO) {
      query += `, Data_Resolucao = GETDATE()`;
    }

    query += ` WHERE id_chamado = @id`;

    await request.query(query);

    console.log(`✅ Status do chamado ${id} atualizado`);

    res.json({
      success: true,
      message: 'Status atualizado com sucesso'
    });

  } catch (error) {
    console.error('❌ Erro ao atualizar status:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao atualizar status',
      error: error.message
    });
  }
});

// ========================================
// FILTRAR CHAMADOS POR STATUS
// ========================================
router.get('/filtrar/status/:status', async (req, res) => {
  try {
    const { status } = req.params;
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('status', sql.Int, parseInt(status))
      .query(`
        SELECT 
          c.id_chamado as id,
          c.categoria,
          c.prioridade,
          c.descricao,
          c.Status as status,
          c.Data_Registro as dataAbertura,
          c.Afetado as afetadoId,
          c.Tecnico_Atribuido as tecnicoId,
          c.titulo,
          u.nome as usuarioNome,
          e.E_mail as usuarioEmail
        FROM dbo.chamados c
        LEFT JOIN dbo.Usuario u ON c.Afetado = u.Id_usuario
        LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE c.Status = @status
        ORDER BY c.Data_Registro DESC
      `);

    res.json({
      success: true,
      total: result.recordset.length,
      chamados: result.recordset
    });

  } catch (error) {
    console.error('❌ Erro ao filtrar chamados:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao filtrar chamados',
      error: error.message
    });
  }
});

// ========================================
// ESTATÍSTICAS DE CHAMADOS
// ========================================
router.get('/stats/resumo', async (req, res) => {
  try {
    const pool = await getConnection();
    
    const result = await pool.request().query(`
      SELECT 
        COUNT(*) as total,
        SUM(CASE WHEN Status = ${STATUS.ABERTO} THEN 1 ELSE 0 END) as abertos,
        SUM(CASE WHEN Status = ${STATUS.EM_ANDAMENTO} THEN 1 ELSE 0 END) as emAndamento,
        SUM(CASE WHEN Status = ${STATUS.RESOLVIDO} THEN 1 ELSE 0 END) as resolvidos,
        SUM(CASE WHEN Status = ${STATUS.FECHADO} THEN 1 ELSE 0 END) as fechados,
        SUM(CASE WHEN Status = ${STATUS.CANCELADO} THEN 1 ELSE 0 END) as cancelados,
        SUM(CASE WHEN prioridade = ${PRIORIDADE.CRITICA} THEN 1 ELSE 0 END) as criticos,
        SUM(CASE WHEN prioridade = ${PRIORIDADE.ALTA} THEN 1 ELSE 0 END) as altos,
        SUM(CASE WHEN prioridade = ${PRIORIDADE.MEDIA} THEN 1 ELSE 0 END) as medios,
        SUM(CASE WHEN prioridade = ${PRIORIDADE.BAIXA} THEN 1 ELSE 0 END) as baixos
      FROM dbo.chamados
    `);

    res.json({
      success: true,
      stats: result.recordset[0]
    });

  } catch (error) {
    console.error('❌ Erro ao buscar estatísticas:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar estatísticas',
      error: error.message
    });
  }
});

// Exporta constantes também
module.exports = router;
module.exports.STATUS = STATUS;
module.exports.PRIORIDADE = PRIORIDADE;