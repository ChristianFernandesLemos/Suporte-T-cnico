// Carrega as variáveis de ambiente
require('dotenv').config();

const sql = require('mssql');

// Configuração do banco usando variáveis de ambiente
const config = {
  server: process.env.DB_SERVER,
  database: process.env.DB_NAME,
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  options: {
    encrypt: true, // Obrigatório para Azure
    trustServerCertificate: false,
    enableArithAbort: true
  },
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000
  }
};

// Pool de conexão (reutilizável)
let poolPromise;

const getConnection = async () => {
  try {
    if (!poolPromise) {
      poolPromise = sql.connect(config);
    }
    return await poolPromise;
  } catch (err) {
    console.error('❌ Erro ao conectar no banco:', err.message);
    throw err;
  }
};

// Função para testar a conexão
async function testConnection() {
  try {
    console.log('🔄 Tentando conectar ao banco de dados...');
    console.log('📡 Server:', config.server);
    console.log('🗄️  Database:', config.database);
    
    const pool = await getConnection();
    console.log('✅ Conectado ao SQL Server!');
    
    // Teste simples
    const result = await pool.request().query('SELECT 1 as test, GETDATE() as date');
    console.log('✅ Teste de query bem-sucedido:', result.recordset);
    
    return true;
  } catch (err) {
    console.error('❌ Erro na conexão:');
    console.error('   Mensagem:', err.message);
    console.error('   Código:', err.code);
    console.error('\n💡 Verifique:');
    console.error('   - Se as credenciais no .env estão corretas');
    console.error('   - Se o firewall do Azure permite seu IP');
    console.error('   - Se o servidor está disponível');
    return false;
  }
}

// Exporta as funções
module.exports = {
  sql,
  getConnection,
  testConnection
};

// Se executar diretamente este arquivo, testa a conexão
if (require.main === module) {
  testConnection().then(() => {
    process.exit(0);
  });
}
