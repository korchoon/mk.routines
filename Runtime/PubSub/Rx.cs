// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

#define M_DISABLED
using Lib.Attributes;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib
{
    [NonPerformant(PerfKind.GC)]
    public static class Rx
    {
#if M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool)> Of(IScope scope, ISub s1, ISub s2)
        {
            var (pub, sub) = scope.PubSub<(bool, bool)>();
            var res = sub;

            s1.OnNext(() => pub.Next((true, false)), scope);
            s2.OnNext(() => pub.Next((false, true)), scope);

            return res;
        }
#endif

#if M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, Option<T>)> Of<T>(IScope scope, ISub s1, ISub<T> s2)
        {
            var (pub, sub) = scope.PubSub<(bool, Option<T>)>();

            s1.OnNext(() => pub.Next((true, default)), scope);
            s2.OnNext(msg => pub.Next((false, msg)), scope);

            return sub;
        }
#endif

        [NonPerformant(PerfKind.GC)]
        public static ISub<(Option<T1>, Option<T2>)> Of<T1, T2>(this IScope scope, ISub<T1> s1, ISub<T2> s2)
        {
            var (pub, sub) = scope.PubSub<(Option<T1>, Option<T2>)>();

            s1.OnNext(msg => pub.Next((msg, default)), scope);
            s2.OnNext(msg => pub.Next((default, msg)), scope);

            return sub;
        }

#if M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool, bool)> Of(IScope scope, ISub s1, ISub s2, ISub s3)
        {
            var (pub, sub) = scope.PubSub<(bool, bool, bool)>();

            s1.OnNext(() => pub.Next((true, default, default)), scope);
            s2.OnNext(() => { pub.Next((default, true, default)); }, scope);
            s3.OnNext(() => { pub.Next((false, default, true)); },scope);

            return sub;
        }
#endif

#if M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool, Option<T3>)> Of<T3>(IScope scope, ISub s1, ISub s2, ISub<T3> s3)
        {
            var (pub, sub) = scope.PubSub<(bool, bool, Option<T3>)>();

            s1.OnNext(() => { pub.Next((true, default, default)); },scope);
            s2.OnNext(() => { pub.Next((default, true, default)); },scope);
            s3.OnNext(msg => { pub.Next((default, default, msg)); },scope);

            return sub;
        }
#endif


#if M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, Option<T2>, Option<T3>)> Of<T2, T3>(this IScope scope, ISub s1, ISub<T2> s2, ISub<T3> s3)
        {
            var (pub, sub) = scope.PubSub<(bool, Option<T2>, Option<T3>)>();

            s1.OnNext(() => pub.Next((true, default, default)), scope);
            s2.OnNext(msg => pub.Next((default, msg, default)), scope);
            s3.OnNext(msg => pub.Next((default, default, msg)), scope);

            return sub;
        }
#endif

#if M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool, bool, bool)> Of(IScope scope, ISub s1, ISub s2, ISub s3, ISub s4)
        {
            var (pub, sub) = scope.Channel<(bool, bool, bool, bool)>();

            s1.OnNext(() => pub.Next((true, default, default, default)),scope);
            s2.OnNext(() => pub.Next((default, true, default, default)),scope);
            s3.OnNext(() => pub.Next((default, default, true, default)),scope);
            s4.OnNext(() => pub.Next((default, default, default, true)),scope);

            return sub;
        }
#endif
    }
}