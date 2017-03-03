using Hal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public interface IHalBuildConfiguration
    {
        void RegisterHalBuilderFactory(ControllerActionSignature signature, Func<HalBuildContext, IBuilder> halBuilderFactory);

        Func<HalBuildContext, IBuilder> RetrieveHalBuilderFactory(ControllerActionSignature signature);
    }
}
