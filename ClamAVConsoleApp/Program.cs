using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ClamAV.Net;

namespace ClamAVConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
          
            IClamAvClient clamAvClient = ClamAvClient.Create(new Uri("tcp://127.0.0.1:3310"));

           

            for (int i = 0; i < 5; i++)
            {
               
                await clamAvClient.PingAsync();

           
              //  await clamAvClient.PingAsync();

                var result = await clamAvClient.GetVersionAsync();


                using (HttpClient httpClient = new HttpClient())
                using (Stream stream = await httpClient.GetStreamAsync("https://www.eicar.org/download/eicarcom2.zip"))
                {
                    await clamAvClient.ScanDataAsync(stream);
                }






                //  await clamAvClient.ScanDataAsync(new MemoryStream(new byte[5]));


                //Console.WriteLine($"#{i} {result.ProgramVersion} {result.VirusDBVersion}");
            }

            Console.ReadKey();

        }
    }
}
