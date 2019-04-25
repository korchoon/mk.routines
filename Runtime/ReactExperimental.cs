using System;
using JetBrains.Annotations;
using Lib.Async;
using Lib.DataFlow;
using Lib.Timers;
using UnityEngine.Events;

namespace Lib
{
    public static class ReactExperimental
    {
        public static void DelayedAction(this float sec, Action continuation, IScope scope)
        {
            var scopeAwaiter = sec.GetAwaiter();
            scopeAwaiter.UnsafeOnCompleted(continuation);
        }

        public static Func<ISub<T>> ToSub<T>(this Func<Routine<T>> callback, IScope scope)
        {
            return DynamicMethod;

            ISub<T> DynamicMethod()
            {
                var routine = callback.Invoke();
                var (p, s) = React.Channel<T>(scope);

                var aw = routine.GetAwaiter();
                routine.Scope.OnDispose(() =>
                {
                    var r = aw.GetResult();
                    p.Next(r);
                });
                return s;
            }
        }

        [MustUseReturnValue]
        public static (IDisposable disposable, IScope scope) ScopeTuple()
        {
            var res = new ScopeStack();
            return (res, res);
        }

        public static IPub DelegateToPub(this Action t, IScope sd)
        {
            var (pub1, sub1) = React.Channel(sd);
            sub1.OnNext(t.Invoke, sd);
            return pub1;
        }

        public static IPub<T> DelegateToPub<T>(this Action<T> t, IScope sd)
        {
            var (pub1, sub1) = React.Channel<T>(sd);
            sub1.OnNext(t.Invoke, sd);
            return pub1;
        }

        public static IPub Wrap(this IPub pub, Action<IPub> proxy, IScope sd)
        {
            var (pub1, sub1) = React.Channel(sd);
            sub1.OnNext(Pub, sd);
            return pub1;

            void Pub() => proxy.Invoke(pub);
        }

        public static IPub<T> Wrap<T>(this IPub<T> pub, Action<T, IPub<T>> proxy, IScope sd)
        {
            var (pub1, sub1) = React.Channel<T>(sd);
            sub1.OnNext(Pub, sd);
            return pub1;

            void Pub(T msg) => proxy.Invoke(msg, pub);
        }

        [MustUseReturnValue]
        public static IDisposeWith<Exception> ErrScope(out IScope<Exception> scope)
        {
            return new CatchStack(out scope);
        }

        public static ISub<T> ToSub<T>(Func<IPub<T>, Routine> ctor, IScope scope)
        {
            IPub<T> pub;
            ISub<T> sub;
            (pub, sub) = React.Channel<T>(scope);
            var res = sub;
            ctor.Invoke(pub).DisposeOn(scope);
            return res;
        }

        public static ISub ToSub(this Routine r, IScope scope)
        {
            var (pub, sub) = React.Channel(scope);

            // will implicitly dispose r
            Local().DisposeOn(scope);
            return sub;

            async Routine Local()
            {
                try
                {
                    await r;
                }
                finally
                {
                    pub.Next();
                }
            }
        }
    }
}