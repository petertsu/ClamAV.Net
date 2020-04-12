using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net
{
    public interface IClamAvClient
    {
        /// <summary>
        /// Get ClamAV engine and database versions.
        /// Run VERSION command on the ClamAV server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token used to operation cancel</param>
        /// <returns>VersionResult</returns>
        /// <exception cref="ClamAVException">Thrown when command failed</exception>
        Task<VersionResult> GetVersionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Ping ClamAV server.
        /// Run PING command on the ClamAV server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token used to operation cancel</param>
        /// <exception cref="ClamAVException">Thrown when command failed</exception>
        Task PingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Scan a stream of data. The stream is sent to ClamAV in chunks.
        /// Run INSTREAM command on the ClamAV server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token used to operation cancel</param>
        /// <param name="dataStream">Data stream to scan. The stream should support read operation</param>
        /// <returns>ScanResult</returns>
        /// <exception cref="ClamAVException">Thrown when command failed</exception>
        Task<ScanResult> ScanDataAsync(Stream dataStream, CancellationToken cancellationToken = default);
    }
}