using Apworks.Commands;
using Apworks.Tests.WebAPI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Apworks.Repositories;

namespace Apworks.Tests.WebAPI.CommandHandlers
{
    public class CreateCustomerCommandHandler : CommandHandler<CreateCustomerCommand>
    {
        private readonly ILogger logger;
        private readonly IDomainRepository domainRepository;

        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger, IDomainRepository domainRepository)
        {
            this.logger = logger;
            this.domainRepository = domainRepository;

            this.logger.LogInformation($"CreateCustomerCommandHandler created. Hash code: {this.GetHashCode()}.");
            this.logger.LogInformation($"Domain Repository has been injected into CreateCustomerCommandHandler. Hash Code: {this.domainRepository.GetHashCode()}.");
        }

        public override Task<bool> HandleAsync(CreateCustomerCommand message, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.logger.LogInformation($"CreateCustomerCommandHandler invoked. Hash code: {this.GetHashCode()}.");
            this.logger.LogInformation($"Domain Repository invoked. Hash code: {this.domainRepository.GetHashCode()}.");
            return Task.FromResult(true);
        }
    }
}
