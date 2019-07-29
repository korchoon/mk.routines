// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Lib.DataFlow
{
    public class Cache<T>
    {
        public Dictionary<int, T> All;

        public Cache()
        {
            All = new Dictionary<int, T>();
        }
        
        public T Get(int id)
        {
            All.TryGetValue(id, out var res);
            return res;
        }

        public int Store(T t)
        {
            var hash = t.GetHashCode();
            if (!All.TryGetValue(hash, out _))
                All.Add(hash, t);

            return hash;
        }
    }
}