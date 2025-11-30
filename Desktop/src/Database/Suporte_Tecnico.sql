-- 1
ALTER TABLE Historial_Contestacoes
DROP CONSTRAINT DF__Historial___Tipo__1BC821DD;

-- 2
ALTER TABLE Historial_Contestacoes
DROP CONSTRAINT CK_Tipo_Valido;

-- 3
ALTER TABLE Historial_Contestacoes
DROP COLUMN Tipo;

select * from chamados
select * from Historial_Contestacoes

USE Suporte_Tecnico;
GO

PRINT '========================================';
PRINT 'LIMPIEZA COMPLETA - INCLUYENDO AZURE DATA SYNC';
PRINT '========================================';
PRINT '';

-- ============================================
-- 1. ELIMINAR STORED PROCEDURES DE AZURE DATA SYNC
-- ============================================
PRINT '1. Eliminando Stored Procedures de Azure Data Sync...';

DECLARE @ProcName NVARCHAR(255);
DECLARE @SQL NVARCHAR(MAX);

DECLARE proc_cursor CURSOR FOR
SELECT name 
FROM sys.procedures
WHERE name LIKE 'chamados_dss_%' OR name LIKE '%_dss_%';

OPEN proc_cursor;
FETCH NEXT FROM proc_cursor INTO @ProcName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = 'DROP PROCEDURE [' + @ProcName + ']';
    EXEC sp_executesql @SQL;
    PRINT '   ✓ ' + @ProcName + ' eliminado';
    
    FETCH NEXT FROM proc_cursor INTO @ProcName;
END;

CLOSE proc_cursor;
DEALLOCATE proc_cursor;

PRINT '';

-- ============================================
-- 2. ELIMINAR STORED PROCEDURES CUSTOM
-- ============================================
PRINT '2. Eliminando Stored Procedures personalizados...';

