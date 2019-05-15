namespace Lib.DataFlow
{
    public interface IPubIterator
    {
        bool Next();
    }

    public interface IPubIterator<in T>
    {
        bool Next(T msg);
    }
}