-- =============================================
-- BASE DE DATOS: Suporte_Tecnico
-- Sistema de Chamados - Versión Mejorada
-- =============================================

-- Crear base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Suporte_Tecnico')
BEGIN
    CREATE DATABASE Suporte_Tecnico;
END
GO

USE Suporte_Tecnico;
GO

-- =============================================
-- 1. TABLA: Nivel_de_acesso (debe crearse primero)
-- =============================================
IF OBJECT_ID('Nivel_de_acesso', 'U') IS NOT NULL
    DROP TABLE Nivel_de_acesso;

CREATE TABLE Nivel_de_acesso (
    codigo INT PRIMARY KEY,
    Nivel_acesso VARCHAR(20) NOT NULL
);

-- Insertar niveles
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (1, 'Funcionario');
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (2, 'Administrador');
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (3, 'Tecnico');

-- =============================================
-- 2. TABLA: Usuario
-- =============================================
IF OBJECT_ID('Usuario', 'U') IS NOT NULL
    DROP TABLE Usuario;

CREATE TABLE Usuario (
    Id_usuario INT PRIMARY KEY IDENTITY(1,1),
    nome VARCHAR(100) NOT NULL,
    senha VARCHAR(100) NOT NULL,
    Cpf CHAR(11) NOT NULL UNIQUE,
    Acess_codigo INT NOT NULL,
    DataCadastro DATETIME DEFAULT GETDATE(),
    Ativo BIT DEFAULT 1,
    CONSTRAINT FK_Usuario_Nivel FOREIGN KEY (Acess_codigo) 
        REFERENCES Nivel_de_acesso(codigo)
);

-- =============================================
-- 3. TABLA: E_mail
-- =============================================
IF OBJECT_ID('E_mail', 'U') IS NOT NULL
    DROP TABLE E_mail;

CREATE TABLE E_mail (
    Id INT PRIMARY KEY IDENTITY(1,1),
    E_mail VARCHAR(100) NOT NULL UNIQUE,
    Id_usuario INT NOT NULL,
    CONSTRAINT FK_Email_Usuario FOREIGN KEY (Id_usuario) 
        REFERENCES Usuario(Id_usuario) ON DELETE CASCADE
);

-- =============================================
-- 4. TABLA: Contestacoes
-- =============================================
IF OBJECT_ID('Contestacoes', 'U') IS NOT NULL
    DROP TABLE Contestacoes;

CREATE TABLE Contestacoes (
    Codigo INT PRIMARY KEY IDENTITY(1,1),
    Justificativa VARCHAR(500),
    DataContestacao DATETIME DEFAULT GETDATE()
);

-- =============================================
-- 5. TABLA: chamados (CON TECNICO ATRIBUIDO)
-- =============================================
IF OBJECT_ID('chamados', 'U') IS NOT NULL
    DROP TABLE chamados;

CREATE TABLE chamados (
    id_chamado INT PRIMARY KEY IDENTITY(1,1),
    categoria VARCHAR(20) NOT NULL,
    prioridade INT NOT NULL DEFAULT 2, -- 1=Baixa, 2=Media, 3=Alta, 4=Critica
    descricao VARCHAR(1000) NOT NULL,
    Afetado INT NOT NULL, -- ID do usuario solicitante
    Data_Registro DATETIME NOT NULL DEFAULT GETDATE(),
    Status INT NOT NULL DEFAULT 1, -- 1=Aberto, 2=EmAndamento, 3=Resolvido, 4=Fechado, 5=Cancelado
    Solucao VARCHAR(1000),
    Contestacoes_Codigo INT,
    Tecnico_Atribuido INT, -- *** NUEVA COLUMNA ***
    DataResolucao DATETIME,
    CONSTRAINT FK_Chamados_Afetado FOREIGN KEY (Afetado) 
        REFERENCES Usuario(Id_usuario),
    CONSTRAINT FK_Chamados_Tecnico FOREIGN KEY (Tecnico_Atribuido) 
        REFERENCES Usuario(Id_usuario),
    CONSTRAINT FK_Chamados_Contestacoes FOREIGN KEY (Contestacoes_Codigo) 
        REFERENCES Contestacoes(Codigo) ON DELETE SET NULL
);

-- =============================================
-- 6. TABLA: registra
-- =============================================
IF OBJECT_ID('registra', 'U') IS NOT NULL
    DROP TABLE registra;

CREATE TABLE registra (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Id_usuario INT NOT NULL,
    id_chamado INT NOT NULL,
    DataRegistro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Registra_Usuario FOREIGN KEY (Id_usuario) 
        REFERENCES Usuario(Id_usuario),
    CONSTRAINT FK_Registra_Chamado FOREIGN KEY (id_chamado) 
        REFERENCES chamados(id_chamado) ON DELETE CASCADE
);

-- =============================================
-- 7. DATOS DE PRUEBA
-- =============================================

-- Usuarios de prueba
SET IDENTITY_INSERT Usuario ON;

INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, Ativo) VALUES 
(1, 'Christian', 'MinhaSenha', '12345678912', 2, 1);

INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, Ativo) VALUES 
(2, 'Juan', 'senhaJuan', '21987654321', 3, 1);

INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, Ativo) VALUES 
(3, 'Theo', 'senhaTheo', '10192838374', 1, 1);

INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, Ativo) VALUES 
(4, 'Nycolas', 'senhaNycolas', '65473923981', 1, 1);

SET IDENTITY_INSERT Usuario OFF;

-- Emails
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('chriscamplopes@gmail.com', 1);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('Juan@gmail.com', 2);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('theo@gmail.com', 3);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('nycolas@gmail.com', 4);

-- Contestaciones
SET IDENTITY_INSERT Contestacoes ON;
INSERT INTO Contestacoes (Codigo, Justificativa) VALUES 
(1, 'O problema é critico pois se eu não conseguir entregar os relatórios a empresa vai parar');
SET IDENTITY_INSERT Contestacoes OFF;

-- Chamados de exemplo
SET IDENTITY_INSERT chamados ON;
INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Solucao, Contestacoes_Codigo, Tecnico_Atribuido) VALUES 
(1, 'Software', 1, 'O sistema do computador esta com problema', 3, '2025-01-05', 1, NULL, 1, 2);
SET IDENTITY_INSERT chamados OFF;

-- Registro
INSERT INTO registra (Id_usuario, id_chamado) VALUES (3, 1);

-- =============================================
-- 8. VIEWS ÚTILES PARA O SISTEMA
-- =============================================

-- View para login com email
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
    DROP VIEW vw_LoginUsuarios;
GO

CREATE VIEW vw_LoginUsuarios AS
SELECT 
    u.Id_usuario as Id,
    u.nome as Nome,
    u.senha as Senha,
    u.Cpf,
    e.E_mail as Email,
    u.Acess_codigo as NivelAcesso,
    n.Nivel_acesso as TipoFuncionario,
    u.DataCadastro,
    u.Ativo
FROM Usuario u
INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario;
GO

-- View para chamados completos
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_ChamadosCompletos')
    DROP VIEW vw_ChamadosCompletos;
GO

CREATE VIEW vw_ChamadosCompletos AS
SELECT 
    c.id_chamado as IdChamado,
    c.categoria as Categoria,
    c.prioridade as Prioridade,
    c.descricao as Descricao,
    c.Afetado,
    u1.nome as NomeSolicitante,
    c.Data_Registro as DataChamado,
    c.Status,
    c.Solucao,
    c.Tecnico_Atribuido as TecnicoResponsavel,
    u2.nome as NomeTecnico,
    c.DataResolucao,
    c.Contestacoes_Codigo,
    cont.Justificativa as Contestacao
FROM chamados c
INNER JOIN Usuario u1 ON c.Afetado = u1.Id_usuario
LEFT JOIN Usuario u2 ON c.Tecnico_Atribuido = u2.Id_usuario
LEFT JOIN Contestacoes cont ON c.Contestacoes_Codigo = cont.Codigo;
GO

