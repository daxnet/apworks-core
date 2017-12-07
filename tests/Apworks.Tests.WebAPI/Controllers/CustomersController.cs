using Apworks.Commands;
using Apworks.Tests.WebAPI.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICommandSender commandSender;
        private readonly ILogger logger;

        public CustomersController(ICommandSender commandSender,
            ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<CustomersController>();
            this.commandSender = commandSender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] dynamic createModel)
        {
            var name = (string)createModel.name;
            await this.commandSender.PublishAsync(new CreateCustomerCommand(name));
            return Ok();
        }
    }
}
