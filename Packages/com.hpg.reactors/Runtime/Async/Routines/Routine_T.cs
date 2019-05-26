using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;
using Utility;
using Utility.Asserts;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder<>))]
    public sealed class Routine<T>
    {
        internal (IPub Pub, ISub Sub) Complete { get; }
        internal (IPub<T> Pub, ISub<T> Sub) Complete2 { get; }
        internal (IPub Pub, ISub Sub) BreakInnerFromOuter { get; }
        internal (IDisposable Pub, IScope Sub) Scope { get; }


        internal Routine(Action first)
        {
            var p = React.Scope(out var s);
            Scope = (p, s);
            BreakInnerFromOuter = Scope.Sub.PubSub();
            Complete2 = Scope.Sub.PubSub<T>();
            Complete = Scope.Sub.PubSub();
            BreakInnerFromOuter.Sub.OnNext(first, Scope.Sub);
            BreakInnerFromOuter.Sub.OnNext(Scope.Pub.Dispose, Scope.Sub);

            Complete.Sub.OnNext(Scope.Pub.Dispose, Scope.Sub);
            Complete2.Sub.OnNext(_ => Complete.Pub.Next(), Scope.Sub);
        }

        [UsedImplicitly]
        public GenericAwaiter<T> GetAwaiter()
        {
            throw new NotImplementedException();
//            return new GenericAwaiter<T>((Scope.Sub, BreakInnerFromOuter.Pub.Next), Complete2.Sub);
        }
    }
}