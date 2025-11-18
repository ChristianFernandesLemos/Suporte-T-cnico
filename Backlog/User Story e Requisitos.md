# *Backlog do Produto* 📋

|*Quem?*        | *O que?*                                                              |*Para*                                                                                | *Prioridade* | *Requisitos*         | *Status*      |
|---------------|-----------------------------------------------------------------------|--------------------------------------------------------------------------------------|--------------|----------------------|---------------| 
|Funcionario    | Quero criar chamados de suporte com descrição e categoria detalhadas. | Relatar problemas técnicos de forma organizada.                                      |P1            |Re 001                |Concluído ✅ |
|Funcionario    | Quero ver o status de meus chamados anteriores                        | Fazer um acompanhamento sem entrar em contato com a area de TI.                      |P2            |Re 001                |Concluído ✅|
|Sistema        | Como um sistema de IA, devo analisar a descrição do chamado.          | Atribuição de prioridade (baixa/média/alta) com base em palavras-chave históricas.   |P1            |Re 002                |Concluído ✅| 
|Sistema        | Como um sistema de IA, devo atribuir chamados ao técnico              | Que o técnico designado resolva o chamado.                                           |P1            |Re 002                |Concluído ✅| 
|Técnico        | Quero Exbir chamados atribuídos                                       | Saber quais chamados estão pendentes para serem resolvidos.                          |P1            |Re 003                |Concluído ✅|
|Técnico        | Quero gerenciar meus chamados asginados                               | Ser mais organizado em meu trabalho diário.                                          |P2            |Re 003                |Concluído ✅|
|Técnico        | Quero marcar meu chamado como resolvido                               | Saber quando o problema foi resolvido.                                               |P2            |Re 003                |Concluído ✅|
|Técnico        | Quero ver meus chamados já resolvidos                                 | ter um acompanhamento dos problemas resolvidos.                                      |P2            |Re 005                |Concluído ✅| 
|Técnico/Admin  | Quero criar relatorios dos chamados criados, resolvidos e pendentes   | para saber qual área está gerando mais problemas na empresa.                         |P2            |Re 004                |Concluído ✅ |
|Administrador  | Desejo poder gerenciar os níveis de acesso                            | que os usuários possam ter sua função atribuída a eles.                              |P1            |Re 005                |Concluído ✅|
|Administrador  | Desejo poder gerenciar os chamados                                    | Conhecer quais problemas existem na empresa e que prioridade está sendo dada a eles. |P1            |Re 003                |Concluído ✅ |
  

## Requisitos 

Prioridade 1: 
Re001: Usuários devem ser capazes de criar chamados, estes chamados devem ter descrição, categoria (software, hardware e Rede) e quem  ele afeta, se o problema afeta apenas o funcionário, a equipe ou a empresa. Os chamados pendentes e concluídos poderão ser acessados pelo funcionario para acompanhar seus status
 
Re002: Tratamento de Chamados via I.A, O sistema deve analisar os chamados por meio de uma I.A, e após a análise deve atribuir um nível de prioridade com base na descrição, categoria e quem o chamado afeta os enviar para o responsável técnico, e caso necessário recategorizar o chamado.

ReNF001: O banco de dados deve ser MS SQL server hospedado em Windows Server.

ReNF002: Desktop: Interface em C# com Windows Forms, WPF para gestão administrativa do sistema, Blazor etc.

ReNF003: Web: ASP.NET e C# para a aplicação web responsiva.

ReNF004: Mobile: Desenvolvimento para Android, permitindo que usuários abram chamados via aplicativo

ReNF005: IA: Os alunos poderão utilizar ferramentas de IA generativa como apoio à elaboração de FAQ, sugestões de resposta automática e documentação, simulando funcionalidades inteligentes com base em dados históricos.

Prioridade 2: 
Re003: Gerenciamento de chamados, Os chamados pendentes e concluídos poderão ser acessados pelo(s) responsável técnico e podem ser marcados como concluídos após o problema ser resolvido.

Re004: Criação de Relatórios: O sistema deve ser capaz de criar relatórios semanais e mensais sobre os chamados criados, resolvidos e pendentes.

Prioridade 3: 
Re005: Níveis de acesso: O Sistema deve possuir níveis de acesso (funcionário, responsável técnico e administrador).