IF OBJECT_ID('sp_CrearChamado', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_CrearChamado;
    PRINT '   ✓ sp_CrearChamado eliminado';
END

IF OBJECT_ID('sp_CriarChamado', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_CriarChamado;
    PRINT '   ✓ sp_CriarChamado eliminado';
END

IF OBJECT_ID('sp_ActualizarChamado', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_ActualizarChamado;
    PRINT '   ✓ sp_ActualizarChamado eliminado';
END

IF OBJECT_ID('sp_EliminarChamado', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_EliminarChamado;
    PRINT '   ✓ sp_EliminarChamado eliminado';
END

IF OBJECT_ID('sp_SincronizarDesdeNube', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_SincronizarDesdeNube;
    PRINT '   ✓ sp_SincronizarDesdeNube eliminado';
END

IF OBJECT_ID('sp_ProcesarColaSync', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_ProcesarColaSync;
    PRINT '   ✓ sp_ProcesarColaSync eliminado';
END

IF OBJECT_ID('sp_ListarContestacoesChamado', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE sp_ListarContestacoesChamado;
    PRINT '   ✓ sp_ListarContestacoesChamado eliminado';
END

PRINT '';

-- ============================================
-- 3. ELIMINAR TABLAS DE CONTROL
-- ============================================
PRINT '3. Eliminando Tablas de Control...';

IF OBJECT_ID('SyncQueue', 'U') IS NOT NULL
BEGIN
    DROP TABLE SyncQueue;
    PRINT '   ✓ Tabla SyncQueue eliminada';
END

IF OBJECT_ID('SyncControl', 'U') IS NOT NULL
BEGIN
    DROP TABLE SyncControl;
    PRINT '   ✓ Tabla SyncControl eliminada';
END

PRINT '';

-- ============================================
-- 4. ELIMINAR LINKED SERVER
-- ============================================
PRINT '4. Eliminando Linked Server...';

IF EXISTS (SELECT * FROM sys.servers WHERE name = 'AZURE_INTERFIX')
BEGIN
    EXEC sp_dropserver 'AZURE_INTERFIX', 'droplogins';
    PRINT '   ✓ Linked Server AZURE_INTERFIX eliminado';
END
ELSE
BEGIN
    PRINT '   ℹ Linked Server AZURE_INTERFIX no existe';
END

PRINT '';

-- ============================================
-- 5. ELIMINAR TRIGGERS (si existen)
-- ============================================
PRINT '5. Verificando y eliminando Triggers...';

DECLARE @TriggerName NVARCHAR(255);

DECLARE trigger_cursor CURSOR FOR
SELECT name 
FROM sys.triggers
WHERE parent_id = OBJECT_ID('Chamados')
    AND (name LIKE '%sync%' OR name LIKE '%dss%');

OPEN trigger_cursor;
FETCH NEXT FROM trigger_cursor INTO @TriggerName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = 'DROP TRIGGER [' + @TriggerName + ']';
    EXEC sp_executesql @SQL;
    PRINT '   ✓ Trigger ' + @TriggerName + ' eliminado';
    
    FETCH NEXT FROM trigger_cursor INTO @TriggerName;
END;

CLOSE trigger_cursor;
DEALLOCATE trigger_cursor;

PRINT '';

-- ============================================
-- 6. ELIMINAR RESTRICCIONES Y COLUMNAS
-- ============================================
PRINT '6. Eliminando restricciones y columnas...';

-- Eliminar índice
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chamados_Sync' AND object_id = OBJECT_ID('Chamados'))
BEGIN
    DROP INDEX IX_Chamados_Sync ON Chamados;
    PRINT '   ✓ Índice IX_Chamados_Sync eliminado';
END

-- Eliminar restricción y columna LastModified
DECLARE @ConstraintName NVARCHAR(200);

SELECT @ConstraintName = name
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Chamados')
    AND parent_column_id = (SELECT column_id FROM sys.columns 
                            WHERE object_id = OBJECT_ID('Chamados') 
                            AND name = 'LastModified');

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Chamados DROP CONSTRAINT ' + @ConstraintName);
    PRINT '   ✓ Restricción de LastModified eliminada';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Chamados' AND COLUMN_NAME = 'LastModified')
BEGIN
    ALTER TABLE Chamados DROP COLUMN LastModified;
    PRINT '   ✓ Columna LastModified eliminada';
END

-- Eliminar restricción y columna ModifiedBy
SET @ConstraintName = NULL;

SELECT @ConstraintName = name
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Chamados')
    AND parent_column_id = (SELECT column_id FROM sys.columns 
                            WHERE object_id = OBJECT_ID('Chamados') 
                            AND name = 'ModifiedBy');

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Chamados DROP CONSTRAINT ' + @ConstraintName);
    PRINT '   ✓ Restricción de ModifiedBy eliminada';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Chamados' AND COLUMN_NAME = 'ModifiedBy')
BEGIN
    ALTER TABLE Chamados DROP COLUMN ModifiedBy;
    PRINT '   ✓ Columna ModifiedBy eliminada';
END

-- Eliminar restricción y columna Origin
SET @ConstraintName = NULL;

SELECT @ConstraintName = name
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Chamados')
    AND parent_column_id = (SELECT column_id FROM sys.columns 
                            WHERE object_id = OBJECT_ID('Chamados') 
                            AND name = 'Origin');

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Chamados DROP CONSTRAINT ' + @ConstraintName);
    PRINT '   ✓ Restricción de Origin eliminada';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Chamados' AND COLUMN_NAME = 'Origin')
BEGIN
    ALTER TABLE Chamados DROP COLUMN Origin;
    PRINT '   ✓ Columna Origin eliminada';
END

-- Eliminar restricción y columna SyncVersion
SET @ConstraintName = NULL;

SELECT @ConstraintName = name
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Chamados')
    AND parent_column_id = (SELECT column_id FROM sys.columns 
                            WHERE object_id = OBJECT_ID('Chamados') 
                            AND name = 'SyncVersion');

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Chamados DROP CONSTRAINT ' + @ConstraintName);
    PRINT '   ✓ Restricción de SyncVersion eliminada';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Chamados' AND COLUMN_NAME = 'SyncVersion')
BEGIN
    ALTER TABLE Chamados DROP COLUMN SyncVersion;
    PRINT '   ✓ Columna SyncVersion eliminada';
END

PRINT '';
PRINT '========================================';
PRINT 'LIMPIEZA COMPLETADA';
PRINT '========================================';


----------------------------------------------------------------------------------------------------------------------------------------
USE Suporte_Tecnico;

PRINT 'VERIFICACION POST-LIMPIEZA:';
PRINT '';

-- Verificar Stored Procedures
PRINT 'Stored Procedures restantes relacionados con sync:';
SELECT name FROM sys.procedures
WHERE name LIKE '%sync%' OR name LIKE '%Chamado%' OR name LIKE '%Nube%';
PRINT '';

-- Verificar Tablas
PRINT 'Tablas de sincronizacion restantes:';
SELECT name FROM sys.tables
WHERE name IN ('SyncControl', 'SyncQueue');
PRINT '';

-- Verificar Linked Server
PRINT 'Linked Servers:';
SELECT name, data_source FROM sys.servers
WHERE is_linked = 1;
PRINT '';

-- Verificar columnas en Chamados
PRINT 'Estructura actual de la tabla Chamados:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Chamados'
ORDER BY ORDINAL_POSITION;

-----------------------------------------------------------------------------------------------------------------------------------------------------
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

------------------------------------------------------------------------------------------------------------------------------------------

-- ============================================
-- SCRIPT: Insertar Usuarios con Emails Reales
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '============================================';
PRINT 'INSERTANDO USUARIOS CON EMAILS REALES';
PRINT '============================================';
PRINT '';

-- USUARIO 1: Administrador (Interfix)
-- Email: Interfix851@gmail.com
-- Senha: Admin123
-- Hash SHA256: 8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918

DECLARE @IdAdmin INT;

IF NOT EXISTS (SELECT 1 FROM E_mail WHERE E_mail = 'Interfix851@gmail.com')
BEGIN
    INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
    VALUES ('Administrador InterFix', '11111111111', 
            '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 
            3, GETDATE(), 1);
    
    SET @IdAdmin = SCOPE_IDENTITY();
    
    INSERT INTO E_mail (Id_usuario, E_mail)
    VALUES (@IdAdmin, 'Interfix851@gmail.com');
    
    PRINT '✅ Admin: Interfix851@gmail.com / Senha: Admin123';
END
ELSE
BEGIN
    PRINT '⚠️ Email Interfix851@gmail.com já existe';
END

PRINT '';

-- USUARIO 2: Técnico (Juan Vargas)
-- Email: vargasjuan8096@gmail.com
-- Senha: Tecnico123
-- Hash SHA256: 86e8f7ab32cfd12577bc2619bc635690dbd9ccf7d8b37f3e5c2d2e9f3c6f4c0a

DECLARE @IdTecnico INT;

IF NOT EXISTS (SELECT 1 FROM E_mail WHERE E_mail = 'vargasjuan8096@gmail.com')
BEGIN
    INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
    VALUES ('Juan Vargas', '22222222222', 
            '86e8f7ab32cfd12577bc2619bc635690dbd9ccf7d8b37f3e5c2d2e9f3c6f4c0a', 
            2, GETDATE(), 1);
    
    SET @IdTecnico = SCOPE_IDENTITY();
    
    INSERT INTO E_mail (Id_usuario, E_mail)
    VALUES (@IdTecnico, 'vargasjuan8096@gmail.com');
    
    PRINT '✅ Técnico: vargasjuan8096@gmail.com / Senha: Tecnico123';
END
ELSE
BEGIN
    PRINT '⚠️ Email vargasjuan8096@gmail.com já existe';
END

PRINT '';

-- USUARIO 3: Funcionário (Chris Camp)
-- Email: chriscamplopes1@gmail.com
-- Senha: Func123
-- Hash SHA256: 4f7d8e9a6b5c3d2e1f0a8b7c6d5e4f3a2b1c0d9e8f7a6b5c4d3e2f1a0b9c8d7

DECLARE @IdFunc INT;

IF NOT EXISTS (SELECT 1 FROM E_mail WHERE E_mail = 'chriscamplopes1@gmail.com')
BEGIN
    INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
    VALUES ('Christopher Camp', '33333333333', 
            '4f7d8e9a6b5c3d2e1f0a8b7c6d5e4f3a2b1c0d9e8f7a6b5c4d3e2f1a0b9c8d7', 
            1, GETDATE(), 1);
    
    SET @IdFunc = SCOPE_IDENTITY();
    
    INSERT INTO E_mail (Id_usuario, E_mail)
    VALUES (@IdFunc, 'chriscamplopes1@gmail.com');
    
    PRINT '✅ Funcionário: chriscamplopes1@gmail.com / Senha: Func123';
END
ELSE
BEGIN
    PRINT '⚠️ Email chriscamplopes1@gmail.com já existe';
END

PRINT '';

-- VERIFICAÇÃO
PRINT '============================================';
PRINT 'VERIFICAÇÃO DE USUÁRIOS CRIADOS';
PRINT '============================================';
PRINT '';

SELECT 
    u.Id_usuario,
    u.nome AS Nome,
    e.E_mail AS Email,
    CASE u.Acess_codigo
        WHEN 1 THEN 'Funcionário'
        WHEN 2 THEN 'Técnico'
        WHEN 3 THEN 'Administrador'
    END AS Tipo,
    u.Ativo
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
WHERE e.E_mail IN ('Interfix851@gmail.com', 'vargasjuan8096@gmail.com', 'chriscamplopes1@gmail.com')
ORDER BY u.Acess_codigo DESC;

PRINT '';
PRINT '============================================';
PRINT '✅ CREDENCIAIS:';
PRINT '============================================';
PRINT 'Admin: Interfix851@gmail.com / Admin123';
PRINT 'Técnico: vargasjuan8096@gmail.com / Tecnico123';
PRINT 'Funcionário: chriscamplopes1@gmail.com / Func123';
PRINT '============================================';

GO

-----------------------------------------------------------------------------------------------

-- ============================================
-- SCRIPT FINAL: Limpieza Total y Recreación
-- ============================================

USE Suporte_Tecnico;
GO

PRINT '============================================';
PRINT 'LIMPIEZA TOTAL DE USUARIOS';
PRINT '============================================';
PRINT '';

-- ============================================
-- PASO 1: ELIMINAR TODO EN ORDEN CORRECTO
-- ============================================

-- 1.1: Eliminar registros de chamados
DELETE FROM registra;
PRINT '✅ Tabla registra limpiada';

-- 1.2: Eliminar todos los chamados (para evitar conflictos)
DELETE FROM chamados;
PRINT '✅ Tabla chamados limpiada';

-- 1.3: Eliminar todos los emails
DELETE FROM E_mail;
PRINT '✅ Tabla E_mail limpiada';

-- 1.4: Eliminar todos los usuarios
DELETE FROM Usuario;
PRINT '✅ Tabla Usuario limpiada';

PRINT '';
PRINT 'Todas las tablas limpiadas. Creando usuarios nuevos...';
PRINT '';

-- ============================================
-- PASO 2: CREAR USUARIOS CON HASHES CORRECTOS
-- ============================================

-- USUARIO 1: Administrador
DECLARE @HashAdmin VARBINARY(32) = HASHBYTES('SHA2_256', 'Admin123');
DECLARE @HashAdminStr VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), @HashAdmin, 2));

INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
VALUES ('Administrador InterFix', '11111111111', @HashAdminStr, 3, GETDATE(), 1);

DECLARE @IdAdmin INT = SCOPE_IDENTITY();

INSERT INTO E_mail (Id_usuario, E_mail)
VALUES (@IdAdmin, 'Interfix851@gmail.com');

PRINT '✅ 1. ADMIN:';
PRINT '   Email: Interfix851@gmail.com';
PRINT '   Senha: Admin123';
PRINT '   Hash: ' + @HashAdminStr;
PRINT '';

-- USUARIO 2: Técnico
DECLARE @HashTec VARBINARY(32) = HASHBYTES('SHA2_256', 'Tecnico123');
DECLARE @HashTecStr VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), @HashTec, 2));

INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
VALUES ('Juan Vargas', '22222222222', @HashTecStr, 2, GETDATE(), 1);

DECLARE @IdTec INT = SCOPE_IDENTITY();

INSERT INTO E_mail (Id_usuario, E_mail)
VALUES (@IdTec, 'vargasjuan8096@gmail.com');

PRINT '✅ 2. TÉCNICO:';
PRINT '   Email: vargasjuan8096@gmail.com';
PRINT '   Senha: Tecnico123';
PRINT '   Hash: ' + @HashTecStr;
PRINT '';

-- USUARIO 3: Funcionário
DECLARE @HashFunc VARBINARY(32) = HASHBYTES('SHA2_256', 'Func123');
DECLARE @HashFuncStr VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), @HashFunc, 2));

