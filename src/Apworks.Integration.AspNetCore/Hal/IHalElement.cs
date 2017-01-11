using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    public interface IHalElement
    {
        string ToJson();
        string ToXml();

    }
}
