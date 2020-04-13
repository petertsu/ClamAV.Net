using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClamAV.Net.Connection;
using ClamAV.Net.Exceptions;
using ClamAV.Net.Socket;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ClamAV.Net.Tests.Connection
{
    public class ConnectionFactoryTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("xx://server")]
        [InlineData("tcp:/server")]
        [InlineData("tcp://server")]
      
        public void Ctor_Parameter_Validation(string invalidUri)
        {
            if(invalidUri == null)
                Assert.Throws<ArgumentNullException>("connectionUri", () => new ConnectionFactory(null,NullLoggerFactory.Instance));
            else
                Assert.Throws<ArgumentException>("connectionUri", () => new ConnectionFactory(new Uri(invalidUri), NullLoggerFactory.Instance));
        }

        [Fact]
        public async Task CreateAsync_Failed_Should_Throw_ClamAvException()
        {
            IConnectionFactory connectionFactory = new ConnectionFactory(new Uri("tcp://127.0.0.1:12897"), NullLoggerFactory.Instance);

            ClamAvException ex =
                await Assert.ThrowsAsync<ClamAvException>(async () => await connectionFactory.CreateAsync().ConfigureAwait(false)).ConfigureAwait(false);

            ex.InnerException.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_Connected_Should_Return()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 57310);

            listener.Start();

            IConnectionFactory connectionFactory = new ConnectionFactory(new Uri("tcp://127.0.0.1:57310"), NullLoggerFactory.Instance);

            IConnection connection = await connectionFactory.CreateAsync().ConfigureAwait(false);

            connection.GetType().Should().Be(typeof(TcpSocketClient));

            listener.Stop();
        }
    }
}