# ClamAV.Net
ClamAV .NETStandard 2.0 client

[![Build status](https://ci.appveyor.com/api/projects/status/uep7igf5d3p9kbg2?svg=true)](https://ci.appveyor.com/project/petertsu/clamav-net)


```csharp
private static async Task Main()
{
	const string connectionString = "tcp://127.0.0.1:3310";
	const string eicarAvTest = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";

	//Create a client
	IClamAvClient clamAvClient = ClamAvClient.Create(new Uri(connectionString));

	//Send PING command to ClamAV
	await clamAvClient.PingAsync().ConfigureAwait(false);

	//Get ClamAV engine and virus database version
	VersionResult result = await clamAvClient.GetVersionAsync().ConfigureAwait(false);

	Console.WriteLine(
		$"ClamAV version - {result.ProgramVersion} , virus database version {result.VirusDbVersion}");

	await using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(eicarAvTest)))
	{
		//Send a stream to ClamAV scan
		ScanResult res = await clamAvClient.ScanDataAsync(memoryStream).ConfigureAwait(false);

		Console.WriteLine($"Scan result : Infected - {res.Infected} , Virus name {res.VirusName}");
	}
}
```
