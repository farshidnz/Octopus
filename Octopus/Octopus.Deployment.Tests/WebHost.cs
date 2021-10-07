using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Octopus.Deployment.Tests
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class WebHost<TStartup> : IDisposable where TStartup : class
    {
        private readonly IHost _host;
        private readonly TestServer _server;

        public WebHost(Action<IServiceCollection>? testServices = default, Action<IConfigurationBuilder>? configure = default)
        {
            var hostBuilder = Host
                .CreateDefaultBuilder()
                .ConfigureLogging(builder => builder.AddConsole().AddDebug())
                .ConfigureAppConfiguration(builder => configure?.Invoke(builder))
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .ConfigureTestServices(services =>{ })
                        .UseEnvironment("Integration")
                        .UseTestServer()
                        .UseStartup<TStartup>();
                });

            _host = hostBuilder.Start();

            _server = _host.GetTestServer();

            Client = _host.GetTestClient();

            ServiceProvider = _server.Services;
        }
        public HttpClient Client { get; }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            _host.Dispose();
            _server.Dispose();
        }
    }
}
