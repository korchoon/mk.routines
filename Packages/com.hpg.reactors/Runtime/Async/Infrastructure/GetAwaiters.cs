using System;
using System.Threading;
using System.Threading.Tasks;
using Lib.DataFlow;
using Lib.Timers;
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
            routine.Scope(scope).OnDispose(cts.Dispose);
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
            scope.OnDispose(() => routine.BreakInnerFromOuter.Pub.Next());
            routine.Scope.Sub.OnDispose(cts.Dispose);
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
            var d = React.Scope(out var scope);
            return new GenericAwaiter2<T>(s, scope, d);
        }

        public static GenericAwaiter2 GetAwaiter(this ISub aw)
        {
            var d = React.Scope(out var scope);
            aw.OnNext(d.Dispose, scope);
            return new GenericAwaiter2(aw, scope, d);
        }

        public static GenericAwaiter2 GetAwaiter(this IScope aw)
        {
            var d = React.Scope(out var scope);
            aw.OnDispose(d.Dispose);
            return new GenericAwaiter2(aw, scope, d);
        }

        public static GenericAwaiter2 GetAwaiter(this float sec) => Delay(sec, DefaultSch).GetAwaiter();

        public static GenericAwaiter2 GetAwaiter(this int sec) => GetAwaiter((float) sec);
        public static GenericAwaiter2 GetAwaiter(this double sec) => GetAwaiter((float) sec);


        public static IScope Delay(float seconds, ISub s)
        {
            var pub = React.Scope(out var res);
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