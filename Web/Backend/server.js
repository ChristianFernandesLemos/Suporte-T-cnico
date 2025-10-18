// Carrega as variáveis de ambiente
require('dotenv').config();

// Importa o Express
const express = require('express');
const app = express();

// Middleware para interpretar JSON
app.use(express.json());

// Rota raiz
app.get('/', (req, res) => {
  res.send('Bem-vindo ao Interfix! Acesse /api/ para usar a API');
});

// Rota principal da API
app.get('/api/', (req, res) => {
  res.send('API funcionando!');
});

// Exemplo de rota de usuários
let users = [];

app.get('/api/users', (req, res) => {
  res.json(users);
});

app.post('/api/users', (req, res) => {
  const newUser = {
    id: users.length + 1,
    nome: req.body.nome,
    email: req.body.email,
  };
  users.push(newUser);
  res.status(201).json(newUser);
});

// Inicia o servidor
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`🚀 Servidor rodando na porta ${PORT}`);
  console.log(`📍 Acesse: http://localhost:${PORT}`);
  console.log(`📍 API: http://localhost:${PORT}/api/`);
});