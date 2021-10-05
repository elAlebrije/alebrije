using System;
using Alebrije.Abstractions.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace Alebrije.DependencyInjection
{
    public static class FactoryExtensions
    {
        public static ServicesByNameBuilder<TService> AddFactory<TService>(this IServiceCollection services)
        {
            return new(services);
        }
        
        public static TService GetFactory<TService>(this IServiceProvider provider, string name)
        {
            var factory = provider.GetService<IFactory<TService>>();
            return factory == null
                ? throw new InvalidOperationException($"The factory {typeof(IFactory<TService>)} is not registered. Please use {nameof(FactoryExtensions)}.{nameof(AddFactory)}() to register names.")
                : factory.Create(name);
        }
    }
}