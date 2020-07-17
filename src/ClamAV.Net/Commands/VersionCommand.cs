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

        public VersionResult ProcessRawResponse(byte[] rawResponse)
        {
            if (rawResponse == null)
                throw new ClamAvException("Raw response is null");

            string actualResponse = Encoding.ASCII.GetString(rawResponse);

            string[] versions = actualResponse.Split(new[] {Path.AltDirectorySeparatorChar.ToString()},
                StringSplitOptions.RemoveEmptyEntries);

            if (versions.Length < 2)
            {
                throw
                    new ClamAvException($"Unexpected raw response '{actualResponse}'");
            }

            return new VersionResult(versions[0], versions[1]);
        }
    }
}