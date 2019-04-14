using Lib.Utility;
using Sirenix.OdinInspector;

namespace Lib.DataFlow
{
    // todo replace IAwait (pull) completely by ISubDispose (push) 
    public interface IAwait
    {
        [ShowInInspector] bool IsCompleted { get; }
    }

    public interface IAwait<T> : IAwait, IOption<T>
    {
    }
}