-- =============================================
-- 9. STORED PROCEDURES
-- =============================================

-- SP para validar login
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ValidarLogin')
    DROP PROCEDURE sp_ValidarLogin;
GO

CREATE PROCEDURE sp_ValidarLogin
    @Email VARCHAR(100),
    @Senha VARCHAR(100)
AS
BEGIN
    SELECT * FROM vw_LoginUsuarios
    WHERE Email = @Email 
      AND Senha = @Senha 
      AND Ativo = 1;
END
GO

-- SP para listar técnicos
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ListarTecnicos')
    DROP PROCEDURE sp_ListarTecnicos;
GO

CREATE PROCEDURE sp_ListarTecnicos
AS
BEGIN
    SELECT 
        u.Id_usuario as Id,
        u.nome as Nome,
        e.E_mail as Email,
        u.Cpf,
        u.Acess_codigo as NivelAcesso
    FROM Usuario u
    LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE u.Acess_codigo IN (2, 3) -- Admin e Técnico
      AND u.Ativo = 1;
END
GO

-- =============================================
-- 10. CONSULTAS DE VERIFICACIÓN
-- =============================================

PRINT '=== TABELAS CRIADAS ==='
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

PRINT '=== USUARIOS ==='
SELECT u.Id_usuario, u.nome, u.Cpf, n.Nivel_acesso, e.E_mail 
FROM Usuario u
LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario;

PRINT '=== CHAMADOS ==='
SELECT * FROM vw_ChamadosCompletos;

PRINT '=== MIGRATION COMPLETA COM SUCESSO ==='




---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- ============================================
-- SCRIPT DE CRIAÇÃO DO BANCO DE DADOS
-- Sistema de Chamados InterFix
-- ============================================

USE master;
GO

-- Verificar se o banco existe e excluí-lo
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Suporte_Tecnico')
BEGIN
    ALTER DATABASE SistemaChamados SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaChamados;
    PRINT '✓ Banco de dados anterior removido';
END
GO

-- Criar novo banco de dados
CREATE DATABASE Suporte_Tecnico
USE Suporte_Tecnico;
GO

-- ============================================
-- TABELA: Nivel_de_acesso
-- Descrição: Armazena os níveis de acesso do sistema
-- ============================================
CREATE TABLE Nivel_de_acesso (
    codigo DECIMAL(18, 0) NOT NULL PRIMARY KEY,
    Nivel_acesso VARCHAR(20) NULL,
    CONSTRAINT CK_Nivel_Acesso_Valido CHECK (codigo BETWEEN 1 AND 3)
);
GO

PRINT '✓ Tabela Nivel_de_acesso criada';
GO

-- ============================================
-- TABELA: Usuario
-- Descrição: Armazena informações dos usuários do sistema
-- ============================================
CREATE TABLE Usuario (
    Id_usuario INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    senha VARCHAR(100) NOT NULL, 
    Cpf CHAR(11) NOT NULL UNIQUE,
    Acess_codigo DECIMAL(18, 0) NOT NULL,
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE(),
    Ativo BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_Usuario_NivelAcesso 
        FOREIGN KEY (Acess_codigo) 
        REFERENCES Nivel_de_acesso(codigo),
    
    CONSTRAINT CK_Cpf_Valido CHECK (LEN(Cpf) = 11),
    CONSTRAINT CK_Senha_Nao_Vazia CHECK (LEN(senha) > 0)
);
GO

PRINT '✓ Tabela Usuario criada';
GO

-- Índices para melhor performance
CREATE INDEX IX_Usuario_Cpf ON Usuario(Cpf);
CREATE INDEX IX_Usuario_Ativo ON Usuario(Ativo);
GO

-- ============================================
-- TABELA: E_mail
-- Descrição: Armazena e-mails dos usuários
-- ============================================
CREATE TABLE E_mail (
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    E_mail VARCHAR(100) NOT NULL UNIQUE,
    Id_usuario INT NOT NULL,
    
    CONSTRAINT FK_Email_Usuario 
        FOREIGN KEY (Id_usuario) 
        REFERENCES Usuario(Id_usuario)
        ON DELETE CASCADE,
    
    CONSTRAINT CK_Email_Valido CHECK (E_mail LIKE '%_@__%.__%')
);
GO

PRINT '✓ Tabela E_mail criada';
GO

CREATE INDEX IX_Email_Usuario ON E_mail(Id_usuario);
GO

-- ============================================
-- TABELA: Contestacoes
-- Descrição: Armazena contestações de chamados
-- ============================================
CREATE TABLE Contestacoes (
    Codigo INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Justificativa VARCHAR(500) NULL,
    DataContestacao DATETIME NULL DEFAULT GETDATE()
);
GO

PRINT '✓ Tabela Contestacoes criada';
GO

-- ============================================
-- TABELA: chamados
-- Descrição: Armazena todos os chamados do sistema
-- ============================================
CREATE TABLE chamados (
    id_chamado INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    categoria VARCHAR(20) NOT NULL,
    prioridade INT NOT NULL,
    descricao VARCHAR(1000) NOT NULL,
    Afetado INT NOT NULL,
    Data_Registro DATETIME NOT NULL DEFAULT GETDATE(),
    Status INT NOT NULL DEFAULT 1,
    Solucao VARCHAR(1000) NULL,
    Contestacoes_Codigo DECIMAL(18, 0) NULL,
    Tecnico_Atribuido INT NULL,
    Data_Resolucao DATETIME NULL,
    
    CONSTRAINT FK_Chamado_Afetado 
        FOREIGN KEY (Afetado) 
        REFERENCES Usuario(Id_usuario),
    
    CONSTRAINT FK_Chamado_Tecnico 
        FOREIGN KEY (Tecnico_Atribuido) 
        REFERENCES Usuario(Id_usuario),
    
    CONSTRAINT CK_Prioridade_Valida CHECK (prioridade BETWEEN 1 AND 4),
    CONSTRAINT CK_Status_Valido CHECK (Status BETWEEN 1 AND 5),
    CONSTRAINT CK_Categoria_Valida CHECK (categoria IN ('Hardware', 'Software', 'Rede', 'Outros'))
);
GO

PRINT '✓ Tabela chamados criada';
GO

-- Índices para melhor performance
CREATE INDEX IX_Chamados_Status ON chamados(Status);
CREATE INDEX IX_Chamados_Prioridade ON chamados(prioridade);
CREATE INDEX IX_Chamados_Afetado ON chamados(Afetado);
CREATE INDEX IX_Chamados_Tecnico ON chamados(Tecnico_Atribuido);
CREATE INDEX IX_Chamados_Data ON chamados(Data_Registro);
GO

-- ============================================
-- TABELA: registra
-- Descrição: Tabela de relacionamento usuário-chamado
-- ============================================
CREATE TABLE registra (
    Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Id_usuario INT NOT NULL,
    id_chamado INT NOT NULL,
    DataRegistro DATETIME NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Registra_Usuario 
        FOREIGN KEY (Id_usuario) 
        REFERENCES Usuario(Id_usuario),
    
    CONSTRAINT FK_Registra_Chamado 
        FOREIGN KEY (id_chamado) 
        REFERENCES chamados(id_chamado)
        ON DELETE CASCADE
);
GO

PRINT '✓ Tabela registra criada';
GO

CREATE INDEX IX_Registra_Usuario ON registra(Id_usuario);
CREATE INDEX IX_Registra_Chamado ON registra(id_chamado);
GO

-- ============================================
-- INSERÇÃO DE DADOS INICIAIS
-- ============================================

-- Inserir níveis de acesso
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES 
(1, 'Funcionário'),
(2, 'Técnico'),
(3, 'Administrador');
GO

PRINT '✓ Níveis de acesso inseridos';
GO

