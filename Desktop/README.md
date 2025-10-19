# Sistema de Chamados - C# Windows Forms

Sistema orientado a objetos para gerenciamento de chamados técnicos desenvolvido em C# com Windows Forms e SQL Server.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Uso](#uso)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Classes Principais](#classes-principais)
- [Banco de Dados](#banco-de-dados)
- [Funcionalidades](#funcionalidades)
- [Troubleshooting](#troubleshooting)

## 🎯 Visão Geral

O Sistema de Chamados é uma aplicação desktop desenvolvida para gerenciar solicitações de suporte técnico em empresas. O sistema permite o controle de chamados, usuários e possui diferentes níveis de acesso (Administrador e Técnico).

### Principais Características:
- **Arquitetura Orientada a Objetos** com herança e polimorfismo
- **Interface Windows Forms** moderna e intuitiva
- **Banco de dados SQL Server** com procedures e triggers
- **Sistema de autenticação** com diferentes níveis de acesso
- **Controle de status e prioridades** de chamados
- **Auditoria e logs** de operações

## 🏗️ Arquitetura

O sistema segue o padrão **MVC (Model-View-Controller)** com as seguintes camadas:

```
├── Models/          # Modelos de dados (Funcionários, Chamados)
├── Controllers/     # Lógica de negócio
├── Forms/          # Interface do usuário (Windows Forms)
├── Data/           # Acesso a dados (SQL Server)
├── Interfaces/     # Contratos e interfaces
├── Config/         # Configurações do sistema
└── Database/       # Scripts SQL
```

## 📋 Pré-requisitos

### Software Necessário:
- **Windows 10/11** ou Windows Server 2016+
- **.NET Framework 4.7.2** ou superior
- **SQL Server 2016** ou superior (Express, Standard, Enterprise)
- **Visual Studio 2019/2022** (para desenvolvimento)

### Permissões Necessárias:
- Acesso ao SQL Server com permissões de criação de banco
- Permissões de escrita na pasta de logs (`C:\Logs\SistemaChamados\`)

## 🚀 Instalação

### 1. Preparar o Banco de Dados

#### Opção A: SQL Server Express (Recomendado para desenvolvimento)
```sql
-- 1. Instalar SQL Server Express
-- 2. Conectar usando SQL Server Management Studio (SSMS)
-- 3. Criar o banco de dados
CREATE DATABASE SistemaChamados;
```

#### Opção B: SQL Server Completo
```sql
-- Conectar ao servidor SQL Server
-- Executar o script de criação
USE master;
CREATE DATABASE SistemaChamados;
```

### 2. Executar Scripts de Criação

```sql
-- Executar o arquivo: src/Database/CreateTables.sql
-- Este script criará:
-- - Tabelas (Funcionarios, Chamados, ChamadosAuditoria, Categorias)
-- - Índices para performance
-- - Constraints de integridade
-- - Triggers de auditoria
-- - Dados iniciais (usuário admin padrão)
```

### 3. Configurar String de Conexão

Edite o arquivo `App.config`:

```xml
<connectionStrings>
    <!-- Para SQL Server Express -->
    <add name="SistemaChamados" 
         connectionString="Server=.\SQLEXPRESS;Database=SistemaChamados;Integrated Security=true;" />
    
    <!-- Para SQL Server com usuário/senha -->
    <add name="SistemaChamados" 
         connectionString="Server=SEU_SERVIDOR;Database=SistemaChamados;User Id=SEU_USUARIO;Password=SUA_SENHA;" />
</connectionStrings>
```

### 4. Compilar e Executar

```bash
# No Visual Studio:
# 1. Abrir a solution
# 2. Build > Build Solution (Ctrl+Shift+B)
# 3. Debug > Start Debugging (F5)

# Ou via linha de comando:
msbuild SistemaChamados.sln /p:Configuration=Release
```

## ⚙️ Configuração

### Configurações Principais (App.config)

```xml
<appSettings>
    <!-- Segurança -->
    <add key="SessionTimeoutMinutes" value="30" />
    <add key="MaxLoginAttempts" value="3" />
    
    <!-- Logs -->
    <add key="LogPath" value="C:\Logs\SistemaChamados\" />
    <add key="EnableLogging" value="true" />
    
    <!-- Email (opcional) -->
    <add key="EnableEmailNotifications" value="false" />
    <add key="SmtpServer" value="smtp.gmail.com" />
</appSettings>
```

### Primeiro Acesso

**Usuário Administrador Padrão:**
- **Email:** `admin@sistema.com`
- **Senha:** `admin123`

⚠️ **IMPORTANTE:** Altere a senha padrão após o primeiro login!

## 📖 Uso

### 1. Login no Sistema
- Execute a aplicação
- Use as credenciais do administrador padrão
- O sistema detectará automaticamente o tipo de usuário

### 2. Funcionalidades por Perfil

#### 👨‍💼 Administrador (ADM)
- ✅ Visualizar todos os chamados
- ✅ Adicionar/remover funcionários/técnicos
- ✅ Alterar senhas de qualquer usuário
- ✅ Gerar relatórios completos
- ✅ Configurar categorias e prioridades

#### 🔧 Técnico
- ✅ Visualizar chamados atribuídos
- ✅ Marcar chamados como resolvidos
- ✅ Alterar prioridade de chamados
- ✅ Adicionar comentários/contestações
- ✅ Gerar relatórios completos

####  Funcionário Comum
- ✅ Criar novos chamados
- ✅ Visualizar status dos próprios chamados
- ✅ Adicionar comentários/contestações na criação
- ✅ Receber notificações por email (se configurado)

####  IA (Inteligência Artificial)
- ✅ Atribuir técnicos automaticamente com base na carga de trabalho
- ✅ Sugerir prioridades com base na descrição do chamado



### 3. Fluxo de Trabalho Típico

```
1. Funcionário cria chamado
2. IA atribui técnico
3. Técnico trabalha no chamado
4. Técnico marca como resolvido
5. Técnico fecha o chamado
```

## 📁 Estrutura do Projeto

```
SistemaChamados/
│
├── Properties/
│   ├── AssemblyInfo.cs
│   └── Resources.resx
│   └── Settings.settings
|
|
├── src/
│   ├── Models/
│   │   ├── Funcionarios.cs      # Classe base
|   |   |   Funcionario.cs       # Herda de Funcionarios
│   │   ├── Tecnico.cs           # Herda de Funcionarios
│   │   ├── ADM.cs               # Herda de Funcionarios
│   │   └── Chamados.cs          # Modelo de chamados
│   │
│   ├── Controllers/
│   │   ├── FuncionariosController.cs
│   │   └── ChamadosController.cs
│   │
│   ├── Forms/
│   │   └── LoginForm.cs             # Interface de login
│   │   └── AlterarPrioridadeForm.cs # Interface para alterar prioridade
│   │   └── AlterarStatusForm.cs     # Interface para alterar status
│   │   └── ContestacaoForm.cs       # Interface para contestações
│   │   └── CriarChamadoForm.cs      # Interface para criar chamados
│   │   └── DetalhesChamadoForm.cs   # Interface para detalhes do chamado
│   │   └── EditarUsuarioForm.cs     # Interface para editar usuário
│   │   └── GerenciarChamadosForm.cs # Interface para gerenciar chamados
│   │   └── GerenciarUsuariosForm.cs # Interface para gerenciar usuarios
│   │   └── NovoUsuarioForm.cs       # Interface para gerenciar chamados
│   │   └── VisualizarChamadosForm.cs# Interface para gerenciar chamados
│   │
│   ├── Data/
│   │   └── SqlServerConnection.cs
│   │
│   ├── Interfaces/
│   │   └── IDatabaseConnection.cs
│   │
│   ├── Config/
│   │   └── DatabaseConfig.cs
│   │
│   ├── Database/
│   │   └── SistemaChamados.sql
│   │
│   └── Program.cs               # Ponto de entrada
│
├── App.config                   # Configurações
└── README.md                    # Este arquivo
```

## 🏛️ Classes Principais

### Hierarquia de Classes

```csharp
// Classe Base
public abstract class Funcionarios
{
    public char Nome { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public int Id { get; set; }
    public int NivelAcesso { get; set; }
    
    public virtual void VisualizarChamados() { }
}

// Classes Filhas
public class Tecnico : Funcionarios
{
    public void MarcarChamadoResolvido(int idChamado) { }
    public void TrocarPrioridadeDoChamado(int idChamado, int novaPrioridade) { }
}

public class ADM : Funcionarios
{
    public void AdicionarUsuarios(Funcionarios novoFuncionario) { }
    public void AlterarSenha(int idFuncionario, string novaSenha) { }
}

public class Funcionario : Funcionarios
{
    public void CriarChamado(string categoria, string descricao) { }
    public void VerStatusChamado(int idChamado) { }
}

// Classe Independente
public class Chamados
{
    public int IdChamado { get; set; }
    public string Categoria { get; set; }
    public string Contestacoes { get; set; }
    public int Prioridade { get; set; }
    public string Descricao { get; set; }
    public int Afetado { get; set; }
    public DateTime DataChamado { get; set; }
    
    public void CriarChamado() { }
    public void AlterarPrioridade(int novaPrioridade) { }
    public void AlterarStatus(StatusChamado novoStatus) { }
}
```

## 🗄️ Banco de Dados

### Estrutura das Tabelas

#### Funcionarios
```sql
CREATE TABLE Funcionarios (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Nome nvarchar(100) NOT NULL,
    Cpf nvarchar(14) UNIQUE NOT NULL,
    Email nvarchar(100) UNIQUE NOT NULL,
    Senha nvarchar(255) NOT NULL,
    NivelAcesso int NOT NULL,
    DataCriacao datetime DEFAULT GETDATE(),
    Ativo bit DEFAULT 1
);
```

#### Chamados
```sql
CREATE TABLE Chamados (
    IdChamado int IDENTITY(1,1) PRIMARY KEY,
    Categoria nvarchar(50) NOT NULL,
    Contestacoes ntext NULL,
    Prioridade int DEFAULT 2,
    Descricao ntext NOT NULL,
    Afetado int NOT NULL,
    DataChamado datetime DEFAULT GETDATE(),
    Status int DEFAULT 1,
    TecnicoResponsavel int NULL,
    DataResolucao datetime NULL,
    FOREIGN KEY (Afetado) REFERENCES Funcionarios(Id),
    FOREIGN KEY (TecnicoResponsavel) REFERENCES Funcionarios(Id)
);
```

### Enumerações

```csharp
public enum StatusChamado
{
    Aberto = 1,
    EmAndamento = 2,
    Resolvido = 3,
    Fechado = 4,
    Cancelado = 5
}

public enum PrioridadeChamado
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}
```

## ✨ Funcionalidades

### 🔐 Autenticação e Autorização
- Login com email e senha
- Controle de sessão
- Diferentes níveis de acesso
- Validação de permissões por operação

### 📋 Gerenciamento de Chamados
- Criação de novos chamados
- Atribuição de técnicos
- Controle de status (Aberto → Em Andamento → Resolvido → Fechado)
- Sistema de prioridades (Baixa, Média, Alta, Crítica)
- Histórico de alterações

### 👥 Gerenciamento de Usuários
- Cadastro de funcionários (ADM)
- Alteração de senhas
- Controle de perfis (Técnico/Administrador)
- Listagem e busca de usuários

### 📊 Relatórios e Estatísticas
- Relatório geral de chamados
- Relatórios por período
- Estatísticas por técnico
- Métricas de performance

### 🔍 Auditoria e Logs
- Log de todas as operações
- Histórico de alterações em chamados
- Rastreamento de ações por usuário
- Backup automático de dados

## 🔧 Troubleshooting

### Problemas Comuns

#### 1. Erro de Conexão com Banco
```
Erro: "Cannot connect to SQL Server"
Solução:
- Verificar se SQL Server está rodando
- Confirmar string de conexão no App.config
- Testar conectividade com SSMS
```

#### 2. Usuário Admin Não Funciona
```
Erro: "Email ou senha incorretos"
Solução:
- Verificar se o script CreateTables.sql foi executado
- Confirmar dados: admin@sistema.com / admin123
- Verificar tabela Funcionarios no banco
```

#### 3. Erro de Permissão de Arquivo
```
Erro: "Access denied to path C:\Logs\"
Solução:
- Executar aplicação como Administrador
- Ou alterar LogPath no App.config para pasta com permissão
```

#### 4. Erro de Assembly
```
Erro: "Could not load file or assembly"
Solução:
- Verificar .NET Framework 4.7.2 instalado
- Rebuild da solution
- Verificar referências do projeto
```

### Logs e Diagnóstico

#### Localização dos Logs
```
C:\Logs\SistemaChamados\
├── error_YYYYMMDD.log    # Logs de erro
├── info_YYYYMMDD.log     # Logs informativos
└── trace.log             # Trace do sistema
```

#### Verificar Conectividade
```csharp
// Teste manual de conexão
var config = new DatabaseConfig();
bool conectado = config.TestarConexao();
Console.WriteLine($"Conexão: {(conectado ? "OK" : "FALHOU")}");
```

### Configurações de Performance

#### Para Ambientes de Produção
```xml
<appSettings>
    <!-- Otimizações -->
    <add key="ConnectionPoolSize" value="100" />
    <add key="CommandTimeout" value="60" />
    <add key="EnableQueryCache" value="true" />
    
    <!-- Logs menos verbosos -->
    <add key="LogLevel" value="Warning" />
</appSettings>
```

## 📞 Suporte

Para suporte técnico ou dúvidas:

1. **Documentação:** Consulte este README
2. **Logs:** Verifique os arquivos de log em `C:\Logs\SistemaChamados\`
3. **Banco de Dados:** Use SQL Server Management Studio para diagnósticos
4. **Código:** Revise os comentários no código-fonte

## 📄 Licença

Este projeto foi desenvolvido como sistema educacional/empresarial. 

---

**Sistema de Chamados v1.0.0**  
Desenvolvido em C# com Windows Forms e SQL Server  
© 2024 - Todos os direitos reservados
