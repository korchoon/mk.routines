// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Lib.DataFlow
{
    public interface IScope
    {
        bool Completed { get; }
        void OnDispose(Action dispose);
        void Unsubscribe(Action dispose);
    }
}