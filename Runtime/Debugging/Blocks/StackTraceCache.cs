using System.Collections.Generic;
using System.Diagnostics;

namespace Lib.DataFlow
{
    public class StackTraceCache
    {
        Dictionary<int, StackTrace> _withoutFileInfo;
        Dictionary<int, StackTrace> _withFileInfo;

        public StackTraceCache()
        {
            _withoutFileInfo = new Dictionary<int, StackTrace>();
            _withFileInfo = new Dictionary<int, StackTrace>();
        }

        public bool TryGetValue(int id, out StackTrace res) => _withFileInfo.TryGetValue(id, out res);

        public int Store(int skip)
        {
            var skipFrames = skip + 1;
            // reduce GC pressure (without fileInfo)
            var without = new StackTrace(skipFrames, fNeedFileInfo: false);
            var hashWithout = without.GetHashCode();

            if (!_withoutFileInfo.ContainsKey(hashWithout))
            {
                var with = new StackTrace(skipFrames, fNeedFileInfo: true);
                _withoutFileInfo.Add(hashWithout, without);
                _withFileInfo.Add(hashWithout, with);
            }

            return hashWithout;
        }
    }
}