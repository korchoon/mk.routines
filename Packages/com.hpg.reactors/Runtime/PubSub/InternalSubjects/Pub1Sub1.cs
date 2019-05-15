using System;
using Lib.Async;
using Utility.AssertN;

namespace Lib.DataFlow
{
    internal class Pub1Sub1 : ISub, IPub
    {
        Action _action;
        IScope _scope;
        bool _done;
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
            _done = true;
            _action = Empty.Action();
            _scope = ScopeNever.Ever;
        }

        public void OnNext(Func<bool> pub)
        {
            if (_done)
            {
                Asr.Fail("Trying to subscribe more than once or after 1st publish");
                return;
            }

            Asr.IsTrue(_action == Empty.Action());
            _pred = pub;
        }

        public void OnNext(Action pub, IScope scope)
        {
            if (_done)
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

        public bool Next()
        {
            if (_done.WasTrue())
                return false;

            _pred.Invoke();
            RoutineUtils.MoveNextAndClear(ref _action);
            _Complete();
            return false;
        }
    }

    internal class Pub1Sub1<T> : ISub<T>, IPub<T>
    {
        Action<T> _action;
        Func<T, bool> _pred;
        IScope _scope;
        bool _done;

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
            _done = true;
            _action = Empty.Action<T>();
            _scope = ScopeNever.Ever;
        }

        public void OnNext(Func<T, bool> pub)
        {
            if (_done)
            {
                Asr.Fail("Trying to subscribe more than once or after 1st publish");
                return;
            }

            Asr.IsTrue(_action == Empty.Action<T>());
            _pred = pub;
        }

        public void OnNext(Action<T> pub, IScope scope)
        {
            if (_done)
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

        public bool Next(T msg)
        {
            if (_done.WasTrue())
                return false;

            _pred.Invoke(msg);
            RoutineUtils.MoveNextAndClear(ref _action, msg);
            _Complete();
            return false;
        }
    }
}