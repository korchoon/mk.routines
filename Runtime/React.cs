using System;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib
{
    public static class React
    {
        public static (IPub pub, ISub sub) Channel(IScope scope)
        {
            var subject = new Subject(scope);
            return (subject, subject);
        }

        public static (IPub<T> pub, ISub<T> sub) Channel<T>(IScope scope)
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
        
        [MustUseReturnValue]
        public static IDisposable Scope( out IScope scope, IScope outer)
        {
            var subject = new ScopeStack();
            outer.OnDispose(subject.Dispose);
            scope = subject;
            return subject;
        }
    }
}