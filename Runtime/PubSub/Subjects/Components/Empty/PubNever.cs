namespace Lib.DataFlow
{
    internal class PubNever : IPub
    {
        public static PubNever Ever { get; } = new PubNever();

        public bool Next()
        {
            return false;
        }
    }
    internal class PubNever<T> : IPub<T>
    {
        public static PubNever<T> Ever { get; } = new PubNever<T>();


        public bool Next(T msg)
        {
            return false;
        }
    }
}