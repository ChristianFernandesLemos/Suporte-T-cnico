require('dotenv').config();
const sql = require('mssql');

const config = {
  server: process.env.DB_SERVER,
  database: process.env.DB_NAME,
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  options: {
    encrypt: true,
    trustServerCertificate: false,
    enableArithAbort: true
  },
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000
  }
};

// Pool de conexões (reutilizável)
let pool = null;

async function getConnection() {
  try {
    if (!pool) {
      console.log('📡 Conectando ao banco de dados...');
      pool = await sql.connect(config);
      console.log('✅ Conectado ao SQL Server!');
    }
    return pool;
  } catch (err) {
    console.error('❌ Erro ao conectar ao banco:', err.message);
    throw err;
  }
}

// Função para testar a conexão
async function testConnection() {
  try {
    console.log('🔍 Testando conexão...');
    const pool = await getConnection();
    const result = await pool.request().query('SELECT 1 as test');
    console.log('✅ Teste de query:', result.recordset);
    return true;
  } catch (err) {
    console.error('❌ Erro no teste de conexão:');
    console.error('Mensagem:', err.message);
    console.error('Código:', err.code);
    return false;
  }
}

// Fecha a conexão quando a aplicação encerrar
process.on('SIGINT', async () => {
  if (pool) {
    await pool.close();
    console.log('🔌 Conexão com o banco fechada');
  }
  process.exit(0);
});

module.exports = {
  getConnection,
  testConnection,
  sql
};
