using System;

namespace Lib.DataFlow
{
    public interface IDisposeWith<in T> where T : Exception
    {
        void DisposeWith(T msg);
    }
}