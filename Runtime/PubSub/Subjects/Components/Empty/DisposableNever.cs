using System;

namespace Lib.DataFlow
{
    internal class DisposableNever : IDisposable
    {
        public static DisposableNever Ever { get; } = new DisposableNever();
        public void Dispose()
        {
        }
    }
}