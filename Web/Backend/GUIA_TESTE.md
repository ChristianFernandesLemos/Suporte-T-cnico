# 🚀 Guia Rápido - Como Testar o Sistema

## Passo a Passo

### 1️⃣ Preparar o Banco de Dados

1. Abra o **Azure Data Studio** ou **SQL Server Management Studio**
2. Conecte ao servidor: `interfix-db-piml.database.windows.net`
3. Abra o arquivo: `src/config/create-users-table.sql`
4. Execute o script completo (F5)
5. Verifique se a tabela foi criada e os usuários inseridos

### 2️⃣ Testar Conexão com o Banco

```bash
# No terminal, na pasta Backend:
node db.js
```

**Resultado esperado:**
```
🔄 Tentando conectar ao banco de dados...
📡 Server: interfix-db-piml.database.windows.net
🗄️  Database: db-interfix1
✅ Conectado ao SQL Server!
✅ Teste de query bem-sucedido: [ { test: 1, date: 2025-10-18... } ]
```

### 3️⃣ Iniciar o Servidor

```bash
npm start
```

**Resultado esperado:**
```
🚀 ================================
   SERVIDOR INTERFIX INICIADO
   ================================
📍 Servidor: http://localhost:3000
📍 Login: http://localhost:3000/login
📍 Menu: http://localhost:3000/menu
================================
   ROTAS DA API
================================
📍 Auth: http://localhost:3000/api/auth/login
📍 Users: http://localhost:3000/api/users
📍 Health: http://localhost:3000/api/health
================================

🔄 Testando conexão com banco de dados...

✅ Sistema pronto para uso!
```

### 4️⃣ Testar no Navegador

#### **A) Testar Página de Login**

1. Abra: `http://localhost:3000/login`
2. Digite:
   - Email: `admin@interfix.com`
   - Senha: `admin123`
3. Clique em **Entrar**

**Resultado esperado:**
- Mensagem: "Login realizado com sucesso!"
- Redirecionamento automático para o menu principal

#### **B) Testar Menu Principal**

1. Após login, você deve ver:
   - Nome do usuário: "Administrador"
   - Menu lateral com opções
   - Botão "Sair" no final do menu
2. Teste clicar nas opções do menu
3. Teste o botão "Sair"

### 5️⃣ Testar API com Postman/Insomnia

#### **Login**
```http
POST http://localhost:3000/api/auth/login
Content-Type: application/json

{
  "email": "admin@interfix.com",
  "senha": "admin123"
}
```

**Resposta esperada:**
```json
{
  "success": true,
  "message": "Login realizado com sucesso!",
  "token": "abc123xyz...",
  "user": {
    "id": 1,
    "nome": "Administrador",
    "email": "admin@interfix.com",
    "tipo_usuario": "admin"
  }
}
```

#### **Listar Usuários** (copie o token do login)
```http
GET http://localhost:3000/api/users
Authorization: Bearer {SEU_TOKEN_AQUI}
```

**Resposta esperada:**
```json
{
  "success": true,
  "users": [
    {
      "id": 1,
      "nome": "Administrador",
      "email": "admin@interfix.com",
      "tipo_usuario": "admin",
      "ativo": true,
      "data_criacao": "2025-10-18T..."
    },
    ...
  ]
}
```

#### **Criar Novo Usuário**
```http
POST http://localhost:3000/api/users
Authorization: Bearer {SEU_TOKEN_AQUI}
Content-Type: application/json

{
  "nome": "Teste Silva",
  "email": "teste@interfix.com",
  "senha": "teste123",
  "tipo_usuario": "tecnico"
}
```

#### **Verificar Sessão**
```http
GET http://localhost:3000/api/auth/verify
Authorization: Bearer {SEU_TOKEN_AQUI}
```

#### **Logout**
```http
POST http://localhost:3000/api/auth/logout
Authorization: Bearer {SEU_TOKEN_AQUI}
```

### 6️⃣ Testar Health Check

Abra no navegador: `http://localhost:3000/api/health`

**Resposta esperada:**
```json
{
  "status": "OK",
  "database": "Conectado",
  "timestamp": "2025-10-18T..."
}
```

---

## ✅ Checklist de Testes

### Frontend (Navegador)
- [ ] Página de login carrega corretamente
- [ ] Login com credenciais corretas funciona
- [ ] Mensagem de erro para credenciais incorretas
- [ ] Redirecionamento para menu após login
- [ ] Nome do usuário aparece no menu
- [ ] Botão de logout funciona
- [ ] Ao fazer logout, volta para login
- [ ] Não consegue acessar /menu sem estar logado
- [ ] CSS e imagens carregam corretamente

