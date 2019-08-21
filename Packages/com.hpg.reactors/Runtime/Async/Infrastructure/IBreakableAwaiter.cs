// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Lib.Async
{
    public interface IBreakableAwaiter
    {
        void BreakInner();
        void Unsub();
    }
}