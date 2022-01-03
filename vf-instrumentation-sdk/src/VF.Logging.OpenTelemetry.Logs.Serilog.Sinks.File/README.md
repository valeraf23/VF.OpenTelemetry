# VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File

This package is a workaround for **importing logs to otlp collector**. Logs save to app.log file from which agent read this file and import logs to the collector. Underhood we use [`Serilog.Sinks.File`](https://github.com/serilog/serilog-sinks-file)

### Enable VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File

Add a reference to the
[`VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File`](https://www.test)
package.

```shell
dotnet add package VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File
```
This is typically done in the `CreateHostBuilder` of your `Program` class. `AddVFLog` method adds **VF.Logging provider**.

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddVFLog(context.Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```
**By default:**
- *OutputTemplate*:`{Timestamp:yyyy-MM-dd HH:mm:ss,fff} {Level:u3} trace.id={TraceId} span.id={SpanId}{NewLine}{Message:lj}{NewLine}{Exception}`

- *FilePath*: `/logs/app.log`

**You can override it :**
- *In your `appsettings.json` file, **configuration** is read from the `VFLogging` section*

```csharp
  "VFLogging": {
    "OutputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss,fff} {Level:u3} trace.id={TraceId} span.id={SpanId}{NewLine}{Message:lj}{NewLine}{Exception}"
  }
  ```
- *Pass as argument  `VFLoggingConfiguration ` object in  `AddVFLog ` method*

```csharp
    logging.AddVFLog(x=>
    {
       x.Path = "/Logs/test.log";
       x.Buffered = true;
    });
  ```
  ## Usage

Take a look in the [examples](../../../VF-logging-examples/) project for example usage.