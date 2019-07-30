// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

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
            return $"{line}\t[{file}]";
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


        string GetName()
        {
            return Frames(f =>
            {
                var firstOrDefault = f.SkipInner().SkipMoveNext().FirstOrDefault();
                if (firstOrDefault == null)
                    return String.Empty;
                
                return Line2(firstOrDefault);
            });
        }

        [Button(ButtonSizes.Small), HorizontalGroup("1")]
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