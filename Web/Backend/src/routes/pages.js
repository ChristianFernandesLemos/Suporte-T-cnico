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

router.get('/registrar-chamado-p2', (req, res) => {
  res.sendFile(path.join(viewsPath, 'RegistrarChamado-2 Etapa.html'))
});

router.get('/registrar-chamado-p3', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Interrompe o serviço-Chamados.html'))
});

router.get('/registrar-chamado-p4', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Concluir-Chamados.html'))
});

router.get('/prioridadeia', (req, res) => {
  res.sendFile(path.join(viewsPath, 'ConcordaPrioridade.html'))
});

router.get('/contestacao', (req,res) => {
  res.sendFile(path.join(viewsPath, 'Contestação.html'))
})


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

// Página de visualizar chamados
router.get('/chamados', (req, res) => {
  res.sendFile(path.join(viewsPath, 'lista-chamados (1).html'));
});

// Página de editar chamado
router.get('/editar-chamado', (req, res) => {
  res.sendFile(path.join(viewsPath, 'editar-chamado.html'));
});

// Página de visualizar chamados
router.get('/detalhes-chamado', (req, res) => {
  res.sendFile(path.join(viewsPath, 'detalhes-chamado.html'));
});

module.exports = router;
