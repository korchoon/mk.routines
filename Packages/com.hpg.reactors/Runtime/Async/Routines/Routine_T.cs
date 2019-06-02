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
    public sealed class Routine<T> : IDisposable
    {
        internal IPub Complete;
        ISub _onComplete;
        
        internal IScope Scope;
        IDisposable _pubScope;
        
        Option<T> _result;

        internal void SetResult(T res) => _result = res;

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
        public GenericAwaiter2<T> GetAwaiter()
        {
            if (!Scope.Completed)
                _onComplete.OnNext(Dispose, Scope);

            var aw = new GenericAwaiter2(Scope, Dispose);
            return new GenericAwaiter2<T>(aw, () => _result.GetOrFail());
        }

        public void Dispose() => _pubScope.Dispose();
    }
}