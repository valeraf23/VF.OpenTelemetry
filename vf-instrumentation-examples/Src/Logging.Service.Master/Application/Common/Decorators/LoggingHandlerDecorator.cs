using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Decorators
{
    public class LoggingHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly ILogger<LoggingHandlerDecorator<TRequest, TResponse>> _logger;

        public LoggingHandlerDecorator(IRequestHandler<TRequest, TResponse> handler, ILogger<LoggingHandlerDecorator<TRequest, TResponse>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _logger.LogInformation($"---Request:{Environment.NewLine}{jsonRequest}", Array.Empty<object>());
            var response = await _handler.Handle(request, cancellationToken);
            var jsonResponse = JsonSerializer.Serialize(response);
            _logger.LogInformation($"---Request:{Environment.NewLine}{jsonResponse}", Array.Empty<object>());
            return response;
        }
    }
}