-- Inserir usuários padrão
-- Senha padrão para todos: "Senha123" (Hash SHA256)
-- Hash SHA256 de "Senha123" = 96b3984481c494d898901c2a46c55a210a4c79e766edad69cc5cd54284b710d6

INSERT INTO Usuario (nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) VALUES 
-- Administrador
('Christopher Camp', 'ecf91daa4f7f26e51ab52fe4946c8afd5c81287c75dd118e67924ee2df11713d', '12345678900', 3, GETDATE(), 1),

-- Técnicos
('Juan Silva', '96b3984481c494d898901c2a46c55a210a4c79e766edad69cc5cd54284b710d6', '21987654321', 2, GETDATE(), 1),
('Pedro Costa', 'c9db6be15126820596164ba032ae917a1374de4ff1ed05d524b1cea54bafdf56', '11111111111', 2, GETDATE(), 1),

-- Funcionários
('Theo Santos', '1bc6757cf870d000fd8ba617a53655ae3a448fa2d199cfb29529c586d1cea8c5', '10192838374', 1, GETDATE(), 1),
('Nycolas Costa', 'd0c8c4f2cbc5bc15cdf852ce356735ddaaceabc1f3a25f7de270192df2c67eb5', '65473923981', 1, GETDATE(), 1),
('André Silva', 'ad491c4a5a02bd78e608edec4cf12431ab548dabaf1e5c26b5c196f2fd0ce32e', '32165498700', 1, GETDATE(), 1);
GO

PRINT '✓ Usuários padrão inseridos';
GO

-- Inserir e-mails dos usuários
INSERT INTO E_mail (E_mail, Id_usuario) VALUES 
('chriscamplopes@gmail.com', 1),
('juan@gmail.com', 2),
('pedro@gmail.com', 3),
('theo@gmail.com', 4),
('nycolas@gmail.com', 5),
('andre@gmail.com', 6);
GO

PRINT '✓ E-mails inseridos';
GO

-- Inserir chamados de exemplo
INSERT INTO chamados (categoria, prioridade, descricao, Afetado, Data_Registro, Status, Tecnico_Atribuido) VALUES 
('Software', 3, 'Sistema não inicia após atualização', 4, GETDATE()-5, 1, 2),
('Hardware', 2, 'Mouse sem fio não funciona', 5, GETDATE()-4, 1, NULL),
('Rede', 4, 'Sem acesso à internet em todo o setor', 4, GETDATE()-3, 2, 2),
('Hardware', 2, 'TÍTULO: Placa de vídeo com problemas

DESCRIÇÃO: 
A placa de vídeo do computador não está bem instalada e causa travamentos frequentes

AFETADOS: Apenas eu
IMPEDE TRABALHO: Sim', 1, GETDATE()-2, 2, 3),
('Software', 1, 'TÍTULO: Impressora offline

DESCRIÇÃO: 
Impressora mostra mensagem de "offline" mesmo estando conectada

AFETADOS: Meu departamento
IMPEDE TRABALHO: Não', 6, GETDATE()-1, 1, NULL);
GO

PRINT '✓ Chamados de exemplo inseridos';
GO

-- ============================================
-- VIEWS ÚTEIS
-- ============================================

-- View: Chamados com informações completas
CREATE VIEW vw_ChamadosCompletos AS
SELECT 
    c.id_chamado,
    c.categoria,
    c.prioridade,
    CASE c.prioridade
        WHEN 1 THEN 'Baixa'
        WHEN 2 THEN 'Média'
        WHEN 3 THEN 'Alta'
        WHEN 4 THEN 'Crítica'
    END AS PrioridadeTexto,
    c.descricao,
    c.Status,
    CASE c.Status
        WHEN 1 THEN 'Aberto'
        WHEN 2 THEN 'Em Andamento'
        WHEN 3 THEN 'Resolvido'
        WHEN 4 THEN 'Fechado'
        WHEN 5 THEN 'Cancelado'
    END AS StatusTexto,
    c.Data_Registro,
    c.Data_Resolucao,
    c.Solucao,
    uAfetado.nome AS NomeAfetado,
    uAfetado.Id_usuario AS IdAfetado,
    eAfetado.E_mail AS EmailAfetado,
    uTecnico.nome AS NomeTecnico,
    uTecnico.Id_usuario AS IdTecnico,
    eTecnico.E_mail AS EmailTecnico,
    DATEDIFF(HOUR, c.Data_Registro, ISNULL(c.Data_Resolucao, GETDATE())) AS TempoAberto
FROM chamados c
INNER JOIN Usuario uAfetado ON c.Afetado = uAfetado.Id_usuario
LEFT JOIN E_mail eAfetado ON uAfetado.Id_usuario = eAfetado.Id_usuario
LEFT JOIN Usuario uTecnico ON c.Tecnico_Atribuido = uTecnico.Id_usuario
LEFT JOIN E_mail eTecnico ON uTecnico.Id_usuario = eTecnico.Id_usuario;
GO

PRINT '✓ View vw_ChamadosCompletos criada';
GO

-- View: Usuários com informações completas
CREATE VIEW vw_UsuariosCompletos AS
SELECT 
    u.Id_usuario,
    u.nome,
    u.Cpf,
    e.E_mail,
    u.Acess_codigo,
    n.Nivel_acesso,
    u.DataCadastro,
    u.Ativo,
    CASE u.Ativo
        WHEN 1 THEN 'Ativo'
        WHEN 0 THEN 'Inativo'
    END AS StatusTexto
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo;
GO

PRINT '✓ View vw_UsuariosCompletos criada';
GO

-- View: Estatísticas de chamados
CREATE VIEW vw_EstatisticasChamados AS
SELECT 
    COUNT(*) AS TotalChamados,
    SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS Abertos,
    SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS EmAndamento,
    SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS Resolvidos,
    SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END) AS Fechados,
    SUM(CASE WHEN Status = 5 THEN 1 ELSE 0 END) AS Cancelados,
    SUM(CASE WHEN prioridade = 4 THEN 1 ELSE 0 END) AS Criticos,
    SUM(CASE WHEN prioridade = 3 THEN 1 ELSE 0 END) AS Alta,
    AVG(DATEDIFF(HOUR, Data_Registro, ISNULL(Data_Resolucao, GETDATE()))) AS TempoMedioResolucao
FROM chamados;
GO

PRINT '✓ View vw_EstatisticasChamados criada';
GO

-- ============================================
-- STORED PROCEDURES ÚTEIS
-- ============================================

-- SP: Buscar usuário por e-mail e senha
CREATE PROCEDURE sp_ValidarLogin
    @Email VARCHAR(100),
    @SenhaHash VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.Id_usuario,
        u.nome,
        u.Cpf,
        e.E_mail,
        u.senha,
        u.Acess_codigo AS NivelAcesso,
        n.Nivel_acesso AS TipoNivel,
        u.Ativo
    FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
    WHERE e.E_mail = @Email 
      AND u.senha = @SenhaHash
      AND u.Ativo = 1;
END;
GO

PRINT '✓ SP sp_ValidarLogin criada';
GO

-- SP: Criar novo chamado
CREATE PROCEDURE sp_CriarChamado
    @Categoria VARCHAR(20),
    @Prioridade INT,
    @Descricao VARCHAR(1000),
    @IdAfetado INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdChamado INT;
    
    INSERT INTO chamados (categoria, prioridade, descricao, Afetado, Data_Registro, Status)
    VALUES (@Categoria, @Prioridade, @Descricao, @IdAfetado, GETDATE(), 1);
    
    SET @IdChamado = SCOPE_IDENTITY();
    
    -- Registrar na tabela de relacionamento
    INSERT INTO registra (Id_usuario, id_chamado, DataRegistro)
    VALUES (@IdAfetado, @IdChamado, GETDATE());
    
    SELECT @IdChamado AS IdChamadoCriado;
END;
GO

PRINT '✓ SP sp_CriarChamado criada';
GO

-- SP: Atribuir técnico a chamado
CREATE PROCEDURE sp_AtribuirTecnico
    @IdChamado INT,
    @IdTecnico INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE chamados
    SET Tecnico_Atribuido = @IdTecnico,
        Status = 2 -- Em Andamento
    WHERE id_chamado = @IdChamado;
    
    SELECT @@ROWCOUNT AS LinhasAfetadas;
END;
GO

PRINT '✓ SP sp_AtribuirTecnico criada';
GO

-- ============================================
-- TRIGGERS
-- ============================================

-- Trigger: Registrar data de resolução automaticamente
CREATE TRIGGER tr_ChamadoResolvido
ON chamados
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE chamados
    SET Data_Resolucao = GETDATE()
    FROM chamados c
    INNER JOIN inserted i ON c.id_chamado = i.id_chamado
    INNER JOIN deleted d ON c.id_chamado = d.id_chamado
    WHERE i.Status = 3 -- Resolvido
      AND d.Status != 3
      AND c.Data_Resolucao IS NULL;
END;
GO

PRINT '✓ Trigger tr_ChamadoResolvido criado';
GO

-- ============================================
-- INFORMAÇÕES DO BANCO
-- ============================================

PRINT '';
PRINT '================================================';
PRINT '  BANCO DE DADOS CRIADO COM SUCESSO!';
PRINT '================================================';
PRINT '';
PRINT 'Informações do Sistema:';
PRINT '  - Banco: SistemaChamados';
PRINT '  - Tabelas: 6';
PRINT '  - Views: 3';
PRINT '  - Stored Procedures: 3';
PRINT '  - Triggers: 1';
PRINT '';
PRINT 'Usuários Padrão:';
PRINT '  Administrador:';
PRINT '    E-mail: christopher.camp@interfix.com';
PRINT '    Senha: MinhaSenha (Hash já aplicado)';
PRINT '';
PRINT '  Técnicos:';
PRINT '    E-mail: juan.silva@interfix.com';
PRINT '    E-mail: pedro.costa@interfix.com';
PRINT '    Senha: Senha123 (Hash já aplicado)';
PRINT '';
PRINT '  Funcionários:';
PRINT '    E-mail: theo.santos@interfix.com';
PRINT '    E-mail: nycolas.costa@interfix.com';
PRINT '    E-mail: andre.silva@interfix.com';
PRINT '    Senha: Senha123 (Hash já aplicado)';
PRINT '';
PRINT '================================================';
PRINT '';

-- Consulta de verificação
SELECT 'Total de Usuários' AS Informacao, COUNT(*) AS Total FROM Usuario
UNION ALL
SELECT 'Total de Chamados', COUNT(*) FROM chamados
UNION ALL
SELECT 'Total de E-mails', COUNT(*) FROM E_mail;
GO

-- ============================================
-- FIM DO SCRIPT
-- ============================================

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- ============================================
-- SCRIPT PARA RESETEAR IDs SIN PERDER DATOS
-- Sistema de Chamados InterFix
-- ============================================
-- IMPORTANTE: Hace backup automático antes de modificar
-- ============================================

USE Suporte_Tecnico;  
GO

PRINT '================================================';
PRINT '  INICIANDO RESETEO DE IDs';
PRINT '================================================';
PRINT '';

-- ============================================
-- PASO 1: HACER BACKUP DE SEGURIDAD
-- ============================================
PRINT 'Paso 1: Creando backup de seguridad...';

DECLARE @BackupPath NVARCHAR(500);
DECLARE @BackupFile NVARCHAR(500);
DECLARE @Timestamp VARCHAR(50);

SET @Timestamp = CONVERT(VARCHAR(50), GETDATE(), 112) + '_' + REPLACE(CONVERT(VARCHAR(50), GETDATE(), 108), ':', '');
SET @BackupPath = 'C:\Backups\';
SET @BackupFile = @BackupPath + 'Suporte_Tecnico_PreReset_' + @Timestamp + '.bak';

-- Crear carpeta si no existe (requiere xp_cmdshell habilitado)
-- Si no funciona, crear manualmente C:\Backups\

BACKUP DATABASE Suporte_Tecnico 
TO DISK = @BackupFile
WITH FORMAT, NAME = 'Backup antes de resetear IDs';

PRINT '✓ Backup creado: ' + @BackupFile;
PRINT '';

-- ============================================
-- PASO 2: CREAR TABLAS TEMPORALES
-- ============================================
PRINT 'Paso 2: Creando tablas temporales...';

-- Tabla temporal para Usuario
SELECT * 
INTO #TempUsuario
FROM Usuario;

-- Tabla temporal para E_mail
SELECT * 
INTO #TempEmail
FROM E_mail;

-- Tabla temporal para chamados
SELECT * 
INTO #TempChamados
FROM chamados;

-- Tabla temporal para registra
SELECT * 
INTO #TempRegistra
FROM registra;

-- Tabla temporal para Contestacoes
SELECT * 
INTO #TempContestacoes
FROM Contestacoes;

PRINT '✓ Tablas temporales creadas';
PRINT '';

-- ============================================
-- PASO 3: CREAR MAPEO DE IDs ANTIGUOS A NUEVOS
-- ============================================
PRINT 'Paso 3: Creando mapeo de IDs...';

-- Mapeo de Usuarios
CREATE TABLE #MapeoUsuarios (
    IdAntiguo INT,
    IdNuevo INT
);

