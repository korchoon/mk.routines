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
            routine._dispose.DisposeOn(scope);
            routine.Scope.OnDispose(cts.Dispose);
            return routine;

            async Routine<T> _Inner(Task<T> t)
            {
                var aw = t.ConfigureAwait(true).GetAwaiter();
                while (!aw.IsCompleted)
                    await Sch.Update;

                return aw.GetResult();
            }
        }

        public static SubAwaiter<T> GetAwaiter<T>(this ISub<T> s) => new SubAwaiter<T>(s);

        public static SubAwaiter GetAwaiter(this ISub aw)
        {
            var res = SubAwaiter.New();
            aw.OnNext(res.OneOff);
            return res;
        }

        public static ScopeAwaiter GetAwaiter(this IScope aw) => new ScopeAwaiter(aw);

        public static ScopeAwaiter GetAwaiter(this float sec) => Delay(sec, DefaultSch).GetAwaiter();
        
        public static ScopeAwaiter GetAwaiter(this int sec) => GetAwaiter((float) sec);
        public static ScopeAwaiter GetAwaiter(this double sec) => GetAwaiter((float) sec);


        public static IScope Delay(float seconds, ISub s)
        {
            var pub = React.Scope(out var res);
            var delay = new TimeToken(seconds, Time.time);
            s.OnNext(() =>
            {
                var wait = delay.KeepWaiting(Time.time);
                if (!wait)
                    pub.Dispose();
                return wait;
            });

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