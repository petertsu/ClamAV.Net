using System;
using System.Threading.Tasks;
using ClamAV.Net.Commands;
using ClamAV.Net.Socket;
using ClamAV.Net.Tests.ClamAvServer;
using FluentAssertions;
using Xunit;

namespace ClamAV.Net.Tests.Socket
{
    public class TcpSocketClientTests
    {
        [Fact]
        public void Ctor_Validation()
        {
            Assert.Throws<ArgumentNullException>("connectionUri", () => new TcpSocketClient(null));
        }

        [Fact]
        public void Dispose_tests()
        {
            TcpSocketClient client = new TcpSocketClient(new Uri("tcp://127.0.0.1:9090"));

            Action testAction = () => client.Dispose();

            testAction.Should().NotThrow();
            testAction.Should().NotThrow();
            testAction.Should().NotThrow();
        }

        [Fact]
        public async Task SendCommandAsync_Should_Send_Data_And_Read_Response()
        {
            int port = new Random().Next(55000, 56000);
            Uri uri = new Uri($"tcp://127.0.0.1:{port}");

            ClamAvServerMock clamAvServerMock = new ClamAvServerMock(port);

            try
            {
                clamAvServerMock.Start(() => "PONG");

                using TcpSocketClient client = new TcpSocketClient(uri);
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