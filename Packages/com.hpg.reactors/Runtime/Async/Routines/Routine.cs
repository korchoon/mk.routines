using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.Async.Debugger;
using Lib.DataFlow;
using Lib.Utility;
using Utility.Asserts;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder))]
    public sealed class Routine : IDisposable
    {
        RoutineBuilder _builder;

        internal (IPub Pub, ISub Sub) Complete { get; }
        internal (IDisposable Pub, IScope Sub) _scope { get; }

        public IScope GetScope(IScope scope)
        {
            scope.OnDispose(Dispose);
            return _scope.Sub;
        }

        internal Routine(RoutineBuilder builder)
        {
            _builder = builder;
            var disposable = React.Scope(out var scope);
            _scope = (disposable, scope);

            Complete = scope.PubSub();

            Complete.Sub.OnNext(_scope.Pub.Dispose, scope);
        }


        [UsedImplicitly]
        public GenericAwaiter2 GetAwaiter()
        {
            if (!_scope.Sub.Completed)
                Complete.Sub.OnNext(Dispose, _scope.Sub);

            var res = new GenericAwaiter2(_scope.Sub, Dispose);
            return res;
        }

        public void Dispose()
        {
            _builder.BreakCurrent();
            _scope.Pub.Dispose();
        }

        #region Static API

        public static SelfScope SelfScope() => new SelfScope();
        public static SelfDispose SelfDispose() => new SelfDispose();

        #endregion
    }
}