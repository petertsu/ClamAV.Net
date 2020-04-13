using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net.Commands
{
    internal class InStreamCommand : BaseCommand, ICommand<ScanResult>
    {
        private readonly Stream mDataStream;

        public InStreamCommand(Stream dataStream) : base("INSTREAM")
        {
            mDataStream = dataStream ?? throw new ArgumentNullException(nameof(dataStream));

            if (!dataStream.CanRead)
                throw new ArgumentException("Data stream should support read", nameof(dataStream));
        }

        protected override async Task WriteCommandDataAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] dataChunk = new byte[1024];
            int numBytesRead;

            while ((numBytesRead = await mDataStream.ReadAsync(dataChunk, 0, dataChunk.Length, cancellationToken)
                .ConfigureAwait(false)) > 0)
            {
                byte[] dataChunkSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(numBytesRead));

                await stream.WriteAsync(dataChunkSize, 0, dataChunkSize.Length, cancellationToken)
                    .ConfigureAwait(false); //<length>
                await stream.WriteAsync(dataChunk, 0, numBytesRead, cancellationToken).ConfigureAwait(false); //data
            }

            byte[] end = BitConverter.GetBytes(0);

            await stream.WriteAsync(end, 0, end.Length, cancellationToken).ConfigureAwait(false);
        }

        public Task<ScanResult> ProcessRawResponseAsync(byte[] rawResponse,
            CancellationToken cancellationToken = default)
        {
            if (rawResponse == null)
                return Task.FromException<ScanResult>(new ClamAvException("Raw response is null"));

            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            if (actualResponse.EndsWith("OK", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(new ScanResult(false));

            if (!actualResponse.EndsWith("FOUND", StringComparison.OrdinalIgnoreCase))
                return Task.FromException<ScanResult>(
                    new ClamAvException($"Unexpected raw response '{actualResponse}'"));
           
            string[] responseParts = actualResponse.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            if (responseParts.Length < 2)
                return Task.FromException<ScanResult>(
                    new ClamAvException($"Invalid raw response '{actualResponse}'"));

            return Task.FromResult(new ScanResult(true, responseParts[1]));

        }
    }
}