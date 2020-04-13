using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClamAV.Net.Commands.Base
{
    internal interface ICommand

    {
        string Name { get; }
        Task WriteCommandAsync(Stream stream, CancellationToken cancellationToken = default);
    }

    internal interface ICommand<TResponse> : ICommand

    {
        Task<TResponse> ProcessRawResponseAsync(byte[] rawResponse, CancellationToken cancellationToken = default);
    }
}