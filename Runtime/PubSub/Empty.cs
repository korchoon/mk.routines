// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Reactors.DataFlow
{
    public static class Empty
    {
        public static Exception Exception { get; } = EmptyException.Empty;
        public static IDisposable Disposable { get; } = DisposableNever.Ever;

        public static Action Action() => ActionEmpty.Empty;
        public static Action<T> Action<T>() => ActionEmpty<T>.Empty;
        public static Func<T> Func<T>() => FuncEmpty<T>.Empty;
        public static Func<T, bool> FuncPredicate<T>() => FuncEmpty<T>.Empty_bool;
        public static Func<bool> FuncPredicate() => FuncEmpty.Empty;
        public static void Func<T>(out Func<T> res, T val = default) => res = () => val;

        public static ISub Sub() => SubNever.Ever;
        public static ISub<T> Sub<T>() => SubNever<T>.Ever;
        public static void Sub<T>(out ISub<T> res) => res = SubNever<T>.Ever;

        public static IPub Pub() => PubNever.Never;
        public static IPub<T> Pub<T>() => PubNever<T>.Never;
    }
}