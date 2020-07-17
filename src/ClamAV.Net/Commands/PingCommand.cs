using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net.Commands
{
    internal class PingCommand : BaseCommand, ICommand<string>
    {
        private const string EXPECTED_RESPONSE = "PONG";

        public PingCommand() : base(Consts.PING_COMMAND)
        {
        }

        public string ProcessRawResponse(byte[] rawResponse)
        {
            if (rawResponse == null)
                throw new ClamAvException("Raw response is null");

            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            return actualResponse.Contains(EXPECTED_RESPONSE)
                ? EXPECTED_RESPONSE
                : throw new ClamAvException($"Unexpected response '{actualResponse}'");
        }
    }
}