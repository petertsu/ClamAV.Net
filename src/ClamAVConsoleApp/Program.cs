using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;

namespace ClamAVConsoleApp
{
    internal static class Program
    {
        private static async Task Main()
        {
            const string connectionString = "tcp://127.0.0.1:3310";
            const string virusTestDownloadUri = "http://www.eicar.org/download/eicar.com.txt";

            IClamAvClient clamAvClient = ClamAvClient.Create(new Uri(connectionString));

            await clamAvClient.PingAsync().ConfigureAwait(false);

            VersionResult result = await clamAvClient.GetVersionAsync().ConfigureAwait(false);

            Console.WriteLine(
                $"ClamAV version - {result.ProgramVersion} , virus database version {result.VirusDbVersion}");

            using HttpClient httpClient = new HttpClient();
            await using Stream stream =
                await httpClient.GetStreamAsync(virusTestDownloadUri).ConfigureAwait(false);

            ScanResult res = await clamAvClient.ScanDataAsync(stream).ConfigureAwait(false);

            Console.WriteLine($"Scan result : Infected - {res.Infected} , Virus name {res.VirusName}");
        }
    }
}