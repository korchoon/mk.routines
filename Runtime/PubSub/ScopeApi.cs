// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib
{
    public static class ScopeApi
    {
        public static ISub<(Option<T1>, Option<T2>, Option<T3>)> BranchOf<T1, T2, T3>(this IScope scope, ISub<T1> s1, ISub<T2> s2, ISub<T3> s3)
        {
            var subject = new Subject<(Option<T1>, Option<T2>, Option<T3>)>(scope);
            var (pub, sub) = ((IPub<(Option<T1>, Option<T2>, Option<T3>)> pub, ISub<(Option<T1>, Option<T2>, Option<T3>)> sub)) (subject, subject);

            s1.OnNext(msg => { pub.Next((msg, default, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg, default)); }, scope);
            s3.OnNext(msg => { pub.Next((default, default, msg)); }, scope);

            return sub;
        }

        public static ISub<(bool, Option<T2>, Option<T3>)> BranchOf<T2, T3>(this IScope scope, ISub s1, ISub<T2> s2, ISub<T3> s3)
        {
            var subject = new Subject<(bool, Option<T2>, Option<T3>)>(scope);
            var (pub, sub) = ((IPub<(bool, Option<T2>, Option<T3>)> pub, ISub<(bool, Option<T2>, Option<T3>)> sub)) (subject, subject);

            s1.OnNext(() => { pub.Next((true, default, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg, default)); }, scope);
            s3.OnNext(msg => { pub.Next((default, default, msg)); }, scope);

            return sub;
        }

        public static ISub<(bool, Option<T2>)> BranchOf<T2>(this IScope scope, ISub s1, ISub<T2> s2)
        {
            var subject = new Subject<(bool, Option<T2>)>(scope);
            var (pub, sub) = ((IPub<(bool, Option<T2>)> pub, ISub<(bool, Option<T2>)> sub)) (subject, subject);

            s1.OnNext(() => { pub.Next((true, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg)); }, scope);

            return sub;
        }


        public static ISub<(Option<T1>, Option<T2>)> BranchOf<T1, T2>(this IScope scope, ISub<T1> s1, ISub<T2> s2)
        {
            var subject = new Subject<(Option<T1>, Option<T2>)>(scope);
            var (pub, sub) = ((IPub<(Option<T1>, Option<T2>)> pub, ISub<(Option<T1>, Option<T2>)> sub)) (subject, subject);

            s1.OnNext(msg => { pub.Next((msg, default)); }, scope);
            s2.OnNext(msg => { pub.Next((default, msg)); }, scope);

            return sub;
        }

        public static ISub<(Option<T>, bool, bool)> BranchOf<T>(this IScope scope, ISub<T> s1, ISub s2, ISub s3)
        {
            var subject = new Subject<(Option<T>, bool, bool)>(scope);
            var (pub, sub) = ((IPub<(Option<T>, bool, bool)> pub, ISub<(Option<T>, bool, bool)> sub)) (subject, subject);
            var res = sub;

            s1.OnNext(msg => { pub.Next((msg, false, false)); }, scope);
            s2.OnNext(() => { pub.Next((default, true, false)); }, scope);
            s3.OnNext(() => { pub.Next((default, false, true)); }, scope);

            return res;
        }

        public static ISub<(bool, bool, bool)> BranchOf(this IScope scope, ISub s1, ISub s2, ISub s3)
        {
            var subject = new Subject<(bool, bool, bool)>(scope);
            var (pub, sub) = ((IPub<(bool, bool, bool)> pub, ISub<(bool, bool, bool)> sub)) (subject, subject);
            var res = sub;

            s1.OnNext(() => { pub.Next((true, false, false)); }, scope);
            s2.OnNext(() => { pub.Next((false, true, false)); }, scope);
            s3.OnNext(() => { pub.Next((false, false, true)); }, scope);

            return res;
        }

        public static ISub<(bool, bool)> BranchOf(this IScope scope, ISub s1, ISub s2)
        {
            var subject = new Subject<(bool, bool)>(scope);
            var (pub, sub) = ((IPub<(bool, bool)> pub, ISub<(bool, bool)> sub)) (subject, subject);
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
            res.Subscribe(_Remove);
            outer.Subscribe(action);
            return dispose;

            void _Remove() => outer.Unsubscribe(action);
        }

        internal static void Subscribe(this IScope target, Action dispose, IScope unsubScope)
        {
            target.Subscribe(dispose);
            unsubScope.Subscribe(_Remove);

            void _Remove() => target.Unsubscribe(dispose);
        }
    }
}