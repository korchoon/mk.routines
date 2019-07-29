// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.Async;
using Utility.Asserts;

namespace Lib.DataFlow
{

       internal class Pub1Sub1 : ISub, IPub
    {
        Action _action;
        IScope _scope;
        bool _completed;
        Func<bool> _pred;

        public Pub1Sub1(IScope scope)
        {
            _scope = scope;
            _scope.OnDispose(_Complete);
            _action = Empty.Action();
            _pred = Empty.FuncPredicate();
        }

        void _Complete()
        {
            _scope.Unsubscribe(_Complete);
            _completed = true;
            _action = Empty.Action();
            _scope = ScopeNever.Never;
        }

        public void OnNext(Action pub, IScope scope)
        {
            if (_completed)
            {
                Asr.Fail("Trying to subscribe more than once or after 1st publish");
                return;
            }

            _action = pub;
            scope.OnDispose(CompleteLocal);

            void CompleteLocal()
            {
                scope.Unsubscribe(CompleteLocal);
                if (_action == pub) _action = Empty.Action();
            }
        }

        public void Next()
        {
            Asr.IsFalse(_completed);
            if (_completed.WasTrue())
                return;

            _pred.Invoke();
            RoutineUtils.MoveNextAndClear(ref _action);
            _Complete();
        }

    }

    internal class Pub1Sub1<T> : ISub<T>, IPub<T>
    {
        Action<T> _action;
        Func<T, bool> _pred;
        IScope _scope;
        bool _completed;

        public Pub1Sub1(IScope scope)
        {
            _scope = scope;
            _scope.OnDispose(_Complete);
            _action = Empty.Action<T>();
            _pred = Empty.FuncPredicate<T>();
        }

        void _Complete()
        {
            _scope.Unsubscribe(_Complete);
            _completed = true;
            _action = Empty.Action<T>();
            _scope = ScopeNever.Never;
        }


        public void OnNext(Action<T> pub, IScope scope)
        {
            if (_completed)
            {
                Asr.Fail("Trying to subscribe more than once or after 1st publish");
                return;
            }

            _action = pub;
            scope.OnDispose(CompleteLocal);

            void CompleteLocal()
            {
                scope.Unsubscribe(CompleteLocal);
                if (_action == pub) _action = Empty.Action<T>();
            }
        }

        public void Next(T msg)
        {
//            Asr.IsFalse(_completed);
            if (_completed.WasTrue())
                return;

            _pred.Invoke(msg);
            RoutineUtils.MoveNextAndClear(ref _action, msg);
            _Complete();
        }

    }
}