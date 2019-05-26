namespace Lib.Async
{
    public interface IBreakableAwaiter
    {
        void BreakInner();
    }
}