-- Mapeo de Chamados
CREATE TABLE #MapeoChamados (
    IdAntiguo INT,
    IdNuevo INT
);

-- Generar mapeo de usuarios (orden por fecha de creación)
INSERT INTO #MapeoUsuarios (IdAntiguo, IdNuevo)
SELECT 
    Id_usuario AS IdAntiguo,
    ROW_NUMBER() OVER (ORDER BY DataCadastro, Id_usuario) AS IdNuevo
FROM #TempUsuario;

-- Generar mapeo de chamados (orden por fecha de registro)
INSERT INTO #MapeoChamados (IdAntiguo, IdNuevo)
SELECT 
    id_chamado AS IdAntiguo,
    ROW_NUMBER() OVER (ORDER BY Data_Registro, id_chamado) AS IdNuevo
FROM #TempChamados;

PRINT '✓ Mapeo de IDs creado';
PRINT '';

-- ============================================
-- PASO 4: DESHABILITAR CONSTRAINTS
-- ============================================
PRINT 'Paso 4: Deshabilitando constraints...';

-- Deshabilitar todas las foreign keys
ALTER TABLE E_mail NOCHECK CONSTRAINT ALL;
ALTER TABLE chamados NOCHECK CONSTRAINT ALL;
ALTER TABLE registra NOCHECK CONSTRAINT ALL;
ALTER TABLE Usuario NOCHECK CONSTRAINT ALL;
ALTER TABLE Nivel_de_acesso NOCHECK CONSTRAINT ALL;
ALTER TABLE Contestacoes NOCHECK CONSTRAINT ALL;


PRINT '✓ Constraints deshabilitados';
PRINT '';

-- ============================================
-- PASO 5: ELIMINAR Y RECREAR TABLA USUARIO
-- ============================================
PRINT 'Paso 5: Recreando tabla Usuario...';

-- Eliminar tabla Usuario
GO
DROP TABLE Usuario;


-- Recrear tabla Usuario con IDENTITY reiniciado
CREATE TABLE Usuario (
    Id_usuario INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    senha VARCHAR(100) NOT NULL,
    Cpf CHAR(11) NOT NULL UNIQUE,
    Acess_codigo DECIMAL(18, 0) NOT NULL,
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE(),
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Usuario_NivelAcesso FOREIGN KEY (Acess_codigo) REFERENCES Nivel_de_acesso(codigo),
    CONSTRAINT CK_Cpf_Valido CHECK (LEN(Cpf) = 11),
    CONSTRAINT CK_Senha_Nao_Vazia CHECK (LEN(senha) > 0)
);

