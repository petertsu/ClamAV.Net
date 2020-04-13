using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Connection;

namespace ClamAV.Net.Socket
{
    internal class TcpSocketClient : IConnection
    {
        private readonly Uri mConnectionString;
        private readonly TcpClient mClient;

        public TcpSocketClient(Uri connectionString)
        {
            mConnectionString = connectionString;
            mClient = new TcpClient();
        }

        public bool Connected => mClient.Connected;

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            await mClient.ConnectAsync(mConnectionString.Host, mConnectionString.Port);
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

                Console.WriteLine($" {Encoding.UTF8.GetString(memoryStream.ToArray())}");

                return memoryStream.ToArray();
            }
        }

        public void Dispose()
        {
            mClient?.Dispose();
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
    }
}