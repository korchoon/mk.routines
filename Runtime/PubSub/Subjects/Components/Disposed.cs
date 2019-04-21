namespace Lib.DataFlow
{
    public class CompleteToken
    {
        bool _disposed;

        public bool Set()
        {
            var a = _disposed;
            _disposed = true;
            return a;
        }

        public static implicit operator bool(CompleteToken t) => t._disposed;
    }
}