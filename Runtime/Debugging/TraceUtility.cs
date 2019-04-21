using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Sirenix.Utilities;

namespace Lib.DataFlow
{
    public static class TraceUtility
    {
        public static IEnumerable<StackFrame> SkipWhilePath(this StackTrace trace, string path)
        {
            foreach (var fr in trace?.GetFrames())
            {
                if (fr.GetFileName()?.Contains(path) ?? true)
                    continue;

                yield return fr;
            }
        }

        public static string GetFrame(int skip)
        {
            var tr = new StackTrace(skip, true);
            var fr = tr.GetFrame(0);
            return AsString(fr: fr);
        }

        public static string AsString(this StackFrame fr) =>
            $"{Path.GetFileNameWithoutExtension(fr.GetFileName())}. {fr.GetMethod().GetNiceName()} : {fr.GetFileLineNumber()}";
    }
}