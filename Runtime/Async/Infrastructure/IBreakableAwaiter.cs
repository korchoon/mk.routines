using System;

namespace Lib.Async
{
    public interface IBreakableAwaiter
    {
        void Break(Exception e);
    }
}