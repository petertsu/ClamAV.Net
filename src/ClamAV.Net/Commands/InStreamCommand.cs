using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Commands
{
    internal class InStreamCommand : BaseCommand , ICommand<ScanResult>
    {
        private readonly Stream mDataStream;

        public InStreamCommand(Stream dataStream) : base("INSTREAM")
        {
            mDataStream = dataStream;
        }
        
        protected override async Task WriteCommandDataAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] dataChunk = new byte[3];
            int numBytesRead;

            while ((numBytesRead = await mDataStream.ReadAsync(dataChunk, 0, dataChunk.Length, cancellationToken).ConfigureAwait(false)) > 0)
            {
                byte[] dataChunkSize = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(numBytesRead));

                await stream.WriteAsync(dataChunkSize, 0, dataChunkSize.Length, cancellationToken).ConfigureAwait(false); //<length>
                await stream.WriteAsync(dataChunk, 0, numBytesRead, cancellationToken).ConfigureAwait(false); //data
            }

            byte[] end = BitConverter.GetBytes(0);

            await stream.WriteAsync(end, 0, end.Length, cancellationToken).ConfigureAwait(false);
        }


        public Task<ScanResult> ProcessRawResponseAsync(byte[] rawResponse, CancellationToken cancellationToken = default)
        {
            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            if (actualResponse.EndsWith("OK", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(new ScanResult(false));

            if (actualResponse.EndsWith("FOUND", StringComparison.OrdinalIgnoreCase))
            {
                string[] virusName = actualResponse.Split(' ');

                return Task.FromResult(new ScanResult(true, virusName[1]));
            }

            return Task.FromException<ScanResult>(new Exception($"Unexpected response {actualResponse}"));

        }
    }
}