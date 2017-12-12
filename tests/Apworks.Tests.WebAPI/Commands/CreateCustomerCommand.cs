using Apworks.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI.Commands
{
    public class CreateCustomerCommand : Command
    {
        public CreateCustomerCommand(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
