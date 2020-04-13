using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Configuration;
using ClamAV.Net.Connection;

namespace ClamAV.Net.Socket
{
    internal class TcpSocketClient : IConnection
    {
        private readonly ClamAvSettings mClamAvSettings;
        private readonly TcpClient mClient;
        private bool mDisposed;

        public TcpSocketClient(ClamAvSettings clamAvSettings)
        {
            mClamAvSettings = clamAvSettings ?? throw new ArgumentNullException(nameof(clamAvSettings));
            mClient = new TcpClient();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await mClient.ConnectAsync(mClamAvSettings.Host, mClamAvSettings.Port).ConfigureAwait(false);
        }

        private async Task<byte[]> ReadResponse(CancellationToken cancellationToken)
        {
            NetworkStream stream = mClient.GetStream();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                do
                {
                    byte[] answerBytes = new byte[1024];

                    int numBytesRead;

                    while ((numBytesRead = await stream.ReadAsync(answerBytes, 0, answerBytes.Length, cancellationToken)
                        .ConfigureAwait(false)) > 0)
                    {
                        if (numBytesRead < answerBytes.Length &&
                            answerBytes[numBytesRead - 1] == Consts.TERMINATION_BYTE)
                        {
                            await memoryStream.WriteAsync(answerBytes, 0, numBytesRead - 1, cancellationToken)
                                .ConfigureAwait(false);
                            break;
                        }

                        await memoryStream.WriteAsync(answerBytes, 0, numBytesRead, cancellationToken)
                            .ConfigureAwait(false);
                    }
                } while (mClient.Available > 0);

                return memoryStream.ToArray();
            }
        }

        public async Task SendCommandAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            NetworkStream stream = mClient.GetStream();
            await command.WriteCommandAsync(stream, cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            await SendCommandAsync(command as ICommand, cancellationToken).ConfigureAwait(false);

            byte[] rawResponse = await ReadResponse(cancellationToken).ConfigureAwait(false);

            return await command.ProcessRawResponseAsync(rawResponse, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
                return;

            mDisposed = true;

            mClient?.Dispose();
        }

        ~TcpSocketClient()
        {
            Dispose(false);
        }
    }
}