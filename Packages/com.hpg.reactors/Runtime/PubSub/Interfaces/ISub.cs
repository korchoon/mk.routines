﻿// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Reactors.DataFlow
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