using System;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib
{
    public static class ReactExtensions
    {
        public static T1 DisposeOn<T1>(this T1 disposePub, IScope scope1, IScope scope2) where T1 : IDisposable
        {
            var scope = scope1.AddPure(scope2);
            scope.OnDispose(disposePub.Dispose);
            return disposePub;
        }
        
        public static T1 DisposeOn<T1>(this T1 dispose, IScope scope) where T1 : IDisposable
        {
            scope.OnDispose(dispose.Dispose);
            return dispose;
        }

        [MustUseReturnValue]
        public static IScope AddPure(this IScope a, IScope b)
        {
            var res = new ScopeSubject();
            a.OnDispose(res.Dispose);
            b.OnDispose(res.Dispose);
            return res;
        }
    }
}