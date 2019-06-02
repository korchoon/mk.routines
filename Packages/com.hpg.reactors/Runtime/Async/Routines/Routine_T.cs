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
        RoutineBuilder<T> _builder;

        internal (IPub Pub, ISub Sub) Complete { get; }

        Option<T> _result;

        internal void SetResult(T res)
        {
            _result = res;
        }

        internal (IDisposable Pub, IScope Sub) _scope { get; }

        public IScope GetScope(IScope scope)
        {
            scope.OnDispose(Dispose);
            return _scope.Sub;
        }

        internal Routine(RoutineBuilder<T> builder)
        {
            _builder = builder;
            var disposable = React.Scope(out var scope);
            _scope = (disposable, scope);

            Complete = scope.PubSub();

            Complete.Sub.OnNext(_scope.Pub.Dispose, scope);
        }


        [UsedImplicitly]
        public GenericAwaiter2<T> GetAwaiter()
        {
            if (!_scope.Sub.Completed)
                Complete.Sub.OnNext(Dispose, _scope.Sub);
            var aw = new GenericAwaiter2(_scope.Sub, Dispose);
            return new GenericAwaiter2<T>(aw, () => _result.GetOrFail());
        }

        public void Dispose()
        {
            _scope.Pub.Dispose();
            _builder.BreakCurrent();
        }
    }
}