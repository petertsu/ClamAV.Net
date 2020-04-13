using System;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Configuration;
using ClamAV.Net.Exceptions;
using ClamAV.Net.Socket;
using Microsoft.Extensions.Logging;

namespace ClamAV.Net.Connection
{
    internal class ConnectionFactory : IConnectionFactory
    {
        private readonly ILoggerFactory mLoggerFactory;
        private readonly ClamAvSettings mClamAvSettings;
        private readonly ILogger<ConnectionFactory> mLogger;

        public ConnectionFactory(Uri connectionUri, ILoggerFactory loggerFactory)
        {
            mLoggerFactory = loggerFactory;

            mLogger = loggerFactory.CreateLogger<ConnectionFactory>();

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
                TcpSocketClient tcpSocketClient =
                    new TcpSocketClient(mClamAvSettings, mLoggerFactory.CreateLogger<TcpSocketClient>());

                await tcpSocketClient.ConnectAsync(cancellationToken).ConfigureAwait(false);

                return tcpSocketClient;
            }
            catch (Exception e)
            {
                mLogger.LogError(e, "ClamAV server connection failed");

                throw new ClamAvException("Failed to create connection. See inner for details", e);
            }
        }
    }
}