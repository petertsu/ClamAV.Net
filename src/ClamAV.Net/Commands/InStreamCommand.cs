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
    internal class InStreamCommand : BaseCommand, ICommand<ScanResult>
    {
        private readonly Stream mDataStream;

        public InStreamCommand(Stream dataStream) : base(Consts.INSTREAM_COMMAND)
        {
            mDataStream = dataStream ?? throw new ArgumentNullException(nameof(dataStream));

            if (!dataStream.CanRead)
                throw new ArgumentException("Data stream should support read", nameof(dataStream));
        }

        protected override async Task WriteCommandDataAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] dataChunk = new byte[64 * 1024]; //Under 85K for LOH
            int numBytesRead;

            mDataStream.Seek(0, SeekOrigin.Begin);

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

            string[] responseParts = actualResponse.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (responseParts.Length < 3)
            {
                throw
                    new ClamAvException($"Invalid raw response '{actualResponse}'");
            }

            return new ScanResult(true, responseParts[^2]);
        }
    }
}