console.log("Iniciando conexão...");

const sql = require('mssql');

const config = {
  server: 'interfix-db-piml.database.windows.net',
  database: 'db-interfix1',
  user: 'InterfixADM',
  password: '@Pim3Semestre',
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

async function testConnection() {
  try {
    console.log('Tentando conectar...');
    await sql.connect(config);
    console.log('✅ Conectado!');
    
    // Teste simples
    const result = await sql.query`SELECT 1 as test`;
    console.log('Teste de query:', result.recordset);
    
    await sql.close();
  } catch (err) {
    console.error('❌ Erro detalhado:');
    console.error('Mensagem:', err.message);
    console.error('Código:', err.code);
  }
}

testConnection();