using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Hubs;

namespace WebApi
{
    public class Helper
    {
       public static IHubContext GetContext()
        {
            return Startup.ConnectionManager.GetHubContext("infoHub");
        }

    }
}
