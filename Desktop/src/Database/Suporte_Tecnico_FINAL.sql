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
WHERE TABLE_NAME = 'Usuario'
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
-- 🔍 DIAGNÓSTICO COMPLETO DE LOGIN
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '========================================';
PRINT 'DIAGNÓSTICO DE LOGIN';
PRINT '========================================';
PRINT '';

-- ============================================
-- 1. VERIFICAR DATOS EN TABLA Usuario
-- ============================================
PRINT '1. DATOS EN TABLA Usuario:';
PRINT '----------------------------------------';
SELECT 
    Id_usuario,
    nome,
    Cpf,
    senha,
    Acess_codigo,
    DataCadastro,
    Ativo,
    CASE Ativo
        WHEN 1 THEN 'ACTIVO'
        WHEN 0 THEN 'INACTIVO'
        ELSE 'NULL'
    END AS EstadoTexto
FROM Usuario
ORDER BY Id_usuario;
PRINT '';

-- ============================================
-- 2. VERIFICAR DATOS EN TABLA E_mail
-- ============================================
PRINT '2. DATOS EN TABLA E_mail:';
PRINT '----------------------------------------';
SELECT 
    Id,
    E_mail,
    Id_usuario
FROM E_mail
ORDER BY Id_usuario;
PRINT '';

-- ============================================
-- 3. VERIFICAR SI LA VIEW EXISTE
-- ============================================
PRINT '3. VERIFICAR VIEW vw_LoginUsuarios:';
PRINT '----------------------------------------';

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
BEGIN
    PRINT '✅ VIEW vw_LoginUsuarios EXISTE';
    PRINT '';
    
    -- Mostrar definición de la VIEW
    PRINT 'Definición de la VIEW:';
    SELECT OBJECT_DEFINITION(OBJECT_ID('vw_LoginUsuarios')) AS ViewDefinition;
    PRINT '';
    
    -- Mostrar TODAS las columnas de la VIEW
    PRINT 'Columnas de la VIEW:';
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'vw_LoginUsuarios'
    ORDER BY ORDINAL_POSITION;
    PRINT '';
    
    -- Mostrar datos de la VIEW
    PRINT 'Datos en la VIEW:';
    SELECT * FROM vw_LoginUsuarios;
    
END
ELSE
BEGIN
    PRINT '❌ VIEW vw_LoginUsuarios NO EXISTE';
    PRINT 'La VIEW necesita ser creada!';
END
PRINT '';

-- ============================================
-- 4. TEST DE LOGIN - QUERY EXACTA DEL CÓDIGO
-- ============================================
PRINT '========================================';
PRINT '4. TEST DE LOGIN (Query del código C#):';
PRINT '========================================';
PRINT '';

DECLARE @Email VARCHAR(100) = 'chriscamplopes@gmail.com';
DECLARE @Senha VARCHAR(100) = 'MinhaSenha';

PRINT 'Intentando login con:';
PRINT '  Email: ' + @Email;
PRINT '  Senha: ' + @Senha;
PRINT '';

-- Test 1: Query EXACTA que usa el código C#
PRINT 'Test 1: Query del código (con VIEW):';
PRINT '--------------------------------------';

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
BEGIN
    DECLARE @CountView INT;
    
    -- Esta es la query EXACTA del código
    SELECT @CountView = COUNT(*) 
    FROM vw_LoginUsuarios 
    WHERE Email = @Email 
    AND Senha = @Senha 
    AND Ativo = 1;
    
    PRINT 'Resultado del COUNT: ' + CAST(@CountView AS VARCHAR);
    
    IF @CountView > 0
    BEGIN
        PRINT '✅ LOGIN DEBERÍA FUNCIONAR';
        PRINT '';
        PRINT 'Usuario encontrado:';
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
        PRINT '❌ LOGIN FALLARÁ';
        PRINT '';
        
        -- Buscar el usuario sin filtros para ver qué pasa
        PRINT 'Buscando el usuario SIN filtros:';
        SELECT 
            Id,
            Nome,
            Email,
            Senha,
            NivelAcesso,
            Ativo,
            CASE 
                WHEN Email = @Email THEN '✅ Email coincide'
                ELSE '❌ Email NO coincide (DB: ' + ISNULL(Email, 'NULL') + ')'
            END AS CheckEmail,
            CASE 
                WHEN Senha = @Senha THEN '✅ Senha coincide'
                ELSE '❌ Senha NO coincide (DB: ' + ISNULL(Senha, 'NULL') + ')'
            END AS CheckSenha,
            CASE 
                WHEN Ativo = 1 THEN '✅ Usuario activo'
                WHEN Ativo = 0 THEN '❌ Usuario INACTIVO'
                ELSE '❌ Ativo es NULL'
            END AS CheckAtivo
        FROM vw_LoginUsuarios
        WHERE Email = @Email;
        
        IF @@ROWCOUNT = 0
            PRINT '❌ NO se encontró el email en la VIEW';
    END
