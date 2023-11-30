// ----------------------------------------------------------------------------
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Mk.Routines {
    public interface IRoutine {
        bool IsCompleted { get; }
        void Update ();
        void Break ();
        void UpdateParent ();
    }

    public interface IRoutine<out T> : IRoutine {
        T GetResult ();
    }
}