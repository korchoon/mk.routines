using System;
using System.Runtime.CompilerServices;

namespace Lib.DataFlow
{
    public static class Empty
    {
        public static Exception Exception { get; } = EmptyException.Empty;
        public static IDisposable Disposable { get; } = DisposableNever.Ever;

        public static Action Action() => ActionEmpty.Empty;
        public static Action<T> Action<T>() => ActionEmpty<T>.Empty;
        public static Func<T> Func<T>(T val = default) => () => val;
        public static void Func<T>(out Func<T> res, T val = default) => res = () => val;

        public static IScope Scope() => ScopeNever.Ever;

        public static ISub Sub() => SubNever.Ever;
        public static ISub<T> Sub<T>() => SubNever<T>.Ever;
        public static void Sub<T>(out ISub<T> res) => res = SubNever<T>.Ever;

        public static IPub Pub() => PubNever.Ever;
        public static IPub<T> Pub<T>() => PubNever<T>.Ever;
    }
}