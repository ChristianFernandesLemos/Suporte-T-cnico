// Script para criar usuário de teste
require('dotenv').config();
const { getConnection } = require('./db');
const crypto = require('crypto');

// Função para hash de senha
function hashPassword(password) {
  return crypto.createHash('sha256').update(password).digest('hex');
}

async function criarUsuarioTeste() {
  const pool = await getConnection();
  const transaction = pool.transaction();
  
  try {
    console.log('\n🔧 ===== CRIAR USUÁRIO DE TESTE =====\n');
    
    // Dados do usuário de teste
    const nome = 'Usuário Teste';
    const cpf = '12345678900';
    const email = 'teste@interfix.com';
    const senha = 'teste123'; // Senha em texto claro
    const senhaHash = hashPassword(senha);
    const nivelAcesso = 1; // 1 = Funcionário, 2 = Técnico, 3 = Admin
    
    console.log('📋 Dados do usuário:');
    console.log(`   Nome: ${nome}`);
    console.log(`   CPF: ${cpf}`);
    console.log(`   Email: ${email}`);
    console.log(`   Senha: ${senha}`);
    console.log(`   Hash: ${senhaHash}`);
    console.log(`   Nível de Acesso: ${nivelAcesso}`);
    
    await transaction.begin();
    
    // Verifica se o email já existe
    const emailExiste = await transaction.request()
      .input('email', email)
      .query('SELECT * FROM dbo.E_mail WHERE E_mail = @email');
    
    if (emailExiste.recordset.length > 0) {
      console.log('\n⚠️  Email já cadastrado! Deletando usuário anterior...');
      const idUsuarioAntigo = emailExiste.recordset[0].Id_usuario;
      
      await transaction.request()
        .input('id', idUsuarioAntigo)
        .query('DELETE FROM dbo.E_mail WHERE Id_usuario = @id');
      
      await transaction.request()
        .input('id', idUsuarioAntigo)
        .query('DELETE FROM dbo.Usuario WHERE Id_usuario = @id');
      
      console.log('   ✅ Usuário anterior removido!');
    }
    
    // Insere o usuário
    console.log('\n1️⃣ Inserindo usuário na tabela Usuario...');
    const resultUsuario = await transaction.request()
      .input('nome', nome)
      .input('cpf', cpf)
      .input('senha', senhaHash)
      .input('acess_codigo', nivelAcesso)
      .query(`
        INSERT INTO dbo.Usuario (nome, Cpf, senha, Acess_codigo, Ativo, DataCadastro)
        OUTPUT INSERTED.Id_usuario
        VALUES (@nome, @cpf, @senha, @acess_codigo, 1, GETDATE())
      `);
    
    const idUsuario = resultUsuario.recordset[0].Id_usuario;
    console.log(`   ✅ Usuário criado com ID: ${idUsuario}`);
    
    // Insere o email
    console.log('\n2️⃣ Inserindo email na tabela E_mail...');
    await transaction.request()
      .input('email', email)
      .input('id_usuario', idUsuario)
      .query(`
        INSERT INTO dbo.E_mail (E_mail, Id_usuario)
        VALUES (@email, @id_usuario)
      `);
    console.log('   ✅ Email cadastrado!');
    
    await transaction.commit();
    
    console.log('\n✅ ===== USUÁRIO CRIADO COM SUCESSO! =====');
    console.log('\n📧 Credenciais de login:');
    console.log(`   Email: ${email}`);
    console.log(`   Senha: ${senha}`);
    console.log('\n==========================================\n');
    
    // Testa a busca
    console.log('🧪 Testando busca do usuário...');
    const teste = await pool.request()
      .input('email', email)
      .query(`
        SELECT 
          u.Id_usuario,
          u.nome,
          e.E_mail,
          u.senha,
          u.Acess_codigo
        FROM dbo.Usuario u
        INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
        WHERE e.E_mail = @email AND u.Ativo = 1
      `);
    
    if (teste.recordset.length > 0) {
      console.log('✅ Usuário encontrado no banco!');
      console.log(`   Nome: ${teste.recordset[0].nome}`);
      console.log(`   Email: ${teste.recordset[0].E_mail}`);
      console.log('\n🎉 Agora você pode fazer login no sistema!\n');
    } else {
      console.log('❌ Erro: usuário não encontrado após inserção!');
    }
    
  } catch (error) {
    await transaction.rollback();
    console.error('❌ Erro ao criar usuário:', error);
  } finally {
    process.exit(0);
  }
}

criarUsuarioTeste();
