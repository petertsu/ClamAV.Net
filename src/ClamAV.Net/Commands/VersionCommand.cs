using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Commands
{
    internal class VersionCommand : BaseCommand, ICommand<VersionResult>
    {
        public VersionCommand() : base("VERSION")
        {
        }

        public Task<VersionResult> ProcessRawResponseAsync(byte[] rawResponse,
            CancellationToken cancellationToken = default)
        {
            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            string[] versions = actualResponse.Split(Path.AltDirectorySeparatorChar);

            VersionResult versionResult = new VersionResult(versions[0], versions[1]);

            return Task.FromResult(versionResult);
        }
    }
}