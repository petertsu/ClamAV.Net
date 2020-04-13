using System;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Configuration;
using ClamAV.Net.Exceptions;
using ClamAV.Net.Socket;

namespace ClamAV.Net.Connection
{
    internal class ConnectionFactory : IConnectionFactory
    {
        private readonly ClamAvSettings mClamAvSettings;

        public ConnectionFactory(Uri connectionUri)
        {
            ValidateUri(connectionUri);

            mClamAvSettings = new ClamAvSettings(connectionUri.Host, connectionUri.Port);
        }

        private void ValidateUri(Uri connectionUri)
        {
            if (connectionUri == null)
                throw new ArgumentNullException(nameof(connectionUri));

            if (connectionUri.Scheme != "tcp")
                throw new ArgumentException($"Unsupported protocol {connectionUri.Scheme}", nameof(connectionUri));

            if (string.IsNullOrWhiteSpace(connectionUri.Host))
                throw new ArgumentException($"Invalid host {connectionUri.Host}", nameof(connectionUri));

            if (connectionUri.Port < 0)
                throw new ArgumentException($"Invalid port {connectionUri.Port}", nameof(connectionUri));
        }

        public async Task<IConnection> CreateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                TcpSocketClient tcpSocketClient = new TcpSocketClient(mClamAvSettings);
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