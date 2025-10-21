const Usuarios = require('../models/User');
const crypto = require('crypto');

// Função para hash de senha
function hashPassword(password) {
  return crypto.createHash('sha256').update(password).digest('hex');
}

class UserController {
  // Lista todos os usuários
  static async index(req, res) {
    try {
      const users = await User.findAll();

      res.json({
        success: true,
        users: users.map(user => ({
          id: user.id,
          nome: user.nome,
          email: user.email,
          tipo_usuario: user.tipo_usuario,
          ativo: user.ativo,
          data_criacao: user.data_criacao
        }))
      });

    } catch (error) {
      console.error('Erro ao listar usuários:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao listar usuários.'
      });
    }
  }

  // Busca usuário por ID
  static async show(req, res) {
    try {
      const { id } = req.params;

      const user = await User.findById(id);

      if (!user) {
        return res.status(404).json({
          success: false,
          message: 'Usuário não encontrado.'
        });
      }

      res.json({
        success: true,
        user: {
          id: user.id,
          nome: user.nome,
          email: user.email,
          tipo_usuario: user.tipo_usuario,
          ativo: user.ativo,
          data_criacao: user.data_criacao
        }
      });

    } catch (error) {
      console.error('Erro ao buscar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao buscar usuário.'
      });
    }
  }

  // Cria novo usuário
  static async create(req, res) {
    try {
      const { nome, email, senha, tipo_usuario } = req.body;

      // Validações
      if (!nome || !email || !senha) {
        return res.status(400).json({
          success: false,
          message: 'Nome, email e senha são obrigatórios.'
        });
      }

      if (senha.length < 6) {
        return res.status(400).json({
          success: false,
          message: 'A senha deve ter no mínimo 6 caracteres.'
        });
      }

      // Verifica se email já existe
      const existingUser = await User.findByEmail(email);
      
      if (existingUser) {
        return res.status(400).json({
          success: false,
          message: 'Email já cadastrado.'
        });
      }

      // Cria usuário
      const senhaHash = hashPassword(senha);
      
      const newUser = await User.create({
        nome,
        email,
        senha: senhaHash,
        tipo_usuario: tipo_usuario || 'tecnico'
      });

      // Remove senha do retorno
      delete newUser.senha;

      res.status(201).json({
        success: true,
        message: 'Usuário criado com sucesso!',
        user: newUser
      });

    } catch (error) {
      console.error('Erro ao criar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao criar usuário.'
      });
    }
  }

  // Atualiza usuário
  static async update(req, res) {
    try {
      const { id } = req.params;
      const { nome, email, tipo_usuario } = req.body;

      // Verifica se usuário existe
      const user = await User.findById(id);
      
      if (!user) {
        return res.status(404).json({
          success: false,
          message: 'Usuário não encontrado.'
        });
      }

      // Validações
      if (!nome || !email) {
        return res.status(400).json({
          success: false,
          message: 'Nome e email são obrigatórios.'
        });
      }

      // Verifica se email já existe em outro usuário
      if (email !== user.email) {
        const existingUser = await User.findByEmail(email);
        
        if (existingUser && existingUser.id !== parseInt(id)) {
          return res.status(400).json({
            success: false,
            message: 'Email já cadastrado para outro usuário.'
          });
        }
      }

      // Atualiza usuário
      const updatedUser = await User.update(id, {
        nome,
        email,
        tipo_usuario: tipo_usuario || user.tipo_usuario
      });

      res.json({
        success: true,
        message: 'Usuário atualizado com sucesso!',
        user: updatedUser
      });

    } catch (error) {
      console.error('Erro ao atualizar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao atualizar usuário.'
      });
    }
  }

  // Desativa usuário
  static async deactivate(req, res) {
    try {
      const { id } = req.params;

      // Verifica se usuário existe
      const user = await User.findById(id);
      
      if (!user) {
        return res.status(404).json({
          success: false,
          message: 'Usuário não encontrado.'
        });
      }

      // Não permite desativar a si mesmo
      if (parseInt(id) === req.user.userId) {
        return res.status(400).json({
          success: false,
          message: 'Você não pode desativar sua própria conta.'
        });
      }

      await User.deactivate(id);

      res.json({
        success: true,
        message: 'Usuário desativado com sucesso!'
      });

    } catch (error) {
      console.error('Erro ao desativar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao desativar usuário.'
      });
    }
  }

  // Reativa usuário
  static async activate(req, res) {
    try {
      const { id } = req.params;

      await User.activate(id);

      res.json({
        success: true,
        message: 'Usuário reativado com sucesso!'
      });

    } catch (error) {
      console.error('Erro ao reativar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro ao reativar usuário.'
      });
    }
  }
}

module.exports = UserController;
