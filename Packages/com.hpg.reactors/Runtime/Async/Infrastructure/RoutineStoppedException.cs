using System;

namespace Lib.Async
{
    public class RoutineStoppedException : Exception
    {
        RoutineStoppedException()
        {
        }

        public static RoutineStoppedException Empty1 { get; } = new RoutineStoppedException();
    }
}