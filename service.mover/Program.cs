using System;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using service.mover.core;
using service.mover.core.models;
using service.mover.Factories;
using service.mover.Helpers;

namespace service.mover
{
    class Program
    {
        private static readonly Configuration configuration = new Configuration();
        static void Main(string[] args)
        {
            try
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();
                BuildLogger();
                
                Worker worker = serviceProvider.GetService<Worker>();
                while(true)
                {
                    worker.DoWork();
                    Environment.Exit(1);
                    // Thread.Sleep(TimeSpan.FromMinutes(configuration.Sleep));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        private static void BuildLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        }

        private static void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Worker>();
            serviceCollection.AddSingleton<Configuration>();
            serviceCollection.AddTransient<StorageFactory>();
            serviceCollection.AddTransient<Helper>();

        }
    }
}
