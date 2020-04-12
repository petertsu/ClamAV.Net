using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Commands
{
    internal class PingCommand : BaseCommand, ICommand<string>
    {
        private const string EXPECTED_RESPONSE = "PONG";

        public PingCommand() : base("PING")
        {
        }

        public Task<string> ProcessRawResponseAsync(byte[] rawResponse, CancellationToken cancellationToken = default)
        {
            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            if (string.Equals(EXPECTED_RESPONSE, actualResponse, StringComparison.Ordinal))
                return Task.FromResult(actualResponse);

            return Task.FromException<string>(new Exception($"Unexpected response {actualResponse}"));
        }
    }
}