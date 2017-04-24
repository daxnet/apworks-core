using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <seealso cref="Apworks.Messaging.IMessageHandler{TCommand}" />
    public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand>
        where TCommand : ICommand
    {
    }
}
