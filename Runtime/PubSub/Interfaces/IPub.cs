namespace Lib.DataFlow
{
    public interface IPub
    {
        bool Next();
    }

    public interface IPub<in T>
    {
        bool Next(T msg);
    }
}