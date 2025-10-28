# InterFix - Sistema de Gestão de Chamados

## 🚀 Backend - API e Autenticação

Sistema backend para gerenciamento de chamados de suporte técnico com autenticação de usuários.

---

## 📋 Estrutura do Projeto

```
Backend/
├── src/
│   ├── controllers/        # Lógica de negócio
│   │   ├── AuthController.js
│   │   └── UserController.js
│   ├── middleware/         # Middlewares (autenticação)
│   │   └── auth.js
│   ├── models/            # Modelos de dados
│   │   └── User.js
│   ├── routes/            # Rotas da API
│   │   ├── auth.js
│   │   ├── users.js
│   │   └── pages.js
│   ├── views/             # Páginas HTML
│   │   ├── login.html
│   │   ├── login.js
│   │   ├── MenuPrincipal.html
│   │   ├── menu.js
│   │   └── ...
│   └── config/            # Configurações
│       └── create-users-table.sql
├── .env                   # Variáveis de ambiente
├── db.js                  # Conexão com banco de dados
├── server.js              # Servidor principal
└── package.json

```

---

## ⚙️ Instalação

### 1. Instalar dependências

```bash
npm install
```

### 2. Configurar banco de dados

Execute o script SQL no Azure SQL Server:

```bash
# No Azure Data Studio ou SQL Server Management Studio:
# Abra e execute: src/config/create-users-table.sql
```

### 3. Iniciar servidor

```bash
# Modo produção
npm start

# Modo desenvolvimento (com nodemon)
npm run dev
```

---

## 🔐 Autenticação

### Sistema de Login

O sistema usa autenticação baseada em **tokens de sessão**:

1. Usuário faz login com email e senha
2. Sistema retorna um token de autenticação
3. Token é armazenado no `localStorage` do navegador
4. Token é enviado em todas as requisições protegidas

### Credenciais Padrão

Após executar o script SQL, você terá:

**Administrador:**
- Email: `admin@interfix.com`
- Senha: `admin123`

**Técnico:**
- Email: `tecnico@interfix.com`
- Senha: `tecnico123`

---

## 📡 Rotas da API

### Autenticação (`/api/auth`)

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/auth/login` | Login de usuário | ❌ |
| POST | `/api/auth/logout` | Logout de usuário | ✅ |
| GET | `/api/auth/verify` | Verifica sessão | ✅ |
| POST | `/api/auth/change-password` | Altera senha | ✅ |
| POST | `/api/auth/forgot-password` | Recupera senha | ❌ |

### Usuários (`/api/users`)

| Método | Rota | Descrição | Auth | Admin |
|--------|------|-----------|------|-------|
| GET | `/api/users` | Lista todos usuários | ✅ | ✅ |
| GET | `/api/users/:id` | Busca usuário por ID | ✅ | ✅ |
| POST | `/api/users` | Cria novo usuário | ✅ | ✅ |
| PUT | `/api/users/:id` | Atualiza usuário | ✅ | ✅ |
| PATCH | `/api/users/:id/deactivate` | Desativa usuário | ✅ | ✅ |
| PATCH | `/api/users/:id/activate` | Reativa usuário | ✅ | ✅ |

### Páginas HTML

| Rota | Descrição |
|------|-----------|
| `/` | Redireciona para `/login` |
| `/login` | Tela de login |
| `/menu` | Menu principal |
| `/registrar-chamado` | Registrar novo chamado |
| `/chamados` | Visualizar chamados |
| `/adicionar-usuario` | Adicionar usuário (admin) |

---

## 📝 Exemplos de Uso

### Login

```javascript
// POST /api/auth/login
{
  "email": "admin@interfix.com",
  "senha": "admin123"
}

// Resposta
{
  "success": true,
  "message": "Login realizado com sucesso!",
  "token": "abc123...",
  "user": {
    "id": 1,
    "nome": "Administrador",
    "email": "admin@interfix.com",
    "tipo_usuario": "admin"
  }
}
```

### Criar Usuário

```javascript
// POST /api/users
// Headers: Authorization: Bearer {token}
{
  "nome": "Carlos Souza",
  "email": "carlos@interfix.com",
  "senha": "carlos123",
  "tipo_usuario": "tecnico"
}

// Resposta
{
  "success": true,
  "message": "Usuário criado com sucesso!",
  "user": {
    "id": 4,
    "nome": "Carlos Souza",
    "email": "carlos@interfix.com",
    "tipo_usuario": "tecnico"
  }
}
```

### Listar Usuários

```javascript
// GET /api/users
// Headers: Authorization: Bearer {token}

// Resposta
{
  "success": true,
  "users": [
    {
      "id": 1,
      "nome": "Administrador",
      "email": "admin@interfix.com",
      "tipo_usuario": "admin",
      "ativo": true,
      "data_criacao": "2025-01-15T10:00:00"
    },
    // ...
  ]
}
```

---

## 🔒 Níveis de Acesso

### Admin
- Gerenciar usuários
- Ver todos os chamados
- Gerar relatórios
- Acessar todas as funcionalidades

### Técnico
- Registrar chamados
- Ver chamados atribuídos
- Atualizar status de chamados
- Ver relatórios básicos

### Cliente
- Abrir chamados
- Ver próprios chamados
- Acompanhar status

---

## 🛠️ Tecnologias Utilizadas

- **Node.js** - Runtime JavaScript
- **Express** - Framework web
- **mssql** - Driver SQL Server
- **dotenv** - Variáveis de ambiente
- **crypto** - Hash de senhas (SHA256)

---

## 🔐 Segurança

### Senha
- Senhas são hashadas com SHA256
- **Recomendação:** Migrar para bcrypt em produção

### Sessões
- Sessões armazenadas em memória
- Expiram após 24 horas
- **Recomendação:** Usar JWT ou Redis em produção

### Validações
- Email único
- Senha mínima de 6 caracteres
- Verificação de permissões por rota

---

## 📌 Próximos Passos

### Para Produção:
1. [ ] Implementar JWT ao invés de sessões em memória
2. [ ] Usar bcrypt para hash de senhas
3. [ ] Adicionar rate limiting
4. [ ] Implementar CORS
5. [ ] Adicionar logs estruturados
6. [ ] Configurar HTTPS
7. [ ] Implementar refresh tokens

### Funcionalidades:
1. [ ] CRUD de Chamados
2. [ ] Upload de arquivos
3. [ ] Notificações em tempo real
4. [ ] Relatórios e dashboards
5. [ ] Histórico de atividades
6. [ ] Recuperação de senha por email

---

## 🐛 Troubleshooting

### Erro de conexão com banco
```
❌ Erro ao conectar no banco: ConnectionError
```
**Solução:** Verifique:
1. Credenciais no `.env`
2. Firewall do Azure (adicione seu IP)
3. Servidor está online

### Token inválido
```
{
  "success": false,
  "message": "Sessão inválida ou expirada"
}
```
**Solução:** Faça login novamente

### Não consigo criar usuário
```
{
  "success": false,
  "message": "Email já cadastrado"
}
```
**Solução:** Use outro email ou verifique usuários existentes

---

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique os logs do servidor
2. Execute `node db.js` para testar conexão
3. Acesse `/api/health` para verificar status

---

## 📄 Licença

Este projeto é privado e proprietário da InterFix.

---

**Desenvolvido com ❤️ para InterFix**
