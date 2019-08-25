// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Reactors.DataFlow
{
    public interface IScope
    {
        bool Disposing { get; }
        bool Completed { get; }
        void Subscribe(Action dispose);
        void Unsubscribe(Action dispose);
    }
}