-- Insertar usuarios en orden correcto
SET IDENTITY_INSERT Usuario ON;

INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo)
SELECT 
    m.IdNuevo,
    t.nome,
    t.senha,
    t.Cpf,
    t.Acess_codigo,
    t.DataCadastro,
    t.Ativo
FROM #TempUsuario t
INNER JOIN #MapeoUsuarios m ON t.Id_usuario = m.IdAntiguo
ORDER BY m.IdNuevo;

SET IDENTITY_INSERT Usuario OFF;

PRINT '✓ Tabla Usuario recreada con IDs secuenciales';
PRINT '';

-- ============================================
-- PASO 6: ACTUALIZAR TABLA E_MAIL
-- ============================================
PRINT 'Paso 6: Actualizando tabla E_mail...';

TRUNCATE TABLE E_mail;

SET IDENTITY_INSERT E_mail ON;

INSERT INTO E_mail (ID, E_mail, Id_usuario)
SELECT 
    ROW_NUMBER() OVER (ORDER BY t.ID) AS ID,
    t.E_mail,
    m.IdNuevo AS Id_usuario
FROM #TempEmail t
INNER JOIN #MapeoUsuarios m ON t.Id_usuario = m.IdAntiguo;

SET IDENTITY_INSERT E_mail OFF;

PRINT '✓ Tabla E_mail actualizada';
PRINT '';

-- ============================================
-- PASO 7: RECREAR TABLA CHAMADOS
-- ============================================
PRINT 'Paso 7: Recreando tabla chamados...';

DROP TABLE chamados;

CREATE TABLE chamados (
    id_chamado INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    categoria VARCHAR(20) NOT NULL,
    prioridade INT NOT NULL,
    descricao VARCHAR(1000) NOT NULL,
    Afetado INT NOT NULL,
    Data_Registro DATETIME NOT NULL DEFAULT GETDATE(),
    Status INT NOT NULL DEFAULT 1,
    Solucao VARCHAR(1000) NULL,
    Contestacoes_Codigo DECIMAL(18, 0) NULL,
    Tecnico_Atribuido INT NULL,
    Data_Resolucao DATETIME NULL,
    CONSTRAINT FK_Chamado_Afetado FOREIGN KEY (Afetado) REFERENCES Usuario(Id_usuario),
    CONSTRAINT FK_Chamado_Tecnico FOREIGN KEY (Tecnico_Atribuido) REFERENCES Usuario(Id_usuario),
    CONSTRAINT CK_Prioridade_Valida CHECK (prioridade BETWEEN 1 AND 4),
    CONSTRAINT CK_Status_Valido CHECK (Status BETWEEN 1 AND 5),
    CONSTRAINT CK_Categoria_Valida CHECK (categoria IN ('Hardware', 'Software', 'Rede', 'Outros'))
);

SET IDENTITY_INSERT chamados ON;

INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Solucao, Contestacoes_Codigo, Tecnico_Atribuido, Data_Resolucao)
SELECT 
    mc.IdNuevo,
    t.categoria,
    t.prioridade,
    t.descricao,
    muAfetado.IdNuevo AS Afetado,
    t.Data_Registro,
    t.Status,
    t.Solucao,
    t.Contestacoes_Codigo,
    CASE 
        WHEN t.Tecnico_Atribuido IS NOT NULL THEN muTecnico.IdNuevo 
        ELSE NULL 
    END AS Tecnico_Atribuido,
    t.Data_Resolucao
FROM #TempChamados t
INNER JOIN #MapeoChamados mc ON t.id_chamado = mc.IdAntiguo
INNER JOIN #MapeoUsuarios muAfetado ON t.Afetado = muAfetado.IdAntiguo
LEFT JOIN #MapeoUsuarios muTecnico ON t.Tecnico_Atribuido = muTecnico.IdAntiguo
ORDER BY mc.IdNuevo;

SET IDENTITY_INSERT chamados OFF;

PRINT '✓ Tabla chamados recreada con IDs secuenciales';
PRINT '';

-- ============================================
-- PASO 8: ACTUALIZAR TABLA REGISTRA
-- ============================================
PRINT 'Paso 8: Actualizando tabla registra...';

TRUNCATE TABLE registra;

SET IDENTITY_INSERT registra ON;

INSERT INTO registra (Id, Id_usuario, id_chamado, DataRegistro)
SELECT 
    ROW_NUMBER() OVER (ORDER BY t.Id) AS Id,
    mu.IdNuevo AS Id_usuario,
    mc.IdNuevo AS id_chamado,
    t.DataRegistro
FROM #TempRegistra t
INNER JOIN #MapeoUsuarios mu ON t.Id_usuario = mu.IdAntiguo
INNER JOIN #MapeoChamados mc ON t.id_chamado = mc.IdAntiguo;

SET IDENTITY_INSERT registra OFF;

PRINT '✓ Tabla registra actualizada';
PRINT '';

-- ============================================
-- PASO 9: HABILITAR CONSTRAINTS
-- ============================================
PRINT 'Paso 9: Habilitando constraints...';

ALTER TABLE E_mail CHECK CONSTRAINT ALL;
ALTER TABLE chamados CHECK CONSTRAINT ALL;
ALTER TABLE registra CHECK CONSTRAINT ALL;

PRINT '✓ Constraints habilitados';
PRINT '';

-- ============================================
-- PASO 10: LIMPIAR TABLAS TEMPORALES
-- ============================================
PRINT 'Paso 10: Limpiando tablas temporales...';

DROP TABLE #TempUsuario;
DROP TABLE #TempEmail;
DROP TABLE #TempChamados;
DROP TABLE #TempRegistra;
DROP TABLE #TempContestacoes;
DROP TABLE #MapeoUsuarios;
DROP TABLE #MapeoChamados;

PRINT '✓ Tablas temporales eliminadas';
PRINT '';

-- ============================================
-- PASO 11: REINDEXAR Y ACTUALIZAR ESTADÍSTICAS
-- ============================================
PRINT 'Paso 11: Reindexando tablas...';

ALTER INDEX ALL ON Usuario REBUILD;
ALTER INDEX ALL ON E_mail REBUILD;
ALTER INDEX ALL ON chamados REBUILD;
ALTER INDEX ALL ON registra REBUILD;

UPDATE STATISTICS Usuario;
UPDATE STATISTICS E_mail;
UPDATE STATISTICS chamados;
UPDATE STATISTICS registra;

PRINT '✓ Índices reconstruidos';
PRINT '';

-- ============================================
-- VERIFICACIÓN FINAL
-- ============================================
PRINT '================================================';
PRINT '  VERIFICACIÓN DE RESULTADOS';
PRINT '================================================';
PRINT '';

SELECT 
    'Usuario' AS Tabla,
    MIN(Id_usuario) AS MinID,
    MAX(Id_usuario) AS MaxID,
    COUNT(*) AS TotalRegistros
FROM Usuario
UNION ALL
SELECT 
    'chamados',
    MIN(id_chamado),
    MAX(id_chamado),
    COUNT(*)
FROM chamados
UNION ALL
SELECT 
    'E_mail',
    MIN(ID),
    MAX(ID),
    COUNT(*)
FROM E_mail
UNION ALL
SELECT 
    'registra',
    MIN(Id),
    MAX(Id),
    COUNT(*)
FROM registra;

PRINT '';
PRINT '================================================';
PRINT '  ✓ RESETEO COMPLETADO CON ÉXITO!';
PRINT '================================================';
PRINT '';
PRINT 'Todos los IDs ahora son secuenciales comenzando desde 1';
PRINT 'Los datos se mantuvieron intactos';
PRINT 'Las relaciones fueron actualizadas correctamente';
PRINT '';
PRINT 'Backup guardado en: ' + @BackupFile;
PRINT '';
PRINT '================================================';
GO

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- ============================================
-- 🔧 FIX DEFINITIVO - INSERTAR DATOS
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '========================================';
PRINT 'LIMPIANDO Y RECREANDO DATOS';
PRINT '========================================';
PRINT '';

