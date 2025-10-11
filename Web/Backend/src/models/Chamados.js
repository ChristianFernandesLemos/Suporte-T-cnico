class Chamados {
    // Método construtor
    constructor() {
        this.id_chamado = 0;
        this.categoria = '';
        this.prioridade = 0;
        this.descricao = '';
        this.afetado = ''; 
        this.dataChamados = 0;
        this.contestacoes = '';
        this.status = 'Aberto'; // Status padrão
    }

    // Métodos públicos
    CriarChamado(id, categoria, prioridade, descricao, atribuido, data, contestacoes = '') {
        this.id_chamado = id;
        this.categoria = categoria;
        this.prioridade = prioridade;
        this.descricao = descricao;
        this.atribuido = atribuido;
        this.dataChamados = data;
        this.contestacoes = contestacoes;
        this.status = 'Aberto';
        
        console.log(`Chamado #${this.id_chamado} criado com sucesso!`);
        return this;
    }

    AlterarStatus(novoStatus) {
        const statusValidos = ['Aberto', 'Em Andamento', 'Fechado', 'Cancelado'];
        
        if (statusValidos.includes(novoStatus)) {
            const statusAnterior = this.status;
            this.status = novoStatus;
            console.log(`Status alterado de "${statusAnterior}" para "${this.status}"`);
        } else {
            console.log('Status inválido. Use: Aberto, Em Andamento, Fechado ou Cancelado');
        }
        return this;
    }

    AlterarPrioridade(novaPrioridade) {
        if (novaPrioridade >= 1 && novaPrioridade <= 10) {
            const prioridadeAnterior = this.prioridade;
            this.prioridade = novaPrioridade;
            console.log(`Prioridade alterada de ${prioridadeAnterior} para ${this.prioridade}`);
        } else {
            console.log('Prioridade deve ser entre 1 e 10');
        }
        return this;
    }

    // Método para exibir informações
    exibirChamado() {
        return `Chamado #${this.id_chamado}
Categoria: ${this.categoria}
Prioridade: ${this.prioridade}
Descrição: ${this.descricao}
Atribuído: ${this.atribuido}
Data: ${this.dataChamados}
Status: ${this.status}
Contestações: ${this.contestacoes}`;
    }
}

// Exemplo de uso COM construtor
const chamado1 = new Chamados();
chamado1.CriarChamado(
    1,
    'TI',
    8,
    'Problema de conexão com a internet',
    'João Silva',
    '2024-01-15',
    'Nenhuma'
);

chamado1.AlterarStatus('Em Andamento');
chamado1.AlterarPrioridade(9);
console.log(chamado1.exibirChamado());