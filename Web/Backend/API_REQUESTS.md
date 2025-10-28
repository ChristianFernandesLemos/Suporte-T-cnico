# Coleção de Requisições - InterFix API

## Para usar com Postman, Insomnia ou Thunder Client

---

## 🔐 Autenticação

### 1. Login (Admin)
```http
POST http://localhost:3000/api/auth/login
Content-Type: application/json

{
  "email": "admin@interfix.com",
  "senha": "admin123"
}
```

### 2. Login (Técnico)
```http
POST http://localhost:3000/api/auth/login
Content-Type: application/json

{
  "email": "tecnico@interfix.com",
  "senha": "tecnico123"
}
```

### 3. Verificar Sessão
```http
GET http://localhost:3000/api/auth/verify
Authorization: Bearer {SEU_TOKEN_AQUI}
```

### 4. Logout
```http
POST http://localhost:3000/api/auth/logout
Authorization: Bearer {SEU_TOKEN_AQUI}
```

### 5. Alterar Senha
```http
POST http://localhost:3000/api/auth/change-password
Authorization: Bearer {SEU_TOKEN_AQUI}
Content-Type: application/json

{
  "senhaAtual": "admin123",
  "novaSenha": "novasenha123",
  "confirmarSenha": "novasenha123"
}
```

### 6. Recuperar Senha
```http
POST http://localhost:3000/api/auth/forgot-password
Content-Type: application/json

{
  "email": "admin@interfix.com"
}
```

---

## 👥 Usuários (Requer autenticação Admin)

### 1. Listar Todos os Usuários
```http
GET http://localhost:3000/api/users
Authorization: Bearer {SEU_TOKEN_AQUI}
```

### 2. Buscar Usuário por ID
```http
GET http://localhost:3000/api/users/1
Authorization: Bearer {SEU_TOKEN_AQUI}
```

### 3. Criar Novo Usuário
```http
POST http://localhost:3000/api/users
Authorization: Bearer {SEU_TOKEN_AQUI}
Content-Type: application/json

{
  "nome": "Carlos Souza",
  "email": "carlos@interfix.com",
  "senha": "carlos123",
  "tipo_usuario": "tecnico"
}
```

### 4. Atualizar Usuário
```http
PUT http://localhost:3000/api/users/4
Authorization: Bearer {SEU_TOKEN_AQUI}
Content-Type: application/json

{
  "nome": "Carlos Souza Silva",
  "email": "carlos.souza@interfix.com",
  "tipo_usuario": "admin"
}
```

### 5. Desativar Usuário
```http
PATCH http://localhost:3000/api/users/4/deactivate
Authorization: Bearer {SEU_TOKEN_AQUI}
```

### 6. Reativar Usuário
```http
PATCH http://localhost:3000/api/users/4/activate
Authorization: Bearer {SEU_TOKEN_AQUI}
```

---

## 🏥 Health Check

### Verificar Status do Sistema
```http
GET http://localhost:3000/api/health
```

**Resposta esperada:**
```json
{
  "status": "OK",
  "database": "Conectado",
  "timestamp": "2025-10-18T15:30:00.000Z"
}
```

---

## 📋 Respostas Esperadas

### Login com Sucesso
```json
{
  "success": true,
  "message": "Login realizado com sucesso!",
  "token": "abc123xyz789...",
  "user": {
    "id": 1,
    "nome": "Administrador",
    "email": "admin@interfix.com",
    "tipo_usuario": "admin"
  }
}
```

### Login com Erro
```json
{
  "success": false,
  "message": "Email ou senha incorretos."
}
```

### Erro de Autenticação
```json
{
  "success": false,
  "message": "Token não fornecido. Faça login para continuar."
}
```

### Erro de Permissão
```json
{
  "success": false,
  "message": "Acesso negado. Apenas administradores podem acessar este recurso."
}
```

### Usuário Criado
```json
{
  "success": true,
  "message": "Usuário criado com sucesso!",
  "user": {
    "id": 4,
    "nome": "Carlos Souza",
    "email": "carlos@interfix.com",
    "tipo_usuario": "tecnico",
    "ativo": true,
    "data_criacao": "2025-10-18T15:30:00.000Z"
  }
}
```

---

