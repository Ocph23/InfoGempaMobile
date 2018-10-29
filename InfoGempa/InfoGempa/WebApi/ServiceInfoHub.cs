using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Hubs;

namespace WebApi
{
    public class ServiceInfoHub
    {
        private readonly IHubContext<InfoHub> _hubContext;

        public ServiceInfoHub(IHubContext<InfoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task FooMethod(string message)
        {
            await _hubContext.Clients.All.SendMessage("Server", message);
        }
    }
}
