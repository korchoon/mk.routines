// ----------------------------------------------------------------------------
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Mk.Debugs {
    static class Asr2 {
#line hidden
        [Conditional(FLAGS.DEBUG)]
        public static void IsNotNull<T>(T obj, string msg = null) where T : class {
            if (obj != null) return;
            throw new Err(msg);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void IsTrue(bool expr, string msg = null) {
            if (expr) return;
            Fail(msg);
        }


        [Conditional(FLAGS.DEBUG)]
        public static void IsFalse(bool expr, string msg = null) {
            if (!expr) return;
            Fail(msg);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Fail(string message = default) {
            throw new Err(message);
        }

        class Err : Exception {
            public Err(string msg) : base(msg) { }
        }
#line default
    }

    static class FLAGS {
        public const string DEBUG = "DEBUG";
        public const string UNITY_EDITOR = "UNITY_EDITOR";
        public const string UNITY_INCLUDE_TESTS = "UNITY_INCLUDE_TESTS";
    }
}