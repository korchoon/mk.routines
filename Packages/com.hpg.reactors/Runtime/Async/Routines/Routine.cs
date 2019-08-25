// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Reactors.Async.Debugger;
using Reactors.Utility;
using Reactors.DataFlow;
using Utility.Asserts;

namespace Reactors.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder))]
    public sealed class Routine : IDisposable
    {
        internal IPub Complete;

        internal IScope Scope;
        internal IDisposable _Dispose;
        IScope _awaitersScope;

        public IScope GetScope(IScope scope)
        {
            scope.Subscribe(Dispose);
            return Scope;
        }

        internal Routine()
        {
            ISub onComplete;
            _Dispose = Sch.Scope.Scope(out Scope);
            Scope.Scope(out _awaitersScope);
            (Complete, onComplete) = Scope.PubSub();
            onComplete.OnNext(Dispose, Scope);
        }

        [UsedImplicitly]
        public GenericAwaiter GetAwaiter()
        {
            var res = new GenericAwaiter(_awaitersScope, Dispose);
            return res;
        }

        public void Dispose() => _Dispose.Dispose();

        #region Static API

        public static SelfScope SelfScope() => new SelfScope();
        public static SelfDispose SelfDispose() => new SelfDispose();

        #endregion
    }
}