using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClamAV.Net.Socket
{
    public interface ISocketClient : IDisposable
    {
        bool Connected { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);

        //Task<byte[]> SendCommandAsync(byte[] command, bool waitForResponse, Func<Stream, Task> commandData,
        //    CancellationToken cancellationToken);
    }
}