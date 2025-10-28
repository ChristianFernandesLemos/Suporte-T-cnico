// Debug detalhado do login
require('dotenv').config();
const { getConnection } = require('./db');

async function debugDetalhado() {
  try {
    console.log('\n🔍 ===== DEBUG DETALHADO DO LOGIN =====\n');
    
    const pool = await getConnection();
    
    // Passo 1: Listar TODOS os dados das 3 tabelas
    console.log('📊 PASSO 1: Dados da tabela Usuario');
    console.log('=====================================');
    const usuarios = await pool.request().query(`
      SELECT * FROM dbo.Usuario
    `);
    console.table(usuarios.recordset);
    
    console.log('\n📊 PASSO 2: Dados da tabela E_mail');
    console.log('=====================================');
    const emails = await pool.request().query(`
      SELECT * FROM dbo.E_mail
    `);
    console.table(emails.recordset);
    
    console.log('\n📊 PASSO 3: Dados da tabela Nivel_de_acesso');
    console.log('=====================================');
    const niveis = await pool.request().query(`
      SELECT * FROM dbo.Nivel_de_acesso
    `);
    console.table(niveis.recordset);
    
    // Passo 2: Testar JOIN
    console.log('\n🔗 PASSO 4: Resultado do JOIN (query do findByEmail)');
    console.log('=======================================================');
    const joinResult = await pool.request().query(`
      SELECT 
        u.Id_usuario,
        u.nome,
        u.Cpf,
        e.E_mail,
        u.senha,
        u.Acess_codigo,
        n.Nivel_acesso,
        u.Ativo
      FROM dbo.Usuario u
      INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
      INNER JOIN dbo.Nivel_de_acesso n ON u.Acess_codigo = n.codigo
    `);
    
    if (joinResult.recordset.length === 0) {
      console.log('❌ NENHUM RESULTADO! Problemas possíveis:');
      console.log('   - IDs não estão relacionados corretamente');
      console.log('   - Acess_codigo não existe na tabela Nivel_de_acesso');
      console.log('   - Tabelas vazias');
    } else {
      console.log(`✅ ${joinResult.recordset.length} usuário(s) encontrado(s):`);
      console.table(joinResult.recordset);
    }
    
    // Passo 3: Simular busca por email
    if (emails.recordset.length > 0) {
      const primeiroEmail = emails.recordset[0].E_mail;
      console.log(`\n🎯 PASSO 5: Simulando busca por email: "${primeiroEmail}"`);
      console.log('================================================================');
      
      const busca = await pool.request()
        .input('email', primeiroEmail)
        .query(`
          SELECT 
            u.Id_usuario,
            u.nome,
            u.Cpf,
            e.E_mail,
            u.senha,
            u.Acess_codigo,
            n.Nivel_acesso
          FROM dbo.Usuario u
          INNER JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
          INNER JOIN dbo.Nivel_de_acesso n ON u.Acess_codigo = n.codigo
          WHERE e.E_mail = @email AND u.Ativo = 1
        `);
      
      if (busca.recordset.length > 0) {
        console.log('✅ Usuário encontrado pela query do login!');
        const user = busca.recordset[0];
        console.log('\n📝 Dados retornados:');
        console.log(`   Id_usuario: ${user.Id_usuario}`);
        console.log(`   nome: ${user.nome}`);
        console.log(`   Cpf: ${user.Cpf}`);
        console.log(`   E_mail: ${user.E_mail}`);
        console.log(`   senha: ${user.senha}`);
        console.log(`   Acess_codigo: ${user.Acess_codigo}`);
        console.log(`   Nivel_acesso: ${user.Nivel_acesso}`);
        
        console.log('\n🔐 TESTE DE LOGIN:');
        console.log('==================');
        console.log(`   Para fazer login use:`);
        console.log(`   Email: ${user.E_mail}`);
        console.log(`   Senha: ${user.senha}`);
        console.log('\n   ⚠️  Sim, você deve digitar a senha EXATAMENTE como está no campo "senha" acima!');
      } else {
        console.log('❌ Nenhum usuário encontrado com este email!');
        console.log('   Verifique:');
        console.log('   - Campo Ativo = 1?');
        console.log('   - Email existe na tabela E_mail?');
        console.log('   - Id_usuario está correto?');
      }
    }
    
    // Verificar problemas comuns
    console.log('\n🔍 PASSO 6: Verificando problemas comuns');
    console.log('==========================================');
    
    // Verificar usuários sem email
    const semEmail = await pool.request().query(`
      SELECT u.Id_usuario, u.nome 
      FROM dbo.Usuario u
      LEFT JOIN dbo.E_mail e ON u.Id_usuario = e.Id_usuario
      WHERE e.ID IS NULL
    `);
    
    if (semEmail.recordset.length > 0) {
      console.log(`⚠️  ${semEmail.recordset.length} usuário(s) SEM email cadastrado:`);
      console.table(semEmail.recordset);
    } else {
      console.log('✅ Todos os usuários têm email cadastrado');
    }
    
    // Verificar usuários com Acess_codigo inválido
    const codigoInvalido = await pool.request().query(`
      SELECT u.Id_usuario, u.nome, u.Acess_codigo
      FROM dbo.Usuario u
      LEFT JOIN dbo.Nivel_de_acesso n ON u.Acess_codigo = n.codigo
      WHERE n.codigo IS NULL
    `);
    
    if (codigoInvalido.recordset.length > 0) {
      console.log(`\n⚠️  ${codigoInvalido.recordset.length} usuário(s) com Acess_codigo INVÁLIDO:`);
      console.table(codigoInvalido.recordset);
    } else {
      console.log('✅ Todos os usuários têm Acess_codigo válido');
    }
    
    // Verificar usuários inativos
    const inativos = await pool.request().query(`
      SELECT Id_usuario, nome, Ativo 
      FROM dbo.Usuario 
      WHERE Ativo = 0 OR Ativo IS NULL
    `);
    
    if (inativos.recordset.length > 0) {
      console.log(`\n⚠️  ${inativos.recordset.length} usuário(s) INATIVO(S):`);
      console.table(inativos.recordset);
    } else {
      console.log('✅ Não há usuários inativos');
    }
    
    console.log('\n===========================================');
    console.log('🏁 DEBUG CONCLUÍDO!');
    console.log('===========================================\n');
    
  } catch (error) {
    console.error('❌ Erro no debug:', error);
  } finally {
    process.exit(0);
  }
}

debugDetalhado();
