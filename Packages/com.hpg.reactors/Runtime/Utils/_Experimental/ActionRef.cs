// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------
using System;
using UnityEngine.Events;

namespace Lib
{
    // overcome ref in delegates restriction
    public delegate ref UnityAction UnityActionRef();
    public delegate ref Action ActionRef();

    // overcome ref in delegates restriction
    public delegate ref Action<T> ActionRef<T>();
}