('DOMContentLoaded', function() {
    const form = document.querySelector('.ChamadoBox1 form') || document.querySelector('form');

    const tituloInput = document.querySelector('input[name="titulo"]');

    const nomeInput = document.querySelector('input[name="nome"]');

    const emailInput = document.querySelector('input[type="email"]');

    const categoriaInput = document.querySelector('select[name="categoria"]');

    const descricaoInput = document.querySelector('textarea[name="descricao"]')

    const avançarButton = document.querySelector('button[id="avancar"]') || document.querySelector('button')

    

    // ✅ BOTÃO AVANÇAR - Redireciona para a rota do Express
    if (avancarButton) {
        avancarButton.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Validação básica antes de avançar
            if (validarFormulario()) {
                // Salva dados temporariamente se necessário
                const dadosTemporarios = {
                    titulo: tituloInput ? tituloInput.value : '',
                    nome: nomeInput ? nomeInput.value : '',
                    email: emailInput ? emailInput.value : '',
                    categoria: categoriaInput ? categoriaInput.value : '',
                    descricao: descricaoInput ? descricaoInput.value : ''
                };
                
                // Salva no sessionStorage para usar na próxima tela
                sessionStorage.setItem('chamadoTemp', JSON.stringify(dadosTemporarios));
                
                // ✅ REDIRECIONA PARA A ROTA DO EXPRESS
                window.location.href = '/registrar-chamado-p2';
            }
        });
    }

    // ✅ FUNÇÃO DE VALIDAÇÃO CORRIGIDA
    function validarFormulario() {
        console.log('Iniciando validação...');
        
        let valido = true;
        let mensagemErro = '';
        
        // ✅ Validação do título
        if (!tituloInput) {
            console.error('Campo título não encontrado!');
            mensagemErro = 'Erro no sistema: campo título não encontrado';
            valido = false;
        } else if (!tituloInput.value.trim()) {
            mensagemErro = 'Título é obrigatório!';
            tituloInput.focus();
            valido = false;
        }
        
        // ✅ Validação do nome
        else if (!nomeInput) {
            console.error('Campo nome não encontrado!');
            mensagemErro = 'Erro no sistema: campo nome não encontrado';
            valido = false;
        } else if (!nomeInput.value.trim()) {
            mensagemErro = 'Nome é obrigatório!';
            nomeInput.focus();
            valido = false;
        }
        
        // ✅ Validação do email
        else if (!emailInput) {
            console.error('Campo email não encontrado!');
            mensagemErro = 'Erro no sistema: campo email não encontrado';
            valido = false;
        } else if (!emailInput.value.trim()) {
            mensagemErro = 'E-mail é obrigatório!';
            emailInput.focus();
            valido = false;
        } else if (!validarEmail(emailInput.value)) {
            mensagemErro = 'E-mail inválido!';
            emailInput.focus();
            valido = false;
        }
        
        // ✅ Validação da categoria
        else if (!categoriaInput) {
            console.error('Campo categoria não encontrado!');
            mensagemErro = 'Erro no sistema: campo categoria não encontrado';
            valido = false;
        } else if (!categoriaInput.value) {
            mensagemErro = 'Categoria é obrigatória!';
            categoriaInput.focus();
            valido = false;
        }
        
        // ✅ Validação da descrição
        else if (!descricaoInput) {
            console.error('Campo descrição não encontrado!');
            mensagemErro = 'Erro no sistema: campo descrição não encontrado';
            valido = false;
        } else if (!descricaoInput.value.trim()) {
            mensagemErro = 'Descrição é obrigatória!';
            descricaoInput.focus();
            valido = false;
        }

        if (!valido && mensagemErro) {
            alert(mensagemErro);
        }
        
        console.log('Validação resultado:', valido);
        return valido;
    }

    // ✅ FUNÇÃO PARA VALIDAR EMAIL
    function validarEmail(email) {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    }

    console.log('Formulário de chamados inicializado com sucesso!');

})