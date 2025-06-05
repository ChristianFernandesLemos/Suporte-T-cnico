--create database Suporte_Tecnico;
 USE Suporte_Tecnico;

CREATE TABLE Usuario (
    Id_usuario DECIMAL PRIMARY KEY,
    nome VARCHAR(40) NOT NULL,
    senha VARCHAR(20) NOT NULL,
    Cpf CHAR(11)NOT NULL,
    Acess_codigo DECIMAL
);

CREATE TABLE chamados (
    id_chamado DECIMAL PRIMARY KEY,
    categoria VARCHAR(8) NOT NULL,
    prioridade VARCHAR(10) NOT NULL,
    descricao VARCHAR(200) NOT NULL,
    Afetado VARCHAR(10) NOT NULL,
    Data_Registro DATE NOT NULL,
    Solucao VARCHAR(200),
    Contestacoes_Codigo DECIMAL
);

CREATE TABLE Nivel_de_acesso (
    codigo DECIMAL PRIMARY KEY,
    Nivel_acesso VARCHAR(10) NOT NULL
);

CREATE TABLE E_mail (
    E_mail VARCHAR(20) NOT NULL,
    Id_usuario DECIMAL
);

CREATE TABLE Contestacoes (
    Codigo DECIMAL PRIMARY KEY,
    Justificativa VARCHAR(200)
);

CREATE TABLE registra (
    Id_usuario DECIMAL,
    id_chamado DECIMAL
);
 
ALTER TABLE Usuario ADD CONSTRAINT FK_Usuario_2
    FOREIGN KEY (Acess_codigo)
    REFERENCES Nivel_de_acesso (codigo)
    ON DELETE NO ACTION;
 
ALTER TABLE chamados ADD CONSTRAINT FK_chamados_2
    FOREIGN KEY (Contestacoes_Codigo)
    REFERENCES Contestacoes (Codigo)
    ON DELETE CASCADE;
 
ALTER TABLE E_mail ADD CONSTRAINT FK_E_mail_1
    FOREIGN KEY (Id_usuario)
    REFERENCES Usuario (Id_usuario)
    ON DELETE NO ACTION;
 
ALTER TABLE registra ADD CONSTRAINT FK_registra_1
    FOREIGN KEY (Id_usuario)
    REFERENCES Usuario (Id_usuario)
    ON DELETE NO ACTION;
 
ALTER TABLE registra ADD CONSTRAINT FK_registra_2
    FOREIGN KEY (id_chamado)
    REFERENCES chamados (id_chamado)
    ON DELETE NO ACTION;

	alter table Nivel_de_acesso alter column nivel_acesso Varchar (20);
	alter table E_mail alter column E_mail Varchar (30);
    
	-- Tentar inserir usuario sem nome (deve errar)
INSERT INTO Usuario (Id_usuario, senha, Cpf, Acess_codigo) VALUES 
(4, 'failpass', '45678901234', 3);


SELECT * FROM Usuario;

-- Tentar inserir email sem usuario asociado (deve errar)
INSERT INTO E_mail (E_mail, Id_usuario) VALUES ('error@empresa.com', 99);

SELECT * FROM E_mail;

-- Tentar cadastrar um chamado inexistente (deve errar)
INSERT INTO registra (Id_usuario, id_chamado) VALUES (1, 99);

-- Tentar apagar um nivel de acceso em uso (deve errar)
DELETE FROM Nivel_de_acesso WHERE codigo = 1;

-- Deletar contestacoes (deve errar)
DELETE FROM Contestacoes WHERE Codigo = 1;
SELECT * FROM chamados WHERE Contestacoes_Codigo IS NOT NULL; 

-- Tentar atualizar ID de usuario referenciado (deve errar)
UPDATE Usuario SET Id_usuario = 100 WHERE Id_usuario = 1;

-- Tentar inserir chamado sem descricao (deve errar)
INSERT INTO chamados (id_chamado, categoria, prioridade, Afetado, Data_Registro) 
VALUES (5, 'Hardware', 'Media', 'Chris', '2024-08-20');

-- Tentar inserir usuario com ID duplicado (deve errar)
INSERT INTO Usuario (Id_usuario, nome, senha, Cpf, Acess_codigo) 
VALUES (1, 'Duplicado', 'dup123', '99999999999', 3);

-- Tentar inserir uma data invalida (deve errar)
INSERT INTO chamados (id_chamado, categoria, prioridade, descricao, Afetado, Data_Registro) 
VALUES (5, 'Software', 'Media', 'Test', 'Juan', '2023-13-01');

    insert into Usuario values (1,'Christian', 'MinhaSenha','12345678912',2);
	insert into Usuario values (2,'Juan','senhaJuan','21987654321',3)
	insert into Usuario values (3,'Theo','senhaTheo','10192838374',1)
	insert into Usuario values (4,'Nycolas','senhaNycolas','65473923981',1)

    insert into E_mail values ('chriscamplopes@gmail.com',1)
	insert into E_mail values ('Juan@gmail.com',2)

	insert into chamados values (1,'Software','Baixa','O sistema do computador esta com problema','empresa', '01/05/2025','ainda não tem solução', 1);

	insert into Contestacoes values (1,'o problema é critico pois se eu não sonseguir entregar os relatórios a empresa vai parar')
    
	
	
	insert into Nivel_de_acesso values (1,'funcionario');
	insert into Nivel_de_acesso values (2,'Adiministrador');
	insert into Nivel_de_acesso values (3,'Técnico Responsavel');

	select * from E_mail
	select * from Usuario
	select * from chamados
	select * from Contestacoes
	select * from Nivel_de_acesso
	 
	
	 SELECT u.id_usuario, u.nome,u.senha, n.Nivel_acesso FROM Usuario u JOIN Nivel_de_acesso n ON u.acess_codigo = n.codigo;

