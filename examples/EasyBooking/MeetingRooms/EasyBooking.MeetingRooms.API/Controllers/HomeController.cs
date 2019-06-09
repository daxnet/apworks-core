using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBooking.MeetingRooms.API.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