END
ELSE
BEGIN
    PRINT '❌ La VIEW no existe, no se puede probar';
END
PRINT '';

-- ============================================
-- 5. TEST DIRECTO EN TABLAS
-- ============================================
PRINT '========================================';
PRINT '5. TEST DIRECTO EN TABLAS (sin VIEW):';
PRINT '========================================';
PRINT '';

DECLARE @CountTablas INT;
DECLARE @Email VARCHAR (100);
DECLARE @Senha INT;

SELECT @CountTablas = COUNT(*)
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail = @Email
AND u.senha = @Senha
AND u.Ativo = 1;

PRINT 'Resultado en tablas: ' + CAST(@CountTablas AS VARCHAR);

IF @CountTablas > 0
BEGIN
    PRINT '✅ Usuario existe en las tablas';
    PRINT '';
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
    PRINT '❌ Usuario NO encontrado en las tablas';
    PRINT '';
    PRINT 'Buscando solo por email:';
    
    SELECT 
        u.Id_usuario,
        u.nome,
        e.E_mail,
        u.senha,
        u.Acess_codigo,
        u.Ativo,
        CASE 
            WHEN u.senha = @Senha THEN '✅ Senha correcta'
            ELSE '❌ Senha incorrecta (Esperada: ' + @Senha + ', BD: ' + u.senha + ')'
        END AS CheckSenha,
        CASE 
            WHEN u.Ativo = 1 THEN '✅ Activo'
            ELSE '❌ Inactivo'
        END AS CheckEstado
    FROM Usuario u
    INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
    WHERE e.E_mail = @Email;
    
    IF @@ROWCOUNT = 0
        PRINT '❌ Email no existe en la base de datos';
END
PRINT '';

-- ============================================
-- 6. VERIFICAR ESPACIOS EN BLANCO
-- ============================================
PRINT '========================================';
PRINT '6. VERIFICAR ESPACIOS EN DATOS:';
PRINT '========================================';
PRINT '';

SELECT 
	Id_usuario,
    nome,
    '|' + E_mail + '|' AS Email_ConBordes,
    LEN(E_mail) AS Longitud_Email,
    '|' + senha + '|' AS Senha_ConBordes,
    LEN(senha) AS Longitud_Senha,
    Ativo
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail LIKE '%chris%';

PRINT '';

-- ============================================
-- 7. RESUMEN Y DIAGNÓSTICO
-- ============================================
PRINT '========================================';
PRINT 'RESUMEN DEL DIAGNÓSTICO:';
PRINT '========================================';
PRINT '';

-- Contador de problemas
DECLARE @Problemas INT = 0;

-- Check 1: VIEW existe
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
BEGIN
    PRINT '❌ PROBLEMA 1: VIEW vw_LoginUsuarios NO existe';
    SET @Problemas = @Problemas + 1;
END
ELSE
    PRINT '✅ VIEW vw_LoginUsuarios existe';

DECLARE @Problemas INT = 0;

-- Check 2: Usuario existe
IF NOT EXISTS (SELECT * FROM Usuario WHERE Id_usuario = 1)
BEGIN
    PRINT '❌ PROBLEMA 2: Usuario admin NO existe en tabla Usuario';
    SET @Problemas = @Problemas + 1;
END
ELSE
    PRINT '✅ Usuario admin existe en tabla Usuario';

	DECLARE @Email VARCHAR(100);
	DECLARE @Problemas INT = 0;

-- Check 3: Email existe
IF NOT EXISTS (SELECT * FROM E_mail WHERE E_mail = @Email)
BEGIN
    PRINT '❌ PROBLEMA 3: Email NO existe en tabla E_mail';
    SET @Problemas = @Problemas + 1;
END
ELSE
    PRINT '✅ Email existe en tabla E_mail';

	DECLARE @Problemas INT = 0;
	DECLARE @Senha VARCHAR(100);

-- Check 4: Senha correcta
IF NOT EXISTS (SELECT * FROM Usuario WHERE Id_usuario = 1 AND senha = @Senha)
BEGIN
    PRINT '❌ PROBLEMA 4: Senha incorrecta en tabla Usuario';
    SET @Problemas = @Problemas + 1;
END
ELSE
    PRINT '✅ Senha correcta en tabla Usuario';

DECLARE @Problemas INT = 0;

