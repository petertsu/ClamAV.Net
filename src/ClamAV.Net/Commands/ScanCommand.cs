using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net.Commands
{
    internal class ScanCommand : BaseCommand, ICommand<ScanResult>
    {
        public ScanCommand(string path) : base($"{Consts.SCAN} {path}")
        {
            _ = !string.IsNullOrWhiteSpace(path) ? path : throw new ArgumentNullException(nameof(path));
        }

        public ScanResult ProcessRawResponse(byte[] rawResponse)
        {
            if (rawResponse == null)
                throw new ClamAvException("Raw response is null");

            string actualResponse = Encoding.ASCII.GetString(rawResponse);

            if (actualResponse.EndsWith("OK", StringComparison.OrdinalIgnoreCase))
                return new ScanResult(false);

            if (!actualResponse.EndsWith("FOUND", StringComparison.OrdinalIgnoreCase))
            {
                throw
                    new ClamAvException($"Unexpected raw response '{actualResponse}'");
            }

            string[] responseParts = actualResponse.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            if (responseParts.Length < 4)
            {
                throw new ClamAvException($"Invalid raw response '{actualResponse}'");
            }

            return new ScanResult(true, responseParts[2]);
        }
    }
}