using System;

namespace Lib.DataFlow
{
    internal class SubNever : ISub
    {
        public static SubNever Ever { get; } = new SubNever();

        SubNever()
        {
        }

        public void OnNext(Func<bool> pub)
        {
        }

        public void OnNext(Action pub, IScope scope)
        {
        }
    }

    internal class SubNever<T> : ISub<T>
    {
        public static SubNever<T> Ever { get; } = new SubNever<T>();

        SubNever()
        {
        }

        public void OnNext(Func<T, bool> pub)
        {
        }

        public void OnNext(Action<T> pub, IScope scope)
        {
        }
    }
}