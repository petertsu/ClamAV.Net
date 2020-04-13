using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Connection;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net.Client
{
    /// <summary>
    /// ClamAV client
    /// </summary>
    public class ClamAvClient : IClamAvClient
    {
        private readonly IConnectionFactory mConnectionFactory;

        /// <summary>
        /// Create ClamAV client
        /// </summary>
        /// <param name="connectionUri"></param>
        /// <returns></returns>
        public static IClamAvClient Create(Uri connectionUri)
        {
            return new ClamAvClient(new ConnectionFactory(connectionUri));
        }

        internal ClamAvClient(IConnectionFactory connectionFactory)
        {
            mConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        //private async Task SendCommand(ICommand command, CancellationToken cancellationToken)
        //{
        //    using (IConnection connection = await mConnectionFactory.CreateAsync(mConnectionUri, cancellationToken)
        //        .ConfigureAwait(false))
        //    {
        //        await connection.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
        //    }
        //}

        private async Task<TResponse> SendCommand<TResponse>(ICommand<TResponse> command,
            CancellationToken cancellationToken)
        {
            try
            {
                using (IConnection connection = await mConnectionFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false))
                {
                    return await connection.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (ClamAvException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ClamAvException("ClamAV client error occured", e);
            }
        }

        /// <summary>
        /// Ping ClamAV server.
        /// Run PING command on the ClamAV server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token used to operation cancel</param>
        /// <exception cref="ClamAvException">Thrown when command failed</exception>
        public async Task<VersionResult> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            return await SendCommand(new VersionCommand(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Ping ClamAV server.
        /// Run PING command on the ClamAV server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token used to operation cancel</param>
        /// <exception cref="ClamAvException">Thrown when command failed</exception>
        public async Task PingAsync(CancellationToken cancellationToken = default)
        {
            await SendCommand(new PingCommand(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Scan a stream of data. The stream is sent to ClamAV in chunks.
        /// Run INSTREAM command on the ClamAV server
        /// </summary>
        /// <param name="dataStream">Data stream to scan. The stream should support read operation</param>
        /// /// <param name="cancellationToken">Cancellation token used to operation cancel</param>
        /// <returns>ScanResult</returns>
        /// <exception cref="ClamAvException">Thrown when command failed</exception>
        public async Task<ScanResult> ScanDataAsync(Stream dataStream, CancellationToken cancellationToken = default)
        {
            return await SendCommand(new InStreamCommand(dataStream), cancellationToken).ConfigureAwait(false);
        }
    }
}