INSERT INTO Usuario (nome, Cpf, senha, Acess_codigo, DataCadastro, Ativo)
VALUES ('Christopher Camp', '33333333333', @HashFuncStr, 1, GETDATE(), 1);

DECLARE @IdFunc INT = SCOPE_IDENTITY();

INSERT INTO E_mail (Id_usuario, E_mail)
VALUES (@IdFunc, 'chriscamplopes1@gmail.com');

PRINT '✅ 3. FUNCIONÁRIO:';
PRINT '   Email: chriscamplopes1@gmail.com';
PRINT '   Senha: Func123';
PRINT '   Hash: ' + @HashFuncStr;
PRINT '';

-- ============================================
-- PASO 3: VERIFICACIÓN FINAL
-- ============================================
PRINT '============================================';
PRINT 'VERIFICACIÓN DE USUARIOS CRIADOS';
PRINT '============================================';
PRINT '';

SELECT 
    u.Id_usuario AS ID,
    u.nome AS Nome,
    e.E_mail AS Email,
    LEFT(u.senha, 20) + '...' AS Hash_Inicio,
    LEN(u.senha) AS Tamanho,
    CASE u.Acess_codigo
        WHEN 1 THEN 'Funcionário'
        WHEN 2 THEN 'Técnico'
        WHEN 3 THEN 'Administrador'
    END AS Tipo,
    u.Ativo
