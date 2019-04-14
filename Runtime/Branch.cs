using Lib;
using Lib.Attributes;
using Lib.DataFlow;
using Lib.Utility;

namespace Game
{
    [NonPerformant(PerfKind.GC)]
    public static class Branch
    {
        // todo CHECK has single value
        
        static ISub ToSub(IScope scope)
        {
            var (pub, on) = React.Channel(scope);
            scope.OnDispose(() => pub.Next());
            return on;
        }

        public static ISub<(bool, bool, Option<T>)> Of<T>(IScope onMiniGameOver, ISub onFail, ISub<T> onStroke)
        {
            return Of(ToSub(onMiniGameOver), onFail, onStroke);
        }

        public static ISub<(bool, Option<T>)> Of<T>(IScope s1, ISub<T> s2)
        {
            var (pub, sub) = React.Channel<(bool, Option<T>)>(Empty);
            var res = sub;

            s1.OnDispose(() => pub.Next((true, default)));
            s2.OnNext(t =>
            {
                pub.Next((false, t));
                return false;
            });

            return res;
        }

        [NonPerformant(PerfKind.GC)]
        public static ISub<(bool, bool)> Of(ISub s1, ISub s2)
        {
            var (pub, sub) = React.Channel<(bool, bool)>(Empty);
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
            var (pub, sub) = React.Channel<(bool, Option<T>)>(Empty);

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
            var (pub, sub) = React.Channel<(Option<T1>, Option<T2>)>(Empty);

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
            var (pub, sub) = React.Channel<(bool, bool, bool)>(Empty);

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
            var (pub, sub) = React.Channel<(bool, bool, Option<T3>)>(Empty);

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
            var (pub, sub) = React.Channel<(bool, Option<T2>, Option<T3>)>(Empty);

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
            var (pub, sub) = React.Channel<(bool, bool, bool, bool)>(Empty);

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

        static IScope Empty => Lib.DataFlow.Empty.Scope();

        // 1 arg version is just for refactoring ease
        [NonPerformant(PerfKind.GC)]
        public static ISub Of(ISub s1)
        {
            var (pub, sub) = React.Channel(Empty);

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
            var (pub, sub) = React.Channel<T1>(Empty);
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