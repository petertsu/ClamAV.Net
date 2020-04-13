using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Commands;
using ClamAV.Net.Connection;
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
               () => new ClamAvClient(null));
        }

        [Fact]
        public async Task GetVersionAsync_Should_Send_VersionCommand()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken
                cancellationToken) mocks = CreateMocks();

            ClamAvClient clamAvClient =
                new ClamAvClient(mocks.connectionFactoryMock.Object);

            await clamAvClient.GetVersionAsync(mocks.cancellationToken);

            mocks.connectionMock.Verify(
                mock => mock.SendCommandAsync(It.IsAny<VersionCommand>(),
                    It.Is<CancellationToken>(ct => ct == mocks.cancellationToken)), Times.Once());

            mocks.connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        [Fact]
        public async Task PingAsync_Should_Send_PingCommand()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken
                cancellationToken) mocks = CreateMocks();

            ClamAvClient clamAvClient =
                new ClamAvClient(mocks.connectionFactoryMock.Object);

            await clamAvClient.PingAsync(mocks.cancellationToken);

            mocks.connectionMock.Verify(
                mock => mock.SendCommandAsync(It.IsAny<PingCommand>(),
                    It.Is<CancellationToken>(ct => ct == mocks.cancellationToken)), Times.Once());

            mocks.connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        [Fact]
        public async Task ScanDataAsync_Should_Send_InStreamCommand()
        {
            (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken
                cancellationToken) mocks = CreateMocks();

            ClamAvClient clamAvClient =
                new ClamAvClient(mocks.connectionFactoryMock.Object);

            await clamAvClient.ScanDataAsync(Stream.Null, mocks.cancellationToken);

            mocks.connectionMock.Verify(
                mock => mock.SendCommandAsync(It.IsAny<InStreamCommand>(),
                    It.Is<CancellationToken>(ct => ct == mocks.cancellationToken)), Times.Once());

            mocks.connectionMock.Verify(
                mock => mock.Dispose(), Times.Once());
        }

        private (Mock<IConnectionFactory> connectionFactoryMock, Mock<IConnection> connectionMock, CancellationToken cancellationToken) CreateMocks()
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