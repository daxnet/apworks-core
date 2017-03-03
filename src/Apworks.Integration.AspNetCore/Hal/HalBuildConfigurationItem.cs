using Hal.Builders;
using System;

namespace Apworks.Integration.AspNetCore.Hal
{
    public sealed class HalBuildConfigurationItem
    {
        public HalBuildConfigurationItem(ControllerActionSignature signature, Func<HalBuildContext, IBuilder> halBuilderFactory)
        {
            this.Signature = signature;
            this.HalBuilderFactory = halBuilderFactory;
        }

        public ControllerActionSignature Signature { get; }

        public Func<HalBuildContext, IBuilder> HalBuilderFactory { get; }
    }
}
