using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Commands;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Connection;

namespace ClamAV.Net
{
    public class ClamAvClient : IClamAvClient
    {
        private readonly Uri mConnectionUri;
        private readonly IConnectionFactory mConnectionFactory;
    
        public static IClamAvClient Create(Uri connectionUri)
        {
             return new ClamAvClient(connectionUri,new ConnectionFactory());
        }

        private ClamAvClient(Uri connectionUri ,IConnectionFactory connectionFactory)
        {
            mConnectionUri = connectionUri;
            mConnectionFactory = connectionFactory;
        }

        private async Task SendCommand(BaseCommand command, CancellationToken cancellationToken)
        {
            using (IConnection connection = await mConnectionFactory.CreateAsync(mConnectionUri,cancellationToken).ConfigureAwait(false))
            {
                await connection.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<VersionResult> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            VersionCommand cmd = new VersionCommand();

            await SendCommand(cmd, cancellationToken).ConfigureAwait(false);

            return default;
        }

        public async Task PingAsync(CancellationToken cancellationToken = default)
        {
            PingCommand cmd = new PingCommand();

            await SendCommand(cmd, cancellationToken).ConfigureAwait(false);
        }

        public async Task ScanDataAsync(Stream dataStream, CancellationToken cancellationToken = default)
        {
          
            InStreamCommand cmd = new InStreamCommand(dataStream);

            await SendCommand(cmd, cancellationToken).ConfigureAwait(false);
        }
    }
}