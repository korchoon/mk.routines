namespace Lib.DataFlow
{
    public interface IPub
    {
        void Next();
    }

    public interface IPub<in T>
    {
        void Next(T msg);
    }
}