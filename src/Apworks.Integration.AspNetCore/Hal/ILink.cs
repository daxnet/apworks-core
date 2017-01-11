using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public interface ILink : IHalElement
    {
        string Rel { get; set; }

        LinkItemCollection Items { get; set; }
    }
}
