// ==========================================
// GUIA: COMO ADICIONAR NOVAS TELAS
// ==========================================

/*
Este arquivo demonstra como adicionar novas telas ao sistema de chamados.
Use os exemplos abaixo como referência.
*/

// ==========================================
// EXEMPLO 1: Adicionar Tela de Editar Chamado
// ==========================================

class EditarChamado extends ChamadoSystem.FormManager {
    constructor() {
        super('#form-editar-chamado', {
            autoSave: true,
            realtimeValidation: true,
            storageKey: 'editar_chamado_temp'
        });
        
        this.chamadoId = this.getChamadoIdFromUrl();
    }

    // Pegar ID do chamado da URL
    getChamadoIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get('id');
    }

    // Inicializar tela
    async init() {
        if (!this.chamadoId) {
            ChamadoSystem.ui.showError('ID do chamado não encontrado');
            setTimeout(() => window.location.href = '/consultar-chamados.html', 2000);
            return false;
        }

        // Inicializar formulário
        const formInitialized = super.init();
        if (!formInitialized) return false;

        // Carregar dados do chamado
        await this.loadChamadoData();
        
        return true;
    }

    // Carregar dados do chamado
    async loadChamadoData() {
        try {
            ChamadoSystem.ui.showLoading('Carregando dados...');
            
            const chamado = await ChamadoSystem.api.get(
                `${ChamadoSystem.config.API.ENDPOINTS.CHAMADOS}/${this.chamadoId}`
            );
            
            this.fillForm(chamado);
            ChamadoSystem.ui.hideLoading();
            
        } catch (error) {
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showError('Erro ao carregar chamado');
            console.error(error);
        }
    }

    // Manipular submit
    async handleSubmit(event) {
        event.preventDefault();

        if (this.isSubmitting) return;

        // Validar
        ChamadoSystem.ui.clearAllErrors();
        const validation = this.validate();
        
        if (!validation.isValid) {
            for (const [field, message] of Object.entries(validation.errors)) {
                ChamadoSystem.ui.showFieldError(field, message);
            }
            ChamadoSystem.ui.showError('Corrija os erros do formulário');
            return;
        }

        // Enviar atualização
        this.isSubmitting = true;
        ChamadoSystem.ui.setFormDisabled(true);
        ChamadoSystem.ui.updateSubmitButton('Salvando...', true);

        try {
            const formData = this.collectData();
            
            await ChamadoSystem.api.put(
                `${ChamadoSystem.config.API.ENDPOINTS.CHAMADOS}/${this.chamadoId}`,
                formData
            );

            ChamadoSystem.ui.showSuccess('Chamado atualizado com sucesso!');
            this.clearSavedData();

            setTimeout(() => {
                window.location.href = '/consultar-chamados.html';
            }, ChamadoSystem.config.TIMING.REDIRECT_DELAY);

        } catch (error) {
            ChamadoSystem.ui.showError(error.message || 'Erro ao atualizar chamado');
            this.isSubmitting = false;
            ChamadoSystem.ui.setFormDisabled(false);
            ChamadoSystem.ui.updateSubmitButton('Salvar', false);
        }
    }
}

// Registrar a nova tela
ChamadoSystem.screenManager.registerScreen('editar-chamado', EditarChamado);

// ==========================================
// EXEMPLO 2: Tela de Detalhes do Chamado
// ==========================================

class DetalhesChamado {
    constructor() {
        this.chamadoId = this.getChamadoIdFromUrl();
        this.container = null;
    }

    getChamadoIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get('id');
    }

    async init() {
        this.container = document.querySelector('#detalhes-container');
        
        if (!this.container) {
            console.error('Container de detalhes não encontrado');
            return false;
        }

        if (!this.chamadoId) {
            ChamadoSystem.ui.showError('ID do chamado não encontrado');
            return false;
        }

        await this.loadDetalhes();
        this.setupActions();
        
        return true;
    }

    async loadDetalhes() {
        try {
            ChamadoSystem.ui.showLoading('Carregando detalhes...');
            
            const chamado = await ChamadoSystem.api.get(
                `${ChamadoSystem.config.API.ENDPOINTS.CHAMADOS}/${this.chamadoId}`
            );
            
            this.renderDetalhes(chamado);
            ChamadoSystem.ui.hideLoading();
            
        } catch (error) {
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showError('Erro ao carregar detalhes');
            console.error(error);
        }
    }

    renderDetalhes(chamado) {
        const html = `
            <div class="detalhes-chamado">
                <div class="header">
                    <h1>${chamado.titulo}</h1>
                    <span class="status status-${chamado.status.toLowerCase()}">${chamado.status}</span>
                </div>
                
                <div class="info-grid">
                    <div class="info-item">
                        <strong>Categoria:</strong>
                        <span>${chamado.categoria}</span>
                    </div>
                    <div class="info-item">
                        <strong>Prioridade:</strong>
                        <span class="prioridade prioridade-${chamado.prioridade}">
                            ${this.getPrioridadeLabel(chamado.prioridade)}
                        </span>
                    </div>
                    <div class="info-item">
                        <strong>Criado em:</strong>
                        <span>${new Date(chamado.dataChamados).toLocaleString('pt-BR')}</span>
                    </div>
                    <div class="info-item">
                        <strong>Solicitante:</strong>
                        <span>${chamado.nome}</span>
                    </div>
                    <div class="info-item">
                        <strong>E-mail:</strong>
                        <span>${chamado.email}</span>
                    </div>
                </div>
                
                <div class="descricao-section">
                    <h3>Descrição do Problema</h3>
                    <p>${chamado.descricao}</p>
                </div>
                
                <div class="actions">
                    <button id="btn-editar" class="btn-primary">Editar</button>
                    <button id="btn-resolver" class="btn-success">Marcar como Resolvido</button>
                    <button id="btn-cancelar" class="btn-danger">Cancelar Chamado</button>
                    <button id="btn-voltar" class="btn-secondary">Voltar</button>
                </div>
            </div>
        `;

        this.container.innerHTML = html;
    }

    getPrioridadeLabel(prioridade) {
        const labels = {
            1: 'Crítica',
            2: 'Alta',
            3: 'Média',
            4: 'Baixa',
            5: 'Muito Baixa'
        };
        return labels[prioridade] || 'Não definida';
    }

    setupActions() {
        // Botão Editar
        const btnEditar = document.getElementById('btn-editar');
        if (btnEditar) {
            btnEditar.addEventListener('click', () => {
                window.location.href = `/editar-chamado.html?id=${this.chamadoId}`;
            });
        }

        // Botão Resolver
        const btnResolver = document.getElementById('btn-resolver');
        if (btnResolver) {
            btnResolver.addEventListener('click', () => this.resolverChamado());
        }

        // Botão Cancelar
        const btnCancelar = document.getElementById('btn-cancelar');
        if (btnCancelar) {
            btnCancelar.addEventListener('click', () => this.cancelarChamado());
        }

        // Botão Voltar
        const btnVoltar = document.getElementById('btn-voltar');
        if (btnVoltar) {
            btnVoltar.addEventListener('click', () => {
                window.location.href = '/consultar-chamados.html';
            });
        }
    }

    async resolverChamado() {
        if (!confirm('Deseja marcar este chamado como resolvido?')) return;

        try {
            ChamadoSystem.ui.showLoading('Atualizando status...');
            
            await ChamadoSystem.api.put(
                `${ChamadoSystem.config.API.ENDPOINTS.CHAMADOS}/${this.chamadoId}`,
                { status: 'Resolvido' }
            );
            
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showSuccess('Chamado marcado como resolvido!');
            
            setTimeout(() => this.loadDetalhes(), 1500);
            
        } catch (error) {
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showError('Erro ao atualizar status');
            console.error(error);
        }
    }

    async cancelarChamado() {
        if (!confirm('Deseja realmente cancelar este chamado?')) return;

        try {
            ChamadoSystem.ui.showLoading('Cancelando chamado...');
            
            await ChamadoSystem.api.put(
                `${ChamadoSystem.config.API.ENDPOINTS.CHAMADOS}/${this.chamadoId}`,
                { status: 'Cancelado' }
            );
            
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showSuccess('Chamado cancelado!');
            
            setTimeout(() => {
                window.location.href = '/consultar-chamados.html';
            }, 1500);
            
        } catch (error) {
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showError('Erro ao cancelar chamado');
            console.error(error);
        }
    }
}

// Registrar a nova tela
ChamadoSystem.screenManager.registerScreen('detalhes-chamado', DetalhesChamado);

// ==========================================
// EXEMPLO 3: Tela de Relatórios
// ==========================================

class Relatorios {
    constructor() {
        this.container = null;
        this.chartCanvas = null;
    }

    async init() {
        this.container = document.querySelector('#relatorios-container');
        this.chartCanvas = document.querySelector('#grafico-chamados');
        
        if (!this.container) {
            console.error('Container de relatórios não encontrado');
            return false;
        }

        await this.loadEstatisticas();
        this.setupFiltros();
        
        return true;
    }

