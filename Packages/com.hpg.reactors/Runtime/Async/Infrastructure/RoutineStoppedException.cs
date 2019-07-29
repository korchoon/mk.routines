// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

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