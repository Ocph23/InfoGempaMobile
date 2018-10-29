using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hubs;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
          

        public IActionResult Index()
        {
            return View();
        }
    }
}