-- ============================================
-- 1. LIMPIAR TODO
-- ============================================
PRINT '1. Limpiando datos existentes...';

-- Eliminar en orden por FK
DELETE FROM registra;
DELETE FROM chamados;
DELETE FROM Contestacoes;
DELETE FROM E_mail;
DELETE FROM Usuario;
DELETE FROM Nivel_de_acesso;

PRINT '   ✅ Datos eliminados';
PRINT '';

-- ============================================
-- 2. INSERTAR NIVELES DE ACCESO
-- ============================================
PRINT '2. Insertando niveles de acceso...';

INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (1, 'Funcionario');
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (2, 'Tecnico');
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (3, 'Administrador');

PRINT '   ✅ Niveles insertados:';
PRINT '      1 = Funcionario';
PRINT '      2 = Tecnico';
PRINT '      3 = Administrador';
PRINT '';

-- ============================================
-- 3. INSERTAR USUARIOS
-- ============================================
PRINT '3. Insertando usuarios...';

SET IDENTITY_INSERT Usuario ON;

-- Usuario 1: Admin
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (1, 'Christopher Camp', 'MinhaSenha', '12345678900', 3, GETDATE(), 1);

-- Usuario 2: Técnico
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (2, 'Juan Silva', 'senhaJuan', '21987654321', 2, GETDATE(), 1);

-- Usuario 3: Funcionario
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (3, 'Theo Santos', 'senhaTheo', '10192838374', 1, GETDATE(), 1);

-- Usuario 4: Funcionario
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (4, 'Nycolas Costa', 'senhaNycolas', '65473923981', 1, GETDATE(), 1);

SET IDENTITY_INSERT Usuario OFF;

PRINT '   ✅ 4 usuarios insertados';
PRINT '';

-- ============================================
-- 4. INSERTAR EMAILS
-- ============================================
PRINT '4. Insertando emails...';

INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('chriscamplopes@gmail.com', 1);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('Juan@gmail.com', 2);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('theo@gmail.com', 3);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('nycolas@gmail.com', 4);

PRINT '   ✅ 4 emails insertados';
PRINT '';

-- ============================================
-- 5. INSERTAR CONTESTACIONES Y CHAMADOS
-- ============================================
PRINT '5. Insertando chamados...';

-- Contestación
SET IDENTITY_INSERT Contestacoes ON;
INSERT INTO Contestacoes (Codigo, Justificativa, DataContestacao) 
VALUES (1, 'Problema crítico - Sistema não funciona', GETDATE());
SET IDENTITY_INSERT Contestacoes OFF;

-- Chamados
SET IDENTITY_INSERT chamados ON;

INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Tecnico_Atribuido, Contestacoes_Codigo) 
VALUES (1, 'Software', 3, 'Sistema não inicia', 3, GETDATE()-2, 1, 2, NULL);

INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Tecnico_Atribuido, Contestacoes_Codigo) 
VALUES (2, 'Hardware', 2, 'Mouse não funciona', 4, GETDATE()-1, 1, NULL, NULL);

INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Tecnico_Atribuido, Contestacoes_Codigo) 
VALUES (3, 'Rede', 4, 'Sem internet', 3, GETDATE(), 1, 2, 1);

SET IDENTITY_INSERT chamados OFF;

PRINT '   ✅ 3 chamados insertados';
PRINT '';

-- ============================================
-- 6. VERIFICACIÓN COMPLETA
-- ============================================
PRINT '========================================';
PRINT 'VERIFICACIÓN DE DATOS INSERTADOS';
PRINT '========================================';
PRINT '';

PRINT 'USUARIOS:';
SELECT 
    u.Id_usuario AS ID,
    u.nome AS Nombre,
    u.Cpf,
    u.senha AS Senha,
    u.Acess_codigo AS Nivel,
    n.Nivel_acesso AS Tipo,
    u.Ativo
FROM Usuario u
INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
ORDER BY u.Id_usuario;

PRINT '';
PRINT 'EMAILS:';
SELECT 
    Id,
    E_mail AS Email,
    Id_usuario
FROM E_mail
ORDER BY Id_usuario;

PRINT '';
PRINT 'TEST DE LOGIN:';

DECLARE @Email VARCHAR(100) = 'chriscamplopes@gmail.com';
DECLARE @Senha VARCHAR(100) = 'MinhaSenha';

-- Test en tablas directas
DECLARE @CountTablas INT;
SELECT @CountTablas = COUNT(*)
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail = @Email
AND u.senha = @Senha
AND u.Ativo = 1;

PRINT 'Test en tablas: ' + CAST(@CountTablas AS VARCHAR);

IF @CountTablas > 0
BEGIN
    PRINT '✅ LOGIN FUNCIONARÁ EN TABLAS';
    
    SELECT 
        u.Id_usuario,
        u.nome,
        e.E_mail,
        u.senha,
        u.Acess_codigo,
        u.Ativo
    FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE e.E_mail = @Email
    AND u.senha = @Senha
    AND u.Ativo = 1;
END
ELSE
BEGIN
    PRINT '❌ LOGIN NO FUNCIONARÁ';
END

-- Test en VIEW
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
BEGIN
    PRINT '';
    PRINT 'Test en VIEW:';
    
    DECLARE @CountView INT;
    SELECT @CountView = COUNT(*)
    FROM vw_LoginUsuarios
    WHERE Email = @Email
    AND Senha = @Senha
    AND Ativo = 1;
    
    PRINT 'Test en VIEW: ' + CAST(@CountView AS VARCHAR);
    
    IF @CountView > 0
    BEGIN
        PRINT '✅ LOGIN FUNCIONARÁ EN VIEW';
        
        SELECT 
            Id,
            Nome,
            Email,
            Senha,
            NivelAcesso,
            TipoFuncionario,
            Ativo
        FROM vw_LoginUsuarios
        WHERE Email = @Email
        AND Senha = @Senha
        AND Ativo = 1;
    END
    ELSE
    BEGIN
        PRINT '❌ LOGIN NO FUNCIONARÁ EN VIEW';
    END
END

PRINT '';
PRINT '========================================';
PRINT 'PROCESO COMPLETADO';
PRINT '========================================';
PRINT '';
PRINT 'CREDENCIALES DE LOGIN:';
PRINT '';
PRINT '1. ADMINISTRADOR:';
PRINT '   Email: chriscamplopes@gmail.com';
PRINT '   Senha: MinhaSenha';
PRINT '';
PRINT '2. TÉCNICO:';
PRINT '   Email: Juan@gmail.com';
PRINT '   Senha: senhaJuan';
PRINT '';
PRINT '3. FUNCIONARIO (Theo):';
PRINT '   Email: theo@gmail.com';
PRINT '   Senha: senhaTheo';
PRINT '';
PRINT '4. FUNCIONARIO (Nycolas):';
PRINT '   Email: nycolas@gmail.com';
PRINT '   Senha: senhaNycolas';
PRINT '';
PRINT '========================================';

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- ============================================
-- FIX: Verificar e corrigir nome da coluna Data_Resolucao
-- Execute este script no SQL Server Management Studio
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '🔍 DIAGNÓSTICO DA TABELA CHAMADOS';
PRINT '============================================';
PRINT '';

-- 1. Verificar nome atual da coluna
PRINT '📋 Estrutura atual da tabela chamados:';
PRINT '';

SELECT 
    COLUMN_NAME AS [Nome da Coluna],
    DATA_TYPE AS [Tipo],
    IS_NULLABLE AS [Aceita NULL],
    COLUMN_DEFAULT AS [Valor Padrão]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'chamados'
ORDER BY ORDINAL_POSITION;
GO

