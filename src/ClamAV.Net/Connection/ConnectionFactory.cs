using System;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Socket;

namespace ClamAV.Net.Connection
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public async Task<IConnection> CreateAsync(Uri connectionUri, CancellationToken cancellationToken = default)
        {
            TcpSocketClient tcpSocketClient = new TcpSocketClient(connectionUri);
            await tcpSocketClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
            return tcpSocketClient;
        }
    }
}