## 🔄 Fluxo Completo de Teste

### Passo 1: Login
```http
POST http://localhost:3000/api/auth/login
Content-Type: application/json

{
  "email": "admin@interfix.com",
  "senha": "admin123"
}
```
**Copie o token da resposta!**

---

### Passo 2: Verificar Sessão
```http
GET http://localhost:3000/api/auth/verify
Authorization: Bearer {COLE_O_TOKEN_AQUI}
```

---

### Passo 3: Listar Usuários
```http
GET http://localhost:3000/api/users
Authorization: Bearer {COLE_O_TOKEN_AQUI}
```

---

### Passo 4: Criar Novo Usuário
```http
POST http://localhost:3000/api/users
Authorization: Bearer {COLE_O_TOKEN_AQUI}
Content-Type: application/json

{
  "nome": "Novo Usuário",
  "email": "novo@interfix.com",
  "senha": "senha123",
  "tipo_usuario": "tecnico"
}
```

---

### Passo 5: Buscar Usuário Criado
```http
GET http://localhost:3000/api/users/{ID_DO_USUARIO_CRIADO}
Authorization: Bearer {COLE_O_TOKEN_AQUI}
```

---

### Passo 6: Atualizar Usuário
```http
PUT http://localhost:3000/api/users/{ID_DO_USUARIO_CRIADO}
Authorization: Bearer {COLE_O_TOKEN_AQUI}
Content-Type: application/json

{
  "nome": "Usuário Atualizado",
  "email": "novo@interfix.com",
  "tipo_usuario": "admin"
}
```

---

### Passo 7: Logout
```http
POST http://localhost:3000/api/auth/logout
Authorization: Bearer {COLE_O_TOKEN_AQUI}
```

---

## 🧪 Testes de Segurança

### 1. Tentar Acessar Rota Protegida Sem Token
```http
GET http://localhost:3000/api/users
```
**Esperado:** Erro 401 - "Token não fornecido"

---

### 2. Tentar Acessar Rota Admin Como Técnico
```http
# Faça login como técnico primeiro
POST http://localhost:3000/api/auth/login
Content-Type: application/json

{
  "email": "tecnico@interfix.com",
  "senha": "tecnico123"
}

# Depois tente listar usuários com o token do técnico
GET http://localhost:3000/api/users
Authorization: Bearer {TOKEN_DO_TECNICO}
```
**Esperado:** Erro 403 - "Acesso negado"

---

### 3. Tentar Login com Senha Errada
```http
POST http://localhost:3000/api/auth/login
Content-Type: application/json

{
  "email": "admin@interfix.com",
  "senha": "senhaerrada"
}
```
**Esperado:** Erro 401 - "Email ou senha incorretos"

---

### 4. Tentar Criar Usuário com Email Duplicado
```http
POST http://localhost:3000/api/users
Authorization: Bearer {SEU_TOKEN_ADMIN}
Content-Type: application/json

{
  "nome": "Teste",
  "email": "admin@interfix.com",
  "senha": "teste123",
  "tipo_usuario": "tecnico"
}
```
**Esperado:** Erro 400 - "Email já cadastrado"

---

## 📝 Variáveis de Ambiente para Postman

Se usar Postman, crie essas variáveis:

| Variável | Valor |
|----------|-------|
| `base_url` | `http://localhost:3000` |
| `token` | (será preenchido após login) |

Depois use assim:
```
{{base_url}}/api/auth/login
Authorization: Bearer {{token}}
```

---

## 💾 Importar para Postman

1. Copie todo o conteúdo deste arquivo
2. Abra Postman
3. File > Import > Raw Text
4. Cole o conteúdo
5. Clique em Import

---

## 🎯 Ordem Recomendada de Testes

1. ✅ Health Check (sem autenticação)
2. ✅ Login Admin
3. ✅ Verificar Sessão
4. ✅ Listar Usuários
5. ✅ Criar Novo Usuário
6. ✅ Buscar Usuário
7. ✅ Atualizar Usuário
8. ✅ Login com Novo Usuário
9. ✅ Alterar Senha
10. ✅ Desativar Usuário
11. ✅ Logout
12. ✅ Testes de Segurança

---

**Dica:** Salve suas coleções no Postman para reutilizar! 🚀
