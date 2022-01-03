# ASP.NET Core VF.Logging.OpenTelemetry

This is an [Instrumentation
Library](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/glossary.md#instrumentation-library),
which instruments [ASP.NET Core](https://docs.microsoft.com/aspnet/core) and
collect telemetry about incoming web requests, outgoing HTTP requests , outgoing calls to Redis and  collects telemetry about database operations by instruments [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) and [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient).

## Steps to enable VF.Logging.OpenTelemetry

### Step 1: Install Package

Add a reference to the
[`VF.Logging.OpenTelemetry`](https://www.test)
package.

```shell
dotnet add package VF.Logging.OpenTelemetry
```

### Step 2: Enable VF.OpenTelemetry library at application startup

**VF.OpenTelemetry library must be enabled at application startup.** This is typically done in the `ConfigureServices` of your `Startup` class. The example below enables this instrumentation by using an extension method on `IServiceCollection`.

```csharp
using Microsoft.Extensions.DependencyInjection;
using VF.Logging.OpenTelemetry;

public void ConfigureServices(IServiceCollection services) => services.AddOpenTelemetry(Configuration, "nmaster");
```
In your appsettings.json file, configuration is read from the `VFTelemetry` section:
```csharp
"VFTelemetry": {
  "Instruments": [ "AspNetCore", "HttpClient", "EF", "SqlClient", "Redis", "Kafka" ],
  "OtlpExporterOptions": {
     "Endpoint":http://otel-agent-net:4317/
  }
```
In Instruments section listed all instruments currently supported out of box. You can remove instruments which are not used in your app.
Options for available instruments can be define in `TraceInstrumentation` section:
```csharp
"VFTelemetry": {
  "Instruments": [ "AspNetCore", "HttpClient", "EF", "SqlClient", "Redis", "Kafka" ],
  "TraceInstrumentation": {
    "AspNetCore": {
      "RecordException": true,
      "EnableGrpcAspNetCoreSupport": false
    },
    "SqlClient": {
      "SetDbStatementForText": true,
      "SetDbStatementForStoredProcedure": true
    },
    "Redis": {
      "FlushInterval": {
        "format": "%s",
        "Interval": 5
      }
    }
  },
  "Sampler": "on",
  "OtlpExporterEndpoint": "http://otel-agent-net:4317/"
}
```
**Sampling** is a mechanism to control the noise and overhead introduced by OpenTelemetry by reducing the number of samples of traces collected and sent to the backend. We can `on`, `off` or setup coefficient from `[0..1]` in Sampler section

## Custom instrumentations

We can create it by adding class which implements `IVFInstrumentation` interface and call `AddInstrumentation` method:
```csharp
AddOpenTelemetry(
Configuration,
"nmaster",
builder=> builder.AddInstrumentation(Assembly.GetExecutingAssembly())
)
```

## Configure Otlp Exporter Options
It can be do in two ways :
* From appsetting.json
```csharp
  "OtlpExporterOptions": {
    "Endpoint": "http://otel-agent-net:4317/",
    "TimeoutMilliseconds":  50,
    "ExportProcessorType": "Batch",
    "BatchExportProcessorOptions": {
      "MaxQueueSize": 200,
      "ScheduledDelayMilliseconds": 1,
      "ExporterTimeoutMilliseconds": 10,
      "MaxExportBatchSize": 5000
    }
  }
```
* From SDK :
```csharp
AddOpenTelemetry(
Configuration,
"nmaster",
builder=> builder.AddOtlpExporterOptions(o=>o.TimeoutMilliseconds=10)
)
```
## Custom traces

To create custom trace, use an IVFTracer object from dependency injection (DI).

```csharp
public class GetCustomWithSpan : IRequest
{

        internal class GetCustomWithSpanHandler : IRequestHandler<GetCustomWithSpan>
        {
            private readonly IVFTracer _tracer;
            private readonly DummyService _dummyService;

            public GetCustomWithSpanHandler(IVFTracer tracer, DummyService dummyService)
            {
                _tracer = tracer;
                _dummyService = dummyService;
            }

            public Task<Unit> Handle(GetCustomWithSpan request, CancellationToken cancellationToken)
            {
                _tracer.AddSpan("dotnet.custom.method.span", async ()=> await _dummyService.DoSomeWork ());
                return Task.FromResult(Unit.Value);
            }
        }
}

```
## Usage

Take a look in the [examples](../../../VF-instrumentation-examples/) project for example usage.