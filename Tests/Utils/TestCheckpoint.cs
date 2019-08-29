// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;


namespace Reactors.UnitTests
{
    public class TestCheckpoint<TEnum>
    {
        HashSet<TEnum> _set;
        Dictionary<TEnum, int> _counter;
        HashSet<TEnum> _all;

        public TestCheckpoint()
        {
            _set = new HashSet<TEnum>();
            _counter = new Dictionary<TEnum, int>();

            _all = new HashSet<TEnum>();
            foreach (TEnum e in Enum.GetValues(typeof(TEnum))) _all.Add(e);
        }

        public void DoneOnceStrictly(TEnum val)
        {
            _set.Add(val);
        }

        public void Visit(TEnum val)
        {
            if (!_counter.ContainsKey(val))
                _counter.Add(val, 0);

            ++_counter[val];

            if (_set.Contains(val))
                return;

            _set.Add(val);
        }

        public int Count(TEnum key)
        {
            _counter.TryGetValue(key, out var count2);
            return count2;
        }

        public bool Count(TEnum key, int count) => _counter.TryGetValue(key, out var count2) && count == count2;

        public void AssertVisited(params TEnum[] target) => Assert.IsTrue(_set.SetEquals(target), $"{_set} != {target}");

        public bool VisitedAllExcept(out string err, params TEnum[] except)
        {
            if (except.Any(_set.Contains))
            {
                err = Aggregate(except.Where(_set.Contains));
                return false;
            }

            var copy = new HashSet<TEnum>(_all);
            copy.RemoveWhere(except.Contains);

            Assert.IsTrue(copy.Any());

            copy.RemoveWhere(_set.Contains);

            err = Aggregate(copy);
            return !copy.Any();

            string Aggregate(IEnumerable<TEnum> hashSet)
            {
                try
                {
                    return hashSet.Select(e => e.ToString()).Aggregate((a, b) => $"{a}, {b}");
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}