const Usuarios = require('./User');
const Chamados = require('./Chamados');

class Funcionario extends Usuarios {
    constructor() {
        super();
        this.Nivel_Acesso = 1; // Funcionário sempre tem nível 1
    }

    // Registrar novo chamado
    async registrarChamado(categoria, prioridade, descricao, atribuido = null) {
        try {
            const chamado = new Chamados();
            chamado.id_Usuario = this.id_Usuario; // Quem criou o chamado
            chamado.categoria = categoria;
            chamado.prioridade = prioridade;
            chamado.descricao = descricao;
            chamado.id_Tecnico = atribuido; // Técnico atribuído (pode ser null)
            chamado.status = 'aberto';
            chamado.data_Abertura = new Date();
            
            // Salvar chamado no banco (você precisará implementar este método)
            await chamado.salvar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao registrar chamado:', error);
            throw error;
        }
    }

    // Visualizar chamados criados por este funcionário
    async visualizarChamados() {
        try {
            // Busca apenas chamados deste funcionário
            const chamados = await Chamados.findByUsuario(this.id_Usuario);
            return chamados;
        } catch (error) {
            console.error('Erro ao visualizar chamados:', error);
            throw error;
        }
    }

    // Visualizar chamado específico
    async visualizarChamado(idChamado) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            // Verifica se o chamado pertence a este funcionário
            if (chamado && chamado.id_Usuario !== this.id_Usuario) {
                throw new Error('Você não tem permissão para visualizar este chamado');
            }
            
            return chamado;
        } catch (error) {
            console.error('Erro ao visualizar chamado:', error);
            throw error;
        }
    }

    // Adicionar comentário/contestação ao chamado
    async comentarChamado(idChamado, comentario) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado pertence a este funcionário
            if (chamado.id_Usuario !== this.id_Usuario) {
                throw new Error('Você não tem permissão para comentar neste chamado');
            }

            // Adiciona comentário (você precisará implementar este método)
            await chamado.adicionarComentario(this.id_Usuario, this.Nome, comentario);
            
            return chamado;
        } catch (error) {
            console.error('Erro ao comentar chamado:', error);
            throw error;
        }
    }

    // Cancelar chamado (somente se ainda não foi atribuído)
    async cancelarChamado(idChamado, motivo) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado pertence a este funcionário
            if (chamado.id_Usuario !== this.id_Usuario) {
                throw new Error('Você não tem permissão para cancelar este chamado');
            }

            // Só pode cancelar se ainda não foi atribuído ou está em andamento
            if (chamado.status === 'concluido') {
                throw new Error('Não é possível cancelar um chamado já concluído');
            }

            chamado.status = 'cancelado';
            chamado.contestacoes = motivo;
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao cancelar chamado:', error);
            throw error;
        }
    }

    // Reabrir chamado
    async reabrirChamado(idChamado, motivo) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado pertence a este funcionário
            if (chamado.id_Usuario !== this.id_Usuario) {
                throw new Error('Você não tem permissão para reabrir este chamado');
            }

            // Só pode reabrir se foi concluído
            if (chamado.status !== 'concluido') {
                throw new Error('Só é possível reabrir chamados concluídos');
            }

            chamado.status = 'reaberto';
            await chamado.adicionarComentario(this.id_Usuario, this.Nome, `Chamado reaberto. Motivo: ${motivo}`);
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao reabrir chamado:', error);
            throw error;
        }
    }

    // Verifica se é funcionário
    isFuncionario() {
        return this.Nivel_Acesso === 1;
    }
}

module.exports = Funcionario;
