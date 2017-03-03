using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public sealed class HalBuildContext
    {

        public HalBuildContext(object state, ControllerActionDescriptor controllerAction)
        {
            this.State = state;
            this.ControllerAction = controllerAction;
        }

        public object State { get; }

        public ControllerActionDescriptor ControllerAction { get; }
    }
}
