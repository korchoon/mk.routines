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
        static Cache<StackTrace> _cache = new Cache<StackTrace>();

        public static StackTraceHolder New(int skip) => new StackTraceHolder(skip);

        public string FrameFileInfo()
        {
            var stackFrame = _cache.Get(_id).GetFrame(0);
            var line = stackFrame.SourceLine().Trim();
            var file = stackFrame.AsString();
            return $"{line}\t\t[{file}]";
        }

        StackTraceHolder(int skip = 0)
        {
            var trace = new StackTrace(skip + 1, true);
            _id = _cache.Store(trace);
            Name = Frames(f => f.First().AsString());
        }

        string Frames(Func<IEnumerable<StackFrame>, string> selector)
        {
            if (!_cache.All.TryGetValue(_id, out var res))
                return string.Empty;

            if (res.FrameCount == 0)
                return string.Empty;

            return selector.Invoke(res.GetFrames());
        }

        int _id;

        [Button, HorizontalGroup("1")]
        void Breadcrumbs() => Multiline = Frames(f => f.Reverse().Print());

        [Button, HorizontalGroup("1")]
        void Full() => Multiline = _cache.Get(_id)?.ToString();

        [Button, HorizontalGroup("1")]
        void Top() => Line = Frames(f => f.SkipInner().Print());

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