using System;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Socket;

namespace ClamAV.Net.Connection
{
    internal interface IConnectionFactory
    {
        Task<IConnection> CreateAsync(Uri connectionUri, CancellationToken cancellationToken = default);
    }

    class ConnectionFactory : IConnectionFactory
    {


        public async Task<IConnection> CreateAsync(Uri connectionUri, CancellationToken cancellationToken = default)
        {
            TcpSocketClient tcpSocketClient = new TcpSocketClient(connectionUri);
            await tcpSocketClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
            return tcpSocketClient;
        }
    }

    internal interface IConnection : IDisposable
    {
        Task SendCommandAsync(ICommand command, CancellationToken cancellationToken = default);
        Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    }

}
