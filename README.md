# ClamAV.Net
ClamAV .net core client

[![Build status](https://ci.appveyor.com/api/projects/status/uep7igf5d3p9kbg2?svg=true)](https://ci.appveyor.com/project/petertsu/clamav-net)


```csharp
IClamAvClient clamAvClient = ClamAvClient.Create(new Uri("tcp://127.0.0.1:3310"));

//PING ClamAV command
await clamAvClient.PingAsync(); 
```
