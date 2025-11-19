// src/routes/chamados.js - Rotas para gerenciamento de chamados
// ADAPTADO PARA A ESTRUTURA REAL DO BANCO DE DADOS
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
    const pool = await getConnection();
    
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
        c.Contestacoes_Codigo as contestacaoId,
        c.Tecnico_Atribuido as tecnicoId,
        r.id_usuario as usuarioId
      FROM dbo.chamados c
      LEFT JOIN dbo.registra r ON c.id_chamado = r.id_chamado
      ORDER BY c.Data_Registro DESC
    `);

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
    const pool = await getConnection();
    
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
          c.Contestacoes_Codigo as contestacaoId,
          c.Tecnico_Atribuido as tecnicoId,
          r.id_usuario as usuarioId,
          r.DataRegistro as dataRegistroUsuario
        FROM dbo.chamados c
        LEFT JOIN dbo.registra r ON c.id_chamado = r.id_chamado
        WHERE c.id_chamado = @id
      `);

    if (result.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        message: 'Chamado não encontrado'
      });
    }

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
// CRIAR NOVO CHAMADO
// ========================================
router.post('/', async (req, res) => {
  try {
    const {
      categoria,
      prioridade,
      descricao,
      afetadoId,
      usuarioId
    } = req.body;

    // Validação básica
    if (!categoria || !descricao || !usuarioId) {
      return res.status(400).json({
        success: false,
        message: 'Categoria, descrição e usuário são obrigatórios'
      });
    }

    // Define prioridade padrão se não fornecida
    const prioridadeFinal = prioridade || PRIORIDADE.MEDIA;

    const pool = await getConnection();
    
    // Inicia transação
    const transaction = pool.transaction();
    await transaction.begin();

    try {
      // Insere o chamado
      const resultChamado = await transaction.request()
        .input('categoria', sql.NVarChar(20), categoria)
        .input('prioridade', sql.Int, prioridadeFinal)
        .input('descricao', sql.NVarChar(1000), descricao)
        .input('status', sql.Int, STATUS.ABERTO)
        .input('afetadoId', sql.Int, afetadoId)
        .query(`
          INSERT INTO dbo.chamados (
            categoria, prioridade, descricao, Status, Afetado, Data_Registro
          )
          OUTPUT INSERTED.id_chamado
          VALUES (
            @categoria, @prioridade, @descricao, @status, @afetadoId, GETDATE()
          )
        `);

      const novoChamadoId = resultChamado.recordset[0].id_chamado;

      // Insere na tabela registra
      await transaction.request()
        .input('usuarioId', sql.Int, usuarioId)
        .input('chamadoId', sql.Int, novoChamadoId)
        .query(`
          INSERT INTO dbo.registra (id_usuario, id_chamado, DataRegistro)
          VALUES (@usuarioId, @chamadoId, GETDATE())
        `);

      await transaction.commit();

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
          r.id_usuario as usuarioId
        FROM dbo.chamados c
        LEFT JOIN dbo.registra r ON c.id_chamado = r.id_chamado
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
// FILTRAR CHAMADOS POR PRIORIDADE
// ========================================
router.get('/filtrar/prioridade/:prioridade', async (req, res) => {
  try {
    const { prioridade } = req.params;
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('prioridade', sql.Int, parseInt(prioridade))
      .query(`
        SELECT 
          c.id_chamado as id,
          c.categoria,
          c.prioridade,
          c.descricao,
          c.Status as status,
          c.Data_Registro as dataAbertura,
          r.id_usuario as usuarioId
        FROM dbo.chamados c
        LEFT JOIN dbo.registra r ON c.id_chamado = r.id_chamado
        WHERE c.prioridade = @prioridade
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
// BUSCAR CHAMADOS POR USUÁRIO
// ========================================
router.get('/usuario/:usuarioId', async (req, res) => {
  try {
    const { usuarioId } = req.params;
    const pool = await getConnection();
    
    const result = await pool.request()
      .input('usuarioId', sql.Int, usuarioId)
      .query(`
        SELECT 
          c.id_chamado as id,
          c.categoria,
          c.prioridade,
          c.descricao,
          c.Status as status,
          c.Data_Registro as dataAbertura,
          r.DataRegistro as dataRegistroUsuario
        FROM dbo.chamados c
        INNER JOIN dbo.registra r ON c.id_chamado = r.id_chamado
        WHERE r.id_usuario = @usuarioId
        ORDER BY c.Data_Registro DESC
      `);

    res.json({
      success: true,
      total: result.recordset.length,
      chamados: result.recordset
    });

  } catch (error) {
    console.error('❌ Erro ao buscar chamados do usuário:', error);
    res.status(500).json({
      success: false,
      message: 'Erro ao buscar chamados do usuário',
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

// Exporta constantes também para uso em outros módulos
module.exports = router;
module.exports.STATUS = STATUS;
module.exports.PRIORIDADE = PRIORIDADE;