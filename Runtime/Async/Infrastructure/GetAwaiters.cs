using Lib.DataFlow;
using Lib.Timers;
using UnityEngine;

namespace Lib.Async
{
    public static class GetAwaiters
    {
        static ISub DefaultSch => Sch.Update;

        public static SubAwaiter<T> GetAwaiter<T>(this ISub<T> s) => new SubAwaiter<T>(s);

        public static SubAwaiter GetAwaiter(this ISub aw)
        {
            var res = new SubAwaiter();
            aw.OnNext(res.OneOff);
            return res;
        }

        public static ScopeAwaiter GetAwaiter(this IScope aw) => new ScopeAwaiter(aw);

        public static ScopeAwaiter GetAwaiter(this float sec) => Delay(sec).GetAwaiter();
        public static ScopeAwaiter GetAwaiter(this int sec) => GetAwaiter((float) sec);
        public static ScopeAwaiter GetAwaiter(this double sec) => GetAwaiter((float) sec);


        public static IScope Delay(float seconds, ISub s = null)
        {
            s = s ?? DefaultSch;
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

        public static async Routine Delay1(float seconds, ISub s = null)
        {
            s = s ?? DefaultSch;
            var delay = new TimeToken(seconds, Time.time);
            while (delay.KeepWaiting(Time.time))
                await s;
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