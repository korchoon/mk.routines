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
    public class PubOnceScopeOnce : IPub, IScope
    {
        Action _action;
        Action _onDispose;
        bool _completed;

        public PubOnceScopeOnce(Action action)
        {
            _action = action;
            _onDispose = Empty.Action();
        }


        public void Next()
        {
            Asr.IsFalse(_completed);
            if (_completed)
                return;


            RoutineUtils.MoveNextAndClear(ref _action);
            RoutineUtils.MoveNextAndClear(ref _onDispose);
            _completed = true;
            return;
        }

        public bool Completed => _completed;

        public void OnDispose(Action dispose)
        {
            Asr.IsFalse(_completed);
            if (_completed)
            {
                dispose.Invoke();
                return;
            }

            if (_onDispose != Empty.Action())
                Asr.Fail("Subscibed more that once");

            _onDispose = dispose;
        }

        public void Unsubscribe(Action dispose)
        {
            Asr.IsFalse(_completed);
            if (_completed) return;


            if (_onDispose != dispose)
                return;

//            Asr.IsTrue(_onDispose == dispose);
            _onDispose = Empty.Action();
        }
    }

    public class PubOnceScopeOnce<T> : IPub<T>, IScope
    {
        Action<T> _action;
        Action _onDispose;
        bool _completed;

        public PubOnceScopeOnce(Action<T> action)
        {
            _action = action;
            _onDispose = Empty.Action();
        }

        public bool Completed => _completed;

        public void Next(T msg)
        {
            Asr.IsFalse(_completed);
            if (_completed)
                return;

            RoutineUtils.MoveNextAndClear(ref _action, msg);
            RoutineUtils.MoveNextAndClear(ref _onDispose);
            _completed = true;
            return;
        }

        public void OnDispose(Action dispose)
        {
            Asr.IsFalse(_completed);
            if (_completed)
            {
                dispose.Invoke();
                return;
            }

            if (_onDispose != Empty.Action())
                Asr.Fail("Subscibed more that once");

            _onDispose = dispose;
        }

        public void Unsubscribe(Action dispose)
        {
            Asr.IsFalse(_completed);
            if (_completed) return;

            if (_onDispose != dispose)
                return;

//            Asr.IsTrue(_onDispose == dispose);
            _onDispose = Empty.Action();
        }
    }

    public static class UtilsOnce
    {
        public static void OnNextOnce<T>(this ISub<T> sub, Action<T> action)
        {
#if !M_OPT
            var pubScope = new PubOnceScopeOnce<T>(action);
            // todo cache delegate
            sub.OnNext(pubScope.Next, pubScope);

#else
            var dispose = React.Scope(out var scope);
            sub.OnNext(msg =>
            {
                dispose.Dispose();
                action.Invoke(msg);
            }, scope);
#endif
        }

        public static void OnNextOnce(this ISub sub, Action action)
        {
#if !M_OPT
            var pubScope = new PubOnceScopeOnce(action);
            // todo cache delegate
            sub.OnNext(pubScope.Next, pubScope);

#else
            var dispose = React.Scope(out var scope);
            sub.OnNext(() =>
            {
                dispose.Dispose();
                action.Invoke();
            }, scope);
#endif
        }
    }
}