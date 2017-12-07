using Apworks.Commands;
using Apworks.Tests.WebAPI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Apworks.Tests.WebAPI.CommandHandlers
{
    public class CreateCustomerCommandHandler : CommandHandler<CreateCustomerCommand>
    {
        private readonly ILogger logger;

        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger)
        {
            this.logger = logger;
        }

        public override Task<bool> HandleAsync(CreateCustomerCommand message, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.logger.LogInformation($"{message.GetType().FullName} message handled at {message.Timestamp}. name: {message.Name}.");
            return Task.FromResult(true);
        }
    }
}
