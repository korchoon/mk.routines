using Lib.Utility;

namespace Lib.DataFlow
{
    // todo replace IAwait (pull) completely by ISubDispose (push) 
    public interface IAwait
    {
        bool IsCompleted { get; }
    }

    public interface IAwait<T> : IAwait, IOption<T>
    {
    }
}