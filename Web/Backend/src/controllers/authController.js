const User = require('../models/User');
const { createSession, destroySession } = require('../middleware/auth');
const crypto = require('crypto');

// Função para hash de senha (em produção use bcrypt)
function hashPassword(password) {
  return crypto.createHash('sha256').update(password).digest('hex');
}

class AuthController {
  // Login
  static async login(req, res) {
    try {
      const { email, senha } = req.body;

      // Validação básica
      if (!email || !senha) {
        return res.status(400).json({
          success: false,
          message: 'Email e senha são obrigatórios.'
        });
      }

      // Busca usuário
      const user = await User.findByEmail(email);

      if (!user) {
        return res.status(401).json({
          success: false,
          message: 'Email ou senha incorretos.'
        });
      }

      // Verifica senha (hash)
      const senhaHash = hashPassword(senha);
      
      if (user.senha !== senhaHash) {
        return res.status(401).json({
          success: false,
          message: 'Email ou senha incorretos.'
        });
      }

      // Cria sessão
      const token = createSession(user);

      // Remove senha do retorno
      delete user.senha;

      res.json({
        success: true,
        message: 'Login realizado com sucesso!',
        token,
        user: {
          id: user.id,
          nome: user.nome,
          email: user.email,
          tipo_usuario: user.tipo_usuario
        }
      });

    } catch (error) {
      console.error('Erro no login:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao realizar login. Tente novamente.'
      });
    }
  }

  // Logout
  static async logout(req, res) {
    try {
      const token = req.headers.authorization?.replace('Bearer ', '');

      if (token) {
        destroySession(token);
      }

      res.json({
        success: true,
        message: 'Logout realizado com sucesso!'
      });

    } catch (error) {
      console.error('Erro no logout:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao realizar logout.'
      });
    }
  }

  // Verifica sessão atual
  static async verifySession(req, res) {
    try {
      // Se chegou aqui, o middleware já validou a sessão
      res.json({
        success: true,
        user: req.user
      });

    } catch (error) {
      console.error('Erro ao verificar sessão:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao verificar sessão.'
      });
    }
  }

  // Alterar senha
  static async changePassword(req, res) {
    try {
      const { senhaAtual, novaSenha, confirmarSenha } = req.body;
      const userId = req.user.userId;

      // Validações
      if (!senhaAtual || !novaSenha || !confirmarSenha) {
        return res.status(400).json({
          success: false,
          message: 'Todos os campos são obrigatórios.'
        });
      }

      if (novaSenha !== confirmarSenha) {
        return res.status(400).json({
          success: false,
          message: 'A nova senha e a confirmação não coincidem.'
        });
      }

      if (novaSenha.length < 6) {
        return res.status(400).json({
          success: false,
          message: 'A senha deve ter no mínimo 6 caracteres.'
        });
      }

      // Busca usuário
      const user = await User.findById(userId);

      if (!user) {
        return res.status(404).json({
          success: false,
          message: 'Usuário não encontrado.'
        });
      }

      // Verifica senha atual
      const senhaAtualHash = hashPassword(senhaAtual);
      
      if (user.senha !== senhaAtualHash) {
        return res.status(401).json({
          success: false,
          message: 'Senha atual incorreta.'
        });
      }

      // Atualiza senha
      const novaSenhaHash = hashPassword(novaSenha);
      await User.updatePassword(userId, novaSenhaHash);

      res.json({
        success: true,
        message: 'Senha alterada com sucesso!'
      });

    } catch (error) {
      console.error('Erro ao alterar senha:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao alterar senha. Tente novamente.'
      });
    }
  }

  // Recuperar senha (placeholder - implementar envio de email)
  static async forgotPassword(req, res) {
    try {
      const { email } = req.body;

      if (!email) {
        return res.status(400).json({
          success: false,
          message: 'Email é obrigatório.'
        });
      }

      // Verifica se usuário existe
      const user = await User.findByEmail(email);

      if (!user) {
        // Por segurança, não revela se o email existe
        return res.json({
          success: true,
          message: 'Se o email existir, um link de recuperação será enviado.'
        });
      }

      // TODO: Implementar envio de email com token de recuperação
      // Por enquanto, apenas retorna sucesso
      
      res.json({
        success: true,
        message: 'Se o email existir, um link de recuperação será enviado.'
      });

    } catch (error) {
      console.error('Erro ao recuperar senha:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao processar recuperação de senha.'
      });
    }
  }
}

module.exports = AuthController;
