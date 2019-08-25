// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

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

        internal IScope Scope;
        IDisposable _pubScope;
        
        Option<T> _result;
        IScope _awaitersScope;

        internal void SetResult(T res) => _result = res;

        public IScope GetScope(IScope scope)
        {
            scope.OnDispose(Dispose);
            return Scope;
        }

        internal Routine()
        {
            ISub onComplete;
            _pubScope = React.Scope(out Scope);
            Scope.Scope(out _awaitersScope);
            (Complete, onComplete) = Scope.PubSub();
            onComplete.OnNext(Dispose, Scope);
        }

        [UsedImplicitly]
        public GenericAwaiter2<T> GetAwaiter()
        {
            var aw = new GenericAwaiter2(_awaitersScope, Dispose);
            return new GenericAwaiter2<T>(aw, () => _result.GetOrFail());
        }

        public void Dispose() => _pubScope.Dispose();
    }
}