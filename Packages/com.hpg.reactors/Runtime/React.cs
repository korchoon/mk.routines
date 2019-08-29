// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Lib.DataFlow;
using Utility.Asserts;

namespace Lib
{
    public static class React
    {
        [JetBrains.Annotations.MustUseReturnValue]
        public static (IPub pub, ISub sub) PubSub(this IScope scope)
        {
            Asr.IsFalse(scope.Disposing);
            var subject = new Subject(scope);
            return (subject, subject);
        }

        [MustUseReturnValue]
        public static (IPub<T> pub, ISub<T> sub) PubSub<T>(this IScope scope)
        {
            var subject = new Subject<T>(scope);
            return (subject, subject);
        }

        [MustUseReturnValue]
        public static IDisposable Scope(out IScope scope)
        {
            var subject = new ScopeStack();
            scope = subject;
            return subject;
        }
    }
}