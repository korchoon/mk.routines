using System;
using Lib.DataFlow;

namespace Lib.Async
{
    internal static class Utils
    {
        public static void MoveNextAndClear(ref Action moveNextOnce)
        {
            var buf = moveNextOnce;
            moveNextOnce = Empty.Action();
            buf?.Invoke();
        }
    }
}