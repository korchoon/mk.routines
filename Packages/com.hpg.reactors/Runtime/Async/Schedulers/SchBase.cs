using System;
using Lib.DataFlow;

namespace Lib.Async
{
    public class SchBase<T> : ISub<T>
    {
        internal SchBase(string name, out IPub<T> s, IScope scope)
        {
            _toString = name;
            (s, _sub) = React.Channel<T>(scope);
        }

        string _toString;
        public override string ToString() => _toString;
        ISub<T> _sub;

        public void OnNext(Func<T, bool> pub)
        {
            _sub.OnNext(pub);
        }

        public void OnNext(Action<T> pub, IScope sd)
        {
            _sub.OnNext(pub, sd);
        }
    }


    public class SchBase : ISub
    {
        internal SchBase(string name, out IPub s, IScope scope)
        {
            _toString = name;
            (s, _sub) = React.Channel(scope);
        }

        string _toString;
        public override string ToString() => _toString;

        ISub _sub;

        public void OnNext(Func<bool> pub)
        {
            _sub.OnNext(pub);
        }

        public void OnNext(Action pub, IScope sd)
        {
            _sub.OnNext(pub, sd);
        }
    }
}