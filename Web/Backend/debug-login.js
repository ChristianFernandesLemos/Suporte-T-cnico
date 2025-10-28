// Script de debug para testar o login
require('dotenv').config();
const { getConnection } = require('./db');
const crypto = require('crypto');

// Função para hash de senha (mesma do AuthController)
function hashPassword(password) {
  return crypto.createHash('sha256').update(password).digest('hex');
}

async function debugLogin() {
  try {
    console.log('\n🔍 ===== DEBUG DE LOGIN =====\n');
    
    const pool = await getConnection();
    
    // 1. Verifica se existem usuários
    console.log('1️⃣ Verificando usuários na tabela Usuario:');
    const usuarios = await pool.request().query(`
      SELECT Id_usuario, nome, Cpf, senha, Acess_codigo, Ativo 
      FROM dbo.Usuario
    `);
    console.log(`   Total de usuários: ${usuarios.recordset.length}`);
    usuarios.recordset.forEach(u => {
      console.log(`   - ID: ${u.Id_usuario} | Nome: ${u.nome} | Ativo: ${u.Ativo} | Acess_codigo: ${u.Acess_codigo}`);
      console.log(`     Senha (hash): ${u.senha.substring(0, 20)}...`);
    });
    
    // 2. Verifica emails
    console.log('\n2️⃣ Verificando emails na tabela E_mail:');
    const emails = await pool.request().query(`
      SELECT ID, E_mail, Id_usuario 
      FROM dbo.E_mail
    `);
    console.log(`   Total de emails: ${emails.recordset.length}`);
    emails.recordset.forEach(e => {
      console.log(`   - Email: ${e.E_mail} | Id_usuario: ${e.Id_usuario}`);
    });
    
    // 3. Verifica níveis de acesso
    console.log('\n3️⃣ Verificando níveis de acesso:');
    const niveis = await pool.request().query(`
      SELECT codigo, Nivel_acesso 
      FROM dbo.Nivel_de_acesso
    `);
    console.log(`   Total de níveis: ${niveis.recordset.length}`);
    niveis.recordset.forEach(n => {
      console.log(`   - Código: ${n.codigo} | Nível: ${n.Nivel_acesso}`);
    });
    
    // 4. Testa o JOIN completo
    console.log('\n4️⃣ Testando JOIN completo (query do login):');
    const joinTest = await pool.request().query(`
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
      WHERE u.Ativo = 1
    `);
    console.log(`   Usuários retornados pelo JOIN: ${joinTest.recordset.length}`);
    joinTest.recordset.forEach(u => {
      console.log(`   - Email: ${u.E_mail} | Nome: ${u.nome} | Nível: ${u.Nivel_acesso}`);
    });
    
    // 5. Testa busca por email específico
    if (emails.recordset.length > 0) {
      const emailTeste = emails.recordset[0].E_mail;
      console.log(`\n5️⃣ Testando busca por email específico: ${emailTeste}`);
      const resultado = await pool.request()
        .input('email', emailTeste)
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
      
      if (resultado.recordset.length > 0) {
        const user = resultado.recordset[0];
        console.log('   ✅ Usuário encontrado!');
        console.log(`   - Nome: ${user.nome}`);
        console.log(`   - Email: ${user.E_mail}`);
        console.log(`   - Senha (hash): ${user.senha.substring(0, 30)}...`);
        console.log(`   - Nível: ${user.Nivel_acesso}`);
        
        // Testa algumas senhas comuns
        console.log('\n6️⃣ Testando hashes de senhas comuns:');
        const senhasComuns = ['123456', 'admin', 'admin123', '12345678', 'senha123'];
        senhasComuns.forEach(senha => {
          const hash = hashPassword(senha);
          const match = hash === user.senha;
          console.log(`   - "${senha}": ${hash.substring(0, 30)}... ${match ? '✅ MATCH!' : '❌'}`);
        });
      } else {
        console.log('   ❌ Nenhum usuário encontrado com este email!');
      }
    }
    
    console.log('\n================================\n');
    
  } catch (error) {
    console.error('❌ Erro no debug:', error);
  } finally {
    process.exit(0);
  }
}

debugLogin();
