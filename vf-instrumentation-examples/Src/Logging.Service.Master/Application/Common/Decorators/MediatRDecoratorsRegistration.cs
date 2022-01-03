using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Application.Common.Decorators
{
    public static class MediatRDecoratorsRegistration
    {
        public static IServiceCollection AddDecorators(this IServiceCollection services)
        {

            var pipeline = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Any(IsHandlerInterface)).Select(handlerType =>
                {
                    var decoratorsType = ToDecorate(handlerType).Reverse().ToList();
                    var interfaceType = handlerType.GetInterfaces().Single(IsHandlerInterface);
                    var genericArguments = interfaceType.GenericTypeArguments;
                    return (genericArguments, interfaceType, decoratorsType);
                }).Where(pipe => pipe.decoratorsType.Any());

            foreach (var pipe in pipeline)
            {
                services.RegisterDecorators(pipe);
            }

            return services;
        }

        private static void RegisterDecorators(this IServiceCollection services,
            (Type[] genericArguments,
                Type interfaceType,
                List<Type> decoratorsType)
                pipe)
        {
            var (genericArguments, interfaceType, decoratorsType) = pipe;
            foreach (var genericDecorator in decoratorsType.Select(decoratorType =>
                decoratorType.MakeGenericType(genericArguments)))
            {
                services.Decorate(interfaceType, genericDecorator);
            }
        }

        private static IEnumerable<Type> ToDecorate(ICustomAttributeProvider type)
        {
            var attributes = type.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case RedisCacheAttribute:
                        yield return typeof(RedisHandlerDecorator<,>);
                        break;
                    case LoggingAttribute:
                        yield return typeof(LoggingHandlerDecorator<,>);
                        break;
                }
            }
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IRequestHandler<,>);
        }

    }
}