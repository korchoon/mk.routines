// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Reactors.Utility;

namespace Reactors.DataFlow
{
    public class CachedSubject : IPub, ISub
    {
        readonly Subject _subject;
        bool _value;

        public CachedSubject(IScope scope)
        {
            _subject = new Subject(scope);
        }

        public void Next()
        {
            if (_subject.Completed) return;

            _value = true;
            _subject.Next();
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

        public void Next(T msg)
        {
            if (_subject.Completed) return;

            _value = msg;
            _subject.Next(msg);
        }


        public void OnNext(Action<T> pub, IScope scope)
        {
            _subject.OnNext(pub, scope);
            if (_value.TryGet(out var value))
                pub.Invoke(value);
        }
    }
}