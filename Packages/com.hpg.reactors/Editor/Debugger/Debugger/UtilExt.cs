using System;
using Lib;
using Lib.DataFlow;

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