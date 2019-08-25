// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Lib.DataFlow;
using Lib.Timers;
using Lib.Utility;
using UnityEngine;

namespace Lib.Async
{
    public static class GetAwaiters
    {
        static ISub DefaultSch => Sch.Update;

        public static Routine Convert(Func<CancellationToken, Task> factory, IScope scope)
        {
            var cts = new CancellationTokenSource();

            var tt = factory.Invoke(cts.Token);
            var routine = _Inner(tt);
            routine.GetScope(scope).Subscribe(cts.Dispose);
            return routine;

            async Routine _Inner(Task t)
            {
                var aw = t.ConfigureAwait(true).GetAwaiter();
                while (!aw.IsCompleted)
                    await Sch.Update;

                aw.GetResult();
            }
        }

         public static Routine<T> Convert<T>(Func<CancellationToken, Task<T>> factory, IScope scope)
        {
            var cts = new CancellationTokenSource();

            var tt = factory.Invoke(cts.Token);
            var routine = _Inner(tt);
            scope.Subscribe(() => routine.Dispose());
            routine.Scope.Subscribe(cts.Dispose);
            return routine;

            async Routine<T> _Inner(Task<T> t)
            {
                var aw = t.ConfigureAwait(true).GetAwaiter();
                while (!aw.IsCompleted)
                    await Sch.Update;

                return aw.GetResult();
            }
        }

        public static GenericAwaiter2<T> GetAwaiter<T>(this ISub<T> s)
        {
            var result = new Option<T>();
            var d = Sch.Scope.Scope(out var scope);
            bool done = false;
            s.OnNext(Maybe, scope);
            var res1 = new GenericAwaiter2(scope, () => done = true);
            var res = new GenericAwaiter2<T>(res1, () => result.GetOrFail());
            return res;

            void Maybe(T msg)
            {
                result = msg;
                if (done) return;
                d.Dispose();
            }
        }

        public static GenericAwaiter2 GetAwaiter(this ISub aw)
        {
            var d = Sch.Scope.Scope(out var scope);

            bool done = false;
            aw.OnNext(Maybe, scope);
            var res = new GenericAwaiter2(scope, () => done = true);
            return res;

            void Maybe()
            {
                if (done) return;
                d.Dispose();
            }
        }

        public static GenericAwaiter2 GetAwaiter(this IScope outer)
        {
            var d = Sch.Scope.Scope(out var scope);
            var (pub, sub) = outer.PubSub();
            outer.Subscribe(() =>
            {
                if (scope.Completed)
                    return;
                pub.Next();
                d.Dispose();
            });

            sub.OnNext(d.Dispose, outer);
            return new GenericAwaiter2(scope, d.Dispose);
        }

        public static GenericAwaiter2 GetAwaiter(this float sec) => Delay(sec, DefaultSch).GetAwaiter();

        public static GenericAwaiter2 GetAwaiter(this int sec) => GetAwaiter((float) sec);
        public static GenericAwaiter2 GetAwaiter(this double sec) => GetAwaiter((float) sec);


        public static IScope Delay(float seconds, ISub s)
        {
            var pub = Sch.Scope.Scope(out var res);
            var delay = new TimeToken(seconds, Time.time);
            s.OnNext(() =>
            {
                var wait = delay.KeepWaiting(Time.time);
                if (!wait)
                    pub.Dispose();
            }, res);

            return res;
        }

        public static async Routine DelayEditor(float seconds, ISub s)
        {
            var delay = new TimeToken(seconds, Time.time);
#if UNITY_EDITOR
            while (delay.KeepWaiting((float) UnityEditor.EditorApplication.timeSinceStartup))
                await s;
#endif
        }
    }
}