-- Check 5: Usuario activo
IF NOT EXISTS (SELECT * FROM Usuario WHERE Id_usuario = 1 AND Ativo = 1)
BEGIN
    PRINT '❌ PROBLEMA 5: Usuario NO está activo';
    SET @Problemas = @Problemas + 1;
END
ELSE
    PRINT '✅ Usuario está activo';

PRINT '';
PRINT '========================================';
IF @Problemas = 0
    PRINT '✅ TODO CORRECTO - El login DEBERÍA funcionar';
ELSE
    PRINT '❌ SE ENCONTRARON ' + CAST(@Problemas AS VARCHAR) + ' PROBLEMA(S)';
PRINT '========================================';

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- ============================================
-- ✅ SCRIPT SQL FINAL - ESTRUCTURA CORRECTA
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '========================================';
PRINT 'CORRIGIENDO ESTRUCTURA Y DATOS';
PRINT '========================================';
PRINT '';

-- ============================================
-- 1. CORREGIR NIVELES DE ACCESO
-- ============================================
PRINT '1. Corrigiendo tabla Nivel_de_acesso...';

-- Limpiar y reinsertar con niveles correctos
DELETE FROM Nivel_de_acesso;

INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (1, 'Funcionario');
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (2, 'Tecnico');
INSERT INTO Nivel_de_acesso (codigo, Nivel_acesso) VALUES (3, 'Administrador');

PRINT '  ✅ Niveles corregidos:';
PRINT '     1 = Funcionario';
PRINT '     2 = Tecnico';
PRINT '     3 = Administrador';
PRINT '';

-- ============================================
-- 2. LIMPIAR DATOS ANTERIORES
-- ============================================
PRINT '2. Limpiando datos anteriores...';

-- Eliminar en orden por FK
DELETE FROM registra;
DELETE FROM chamados;
DELETE FROM Contestacoes;
DELETE FROM E_mail;
DELETE FROM Usuario;

PRINT '  ✅ Datos antiguos eliminados';
PRINT '';

-- ============================================
-- 3. INSERTAR USUARIOS DE PRUEBA
-- ============================================
PRINT '3. Insertando usuarios de prueba...';

SET IDENTITY_INSERT Usuario ON;

-- Usuario 1: Christian (ADMINISTRADOR - Nivel 3)
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (1, 'Christopher Camp', 'MinhaSenha', '12345678900', 3, GETDATE(), 1);

-- Usuario 2: Juan (TECNICO - Nivel 2)
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (2, 'Juan Silva', 'senhaJuan', '21987654321', 2, GETDATE(), 1);

-- Usuario 3: Theo (FUNCIONARIO - Nivel 1)
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (3, 'Theo Santos', 'senhaTheo', '10192838374', 1, GETDATE(), 1);

-- Usuario 4: Nycolas (FUNCIONARIO - Nivel 1)
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo, DataCadastro, Ativo) 
VALUES (4, 'Nycolas Costa', 'senhaNycolas', '65473923981', 1, GETDATE(), 1);

SET IDENTITY_INSERT Usuario OFF;

PRINT '  ✅ Usuarios creados';

-- ============================================
-- 4. INSERTAR EMAILS
-- ============================================
PRINT '4. Insertando emails...';

INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('chriscamplopes@gmail.com', 1);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('Juan@gmail.com', 2);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('theo@gmail.com', 3);
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('nycolas@gmail.com', 4);

PRINT '  ✅ Emails asociados';
PRINT '';

-- ============================================
-- 5. INSERTAR CONTESTACIONES Y CHAMADOS
-- ============================================
PRINT '5. Insertando chamados de prueba...';

-- Contestación
SET IDENTITY_INSERT Contestacoes ON;
INSERT INTO Contestacoes (Codigo, Justificativa, DataContestacao) VALUES 
(1, 'Problema crítico - Sistema não está funcionando', GETDATE());
SET IDENTITY_INSERT Contestacoes OFF;

-- Chamados
SET IDENTITY_INSERT chamados ON;

-- Chamado 1: Theo solicita, Juan atende
INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Solucao, Contestacoes_Codigo, Tecnico_Atribuido) 
VALUES (1, 'Software', 3, 'Sistema do computador não inicia', 3, GETDATE()-2, 1, NULL, NULL, 2);

-- Chamado 2: Nycolas solicita, sin técnico asignado
INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Solucao, Contestacoes_Codigo, Tecnico_Atribuido) 
VALUES (2, 'Hardware', 2, 'Mouse não funciona corretamente', 4, GETDATE()-1, 1, NULL, NULL, NULL);

