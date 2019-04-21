using System;
using Lib.Async;
using Lib.Attributes;
using Lib.Pooling;

namespace Lib.DataFlow
{
    [NonPerformant(PerfKind.GC)]
    internal class Subject<T> : ISub<T>, IPub<T>
    {
        internal readonly CompleteToken Completed;
        readonly DisposableSubject _comp;
        readonly Subscribers<T> _next;

        static readonly Pool<Subscribers<T>> NextPool = new Pool<Subscribers<T>>(() => new Subscribers<T>(), subs => subs.Reset());

        public Subject(IScope scope)
        {
            scope.OnDispose(_Dispose);
            _comp = DisposableSubject.Pool._GetRaw();
            _next = NextPool._GetRaw();
            Completed = new CompleteToken();
        }

        void _Dispose()
        {
            if (Completed.Set()) return;

            _comp.Dispose();
            _next.Dispose();
            DisposableSubject.Pool._ReleaseRaw(_comp);

            NextPool._ReleaseRaw(_next);
        }

        public void OnNext(Func<T, bool> pub)
        {
            if (Completed)
                return;

            _next.Sub(pub);
        }

        public void OnNext(Action<T> pub, IScope sd)
        {
            if (Completed)
                return;

            var moveNext = true;
            _next.Sub(MoveNext);
            sd.OnDispose(Dispose);

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
#if ASSERT_PUB
            var optional = msg is IMsgOptional;
            var assert = optional ? true : any;
            if (!assert) Dbg.ErrorOnFalse(assert, $"Unobserved '{msg.GetType().Name}'");// in '{new DbgStack()}' \n{DebugDataFlow.Get(this).Json}");
#endif
        }

        public void OnDispose(Action dispose)
        {
            _comp.OnDispose(dispose);
        }
    }

    internal class Subject : ISub, IPub
    {
        internal readonly CompleteToken Completed;
        readonly DisposableSubject _comp;
        readonly Subscribers _next;

        static readonly Pool<Subscribers> NextPool = new Pool<Subscribers>(() => new Subscribers(), subs => subs.Reset());

        public Subject(IScope scope)
        {
            scope.OnDispose(_Dispose);
            _comp = DisposableSubject.Pool._GetRaw();
            _next = NextPool._GetRaw();
            Completed = new CompleteToken();
        }


        void _Dispose()
        {
            if (Completed.Set()) return;

            _comp.Dispose();
            _next.Dispose();
            DisposableSubject.Pool._ReleaseRaw(_comp);

            NextPool._ReleaseRaw(_next);
        }

        public void OnNext(Func<bool> pub)
        {
            if (Completed)
                return;

            _next.Sub(pub);
        }

        public void OnNext(Action pub, IScope sd)
        {
            if (Completed)
                return;

            var moveNext = true;
            _next.Sub(MoveNext);
            sd.OnDispose(Dispose);

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
#if ASSERT_PUB
            var optional = msg is IMsgOptional;
            var assert = optional ? true : any;
            if (!assert) Dbg.ErrorOnFalse(assert, $"Unobserved '{msg.GetType().Name}'");// in '{new DbgStack()}' \n{DebugDataFlow.Get(this).Json}");
#endif
        }
    }
}