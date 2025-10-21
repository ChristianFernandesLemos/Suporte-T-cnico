const Usuarios = require('./Usuarios');
const Chamados = require('./Chamados');

class Tecnico extends Usuarios {
    constructor() {
        super();
        this.Nivel_Acesso = 2; // Técnico sempre tem nível 2
    }

    // Marcar chamado como resolvido
    async marcarChamadoResolvido(idChamado, solucao) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado está atribuído a este técnico
            if (chamado.id_Tecnico !== this.id_Usuario) {
                throw new Error('Este chamado não está atribuído a você');
            }

            chamado.status = 'concluido';
            chamado.data_Conclusao = new Date();
            chamado.solucao = solucao;
            
            await chamado.adicionarComentario(this.id_Usuario, this.Nome, `Chamado concluído. Solução: ${solucao}`);
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao marcar chamado como resolvido:', error);
            throw error;
        }
    }

    // Alterar prioridade do chamado
    async alterarPrioridade(idChamado, novaPrioridade) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado está atribuído a este técnico
            if (chamado.id_Tecnico !== this.id_Usuario) {
                throw new Error('Este chamado não está atribuído a você');
            }

            const prioridadeAnterior = chamado.prioridade;
            chamado.prioridade = novaPrioridade;
            
            await chamado.adicionarComentario(
                this.id_Usuario, 
                this.Nome, 
                `Prioridade alterada de "${prioridadeAnterior}" para "${novaPrioridade}"`
            );
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao alterar prioridade:', error);
            throw error;
        }
    }

    // Alterar status do chamado
    async alterarStatusChamado(idChamado, novoStatus, observacao = null) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado está atribuído a este técnico
            if (chamado.id_Tecnico !== this.id_Usuario) {
                throw new Error('Este chamado não está atribuído a você');
            }

            const statusAnterior = chamado.status;
            chamado.status = novoStatus;
            
            let mensagem = `Status alterado de "${statusAnterior}" para "${novoStatus}"`;
            if (observacao) {
                mensagem += `. Observação: ${observacao}`;
            }
            
            await chamado.adicionarComentario(this.id_Usuario, this.Nome, mensagem);
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao alterar status:', error);
            throw error;
        }
    }

    // Visualizar chamados atribuídos ao técnico
    async visualizarChamadosAtribuidos() {
        try {
            const chamados = await Chamados.findByTecnico(this.id_Usuario);
            return chamados;
        } catch (error) {
            console.error('Erro ao visualizar chamados atribuídos:', error);
            throw error;
        }
    }

    // Visualizar todos os chamados disponíveis (não atribuídos)
    async visualizarChamadosDisponiveis() {
        try {
            const chamados = await Chamados.findDisponiveis();
            return chamados;
        } catch (error) {
            console.error('Erro ao visualizar chamados disponíveis:', error);
            throw error;
        }
    }

    // Assumir um chamado não atribuído
    async assumirChamado(idChamado) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado já está atribuído
            if (chamado.id_Tecnico !== null) {
                throw new Error('Este chamado já está atribuído a outro técnico');
            }

            chamado.id_Tecnico = this.id_Usuario;
            chamado.status = 'em_andamento';
            
            await chamado.adicionarComentario(
                this.id_Usuario, 
                this.Nome, 
                'Chamado assumido pelo técnico'
            );
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao assumir chamado:', error);
            throw error;
        }
    }

    // Adicionar comentário/atualização ao chamado
    async atualizarChamado(idChamado, comentario) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado está atribuído a este técnico
            if (chamado.id_Tecnico !== this.id_Usuario) {
                throw new Error('Este chamado não está atribuído a você');
            }

            await chamado.adicionarComentario(this.id_Usuario, this.Nome, comentario);
            
            return chamado;
        } catch (error) {
            console.error('Erro ao atualizar chamado:', error);
            throw error;
        }
    }

    // Transferir chamado para outro técnico
    async transferirChamado(idChamado, idNovoTecnico, motivo) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado está atribuído a este técnico
            if (chamado.id_Tecnico !== this.id_Usuario) {
                throw new Error('Este chamado não está atribuído a você');
            }

            // Busca o novo técnico
            const novoTecnico = await Usuarios.findById(idNovoTecnico);
            if (!novoTecnico || novoTecnico.Nivel_Acesso !== 2) {
                throw new Error('Técnico de destino inválido');
            }

            chamado.id_Tecnico = idNovoTecnico;
            chamado.status = 'em_andamento';
            
            await chamado.adicionarComentario(
                this.id_Usuario, 
                this.Nome, 
                `Chamado transferido para ${novoTecnico.Nome}. Motivo: ${motivo}`
            );
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao transferir chamado:', error);
            throw error;
        }
    }

    // Solicitar informações adicionais ao usuário
    async solicitarInformacoes(idChamado, pergunta) {
        try {
            const chamado = await Chamados.findById(idChamado);
            
            if (!chamado) {
                throw new Error('Chamado não encontrado');
            }

            // Verifica se o chamado está atribuído a este técnico
            if (chamado.id_Tecnico !== this.id_Usuario) {
                throw new Error('Este chamado não está atribuído a você');
            }

            chamado.status = 'aguardando_usuario';
            
            await chamado.adicionarComentario(
                this.id_Usuario, 
                this.Nome, 
                `Solicitação de informações: ${pergunta}`
            );
            await chamado.atualizar();
            
            return chamado;
        } catch (error) {
            console.error('Erro ao solicitar informações:', error);
            throw error;
        }
    }

    // Verifica se é técnico
    isTecnico() {
        return this.Nivel_Acesso === 2;
    }
}

module.exports = Tecnico;
