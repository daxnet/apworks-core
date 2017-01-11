using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public interface IResource : IHalElement
    {
        object State { get; set; }

        LinkCollection Links { get; set; }

        ResourceCollection Resources { get; set; }
    }
}
