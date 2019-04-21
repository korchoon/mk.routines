using System;

namespace Lib.Pooling
{
    [Serializable]
    public class DebugPoolCounter
    {
        int _total;
        int _inactive;

        public void Release()
        {
            ++_inactive;
        }

        public void Get()
        {
            --_inactive;
        }

        public void New()
        {
            ++_total;
        }

        public string _Inactive => $"{_inactive}/{_total}";
    }
}