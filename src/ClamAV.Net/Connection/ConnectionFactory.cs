using System;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Exceptions;
using ClamAV.Net.Socket;

namespace ClamAV.Net.Connection
{
    internal class ConnectionFactory : IConnectionFactory
    {
        private readonly Uri mConnectionUri;

        public ConnectionFactory(Uri connectionUri)
        {
            mConnectionUri = connectionUri ?? throw new ArgumentNullException(nameof(connectionUri));
            ValidateUri(connectionUri);
        }

        private void ValidateUri(Uri connectionUri)
        {
            if (connectionUri.Scheme != "tcp")
                throw new ArgumentException($"Unsupported protocol {connectionUri.Scheme}", nameof(connectionUri));
        }

        public async Task<IConnection> CreateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                TcpSocketClient tcpSocketClient = new TcpSocketClient(mConnectionUri);
                await tcpSocketClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
                return tcpSocketClient;
            }
            catch (Exception e)
            {
                throw new ClamAvException("Failed to create connection. See inner for details", e);
            }
        }
    }
}