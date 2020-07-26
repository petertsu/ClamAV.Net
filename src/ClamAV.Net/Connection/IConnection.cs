using System;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Connection
{
    internal interface IConnection : IDisposable
    {
        Task SendCommandAsync(ICommand command, CancellationToken cancellationToken = default);

        Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

        bool IsConnected { get; }
    }
}