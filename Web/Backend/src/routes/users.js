const express = require('express');
const router = express.Router();
const UserController = require('../controllers/UserController');
const { requireAuth, requireAdmin } = require('../middleware/auth');

// Todas as rotas de usuários requerem autenticação
router.use(requireAuth);

// Rotas que requerem permissão de admin
router.get('/', requireAdmin, UserController.index);
router.get('/:id', requireAdmin, UserController.show);
router.post('/', requireAdmin, UserController.create);
router.put('/:id', requireAdmin, UserController.update);
router.patch('/:id/deactivate', requireAdmin, UserController.deactivate);
router.patch('/:id/activate', requireAdmin, UserController.activate);

module.exports = router;
