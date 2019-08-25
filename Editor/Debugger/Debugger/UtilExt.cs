// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Reactors;
using Reactors.DataFlow;

namespace MyNamespace
{
    public static class UtilExt
    {
        public static ISub Schedule(this ISub sub, Func<bool> func, IScope scope)
        {
            var (pub, res) = scope.PubSub();
            sub.OnNext(() =>
            {
                if (func.Invoke())
                    pub.Next();
            }, scope);
            return res;
        }
    }
}