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
-- 🔍 VERIFICAR ESTRUCTURA REAL DE LAS TABLAS
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '========================================';
PRINT 'ESTRUCTURA DE TABLAS EN LA BASE DE DATOS';
PRINT '========================================';
PRINT '';

-- ============================================
-- 1. TABLA Usuario
-- ============================================
PRINT '1. TABLA Usuario:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME AS [Nombre Columna],
    DATA_TYPE AS [Tipo],
    CHARACTER_MAXIMUM_LENGTH AS [Tamaño],
    IS_NULLABLE AS [Acepta NULL],
    COLUMN_DEFAULT AS [Valor Default]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuario'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- ============================================
-- 2. TABLA E_mail
-- ============================================
PRINT '2. TABLA E_mail:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME AS [Nombre Columna],
    DATA_TYPE AS [Tipo],
    CHARACTER_MAXIMUM_LENGTH AS [Tamaño],
    IS_NULLABLE AS [Acepta NULL],
    COLUMN_DEFAULT AS [Valor Default]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'E_mail'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- ============================================
-- 3. TABLA Tecnico
-- ============================================
PRINT '3. TABLA Tecnico:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME AS [Nombre Columna],
    DATA_TYPE AS [Tipo],
    CHARACTER_MAXIMUM_LENGTH AS [Tamaño],
    IS_NULLABLE AS [Acepta NULL],
    COLUMN_DEFAULT AS [Valor Default]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tecnico'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- ============================================
-- 4. TABLA Funcionario
-- ============================================
PRINT '4. TABLA Funcionario:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME AS [Nombre Columna],
    DATA_TYPE AS [Tipo],
    CHARACTER_MAXIMUM_LENGTH AS [Tamaño],
    IS_NULLABLE AS [Acepta NULL],
    COLUMN_DEFAULT AS [Valor Default]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Funcionario'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- ============================================
-- 5. TABLA chamados
-- ============================================
PRINT '5. TABLA chamados:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME AS [Nombre Columna],
    DATA_TYPE AS [Tipo],
    CHARACTER_MAXIMUM_LENGTH AS [Tamaño],
    IS_NULLABLE AS [Acepta NULL],
    COLUMN_DEFAULT AS [Valor Default]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'chamados'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- ============================================
-- 6. VERIFICAR SI HAY DATOS
-- ============================================
PRINT '========================================';
PRINT 'CONTEO DE REGISTROS:';
PRINT '========================================';

DECLARE @CountUsuario INT, @CountEmail INT, @CountChamados INT;

SELECT @CountUsuario = COUNT(*) FROM Usuario;
SELECT @CountEmail = COUNT(*) FROM E_mail;
SELECT @CountChamados = COUNT(*) FROM chamados;

PRINT 'Usuario: ' + CAST(@CountUsuario AS VARCHAR) + ' registros';
PRINT 'E_mail: ' + CAST(@CountEmail AS VARCHAR) + ' registros';
PRINT 'chamados: ' + CAST(@CountChamados AS VARCHAR) + ' registros';
PRINT '';

-- ============================================
-- 7. MOSTRAR DATOS EXISTENTES (SI HAY)
-- ============================================

IF @CountUsuario > 0
BEGIN
    PRINT '========================================';
    PRINT 'DATOS EXISTENTES EN Usuario:';
    PRINT '========================================';
    SELECT * FROM Usuario;
    PRINT '';
END

IF @CountEmail > 0
BEGIN
    PRINT '========================================';
    PRINT 'DATOS EXISTENTES EN E_mail:';
    PRINT '========================================';
    SELECT * FROM E_mail;
    PRINT '';
END

PRINT '========================================';
PRINT 'FIN DE LA VERIFICACIÓN';
PRINT '========================================';


---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- ============================================
-- FIX PARA VIEW vw_LoginUsuarios
-- Execute este script no SQL Server
-- ============================================

USE Suporte_Tecnico;
GO

-- Dropar VIEW existente
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
    DROP VIEW vw_LoginUsuarios;
GO

-- Recriar VIEW com os campos corretos
CREATE VIEW vw_LoginUsuarios AS
SELECT 
    u.Id_usuario AS Id,
    u.nome AS Nome,
    u.senha AS Senha,
    u.Cpf,
    e.E_mail AS Email,
    u.Acess_codigo AS NivelAcesso,
    n.Nivel_acesso AS TipoFuncionario,
    u.DataCadastro,
    CAST(u.Ativo AS BIT) AS Ativo,
    CAST(NULL AS VARCHAR(100)) AS Especializacao,
    CAST(NULL AS VARCHAR(100)) AS Departamento,
    CAST(NULL AS VARCHAR(100)) AS Cargo
FROM Usuario u
INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
LEFT JOIN E_mail e ON u.Id_usuario = e.Id_usuario;
GO

-- Verificar se foi criada corretamente
PRINT '✅ VIEW vw_LoginUsuarios recriada!';
PRINT '';
PRINT '📋 Testando VIEW:';
SELECT * FROM vw_LoginUsuarios;
GO

-- Testar login específico
PRINT '';
PRINT '🔍 Testando login do admin:';
SELECT 
    Id,
    Nome,
    Email,
    Senha,
    NivelAcesso,
    TipoFuncionario,
    Ativo
FROM vw_LoginUsuarios
WHERE Email = 'chriscamplopes@gmail.com'
AND Senha = 'MinhaSenha'
AND Ativo = 1;
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