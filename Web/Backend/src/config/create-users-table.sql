-- Script para criar a tabela de usuários no SQL Server

-- Cria a tabela de usuários
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'usuarios')
BEGIN
    CREATE TABLE usuarios (
        id INT PRIMARY KEY IDENTITY(1,1),
        nome NVARCHAR(100) NOT NULL,
        email NVARCHAR(100) NOT NULL UNIQUE,
        senha NVARCHAR(255) NOT NULL,
        tipo_usuario NVARCHAR(20) NOT NULL DEFAULT 'tecnico',
        ativo BIT NOT NULL DEFAULT 1,
        data_criacao DATETIME NOT NULL DEFAULT GETDATE(),
        data_atualizacao DATETIME NULL,
        
        CONSTRAINT CK_tipo_usuario CHECK (tipo_usuario IN ('admin', 'tecnico', 'cliente'))
    );

    PRINT 'Tabela usuarios criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela usuarios já existe.';
END
GO

-- Cria índices para melhorar performance
CREATE INDEX IX_usuarios_email ON usuarios(email);
CREATE INDEX IX_usuarios_tipo ON usuarios(tipo_usuario);
CREATE INDEX IX_usuarios_ativo ON usuarios(ativo);
GO

-- Insere usuário administrador padrão
-- Senha: admin123 (hash SHA256)
IF NOT EXISTS (SELECT * FROM usuarios WHERE email = 'admin@interfix.com')
BEGIN
    INSERT INTO usuarios (nome, email, senha, tipo_usuario, ativo)
    VALUES (
        'Administrador',
        'admin@interfix.com',
        '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- admin123
        'admin',
        1
    );

    PRINT 'Usuário administrador criado com sucesso!';
    PRINT 'Email: admin@interfix.com';
    PRINT 'Senha: admin123';
END
GO

-- Insere alguns usuários de teste
IF NOT EXISTS (SELECT * FROM usuarios WHERE email = 'tecnico@interfix.com')
BEGIN
    INSERT INTO usuarios (nome, email, senha, tipo_usuario, ativo)
    VALUES 
    (
        'João Silva',
        'tecnico@interfix.com',
        'e99a18c428cb38d5f260853678922e03abc787edaa8bb6e1ca58eae6e24db0c0', -- tecnico123
        'tecnico',
        1
    ),
    (
        'Maria Santos',
        'maria@interfix.com',
        '65e84be33532fb784c48129675f9eff3a682b27168c0ea744b2cf58ee02337c5', -- maria123
        'tecnico',
        1
    );

    PRINT 'Usuários de teste criados com sucesso!';
END
GO

-- Exibe todos os usuários
SELECT 
    id,
    nome,
    email,
    tipo_usuario,
    ativo,
    data_criacao
FROM usuarios
ORDER BY id;
GO

PRINT '=================================';
PRINT 'CREDENCIAIS DE ACESSO';
PRINT '=================================';
PRINT 'Admin:';
PRINT '  Email: admin@interfix.com';
PRINT '  Senha: admin123';
PRINT '';
PRINT 'Técnico:';
PRINT '  Email: tecnico@interfix.com';
PRINT '  Senha: tecnico123';
PRINT '=================================';
