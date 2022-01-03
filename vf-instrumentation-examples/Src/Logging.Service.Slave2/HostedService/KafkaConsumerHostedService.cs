using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartWait.Core;

namespace Slave2.HostedService
{
    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly IConsumer<Ignore,string> _messageReceiver;

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger, IConsumer<Ignore, string> messageReceiver)
        {
            _logger = logger;
            _messageReceiver = messageReceiver;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string topic = "vf.kafka_instrumentation_tracing";

            stoppingToken.ThrowIfCancellationRequested();
            var thread = new Thread(() => Start(topic, stoppingToken))
            {
                IsBackground = true
            };
            thread.Start();

            return Task.CompletedTask;
        }

        private void Start(string topic, CancellationToken stoppingToken)
        {
            WaitFor.Condition(()  =>  
            {
                try
                {
                    _messageReceiver.Subscribe(topic);
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var cts = new CancellationTokenSource();
                        Console.CancelKeyPress += (_, e) =>
                        {
                            e.Cancel = true;
                            cts.Cancel();
                        };
                        var msg = _messageReceiver.Consume(cts.Token);
                        _logger.LogInformation(msg.Message.Value, Array.Empty<object>());
                    }
                }
                catch (ConsumeException exception)
                {
                    _logger.LogInformation(exception, exception.Message, Array.Empty<object>());
                    throw;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("A task/operation cancelled exception was caught.", Array.Empty<object>());
                    _messageReceiver.Close();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "An unhandled exception was thrown.", Array.Empty<object>());
                    _messageReceiver.Dispose();
                    return true;
                }

                return true;
            }, $"{nameof(KafkaConsumerHostedService)} was stopped",TimeSpan.FromMinutes(5));
            
        }
    }
}