﻿# Sistema de suporte técnico com IA 🚀

Solução abrangente para automatizar e otimizar o gerenciamento de chamados de suporte técnico. O sistema permite:

Prioridade 1:

Re001: Usuários devem ser capazes de criar chamados, estes chamados devem ter descrição, categoria (software e hardware) e quem  ele afeta, se o problema afeta apenas o funcionário, a equipe ou a empresa. 

Re002: Tratamento de Chamados via I.A, O sistema deve analisar os chamados por meio de uma I.A, e após a análise deve atribuir um nível de prioridade com base na descrição, categoria e quem o chamado afeta os enviar para o responsável técnico, e caso necessário recategorizar o chamado.

Re003: O banco de dados deve ser MS SQL server hospedado em Windows Server.

Prioridade 2:

Re004: Gerenciamento de chamados, Os chamados pendentes e concluídos poderão ser acessados pelo(s) responsável técnico, e podem ser marcados como concluídos após o problema ser resolvido.

Re005: Criação de Relatórios: O sistema deve ser capaz de criar relatórios semanais e mensais sobre os chamados criados, resolvidos e pendentes.

Prioridade 3:

Re006: Níveis de acesso: O Sistema deve possuir níveis de acesso (funcionário, responsável técnico e administrador).

## *User Story* ✍️

|*Quem?*        | *O que?*                                                              |*Para*                                                                                | *Prioridade* |    
|---------------|-----------------------------------------------------------------------|--------------------------------------------------------------------------------------|--------------|
|Funcionario    | Quero criar chamados de suporte com descrição e categoria detalhadas. | Relatar problemas técnicos de forma organizada.                                      |P1            |
|Funcionario    | Quero ver o status de meus chamados anteriores                        | Fazer um acompanhamento sem entrar em contato com a area de TI.                      |P2            |
|Sistema        | Como um sistema de IA, devo analisar a descrição do tíquete.          | Atribuição de prioridade (baixa/média/alta) com base em palavras-chave históricas.   |P1            |
|Sistema        | Como um sistema de IA, devo atribuir chamados ao técnico              | Que o técnico designado resolva o chamado.                                           |P1            |
|Técnico        | Quero Exbir chamados atribuídos                                       | Saber quais chamados estão pendentes para serem resolvidos.                          |P1            | 
|Técnico        | Quero gerenciar meus chamados asginados                               | Ser mais organizado em meu trabalho diário.                                          |P2            |
|Técnico        | Quero marcar meu chamado como resolvido                               | Saber quando o problema foi resolvido.                                               |P2            |
|Técnico        | Quero ver meus chamados já resolvidos                                 | ter um acompanhamento dos problemas resolvidos.                                      |P2            | 
|Administrador  | Desejo poder gerenciar os níveis de acesso                            | que os usuários possam ter sua função atribuída a eles.                              |P1            |
|Administrador  | Desejo poder gerenciar os chamados                                    | Conhecer quais problemas existem na empresa e que prioridade está sendo dada a eles. |P1            |



## Integrantes 👥

Função       | Nome                | Github                                                       |
------------ | --------------------| -------------------------------------------------------------|
Project Owner| Christian Fernandes | [Acessar Github](https://github.com/ChristianFernandesLemos) |
Scrum Master | Juan Vargas         | [Acessar Github](https://github.com/RenteriaJuan)            |
Dev Team     | Théo Pinto          | [Acessar Github](https://github.com/Thorphinm)               |
Dev Team     | Ana Beatriz         | [Acessar Github](https://github.com/Anasouza2802)            |
Dev Team     |Gustavo Gramacho     | [Acessar Github](https://github.com/gramachoo)               |
Dev Team     | Lukas Keiji         | [Acessar Github](https://github.com/Lucaskeiji)              |

## Tecnologias Utilizadas 💻

Categoría | Tecnologías
--------- | -------------
FrontEnd | HTML, CSS e Figma (Modelagem da interface) 
Base De Dados | SQL Server (Windows Server)
IA | BlackBox AI
Metodología | Scrum + Git FLow 

## Metodologia Scrum 🎯

### Planejamento de Sprints
1. Reunião inicial e definição de objetivos.
2. Levantamento de Requisitos (funcionais e não funcionais).
3. Criação do backlog priorizado.
4. Desenho da arquitetura do sistema.
5. Desenho da base de Dados.
6. Desenho da interface com o usuário.
7. definição de níveis de acesso e segurança.
8. Revisão final e planejamento de desenvolvimento.
 

   
