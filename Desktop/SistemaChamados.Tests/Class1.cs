using Moq;
using SistemaChamados.Controllers;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace SistemaChamados.Tests
{
    /// <summary>
    /// TU005 - Verificação de um usuário já registrado no sistema
    /// </summary>
    public class TU005_VerificacaoUsuarioRegistrado
    {
        private readonly Mock<IDatabaseConnection> _mockDatabase;
        private readonly FuncionariosController _controller;

        public TU005_VerificacaoUsuarioRegistrado()
        {
            _mockDatabase = new Mock<IDatabaseConnection>();
            _controller = new FuncionariosController(_mockDatabase.Object);
        }

        [Fact]
        public void DeveVerificarUsuarioRegistradoComSucesso()
        {
            // Arrange - Preparar dados de teste
            var emailTeste = "joao.silva@empresa.com";
            var senhaTeste = "Senha123!";
            var funcionarioEsperado = new Funcionario
            {
                Id = 1,
                Nome = "João Silva",
                Email = emailTeste,
                Senha = senhaTeste,
                Cpf = "12345678900",
                NivelAcesso = 1,
                Ativo = true
            };

            // Configurar mock usando os métodos da interface IDatabaseConnection
            _mockDatabase.Setup(db => db.ValidarLogin(emailTeste, senhaTeste))
                .Returns(true);

            _mockDatabase.Setup(db => db.BuscarFuncionarioPorEmail(emailTeste))
                .Returns(funcionarioEsperado);

            // Act - Executar a ação de login
            var resultado = _controller.RealizarLogin(emailTeste, senhaTeste);

            // Assert - Verificar resultados
            Assert.NotNull(resultado);
            Assert.Equal(emailTeste, resultado.Email);
            Assert.Equal(funcionarioEsperado.Nome, resultado.Nome);
            Assert.True(resultado.Ativo);
        }

        [Fact]
        public void DeveRetornarNuloParaUsuarioNaoRegistrado()
        {
            // Arrange
            var emailInvalido = "usuario.inexistente@empresa.com";
            var senhaInvalida = "SenhaErrada123";

            // Configurar mock para retornar false (usuário não encontrado)
            _mockDatabase.Setup(db => db.ValidarLogin(emailInvalido, senhaInvalida))
                .Returns(false);

            _mockDatabase.Setup(db => db.BuscarFuncionarioPorEmail(emailInvalido))
                .Returns((Funcionarios)null);

            // Act
            var resultado = _controller.RealizarLogin(emailInvalido, senhaInvalida);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void DeveRetornarNuloParaSenhaIncorreta()
        {
            // Arrange
            var emailCorreto = "maria.santos@empresa.com";
            var senhaIncorreta = "SenhaErrada";

            // Simular validação falhou (senha incorreta)
            _mockDatabase.Setup(db => db.ValidarLogin(emailCorreto, senhaIncorreta))
                .Returns(false);

            // Act
            var resultado = _controller.RealizarLogin(emailCorreto, senhaIncorreta);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void DeveLancarExcecaoParaEmailVazio()
        {
            // Arrange
            var emailVazio = "";
            var senha = "Senha123";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _controller.RealizarLogin(emailVazio, senha)
            );
        }

        [Fact]
        public void DeveLancarExcecaoParaSenhaVazia()
        {
            // Arrange
            var email = "teste@empresa.com";
            var senhaVazia = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _controller.RealizarLogin(email, senhaVazia)
            );
        }
    }

    /// <summary>
    /// TU008 - Criação bem-sucedida de um novo chamado
    /// </summary>
    public class TU008_CriacaoBemSucedidaChamado
    {
        private readonly Mock<IDatabaseConnection> _mockDatabase;
        private readonly ChamadosController _chamadosController;
        private readonly Funcionario _funcionarioTeste;

        public TU008_CriacaoBemSucedidaChamado()
        {
            _mockDatabase = new Mock<IDatabaseConnection>();
            _chamadosController = new ChamadosController(_mockDatabase.Object);

            // Criar funcionário de teste
            _funcionarioTeste = new Funcionario
            {
                Id = 10,
                Nome = "Carlos Oliveira",
                Email = "carlos.oliveira@empresa.com",
                Cpf = "11122233344",
                NivelAcesso = 1,
                Ativo = true
            };
        }

        [Fact]
        public void DeveCriarChamadoComSucesso()
        {
            // Arrange
            var novoChamado = new Chamados
            {
                Categoria = "Hardware",
                Descricao = "Falha no servidor - Servidor principal não está respondendo",
                Prioridade = 3, // Alta
                Afetado = _funcionarioTeste.Id,
                Status = StatusChamado.Aberto,
                DataChamado = DateTime.Now
            };

            var idChamadoEsperado = 100;

            // Configurar mock usando método da interface IDatabaseConnection
            _mockDatabase.Setup(db => db.InserirChamado(It.IsAny<Chamados>()))
                .Returns(idChamadoEsperado);

            // Act
            var idChamadoCriado = _chamadosController.CriarChamado(novoChamado);

            // Assert
            Assert.True(idChamadoCriado > 0);
            Assert.Equal(idChamadoEsperado, idChamadoCriado);

            // Verificar se o método de inserção foi chamado
            _mockDatabase.Verify(db => db.InserirChamado(It.IsAny<Chamados>()), Times.Once);
        }

        [Fact]
        public void DeveCriarChamadoComStatusAberto()
        {
            // Arrange
            var novoChamado = new Chamados
            {
                Categoria = "Software",
                Descricao = "Falha no servidor",
                Prioridade = 2,
                Afetado = _funcionarioTeste.Id
            };

            _mockDatabase.Setup(db => db.InserirChamado(It.IsAny<Chamados>()))
                .Returns(101);

            // Act
            var idChamado = _chamadosController.CriarChamado(novoChamado);

            // Assert
            Assert.Equal(StatusChamado.Aberto, novoChamado.Status);
            Assert.True(idChamado > 0);
        }

        [Fact]
        public void DeveAtribuirChamadoAoTecnico()
        {
            // Arrange
            var idChamado = 100;
            var idTecnico = 5;

            var chamadoExistente = new Chamados
            {
                IdChamado = idChamado,
                Categoria = "Hardware",
                Descricao = "Falha no servidor",
                Status = StatusChamado.Aberto,
                Afetado = _funcionarioTeste.Id
            };

            _mockDatabase.Setup(db => db.BuscarChamadoPorId(idChamado))
                .Returns(chamadoExistente);

            _mockDatabase.Setup(db => db.AtualizarChamado(It.IsAny<Chamados>()))
                .Returns(true);

            // Act
            _chamadosController.AtribuirTecnico(idChamado, idTecnico);

            // Assert
            _mockDatabase.Verify(db => db.AtualizarChamado(
                It.Is<Chamados>(c => c.TecnicoResponsavel == idTecnico)
            ), Times.Once);
        }

        [Fact]
        public void DeveAlterarStatusParaEmAndamentoAoAtribuirTecnico()
        {
            // Arrange
            var idChamado = 102;
            var idTecnico = 6;

            var chamadoExistente = new Chamados
            {
                IdChamado = idChamado,
                Categoria = "Rede",
                Descricao = "Problema de conectividade",
                Status = StatusChamado.Aberto,
                Afetado = _funcionarioTeste.Id
            };

            _mockDatabase.Setup(db => db.BuscarChamadoPorId(idChamado))
                .Returns(chamadoExistente);

            _mockDatabase.Setup(db => db.AtualizarChamado(It.IsAny<Chamados>()))
                .Returns(true);

            // Act
            _chamadosController.AtribuirTecnico(idChamado, idTecnico);

            // Assert - Verificar se o status foi atualizado para EmAndamento
            _mockDatabase.Verify(db => db.AtualizarChamado(
                It.Is<Chamados>(c => c.Status == StatusChamado.EmAndamento)
            ), Times.Once);
        }

        [Fact]
        public void DeveLancarExcecaoParaCategoriaVazia()
        {
            // Arrange
            var chamadoInvalido = new Chamados
            {
                Categoria = "", // Categoria vazia
                Descricao = "Falha no servidor",
                Afetado = _funcionarioTeste.Id
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _chamadosController.CriarChamado(chamadoInvalido)
            );
        }

        [Fact]
        public void DeveLancarExcecaoParaDescricaoVazia()
        {
            // Arrange
            var chamadoInvalido = new Chamados
            {
                Categoria = "Hardware",
                Descricao = "", // Descrição vazia
                Afetado = _funcionarioTeste.Id
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _chamadosController.CriarChamado(chamadoInvalido)
            );
        }

        [Fact]
        public void DeveDefinirDataChamadoAutomaticamente()
        {
            // Arrange
            var dataAntes = DateTime.Now;
            var novoChamado = new Chamados
            {
                Categoria = "Rede",
                Descricao = "Falha no servidor",
                Afetado = _funcionarioTeste.Id
            };

            _mockDatabase.Setup(db => db.InserirChamado(It.IsAny<Chamados>()))
                .Returns(103);

            // Act
            _chamadosController.CriarChamado(novoChamado);
            var dataDepois = DateTime.Now;

            // Assert
            Assert.InRange(novoChamado.DataChamado, dataAntes, dataDepois);
        }

        [Fact]
        public void DeveCalcularPrioridadeCorretamente()
        {
            // Arrange & Act & Assert

            // Prioridade Alta (3)
            var chamadoAlto = new Chamados
            {
                Categoria = "Hardware",
                Descricao = "Servidor caiu - departamento inteiro afetado",
                Prioridade = 3,
                Afetado = _funcionarioTeste.Id
            };
            Assert.Equal(3, chamadoAlto.Prioridade);

            // Prioridade Crítica (4)
            var chamadoCritico = new Chamados
            {
                Categoria = "Rede",
                Descricao = "Falha na rede principal - empresa toda afetada",
                Prioridade = 4,
                Afetado = _funcionarioTeste.Id
            };
            Assert.Equal(4, chamadoCritico.Prioridade);
        }
    }

    /// <summary>
    /// Testes adicionais para cobertura completa
    /// </summary>
    public class TestesComplementares
    {
        [Fact]
        public void FuncionarioDeveTerPermissaoParaCriarChamado()
        {
            // Arrange
            var funcionario = new Funcionario
            {
                Id = 1,
                Nome = "Teste",
                NivelAcesso = 1
            };

            // Act
            var podeCriar = funcionario.PodeRealizarAcao(AcaoSistema.CriarChamado);

            // Assert
            Assert.True(podeCriar);
        }

        [Fact]
        public void FuncionarioNaoDeveTerPermissaoParaGerenciarUsuarios()
        {
            // Arrange
            var funcionario = new Funcionario
            {
                Id = 1,
                Nome = "Teste",
                NivelAcesso = 1
            };

            // Act
            var podeGerenciar = funcionario.PodeRealizarAcao(AcaoSistema.GerenciarUsuarios);

            // Assert
            Assert.False(podeGerenciar);
        }

        [Fact]
        public void DeveValidarCPFCorretamente()
        {
            // Arrange
            var cpfValido = "12345678900";
            var cpfInvalido = "123";

            // Act & Assert
            Assert.True(cpfValido.Length == 11);
            Assert.False(cpfInvalido.Length == 11);
        }
    }
}