using System;
using Lib.Utility;

namespace Lib.DataFlow
{
    public class CachedChannel<T> : IPub<T>, ISub<T>, IAwait<T>
    {
        readonly Subject<T> _subject;
        Option<T> _value;

        public CachedChannel(IScope scope)
        {
            _subject = new Subject<T>(scope);
            _value = new Option<T>();
        }


        public bool IsCompleted => _subject.Completed;
        public bool TryGet(out T value) => _value.TryGet(out value);

        public bool Next(T msg)
        {
            if (IsCompleted) return false;

            _value = msg;
            _subject.Next(msg);
            return true;
        }


        public void OnNext(Func<T, bool> pub)
        {
            if (_value.TryGet(out var v))
                pub.Invoke(v);
            _subject.OnNext(pub);
        }

        public void OnNext(Action<T> pub, IScope sd)
        {
            if (_value.TryGet(out var v))
                pub.Invoke(v);
            _subject.OnNext(pub, sd);
        }
    }
}