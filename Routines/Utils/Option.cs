using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public static class Option {
    public static Option<T> FirstMaybe<T> (this IEnumerable<T> t, Func<T, bool> predicate) {
        foreach (var t1 in t.Where (predicate)) {
            return t1;
        }

        return Option<T>.None;
    }

    public static Option<T> Some<T> (T value) => new Option<T> (value, true);

    public static Option<T> None<T> () => new Option<T> (default, false);

    public static bool IsNotNull<T> (T t) where T : class {
        return Option<T>.IsValueType || t != null;
    }
}


[Serializable]
public readonly struct Option<T> {
    public static readonly Option<T> None = new Option<T> ();

    // ReSharper disable once StaticMemberInGenericType
    internal static readonly bool IsValueType;

    [SerializeReference] public readonly bool HasValue;
    [SerializeReference] readonly T _value;

    static Option () {
        IsValueType = typeof (T).IsValueType;
    }


    public Option (T value) {
        if (typeof (T).IsValueType) {
            this.HasValue = true;
            this._value = value;
        }
        // ReSharper disable once CompareNonConstrainedGenericWithNull
        else if (value != null) {
            this.HasValue = true;
            this._value = value;
        }
        else {
            this.HasValue = false;
            this._value = default;
        }
    }

    internal Option (T value, bool hasValue) {
        _value = value;
        HasValue = hasValue;
    }

    public void GetOrFail (out T value) {
        if (!TryGet (out value))
            Fail ($"Option<{typeof (T).Name}> has no value");
    }

    [MustUseReturnValue]
    public T GetOrFail () {
        if (!TryGet (out var value))
            Fail ($"Option<{typeof (T).Name}> has no value");
        return value;
    }

    public T GetOrElseLazy<TContext> (Func<TContext, T> defaultCtor, TContext context) {
        if (this.HasValue) {
            return this._value;
        }
        else {
            return defaultCtor.Invoke (context);
        }
    }

    [Conditional ("DEBUG1")]
    static void Fail (string format = null) {
        throw new Exception (format);
    }

    public Option<TResult> Map<TResult> (Func<T, TResult> f) {
        if (this.HasValue) {
            var t = f.Invoke (this._value);
            return new Option<TResult> (t);
        }
        else {
            return Option.None<TResult> ();
        }
    }

    public void MatchSome (Action<T> f) {
        if (this.HasValue) {
            f.Invoke (this._value);
        }
    }

    /// Transform this Option using the mapping function <paramref name="f"/> with a context object.
    /// Use the context object to avoid closure allocations.
    [Pure]
    public Option<TResult> Map<TResult, TContext> (Func<T, TContext, TResult> f, TContext context) {
        return this.HasValue ? new Option<TResult> (f (this._value, context)) : Option<TResult>.None;
    }

    public bool TryGet (out T value) {
        if (!HasValue) {
            value = default (T);
            return false;
        }

        value = _value;
        return true;
    }

    public Option<T> ValueOr (Option<T> alternative) {
        return HasValue ? _value : alternative;
    }

    public T ValueOr (T alternative) {
        return HasValue ? _value : alternative;
    }

    // for debug purposes
    public override string ToString () {
        if (!HasValue) return "None";

        return _value == null ? "Some(null)" : $"Some({_value})";
    }

    public Enumerator GetEnumerator () => new Enumerator (this);

    public struct Enumerator : IEnumerator<T> {
        Option<T> _value;
        int _counter;

        public Enumerator (Option<T> option) {
            _value = option;
            _counter = 0;
            Current = default;
        }

        public bool MoveNext () {
            if (!_value.TryGet (out var value)) return false;

            if (_counter > 0) return false;
            _counter += 1;
            Current = value;
            return true;
        }


        public T Current { get; private set; }

        public void Dispose () {
            Current = default;
            _value = default;
        }

        public void Reset () => throw new NotImplementedException ();
        object IEnumerator.Current => throw new NotImplementedException ();
    }

    public static implicit operator Option<T> (T arg) {
        if (!IsValueType) return ReferenceEquals (arg, null) ? new Option<T> () : Option.Some (arg);
#if M_WARN
          if (arg.Equals(default(T))) 
              Warn.Warning($"{arg} has default value");
#endif
        return Option.Some (arg);
    }

    #region eq comparers boilerplate

    public bool Equals (Option<T> other) {
        if (!HasValue && !other.HasValue)
            return true;

        if (HasValue && other.HasValue)
            return EqualityComparer<T>.Default.Equals (_value, other._value);

        return false;
    }

    public override bool Equals (object obj) {
        return obj is Option<T> && Equals ((Option<T>)obj);
    }

    public static bool operator == (Option<T> left, Option<T> right) {
        return left.Equals (right);
    }

    public static bool operator != (Option<T> left, Option<T> right) {
        return !left.Equals (right);
    }

    public override int GetHashCode () {
        if (!HasValue) return 0;

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        // ReSharper disable once CompareNonConstrainedGenericWithNull
        if (IsValueType || _value != null) {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _value.GetHashCode ();
        }
        else {
            return 1;
        }
    }

    #endregion
}