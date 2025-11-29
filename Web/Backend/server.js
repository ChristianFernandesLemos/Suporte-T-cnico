// Carrega as variáveis de ambiente
require('dotenv').config();

// Importa o Express
const express = require('express');
const path = require('path');
const app = express();

// Importa a conexão do banco
const { testConnection } = require('./db');

// Importa as rotas
const authRoutes = require('./src/routes/auth');
const userRoutes = require('./src/routes/users');
const chamadosRoutes = require('./src/routes/chamados');
const contestacoesRoutes = require('./src/routes/contestacoes'); // ⭐ NOVA LINHA ADICIONADA

const pagesRoutes = require('./src/routes/pages');

// Middleware para interpretar JSON
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Serve arquivos estáticos (CSS, imagens, JS)
app.use('/static', express.static(path.join(__dirname, 'src/views')));

// Middleware de log (desenvolvimento)
if (process.env.NODE_ENV === 'development') {
  app.use((req, res, next) => {
    console.log(`${req.method} ${req.url}`);
    next();
  });
}

// Rota raiz - redireciona para login
app.get('/', (req, res) => {
  res.redirect('/login');
});

// Rotas de páginas HTML
app.use('/', pagesRoutes);

// Rotas da API
app.use('/api/auth', authRoutes);
app.use('/api/users', userRoutes);
app.use('/api/chamados', chamadosRoutes);
app.use('/api/contestacoes', contestacoesRoutes); // ⭐ NOVA LINHA ADICIONADA

// Rota de health check (verifica conexão com banco)
app.get('/api/health', async (req, res) => {
  try {
    const pool = await require('./db').getConnection();
    const result = await pool.request().query('SELECT 1 as test');
    
    res.json({
      status: 'OK',
      database: 'Conectado',
      timestamp: new Date()
    });
  } catch (error) {
    res.status(500).json({
      status: 'ERROR',
      database: 'Desconectado',
      error: error.message,
      timestamp: new Date()
    });
  }
});

// Rota 404
app.use((req, res) => {
  res.status(404).json({
    success: false,
    message: 'Rota não encontrada'
  });
});

// Tratamento de erros global
app.use((err, req, res, next) => {
  console.error('Erro:', err);
  res.status(500).json({
    success: false,
    message: 'Erro interno do servidor',
    error: process.env.NODE_ENV === 'development' ? err.message : undefined
  });
});

// Inicia o servidor
const PORT = process.env.PORT || 3000;

app.listen(PORT, async () => {
  console.log('\n🚀 ================================');
  console.log('   SERVIDOR INTERFIX INICIADO');
  console.log('   ================================');
  console.log(`📍 Servidor: http://localhost:${PORT}`);
  console.log(`📍 Login: http://localhost:${PORT}/login`);
  console.log(`📍 Menu: http://localhost:${PORT}/menu`);
  console.log('================================');
  console.log('   ROTAS DA API');
  console.log('================================');
  console.log(`📍 Auth: http://localhost:${PORT}/api/auth/login`);
  console.log(`📍 Users: http://localhost:${PORT}/api/users`);
  console.log(`📍 Chamados: http://localhost:${PORT}/api/chamados`);
  console.log(`📍 Contestações: http://localhost:${PORT}/api/contestacoes`); // ⭐ NOVA LINHA ADICIONADA
  console.log(`📍 Health: http://localhost:${PORT}/api/health`);
  console.log('================================\n');
  
  // Testa conexão com banco ao iniciar
  console.log('🔄 Testando conexão com banco de dados...\n');
  const connected = await testConnection();
  
  if (connected) {
    console.log('\n✅ Sistema pronto para uso!\n');
  } else {
    console.log('\n⚠️  Sistema iniciado, mas banco de dados offline\n');
    console.log('💡 Verifique as credenciais no arquivo .env\n');
  }
});