using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public interface ILinkItem : IHalElement
    {
        string Name { get; set; }

        string Href { get; set; }

        bool? Templated { get; set; }

        IEnumerable<KeyValuePair<string, object>> Properties { get; }
    }
}
