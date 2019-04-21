using System;

namespace Lib.DataFlow
{
    public class EmptyException : Exception
    {
        EmptyException()
        {
        }
        
        public static EmptyException Empty { get; } = new EmptyException();
    }
}