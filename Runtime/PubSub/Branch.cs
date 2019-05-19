using System;
using JetBrains.Annotations;
using Lib.Attributes;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib
{
    public static class ScopeApi
    {
        public static ISub<(Option<T1>, Option<T2>, Option<T3>)> BranchOf<T1, T2, T3>(this IScope scope, ISub<T1> s1, ISub<T2> s2, ISub<T3> s3)
        {
            var (pub, sub) = scope.PubSub11<(Option<T1>, Option<T2>, Option<T3>)>();

            s1.OnNext(msg => { pub.Next((msg, default, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg, default)); }, scope);
            s3.OnNext(msg => { pub.Next((default, default, msg)); }, scope);

            return sub;
        }

        public static ISub<(bool, Option<T2>, Option<T3>)> BranchOf<T2, T3>(this IScope scope, ISub s1, ISub<T2> s2, ISub<T3> s3)
        {
            var (pub, sub) = scope.PubSub11<(bool, Option<T2>, Option<T3>)>();

            s1.OnNext(() => { pub.Next((true, default, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg, default)); }, scope);
            s3.OnNext(msg => { pub.Next((default, default, msg)); }, scope);

            return sub;
        }

        public static ISub<(bool, Option<T2>)> BranchOf<T2>(this IScope scope, ISub s1, ISub<T2> s2)
        {
            var (pub, sub) = scope.PubSub11<(bool, Option<T2>)>();

            s1.OnNext(() => { pub.Next((true, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg)); }, scope);

            return sub;
        }


        public static ISub<(Option<T1>, Option<T2>)> BranchOf<T1, T2>(this IScope scope, ISub<T1> s1, ISub<T2> s2)
        {
            var (pub, sub) = scope.PubSub11<(Option<T1>, Option<T2>)>();

            s1.OnNext(msg => { pub.Next((msg, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg)); }, scope);

            return sub;
        }

        public static ISub<(Option<T>, bool, bool)> BranchOf<T>(this IScope scope, ISub<T> s1, ISub s2, ISub s3)
        {
            var (pub, sub) = scope.PubSub11<(Option<T>, bool, bool)>();
            var res = sub;

            s1.OnNext(msg => { pub.Next((msg, false, false)); }, scope);
            s2.OnNext(() => { pub.Next((default, true, false)); }, scope);
            s3.OnNext(() => { pub.Next((default, false, true)); }, scope);

            return res;
        }

        public static ISub<(bool, bool, bool)> BranchOf(this IScope scope, ISub s1, ISub s2, ISub s3)
        {
            var (pub, sub) = scope.PubSub11<(bool, bool, bool)>();
            var res = sub;

            s1.OnNext(() => { pub.Next((true, false, false)); }, scope);
            s2.OnNext(() => { pub.Next((false, true, false)); }, scope);
            s3.OnNext(() => { pub.Next((false, false, true)); }, scope);

            return res;
        }

        public static ISub<(bool, bool)> BranchOf(this IScope scope, ISub s1, ISub s2)
        {
            var (pub, sub) = scope.PubSub11<(bool, bool)>();
            var res = sub;

            s1.OnNext(() => { pub.Next((true, false)); }, scope);
            s2.OnNext(() => { pub.Next((false, true)); }, scope);

            return res;
        }

        [MustUseReturnValue]
        public static IDisposable Scope(this IScope outer, out IScope res)
        {
            var dispose = React.Scope(out res);
            Action action = dispose.Dispose;
            res.OnDispose(_Remove);
            outer.OnDispose(action);
            return dispose;

            void _Remove() => outer.Unsubscribe(action);
        }
    }

    [NonPerformant(PerfKind.GC)]
    public static class BranchEvery
    {
#if M_DISABLED
         [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool)> Of(ISub s1, ISub s2)
        {
            var (pub, sub) = React.Channel<(bool, bool)>(scope);
            var res = sub;

            s1.OnNext(() =>
            {
                pub.Next((true, false));
                return false;
            });
            s2.OnNext(() =>
            {
                pub.Next((false, true));
                return false;
            });

            return res;
        }
#endif

#if M_DISABLED
         [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, Option<T>)> Of<T>(ISub s1, ISub<T> s2)
        {
            var (pub, sub) = React.Channel<(bool, Option<T>)>(scope);

            s1.OnNext(() =>
            {
                pub.Next((true, default));
                return false;
            });
            s2.OnNext(msg =>
            {
                pub.Next((false, msg));
                return false;
            });

            return sub;
        }
#endif

        [NonPerformant(PerfKind.GC)]
        public static ISub<(Option<T1>, Option<T2>)> Merge<T1, T2>(this IScope scope, ISub<T1> s1, ISub<T2> s2)
        {
            var (pub, sub) = React.PubSub<(Option<T1>, Option<T2>)>(scope);

            s1.OnNext(msg => pub.Next((msg, default)), scope);
            s2.OnNext(msg => pub.Next((default, msg)), scope);

            return sub;
        }

#if M_DISABLED
         [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool, bool)> Of(ISub s1, ISub s2, ISub s3)
        {
            var (pub, sub) = React.Channel<(bool, bool, bool)>(scope);

            s1.OnNext(() =>
            {
                pub.Next((true, default, default));
                return false;
            });
            s2.OnNext(() =>
            {
                pub.Next((default, true, default));
                return false;
            });
            s3.OnNext(() =>
            {
                pub.Next((false, default, true));
                return false;
            });

            return sub;
        }
#endif

#if M_DISABLED
         [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool, Option<T3>)> Of<T3>(ISub s1, ISub s2, ISub<T3> s3)
        {
            var (pub, sub) = React.Channel<(bool, bool, Option<T3>)>(scope);

            s1.OnNext(() =>
            {
                pub.Next((true, default, default));
                return false;
            });
            s2.OnNext(() =>
            {
                pub.Next((default, true, default));
                return false;
            });
            s3.OnNext(msg =>
            {
                pub.Next((default, default, msg));
                return false;
            });

            return sub;
        }
#endif


#if !M_DISABLED
        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, Option<T2>, Option<T3>)> Merge<T2, T3>(this IScope scope, ISub s1, ISub<T2> s2, ISub<T3> s3)
        {
            var (pub, sub) = React.PubSub<(bool, Option<T2>, Option<T3>)>(scope);

            s1.OnNext(() => { pub.Next((true, default, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg, default)); }, scope);
            s3.OnNext(msg => { pub.Next((default, default, msg)); }, scope);

            return sub;
        }
#endif

#if M_DISABLED
         [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool, bool, bool)> Of(ISub s1, ISub s2, ISub s3, ISub s4)
        {
            var (pub, sub) = React.Channel<(bool, bool, bool, bool)>(scope);

            s1.OnNext(() =>
            {
                pub.Next((true, default, default, default));
                return false;
            });
            s2.OnNext(() =>
            {
                pub.Next((default, true, default, default));
                return false;
            });
            s3.OnNext(() =>
            {
                pub.Next((default, default, true, default));
                return false;
            });
            s4.OnNext(() =>
            {
                pub.Next((default, default, default, true));
                return false;
            });

            return sub;
        }
#endif


#if M_DISABLED
         // 1 arg version is just for refactoring ease
        [NonPerformant(PerfKind.GC)]
        public static ISub Of(ISub s1)
        {
            var (pub, sub) = React.Channel(scope);

            s1.OnNext(() =>
            {
                pub.Next();
                return false;
            });
            return sub;
        }
#endif
    }
}