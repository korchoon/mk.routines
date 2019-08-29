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
    public sealed class Routine<T>
    {
        internal IPub Complete;

        internal IScope Scope;
        internal IDisposable _Dispose;

        Option<T> _result;
        IScope _awaitersScope;

        internal void SetResult(T res) => _result = res;

        internal Routine()
        {
            ISub onComplete;
            _Dispose = Sch.Scope.Scope(out Scope);
            Scope.Scope(out _awaitersScope);
            (Complete, onComplete) = Scope.PubSub();
            onComplete.OnNext(_Dispose.Dispose, Scope);
        }

        [UsedImplicitly]
        public GenericAwaiter<T> GetAwaiter()
        {
            var aw = new GenericAwaiter(_awaitersScope, _Dispose.Dispose);
            return new GenericAwaiter<T>(aw, () => _result.GetOrFail());
        }

        public Optional ToOptional() => new Optional(GetAwaiterOptional(), _Dispose, Scope);

        GenericAwaiter<Option<T>> GetAwaiterOptional()
        {
            // todo assert awaiterScope is empty
            // todo assert awaiterScope will not be used (no subscribes)
            var aw = new GenericAwaiter(_awaitersScope, _Dispose.Dispose);
            return new GenericAwaiter<Option<T>>(aw, () => _result);
        }


        public class Optional : IDisposable
        {
            GenericAwaiter<Option<T>> _aw;
            IDisposable _disposable;
            IScope _scope;

            public Optional(GenericAwaiter<Option<T>> aw, IDisposable disposable, IScope scope)
            {
                _aw = aw;
                _disposable = disposable;
                _scope = scope;
            }

            // todo: should I copy or reuse?
            public GenericAwaiter<Option<T>> GetAwaiter() => _aw;

            public IScope GetScope(IScope scope)
            {
                scope.Subscribe(Dispose);
                return _scope;
            }

            public void Dispose()
            {
                _disposable.Dispose();
            }
        }
    }
}