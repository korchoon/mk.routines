namespace Lib.Async
{
    public interface IBreakableAwaiter
    {
        void BreakInner();
        void Unsub();
    }
}