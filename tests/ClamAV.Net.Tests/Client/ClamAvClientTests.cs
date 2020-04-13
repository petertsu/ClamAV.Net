using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Commands;
using ClamAV.Net.Connection;
using ClamAV.Net.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ClamAV.Net.Tests.Client
{
    public class ClamAvClientTests
    {
        [Fact]
        public void Ctor_Parameter_Validation()
        {
            Assert.Throws<ArgumentNullException>("connectionFactory",
                () => new ClamAvClient(null, new NullLogger<ClamAvClient>()));
        }

        [Fact]
        public async Task GetVersionAsync_Should_Send_VersionCommand()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken cancellationToken) = CreateMocks();

            ClamAvClient clamAvClient =
                new ClamAvClient(connectionFactoryMock.Object, new NullLogger<ClamAvClient>());

            await clamAvClient.GetVersionAsync(cancellationToken).ConfigureAwait(false);

            connectionMock.Verify(
                mock => mock.SendCommandAsync(It.IsAny<VersionCommand>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)), Times.Once());

            connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        [Fact]
        public async Task PingAsync_Should_Send_PingCommand()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken cancellationToken) = CreateMocks();

            ClamAvClient clamAvClient =
                new ClamAvClient(connectionFactoryMock.Object, new NullLogger<ClamAvClient>());

            await clamAvClient.PingAsync(cancellationToken).ConfigureAwait(false);

            connectionMock.Verify(
                mock => mock.SendCommandAsync(It.IsAny<PingCommand>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)), Times.Once());

            connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        [Fact]
        public async Task PingAsync_Should_Throw_ClamAvException_With_Inner()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock,
                CancellationToken cancellationToken) = CreateMocks();

            Exception thrownException = new Exception("Some error");

            ClamAvClient clamAvClient =
                new ClamAvClient(connectionFactoryMock.Object, new NullLogger<ClamAvClient>());

            connectionMock.Setup(
                mock => mock.SendCommandAsync(It.IsAny<PingCommand>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken))).Throws(thrownException);

            ClamAvException actualException = await Assert.ThrowsAsync<ClamAvException>(async () =>
                await clamAvClient.PingAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            actualException.InnerException.Should().BeEquivalentTo(thrownException);

            connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        [Fact]
        public async Task PingAsync_Should_Throw_ClamAvException()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock,
                CancellationToken cancellationToken) = CreateMocks();

            ClamAvException thrownException = new ClamAvException("Some error");

            ClamAvClient clamAvClient =
                new ClamAvClient(connectionFactoryMock.Object, new NullLogger<ClamAvClient>());

            connectionMock.Setup(
                mock => mock.SendCommandAsync(It.IsAny<PingCommand>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken))).Throws(thrownException);

            ClamAvException actualException = await Assert.ThrowsAsync<ClamAvException>(async () =>
                await clamAvClient.PingAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            actualException.Should().BeEquivalentTo(thrownException);

            connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        [Fact]
        public void Create_Should_Create_Client()
        {
            IClamAvClient clamAvClient = ClamAvClient.Create(new Uri("tcp://127.0.0.1:33753"));
            clamAvClient.Should().NotBeNull();
        }

        [Fact]
        public async Task ScanDataAsync_Should_Send_InStreamCommand()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken cancellationToken) = CreateMocks();

            ClamAvClient clamAvClient =
                new ClamAvClient(connectionFactoryMock.Object, new NullLogger<ClamAvClient>());

            await clamAvClient.ScanDataAsync(Stream.Null, cancellationToken).ConfigureAwait(false);

            connectionMock.Verify(
                mock => mock.SendCommandAsync(It.IsAny<InStreamCommand>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)), Times.Once());

            connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        private static (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken cancellationToken) CreateMocks()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken cancellationToken) data = (
                    new Mock<IConnectionFactory>(),
                    new Mock<IConnection>(),
                    new CancellationToken()
                    );

            data.connectionFactoryMock.Setup(connectionFactory =>
                    connectionFactory.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(data.connectionMock.Object));

            return data;
        }
    }
}