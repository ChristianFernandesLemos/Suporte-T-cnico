const express = require('express');
const router = express.Router();
const path = require('path');

// Diretório das views
const viewsPath = path.join(__dirname, '../views');

// Página de login
router.get('/login', (req, res) => {
  res.sendFile(path.join(viewsPath, 'Index.html'));
});

// Página Esqueci a senha
router.get('/esquecisenha', (req, res) => {
  res.sendFile(path.join(viewsPath, 'esquecisenha.html'));
});

// Página do menu principal
router.get('/menu', (req, res) => {
  res.sendFile(path.join(viewsPath, 'MenuPrincipal.html'));
});

// ===========================================
// ROTAS DE REGISTRO
// ===========================================

// Página de registrar chamado (Passo 1)
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


// ===========================================
// ROTAS DE CHAMADOS
// ===========================================

// Página de visualizar chamados (Lista)
router.get('/chamados', (req, res) => {
  // Assumindo que o nome do arquivo de lista é 'lista-chamados.html' ou 'VizualizarChamados.html'
  // Usamos 'VizualizarChamados.html' baseado na estrutura do seu 'server.js'
  res.sendFile(path.join(viewsPath, 'lista-chamados (1).html'));
});

// 🌟 CORREÇÃO: Rota para a página de Detalhes de um chamado
router.get('/detalhes', (req, res) => {
  res.sendFile(path.join(viewsPath, 'detalhes-chamado.html'));
});

// 🌟 CORREÇÃO: Rota para a página de Edição de um chamado
router.get('/editar', (req, res) => {
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


// ===========================================
// ROTAS DE ADMINISTRAÇÃO/USUÁRIOS
// ===========================================

// Página de adicionar usuário
router.get('/adicionar-usuario', (req, res) => {
  res.sendFile(path.join(viewsPath, 'adicionar-usuario.html'));
});


module.exports = router;