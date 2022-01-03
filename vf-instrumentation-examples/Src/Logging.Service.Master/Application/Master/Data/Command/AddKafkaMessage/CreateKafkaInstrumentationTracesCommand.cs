using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Master.Data.Command.AddKafkaMessage
{
    public class CreateKafkaInstrumentationTracesCommand : IRequest
    {
        public string Value { get; set; } = "Default";

        internal class CreateKafkaInstrumentationTracesHandler : IRequestHandler<CreateKafkaInstrumentationTracesCommand>
        {
            private readonly ILogger<CreateKafkaInstrumentationTracesHandler> _logger;
            private readonly IProducer<Null, string> _messageSender;

            public CreateKafkaInstrumentationTracesHandler(ILogger<CreateKafkaInstrumentationTracesHandler> logger, IProducer<Null, string> messageSender)
            {
                _logger = logger;
                _messageSender = messageSender;
            }

            public async Task<Unit> Handle(CreateKafkaInstrumentationTracesCommand request, CancellationToken cancellationToken)
            {
                var value = $"{request.Value} {DateTime.Now:g}";

                var message = new Message<Null, string>
                {
                    Value = value
                };

                var deliveryReport =
                    await _messageSender.ProduceAsync("vf.kafka_instrumentation_tracing", message, cancellationToken);
                _logger.LogInformation($"DeliveryReport.Status: {Convert.ToString(deliveryReport.Status)}",Array.Empty<object>());
                _logger.LogInformation($"Topic: {deliveryReport.TopicPartitionOffset.Topic}", Array.Empty<object>());
                _logger.LogInformation(value, Array.Empty<object>());
                return Unit.Value;
            }
        }
    }
}