-- 2. Verificar se a coluna Data_Resolucao existe
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'chamados' 
    AND COLUMN_NAME = 'Data_Resolucao'
)
BEGIN
    PRINT '';
    PRINT '✅ Coluna Data_Resolucao JÁ EXISTE!';
    PRINT 'Não é necessário fazer nada.';
END
ELSE
BEGIN
    PRINT '';
    PRINT '❌ Coluna Data_Resolucao NÃO EXISTE!';
    PRINT '';
    
    -- Verificar se existe com outro nome
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'chamados' 
        AND COLUMN_NAME = 'DataResolucao'
    )
    BEGIN
        PRINT '✅ Encontrada coluna "DataResolucao" (sem underscore)';
        PRINT '🔧 Renomeando para "Data_Resolucao"...';
        
        EXEC sp_rename 'chamados.DataResolucao', 'Data_Resolucao', 'COLUMN';
        
        PRINT '✅ Coluna renomeada com sucesso!';
    END
    ELSE
    BEGIN
        PRINT '⚠️  Coluna de data de resolução não encontrada!';
        PRINT '🔧 Criando coluna Data_Resolucao...';
        
        ALTER TABLE chamados 
        ADD Data_Resolucao DATETIME NULL;
        
        PRINT '✅ Coluna criada com sucesso!';
    END
END
GO

-- 3. Verificar estrutura final
PRINT '';
PRINT '📋 Estrutura final da tabela chamados:';
PRINT '';

SELECT 
    COLUMN_NAME AS [Nome da Coluna],
    DATA_TYPE AS [Tipo],
    CHARACTER_MAXIMUM_LENGTH AS [Tamanho],
    IS_NULLABLE AS [Aceita NULL]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'chamados'
ORDER BY ORDINAL_POSITION;
GO

-- 4. Teste rápido
PRINT '';
PRINT '🔍 Testando query de chamados:';

SELECT TOP 5
    id_chamado,
    categoria,
    prioridade,
    descricao,
    Afetado,
    Data_Registro,
    Status,
    Tecnico_Atribuido,
    Data_Resolucao
FROM chamados;
GO

PRINT '';
PRINT '✅ FIX COMPLETO!';

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- =====================================================
-- SCRIPT DE MIGRAÇÃO PERSONALIZADO
-- Sistema de Chamados - Estrutura Real
-- =====================================================
-- ATENÇÃO: Execute este script com MUITO cuidado!
-- Faça BACKUP da base de dados antes de executar!
-- =====================================================

USE Suporte_Tecnico;
GO

PRINT '============================================================';
PRINT 'INICIANDO MIGRAÇÃO DE SENHAS PARA HASH SHA256';
PRINT '============================================================';
PRINT '';

-- =====================================================
-- PASSO 1: VERIFICAR ESTRUTURA ATUAL
-- =====================================================
PRINT 'Verificando estrutura atual...';
PRINT '';

SELECT 
    'Estrutura da tabela Usuario:' AS Info;
    
SELECT 
    COLUMN_NAME AS Coluna,
    DATA_TYPE AS Tipo,
    CHARACTER_MAXIMUM_LENGTH AS Tamanho,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuario';

PRINT '';
GO

-- =====================================================
-- PASSO 2: CRIAR BACKUP DA TABELA USUARIO
-- =====================================================
PRINT 'Criando backup da tabela Usuario...';

-- Verificar se já existe backup
IF OBJECT_ID('Usuario_Backup_PreHash', 'U') IS NOT NULL
BEGIN
    PRINT 'Backup anterior encontrado. Excluindo...';
    DROP TABLE Usuario_Backup_PreHash;
END

-- Criar backup
SELECT * 
INTO Usuario_Backup_PreHash 
FROM Usuario;

DECLARE @TotalBackup INT = (SELECT COUNT(*) FROM Usuario_Backup_PreHash);
PRINT CONCAT('✅ Backup criado com sucesso! Total de registros: ', @TotalBackup);
PRINT '';
GO

-- =====================================================
-- PASSO 3: VERIFICAR DADOS ANTES DA MIGRAÇÃO
-- =====================================================
PRINT 'Dados ANTES da migração:';
PRINT '';

SELECT 
    u.Id_usuario,
    u.nome,
    e.E_mail,
    LEFT(u.senha, 15) + '...' AS SenhaAtual,
    LEN(u.senha) AS TamanhoAtual,
    n.Nivel_acesso,
    u.Ativo
FROM Usuario u
LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
ORDER BY u.Id_usuario;

PRINT '';
GO

-- =====================================================
-- PASSO 4: ALTERAR ESTRUTURA (SE NECESSÁRIO)
-- =====================================================
PRINT 'Verificando tamanho do campo senha...';

DECLARE @TamanhoAtual INT = (
    SELECT CHARACTER_MAXIMUM_LENGTH 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Usuario' AND COLUMN_NAME = 'senha'
);

PRINT CONCAT('Tamanho atual do campo senha: ', @TamanhoAtual);

IF @TamanhoAtual < 64
BEGIN
    PRINT 'Alterando tamanho do campo senha para 100 caracteres...';
    
    ALTER TABLE Usuario
    ALTER COLUMN senha VARCHAR(100) NOT NULL;
    
    PRINT '✅ Campo senha alterado para VARCHAR(100)';
END
ELSE
BEGIN
    PRINT '✅ Campo senha já possui tamanho adequado';
END

PRINT '';
GO

-- =====================================================
-- PASSO 5: ATUALIZAR SENHAS PARA HASH
-- =====================================================
PRINT '============================================================';
PRINT 'ATUALIZANDO SENHAS PARA HASH SHA256';
PRINT '============================================================';
PRINT '';
PRINT 'Usuários identificados no sistema:';
PRINT '';

-- Mostrar usuários atuais
SELECT 
    u.Id_usuario,
    u.nome,
    e.E_mail,
    u.senha AS SenhaAtual,
    n.Nivel_acesso
FROM Usuario u
LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
ORDER BY u.Id_usuario;

PRINT '';
PRINT '⚠️ IMPORTANTE: Verifique os emails acima e execute os UPDATE abaixo!';
PRINT '';

-- =====================================================
-- UPDATES COM OS HASHES FORNECIDOS
-- =====================================================

-- 1. ADMINISTRADOR (Chris)
UPDATE u
SET u.senha = 'ecf91daa4f7f26e51ab52fe4946c8afd5c81287c75dd118e67924ee2df11713d'
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail = 'chriscamplopes@gmail.com';

IF @@ROWCOUNT > 0
    PRINT '✅ Senha do ADMINISTRADOR (Chris) atualizada';
ELSE
    PRINT '❌ ERRO: Administrador não encontrado!';

-- 2. TÉCNICO (Juan)
UPDATE u
SET u.senha = '96b3984481c494d898901c2a46c55a210a4c79e766edad69cc5cd54284b710d6'
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail = 'Juan@gmail.com';

IF @@ROWCOUNT > 0
    PRINT '✅ Senha do TÉCNICO (Juan) atualizada';
ELSE
    PRINT '❌ ERRO: Técnico Juan não encontrado!';

-- 3. FUNCIONÁRIO (Theo)
UPDATE u
SET u.senha = '1bc6757cf870d000fd8ba617a53655ae3a448fa2d199cfb29529c586d1cea8c5'
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail = 'theo@gmail.com';

IF @@ROWCOUNT > 0
    PRINT '✅ Senha do FUNCIONÁRIO (Theo) atualizada';
ELSE
    PRINT '❌ ERRO: Funcionário Theo não encontrado!';

-- 4. TÉCNICO (Nycolas)
UPDATE u
SET u.senha = 'd0c8c4f2cbc5bc15cdf852ce356735ddaaceabc1f3a25f7de270192df2c67eb5'
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail = 'nycolas@gmail.com';

IF @@ROWCOUNT > 0
    PRINT '✅ Senha do TÉCNICO (Nycolas) atualizada';
