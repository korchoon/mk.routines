using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lib.DataFlow
{
    [HideReferenceObjectPicker, InlineProperty]
    public class StackTraceHolder
    {
        #region static

        static StackTraceCache _cache;

        static StackTraceHolder()
        {
            _cache = new StackTraceCache();
        }

        public static StackTraceHolder New(int skip) => new StackTraceHolder(skip);

        #endregion
        
        int _id;

        public string FrameFileInfo()
        {
            if (!_cache.TryGetValue(_id, out var trace))
                return String.Empty;

            var stackFrame = trace.GetFrame(0);
            return Line2(stackFrame: stackFrame);
        }

        static string Line2(StackFrame stackFrame)
        {
            var line = stackFrame.SourceLine().Trim();
            var file = stackFrame.AsString();
//            return $"{line}";
            return $"{line} [{stackFrame.GetFileLineNumber()}]";
        }

        StackTraceHolder(int skip = 0)
        {
            _id = _cache.Store(skip + 1);
            Name = GetName().Trim();
        }

        string Frames(Func<IEnumerable<StackFrame>, string> selector)
        {
            if (!_cache.TryGetValue(_id, out var res))
                return string.Empty;

            if (res == null)
                return String.Empty;
            
            if (res.FrameCount == 0)
                return string.Empty;

            
            var stackFrames = res.GetFrames();
            return selector.Invoke(stackFrames);
        }


        public string GetName(bool skipInner = true)
        {
            return Frames(f =>
            {
                if (skipInner)
                    f = f.SkipInner();
                
                var firstOrDefault = f
                    .SkipMoveNext()
                    .FirstOrDefault();
                
                if (firstOrDefault == null)
                    return String.Empty;
                
                return Line2(firstOrDefault);
            });
        }

        [Button(), HorizontalGroup("1")]
        void StackTrace()
        {
            if (!_cache.TryGetValue(_id, out var trace))
                return;

            Multiline = trace?.ToString();
        }

        bool _LineEmpty() => Line.NullOrEmpty();
        bool _MultilineEmpty() => Multiline.NullOrEmpty();

        [HorizontalGroup("1"), PropertyOrder(-1), HideLabel, ReadOnly]
        public string Name;

        [HideIf(nameof(_LineEmpty)), HideLabel]
        public string Line;

        [HideIf(nameof(_MultilineEmpty)), Multiline(3), HideLabel]
        public string Multiline;

    }
}