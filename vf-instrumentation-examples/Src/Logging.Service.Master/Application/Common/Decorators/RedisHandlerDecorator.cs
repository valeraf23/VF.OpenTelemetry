using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedisCache;

namespace Application.Common.Decorators
{
    public class RedisHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly IRedisCacheService _redisCacheService;

        public RedisHandlerDecorator(IRequestHandler<TRequest, TResponse> handler, IRedisCacheService redisCacheService)
        {
            _handler = handler;
            _redisCacheService = redisCacheService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var key = $"{request.GetType().Name}:{Environment.NewLine}{JsonSerializer.Serialize(request)}";
            var cachedData = await _redisCacheService.GetValue(key);
            if (!string.IsNullOrEmpty(cachedData)) return JsonSerializer.Deserialize<TResponse>(cachedData);
            var response = await _handler.Handle(request, cancellationToken);
            await _redisCacheService.SetValue(key, JsonSerializer.Serialize(response));
            return response;
        }
    }
}