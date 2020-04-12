using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Connection;

namespace ClamAV.Net.Socket
{
    internal class TcpSocketClient : ISocketClient , IConnection
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
        

        public async Task<byte[]> SendCommandAsyncold(byte[] command, bool waitForResponse, Func<Stream, Task> commandData, CancellationToken cancellationToken)
        {
            NetworkStream stream = mClient.GetStream();
            await stream.WriteAsync(command, 0, command.Length, cancellationToken).ConfigureAwait(false);

            if (commandData != null)
                await commandData.Invoke(stream).ConfigureAwait(false);

            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);

            while (waitForResponse && mClient.Available == 0)
            {
                await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            }

            if (waitForResponse)
                return await ReadResponse(cancellationToken).ConfigureAwait(false);
            else
            {
                return new byte[0];
            }
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
                        await memoryStream.WriteAsync(answerBytes, 0, numBytesRead, cancellationToken)
                            .ConfigureAwait(false);

                        if (numBytesRead < answerBytes.Length && answerBytes[numBytesRead - 1] == 0)
                            break;
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

            await ReadResponse(cancellationToken);



        }
    }
}