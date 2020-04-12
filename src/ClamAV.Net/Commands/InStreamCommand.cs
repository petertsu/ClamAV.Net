using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Commands
{
    internal class InStreamCommand : BaseCommand
    {
        private readonly Stream mDataStream;

        public InStreamCommand(Stream dataStream) : base("INSTREAM")
        {
            mDataStream = dataStream;
        }
        
        protected override async Task WriteCommandDataAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] dataChunk = new byte[1024];

            while (await mDataStream.ReadAsync(dataChunk, 0, dataChunk.Length, cancellationToken).ConfigureAwait(false) > 0)
            {
                byte[] dataChunkSize = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(dataChunk.Length));

                await stream.WriteAsync(dataChunkSize, 0, dataChunkSize.Length, cancellationToken).ConfigureAwait(false); //<length>
                await stream.WriteAsync(dataChunk, 0, dataChunk.Length, cancellationToken).ConfigureAwait(false); //data
            }

            byte[] end = BitConverter.GetBytes(0);

            await stream.WriteAsync(end, 0, end.Length, cancellationToken).ConfigureAwait(false);
        }


    }
}