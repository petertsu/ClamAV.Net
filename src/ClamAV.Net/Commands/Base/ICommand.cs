using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClamAV.Net.Commands.Base
{
    internal struct VoidResponse
    {
    }


    internal interface ICommand

    {
        Task WriteCommandAsync(Stream stream, CancellationToken cancellationToken = default);
    }

    internal interface ICommand<TResponse> : ICommand

    {
        Task<TResponse> ProcessRawResponseAsync(byte[] rawResponse, CancellationToken cancellationToken = default);

    }

}