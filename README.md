# ClamAV.Net
![ClamAV Logo](http://www.clamav.net/assets/clamav-trademark.png)

ClamAV .NETCore 2.1 client

[![Build status](https://ci.appveyor.com/api/projects/status/uep7igf5d3p9kbg2?svg=true)](https://ci.appveyor.com/project/petertsu/clamav-net)
[![NuGet version](https://badge.fury.io/nu/ClamAV.Net.svg)](https://badge.fury.io/nu/ClamAV.Net)


[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=bugs)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=code_smells)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=coverage)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=alert_status)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=security_rating)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=petertsu_ClamAV.Net&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=petertsu_ClamAV.Net)


## Usage example
Create Uri in form "tcp://clamav-server:3310" and pass it to ClamAV.Net client

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
## Output

```bash
ClamAV version - ClamAV 0.102.1 , virus database version 25779
Scan result : Infected - True , Virus name Win.Test.EICAR_HDB-1
```

## Use .NET Core Logger
ClamAV.Net client has optional parameter for Microsoft.Extensions.Logging.ILoggerFactory.

```csharp
ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole(opt => opt.Format = ConsoleLoggerFormat.Systemd)
                    .SetMinimumLevel(LogLevel.Information));

//Create a client
IClamAvClient clamAvClient = ClamAvClient.Create(new Uri("tcp://127.0.0.1:3310"), loggerFactory);
```
## Output systemd style

```
<7>ClamAV.Net.Socket.TcpSocketClient[0] Connecting to Server=127.0.0.1:3310 , parameters (ReadBufferSize=1024)
<7>ClamAV.Net.Socket.TcpSocketClient[0] Start writing command 'INSTREAM' to the network stream
<7>ClamAV.Net.Socket.TcpSocketClient[0] End writing command 'INSTREAM' to the network stream
<7>ClamAV.Net.Socket.TcpSocketClient[0] Start reading command 'INSTREAM' response
<7>ClamAV.Net.Socket.TcpSocketClient[0] End reading command 'INSTREAM' response. Total 34 bytes
<7>ClamAV.Net.Socket.TcpSocketClient[0] Socket disposed
<6>ClamAV.Net.Samples.Console.Program[0] Scan result : Infected - True , Virus name Win.Test.EICAR_HDB-1
```


## Run ClamAV docker

```bash
    docker run -d -p 3310:3310 mkodockx/docker-clamav:alpine
```
