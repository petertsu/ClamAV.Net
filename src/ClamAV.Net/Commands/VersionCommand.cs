using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net.Commands
{
    internal class VersionCommand : BaseCommand, ICommand<VersionResult>
    {
        public VersionCommand() : base(Consts.VERSION_COMMAND)
        {
        }

        public Task<VersionResult> ProcessRawResponseAsync(byte[] rawResponse,
            CancellationToken cancellationToken = default)
        {
            if (rawResponse == null)
                return Task.FromException<VersionResult>(new ClamAvException("Raw response is null"));

            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            string[] versions = actualResponse.Split(new[] { Path.AltDirectorySeparatorChar.ToString() },
                StringSplitOptions.RemoveEmptyEntries);

            if (versions.Length < 2)
            {
                return Task.FromException<VersionResult>(
                    new ClamAvException($"Unexpected raw response '{actualResponse}'"));
            }

            VersionResult versionResult = new VersionResult(versions[0], versions[1]);

            return Task.FromResult(versionResult);
        }
    }
}