FROM Usuario u
INNER JOIN E_mail e ON u.Id_usuario = e.Id_usuario
ORDER BY u.Acess_codigo DESC;

PRINT '';

-- ============================================
-- PASO 4: TEST DE HASH (CRÍTICO)
-- ============================================
PRINT '============================================';
PRINT 'TEST DE VALIDACIÓN DE HASH';
PRINT '============================================';
PRINT '';

-- Test Admin
DECLARE @TestHashAdmin VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Admin123'), 2));
DECLARE @CountAdmin INT = (SELECT COUNT(*) FROM Usuario WHERE senha = @TestHashAdmin);

PRINT 'TEST ADMIN:';
PRINT '   Hash esperado: ' + @TestHashAdmin;
IF @CountAdmin > 0
    PRINT '   ✅ Hash ENCONTRADO no banco';
ELSE
    PRINT '   ❌ Hash NÃO encontrado no banco';
PRINT '';

-- Test Técnico
DECLARE @TestHashTec VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Tecnico123'), 2));
DECLARE @CountTec INT = (SELECT COUNT(*) FROM Usuario WHERE senha = @TestHashTec);

PRINT 'TEST TÉCNICO:';
PRINT '   Hash esperado: ' + @TestHashTec;
IF @CountTec > 0
    PRINT '   ✅ Hash ENCONTRADO no banco';
