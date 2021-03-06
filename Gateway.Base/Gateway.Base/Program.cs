﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using BeetleX;
using Bumblebee;
using System.Diagnostics;

namespace HttpGateway.Base
{
    class Program
    {
        private static Gateway g;

        static void Main(string[] args)
        {

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<HttpServerHosted>();

                });
            builder.Build().Run();
        }
    }

    public class HttpServerHosted : IHostedService
    {
        private Gateway g;

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            g = new Gateway();
            g.GatewayQueueSize = 200;
            g.AgentRequestQueueSize = 200;
            g.Open();
            g.LoadPlugin(typeof(Program).Assembly);
            g.Agents.SetServer("http://localhost:8080", 300);
            g.Agents.SetServer("http://localhost:8081", 300);
            g.Agents.SetServer("http://localhost:8082", 300);
            g.Routes.GetRoute("*").AddServer("http://localhost:8080", "http://localhost:8081", "http://localhost:8082");
            return Task.CompletedTask;
        }
        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            g.Dispose();
            return Task.CompletedTask;
        }
    }
}
