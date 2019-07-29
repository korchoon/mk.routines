// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Lib.DataFlow
{
    public interface ISub
    {
        void OnNext(Action pub, IScope scope);
    }

    public interface ISub<out T>
    {
        void OnNext(Action<T> pub, IScope scope);
    }
}