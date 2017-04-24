using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apworks.Messaging;

namespace Apworks.Commands
{
    public abstract class Command : Message, ICommand
    {
    }
}
