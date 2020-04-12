using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ClamAV.Net;
using ClamAV.Net.Client;

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
                Console.WriteLine($"#{i}: {result.ProgramVersion} , {result.VirusDbVersion}");

                using (HttpClient httpClient = new HttpClient())
                using (Stream stream = await httpClient.GetStreamAsync("http://www.eicar.org/download/eicar.com.txt?xxxx"))
                {
                    

                   ScanResult res = await clamAvClient.ScanDataAsync(stream);
                   Console.WriteLine($"#{i}: {res.Infected} , {res.VirusName}");
                }






                //  await clamAvClient.ScanDataAsync(new MemoryStream(new byte[5]));


                
            }

            Console.ReadKey();

        }
    }
}
