// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Reactors.DataFlow
{
     interface IErrorScope<out T>
    {
        // todo rename to Subscribe
        void OnDispose(Action<T> onMsg);
    }
}