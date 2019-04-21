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
            if (_value)
                pub.Invoke();
            _subject.OnNext(pub);
        }

        public void OnNext(Action pub, IScope sd)
        {
            if (_value)
                pub.Invoke();
            _subject.OnNext(pub, sd);
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
            if (_value.TryGet(out var value))
                pub.Invoke(value);
            _subject.OnNext(pub);
        }

        public void OnNext(Action<T> pub, IScope sd)
        {
            if (_value.TryGet(out var value))
                pub.Invoke(value);
            _subject.OnNext(pub, sd);
        }
    }
}