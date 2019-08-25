// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Reactors.Utility;
using Debug = UnityEngine.Debug;

namespace Utility
{
    public static class Dbg
    {
        [Conditional(FLAGS.DEBUG)]
        public static void LogLine()
        {
            var fr = new StackTrace(1, true).GetFrame(0);
            var filename = Path.GetFileNameWithoutExtension(fr.GetFileName());

            Debug.Log($"{filename}.{fr.GetMethod().Name} : {fr.GetFileLineNumber()}");
        }
        
        [Conditional(FLAGS.DEBUG)]
        public static void LogLine<T>(Option<T> opt = default)
        {
            var fr = new StackTrace(1, true).GetFrame(0);
            var filename = Path.GetFileNameWithoutExtension(fr.GetFileName());

            var pre = opt.TryGet(out var val) ? $"{val.ToString()}: " : string.Empty;
            Debug.Log($"{pre}{filename}.{fr.GetMethod().Name} : {fr.GetFileLineNumber()}");
        }

        [Conditional(FLAGS.DEBUG)]
        public static void ErrorOnTrue(bool expr, string msg = null)
        {
            if (!expr) return;

            LogError(msg);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void ErrorOnFalse(bool expr, string msg = null)
        {
            if (expr) return;

            LogError(msg);
        }


        [Conditional(FLAGS.DEBUG)]
        public static void Log(object s)
        {
            Debug.Log(s);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Log(string s)
        {
            Debug.Log(s);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void LogException(Exception eException)
        {
            Debug.LogException(eException);
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough]
        public static void LogError(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Debug.LogError("Assert Failed");
                return;
            }

            Debug.LogError(s);
        }

        public static void LogFull()
        {
        }
    }
}