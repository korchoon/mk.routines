using System;

namespace Lib.Async
{
    public class MethodScope : IDisposable
    {
        public bool Completed { get; private set; }

        void IDisposable.Dispose() => Completed = true;
    }
}