### Backend (API)
- [ ] Servidor inicia sem erros
- [ ] Conexão com banco de dados OK
- [ ] POST /api/auth/login retorna token
- [ ] POST /api/auth/login rejeita senha incorreta
- [ ] GET /api/auth/verify valida token
- [ ] GET /api/users lista usuários (admin)
- [ ] POST /api/users cria novo usuário (admin)
- [ ] PUT /api/users/:id atualiza usuário
- [ ] PATCH /api/users/:id/deactivate desativa usuário
- [ ] Rotas protegidas sem token retornam 401
- [ ] Rotas admin sem permissão retornam 403

### Banco de Dados
- [ ] Tabela `usuarios` criada
- [ ] Usuário admin inserido
- [ ] Usuários de teste inseridos
- [ ] Índices criados
- [ ] Queries executam sem erro

---

## 🐛 Problemas Comuns

### "Cannot GET /"
**Solução:** O servidor redireciona `/` para `/login`. Acesse `http://localhost:3000/login`

### "Erro ao conectar no banco"
**Soluções:**
1. Verifique `.env` tem credenciais corretas
2. Adicione seu IP no firewall do Azure
3. Teste com: `node db.js`

### "Token não fornecido"
**Solução:** Adicione o header `Authorization: Bearer {token}` nas requisições

### "Sessão inválida ou expirada"
**Solução:** Faça login novamente para obter novo token

### CSS não carrega
**Solução:** Certifique-se que:
1. Arquivos estão em `src/views/`
2. Links usam `/static/` (ex: `/static/style.css`)
3. Servidor está rodando

### Scripts JS não executam
**Solução:** Verifique:
1. Scripts têm `defer` no HTML
2. Caminhos corretos: `/static/login.js`, `/static/menu.js`
3. Console do navegador (F12) para erros

---

## 🔍 Como Debugar

### Ver logs do servidor
```bash
# O servidor já exibe logs automáticos
# Toda requisição é logada em modo development
```

### Inspecionar no navegador
1. Pressione **F12** para abrir DevTools
2. Aba **Console** - veja erros JavaScript
3. Aba **Network** - veja requisições HTTP
4. Aba **Application > Local Storage** - veja token armazenado

### Testar queries SQL direto
```sql
-- Verificar usuários
SELECT * FROM usuarios;

-- Verificar login específico
SELECT * FROM usuarios WHERE email = 'admin@interfix.com';

-- Contar usuários
SELECT COUNT(*) as total FROM usuarios;
```

---

## 📊 Fluxo de Autenticação

```
1. Usuário acessa /login
   ↓
2. Digita email e senha
   ↓
3. JavaScript envia POST /api/auth/login
   ↓
4. Backend valida credenciais no banco
   ↓
5. Se OK: retorna token + dados do usuário
   ↓
6. Frontend salva token no localStorage
   ↓
7. Redireciona para /menu
   ↓
8. Menu verifica se tem token válido
   ↓
9. Se válido: carrega informações do usuário
   ↓
10. Se inválido: volta para /login
```

---

## 🎯 Próximos Testes

Depois que tudo estiver funcionando:

1. **Teste com diferentes usuários:**
   - Admin pode gerenciar usuários ✅
   - Técnico NÃO pode gerenciar usuários ❌

2. **Teste recuperação de senha:**
   - Clique em "Esqueci a senha"
   - Digite email cadastrado

3. **Teste criação de novo usuário:**
   - Faça login como admin
   - Use Postman para criar usuário
   - Tente fazer login com novo usuário

4. **Teste segurança:**
   - Tente acessar /api/users sem token (deve dar erro 401)
   - Tente acessar rotas admin como técnico (deve dar erro 403)

---

## 📝 Credenciais de Teste

| Tipo | Email | Senha | Permissões |
|------|-------|-------|------------|
| Admin | admin@interfix.com | admin123 | Todas |
| Técnico | tecnico@interfix.com | tecnico123 | Limitadas |
| Técnico 2 | maria@interfix.com | maria123 | Limitadas |

---

## 💡 Dicas

1. **Mantenha o terminal aberto** para ver logs em tempo real
2. **Use o modo dev** com `npm run dev` para recarregar automaticamente
3. **Teste sempre com F12 aberto** no navegador para ver erros
4. **Salve o token** ao testar com Postman para reutilizar
5. **Limpe o localStorage** se tiver problemas: `localStorage.clear()`

---

## ✨ Tudo Funcionando?

Se todos os testes passarem, você está pronto para:

1. ✅ Desenvolver CRUD de Chamados
2. ✅ Adicionar mais funcionalidades
3. ✅ Integrar com frontend
4. ✅ Deploy em produção

**Parabéns! 🎉 Seu backend está funcionando!**
