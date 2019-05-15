using System;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib
{
    public static class React
    {
        [MustUseReturnValue]
        // single-next, single-onnext 
        internal static (IPub<T> pub, ISub<T> sub) PubSub11<T>(this IScope scope)
        {
            var subject = new Pub1Sub1<T>(scope);
            return (subject, subject);
        }

        [MustUseReturnValue]
        // single-next, single-onnext 
        internal static (IPub pub, ISub sub) PubSub11(this IScope scope)
        {
            var subject = new Pub1Sub1(scope);
            return (subject, subject);
        }

        [MustUseReturnValue]
        public static (IPub pub, ISub sub) Channel(this IScope scope)
        {
            var subject = new Subject(scope);
            return (subject, subject);
        }

        [MustUseReturnValue]
        public static (IPub<T> pub, ISub<T> sub) Channel<T>(this IScope scope)
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