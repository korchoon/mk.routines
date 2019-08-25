// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Utility.Asserts;

namespace Reactors
{
    public static class ScriptingDefineUtil
    {
        public static string Current()
        {
            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget);
        }
        
        public static bool IsNullOrWhitespace(this string s) => string.IsNullOrWhiteSpace(s);
        
        static StringComparer _comparer = StringComparer.Ordinal;

        public static bool TryAddFlag(string flag, string symbols, out string result)
        {
            Asr.IsTrue(Valid(symbols));
            var sp = Split(symbols);
            if (!sp.Add(flag))
            {
                result = symbols;
                return false;
            }

            symbols = Concat(sp);
            Asr.IsTrue(Valid(symbols));
            result = symbols;
            return true;
        }

        public static bool TryRemoveFlag(string flag, string symbols, out string result)
        {
            Asr.IsTrue(Valid(symbols));
            var sp = Split(symbols);
            if (!sp.Remove(flag))
            {
                result = flag;
                return false;
            }

            symbols = Concat(sp);

            Asr.IsTrue(Valid(symbols));
            result = symbols;
            return true;
        }

        public static HashSet<string> Split(string symbols)
        {
            Asr.IsTrue(Valid(symbols));

            var collection = symbols.Split(';')
                .Where(s => !s.IsNullOrWhitespace())
                .Distinct();

            return new HashSet<string>(collection, _comparer);
        }

        public static string Concat(IEnumerable<string> flags)
        {
            var res = flags.Aggregate((a, b) => $"{a};{b}");
            Asr.IsTrue(Valid(res));
            return res;
        }

        static string Cleanup(string s)
        {
            s = s.Replace(";;", ";");
            s = s.Trim();
            return s;
        }

        static bool Invalid(string s)
        {
            return s.Contains(";;");
        }

        static bool Valid(string s) => !Invalid(s);
    }
}
#endif