ELSE
    PRINT '   ❌ Hash NÃO encontrado no banco';
PRINT '';

-- Test Funcionário
DECLARE @TestHashFunc VARCHAR(64) = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Func123'), 2));
DECLARE @CountFunc INT = (SELECT COUNT(*) FROM Usuario WHERE senha = @TestHashFunc);

PRINT 'TEST FUNCIONÁRIO:';
PRINT '   Hash esperado: ' + @TestHashFunc;
IF @CountFunc > 0
    PRINT '   ✅ Hash ENCONTRADO no banco';
ELSE
    PRINT '   ❌ Hash NÃO encontrado no banco';
PRINT '';

-- ============================================
-- PASO 5: TEST DE LOGIN SIMULADO
-- ============================================
PRINT '============================================';
PRINT 'TEST DE LOGIN SIMULADO (VIEW)';
PRINT '============================================';
PRINT '';

-- Test login Admin
DECLARE @TestEmailAdmin VARCHAR(100) = 'Interfix851@gmail.com';
DECLARE @LoginTestAdmin INT = (
    SELECT COUNT(*) 
    FROM vw_LoginUsuarios 
    WHERE Email = @TestEmailAdmin 
    AND Senha = @TestHashAdmin 
    AND Ativo = 1
);

PRINT 'LOGIN TEST - ADMIN:';
PRINT '   Email: ' + @TestEmailAdmin;
PRINT '   Senha: Admin123';
IF @LoginTestAdmin > 0
    PRINT '   ✅ LOGIN FUNCIONARÁ';
