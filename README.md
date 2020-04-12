# ClamAV.Net
ClamAV .net core client

[![Build status](https://ci.appveyor.com/api/projects/status/uep7igf5d3p9kbg2?svg=true)](https://ci.appveyor.com/project/petertsu/clamav-net)


```csharp

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
```
