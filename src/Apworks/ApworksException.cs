using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks
{
    public class ApworksException : Exception
    {
        public ApworksException()
            : base()
        { }

        public ApworksException(string message)
            : base(message)
        { }

        public ApworksException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public ApworksException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
