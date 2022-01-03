using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka;
using VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File;
using System;
using System.Threading;
using System.Threading.Tasks;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;

namespace KafkaConsoleApp
{
    internal class Program
    {
        private static readonly TimeSpan WorkTime = TimeSpan.FromSeconds(30);

        private static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource(WorkTime);
            var conf = new Configurations();

            using var openTelemetry = VfSdk.CreateTracerProviderBuilder("Kafka-tracing-ConsoleApp", conf.otlp_exporter)
                .AddKafkaInstrumentation()
                .AddConsoleExporter()
                .Build();

            var logger = Logging.CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
                builder
                    .AddConsole()
                    .AddVfLog());

            using var root = VfSdk.StartActiveSpan($"Producer:{conf.producer_topic}", SpanKind.Server);

            try
            {
                logger.Information("-----ConsoleApp WORKING-----");

                _ = Task.Run(() =>
                {
                    Console.CancelKeyPress += (_, e) =>
                    {
                        e.Cancel = true;
                        cancellationTokenSource.Cancel();
                        cancellationTokenSource.Dispose();
                    };
                }, cancellationTokenSource.Token);

                var consumerTask = Task.Run(
                    () => RunConsumer(loggerFactory, conf.consumer_topic, conf.kafka_endpoint, cancellationTokenSource),
                    cancellationTokenSource.Token);

                _ = Task.Run(async () => await RunProducer(cancellationTokenSource, loggerFactory, conf),
                    cancellationTokenSource.Token);

                await consumerTask;
            }
            catch (OperationCanceledException)
            {
                logger.Error("A task/operation cancelled exception was caught.", Array.Empty<object>());
            }
            catch (Exception e)
            {
                logger.Fatal(e, "An unhandled exception was thrown.", Array.Empty<object>());
            }
        }

        private static async Task RunProducer(CancellationTokenSource cancelTokenSource, ILoggerFactory loggerFactory, Configurations conf)
        {
            var config = new ProducerConfig {BootstrapServers = conf.kafka_endpoint, BatchSize = 100000, LingerMs = 100};
            using var sender = VfLoggingKafkaSdk.CreateProducer<string, string>(config, loggerFactory, new[] {"body"});
            while (!cancelTokenSource.IsCancellationRequested)
            {
                var rnd = new Random();
                var number = rnd.NextDouble();
                var sec = Math.Floor(number * 1000);
                await Task.Delay(Convert.ToInt32(sec), cancelTokenSource.Token);
                await Push(sender, loggerFactory, conf.producer_topic);
            }
        }

        private static async Task Push(IProducer<string, string> sender, ILoggerFactory loggerFactory, string producerTopic)
        {

            var msg = new Message<string, string> { Key = "key", Value = $"{DateTime.Now:g}" };

            await VfSdk.AddSpan($"Producer Send Message: \"Key: {msg.Key}-- Value: {msg.Value}\"", async () =>
            {
                var res = await sender.ProduceAsync(producerTopic, msg);
                sender.Flush(TimeSpan.FromSeconds(10));
                loggerFactory.CreateLogger("RunProducer").LogInformation(res.Message.Value, Array.Empty<object>());
            });
        }

        private static void RunConsumer(ILoggerFactory loggerFactory, string consumerTopic, string kafkaEndpoint, CancellationTokenSource stoppingToken)
        {
            var configConsumer = new ConsumerConfig
            {
                BootstrapServers = kafkaEndpoint,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 10000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                GroupId = "group.id"
            };

             using var receiver = VfLoggingKafkaSdk.CreateConsumer<string, string>(
                 configConsumer,
                 loggerFactory,
                 new[] { "body", "messaging.kafka.message_key" }
            );

            Receive(receiver, consumerTopic, loggerFactory.CreateLogger("Consumer"), stoppingToken);
            receiver.Close();
        }

        private static void Receive(IConsumer<string, string> consumer, string consumerTopic, ILogger logger, CancellationTokenSource stoppingToken)
        {
            consumer.Subscribe(consumerTopic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var res = consumer.Consume(stoppingToken.Token);
                        logger.LogInformation($"Key: {res?.Message.Key}-- Value: {res?.Message.Value}", Array.Empty<object>());
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogError("A task/operation cancelled exception was caught.", Array.Empty<object>());
                consumer.Close();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "An unhandled exception was thrown.", Array.Empty<object>());
                consumer.Close();
            }
        }
    }
}