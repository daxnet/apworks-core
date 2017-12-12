using Apworks.Commands;
using Apworks.Tests.WebAPI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI.CommandHandlers
{
    public class PingCommandHandler : CommandHandler<PingCommand>
    {
        public override Task<bool> HandleAsync(PingCommand message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}