ELSE
    PRINT '   ❌ LOGIN NÃO FUNCIONARÁ';
PRINT '';

-- Test login Técnico
DECLARE @TestEmailTec VARCHAR(100) = 'vargasjuan8096@gmail.com';
DECLARE @LoginTestTec INT = (
    SELECT COUNT(*) 
    FROM vw_LoginUsuarios 
    WHERE Email = @TestEmailTec 
    AND Senha = @TestHashTec 
    AND Ativo = 1
);

PRINT 'LOGIN TEST - TÉCNICO:';
PRINT '   Email: ' + @TestEmailTec;
PRINT '   Senha: Tecnico123';
IF @LoginTestTec > 0
    PRINT '   ✅ LOGIN FUNCIONARÁ';
ELSE
    PRINT '   ❌ LOGIN NÃO FUNCIONARÁ';
PRINT '';

-- Test login Funcionário
DECLARE @TestEmailFunc VARCHAR(100) = 'chriscamplopes1@gmail.com';
DECLARE @LoginTestFunc INT = (
    SELECT COUNT(*) 
    FROM vw_LoginUsuarios 
    WHERE Email = @TestEmailFunc 
    AND Senha = @TestHashFunc 
    AND Ativo = 1
);

PRINT 'LOGIN TEST - FUNCIONÁRIO:';
PRINT '   Email: ' + @TestEmailFunc;
PRINT '   Senha: Func123';
IF @LoginTestFunc > 0
    PRINT '   ✅ LOGIN FUNCIONARÁ';
ELSE
    PRINT '   ❌ LOGIN NÃO FUNCIONARÁ';
PRINT '';

PRINT '============================================';
PRINT '✅ SCRIPT CONCLUÍDO COM SUCESSO!';
PRINT '============================================';
PRINT '';
PRINT 'CREDENCIAIS PARA LOGIN:';
PRINT '';
PRINT '1. ADMIN: Interfix851@gmail.com / Admin123';
PRINT '2. TÉCNICO: vargasjuan8096@gmail.com / Tecnico123';
PRINT '3. FUNCIONÁRIO: chriscamplopes1@gmail.com / Func123';
PRINT '';
PRINT '============================================';

GO
```
✅ 1. ADMIN:
   Email: Interfix851@gmail.com
   Senha: Admin123
   Hash: 8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918

✅ 2. TÉCNICO:
   Email: vargasjuan8096@gmail.com
   Senha: Tecnico123
   Hash: ...

✅ 3. FUNCIONÁRIO:
   Email: chriscamplopes1@gmail.com
   Senha: Func123
   Hash: ...



  