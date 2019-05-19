namespace Lib.Utility
{
    public interface IOption
    {
        bool HasValue { get; }
    }
    public interface IOption<T>
    {
        bool TryGet(out T value);
    }
}