using System;
using Lib.Async;
using Lib.Attributes;
using Lib.Pooling;

namespace Lib.DataFlow
{
    [NonPerformant(PerfKind.GC)]
    internal class Subject<T> : ISub<T>, IPub<T>
    {
        internal bool Completed;
        Subscribers<T> _next;

        static readonly Pool<Subscribers<T>> NextPool = new Pool<Subscribers<T>>(() => new Subscribers<T>(), subs => subs.Reset());

        public Subject(IScope scope)
        {
            scope.OnDispose(_Dispose);
            _next = NextPool.Get();
            Completed = false;
        }

        void _Dispose()
        {
            if (Completed.WasTrue()) return;

            _next.Dispose();

            NextPool.Release(ref _next);
        }

        public void OnNext(Func<T, bool> pub)
        {
            if (Completed)
                return;

            _next.Sub(pub);
        }

        public void OnNext(Action<T> pub, IScope scope)
        {
            if (Completed)
                return;

            var moveNext = true;
            _next.Sub(MoveNext);
            scope.OnDispose(Dispose);

            void Dispose() => moveNext = false;

            bool MoveNext(T arg)
            {
                if (!moveNext) return false;

                pub.Invoke(arg);
                return moveNext;
            }
        }


        public bool Next(T msg)
        {
            if (Completed) return false;

            return _next.Next(msg);
        }
    }

    internal class Subject : ISub, IPub
    {
        internal bool Completed;
        Subscribers _next;

        static readonly Pool<Subscribers> NextPool = new Pool<Subscribers>(() => new Subscribers(), subs => subs.Reset());

        public Subject(IScope scope)
        {
            scope.OnDispose(_Dispose);
            _next = NextPool.Get();
            Completed = false;
        }


        void _Dispose()
        {
            if (Completed.WasTrue()) return;

            _next.Dispose();
            NextPool.Release(ref _next);
        }

        public void OnNext(Func<bool> pub)
        {
            if (Completed)
                return;

            _next.Sub(pub);
        }

        public void OnNext(Action pub, IScope scope)
        {
            if (Completed)
                return;

            var moveNext = true;
            _next.Sub(MoveNext);
            scope.OnDispose(Dispose);

            bool MoveNext()
            {
                if (!moveNext) return false;

                pub.Invoke();
                return moveNext;
            }

            void Dispose() => moveNext = false;
        }


        public bool Next()
        {
            if (Completed) return false;

            return _next.Next();
        }
    }
}