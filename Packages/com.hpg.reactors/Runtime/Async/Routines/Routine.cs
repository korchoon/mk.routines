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
        internal (IDisposable Pub, IScope Sub) _scope { get; }

        public IScope GetScope(IScope scope)
        {
            scope.OnDispose(Dispose);
            return _scope.Sub;
        }

        internal Routine(Action first)
        {
            var p = React.Scope(out var s);
            _scope = (p, s);
            BreakInnerFromOuter = _scope.Sub.PubSub();
            Complete = _scope.Sub.PubSub();
            BreakInnerFromOuter.Sub.OnNext(first, _scope.Sub);
            BreakInnerFromOuter.Sub.OnNext(_scope.Pub.Dispose, _scope.Sub);
            Complete.Sub.OnNext(_scope.Pub.Dispose, _scope.Sub);
        }

        [UsedImplicitly]
        public GenericAwaiter2 GetAwaiter() => new GenericAwaiter2(Complete.Sub, _scope.Sub, _scope.Pub);

        public void Dispose() => BreakInnerFromOuter.Pub.Next();

        #region Static API

        public static SelfScope SelfScope() => new SelfScope();
        public static SelfDispose SelfDispose() => new SelfDispose();

        #endregion
    }
}