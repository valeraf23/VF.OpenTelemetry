VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka for Apache Kafka<sup>TM</sup>
=======================================================================================

To install np.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka from within Visual Studio, search for Confluent.Kafka in the NuGet Package Manager UI, or run the following command in the Package Manager Console:

```
Install-Package VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka
```

To add a reference to a dotnet core project, execute the following at the command line:

```
dotnet add package VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka
```

Note: `VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka` depends on the `Confluent.Kafka` package.

## Usage

Take a look in the [examples](../../../VF-logging-demo/) project for example usage.

### Basic Producer Examples

You should use the `VFSdk.CreateTracerProviderBuilder` method if you would like to push activities to otl-collector. And subscribe to `Message Source` by use method `AddKafkaInstrumentation`.
And you have to create root active span by `VFSdk.StartActiveSpan(string name, SpanKind kind = SpanKind.Internal)`.

```csharp
class Program
{
    public static async Task Main(string[] args)
    {
        using var otl = VFSdk.CreateTracerProviderBuilder("Kafka-tracing", "http://localhost:4317/")
                .AddKafkaInstrumentation()
                .AddConsoleExporter()
                .Build();

        var log = LoggerFactory.Create(builder=> builder.AddConsole());
        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

        using var root = VFSdk.StartActiveSpan($"Producer", SpanKind.Server);

        using (var p = VFLoggingKafkaSdk.CreateProducer<Null, string>(config, log))
        {
            try
            {
                var dr = await p.ProduceAsync("test-topic", new Message<Null, string> { Value="test" });
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}
```

### Basic Consumer Example

```csharp
class Program
{
    public static void Main(string[] args)
    {
        using var otl = VFSdk.CreateTracerProviderBuilder("Kafka-tracing", "http://localhost:4317/")
                .AddKafkaInstrumentation()
                .AddConsoleExporter()
                .Build();

        var log = LoggerFactory.Create(builder=> builder.AddConsole());
        var conf = new ConsumerConfig
        {
            GroupId = "test-consumer-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var root = VFSdk.StartActiveSpan($"Consumer", SpanKind.Server);

        using (var c = VFLoggingKafkaSdk.CreateConsumer<Ignore, string>(conf, log)
        {
            c.Subscribe("my-topic");

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume(cts.Token);
                        Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                c.Close();
            }
        }
    }
}
```

### IHostedService and Web Application Integration

The [Web](../../../VF-logging-examples/Src/Logging.Api/Api) example demonstrates how to integrate
Apache Kafka with a web application, including how to implement `IHostedService` to realize a long running consumer poll loop, how to
register a producer as a singleton service, and how to bind configuration from an injected `IConfiguration` instance.