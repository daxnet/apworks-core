using Apworks.Integration.AspNetCore.Hal;
using Hal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    internal sealed class DataServiceHalBuildConfiguration : HalBuildConfiguration
    {
        public DataServiceHalBuildConfiguration()
        {
            this.RegisterHalBuilderFactory("*.Get", context =>
                {
                    dynamic state = context.State;
                    var pageSize = (int)state.PageSize;
                    var pageNumber = (int)state.PageNumber;
                    return new ResourceBuilder()
                        .WithState(new { pageSize, pageNumber })
                        .AddEmbedded(context.ControllerAction.ControllerName.ToLower())
                        .Resource(new ResourceBuilder().WithState((object)state));
                }
            );
        }
    }
}
