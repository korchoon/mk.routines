using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.Utility
{
    public static class Option
    {
        public static Option<T> Unwrap<T>(this Option<Option<T>> opt) => opt.TryGet(out var res1) ? res1 : default;

        public static bool UnwrapTryGet<T>(this Option<Option<T>> opt, out T value)
        {
            if (!opt.TryGet(out var res1))
            {
                value = default;
                return false;
            }

            if (!res1.TryGet(out var res2))
            {
                value = default;
                return false;
            }

            value = res2;
            return true;
        }

        public static Option<T> Some<T>(this T value) => new Option<T>(value, true);
        public static Option<T> None<T>() => new Option<T>(default, false);

        public static Option<T, TException> Some<T, TException>(T value) => new Option<T, TException>(value, default, true);
        public static Option<T, TException> None<T, TException>(TException exception) => new Option<T, TException>(default, exception, false);

        public static bool LastMaybe<T>(this IEnumerable<T> i, out T value)
        {
            foreach (var variable in i.Reverse())
            {
                value = variable;
                return true;
            }

            value = default;
            return false;
        }

        public static Option<T> FirstMaybe<T>(this IEnumerable<T> i)
        {
            foreach (var variable in i)
                return variable;

            return new Option<T>();
        }


        static bool IsNull<T>(this T t)
        {
            return !Option<T>.IsValueType && t == null;
        }

        public static IEnumerable<TSource> SelectValues<TSource>(this IEnumerable<Option<TSource>> source)
        {
            foreach (var maybe in source)
            {
                TSource t;
                if (!maybe.TryGet(out t))
                    continue;

                yield return t;
            }
        }

        public static IEnumerable<TResult> SelectValues<TSource, TResult>(this IEnumerable<Option<TSource>> source, Func<TSource, TResult> selector)
        {
            foreach (var maybe in source)
            {
                TSource t;
                if (!maybe.TryGet(out t))
                    continue;

                yield return selector.Invoke(t);
            }
        }

        public static bool TryGetFirst<T>(this IEnumerable<T> i, out T value)
        {
            foreach (var variable in i)
            {
                value = variable;
                return true;
            }

            value = default(T);
            return false;
        }


        public static bool IsNotNull<T>(this T t)
        {
            return Option<T>.IsValueType || t != null;
        }
    }
}