const express = require('express');
const router = express.Router();
const { solicitarRecuperacaoSenha } = require('../controllers/recuperarController');

// Rota POST para solicitar recuperação de senha
// URL final: POST /api/recuperar/solicitar
router.post('/solicitar', async (req, res) => {
    try {
        console.log('📩 Requisição de recuperação recebida:', req.body);
        
        // 1. Recebe os dados JSON do front-end
        const { cpf, email } = req.body;

        // 2. Validação básica
        if (!cpf || !email) {
            console.log('❌ Validação falhou: CPF ou email ausente');
            return res.status(400).json({
                sucesso: false,
                mensagem: 'CPF e e-mail são obrigatórios!'
            });
        }

        console.log('🔍 Processando recuperação para:', { cpf, email });

        // 3. Chama a função do controller
        const resultado = await solicitarRecuperacaoSenha(cpf, email);

        console.log('📤 Resultado do controller:', resultado);

        // 4. Retorna resposta adequada
        if (resultado.sucesso) {
            return res.status(200).json(resultado);
        } else {
            return res.status(400).json(resultado);
        }

    } catch (error) {
        console.error('❌ Erro na rota de recuperação:', error);
        return res.status(500).json({
            sucesso: false,
            mensagem: 'Erro interno do servidor. Tente novamente mais tarde.'
        });
    }
});

module.exports = router;