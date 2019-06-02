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
        internal IPub Complete;
        ISub _onComplete;

        internal IScope Scope;
        IDisposable _pubScope;

        public IScope GetScope(IScope scope)
        {
            scope.OnDispose(Dispose);
            return Scope;
        }

        internal Routine()
        {
            _pubScope = React.Scope(out Scope);
            (Complete, _onComplete) = Scope.PubSub();
            _onComplete.OnNext(Dispose, Scope);
        }

        [UsedImplicitly]
        public GenericAwaiter2 GetAwaiter()
        {
            if (!Scope.Completed)
                _onComplete.OnNext(Dispose, Scope);

            return new GenericAwaiter2(Scope, Dispose);
        }

        public void Dispose() => _pubScope.Dispose();

        #region Static API

        public static SelfScope SelfScope() => new SelfScope();
        public static SelfDispose SelfDispose() => new SelfDispose();

        #endregion
    }
}