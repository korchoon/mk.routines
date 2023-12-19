// ----------------------------------------------------------------------------
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Mk.Routines {
    public interface IRoutine : IDisposable{
        bool IsCompleted { get; }
        void Tick ();
        void UpdateParent ();
    }

    public interface IRoutine<out T> : IRoutine {
        T GetResult ();
    }
}