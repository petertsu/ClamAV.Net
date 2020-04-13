using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ClamAV.Net;
using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;

namespace ClamAVConsoleApp
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            IClamAvClient clamAvClient = ClamAvClient.Create(new Uri("tcp://127.0.0.1:3310"));

            await clamAvClient.PingAsync();

            VersionResult result = await clamAvClient.GetVersionAsync();

            Console.WriteLine(
                $"ClamAV version - {result.ProgramVersion} , virus database version {result.VirusDbVersion}");

            using (HttpClient httpClient = new HttpClient())
            {
                await using Stream stream =
                    await httpClient.GetStreamAsync("http://www.eicar.org/download/eicar.com.txt");

                ScanResult res = await clamAvClient.ScanDataAsync(stream);

                Console.WriteLine($"Scan result : Infected - {res.Infected} , Virus name {res.VirusName}");
            }
        }
    }
}