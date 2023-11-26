using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.ToolboxAppOptions.Services
{
    internal class StartupValidationService : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigurationOptions _configurationOptions;
        public StartupValidationService(
            IServiceProvider serviceProvider,
            ConfigurationOptions configurationOptions)
        {
            _serviceProvider = serviceProvider;
            _configurationOptions = configurationOptions;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            List<Exception> exceptions = null;

            foreach (var type in _configurationOptions.ConfigurationTypes)
            {
                try
                {
                    _ = _serviceProvider.GetRequiredService(type);
                }
                catch (OptionsValidationException ex)
                {
                    if (exceptions is null)
                        exceptions = new List<Exception>();
                    exceptions.Add(ex);
                }
            }

            if (exceptions is null || exceptions.Count == 0)
                return Task.CompletedTask;

            if (exceptions.Count == 1)
            {
                // Rethrow if it's a single error
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            if (exceptions.Count > 1)
            {
                // Aggregate if we have many errors
                throw new AggregateException(exceptions);
            }

            return Task.CompletedTask;
        }
    }
}
