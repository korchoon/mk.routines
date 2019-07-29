// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using Lib.DataFlow;

namespace Lib.Async
{
    internal static class RoutineUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MoveNextAndClear<T>(ref Action<T> moveNextOnce, T msg)
        {
            if (moveNextOnce == null)
            {
                moveNextOnce = Empty.Action<T>();
                return;
            }

            var buf = moveNextOnce;
            moveNextOnce = Empty.Action<T>();
            buf.Invoke(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MoveNextAndClear(ref Action moveNextOnce)
        {
            if (moveNextOnce == null)
            {
                moveNextOnce = Empty.Action();
                return;
            }

            var buf = moveNextOnce;
            moveNextOnce = Empty.Action();
            buf.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WasTrue(ref this bool flag)
        {
            var copy = flag;
            flag = true;
            return copy;
        }
    }
}