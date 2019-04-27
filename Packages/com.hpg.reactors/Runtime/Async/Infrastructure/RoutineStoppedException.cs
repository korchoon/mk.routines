using System;

namespace Lib.Async
{
    public class RoutineStoppedException : Exception
    {
        RoutineStoppedException()
        {
        }

        public static RoutineStoppedException Empty { get; } = new RoutineStoppedException();
    }
}