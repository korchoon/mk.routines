// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib
{
    public static class React
    {
        [MustUseReturnValue]
        public static (IPub pub, ISub sub) PubSub(this IScope scope)
        {
            var subject = new Subject(scope);
            return (subject, subject);
        }

        [MustUseReturnValue]
        public static (IPub<T> pub, ISub<T> sub) PubSub<T>(this IScope scope)
        {
            var subject = new Subject<T>(scope);
            return (subject, subject);
        }

        [MustUseReturnValue, Obsolete("Use PubSub instead")]
        public static (IPub pub, ISub sub) Channel(this IScope scope) => PubSub(scope);

        [MustUseReturnValue, Obsolete("Use PubSub instead")]
        public static (IPub<T> pub, ISub<T> sub) Channel<T>(this IScope scope) => PubSub<T>(scope);

        [MustUseReturnValue]
        public static IDisposable Scope(out IScope scope)
        {
            var subject = new ScopeStack();
            scope = subject;
            return subject;
        }
    }
}