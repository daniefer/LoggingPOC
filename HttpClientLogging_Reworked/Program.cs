using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpClientLogging_Reworked
{
    class Program
    {
        private static string BasePath = Path.GetFullPath(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "..\\..\\..\\"));

        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureHostConfiguration(config => config
                    .SetBasePath(BasePath)
                    .AddEnvironmentVariables()
                    .AddUserSecrets("testlogging")
                    .AddJsonFile(Path.Combine(BasePath, "appsettings.json"), false, true)
                    .AddCommandLine(args)
                )
                .ConfigureLogging((context, loggingBuiler) =>
                {
                    loggingBuiler.AddConsole();
                    loggingBuiler.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddTransient<RequestHeaderLoggingHandler>();
                    services.AddTransient<RequestBodyLoggingHandler>();
                    services.AddTransient<ResponseHeaderLoggingHandler>();
                    services.AddTransient<ResponseBodyLoggingHandler>();
                    services.AddHttpClient("testClient")
                            .AddHttpMessageHandler<RequestHeaderLoggingHandler>()
                            .AddHttpMessageHandler<RequestBodyLoggingHandler>()
                            .AddHttpMessageHandler<ResponseHeaderLoggingHandler>()
                            .AddHttpMessageHandler<ResponseBodyLoggingHandler>();
                    services.AddHostedService<HttpClientWorker>();
                });
    }
}
