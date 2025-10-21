const express = require('express');
const router = express.Router();
const AuthController = require('../controllers/AuthController');
const { requireAuth } = require('../middleware/auth');

// Rotas públicas (não requerem autenticação)
router.post('/login', AuthController.login);
router.post('/forgot-password', AuthController.forgotPassword);

// Rotas protegidas (requerem autenticação)
router.post('/logout', requireAuth, AuthController.logout);
router.get('/verify', requireAuth, AuthController.verifySession);
router.post('/change-password', requireAuth, AuthController.changePassword);

module.exports = router;
