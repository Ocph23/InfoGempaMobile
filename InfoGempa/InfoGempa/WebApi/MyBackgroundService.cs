using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Hubs;

namespace WebApi
{
    public class MyBackgroundService : IHostedService, IDisposable
    {
        public  IHubContext<InfoHub> HubContext { get; set; }

        public MyBackgroundService(IHubContext<InfoHub> hubContext)
        {
            HubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //TODO: your start logic, some timers, singletons, etc
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //TODO: your stop logic
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
