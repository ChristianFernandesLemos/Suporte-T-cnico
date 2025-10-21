const Usuarios = require('./Usuarios');
const Funcionario = require('./Funcionario');
const Tecnico = require('./Tecnico');

class Adm extends Usuarios {
    constructor() {
        super();
        this.Nivel_Acesso = 3; // Admin sempre tem nível 3
    }

    // Adiciona novo usuário (cria instância da classe correta)
    async adicionarUsuario(nome, cpf, email, senha, nivelAcesso) {
        let novoUsuario;

        if (nivelAcesso === 1) {
            novoUsuario = new Funcionario();
        } else if (nivelAcesso === 2) {
            novoUsuario = new Tecnico();
        } else if (nivelAcesso === 3) {
            novoUsuario = new Adm();
        } else {
            throw new Error('Nível de acesso inválido');
        }

        novoUsuario.Nome = nome;
        novoUsuario.Cpf = cpf;
        novoUsuario.E_mail = email;
        novoUsuario.Senha = senha;
        novoUsuario.Nivel_Acesso = nivelAcesso;

        await novoUsuario.salvar();
        return novoUsuario;
    }

    // Remove usuário (desativa)
    async removerUsuario(idUsuario) {
        const usuario = await Usuarios.findById(idUsuario);
        
        if (!usuario) {
            throw new Error('Usuário não encontrado');
        }

        // Não permite remover a si mesmo
        if (usuario.id_Usuario === this.id_Usuario) {
            throw new Error('Você não pode remover sua própria conta');
        }

        await usuario.desativar();
        return true;
    }

    // Altera nível de acesso de um usuário
    async alterarNivelAcesso(idUsuario, novoNivelAcesso) {
        const usuario = await Usuarios.findById(idUsuario);
        
        if (!usuario) {
            throw new Error('Usuário não encontrado');
        }

        // Não permite alterar próprio nível
        if (usuario.id_Usuario === this.id_Usuario) {
            throw new Error('Você não pode alterar seu próprio nível de acesso');
        }

        usuario.Nivel_Acesso = novoNivelAcesso;
        await usuario.atualizar();
        return usuario;
    }

    // Lista todos os usuários
    async listarUsuarios() {
        return await Usuarios.findAll();
    }

    // Busca usuário específico
    async buscarUsuario(idUsuario) {
        return await Usuarios.findById(idUsuario);
    }

    // Reativa usuário desativado
    async reativarUsuario(idUsuario) {
        const usuario = await Usuarios.findById(idUsuario);
        
        if (!usuario) {
            throw new Error('Usuário não encontrado');
        }

        await usuario.reativar();
        return usuario;
    }

    // Reseta senha de um usuário
    async resetarSenha(idUsuario, novaSenha) {
        const usuario = await Usuarios.findById(idUsuario);
        
        if (!usuario) {
            throw new Error('Usuário não encontrado');
        }

        await usuario.alterarSenha(novaSenha);
        return true;
    }

    // Verifica se é admin
    isAdmin() {
        return this.Nivel_Acesso === 3;
    }
}

module.exports = Adm;
