using System;
using JetBrains.Annotations;
using Lib.Attributes;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib
{
    public static class ScopeApi
    {
        public static ISub<(bool, Option<T2>, Option<T3>)> BranchOf<T2, T3>(this IScope scope, ISub s1, ISub<T2> s2, ISub<T3> s3)
        {
            var (pub, sub) = React.PubSub11<(bool, Option<T2>, Option<T3>)>(scope);

            s1.OnNext(() =>
            {
                pub.Next((true, default, default));
                return false;
            });
            s2.OnNext(msg =>
            {
                pub.Next((default, msg, default));
                return false;
            });
            s3.OnNext(msg =>
            {
                pub.Next((default, default, msg));
                return false;
            });

            return sub;
        }

        public static ISub<(Option<T1>, Option<T2>)> BranchOf<T1, T2>(this IScope scope, ISub<T1> s1, ISub<T2> s2)
        {
            var (pub, sub) = React.PubSub11<(Option<T1>, Option<T2>)>(scope);

            s1.OnNext(msg =>
            {
                pub.Next((msg, default));
                return false;
            });
            s2.OnNext(msg =>
            {
                pub.Next((default, msg));
                return false;
            });

            return sub;
        }

        public static ISub<(bool, bool)> BranchOf(this IScope scope, ISub s1, ISub s2)
        {
            var (pub, sub) = React.PubSub11<(bool, bool)>(scope);
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

        [MustUseReturnValue]
        public static IScope Scope(this IScope outer)
        {
            var dispose = React.Scope(out var res);
            Action action = dispose.Dispose;
            res.OnDispose(_Remove);
            outer.OnDispose(action);
            return res;

            void _Remove() => outer.Unsubscribe(action);
        }
    }

    [NonPerformant(PerfKind.GC)]
    public static class BranchEvery
    {
        static IScope scope => Lib.DataFlow.Empty.Scope();

        // todo CHECK has single value

        static ISub ToSub(IScope scope)
        {
            var (pub, on) = React.Channel(scope);
            scope.OnDispose(() => pub.Next());
            return on;
        }

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

        [NonPerformant(PerfKind.GC)]
        public static ISub<(Option<T1>, Option<T2>)> Of<T1, T2>(ISub<T1> s1, ISub<T2> s2)
        {
            var (pub, sub) = React.Channel<(Option<T1>, Option<T2>)>(scope);

            s1.OnNext(msg =>
            {
                pub.Next((msg, default));
                return false;
            });
            s2.OnNext(msg =>
            {
                pub.Next((default, msg));
                return false;
            });

            return sub;
        }

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


        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, Option<T2>, Option<T3>)> Of<T2, T3>(ISub s1, ISub<T2> s2, ISub<T3> s3)
        {
            var (pub, sub) = React.Channel<(bool, Option<T2>, Option<T3>)>(scope);

            s1.OnNext(() =>
            {
                pub.Next((true, default, default));
                return false;
            });
            s2.OnNext(msg =>
            {
                pub.Next((default, msg, default));
                return false;
            });
            s3.OnNext(msg =>
            {
                pub.Next((default, default, msg));
                return false;
            });

            return sub;
        }

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

        // 1 arg version is just for refactoring ease
        [NonPerformant(PerfKind.GC)]
        public static ISub<T1> Of<T1>(ISub<T1> s1)
        {
            var (pub, sub) = React.Channel<T1>(scope);
            var res = sub;

            s1.OnNext(msg =>
            {
                pub.Next(msg);
                return false;
            });

            return res;
        }
    }
}