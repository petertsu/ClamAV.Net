using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClamAV.Net.Commands.Base
{
    internal interface ICommand

    {
        Task WriteCommandAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}