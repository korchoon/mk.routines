// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reactors.DataFlow
{
    public static class TraceUtility
    {
        public static string SourceLine(this StackFrame fr) => SourceLine(fr.GetFileName(), fr.GetFileLineNumber() - 1);

        public static string SourceLine(string path, int line)
        {
            if (!File.Exists(path))
                return String.Empty;

            return File.ReadAllLines(path)[line];
        }

        public static bool NullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        public static IEnumerable<StackFrame> GetFrames(this StackTrace tr, int skip)
        {
            var fs = tr.GetFrames();
            for (var index = skip; index < fs.Length; index++)
                yield return fs[index];
        }

        public static string Print(this IEnumerable<StackFrame> frames)
        {
            try
            {
                return frames.Select(f => f.AsString()).Aggregate((a, b) => $"{a} > {b}");
            }
            catch
            {
                return string.Empty;
            }
        }


        public static IEnumerable<StackFrame> SkipMoveNext(this IEnumerable<StackFrame> trace)
        {
            return trace.Where(fr => !(fr.GetMethod().ToString()?.Contains(".MoveNext ()", StringComparison.OrdinalIgnoreCase) ?? true));
        }


        public static IEnumerable<StackFrame> SkipInner(this IEnumerable<StackFrame> trace)
        {
            return trace.Where(fr => !(fr.GetFileName()?.Contains("packages", StringComparison.OrdinalIgnoreCase) ?? true));
        }

        static bool Contains(this string source, string toCheck, StringComparison comparisonType) => source.IndexOf(toCheck, comparisonType) >= 0;

        public static IEnumerable<StackFrame> SkipWhilePath(this StackTrace trace, string path)
        {
            foreach (var fr in trace?.GetFrames())
            {
                if (fr.GetFileName()?.Contains(path, StringComparison.OrdinalIgnoreCase) ?? true)
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
            $"{Path.GetFileNameWithoutExtension(fr.GetFileName())}.{fr.GetMethod().Name} : {fr.GetFileLineNumber()}";
    }
}