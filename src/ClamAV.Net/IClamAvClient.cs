using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;

namespace ClamAV.Net
{
    public interface IClamAvClient
    {
        Task<VersionResult> GetVersionAsync(CancellationToken cancellationToken = default);

        Task PingAsync(CancellationToken cancellationToken = default);

        Task<ScanResult> ScanDataAsync(Stream dataStream, CancellationToken cancellationToken = default);
    }
}