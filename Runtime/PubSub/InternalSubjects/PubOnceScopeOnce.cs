// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.Async;
using Utility.Asserts;

namespace Lib.DataFlow
{
    public static class UtilsOnce
    {
        public static void OnNextOnce<T>(this ISub<T> sub, Action<T> action)
        {
#if M_OPT
            var pubScope = new PubOnceScopeOnce<T>(action);
            // todo cache delegate
            sub.OnNext(pubScope.Next, pubScope);

#else
            var dispose = React.Scope(out var scope);
            sub.OnNext(msg =>
            {
                dispose.Dispose();
                action.Invoke(msg);
            }, scope);
#endif
        }

        public static void OnNextOnce(this ISub sub, Action action)
        {
#if M_OPT
            var pubScope = new PubOnceScopeOnce(action);
            // todo cache delegate
            sub.OnNext(pubScope.Next, pubScope);

#else
            var dispose = React.Scope(out var scope);
            sub.OnNext(() =>
            {
                dispose.Dispose();
                action.Invoke();
            }, scope);
#endif
        }
    }
}