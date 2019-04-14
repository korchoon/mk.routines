using System;
using Lib.Utility;

namespace Lib.DataFlow
{
    public class CachedSubject<T> : IPub<T>, ISub<T>, IAwait
    {
        readonly Subject<T> _subject;
        Option<T> _value;

        public CachedSubject(IScope scope)
        {
            _subject = new Subject<T>(scope);
        }

        public bool IsCompleted => _subject.Completed;

        public bool Next(T msg)
        {
            if (IsCompleted) return false;

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