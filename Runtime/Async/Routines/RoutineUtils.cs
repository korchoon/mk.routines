using System;
using Lib.DataFlow;

namespace Lib.Async
{
    public static class RoutineUtils
    {
        public static void MoveNextAndClear(ref Action moveNextOnce)
        {
            var buf = moveNextOnce;
            moveNextOnce = Empty.Action();
            buf?.Invoke();
        }
    }
}