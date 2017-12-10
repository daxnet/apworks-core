using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Commands
{
    /// <summary>
    /// Represents that the implemented classes are command handlers.
    /// </summary>
    /// <seealso cref="Apworks.Messaging.IMessageHandler" />
    public interface ICommandHandler : IMessageHandler
    { }

    /// <summary>
    /// Represents that the implemented classes are command handlers.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <seealso cref="Apworks.Messaging.IMessageHandler{TCommand}" />
    public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand>, ICommandHandler
        where TCommand : ICommand
    {
    }
}
