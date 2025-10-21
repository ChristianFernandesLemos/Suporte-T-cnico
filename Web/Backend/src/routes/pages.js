const express = require('express');
const router = express.Router();
const path = require('path');

// Diretório das views
const viewsPath = path.join(__dirname, '../views');

// Página de login
router.get('/login', (req, res) => {
  res.sendFile(path.join(viewsPath, 'login.html'));
});

// Página do menu principal
router.get('/menu', (req, res) => {
  res.sendFile(path.join(viewsPath, 'MenuPrincipal.html'));
});

// Página de registrar chamado
router.get('/registrar-chamado', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Registrar-Chamados.html'));
});

// Página de visualizar chamados
router.get('/chamados', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Afeta-Chamados.html'));
});

// Página de editar chamado
router.get('/editar-chamado', (req, res) => {
  res.sendFile(path.join(viewsPath, 'editar-chamado.html'));
});

// Página de concluir chamado
router.get('/concluir-chamado', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Concluir-Chamados.html'));
});

// Página de prioridade de chamados
router.get('/prioridade-chamados', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Prioridade-Chamados.html'));
});

// Página de adicionar usuário
router.get('/adicionar-usuario', (req, res) => {
  res.sendFile(path.join(viewsPath, 'adicionar-usuario.html'));
});

module.exports = router;
