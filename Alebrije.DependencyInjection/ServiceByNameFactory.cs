using System;
using System.Collections.Generic;
using Alebrije.Abstractions.Patterns;

namespace Alebrije.DependencyInjection
{
    internal class ServiceByNameFactory<TService> : IFactory<TService>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _registrations;

        public ServiceByNameFactory(IServiceProvider serviceProvider, IDictionary<string, Type> registrations)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
        }

        public TService Create()
        {
            return _registrations.TryGetValue(string.Empty, out var implementationType)
                ? (TService)_serviceProvider.GetService(implementationType)
                : default;
        }

        public TService Create(string name)
        {
            return _registrations.TryGetValue(name, out var implementationType)
                ? (TService)_serviceProvider.GetService(implementationType)
                : throw new ArgumentException($"Service name '{name}' is not registered");
        }
    }
}