using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Connection;

namespace ClamAV.Net.Client
{
    public class ClamAvClient : IClamAvClient
    {
        private readonly Uri mConnectionUri;
        private readonly IConnectionFactory mConnectionFactory;

        public static IClamAvClient Create(Uri connectionUri)
        {
            return new ClamAvClient(connectionUri, new ConnectionFactory());
        }

        private ClamAvClient(Uri connectionUri, IConnectionFactory connectionFactory)
        {
            mConnectionUri = connectionUri;
            mConnectionFactory = connectionFactory;
        }

        private async Task SendCommand(ICommand command, CancellationToken cancellationToken)
        {
            using (IConnection connection = await mConnectionFactory.CreateAsync(mConnectionUri, cancellationToken)
                .ConfigureAwait(false))
            {
                await connection.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<TResponse> SendCommand<TResponse>(ICommand<TResponse> command,
            CancellationToken cancellationToken)
        {
            using (IConnection connection = await mConnectionFactory.CreateAsync(mConnectionUri, cancellationToken)
                .ConfigureAwait(false))
            {
                return await connection.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<VersionResult> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            return await SendCommand(new VersionCommand(), cancellationToken).ConfigureAwait(false);
        }

        public async Task PingAsync(CancellationToken cancellationToken = default)
        {
            await SendCommand(new PingCommand(), cancellationToken).ConfigureAwait(false);
        }

        public async Task<ScanResult> ScanDataAsync(Stream dataStream, CancellationToken cancellationToken = default)
        {
            return await SendCommand(new InStreamCommand(dataStream), cancellationToken).ConfigureAwait(false);
        }
    }
}