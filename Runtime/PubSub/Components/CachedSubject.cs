using System;
using Lib.Utility;

namespace Lib.DataFlow
{
    public class CachedSubject : IPub, ISub, IAwait
    {
        readonly Subject _subject;
        bool _value;

        public CachedSubject(IScope scope)
        {
            _subject = new Subject(scope);
        }

        public bool IsCompleted => _subject.Completed;

        public bool Next()
        {
            if (IsCompleted) return false;

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
}