// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Reactors.DataFlow;

namespace Reactors.Async
{
    public class SchBase<T> : ISub<T>
    {
        internal SchBase(string name, out IPub<T> s, IScope scope)
        {
            _toString = name;
            (s, _sub) = React.PubSub<T>(scope);
        }

        string _toString;
        public override string ToString() => _toString;
        ISub<T> _sub;

        public void OnNext(Action<T> pub, IScope scope)
        {
            _sub.OnNext(pub, scope);
        }
    }


    public class SchBase : ISub
    {
        internal SchBase(string name, out IPub s, IScope scope)
        {
            _toString = name;
            (s, _sub) = React.PubSub(scope);
        }

        string _toString;
        public override string ToString() => _toString;

        ISub _sub;

        public void OnNext(Action pub, IScope scope)
        {
            _sub.OnNext(pub, scope);
        }
    }
}