using Lib.DataFlow;

namespace Lib.Async
{
    public interface IBreakableAwaiter
    {
        void BreakOn(IScope scope);
    }
}