using System;
using System.Collections.Generic;

namespace Lib.Utility
{
    public struct Option<T, TException> : IEquatable<Option<T, TException>>, IComparable<Option<T, TException>>
    {
        readonly bool _hasValue;

        readonly T _value;

        TException Exception { get; }

        internal Option(T value, TException exception, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
            Exception = exception;
        }

        public bool Equals(Option<T, TException> other)
        {
            if (!_hasValue && !other._hasValue)
                return EqualityComparer<TException>.Default.Equals(Exception, other.Exception);

            if (_hasValue && other._hasValue)
                return EqualityComparer<T>.Default.Equals(_value, other._value);

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T, TException> a && Equals(a);
        }

        public static bool operator ==(Option<T, TException> left, Option<T, TException> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T, TException> left, Option<T, TException> right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            if (_hasValue)
                return _value == null ? 1 : _value.GetHashCode();

            return Exception == null ? 0 : Exception.GetHashCode();
        }


        public int CompareTo(Option<T, TException> other)
        {
            if (_hasValue && !other._hasValue) return 1;
            if (!_hasValue && other._hasValue) return -1;

            return _hasValue
                ? Comparer<T>.Default.Compare(_value, other._value)
                : Comparer<TException>.Default.Compare(Exception, other.Exception);
        }

        public static bool operator <(Option<T, TException> left, Option<T, TException> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Option<T, TException> left, Option<T, TException> right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Option<T, TException> left, Option<T, TException> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Option<T, TException> left, Option<T, TException> right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            if (_hasValue)
                return _value == null ? "Some(null)" : $"Some({_value})";

            return Exception == null ? "None(null)" : $"None({Exception})";
        }


        public bool Contains(T value)
        {
            if (!_hasValue) return false;
            if (_value == null)
                return value == null;

            return _value.Equals(value);
        }

        public bool Exists(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return _hasValue && predicate(_value);
        }

        public T ValueOr(T alternative)
        {
            return _hasValue ? _value : alternative;
        }

        public T ValueOr(Func<T> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return _hasValue ? _value : alternativeFactory();
        }

        public T ValueOr(Func<TException, T> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return _hasValue ? _value : alternativeFactory(Exception);
        }

        public Option<T, TException> Or(T alternative)
        {
            return _hasValue ? this : Option.Some<T, TException>(alternative);
        }

        public Option<T, TException> Or(Func<T> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return _hasValue ? this : Option.Some<T, TException>(alternativeFactory());
        }

        public Option<T, TException> Or(Func<TException, T> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return _hasValue ? this : Option.Some<T, TException>(alternativeFactory(Exception));
        }

        public Option<T, TException> Else(Option<T, TException> alternativeMaybe)
        {
            return _hasValue ? this : alternativeMaybe;
        }

        public Option<T, TException> Else(Func<Option<T, TException>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            return _hasValue ? this : alternativeOptionFactory();
        }

        public Option<T, TException> Else(Func<TException, Option<T, TException>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            return _hasValue ? this : alternativeOptionFactory(Exception);
        }

        public Option<T> WithoutException()
        {
            return Match(
                some: Option.Some,
                none: _ => Option.None<T>()
            );
        }

        public bool TryGet(out T value)
        {
            if (_hasValue)
            {
                value = _value;
                return true;
            }

            value = default(T);
            return false;
        }

        public bool TryGetError(out TException ex)
        {
            if (Exception != null)
            {
                ex = Exception;
                return true;
            }

            ex = default(TException);
            return false;
        }

        public TResult Match<TResult>(Func<T, TResult> some, Func<TException, TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return _hasValue ? some(_value) : none(Exception);
        }

        public void Match(Action<T> some, Action<TException> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (_hasValue)
            {
                some(_value);
            }
            else
            {
                none(Exception);
            }
        }

        public void MatchSome(Action<T> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));

            if (_hasValue)
            {
                some(_value);
            }
        }

        public void MatchNone(Action<TException> none)
        {
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (!_hasValue)
            {
                none(Exception);
            }
        }

        public Option<TResult, TException> Map<TResult>(Func<T, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                some: value => Option.Some<TResult, TException>(mapping(value)),
                none: exception => Option.None<TResult, TException>(exception)
            );
        }

        public Option<T, TExceptionResult> MapException<TExceptionResult>(Func<TException, TExceptionResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                some: Option.Some<T, TExceptionResult>,
                none: exception => Option.None<T, TExceptionResult>(mapping(exception))
            );
        }

        public Option<TResult, TException> FlatMap<TResult>(Func<T, Option<TResult, TException>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                some: mapping,
                none: exception => Option.None<TResult, TException>(exception)
            );
        }

        public Option<T, TException> Filter(bool condition, TException exception)
        {
            return _hasValue && !condition ? Option.None<T, TException>(exception) : this;
        }

        public Option<T, TException> Filter(bool condition, Func<TException> exceptionFactory)
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));

            return _hasValue && !condition ? Option.None<T, TException>(exceptionFactory()) : this;
        }

        public Option<T, TException> Filter(Func<T, bool> predicate, TException exception)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return _hasValue && !predicate(_value) ? Option.None<T, TException>(exception) : this;
        }

        public Option<T, TException> Filter(Func<T, bool> predicate, Func<TException> exceptionFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));

            return _hasValue && !predicate(_value) ? Option.None<T, TException>(exceptionFactory()) : this;
        }

        public Option<T, TException> NotNull(TException exception)
        {
            return _hasValue && _value == null ? Option.None<T, TException>(exception) : this;
        }

        public Option<T, TException> NotNull(Func<TException> exceptionFactory)
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));

            return _hasValue && _value == null ? Option.None<T, TException>(exceptionFactory()) : this;
        }
    }
}