using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SimulationApi;

[ApiController]
[Route("/")]
public class TestController : ControllerBase
{
    [HttpGet("/delay/{seconds:int}")]
    public async Task<IActionResult> Delay([FromRoute] int seconds = 10)
    {
        await Task.Delay(seconds * 1000);

        return Ok(GetSystemInfo());
    }

    [HttpGet("/cpu/{seconds:int}/{percentage:int}")]
    public IActionResult Cpu(int seconds = 10, int percentage = 100)
    {
        var timeControl = new Stopwatch();
        timeControl.Start();

        var tasks = new List<Task>();

        for (var i = 0; i < Environment.ProcessorCount; i++)
        {
            tasks.Add(
                Task.Factory.StartNew(() =>
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    while (true)
                    {
                        if (timeControl.Elapsed.Seconds > seconds)
                            break;

                        if (watch.ElapsedMilliseconds > percentage)
                        {
                            Thread.Sleep(100 - percentage);
                            watch.Reset();
                            watch.Start();
                        }
                    }
                })
            );
        }

        Task.WaitAll(tasks.ToArray());

        return Ok(GetSystemInfo());
    }

    [HttpGet("/memory/{seconds:int}/{sizeInM:int}")]
    public async Task<IActionResult> Memory(int seconds = 10, int sizeInM = 1024 )
    {
        var m = 1024 * 1024;
        var bs = new byte[m];
        
        var ps = new List<IntPtr>();
        for (var i = 0; i < sizeInM; i++)
        {
            var p = Marshal.AllocHGlobal(m);
            Marshal.Copy(bs, 0, p, bs.Length);
            
            ps.Add(p);
        }
        
        await Task.Delay(seconds * 1000);

        foreach (var ptr in ps)
            Marshal.FreeHGlobal(ptr);

        return Ok(GetSystemInfo());
    }

    [HttpGet("/disk/{seconds:int}/{sizeInM:int}")]
    public async Task<IActionResult> Disk(int seconds = 10, int sizeInM = 1024)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"test_file_{Guid.NewGuid().ToString()}");

        var data = new byte[8192];
        var rng = new Random();
        await using (var stream = System.IO.File.OpenWrite(tempFile))
        {
            for (var i = 0; i < sizeInM * 128; i++)
            {
                rng.NextBytes(data);
                stream.Write(data, 0, data.Length);
            }
        }
        
        await Task.Delay(seconds * 1000);
        
        System.IO.File.Delete(tempFile);
        
        return Ok(GetSystemInfo());
    }

    [HttpGet("/{statusCode:int}")]
    public IActionResult ReturnStatusCode(int statusCode)
    {
        return StatusCode(statusCode);
    }
    
    [HttpGet("exception")]
    public IActionResult Exception()
    {
        throw new Exception("Exception simulation.");
    }
    
    [HttpGet("exit")]
    public void Exit()
    {
        Environment.Exit(0);
    }
    
    [HttpGet("crash")]
    public void Crash()
    {
        Environment.FailFast("Application crash simulation.");
    }
    
    private dynamic GetSystemInfo()
    {
        var info = new
        {
            Hostname = Environment.MachineName,
            OsPlatform = RuntimeInformation.OSDescription,
            IpAddressV4 = GetIpAddressV4(),
            IpAddressV6 = GetIpAddressV6(),
            IpAddressesAll = GetAllIpAddresses(),
            AppName = Environment.GetEnvironmentVariable("APP_NAME"),
            DotNetCoreVersion = GetNetCoreVersion(),
            AspNetCoreVersion = GetAspNetCoreVersion(),
            AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Now = DateTimeOffset.Now.ToString(),
        };

        return info;
    }
    
    private string GetNetCoreVersion()
    {
        var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
        var assemblyPath = assembly.Location.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        var netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
        if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
            return assemblyPath[netCoreAppIndex + 1];

        return string.Empty;
    }

    private string? GetAspNetCoreVersion()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_VERSION");
        if (env != null) return env;

        return Assembly
            .GetEntryAssembly()?
            .GetCustomAttribute<TargetFrameworkAttribute>()?
            .FrameworkName;
    }
    
    private string? GetAllIpAddresses()
    {
        return string.Join(",", Dns.GetHostAddresses(Dns.GetHostName()).Select(_ => _.ToString()));
    }
    
    private string? GetIpAddressV4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(_ => _.AddressFamily == AddressFamily.InterNetwork)
            ?.ToString();
    }
    
    private string? GetIpAddressV6()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(_ => _.AddressFamily == AddressFamily.InterNetworkV6)
            ?.ToString();
    }
}