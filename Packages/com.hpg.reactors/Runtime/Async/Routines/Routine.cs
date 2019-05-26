using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;
using Utility.Asserts;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder))]
    public sealed class Routine : IDisposable
    {
        internal (IPub Pub, ISub Sub) Complete { get; }
        (IPub Pub, ISub Sub) BreakInnerFromOuter { get; }
        internal (IDisposable Pub, IScope Sub) Scope1 { get; }

        public IScope GetScope(IScope scope) => Scope(scope);

        public IScope Scope(IScope scope)
        {
            scope.OnDispose(Dispose);
            return Scope1.Sub;
        }

        internal Routine(Action first)
        {
            var p = React.Scope(out var s);
            Scope1 = (p, s);
            BreakInnerFromOuter = Scope1.Sub.PubSub();
            Complete = Scope1.Sub.PubSub();
            BreakInnerFromOuter.Sub.OnNext(first, Scope1.Sub);
            BreakInnerFromOuter.Sub.OnNext(Scope1.Pub.Dispose, Scope1.Sub);
            Complete.Sub.OnNext(Scope1.Pub.Dispose, Scope1.Sub);
        }

        [UsedImplicitly]
        public GenericAwaiter2 GetAwaiter()
        {
            return new GenericAwaiter2(Complete.Sub, Scope1.Sub, Scope1.Pub);
//            throw new NotImplementedException();
//            return new GenericAwaiter((Scope1.Sub, BreakInnerFromOuter.Pub.Next), Complete.Sub);
        }

        public void Dispose() => BreakInnerFromOuter.Pub.Next();

        #region Static API

        public static SelfScope SelfScope() => new SelfScope();
        public static SelfDispose SelfDispose() => new SelfDispose();

        #endregion
    }
}