    async loadEstatisticas(periodo = 'mes') {
        try {
            ChamadoSystem.ui.showLoading('Carregando estatísticas...');
            
            const stats = await ChamadoSystem.api.get(
                `${ChamadoSystem.config.API.ENDPOINTS.CHAMADOS}/estatisticas`,
                { periodo }
            );
            
            this.renderEstatisticas(stats);
            
            if (this.chartCanvas) {
                this.renderGrafico(stats);
            }
            
            ChamadoSystem.ui.hideLoading();
            
        } catch (error) {
            ChamadoSystem.ui.hideLoading();
            ChamadoSystem.ui.showError('Erro ao carregar estatísticas');
            console.error(error);
        }
    }

    renderEstatisticas(stats) {
        const html = `
            <div class="estatisticas-grid">
                <div class="stat-card">
                    <h3>Total de Chamados</h3>
                    <p class="stat-number">${stats.total || 0}</p>
                </div>
                <div class="stat-card">
                    <h3>Abertos</h3>
                    <p class="stat-number status-aberto">${stats.abertos || 0}</p>
                </div>
                <div class="stat-card">
                    <h3>Em Andamento</h3>
                    <p class="stat-number status-andamento">${stats.emAndamento || 0}</p>
                </div>
                <div class="stat-card">
                    <h3>Resolvidos</h3>
                    <p class="stat-number status-resolvido">${stats.resolvidos || 0}</p>
                </div>
            </div>
            
            <div class="categoria-stats">
                <h3>Chamados por Categoria</h3>
                <ul>
                    ${Object.entries(stats.porCategoria || {}).map(([cat, count]) => `
                        <li>${cat}: <strong>${count}</strong></li>
                    `).join('')}
                </ul>
            </div>
        `;

        this.container.innerHTML = html;
    }

    renderGrafico(stats) {
        // Exemplo simples - você pode usar Chart.js ou outra biblioteca
        console.log('Renderizar gráfico com:', stats);
        // Implementar visualização de dados aqui
    }

    setupFiltros() {
        const filtroSelect = document.querySelector('#periodo-filtro');
        if (filtroSelect) {
            filtroSelect.addEventListener('change', (e) => {
                this.loadEstatisticas(e.target.value);
            });
        }
    }
}

// Registrar a nova tela
ChamadoSystem.screenManager.registerScreen('relatorios', Relatorios);

// ==========================================
// COMO USAR AS NOVAS TELAS
// ==========================================

/*
1. No HTML da sua página, adicione um script para inicializar a tela desejada:

<script>
    // Para a tela de editar
    ChamadoSystem.screenManager.initScreen('editar-chamado');
    
    // Para a tela de detalhes
    ChamadoSystem.screenManager.initScreen('detalhes-chamado');
    
    // Para a tela de relatórios
    ChamadoSystem.screenManager.initScreen('relatorios');
</script>

2. Ou deixe o sistema detectar automaticamente baseado no DOM:
   - O sistema irá detectar qual tela carregar baseado nos elementos presentes

3. Para criar uma nova tela personalizada:

class MinhaNovaTelaCustomizada {
    constructor() {
        // Seu código de inicialização
    }

    async init() {
        // Configurar a tela
        // Retornar true se sucesso, false se erro
        return true;
    }

    // Adicionar métodos personalizados conforme necessário
}

// Registrar
ChamadoSystem.screenManager.registerScreen('minha-tela', MinhaNovaTelaCustomizada);

// Usar
ChamadoSystem.screenManager.initScreen('minha-tela');

*/

// ==========================================
// DICAS IMPORTANTES
// ==========================================

/*
✅ Use ChamadoSystem.ui para todas as notificações
✅ Use ChamadoSystem.api para todas as requisições HTTP
✅ Use ChamadoSystem.storage para armazenamento local
✅ Use ChamadoSystem.Validators para validações
✅ Estenda FormManager para formulários automáticos
✅ Use screenManager.registerScreen() para registrar novas telas

📦 Recursos Disponíveis:
- ChamadoSystem.ui.showSuccess(message)
- ChamadoSystem.ui.showError(message)
- ChamadoSystem.ui.showWarning(message)
- ChamadoSystem.ui.showInfo(message)
- ChamadoSystem.ui.showLoading(message)
- ChamadoSystem.ui.hideLoading()
- ChamadoSystem.api.get(endpoint, params)
- ChamadoSystem.api.post(endpoint, data)
- ChamadoSystem.api.put(endpoint, data)
- ChamadoSystem.api.delete(endpoint)
- ChamadoSystem.storage.save(key, data)
- ChamadoSystem.storage.load(key)
- ChamadoSystem.storage.remove(key)
*/
