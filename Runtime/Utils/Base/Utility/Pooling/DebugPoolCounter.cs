using System;
using Sirenix.OdinInspector;

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

        public void Pop()
        {
            --_inactive;
        }

        public void Create()
        {
            ++_total;
        }

        [ShowInInspector, HideLabel]
        public string _Inactive => $"{_inactive}/{_total}";
    }
}