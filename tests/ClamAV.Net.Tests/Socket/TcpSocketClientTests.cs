using System;
using System.Threading.Tasks;
using ClamAV.Net.Commands;
using ClamAV.Net.Configuration;
using ClamAV.Net.Socket;
using ClamAV.Net.Tests.ClamAvServer;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ClamAV.Net.Tests.Socket
{
    public class TcpSocketClientTests
    {
        [Fact]
        public void Ctor_Validation()
        {
            Assert.Throws<ArgumentNullException>("clamAvSettings",
                () => new TcpSocketClient(null, NullLogger<TcpSocketClient>.Instance));
        }

        [Fact]
        public void Dispose_tests()
        {
            TcpSocketClient client = new TcpSocketClient(new ClamAvSettings("127.0.0.0", 33100), NullLogger<TcpSocketClient>.Instance);

            Action testAction = () => client.Dispose();

            testAction.Should().NotThrow();
            testAction.Should().NotThrow();
            testAction.Should().NotThrow();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(2)]
        public async Task SendCommandAsync_Should_Send_Data_And_Read_Response(int readBufferSize)
        {
            int port = new Random().Next(55000, 56000);

            ClamAvServerMock clamAvServerMock = new ClamAvServerMock(port);

            try
            {
                clamAvServerMock.Start(() => "PONG");

                using TcpSocketClient client = new TcpSocketClient(new ClamAvSettings("127.0.0.1", port, readBufferSize), NullLogger<TcpSocketClient>.Instance);
                await client.ConnectAsync().ConfigureAwait(false);

                string result = await client.SendCommandAsync(new PingCommand()).ConfigureAwait(false);

                result.Should().Be("PONG");
            }
            finally
            {
                clamAvServerMock.Stop();
            }
        }
    }
}