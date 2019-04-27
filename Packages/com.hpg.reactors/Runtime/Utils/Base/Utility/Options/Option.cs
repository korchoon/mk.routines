using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using Utility.AssertN;


namespace Lib.Utility
{
    [Serializable,]
    public struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>, IOption, IOption<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly bool IsValueType;

        [OdinSerialize, SerializeField]
        public bool HasValue { get; private set; }

        [OdinSerialize,  SerializeField]
        T Value { get; set; }

        public static implicit operator Option<T>(T arg)
        {
            if (!IsValueType) return ReferenceEquals(arg, null) ? new Option<T>() : arg.Some();

#if M_WARN
            if (arg.Equals(default(T)))
            {
                Warn.Warning($"{arg} has default value");
            }
#endif
            
            return arg.Some();
        }

        static Option()
        {
            IsValueType = typeof(T).IsValueType;
        }

        public void GetOrFail(out T value)
        {
            if (!TryGet(out value))
                Asr.Fail($"Option<{typeof(T).Name}> has no value");
        }

        public bool TryGet(out T value)
        {
            if (!HasValue)
            {
                value = default(T);
                return false;
            }

            value = Value;
            return true;
        }

        internal Option(T value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        public T ValueOr(T alternative)
        {
            return HasValue ? Value : alternative;
        }

        TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return HasValue ? some(Value) : none();
        }

        public Option<TResult> Map<TResult>(Func<T, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                v => mapping(v).Some(),
                Option.None<TResult>
            );
        }

        public override string ToString()
        {
            if (!HasValue) return "None";

            return Value == null ? "Some(null)" : $"Some({Value})";
        }

        #region eq comparers boilerplate

        public bool Equals(Option<T> other)
        {
            if (!HasValue && !other.HasValue)
                return true;

            if (HasValue && other.HasValue)
                return EqualityComparer<T>.Default.Equals(Value, other.Value);

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T> && Equals((Option<T>) obj);
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            if (!HasValue) return 0;

            return Value.IsNotNull() ? Value.GetHashCode() : 1;
        }

        public int CompareTo(Option<T> other)
        {
            if (HasValue && !other.HasValue) return 1;
            if (!HasValue && other.HasValue) return -1;

            return Comparer<T>.Default.Compare(Value, other.Value);
        }

        public static bool operator <(Option<T> left, Option<T> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Option<T> left, Option<T> right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Option<T> left, Option<T> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Option<T> left, Option<T> right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion
    }
}