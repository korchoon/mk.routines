using Lib.DataFlow;
using Lib.Timers;
using UnityEngine;

namespace Lib.Async
{
    public static class GetAwaiters
    {
        static ISub DefaultSch => Sch.Update;

        public static SingleAwaiter<T> GetAwaiter<T>(this ISub<T> s) => new SingleAwaiter<T>(s);
        public static SubAwaiter GetAwaiter(this ISub aw)
        {
            var res = new SubAwaiter();
            aw.OnNext(res.OneOff);
            return res;
        }

        public static SubAwaiter GetAwaiter(this IScope aw)
        {
            var res = new SubAwaiter();
            aw.OnDispose(() => res.Dispose());
            return res;
        }

        public static Routine.Awaiter GetAwaiter(this float sec) => Delay(sec).GetAwaiter();
        public static Routine.Awaiter GetAwaiter(this int sec) => GetAwaiter((float) sec);
        public static Routine.Awaiter GetAwaiter(this double sec) => GetAwaiter((float) sec);

        public static async Routine Delay(float seconds, ISub s = null)
        {
            s = s ?? DefaultSch;
            var delay = new TimeToken(seconds, Time.time);
            while (delay.KeepWaiting(Time.time))
                await s;
        }
    }
}