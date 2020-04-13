using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ClamAV.Net.Samples.Console
{
    internal class Program
    {
        private static async Task Main()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole(opt => opt.Format = ConsoleLoggerFormat.Systemd)
                    .SetMinimumLevel(LogLevel.Information));

            ILogger logger = loggerFactory.CreateLogger<Program>();

            const string connectionString = "tcp://127.0.0.1:3310";
            const string eicarAvTest = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";

            //Create a client
            IClamAvClient clamAvClient = ClamAvClient.Create(new Uri(connectionString), loggerFactory);

            //Send PING command to ClamAV
            await clamAvClient.PingAsync().ConfigureAwait(false);

            //Get ClamAV engine and virus database version
            VersionResult result = await clamAvClient.GetVersionAsync().ConfigureAwait(false);

            logger.LogInformation((
                $"ClamAV version - {result.ProgramVersion} , virus database version {result.VirusDbVersion}"));

            await using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(eicarAvTest));

            //Send a stream to ClamAV scan
            ScanResult res = await clamAvClient.ScanDataAsync(memoryStream).ConfigureAwait(false);

            logger
                .LogInformation(($"Scan result : Infected - {res.Infected} , Virus name {res.VirusName}"));
        }
    }
}