-- Chamado 3: Theo solicita con contestación
INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro, Status, Solucao, Contestacoes_Codigo, Tecnico_Atribuido) 
VALUES (3, 'Rede', 4, 'Sem acesso à internet', 3, GETDATE(), 1, NULL, 1, 2);

SET IDENTITY_INSERT chamados OFF;

PRINT '  ✅ Chamados de prueba creados';
PRINT '';

-- ============================================
-- 6. RECREAR VIEW vw_LoginUsuarios
-- ============================================
PRINT '6. Recreando VIEW vw_LoginUsuarios...';

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_LoginUsuarios')
    DROP VIEW vw_LoginUsuarios;
GO

CREATE VIEW vw_LoginUsuarios AS
SELECT 
    u.Id_usuario AS Id,
    u.nome AS Nome,
    u.Cpf,
    e.E_mail AS Email,
    u.senha AS Senha,
    u.Acess_codigo AS NivelAcesso,
    n.Nivel_acesso AS TipoFuncionario,
    u.DataCadastro,
    u.Ativo,
    CAST(NULL AS VARCHAR(100)) AS Especializacao,
    CAST(NULL AS VARCHAR(100)) AS Departamento,
    CAST(NULL AS VARCHAR(100)) AS Cargo
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo;
GO

PRINT '  ✅ VIEW vw_LoginUsuarios creada';
PRINT '';

-- ============================================
-- 7. RECREAR VIEW vw_ChamadosCompletos
-- ============================================
PRINT '7. Recreando VIEW vw_ChamadosCompletos...';

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_ChamadosCompletos')
    DROP VIEW vw_ChamadosCompletos;
GO

CREATE VIEW vw_ChamadosCompletos AS
SELECT 
    c.id_chamado,
    c.categoria,
    c.prioridade,
    c.descricao,
    c.Afetado,
    u1.nome AS Solicitante_Nome,
    c.Data_Registro,
    c.Status,
    c.Tecnico_Atribuido,
    u2.nome AS Tecnico_Nome,
    c.DataResolucao,
    cont.Justificativa,
    cont.DataContestacao AS Data_contestacao
FROM chamados c
INNER JOIN Usuario u1 ON c.Afetado = u1.Id_usuario
LEFT JOIN Usuario u2 ON c.Tecnico_Atribuido = u2.Id_usuario
LEFT JOIN Contestacoes cont ON c.Contestacoes_Codigo = cont.Codigo;
GO

PRINT '  ✅ VIEW vw_ChamadosCompletos creada';
PRINT '';

-- ============================================
-- 8. VERIFICACIÓN
-- ============================================
PRINT '========================================';
PRINT 'VERIFICACIÓN DE DATOS';
PRINT '========================================';
PRINT '';

PRINT 'USUARIOS CREADOS:';
SELECT 
    u.Id_usuario AS ID,
    u.nome AS Nombre,
    e.E_mail AS Email,
    u.senha AS Senha,
    u.Acess_codigo AS Nivel,
    n.Nivel_acesso AS Tipo,
    CASE u.Ativo
        WHEN 1 THEN 'Activo'
        ELSE 'Inactivo'
    END AS Estado
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
INNER JOIN Nivel_de_acesso n ON u.Acess_codigo = n.codigo
ORDER BY u.Acess_codigo DESC, u.Id_usuario;

PRINT '';
PRINT 'CHAMADOS CREADOS:';
SELECT * FROM vw_ChamadosCompletos;

PRINT '';
PRINT '========================================';
PRINT 'TEST DE LOGIN PARA ADMIN';
PRINT '========================================';

DECLARE @TestCount INT;

SELECT @TestCount = COUNT(*) 
FROM vw_LoginUsuarios 
WHERE Email = 'chriscamplopes@gmail.com'
AND Senha = 'MinhaSenha'
AND Ativo = 1;

IF @TestCount > 0
BEGIN
    PRINT '✅ LOGIN FUNCIONARÁ CORRECTAMENTE';
    PRINT '';
    PRINT 'Credenciales:';
    PRINT '  Email: chriscamplopes@gmail.com';
    PRINT '  Senha: MinhaSenha';
    PRINT '  Nivel: 3 (Administrador)';
END
ELSE
BEGIN
    PRINT '❌ LOGIN FALLARÁ';
END

PRINT '';
PRINT '========================================';
PRINT 'TODOS LOS USUARIOS DE PRUEBA:';
PRINT '========================================';
PRINT '';
PRINT '1. ADMIN:';
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
PRINT 'PROCESO COMPLETADO EXITOSAMENTE';
PRINT '========================================';

SELECT * from Usuario;
SELECT * from E_mail;

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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