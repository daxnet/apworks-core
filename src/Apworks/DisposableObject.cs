using Apworks.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks
{
    /// <summary>
    /// Represents the base class of the objects that can be disposed.
    /// </summary>
    /// <remarks>
    /// This class provides the basic implementation of the disposable pattern in .NET.
    /// </remarks>
    public abstract class DisposableObject : IDisposable
    {
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
