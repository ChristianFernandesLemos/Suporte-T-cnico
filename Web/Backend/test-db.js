// Script para testar a conexão com o banco de dados
const { testConnection } = require('./src/db');

console.log('='.repeat(50));
console.log('🧪 TESTE DE CONEXÃO COM O BANCO DE DADOS');
console.log('='.repeat(50));

testConnection()
  .then((success) => {
    if (success) {
      console.log('\n✅ Conexão estabelecida com sucesso!');
      console.log('Você pode começar a usar o banco de dados.');
    } else {
      console.log('\n❌ Falha na conexão.');
      console.log('Verifique suas credenciais no arquivo .env');
    }
    process.exit(success ? 0 : 1);
  })
  .catch((err) => {
    console.error('\n❌ Erro inesperado:', err);
    process.exit(1);
  });
