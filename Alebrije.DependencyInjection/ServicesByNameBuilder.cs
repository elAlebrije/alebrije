using System;
using System.Collections.Generic;
using Alebrije.Abstractions.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace Alebrije.DependencyInjection
{
    /// <summary>
    /// Provides easy fluent methods for building named registrations of the same interface
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class ServicesByNameBuilder<TService>
    {
        private readonly IServiceCollection _services;

        private readonly IDictionary<string, Type> _registrations;

        internal ServicesByNameBuilder(IServiceCollection services)
        {
            _services = services;
            _registrations = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        public ServicesByNameBuilder<TService> Add(string name, Type implementationType)
        {
            _registrations.Add(name, implementationType);
            return this;
        }

        public ServicesByNameBuilder<TService> Add<TImplementation>(string name)
            where TImplementation : TService
        {
            return Add(name, typeof(TImplementation));
        }

        public void Build()
        {
            var registrations = _registrations;
            _services.AddTransient<IFactory<TService>>(s => new ServiceByNameFactory<TService>(s, registrations));
        }
    }
}