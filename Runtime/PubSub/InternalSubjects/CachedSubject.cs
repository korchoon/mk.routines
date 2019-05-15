using System;
using Lib.Utility;

namespace Lib.DataFlow
{
    public class CachedSubject : IPub, ISub
    {
        readonly Subject _subject;
        bool _value;

        public CachedSubject(IScope scope)
        {
            _subject = new Subject(scope);
        }

        public bool Next()
        {
            if (_subject.Completed) return false;

            _value = true;
            return _subject.Next();
        }

        public void OnNext(Func<bool> pub)
        {
            _subject.OnNext(pub);
            if (_value)
                pub.Invoke();
        }

        public void OnNext(Action pub, IScope scope)
        {
            _subject.OnNext(pub, scope);
            if (_value)
                pub.Invoke();
        }
    }

    public class CachedSubject<T> : IPub<T>, ISub<T>
    {
        readonly Subject<T> _subject;
        Option<T> _value;

        public CachedSubject(IScope scope)
        {
            _subject = new Subject<T>(scope);
        }

        public bool Next(T msg)
        {
            if (_subject.Completed) return false;

            _value = msg;
            return _subject.Next(msg);
        }

        public void OnNext(Func<T, bool> pub)
        {
            _subject.OnNext(pub);
            if (_value.TryGet(out var value))
                pub.Invoke(value);
        }

        public void OnNext(Action<T> pub, IScope scope)
        {
            _subject.OnNext(pub, scope);
            if (_value.TryGet(out var value))
                pub.Invoke(value);
        }
    }
}