ELSE
    PRINT '❌ ERRO: Técnico Nycolas não encontrado!';

PRINT '';
GO

-- =====================================================
-- PASSO 6: VERIFICAR RESULTADO DA MIGRAÇÃO
-- =====================================================
PRINT '============================================================';
PRINT 'VERIFICANDO RESULTADO DA MIGRAÇÃO';
PRINT '============================================================';
PRINT '';

SELECT 
    u.Id_usuario,
    u.nome,
    e.E_mail,
    n.Nivel_acesso,
    CASE 
        WHEN LEN(u.senha) = 64 THEN '✅ Hash OK (64 caracteres)'
        WHEN LEN(u.senha) > 64 THEN '⚠️ Hash muito longo (' + CAST(LEN(u.senha) AS VARCHAR) + ' caracteres)'
        ELSE '❌ SENHA NÃO MIGRADA! (' + CAST(LEN(u.senha) AS VARCHAR) + ' caracteres)'
    END AS StatusMigracao,
    LEN(u.senha) AS TamanhoSenha,
    u.Ativo AS Status
FROM Usuario u
LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
LEFT JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
ORDER BY u.Id_usuario;

PRINT '';

-- Estatísticas
DECLARE @TotalUsuarios INT = (SELECT COUNT(*) FROM Usuario);
DECLARE @Migradas INT = (SELECT COUNT(*) FROM Usuario WHERE LEN(senha) = 64);
DECLARE @NaoMigradas INT = @TotalUsuarios - @Migradas;

PRINT CONCAT('Total de usuários: ', @TotalUsuarios);
PRINT CONCAT('✅ Senhas migradas corretamente: ', @Migradas);
PRINT CONCAT('❌ Senhas NÃO migradas: ', @NaoMigradas);
PRINT '';

IF @NaoMigradas > 0
BEGIN
    PRINT '⚠️⚠️⚠️ ATENÇÃO! ⚠️⚠️⚠️';
    PRINT 'Ainda existem senhas não migradas!';
    PRINT 'Verifique os emails e IDs acima.';
    PRINT '';
    
    -- Mostrar quais não foram migrados
    SELECT 
        u.Id_usuario,
        u.nome,
        e.E_mail,
        u.senha AS SenhaProblematica,
        LEN(u.senha) AS Tamanho
    FROM Usuario u
    LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE LEN(u.senha) != 64;
    
    PRINT '';
    PRINT 'NÃO prossiga até corrigir todos os registros!';
END
ELSE
BEGIN
    PRINT '✅✅✅ SUCESSO TOTAL! ✅✅✅';
    PRINT 'Todas as senhas foram migradas corretamente!';
    PRINT 'Você pode prosseguir com os testes da aplicação.';
END

PRINT '';
GO

-- =====================================================
-- PASSO 7: COMPARAÇÃO ANTES/DEPOIS
-- =====================================================
PRINT '============================================================';
PRINT 'COMPARAÇÃO: ANTES vs DEPOIS';
PRINT '============================================================';
PRINT '';

SELECT 
    'ANTES (Backup)' AS Momento,
    b.Id_usuario,
    b.nome,
    LEFT(b.senha, 20) + '...' AS Senha,
    LEN(b.senha) AS Tamanho
FROM Usuario_Backup_PreHash b
ORDER BY b.Id_usuario;

PRINT '';

SELECT 
    'DEPOIS (Atual)' AS Momento,
    u.Id_usuario,
    u.nome,
    LEFT(u.senha, 20) + '...' AS Senha,
    LEN(u.senha) AS Tamanho
FROM Usuario u
ORDER BY u.Id_usuario;

PRINT '';
GO

-- =====================================================
-- PASSO 8: TESTE DE VALIDAÇÃO
-- =====================================================
PRINT '============================================================';
PRINT 'TESTE DE VALIDAÇÃO';
PRINT '============================================================';
PRINT '';
PRINT 'Testando se é possível fazer "login" com os hashes:';
PRINT '';

-- Teste Admin
DECLARE @TestHashAdmin VARCHAR(64) = 'ecf91daa4f7f26e51ab52fe4946c8afd5c81287c75dd118e67924ee2df11713d';
IF EXISTS (
    SELECT 1 FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE e.E_mail = 'chriscamplopes@gmail.com' 
    AND u.senha = @TestHashAdmin
)
    PRINT '✅ Admin: Login funcionará';
ELSE
    PRINT '❌ Admin: Login NÃO funcionará!';

-- Teste Juan
DECLARE @TestHashJuan VARCHAR(64) = '96b3984481c494d898901c2a46c55a210a4c79e766edad69cc5cd54284b710d6';
IF EXISTS (
    SELECT 1 FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE e.E_mail = 'Juan@gmail.com' 
    AND u.senha = @TestHashJuan
)
    PRINT '✅ Juan: Login funcionará';
ELSE
    PRINT '❌ Juan: Login NÃO funcionará!';

-- Teste Theo
DECLARE @TestHashTheo VARCHAR(64) = '1bc6757cf870d000fd8ba617a53655ae3a448fa2d199cfb29529c586d1cea8c5';
IF EXISTS (
    SELECT 1 FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE e.E_mail = 'theo@gmail.com' 
    AND u.senha = @TestHashTheo
)
    PRINT '✅ Theo: Login funcionará';
ELSE
    PRINT '❌ Theo: Login NÃO funcionará!';

-- Teste Nycolas
DECLARE @TestHashNycolas VARCHAR(64) = 'd0c8c4f2cbc5bc15cdf852ce356735ddaaceabc1f3a25f7de270192df2c67eb5';
IF EXISTS (
    SELECT 1 FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE e.E_mail = 'nycolas@gmail.com' 
    AND u.senha = @TestHashNycolas
)
    PRINT '✅ Nycolas: Login funcionará';
ELSE
    PRINT '❌ Nycolas: Login NÃO funcionará!';

PRINT '';
GO

-- =====================================================
-- OPCIONAL: REVERTER MIGRAÇÃO (SE ALGO DEU ERRADO)
-- =====================================================
/*
-- ⚠️ DESCOMENTE APENAS SE PRECISAR REVERTER!

PRINT 'Revertendo migração...';

UPDATE u
SET u.senha = b.senha
FROM Usuario u
INNER JOIN Usuario_Backup_PreHash b ON u.Id_usuario = b.Id_usuario;

PRINT '✅ Senhas revertidas para valores originais!';
GO
*/

-- =====================================================
-- OPCIONAL: EXCLUIR BACKUP (DEPOIS DE CONFIRMAR)
-- =====================================================
/*
-- ⚠️ DESCOMENTE APENAS DEPOIS DE TESTAR TUDO!

PRINT 'Excluindo backup...';
DROP TABLE Usuario_Backup_PreHash;
PRINT '✅ Backup excluído!';
GO
*/

-- =====================================================
-- FIM DO SCRIPT
-- =====================================================
PRINT '';
PRINT '============================================================';
PRINT '✅ SCRIPT CONCLUÍDO COM SUCESSO!';
PRINT '============================================================';
PRINT '';
PRINT 'PRÓXIMOS PASSOS:';
PRINT '1. Verifique que todos os usuários têm senha com 64 caracteres';
PRINT '2. Atualize o código C# (se ainda não fez)';
PRINT '3. Teste o login com cada usuário:';
PRINT '   - chriscamplopes@gmail.com / MinhaSenha';
PRINT '   - Juan@gmail.com / senhaJuan';
PRINT '   - theo@gmail.com / senhaTheo';
PRINT '   - nycolas@gmail.com / senhaNycolas';
PRINT '4. Teste criar novo usuário';
PRINT '5. Teste alterar senha';
PRINT '6. Após confirmar que tudo funciona, exclua o backup';
PRINT '';
PRINT '⚠️ IMPORTANTE: As senhas acima são as ORIGINAIS!';
PRINT 'O sistema agora usa os HASHES internamente.';
PRINT '';

