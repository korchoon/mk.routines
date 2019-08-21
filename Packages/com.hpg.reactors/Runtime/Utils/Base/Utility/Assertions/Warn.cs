// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utility.Asserts
{
    [DebuggerStepThrough]
    public static class Warn
    {
        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void InRange<T>(int index, T[] arr)
        {
            if (index >= 0 && index <= arr.Length - 1) return;

            Warning($"{index} throws IndexOutOfRange");
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void IsNotNull<T>(T obj, string msg = null) where T : class
        {
            if (obj != null) return;

            throw new AssertException(msg);
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void IsTrue(bool expr, string msg = null)
        {
            if (expr) return;

            Warning(msg, msg);
        }

        class AssertException : Exception
        {
            public AssertException(string msg, string userMsg = null) : base(msg) //todo userMsg
            { 
            }
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void IsNull(object target, string msg = null)
        {
            if (target == null) return;

            Warning(msg);
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void IsFalse(bool expr, string msg = null)
        {
            if (!expr) return;

            Warning(msg);
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void AreEqual<T>(T expected, T actual)
        {
            AreEqual(expected, actual, (string) null);
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void AreEqual<T>(T expected, T actual, string msg)
        {
            AreEqual(expected, actual, msg, EqualityComparer<T>.Default);
        }


        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void Warning(string message, string userMessage = null)
        {
            if (message == null)
                message = "Assertion has failed\n";
            if (userMessage != null)
                message = userMessage + '\n' + message;
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void AreEqual<T>(T expected, T actual, string msg, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
                AreEqual((object) expected as UnityEngine.Object, (object) actual as UnityEngine.Object, msg);
            else
            {
                if (comparer.Equals(actual, expected))
                    return;

                Warning($"{expected} != {actual}");
            }
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void AreEqual(UnityEngine.Object expected, UnityEngine.Object actual, string msg)
        {
            if (!(actual != expected))
                return;

            Warning($"{expected} != {actual}");
        }

        [Conditional(FLAGS.DEBUG)]
        [DebuggerStepThrough, DebuggerHidden, DebuggerNonUserCode]
        public static void AreEqual(Type getType, Type type)
        {
            if (getType == type)
                return;

            Warning($"{type.Name} != {type.Name}");
        }
    }
}